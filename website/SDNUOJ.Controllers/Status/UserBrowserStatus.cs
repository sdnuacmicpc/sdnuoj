using System;
using System.Web.Security;

using SDNUOJ.Entity;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Controllers.Status
{
    /// <summary>
    /// 用户浏览器状态类
    /// </summary>
    public static class UserBrowserStatus
    {
        #region 常量
        public static readonly String BROWSER_STATUS_COOKIE_NAME = FormsAuthentication.FormsCookieName + "bs";
        public static readonly Int32 BROWSER_STATUS_STORE_COUNT = 3;
        #endregion

        #region 用户本地状态(仅浏览器使用)
        /// <summary>
        /// 向缓存中写入当前用户辅助状态
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="permission">用户权限</param>
        /// <param name="unreadMailCount">未读邮件数量</param>
        public static void SetCurrentUserBrowserStatus(String userName, PermissionType permission, Int32 unreadMailCount)
        {
            String cookieBrowserStatus = Cookies.GetValue(BROWSER_STATUS_COOKIE_NAME);

            String newUserName = userName;
            String newUserPermission = ((Int32)permission).ToString();
            String newUserUnReadMail = unreadMailCount.ToString();
            String newBrowserStatus = String.Format("{0}|{1}|{2}", newUserName, newUserPermission, newUserUnReadMail);

            if (!String.Equals(newBrowserStatus, cookieBrowserStatus, StringComparison.OrdinalIgnoreCase))
            {
                Cookies.SetValue(BROWSER_STATUS_COOKIE_NAME, newBrowserStatus, false);
            }
        }

        /// <summary>
        /// 从缓存中删除辅助状态
        /// </summary>
        public static void RemoveCurrentUserBrowserStatus()
        {
            String cookieUserName = Cookies.GetValue(BROWSER_STATUS_COOKIE_NAME);

            if (!String.IsNullOrEmpty(cookieUserName))
            {
                Cookies.Remove(BROWSER_STATUS_COOKIE_NAME);
            }
        }
        #endregion
    }
}