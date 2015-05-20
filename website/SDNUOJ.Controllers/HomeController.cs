using System;
using System.Collections.Generic;
using System.Web.Mvc;

using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;

namespace SDNUOJ.Controllers
{
    public class HomeController : BaseController
    {
        /// <summary>
        /// 首页页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "None")]
        public ActionResult Index()
        {
            NewsEntity defaultNews = NewsManager.GetDefaultNews();
            List<NewsEntity> lastestNews = NewsManager.GetLastestNewsList();
            List<UserEntity> topTenUsers = UserManager.GetWeeklyUserTop10Ranklist();

            return View(new Tuple<NewsEntity, List<NewsEntity>, List<UserEntity>>
                (defaultNews, lastestNews, topTenUsers));
        }
    }
}