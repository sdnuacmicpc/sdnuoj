using System;
using System.Text.RegularExpressions;

namespace SDNUOJ.Utilities.Text.RegularExpressions
{
    /// <summary>
    /// 正则表达式文字提取类
    /// </summary>
    public static class RegexParser
    {
        /// <summary>
        /// 从给定字符串中获取电子邮件地址
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>电子邮件地址</returns>
        public static String ParseEmail(String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return String.Empty;
            }

            Regex regex = new Regex(@"[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+");
            Match match = regex.Match(s);
            return (match.Success ? match.Value : String.Empty);
        }
    }
}