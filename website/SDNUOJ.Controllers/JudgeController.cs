using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Core.Judge;
using SDNUOJ.Utilities.Text;

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

            String error = String.Empty;

            if (JudgeStatusManager.TryJudgeServerLogin(username, password, out error))
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

        #region 私有方法
        private ContentResult ErrorJson(String error)
        {
            return Content("{\"status\":\"error\", \"message\":\"" + JsonEncoder.JsonEncode(error) + "\"}", "application/json");
        }

        private ContentResult SuccessJson()
        {
            return Content("{\"status\":\"success\"}", "application/json");
        }
        #endregion
    }
}