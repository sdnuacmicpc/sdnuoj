using System;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDNUOJ.Controllers.Attributes;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;

namespace SDNUOJ.Controllers
{
    [Function(PageType.Contest)]
    public class ContestsController : BaseController
    {
        /// <summary>
        /// 当前竞赛页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "id")]
        public ActionResult Current(Int32 id = 1)
        {
            PagedList<ContestEntity> list = ContestManager.GetContestList(id, false);

            return ViewWithPager("Index", list, id);
        }

        /// <summary>
        /// 过去竞赛页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "id")]
        public ActionResult Passed(Int32 id = 1)
        {
            PagedList<ContestEntity> list = ContestManager.GetContestList(id, true);

            return ViewWithPager("Index", list, id);
        }

        /// <summary>
        /// 竞赛注册页面
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <returns>操作后的结果</returns>
        [Authorize]
        public ActionResult Register(Int32 id = -1)
        {
            ContestEntity entity = ContestManager.GetRegisterContest(id);

            ViewBag.UserName = UserManager.CurrentUserName;

            return View(entity);
        }

        /// <summary>
        /// 竞赛注册页面
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Int32 id, FormCollection form)
        {
            if (!ContestUserManager.RegisterCurrentUser(id, form["realname"]))
            {
                return RedirectToErrorMessagePage("Contest Registration Failed!");
            }

            return RedirectToSuccessMessagePage("Your have register this contest successfully!");
        }

        /// <summary>
        /// 最近竞赛页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "None")]
        public ActionResult Recent()
        {
            return View();
        }

        /// <summary>
        /// 最近竞赛信息
        /// </summary>
        /// <returns>最近竞赛信息Json</returns>
        public async Task<ActionResult> RecentInfo()
        {
            String content = await RecentContestManager.GetAllRecentContestsJsonAsync();

            return Content(content, "application/json");
        }
    }
}