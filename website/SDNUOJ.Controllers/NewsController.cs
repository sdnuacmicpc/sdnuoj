using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;

namespace SDNUOJ.Controllers
{
    public class NewsController : BaseController
    {
        /// <summary>
        /// 新闻列表页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "id")]
        public ActionResult List(Int32 id = 1)
        {
            PagedList<NewsEntity> list = NewsManager.GetNewsList(id);

            return ViewWithPager(list, id);
        }

        /// <summary>
        /// 新闻页面
        /// </summary>
        /// <param name="id">新闻ID</param>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "id")]
        public ActionResult Detail(Int32 id = -1)
        {
            return View(NewsManager.GetNews(id));
        }
    }
}