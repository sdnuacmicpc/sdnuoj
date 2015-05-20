using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;
using SDNUOJ.Utilities.Web;

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

            ViewBag.PageSize = list.PageSize;
            ViewBag.RecordCount = list.RecordCount;
            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = id;

            return View(list);
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

            if (NewsManager.AdminInsertNews(entity))
            {
                return RedirectToSuccessMessagePage("Your have added news successfully!");
            }
            else
            {
                return RedirectToErrorMessagePage("Failed to add news!");
            }
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

            if (NewsManager.AdminUpdateNews(entity))
            {
                return RedirectToSuccessMessagePage("Your have edited news successfully!");
            }
            else
            {
                return RedirectToErrorMessagePage("Failed to edit news!");
            }
        }

        /// <summary>
        /// 新闻删除
        /// </summary>
        /// <param name="id">新闻ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Delete(String id)
        {
            return ResultToJson(() => 
            {
                NewsManager.AdminDeleteNews(id);
            });
        }
    }
}