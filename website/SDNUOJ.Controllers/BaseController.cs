using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Logging;

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
                String controller = filterContext.RouteData.Values["controller"] as String;
                String action = filterContext.RouteData.Values["action"] as String;

                LogManager.LogException(exception, controller, action);
            }

            this.StopAndResetWatch();
        }

        /// <summary>
        /// 指定指定方法返回Json结果
        /// </summary>
        /// <param name="action">指定方法</param>
        /// <returns>执行成功返回成功，执行失败返回失败</returns>
        protected ActionResult ResultToJson(Action action)
        {
            try
            {
                action();
                return SuccessJson();
            }
            catch (System.Exception ex)
            {
                return ErrorJson(ex.Message);
            }
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
            return Content(String.Format("{{\"status\":\"fail\",\"result\":\"{0}\"}}", error), "application/json");
        }

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
        /// 显示错误信息
        /// </summary>
        /// <param name="msg">信息内容</param>
        protected virtual ActionResult RedirectToErrorMessagePage(String msg)
        {
            return RedirectToMessagePage(msg, String.Empty, MessageType.Danger);
        }

        /// <summary>
        /// 显示成功信息
        /// </summary>
        /// <param name="msg">信息内容</param>
        protected virtual ActionResult RedirectToSuccessMessagePage(String msg)
        {
            return RedirectToMessagePage(msg, String.Empty, MessageType.Success);
        }
        #endregion

        #region 私有方法
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