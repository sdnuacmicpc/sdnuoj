using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Attributes;
using SDNUOJ.Controllers.Core;

namespace SDNUOJ.Controllers
{
    [Function(PageType.Resource)]
    public class ResourceController : BaseController
    {
        /// <summary>
        /// 下载资源页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        [Authorize]
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "None", VaryByCustom = "in")]
        public ActionResult Index()
        {
            return View(ResourceManager.GetResourcesTreeList());
        }
    }
}