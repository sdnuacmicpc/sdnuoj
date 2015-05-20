using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;

namespace SDNUOJ.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class WelcomeController : AdminBaseController
    {
        /// <summary>
        /// 后台欢迎页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult Index()
        {
            ViewData["UserCount"] = UserManager.CountUserRanklist();
            ViewData["ProblemCount"] = ProblemManager.CountProblems();
            ViewData["SolutionCount"] = SolutionManager.CountSolutions();

            ViewData["SystemRunTime"] = this.GetSystemRunTime();
            ViewData["SystemVersion"] = ConfigurationManager.Version;

            ViewData["SystemPlatform"] = ConfigurationManager.Platform;
            ViewData["DataBaseType"] = DatabaseManager.DataBaseType;

            ViewData["ServerName"] = Server.MachineName.ToString() + "/" + Request.ServerVariables["LOCAL_ADDR"] + ":" + Request.ServerVariables["SERVER_PORT"];
            ViewData["DotNetVersion"] = ".NET CLR " + Environment.Version.ToString();

            ViewBag.IsSuperAdministrator = AdminManager.HasPermission(PermissionType.SuperAdministrator);
            ViewBag.IsAccessDatabase = DatabaseManager.IsAccessDB;

            return View();
        }

        /// <summary>
        /// 获取提交数据Json
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>操作后的结果</returns>
        public ActionResult SubmitStatistic(String year = "", String month = "")
        {
            try
            {
                DateTime date = (String.IsNullOrEmpty(year) || String.IsNullOrEmpty(month) ? DateTime.Today : DateTime.Parse(String.Format("{0}-{1}-1", year, month)));
                IDictionary<Int32, Int32> submits = SolutionManager.GetMonthlySubmitStatus(date, false);
                IDictionary<Int32, Int32> accepteds = SolutionManager.GetMonthlySubmitStatus(date, true);
                Int32 maxDay = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1).Day;

                for (Int32 i = 1; i <= maxDay; i++)
                {
                    if (!submits.ContainsKey(i))
                    {
                        submits[i] = 0;
                    }

                    if (!accepteds.ContainsKey(i))
                    {
                        accepteds[i] = 0;
                    }
                }

                StringBuilder sb = new StringBuilder();
                Int32 count = 0;

                sb.Append("{").Append("\"all\":[");
                foreach (KeyValuePair<Int32, Int32> pair in submits)
                {
                    if (count++ > 0)
                    {
                        sb.Append(",");
                    }

                    sb.Append("[").Append(pair.Key).Append(",").Append(pair.Value).Append("]");
                }
                sb.Append("],");

                count = 0;
                sb.Append("\"accepted\":[");
                foreach (KeyValuePair<Int32, Int32> pair in accepteds)
                {
                    if (count++ > 0)
                    {
                        sb.Append(",");
                    }

                    sb.Append("[").Append(pair.Key).Append(",").Append(pair.Value).Append("]");
                }
                sb.Append("],");
                sb.Append("\"date\":\"").Append(date.ToString("yyyy-M")).Append("\"}");

                return SuccessJson(sb.ToString());
            }
            catch (Exception ex)
            {
                return ErrorJson(ex.Message);
            }
        }

        private String GetSystemRunTime()
        {
            TimeSpan ts = DateTime.Now - ConfigurationManager.SystemStartTime;
            String format = String.Empty;

            if (ts.TotalDays >= 1) format = "{0} 天 {1} 小时";
            else if (ts.TotalHours >= 1) format = "{1} 小时 {2} 分";
            else if (ts.TotalMinutes >= 1) format = "{2} 分 {3} 秒";
            else format = "{3} 秒";

            return String.Format(format, ts.TotalDays.ToString("F0"), ts.TotalHours.ToString("F0"), ts.Minutes.ToString("F0"), ts.Seconds.ToString("F0"));
        }
    }
}