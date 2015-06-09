using System;
using System.Collections.Generic;
using System.Web.Mvc;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Attributes;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Entity.Complex;
using SDNUOJ.Utilities;

namespace SDNUOJ.Areas.Contest.Controllers
{
    public class ProblemController : ContestBaseController
    {
        /// <summary>
        /// 题目列表页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "None", VaryByCustom = "nm")]
        public ActionResult Set()
        {
            ContestEntity contest = ViewData["Contest"] as ContestEntity;
            List<ProblemEntity> list = ContestProblemManager.GetProblemSet(contest.ContestID);

            Dictionary<Int32, Int16> userSubmits = SolutionManager.GetUserContestSubmit(contest.ContestID);
            ViewBag.UserSubmits = userSubmits;

            return View(list);
        }

        /// <summary>
        /// 题目页面
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "id", VaryByCustom = "nm")]
        public ActionResult Show(Int32 id = -1)
        {
            ContestEntity contest = ViewData["Contest"] as ContestEntity;
            ProblemEntity entity = ContestProblemManager.GetProblem(contest.ContestID, id);

            ViewBag.ContestProblemID = id.ToString();

            return View(entity);
        }

        /// <summary>
        /// 题目统计页面
        /// </summary>
        /// <param name="pid">题目ID</param>
        /// <param name="page">页面索引</param>
        /// <param name="name">用户名</param>
        /// <param name="lang">提交语言</param>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "pid;id;lang;order", VaryByCustom = "nm")]
        public ActionResult Statistic(Int32 pid = 0, Int32 id = 1, String lang = "all", String order = "default")
        {
            ContestEntity contest = ViewData["Contest"] as ContestEntity;
            ProblemStatistic ps = SolutionManager.GetProblemStatistic(contest.ContestID, pid);

            String reallang = lang.ToByte(LanguageType.Null.ID).ToString();
            String realorder = order.ToInt32(-1).ToString();

            Dictionary<String, Byte> langs = LanguageManager.GetSupportLanguages(contest.SupportLanguage);
            ViewBag.Languages = langs;

            PagedList<SolutionEntity> list = SolutionManager.GetSolutionList(id, contest.ContestID, pid, null, reallang, ((Byte)ResultType.Accepted).ToString(), realorder);

            ViewBag.ContestProblemID = pid.ToString();
            ViewBag.Language = lang;
            ViewBag.Order = order;

            return ViewWithPager(list, ps, id);
        }

        /// <summary>
        /// 代码提交页面
        /// </summary>
        /// <param name="id">提交ID</param>
        /// <returns>操作后的结果</returns>
        [Authorize]
        [ContestSubmit]
        public ActionResult Submit(Int32 id = -1)
        {
            ContestEntity contest = ViewData["Contest"] as ContestEntity;
            ProblemEntity problem = ContestProblemManager.GetProblem(contest.ContestID, id);
            IEnumerable<SelectListItem> items = this.GetLanguageItems(contest.SupportLanguage);

            ViewBag.Languages = items;
            ViewBag.ContestProblemID = id.ToString();

            return View(problem);
        }

        private IEnumerable<SelectListItem> GetLanguageItems(String langTypes)
        {
            Dictionary<String, Byte> langs = LanguageManager.GetSupportLanguages(langTypes);

            return new SelectList(langs, "Value", "Key");
        }
    }
}