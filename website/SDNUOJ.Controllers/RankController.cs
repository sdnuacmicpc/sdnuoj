using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Attributes;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;

namespace SDNUOJ.Controllers
{
    [Function(PageType.MainRanklist)]
    public class RankController : BaseController
    {
        /// <summary>
        /// 排行列表页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam= "id")]
        public ActionResult List(Int32 id = 1)
        {
            PagedList<UserEntity> list = UserManager.GetUserRanklist(id);

            return ViewWithPager(list, id);
        }
    }
}