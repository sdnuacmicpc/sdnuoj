using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;

namespace SDNUOJ.Areas.Contest.Controllers
{
    public class HomeController : ContestBaseController
    {
        /// <summary>
        /// 竞赛首页页面
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "None", VaryByCustom = "nm")]
        public ActionResult Index(Int32 cid)
        {
            ContestEntity entity = ViewData["Contest"] as ContestEntity;
            
            return View(entity);
        }
    }
}