using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDNUOJ.Caching;
using SDNUOJ.Controllers.Attributes;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Controllers.Status;
using SDNUOJ.Entity;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Controllers
{
    public class UserController : BaseController
    {
        /// <summary>
        /// 用户登录页面
        /// </summary>
        /// <param name="returnurl">返回地址</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Login(String returnurl = "")
        {
            ViewBag.ReturnUrl = returnurl;

            return View();
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            String username = form["username"];
            String password = form["password"];
            String userip = this.GetCurrentUserIP();

            IMethodResult result = UserManager.SignIn(username, password, userip);
            this.LogUserOperation(result, username);

            if (!result.IsSuccess)
            {
                return RedirectToErrorMessagePage(result.Description);
            }

            String returnUrl = form["referer"];

            if (!String.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToRefferer();
            }
        }

        /// <summary>
        /// 用户注销
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult Logout()
        {
            String userName = UserManager.CurrentUserName;

            UserCurrentStatus.RemoveCurrentUserStatus();
            UserBrowserStatus.RemoveCurrentUserBrowserStatus();
            UserSubmitStatus.RemoveLastSubmitTime(userName);
            UserMailCache.RemoveUserUnReadMailCountCache(userName);

            return RedirectToRefferer();
        }

        /// <summary>
        /// 用户信息页面
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns>操作后的结果</returns>
        [Function(PageType.UserInfo)]
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "id")]
        public ActionResult Info(String id)
        {
            UserEntity user = UserManager.GetUserInfoWithRank(id);
            List<Int32> lstSolved = SolutionManager.GetSolvedProblemIDListByUser(user.UserName, -1);
            List<Int32> lstUnsolved = SolutionManager.GetUnSolvedProblemIDListByUser(user.UserName, lstSolved, -1);

            return View(new Tuple<UserEntity, List<Int32>, List<Int32>>(user, lstSolved, lstUnsolved));
        }

        /// <summary>
        /// 用户注册页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        [Function(PageType.Register)]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [Function(PageType.Register)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(FormCollection form)
        {
            UserEntity user = new UserEntity()
            {
                UserName = form["username"],
                NickName = form["nickname"],
                School = form["school"],
                Email = form["email"]
            };

            String userip = this.GetCurrentUserIP();
            IMethodResult result = UserManager.SignUp(user, form["password"], form["password2"], form["checkcode"], userip);

            if (!result.IsSuccess)
            {
                return RedirectToErrorMessagePage(result.Description);
            }

            result = UserManager.SignIn(form["username"], form["password"], userip);
            this.LogUserOperation(result, user.UserName);

            if (!result.IsSuccess)
            {
                return RedirectToErrorMessagePage(result.Description);
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 用户控制面板页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        [Function(PageType.UserControl)]
        [Authorize]
        public ActionResult Control()
        {
            UserEntity entity = UserManager.GetUserInfo(UserManager.CurrentUserName);

            return View(entity);
        }

        /// <summary>
        /// 用户修改信息
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [Function(PageType.UserControl)]
        [HttpPost]
        [Authorize]
        public ActionResult Control(FormCollection form)
        {
            UserEntity user = new UserEntity()
            {
                NickName = form["nickname"],
                School = form["school"],
                Email = form["email"]
            };

            return ResultToMessagePage(UserManager.UpdateUserInfo, user, form["password"], form["newpassword"], form["newpassword2"], "Your user profile was updated successfully!");
        }

        /// <summary>
        /// 用户找回密码页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        [Function(PageType.ForgetPassword)]
        public ActionResult ForgetPassword()
        {
            return View();
        }

        /// <summary>
        /// 用户找回密码
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [Function(PageType.ForgetPassword)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgetPassword(FormCollection form)
        {
            return await ResultToMessagePage(async () =>
            {
                String userip = this.GetCurrentUserIP();
                String link = Url.Action("ResetPassword", "User") + "?rid=";

                IMethodResult result = await UserForgetPasswordManager.RequestResetUserPassword(form["username"], form["email"], userip, form["checkcode"], link);

                if (!result.IsSuccess)
                {
                    return new Tuple<IMethodResult, String>(result, String.Empty);
                }

                String successInfo = "We have sent a password reset link to your email address, the link is valid for 24 hours.";

                return new Tuple<IMethodResult, String>(result, successInfo);
            });
        }

        /// <summary>
        /// 用户重设密码页面
        /// </summary>
        /// <param name="rid">重设密码ID</param>
        /// <returns>操作后的结果</returns>
        [Function(PageType.ForgetPassword)]
        public ActionResult ResetPassword(String rid)
        {
            return View(UserForgetPasswordManager.GetUserForgetRequest(rid));
        }

        /// <summary>
        /// 用户重设密码
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [Function(PageType.ForgetPassword)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(FormCollection form)
        {
            String userip = this.GetCurrentUserIP();

            return ResultToMessagePage(UserForgetPasswordManager.ResetUserPassword,
                form["rid"], form["username"], form["password"], form["password2"], userip, 
                "Your password was updated successfully!");
        }

        /// <summary>
        /// 重定向到来源网址
        /// </summary>
        /// <returns>操作后的结果</returns>
        private ActionResult RedirectToRefferer()
        {
            String referrer = (Request.UrlReferrer != null ? Request.UrlReferrer.ToString().ToLowerInvariant() : "");

            if (String.IsNullOrEmpty(referrer))
            {
                return RedirectToAction("Index", "Home");
            }
            else if (referrer.IndexOf("/info?") >= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (referrer.IndexOf("/user/register") >= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (referrer.IndexOf("/user/login") >= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (referrer.IndexOf("/mail/detail/") >= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
        }
    }
}