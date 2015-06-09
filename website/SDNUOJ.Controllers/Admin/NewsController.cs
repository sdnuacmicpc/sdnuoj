using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;

namespace SDNUOJ.Areas.Admin.Controllers
{
    [Authorize(Roles = "NewsManage")]
    public class NewsController : AdminBaseController
    {
        /// <summary>
        /// 新闻管理页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <returns>操作后的结果</returns>
        public ActionResult List(Int32 id = 1)
        {
            PagedList<NewsEntity> list = NewsManager.AdminGetNewsList(id);

            return ViewWithPager(list, id);
        }

        /// <summary>
        /// 新闻添加页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult Add()
        {
            return View("Edit", new NewsEntity());
        }

        /// <summary>
        /// 新闻添加
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Add(FormCollection form)
        {
            NewsEntity entity = new NewsEntity()
            {
                Title = form["title"],
                Description = form["description"]
            };

            return ResultToMessagePage(NewsManager.AdminInsertNews, entity, "Your have added news successfully!");
        }

        /// <summary>
        /// 新闻编辑页面
        /// </summary>
        /// <param name="id">新闻ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Edit(Int32 id = -1)
        {
            return View("Edit", NewsManager.AdminGetNews(id));
        }

        /// <summary>
        /// 新闻编辑
        /// </summary>
        /// <param name="id">新闻ID</param>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Int32 id, FormCollection form)
        {
            NewsEntity entity = new NewsEntity()
            {
                AnnounceID = id,
                Title = form["title"],
                Description = form["description"]
            };

            return ResultToMessagePage(NewsManager.AdminUpdateNews, entity, "Your have edited news successfully!");
        }

        /// <summary>
        /// 新闻删除
        /// </summary>
        /// <param name="id">新闻ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Delete(String id)
        {
            return ResultToJson(NewsManager.AdminDeleteNews, id);
        }
    }
}