using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Areas.Admin.Controllers
{
    [Authorize(Roles = "ResourceManage")]
    public class ResourceController : AdminBaseController
    {
        /// <summary>
        /// 资源管理页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <returns>操作后的结果</returns>
        public ActionResult List(Int32 id = 1)
        {
            PagedList<ResourceEntity> list = ResourceManager.AdminGetResourceList(id);

            ViewBag.PageSize = list.PageSize;
            ViewBag.RecordCount = list.RecordCount;
            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = id;

            return View(list);
        }

        /// <summary>
        /// 资源添加页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult Add()
        {
            return View("Edit", new ResourceEntity());
        }

        /// <summary>
        /// 资源添加
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(FormCollection form)
        {
            ResourceEntity entity = new ResourceEntity()
            {
                Title = form["title"],
                Url = form["url"],
                Type = form["type"]
            };

            if (ResourceManager.AdminInsertResource(entity))
            {
                return RedirectToSuccessMessagePage("Your have added resource successfully!");
            }
            else
            {
                return RedirectToErrorMessagePage("Failed to add resource!");
            }
        }

        /// <summary>
        /// 资源编辑页面
        /// </summary>
        /// <param name="id">资源ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Edit(Int32 id = -1)
        {
            return View("Edit", ResourceManager.AdminGetResource(id));
        }

        /// <summary>
        /// 资源编辑
        /// </summary>
        /// <param name="id">资源ID</param>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Int32 id, FormCollection form)
        {
            ResourceEntity entity = new ResourceEntity()
            {
                ResourceID = id,
                Title = form["title"],
                Url = form["url"],
                Type = form["type"]
            };

            if (ResourceManager.AdminUpdateResource(entity))
            {
                return RedirectToSuccessMessagePage("Your have edited resource successfully!");
            }
            else
            {
                return RedirectToErrorMessagePage("Failed to edit resource!");
            }
        }

        /// <summary>
        /// 资源删除
        /// </summary>
        /// <param name="id">资源ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Delete(String id)
        {
            return ResultToJson(() =>
            {
                ResourceManager.AdminDeleteResources(id);
            });
        }
    }
}