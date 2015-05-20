using System;
using System.Web;
using System.Web.Security;

using SDNUOJ.Entity;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Controllers.Status
{
    /// <summary>
    /// 用户登录状态类
    /// </summary>
    public static class UserCurrentStatus
    {
        #region 常量
        private const Int32 AUTH_COOKIE_TIME_OUT = 360;
        #endregion

        #region 用户身份替换
        /// <summary>
        /// 替换系统Form模型
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <remarks>应放在Global.asax.cs的Application_AuthenticateRequest时执行</remarks>
        public static UserStatus ReplaceFormAuthenticateModel(HttpContext context)
        {
            if (context == null || context.User == null)
            {
                UserBrowserStatus.RemoveCurrentUserBrowserStatus();
                return null;
            }

            if (!context.User.Identity.IsAuthenticated)
            {
                UserBrowserStatus.RemoveCurrentUserBrowserStatus();
                return null;
            }

            if (!(context.User.Identity is FormsIdentity))
            {
                UserBrowserStatus.RemoveCurrentUserBrowserStatus();
                return null;
            }

            HttpCookie cookie = Cookies.GetCookie(FormsAuthentication.FormsCookieName);
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

            if (ticket == null)
            {
                UserBrowserStatus.RemoveCurrentUserBrowserStatus();
                return null;
            }

            Int32 permission = 0;
            if (!String.IsNullOrEmpty(ticket.UserData)) Int32.TryParse(ticket.UserData, out permission);

            UserStatus user = new UserStatus(ticket.Name, (PermissionType)permission);
            context.User = user;

            return user;
        }
        #endregion

        #region 用户基本状态
        /// <summary>
        /// 向缓存中写入当前用户登录状态
        /// </summary>
        /// <param name="user">当前用户登录状态</param>
        public static void SetCurrentUserStatus(UserEntity user)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(UserStatus.USER_STAUTS_VERSION, user.UserName, DateTime.Now, DateTime.Now.AddMinutes(AUTH_COOKIE_TIME_OUT), false, ((Int32)user.Permission).ToString(), FormsAuthentication.FormsCookiePath);
            String hash = FormsAuthentication.Encrypt(ticket);
            
            Cookies.SetValue(FormsAuthentication.FormsCookieName, hash, true);
        }

        /// <summary>
        /// 获取当前是否登陆
        /// </summary>
        public static Boolean GetIsUserLogined()
        {
            return HttpContext.Current.Request.IsAuthenticated;
        }

        /// <summary>
        /// 获取当前登陆的用户名
        /// </summary>
        public static String GetCurrentUserName()
        {
            return HttpContext.Current.User.Identity.Name;
        }

        /// <summary>
        /// 从缓存中读取当前用户登录状态
        /// </summary>
        /// <returns>当前用户登录状态</returns>
        public static UserStatus GetCurrentUserStatus()
        {
            return HttpContext.Current.User as UserStatus;
        }

        /// <summary>
        /// 从缓存中删除当前用户登录状态
        /// </summary>
        public static void RemoveCurrentUserStatus()
        {
            FormsAuthentication.SignOut();
        }
        #endregion
    }
}