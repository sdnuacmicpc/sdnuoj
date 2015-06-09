using System;
using System.Collections.Generic;
using System.Web.Mvc;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;

namespace SDNUOJ.Areas.Contest.Controllers
{
    public class StatusController : ContestBaseController
    {
        /// <summary>
        /// 提交状态列表页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <param name="pid">题目ID</param>
        /// <param name="name">用户名</param>
        /// <param name="lang">提交语言</param>
        /// <param name="type">评测结果</param>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "MinimumPageCache", VaryByParam = "id;pid;name;lang;type", VaryByCustom = "nm")]
        public ActionResult List(Int32 id = 1, Int32 pid = -1, String name = "", String lang = "", String type = "")
        {
            ContestEntity contest = ViewData["Contest"] as ContestEntity;
            Dictionary<String, Byte> langs = LanguageManager.GetSupportLanguages(contest.SupportLanguage);
            ViewBag.Languages = langs;

            PagedList<SolutionEntity> list = SolutionManager.GetSolutionList(id, contest.ContestID, pid, name, lang, type, null);

            ViewBag.ProblemID = pid;
            ViewBag.UserName = name;
            ViewBag.Language = lang;
            ViewBag.SearchType = type;

            return ViewWithPager(list, id);
        }
    }
}