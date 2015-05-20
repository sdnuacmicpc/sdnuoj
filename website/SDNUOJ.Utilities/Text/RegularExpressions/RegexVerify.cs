using System;
using System.Text.RegularExpressions;

namespace SDNUOJ.Utilities.Text.RegularExpressions
{
    /// <summary>
    /// 正则表达式验证类
    /// </summary>
    public static class RegexVerify
    {
        /// <summary>
        /// 判断给定文字是否是合法的用户名
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>给定字符串是否是合法的用户名</returns>
        public static Boolean IsUserName(String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return false;
            }

            String pattern = @"^\w+$";
            return Regex.IsMatch(s, pattern);
        }

        /// <summary>
        /// 判断给定文字是否是合法的页面名称
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>给定字符串是否是合法的页面名称</returns>
        public static Boolean IsPageName(String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return false;
            }

            String pattern = @"^\w+$";
            return Regex.IsMatch(s, pattern);
        }

        /// <summary>
        /// 判断给定文字是否是以逗号分隔的数字ID
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>给定字符串是否是以逗号分隔的数字ID</returns>
        public static Boolean IsNumericIDs(String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return false;
            }

            String pattern = @"^(\d+)(,\d+)*(,)?$";
            return Regex.IsMatch(s, pattern);
        }

        /// <summary>
        /// 判断给定文字是否是数字
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>给定字符串是否是数字</returns>
        public static Boolean IsNumeric(String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return false;
            }

            String pattern = @"^\-?[0-9]+$";
            return Regex.IsMatch(s, pattern);
        }

        /// <summary>
        /// 判断给定文字是否是URL
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>给定字符串是否是URL</returns>
        public static Boolean IsUrl(String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return false;
            }

            String pattern = @"^(http|https|ftp|rtsp|mms):(\/\/|\\\\)[A-Za-z0-9%\-_@]+\.[A-Za-z0-9%\-_@]+[A-Za-z0-9\.\/=\?%\-&_~`@:\+!;]*$";
            return Regex.IsMatch(s, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 判断给定文字是否是电子邮件地址
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>给定字符串是否是电子邮件地址</returns>
        public static Boolean IsEmail(String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return false;
            }

            String pattern = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
            return Regex.IsMatch(s, pattern);
        }

        /// <summary>
        /// 判断给定文字是否是绝对路径
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>给定字符串是否是绝对路径</returns>
        public static Boolean IsPhysicalPath(String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return false;
            }

            String pattern = @"^\s*[a-zA-Z]:.*$";
            return Regex.IsMatch(s, pattern);
        }

        /// <summary>
        /// 判断给定文字是否是相对路径
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>给定字符串是否是相对路径</returns>
        public static Boolean IsRelativePath(String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return false;
            }
            else if (s.StartsWith("/") || s.StartsWith("?"))
            {
                return false;
            }
            else if (Regex.IsMatch(s, @"^\s*[a-zA-Z]{1,10}:.*$"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 判断给定文字是否是IPv4
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>给定字符串是否是IPv4</returns>
        public static Boolean IsIPv4(String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return false;
            }

            String pattern = @"(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])";
            return Regex.IsMatch(s, pattern);
        }
    }
}