using System;
using System.Web;
using System.Web.Security;

namespace SDNUOJ.Utilities.Web
{
    /// <summary>
    /// Cookies操作封装类
    /// </summary>
    public static class Cookies
    {
        /// <summary>
        /// 判断Cookie变量是否为空
        /// </summary>
        /// <param name="key">Cookie变量名</param>
        /// <returns>返回Cookie变量是否为空</returns>
        public static Boolean IsEmpty(String key)
        {
            return (Cookies.GetCookie(key) == null);
        }

        /// <summary>
        /// 设置Cookie变量
        /// </summary>
        /// <param name="key">Cookie变量名</param>
        /// <param name="value">Cookie变量值</param>
        public static void SetValue(String key, String value)
        {
            HttpCookie cookie = new HttpCookie(key, value);
            Cookies.SetCookie(cookie);
        }

        /// <summary>
        /// 设置Cookie变量
        /// </summary>
        /// <param name="key">Cookie变量名</param>
        /// <param name="value">Cookie变量值</param>
        /// <param name="expires">Cookie生存时间</param>
        public static void SetValue(String key, String value, DateTime expires)//重载
        {
            HttpCookie cookie = new HttpCookie(key, value);
            cookie.Expires = expires;

            Cookies.SetCookie(cookie);
        }

        /// <summary>
        /// 设置Cookie变量
        /// </summary>
        /// <param name="key">Cookie变量名</param>
        /// <param name="value">Cookie变量值</param>
        /// <param name="httpOnly">是否只能服务器端访问</param>
        public static void SetValue(String key, String value, Boolean httpOnly)
        {
            HttpCookie cookie = new HttpCookie(key, value);
            cookie.HttpOnly = httpOnly;

            Cookies.SetCookie(cookie);
        }

        /// <summary>
        /// 设置Cookie变量
        /// </summary>
        /// <param name="key">Cookie变量名</param>
        /// <param name="value">Cookie变量值</param>
        /// <param name="httpOnly">是否只能服务器端访问</param>
        /// <param name="expires">Cookie生存时间</param>
        public static void SetValue(String key, String value, Boolean httpOnly, DateTime expires)
        {
            HttpCookie cookie = new HttpCookie(key, value);
            cookie.HttpOnly = httpOnly;
            cookie.Expires = expires;

            Cookies.SetCookie(cookie);
        }

        /// <summary>
        /// 设置Cookie变量
        /// </summary>
        /// <param name="cookie">Cookie</param>
        public static void SetCookie(HttpCookie cookie)
        {
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            cookie.Domain = FormsAuthentication.CookieDomain;

            HttpContext.Current.Response.Cookies.Set(cookie);
        }

        /// <summary>
        /// 获取Cookie变量值
        /// </summary>
        /// <param name="key">Cookie变量名</param>
        /// <returns>Cookie变量值</returns>
        public static String GetValue(String key)
        {
            HttpCookie cookie = GetCookie(key);
            return (cookie == null ? String.Empty : cookie.Value);
        }

        /// <summary>
        /// 获取Cookie变量
        /// </summary>
        /// <param name="key">Cookie变量名</param>
        /// <returns>Cookie变量</returns>
        public static HttpCookie GetCookie(String key)
        {
            return HttpContext.Current.Request.Cookies.Get(key);
        }

        /// <summary>
        /// 删除Cookie变量
        /// </summary>
        /// <param name="key">Cookie变量名</param>
        public static void Remove(String key)
        {
            Cookies.Remove(GetCookie(key));
        }

        /// <summary>
        /// 删除Cookie变量
        /// </summary>
        /// <param name="cookie">Cookie变量</param>
        public static void Remove(HttpCookie cookie)
        {
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                Cookies.SetCookie(cookie);
            }
        }
    }
}