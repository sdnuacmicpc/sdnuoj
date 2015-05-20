using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

using SDNUOJ.Caching;
using SDNUOJ.Controllers.Attributes;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Controllers.Status;
using SDNUOJ.Entity;

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
            String userName = form["username"];
            String passWord = form["password"];
            String result;

            if (!UserManager.TrySignIn(userName, passWord, out result))
            {
                throw new OperationFailedException(result);
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
            String result;
            UserEntity user = new UserEntity()
            {
                UserName = form["username"],
                NickName = form["nickname"],
                School = form["school"],
                Email = form["email"]
            };

            if (!UserManager.TrySignUp(user, form["password"], form["password2"], form["checkcode"], out result))
            {
                throw new OperationFailedException(result);
            }

            if (!UserManager.TrySignIn(form["username"], form["password"], out result))
            {
                throw new OperationFailedException(result);
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
            String result;
            UserEntity user = new UserEntity()
            {
                NickName = form["nickname"],
                School = form["school"],
                Email = form["email"]
            };

            if (!UserManager.TryUpdateUserInfo(user, form["password"], form["newpassword"], form["newpassword2"], out result))
            {
                throw new OperationFailedException(result);
            }

            return RedirectToSuccessMessagePage("Your user profile was updated successfully!");
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
            String link = Url.Action("ResetPassword", "User") + "?rid=";

            if (!await UserForgetPasswordManager.RequestResetUserPassword(form["username"], form["email"], form["checkcode"], link))
            {
                throw new OperationFailedException("Failed to send a password reset link to your email address.");
            }

            return RedirectToSuccessMessagePage("We have sent a password reset link to your email address, the link is valid for 24 hours.");
        }

        /// <summary>
        /// 用户重设密码页面
        /// </summary>
        /// <param name="rid">重设密码ID</param>
        /// <returns>操作后的结果</returns>
        [Function(PageType.ForgetPassword)]
        public ActionResult ResetPassword(String rid)
        {
            UserForgetPasswordManager.GetUserName(rid);

            ViewBag.RequestID = rid;

            return View();
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
            if (!UserForgetPasswordManager.ResetUserPassword(form["rid"], form["username"], form["password"], form["password2"]))
            {
                throw new OperationFailedException("Failed to update your password!");
            }

            return RedirectToSuccessMessagePage("Your password was updated successfully!");
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