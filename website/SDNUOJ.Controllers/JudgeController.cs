using System;
using System.Web.Mvc;
using System.Web.Routing;

using SDNUOJ.Controllers.Core.Judge;
using SDNUOJ.Logging;
using SDNUOJ.Utilities.Text;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Controllers
{
    public class JudgeController : Controller
    {
        /// <summary>
        /// 评测机登录
        /// </summary>
        /// <returns>Json结果</returns>
        public ActionResult Login()
        {
            String username = Request["username"];
            String password = Request["password"];
            String userip = HttpContext.Request.GetRemoteClientIPv4();

            String error = String.Empty;

            if (JudgeStatusManager.TryJudgeServerLogin(username, password, userip, out error))
            {
                return SuccessJson();
            }
            else
            {
                return ErrorJson(error);
            }
        }

        /// <summary>
        /// 评测机获取评测列表
        /// </summary>
        /// <returns>Json结果</returns>
        public ActionResult GetPending()
        {
            String count = Request["count"];
            String lanaugeSupport = Request["supported_languages"];

            String result = String.Empty;
            String error = String.Empty;

            if (JudgeSolutionManager.TryGetPendingListJson(lanaugeSupport, count, out result, out error))
            {
                return Content(result, "application/json");
            }
            else
            {
                return ErrorJson(error);
            }
        }

        /// <summary>
        /// 评测机获取题目数据
        /// </summary>
        /// <returns>Json结果</returns>
        public ActionResult GetProblem()
        {
            String pid = Request["pid"];

            String dataPath = String.Empty;
            String error = String.Empty;

            if (JudgeProblemManager.TryGetProblemDataPath(pid, out dataPath, out error))
            {
                return File(dataPath, "application/zip");
            }
            else
            {
                return ErrorJson(error);
            }
        }

        /// <summary>
        /// 评测机更新评测状态
        /// </summary>
        /// <returns>Json结果</returns>
        [ValidateInput(false)]
        public ActionResult UpdateStatus()
        {
            String sid = Request["sid"];
            String pid = Request["pid"];
            String username = Request["username"];
            String resultcode = Request["resultcode"];
            String detail = Request["detail"];
            String timecost = Request["timecost"];
            String memorycost = Request["memorycost"];

            String error = String.Empty;

            if (JudgeSolutionManager.TryUpdateSolutionStatus(sid, pid, username, resultcode, detail, timecost, memorycost, out error))
            {
                return SuccessJson();
            }
            else
            {
                return ErrorJson(error);
            }
        }

        #region 保护方法
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            try
            {
                this.LogException(filterContext.Exception, filterContext.RouteData);
            } catch (System.Exception)
            {
                ;//Do Nothing
            }
        }
        #endregion

        #region 私有方法
        private ContentResult ErrorJson(String error)
        {
            return Content("{\"status\":\"error\", \"message\":\"" + JsonEncoder.JsonEncode(error) + "\"}", "application/json");
        }

        private ContentResult SuccessJson()
        {
            return Content("{\"status\":\"success\"}", "application/json");
        }

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
                Username = JudgeStatusManager.JudgeUserName,
                UserIP = this.Request.GetRemoteClientIPv4(),
                UserAgent = HttpContext.Request.UserAgent,
                TimeStamp = DateTime.Now,
            };

            LogManager.LogException(context);
        }
        #endregion
    }
}