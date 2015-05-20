using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;

namespace SDNUOJ.Controllers
{
    public class TopicPageController : BaseController
    {
        /// <summary>
        /// 专题页面
        /// </summary>
        /// <param name="name">专题页面名称</param>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "name", VaryByCustom = "sa")]
        public ActionResult Index(String name)
        {
            try
            {
                return View(TopicPageManager.GetTopicPage(name));
            }
            catch
            {
                return HttpNotFound();
            }
        }
    }
}