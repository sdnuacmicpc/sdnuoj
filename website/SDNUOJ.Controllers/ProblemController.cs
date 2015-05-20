using System;
using System.Collections.Generic;
using System.Web.Mvc;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Attributes;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Entity.Complex;
using SDNUOJ.Utilities;

namespace SDNUOJ.Controllers
{
    public class ProblemController : BaseController
    {
        /// <summary>
        /// 题目列表页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <returns>操作后的结果</returns>
        [Function(PageType.MainProblem)]
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "id", VaryByCustom = "pm")]
        public ActionResult Set(Int32 id = 1)
        {
            PagedList<ProblemEntity> list = ProblemManager.GetProblemSet(id);

            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = id;

            return View(list);
        }

        /// <summary>
        /// 题目搜索页面
        /// </summary>
        /// <param name="type">搜索类型</param>
        /// <param name="content">搜索值</param>
        /// <returns>操作后的结果</returns>
        [Function(PageType.MainProblem)]
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "type;content", VaryByCustom = "pm")]
        public ActionResult Search(String type, String content)
        {
            List<ProblemEntity> list = ProblemManager.GetProblemBySearch(type, content);

            ViewBag.SearchType = type;
            ViewBag.Content = content;

            return View(list);
        }

        /// <summary>
        /// 题目页面
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <returns>操作后的结果</returns>
        [Function(PageType.MainProblem)]
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "id", VaryByCustom = "pm")]
        public ActionResult Show(Int32 id = -1)
        {
            return View(ProblemManager.GetProblem(id));
        }

        /// <summary>
        /// 题目分类页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        [Function(PageType.MainProblem)]
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "None")]
        public ActionResult Category()
        {
            List<ProblemCategoryEntity> list = ProblemCategoryManager.GetProblemCategoryList();

            return View(list);
        }

        /// <summary>
        /// 题目统计页面
        /// </summary>
        /// <param name="pid">题目ID</param>
        /// <param name="page">页面索引</param>
        /// <param name="name">用户名</param>
        /// <param name="lang">提交语言</param>
        /// <returns>操作后的结果</returns>
        [Function(PageType.MainProblem)]
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "pid;id;lang;order")]
        public ActionResult Statistic(Int32 pid = 0, Int32 id = 1, String lang = "all", String order = "default")
        {
            ProblemStatistic ps = SolutionManager.GetProblemStatistic(-1, pid);

            String reallang = lang.ToByte(LanguageType.Null.ID).ToString();
            String realorder = order.ToInt32(-1).ToString();

            PagedList<SolutionEntity> list = SolutionManager.GetSolutionList(id, -1, pid, null, reallang, ((Byte)ResultType.Accepted).ToString(), realorder);

            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = id;

            Dictionary<String, Byte> langs = LanguageManager.MainSubmitSupportLanguages;
            ViewBag.Languages = langs;

            ViewBag.Language = lang;
            ViewBag.Order = order;

            return View(new Tuple<ProblemStatistic, PagedList<SolutionEntity>>(ps, list));
        }

        /// <summary>
        /// 代码提交页面
        /// </summary>
        /// <param name="id">提交ID</param>
        /// <returns>操作后的结果</returns>
        [Function(PageType.MainSubmit)]
        [Authorize]
        public ActionResult Submit(Int32 id = -1)
        {
            ProblemEntity problem = ProblemManager.GetProblem(id);
            IEnumerable<SelectListItem> items = this.GetLanguageItems();

            ViewBag.Languages = items;

            return View(problem);
        }

        private IEnumerable<SelectListItem> GetLanguageItems()
        {
            Dictionary<String, Byte> langs = LanguageManager.MainSubmitSupportLanguages;

            return new SelectList(langs, "Value", "Key");
        }
    }
}