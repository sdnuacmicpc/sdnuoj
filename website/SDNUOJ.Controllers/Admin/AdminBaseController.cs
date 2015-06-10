using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using SDNUOJ.Controllers;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Utilities;

namespace SDNUOJ.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminBaseController : BaseController
    {
        #region 常量
        private static readonly Dictionary<String, String> ContentTypeTable = new Dictionary<String, String>()
        {
            { "txt", "text/plain" },
            { "xls", "application/vnd.ms-excel" },
            { "mdb", "application/msaccess" },
            { "resx", "application/msaccess" },
            { "zip", "application/zip" }
        };
        #endregion

        #region 属性
        /// <summary>
        /// 获取是否永远可以访问
        /// </summary>
        protected override Boolean IsAlwaysOpen { get { return true; } }

        /// <summary>
        /// 获取是否输出当前竞赛数量
        /// </summary>
        protected override Boolean OutputCurrentContestCount { get { return false; } }
        #endregion

        #region 方法
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            if (filterContext.Exception != null)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = RedirectToErrorMessagePage(filterContext.Exception.Message);
            }
        }

        /// <summary>
        /// 返回带有导航的页面
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="list">内容列表</param>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>操作后的结果</returns>
        protected override ActionResult ViewWithPager<T>(PagedList<T> list, Int32 pageIndex)
        {
            ViewBag.PageSize = list.PageSize;
            ViewBag.RecordCount = list.RecordCount;
            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = pageIndex;

            return View(list);
        }

        /// <summary>
        /// 返回带有导航的页面
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="viewName">视图名称</param>
        /// <param name="list">内容列表</param>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>操作后的结果</returns>
        protected override ActionResult ViewWithPager<T>(String viewName, PagedList<T> list, Int32 pageIndex)
        {
            ViewBag.PageSize = list.PageSize;
            ViewBag.RecordCount = list.RecordCount;
            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = pageIndex;

            return View(viewName, list);
        }

        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="msg">信息内容</param>
        /// <param name="backurl">返回转向页面</param>
        /// <param name="type">信息类型</param>
        protected override ActionResult RedirectToMessagePage(String msg, String backurl, MessageType type)
        {
            return RedirectToAction("Index", "Info", new
            {
                area = "Admin",
                c = HttpUtility.UrlEncode(msg),
                s = (type != MessageType.Generic ? type.ToString().ToLowerInvariant() : ""),
                b = backurl
            });
        }
        #endregion

        #region ResultToMessagePage
        /// <summary>
        /// 指定方法返回信息页面
        /// </summary>
        /// <param name="func">操作方法</param>
        /// <param name="successInfo">成功后提示的内容</param>
        /// <returns>操作后的结果</returns>
        private ActionResult ResultToMessagePage(Func<IMethodResult> func, String successInfo)
        {
            IMethodResult result = null;

            try
            {
                result = func();

                if (result.IsSuccess)
                {
                    return RedirectToSuccessMessagePage(successInfo);
                }
                else
                {
                    return RedirectToErrorMessagePage(result.Description);
                }
            }
            finally
            {
                if (result != null)
                {
                    this.LogUserOperation(result);
                }
            }
        }

        /// <summary>
        /// 指定方法返回信息页面
        /// </summary>
        /// <param name="func">操作方法</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToMessagePage(Func<Tuple<IMethodResult, String>> func)
        {
            Tuple<IMethodResult, String> result = func();

            return ResultToMessagePage(() =>
            {
                return result.Item1;
            }, result.Item2);
        }

        /// <summary>
        /// 指定方法返回信息页面
        /// </summary>
        /// <typeparam name="T">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg">传入参数</param>
        /// <param name="successInfo">成功后提示的内容</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToMessagePage<T>(Func<T, IMethodResult> func, T arg, String successInfo)
        {
            return ResultToMessagePage(() =>
            {
                return func(arg);
            }, successInfo);
        }

        /// <summary>
        /// 指定方法返回信息页面
        /// </summary>
        /// <typeparam name="T1">传入参数类型</typeparam>
        /// <typeparam name="T2">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg1">传入参数</param>
        /// <param name="arg2">传入参数</param>
        /// <param name="successInfo">成功后提示的内容</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToMessagePage<T1, T2>(Func<T1, T2, IMethodResult> func, T1 arg1, T2 arg2, String successInfo)
        {
            return ResultToMessagePage(() =>
            {
                return func(arg1, arg2);
            }, successInfo);
        }

        /// <summary>
        /// 指定方法返回信息页面
        /// </summary>
        /// <typeparam name="T1">传入参数类型</typeparam>
        /// <typeparam name="T2">传入参数类型</typeparam>
        /// <typeparam name="T3">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg1">传入参数</param>
        /// <param name="arg2">传入参数</param>
        /// <param name="arg3">传入参数</param>
        /// <param name="successInfo">成功后提示的内容</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToMessagePage<T1, T2, T3>(Func<T1, T2, T3, IMethodResult> func, T1 arg1, T2 arg2, T3 arg3, String successInfo)
        {
            return ResultToMessagePage(() =>
            {
                return func(arg1, arg2, arg3);
            }, successInfo);
        }
        #endregion

        #region ResultToView
        /// <summary>
        /// 指定方法返回页面
        /// </summary>
        /// <param name="func">操作方法</param>
        /// <param name="viewName">返回的视图名称</param>
        /// <returns>操作后的结果</returns>
        private ActionResult ResultToView(Func<IMethodResult> func, String viewName)
        {
            IMethodResult result = null;

            try
            {
                result = func();

                if (result.IsSuccess)
                {
                    if (String.IsNullOrEmpty(viewName))
                    {
                        return View(result.ResultObject);
                    }
                    else
                    {
                        return View(viewName, result.ResultObject);
                    }
                }
                else
                {
                    return RedirectToErrorMessagePage(result.Description);
                }
            }
            finally
            {
                if (result != null)
                {
                    this.LogUserOperation(result);
                }
            }
        }

        /// <summary>
        /// 指定方法返回页面
        /// </summary>
        /// <param name="func">操作方法</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToView(Func<IMethodResult> func)
        {
            return ResultToView(() =>
            {
                return func();
            }, String.Empty);
        }

        /// <summary>
        /// 指定方法返回页面
        /// </summary>
        /// <typeparam name="T">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg">传入参数</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToView<T>(Func<T, IMethodResult> func, T arg)
        {
            return ResultToView(() =>
            {
                return func(arg);
            }, String.Empty);
        }

        /// <summary>
        /// 指定方法返回页面
        /// </summary>
        /// <typeparam name="T">传入参数类型</typeparam>
        /// <param name="viewName">返回的视图名称</param>
        /// <param name="func">操作方法</param>
        /// <param name="arg">传入参数</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToView<T>(String viewName, Func<T, IMethodResult> func, T arg)
        {
            return ResultToView(() =>
            {
                return func(arg);
            }, viewName);
        }

        /// <summary>
        /// 指定方法返回页面
        /// </summary>
        /// <typeparam name="T1">传入参数类型</typeparam>
        /// <typeparam name="T2">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg1">传入参数</param>
        /// <param name="arg2">传入参数</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToView<T1, T2>(Func<T1, T2, IMethodResult> func, T1 arg1, T2 arg2)
        {
            return ResultToView(() =>
            {
                return func(arg1, arg2);
            }, String.Empty);
        }
        #endregion

        #region ResultToAction
        /// <summary>
        /// 指定方法返回页面
        /// </summary>
        /// <param name="func">操作方法</param>
        /// <param name="actionName">操作名称</param>
        /// <param name="controllerName">控制器名称</param>
        /// <param name="routeValues">传递参数</param>
        /// <returns>操作后的结果</returns>
        private ActionResult ResultToAction(Func<IMethodResult> func, String actionName, String controllerName, Object routeValues)
        {
            IMethodResult result = null;

            try
            {
                result = func();

                if (result.IsSuccess)
                {
                    if (routeValues != null)
                    {
                        return RedirectToAction(actionName, controllerName, routeValues);
                    }
                    else
                    {
                        return RedirectToAction(actionName, controllerName);
                    }
                }
                else
                {
                    return RedirectToErrorMessagePage(result.Description);
                }
            }
            finally
            {
                if (result != null)
                {
                    this.LogUserOperation(result);
                }
            }
        }

        /// <summary>
        /// 指定方法返回页面
        /// </summary>
        /// <param name="actionName">操作名称</param>
        /// <param name="controllerName">控制器名称</param>
        /// <param name="routeValues">传递参数</param>
        /// <typeparam name="T">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg">传入参数</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToAction<T>(String actionName, String controllerName, Object routeValues, Func<T, IMethodResult> func, T arg)
        {
            return ResultToAction(() =>
            {
                return func(arg);
            }, actionName, controllerName, routeValues);
        }

        /// <summary>
        /// 指定方法返回页面
        /// </summary>
        /// <param name="actionName">操作名称</param>
        /// <param name="controllerName">控制器名称</param>
        /// <typeparam name="T1">传入参数类型</typeparam>
        /// <typeparam name="T2">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg1">传入参数</param>
        /// <param name="arg2">传入参数</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToAction<T1, T2>(String actionName, String controllerName, Func<T1, T2, IMethodResult> func, T1 arg1, T2 arg2)
        {
            return ResultToAction(() =>
            {
                return func(arg1, arg2);
            }, actionName, controllerName, null);
        }

        /// <summary>
        /// 指定方法返回页面
        /// </summary>
        /// <param name="actionName">操作名称</param>
        /// <param name="controllerName">控制器名称</param>
        /// <param name="routeValues">传递参数</param>
        /// <typeparam name="T1">传入参数类型</typeparam>
        /// <typeparam name="T2">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg1">传入参数</param>
        /// <param name="arg2">传入参数</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToAction<T1, T2>(String actionName, String controllerName, Object routeValues, Func<T1, T2, IMethodResult> func, T1 arg1, T2 arg2)
        {
            return ResultToAction(() =>
            {
                return func(arg1, arg2);
            }, actionName, controllerName, routeValues);
        }
        #endregion

        #region ResultToFile
        /// <summary>
        /// 指定方法返回下载
        /// </summary>
        /// <param name="func">操作方法</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileExtension">文件扩展名</param>
        /// <returns>操作后的结果</returns>
        private ActionResult ResultToFile(Func<IMethodResult> func, String fileName, String fileExtension)
        {
            IMethodResult result = null;

            try
            {
                result = func();

                if (result.IsSuccess)
                {
                    String contentType = "";

                    if (!ContentTypeTable.TryGetValue(fileExtension, out contentType))
                    {
                        contentType = "application/octet-stream";
                    }

                    return File(result.ResultObject as Byte[], contentType, String.Format("{0}.{1}", fileName, fileExtension));
                }
                else
                {
                    return RedirectToErrorMessagePage(result.Description);
                }
            }
            finally
            {
                if (result != null)
                {
                    this.LogUserOperation(result);
                }
            }
        }

        /// <summary>
        /// 指定方法返回下载
        /// </summary>
        /// <typeparam name="T">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg">传入参数</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileExtension">文件扩展名</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToFile<T>(Func<T, IMethodResult> func, T arg, String fileName, String fileExtension)
        {
            return ResultToFile(() =>
            {
                return func(arg);
            }, fileName, fileExtension);
        }

        /// <summary>
        /// 指定方法返回下载
        /// </summary>
        /// <typeparam name="T1">传入参数类型</typeparam>
        /// <typeparam name="T2">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg1">传入参数</param>
        /// <param name="arg2">传入参数</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileExtension">文件扩展名</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToFile<T1, T2>(Func<T1, T2, IMethodResult> func, T1 arg1, T2 arg2, String fileName, String fileExtension)
        {
            return ResultToFile(() =>
            {
                return func(arg1, arg2);
            }, fileName, fileExtension);
        }

        /// <summary>
        /// 指定方法返回下载
        /// </summary>
        /// <typeparam name="T1">传入参数类型</typeparam>
        /// <typeparam name="T2">传入参数类型</typeparam>
        /// <typeparam name="T3">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg1">传入参数</param>
        /// <param name="arg2">传入参数</param>
        /// <param name="arg3">传入参数</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileExtension">文件扩展名</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToFile<T1, T2, T3>(Func<T1, T2, T3, IMethodResult> func, T1 arg1, T2 arg2, T3 arg3, String fileName, String fileExtension)
        {
            return ResultToFile(() =>
            {
                return func(arg1, arg2, arg3);
            }, fileName, fileExtension);
        }
        #endregion

        #region ResultToFilePath
        /// <summary>
        /// 指定方法返回下载
        /// </summary>
        /// <param name="func">操作方法</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileExtension">文件扩展名</param>
        /// <returns>操作后的结果</returns>
        private ActionResult ResultToFilePath(Func<IMethodResult> func, String fileName, String fileExtension)
        {
            IMethodResult result = null;

            try
            {
                result = func();

                if (result.IsSuccess)
                {
                    if (String.IsNullOrEmpty(fileName))
                    {
                        fileName = Path.GetFileNameWithoutExtension(result.ResultObject as String);
                    }

                    if (String.IsNullOrEmpty(fileExtension))
                    {
                        fileExtension = Path.GetExtension(result.ResultObject as String);

                        if (!String.IsNullOrEmpty(fileExtension) && fileExtension[0] == '.')
                        {
                            fileExtension = fileExtension.Substring(1);
                        }
                    }

                    String contentType = "";

                    if (!ContentTypeTable.TryGetValue(fileExtension, out contentType))
                    {
                        contentType = "application/octet-stream";
                    }

                    return File(result.ResultObject as String, contentType, String.Format("{0}.{1}", fileName, fileExtension));
                }
                else
                {
                    return RedirectToErrorMessagePage(result.Description);
                }
            }
            finally
            {
                if (result != null)
                {
                    this.LogUserOperation(result);
                }
            }
        }

        /// <summary>
        /// 指定方法返回下载
        /// </summary>
        /// <typeparam name="T">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg">传入参数</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToFilePath<T>(Func<T, IMethodResult> func, T arg)
        {
            return ResultToFilePath(() =>
            {
                return func(arg);
            }, String.Empty, String.Empty);
        }

        /// <summary>
        /// 指定方法返回下载
        /// </summary>
        /// <typeparam name="T">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg">传入参数</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileExtension">文件扩展名</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToFilePath<T>(Func<T, IMethodResult> func, T arg, String fileName, String fileExtension)
        {
            return ResultToFilePath(() =>
            {
                return func(arg);
            }, fileName, fileExtension);
        }
        #endregion

        #region ResultToJson
        /// <summary>
        /// 指定方法返回Json结果
        /// </summary>
        /// <param name="func">操作方法</param>
        /// <returns>操作后的结果</returns>
        private ActionResult ResultToJson(Func<IMethodResult> func)
        {
            IMethodResult result = null;

            try
            {
                result = func();

                if (result.IsSuccess)
                {
                    return SuccessJson();
                }
                else
                {
                    return ErrorJson(result.Description);
                }
            }
            catch (System.Exception ex)
            {
                RouteData routeData = RouteData.Route.GetRouteData(this.HttpContext);
                this.LogException(ex, routeData);

                return ErrorJson(ex.Message);
            }
            finally
            {
                if (result != null)
                {
                    this.LogUserOperation(result);
                }
            }
        }

        /// <summary>
        /// 指定方法返回Json结果
        /// </summary>
        /// <typeparam name="T">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg">传入参数</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToJson<T>(Func<T, IMethodResult> func, T arg)
        {
            return ResultToJson(() =>
            {
                return func(arg);
            });
        }

        /// <summary>
        /// 指定方法返回Json结果
        /// </summary>
        /// <typeparam name="T">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg">传入参数</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToJson<T>(Func<T, String, String, String, String, String, String, String, IMethodResult> func, T arg)
        {
            return ResultToJson(() =>
            {
                return func(arg, null, null, null, null, null, null, null);
            });
        }

        /// <summary>
        /// 指定方法返回Json结果
        /// </summary>
        /// <typeparam name="T1">传入参数类型</typeparam>
        /// <typeparam name="T2">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg1">传入参数</param>
        /// <param name="arg2">传入参数</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToJson<T1, T2>(Func<T1, T2, IMethodResult> func, T1 arg1, T2 arg2)
        {
            return ResultToJson(() =>
            {
                return func(arg1, arg2);
            });
        }

        /// <summary>
        /// 指定方法返回Json结果
        /// </summary>
        /// <typeparam name="T1">传入参数类型</typeparam>
        /// <typeparam name="T2">传入参数类型</typeparam>
        /// <typeparam name="T3">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg1">传入参数</param>
        /// <param name="arg2">传入参数</param>
        /// <param name="arg3">传入参数</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToJson<T1, T2, T3>(Func<T1, T2, T3, IMethodResult> func, T1 arg1, T2 arg2, T3 arg3)
        {
            return ResultToJson(() =>
            {
                return func(arg1, arg2, arg3);
            });
        }

        /// <summary>
        /// 指定方法返回Json结果
        /// </summary>
        /// <typeparam name="T">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="func2">操作方法2</param>
        /// <param name="arg">传入参数</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToJson<T>(Func<T, IMethodResult> func, Func<T, IMethodResult> func2, T arg)
        {
            return ResultToJson(() =>
            {
                IMethodResult ret1 = func(arg);

                if (!ret1.IsSuccess)
                {
                    return ret1;
                }

                IMethodResult ret2 = func2(arg);

                if (!ret2.IsSuccess)
                {
                    return ret2;
                }

                if (!ret1.IsWriteLog && !ret2.IsWriteLog)
                {
                    return MethodResult.Success();
                }
                else if (ret1.IsWriteLog && !ret2.IsWriteLog)
                {
                    return ret1;
                }
                else if (!ret1.IsWriteLog && ret2.IsWriteLog)
                {
                    return ret2;
                }
                else
                {
                    return MethodResult.SuccessAndLog("{0}; {1}", ret1.Description, ret2.Description);
                }
            });
        }
        #endregion
    }
}