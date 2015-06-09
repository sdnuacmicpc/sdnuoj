using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Attributes;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;

namespace SDNUOJ.Controllers
{
    [Authorize]
    [Function(PageType.UserMail)]
    public class MailController : BaseController
    {
        /// <summary>
        /// 邮件列表页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <param name="title">邮件标题</param>
        /// <param name="name">对方用户名</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Box(Int32 id = 1, String title = "", String name = "")
        {
            PagedList<UserMailEntity> list = UserMailManager.GetUserMails(id);

            ViewBag.Title = title;
            ViewBag.ToName = name;

            return ViewWithPager(list, id);
        }

        /// <summary>
        /// 邮件页面
        /// </summary>
        /// <param name="id">邮件ID</param>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "id", VaryByCustom = "nm")]
        public ActionResult Detail(Int32 id = -1)
        {
            return View(UserMailManager.GetUserMail(id));
        }

        /// <summary>
        /// 邮件发送页面
        /// </summary>
        /// <param name="title">邮件标题</param>
        /// <param name="name">对方用户名</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Send(String title = "", String name = "")
        {
            ViewBag.Title = title;
            ViewBag.ToName = name;

            return View();
        }

        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Send(FormCollection form)
        {
            String result;
            UserMailEntity mail = new UserMailEntity()
            {
                ToUserName = form["tousername"],
                Title = form["title"],
                Content = form["content"]
            };
            
            if (UserMailManager.TrySendUserMail(mail, out result))
            {
                return RedirectToSuccessMessagePage("Your mail has been successfully sent!");
            }
            else
            {
                return RedirectToErrorMessagePage(result);
            }
        }

        /// <summary>
        /// 邮件删除
        /// </summary>
        /// <param name="mailid">待删除的ID</param>
        /// <param name="page">当前列表页面</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(FormCollection form)
        {
            UserMailManager.DeleteUserMails(form["mailid"]);

            return RedirectToAction("Box", "Mail", new { id = form["page"] });
        }
    }
}