using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Logging;
using SDNUOJ.Utilities;
using SDNUOJ.Utilities.Text;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        protected enum MessageType
        {
            Generic,
            Info,
            Success,
            Warning,
            Danger
        }

        #region 字段
        private Stopwatch _stopWatch;
        #endregion

        #region 属性
        /// <summary>
        /// 获取是否永远可以访问
        /// </summary>
        protected virtual Boolean IsAlwaysOpen { get { return false; } }

        /// <summary>
        /// 获取是否输出当前竞赛数量
        /// </summary>
        protected virtual Boolean OutputCurrentContestCount { get { return true; } }
        #endregion

        #region 构造方法
        protected BaseController()
        {
            this._stopWatch = new Stopwatch();
            this._stopWatch.Start();
        }

        ~BaseController()
        {
            if (this._stopWatch.IsRunning)
            {
                this._stopWatch.Stop();
            }
        }
        #endregion

        #region 方法
        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, Object state)
        {
            this._stopWatch.Start();

            return base.BeginExecute(requestContext, callback, state);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (ConfigurationManager.SystemClosed && !this.IsAlwaysOpen)
            {
                String controller = filterContext.RouteData.Values["controller"] as String;
                String action = filterContext.RouteData.Values["action"] as String;

                if (!String.Equals(controller, "user", StringComparison.OrdinalIgnoreCase) ||
                    !String.Equals(action, "login", StringComparison.OrdinalIgnoreCase))
                {
                    String message = ConfigurationManager.CloseInformation;

                    if (String.IsNullOrEmpty(message))
                    {
                        throw new SystemNotOpenException();
                    }
                    else
                    {
                        throw new SystemNotOpenException(message);
                    }
                }
            }

            base.OnAuthorization(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.CurrentContestCount = (this.OutputCurrentContestCount ? ContestManager.CountContests(false) : -1).ToString();

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            this.StopAndResetWatch();
            
            base.OnActionExecuted(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            System.Exception exception = filterContext.Exception;
            UserException userException = exception as UserException;

            if (userException == null || userException.IsNeedLog)
            {
                RouteData routeData = filterContext.RouteData;
                this.LogException(exception, routeData);
            }

            this.StopAndResetWatch();
        }

        /// <summary>
        /// 返回带有导航的页面
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="list">内容列表</param>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>操作后的结果</returns>
        protected virtual ActionResult ViewWithPager<T>(PagedList<T> list, Int32 pageIndex)
        {
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
        protected virtual ActionResult ViewWithPager<T>(String viewName, PagedList<T> list, Int32 pageIndex)
        {
            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = pageIndex;

            return View(viewName, list);
        }

        /// <summary>
        /// 返回带有导航的页面
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <typeparam name="TAnother">另一个对象类型</typeparam>
        /// <param name="list">内容列表</param>
        /// <param name="obj">另一个对象</param>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>操作后的结果</returns>
        protected virtual ActionResult ViewWithPager<T, TAnother>(PagedList<T> list, TAnother obj, Int32 pageIndex)
        {
            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = pageIndex;

            return View(new Tuple<TAnother, PagedList<T>>(obj, list));
        }

        /// <summary>
        /// 获取当前用户IP
        /// </summary>
        /// <returns>当前用户IP地址</returns>
        protected String GetCurrentUserIP()
        {
            return this.Request.GetRemoteClientIPv4();
        }

        /// <summary>
        /// 返回成功Json
        /// </summary>
        /// <returns>错误信息Json</returns>
        protected ActionResult SuccessJson()
        {
            return Content(String.Format("{{\"status\":\"success\"}}"), "application/json");
        }

        /// <summary>
        /// 返回成功Json
        /// </summary>
        /// <param name="json">成功内容</param>
        /// <returns>错误信息Json</returns>
        protected ActionResult SuccessJson(String json)
        {
            return Content(String.Format("{{\"status\":\"success\",\"result\":{0}}}", json), "application/json");
        }

        /// <summary>
        /// 返回错误信息Json
        /// </summary>
        /// <param name="error">错误信息</param>
        /// <returns>错误信息Json</returns>
        protected ActionResult ErrorJson(String error)
        {
            return Content(String.Format("{{\"status\":\"fail\",\"result\":\"{0}\"}}", JsonEncoder.JsonEncode(error)), "application/json");
        }
        #endregion

        #region RedirectToMessagePage
        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="msg">信息内容</param>
        /// <param name="backurl">返回转向页面</param>
        /// <param name="type">信息类型</param>
        protected virtual ActionResult RedirectToMessagePage(String msg, String backurl, MessageType type)
        {
            return RedirectToAction("Index", "Info", new
            {
                area = "",
                c = HttpUtility.UrlEncode(msg),
                s = (type != MessageType.Generic ? type.ToString().ToLowerInvariant() : ""),
                b = backurl
            });
        }

        /// <summary>
        /// 显示成功信息
        /// </summary>
        /// <param name="msg">信息内容</param>
        /// <returns>操作后的结果</returns>
        protected virtual ActionResult RedirectToSuccessMessagePage(String msg)
        {
            return RedirectToMessagePage(msg, String.Empty, MessageType.Success);
        }

        /// <summary>
        /// 显示错误信息
        /// </summary>
        /// <param name="msg">信息内容</param>
        /// <returns>操作后的结果</returns>
        protected virtual ActionResult RedirectToErrorMessagePage(String msg)
        {
            return RedirectToMessagePage(msg, String.Empty, MessageType.Danger);
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
        /// <param name="func">操作方法</param>
        /// <returns>操作后的结果</returns>
        protected async Task<ActionResult> ResultToMessagePage(Func<Task<Tuple<IMethodResult, String>>> func)
        {
            Tuple<IMethodResult, String> result = await func();

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

        /// <summary>
        /// 指定方法返回信息页面
        /// </summary>
        /// <typeparam name="T1">传入参数类型</typeparam>
        /// <typeparam name="T2">传入参数类型</typeparam>
        /// <typeparam name="T3">传入参数类型</typeparam>
        /// <typeparam name="T4">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg1">传入参数</param>
        /// <param name="arg2">传入参数</param>
        /// <param name="arg3">传入参数</param>
        /// <param name="arg4">传入参数</param>
        /// <param name="successInfo">成功后提示的内容</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToMessagePage<T1, T2, T3, T4>(Func<T1, T2, T3, T4, IMethodResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, String successInfo)
        {
            return ResultToMessagePage(() =>
            {
                return func(arg1, arg2, arg3, arg4);
            }, successInfo);
        }

        /// <summary>
        /// 指定方法返回信息页面
        /// </summary>
        /// <typeparam name="T1">传入参数类型</typeparam>
        /// <typeparam name="T2">传入参数类型</typeparam>
        /// <typeparam name="T3">传入参数类型</typeparam>
        /// <typeparam name="T4">传入参数类型</typeparam>
        /// <typeparam name="T5">传入参数类型</typeparam>
        /// <param name="func">操作方法</param>
        /// <param name="arg1">传入参数</param>
        /// <param name="arg2">传入参数</param>
        /// <param name="arg3">传入参数</param>
        /// <param name="arg4">传入参数</param>
        /// <param name="arg5">传入参数</param>
        /// <param name="successInfo">成功后提示的内容</param>
        /// <returns>操作后的结果</returns>
        protected ActionResult ResultToMessagePage<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, IMethodResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, String successInfo)
        {
            return ResultToMessagePage(() =>
            {
                return func(arg1, arg2, arg3, arg4, arg5);
            }, successInfo);
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
        protected ActionResult ResultToSuccessJson<T>(Func<T, IMethodResult> func, Func<T, IMethodResult> func2, T arg)
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

        #region Logging
        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="result">操作结果</param>
        /// <param name="username">可选用户名（为空则使用当前用户名）</param>
        protected void LogUserOperation(IMethodResult result, String username = null)
        {
            if (!result.IsWriteLog)
            {
                return;
            }

            RouteData routeData = RouteData.Route.GetRouteData(this.HttpContext);
            String controller = routeData.Values["controller"] as String;
            String action = routeData.Values["action"] as String;

            LogContext context = new LogContext()
            {
                Level = result.IsSuccess ? LogLevel.Information : LogLevel.Warning,
                Type = result.IsSuccess ? "success" : "failed",
                RequestUrl = HttpContext.Request.RawUrl,
                RefererUrl = (HttpContext.Request.UrlReferrer != null ? HttpContext.Request.UrlReferrer.ToString() : "null"),
                Controller = controller,
                Action = action,
                Message = result.Description,
                Username = String.IsNullOrEmpty(username) ? UserManager.CurrentUserName : username,
                UserIP = this.GetCurrentUserIP(),
                UserAgent = HttpContext.Request.UserAgent,
                TimeStamp = DateTime.Now,
            };

            LogManager.LogOperation(context);
        }

        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="routeData">路由数据</param>
        private void LogException(System.Exception exception, RouteData routeData)
        {
            String controller = routeData.Values["controller"] as String;
            String action = routeData.Values["action"] as String;

            ExceptionLogContext context = new ExceptionLogContext(exception)
            {
                Level = LogLevel.Error,
                RequestUrl = HttpContext.Request.RawUrl,
                RefererUrl = (HttpContext.Request.UrlReferrer != null ? HttpContext.Request.UrlReferrer.ToString() : "null"),
                Controller = controller,
                Action = action,
                Username = UserManager.CurrentUserName,
                UserIP = this.GetCurrentUserIP(),
                UserAgent = HttpContext.Request.UserAgent,
                TimeStamp = DateTime.Now,
            };

            LogManager.LogException(context);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 停止并重启计时器
        /// </summary>
        private void StopAndResetWatch()
        {
            if (this._stopWatch.IsRunning)
            {
                this._stopWatch.Stop();
                ViewBag.ExecutedTime = (((Double)this._stopWatch.ElapsedTicks) / Stopwatch.Frequency).ToString("F6");
                this._stopWatch.Reset();
            }
        }
        #endregion
    }
}