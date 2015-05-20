using System;
using System.Web.Mvc;

namespace SDNUOJ.Controllers
{
    public class FAQController : Controller
    {
        /// <summary>
        /// FAQ页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "None")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// FAQ英文页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "None")]
        public ActionResult EN_US()
        {
            return View("Index");
        }

        /// <summary>
        /// FAQ中文页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "None")]
        public ActionResult ZH_CN()
        {
            return View();
        }
    }
}