using System;
using System.Web.Mvc;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;

namespace SDNUOJ.Areas.Admin.Controllers
{
    [Authorize(Roles = "SolutionManage")]
    public class SolutionController : AdminBaseController
    {
        /// <summary>
        /// 提交列表页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <param name="order">排序类型</param>
        /// <param name="sids">提交ID列表</param>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="username">发帖人用户名</param>
        /// <param name="lang">提交语言</param>
        /// <param name="type">评测结果</param>
        /// <param name="startdate">提交时间范围起始日期</param>
        /// <param name="starthour">提交时间范围起始时</param>
        /// <param name="startminute">提交时间范围起始分</param>
        /// <param name="startsecond">提交时间范围起始秒</param>
        /// <param name="enddate">提交时间范围结束日期</param>
        /// <param name="endhour">提交时间范围结束时</param>
        /// <param name="endminute">提交时间范围结束分</param>
        /// <param name="endsecond">提交时间范围结束秒</param>
        /// <returns>操作后的结果</returns>
        [Authorize(Roles = "SuperAdministrator")]
        public ActionResult List(Int32 id = 1, String order = "",
            String sids = "", String cid = "", String pid = "", String username = "", String lang = "", String type = "",
            String startdate = "", String starthour = "", String startminute = "", String startsecond = "",
            String enddate = "", String endhour = "", String endminute = "", String endsecond = "")
        {
            String startFullDate = (String.IsNullOrEmpty(startdate) ? String.Empty : String.Format("{0} {1}:{2}:{3}", startdate, starthour, startminute, startsecond));
            String endFullDate = (String.IsNullOrEmpty(enddate) ? String.Empty : String.Format("{0} {1}:{2}:{3}", enddate, endhour, endminute, endsecond));

            PagedList<SolutionEntity> list = SolutionManager.AdminGetSolutionList(id,
                sids, cid, pid, username, lang, type, startFullDate, endFullDate, order);

            ViewBag.PageSize = list.PageSize;
            ViewBag.RecordCount = list.RecordCount;
            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = id;

            ViewBag.Languages = LanguageManager.AllSupportLanguages;

            ViewBag.Order = order;
            ViewBag.SolutionIDs = sids;
            ViewBag.ContestID = cid;
            ViewBag.ProblemID = pid;
            ViewBag.UserName = username;
            ViewBag.Language = lang;
            ViewBag.Type = type;
            ViewBag.StartDate = startdate;
            ViewBag.StartHour = starthour;
            ViewBag.StartMinute = startminute;
            ViewBag.StartSecond = startsecond;
            ViewBag.EndDate = enddate;
            ViewBag.EndHour = endhour;
            ViewBag.EndMinute = endminute;
            ViewBag.EndSecond = endsecond;

            return View(list);
        }

        /// <summary>
        /// 快速重测提交
        /// </summary>
        /// <param name="id">提交ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult QuickRejudge(String id)
        {
            return ResultToJson(() =>
            {
                SolutionManager.AdminRejudgeSolution(id, null, null, null, null, null, null, null);
            });
        }

        /// <summary>
        /// 重测提交页面
        /// </summary>
        /// <param name="sids">提交ID列表</param>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="username">发帖人用户名</param>
        /// <param name="lang">提交语言</param>
        /// <param name="type">评测结果</param>
        /// <param name="startdate">提交时间范围起始日期</param>
        /// <param name="starthour">提交时间范围起始时</param>
        /// <param name="startminute">提交时间范围起始分</param>
        /// <param name="startsecond">提交时间范围起始秒</param>
        /// <param name="enddate">提交时间范围结束日期</param>
        /// <param name="endhour">提交时间范围结束时</param>
        /// <param name="endminute">提交时间范围结束分</param>
        /// <param name="endsecond">提交时间范围结束秒</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Rejudge(
            String sids = "", String cid = "", String pid = "", String username = "", String lang = "", String type = "",
            String startdate = "", String starthour = "", String startminute = "", String startsecond = "",
            String enddate = "", String endhour = "", String endminute = "", String endsecond = "")
        {
            ViewBag.Languages = LanguageManager.AllSupportLanguages;

            ViewBag.SolutionIDs = sids;
            ViewBag.ContestID = cid;
            ViewBag.ProblemID = pid;
            ViewBag.UserName = username;
            ViewBag.Language = lang;
            ViewBag.Type = type;
            ViewBag.StartDate = startdate;
            ViewBag.StartHour = starthour;
            ViewBag.StartMinute = startminute;
            ViewBag.StartSecond = startsecond;
            ViewBag.EndDate = enddate;
            ViewBag.EndHour = endhour;
            ViewBag.EndMinute = endminute;
            ViewBag.EndSecond = endsecond;

            return View();
        }

        /// <summary>
        /// 重测提交
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Rejudge(FormCollection form)
        {
            String sids = form["sids"];
            String cid = form["cid"];
            String pid = form["pid"];
            String name = form["username"];
            String lang = form["lang"];
            String type = form["type"];
            String startDate = (String.IsNullOrEmpty(form["startdate"]) ? String.Empty : String.Format("{0} {1}:{2}:{3}", form["startdate"], form["starthour"], form["startminute"], form["startsecond"]));
            String endDate = (String.IsNullOrEmpty(form["enddate"]) ? String.Empty : String.Format("{0} {1}:{2}:{3}", form["enddate"], form["endhour"], form["endminute"], form["endsecond"]));

            Int32 count = SolutionManager.AdminRejudgeSolution(sids, cid, pid, name, lang, type, startDate, endDate);

            if (count > 0)
            {
                return RedirectToSuccessMessagePage(String.Format("{0} solution(s) have been successfully rejudged!", count.ToString()));
            }
            else
            {
                return RedirectToErrorMessagePage("No solution has been rejudged!");
            }
        }
    }
}