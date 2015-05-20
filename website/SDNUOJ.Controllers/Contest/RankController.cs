using System;
using System.Collections.Generic;
using System.Web.Mvc;

using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Entity.Complex;

namespace SDNUOJ.Areas.Contest.Controllers
{
    public class RankController : ContestBaseController
    {
        /// <summary>
        /// 排行列表页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "MinimumPageCache", VaryByParam = "None", VaryByCustom = "nm")]
        public ActionResult List()
        {
            ContestEntity contest = ViewData["Contest"] as ContestEntity;

            List<RankItem> ranklist = ContestManager.GetContestRanklist(contest);
            List<ContestProblemEntity> problems = ContestProblemManager.GetContestProblemList(contest.ContestID);

            ViewBag.Problems = problems;

            return View(ranklist);
        }
    }
}