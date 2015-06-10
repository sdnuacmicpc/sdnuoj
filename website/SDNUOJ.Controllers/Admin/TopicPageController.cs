using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;

namespace SDNUOJ.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdministrator")]
    public class TopicPageController : AdminBaseController
    {
        /// <summary>
        /// 专题页面管理页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <returns>操作后的结果</returns>
        public ActionResult List(Int32 id = 1)
        {
            PagedList<TopicPageEntity> list = TopicPageManager.AdminGetTopicPageList(id);

            return ViewWithPager(list, id);
        }

        /// <summary>
        /// 专题页面添加页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult Add()
        {
            return View("Edit", new TopicPageEntity());
        }

        /// <summary>
        /// 专题页面添加
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Add(FormCollection form)
        {
            TopicPageEntity entity = new TopicPageEntity()
            {
                PageName = form["name"],
                Title = form["title"],
                Description = form["description"],
                Content = form["content"]
            };

            return ResultToMessagePage(TopicPageManager.AdminInsertTopicPage, entity, "Your have added page successfully!");
        }

        /// <summary>
        /// 专题页面编辑页面
        /// </summary>
        /// <param name="id">专题页面名称</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Edit(String id)
        {
            return ResultToView("Edit", TopicPageManager.AdminGetTopicPage, id);
        }

        /// <summary>
        /// 专题页面编辑
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection form)
        {
            TopicPageEntity entity = new TopicPageEntity()
            {
                PageName = form["name"],
                Title = form["title"],
                Description = form["description"],
                Content = form["content"]
            };

            return ResultToMessagePage(TopicPageManager.AdminUpdateTopicPage, entity, form["oldname"], "Your have edited page successfully!");
        }

        /// <summary>
        /// 专题页面隐藏
        /// </summary>
        /// <param name="id">专题页面名称</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Hide(String id)
        {
            return ResultToJson(TopicPageManager.AdminUpdateTopicPageIsHide, id, true);
        }

        /// <summary>
        /// 专题页面显示
        /// </summary>
        /// <param name="id">专题页面名称</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Show(String id)
        {
            return ResultToJson(TopicPageManager.AdminUpdateTopicPageIsHide, id, false);
        }

        /// <summary>
        /// 专题页面删除
        /// </summary>
        /// <param name="id">专题页面名称</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Delete(String id)
        {
            return ResultToJson(TopicPageManager.AdminDeleteTopicPages, id);
        }
    }
}