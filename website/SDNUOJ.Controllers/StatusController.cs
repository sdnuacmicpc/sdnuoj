using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            ViewBag.ProblemID = pid;
            ViewBag.UserName = name;
            ViewBag.Language = lang;
            ViewBag.SearchType = type;

            StringBuilder queryBuilder = new StringBuilder();
            Dictionary<Int32, bool> hasResult = new Dictionary<int, bool>();
            foreach(var item in list)
            {
                Boolean working = // 是否在评测或等待评测
                    item.Result == SDNUOJ.Entity.ResultType.Pending ||
                    item.Result == SDNUOJ.Entity.ResultType.RejudgePending ||
                    item.Result == SDNUOJ.Entity.ResultType.Judging;

                hasResult[item.SolutionID] = !working;
                if (working) // 加入查询
                {
                    queryBuilder.Append(item.SolutionID.ToString() + ",");
                }
            }

            ViewBag.HasResult = hasResult;
            ViewBag.QueryStr = queryBuilder.ToString().TrimEnd(',');

            return ViewWithPager(list, id);
        }

        /// <summary>
        /// 查询评测状态, 供AJAX更新Status页面
        /// </summary>
        /// <param name="sids">多个提交ID(逗号分隔)</param>
        /// <returns>查询结果(JSON)</returns>
        public ActionResult QueryStatus(String sids = "")
        {
            List<SolutionEntity> soluList = SolutionManager.GetSolutionListBySids(sids);
            var nowStatus =
                from x in soluList
                select new
                {
                    x.SolutionID,
                    x.Result,
                    x.ResultString,
                    x.TimeCost,
                    x.MemoryCost
                };

            return Json(nowStatus, "text/plain", JsonRequestBehavior.AllowGet);
        }
    }
}