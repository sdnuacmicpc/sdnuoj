using System;
using System.Collections.Generic;
using System.Web.Mvc;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Entity.Complex;

namespace SDNUOJ.Areas.Contest.Controllers
{
    public class OverallController : ContestBaseController
    {
        /// <summary>
        /// 竞赛统计页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        [Authorize(Roles = "ContestManage")]
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "None", VaryByCustom = "nm")]
        public ActionResult Statistics()
        {
            ContestEntity contest = ViewData["Contest"] as ContestEntity;

            IDictionary<Int32, ContestProblemStatistic> statistics = SolutionManager.GetContestStatistic(contest.ContestID);
            List<ContestProblemEntity> problems = ContestProblemManager.GetContestProblemList(contest.ContestID);
            Dictionary<String, Byte> langs = LanguageManager.GetSupportLanguages(contest.SupportLanguage);

            ViewBag.Problems = problems;
            ViewBag.Languages = langs;

            return View(statistics);
        }
    }
}