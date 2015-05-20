using System;
using System.Collections.Generic;
using System.Web.Mvc;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Attributes;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;

namespace SDNUOJ.Controllers
{
    public class StatusController : BaseController
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
        [Function(PageType.MainStatus)]
        [OutputCache(CacheProfile = "MinimumPageCache", VaryByParam = "id;pid;name;lang;type")]
        public ActionResult List(Int32 id = 1, Int32 pid = -1, String name = "", String lang = "", String type = "")
        {
            Dictionary<String, Byte> langs = LanguageManager.MainSubmitSupportLanguages;
            ViewBag.Languages = langs;

            PagedList<SolutionEntity> list = SolutionManager.GetSolutionList(id, -1, pid, name, lang, type, null);

            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = id;

            ViewBag.ProblemID = pid;
            ViewBag.UserName = name;
            ViewBag.Language = lang;
            ViewBag.SearchType = type;

            return View(list);
        }
    }
}