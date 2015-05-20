using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Core;

namespace SDNUOJ.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class HomeController : AdminBaseController
    {
        /// <summary>
        /// 后台首页页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult Index()
        {
            ViewBag.IsAccessDatabase = DatabaseManager.IsAccessDB.ToString().ToLowerInvariant();

            return View();
        }
    }
}