using System;

namespace SDNUOJ.Utilities.Text
{
    /// <summary>
    /// HTML编码类
    /// </summary>
    public static class HtmlEncoder
    {
        /// <summary>
        /// 对HTML进行编码
        /// </summary>
        /// <param name="source">待编码的字符串</param>
        /// <returns>编码后的HTML代码</returns>
        public static String HtmlEncode(String source)
        {
            return HtmlEncode(source, 0, false, false);
        }

        /// <summary>
        /// 对HTML进行编码
        /// </summary>
        /// <param name="source">待编码的字符串</param>
        /// <param name="spaceCount">替换空格需要的空格个数</param>
        /// <param name="replaceTab">是否替换Tab</param>
        /// <param name="replaceEnter">是否替换换行</param>
        /// <returns>编码后的HTML代码</returns>
        public static String HtmlEncode(String source, Int32 spaceCount, Boolean replaceTab, Boolean replaceEnter)
        {
            if (String.IsNullOrEmpty(source))
            {
                return String.Empty;
            }

            String dest = source;

            dest = dest.Replace("&", "&amp;");
            dest = dest.Replace("<", "&lt;");
            dest = dest.Replace(">", "&gt;");
            dest = dest.Replace("\"", "&quot;");

            if (spaceCount > 0)//替换空格
            {
                String srcSpace = new String(' ', spaceCount);
                String dstSpace = srcSpace.Replace(" ", "&nbsp;");
                dest = dest.Replace(srcSpace, dstSpace);
            }

            if (replaceTab)//替换TAB
            {
                dest = dest.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
            }

            if (replaceEnter)//替换回车
            {
                dest = dest.Replace(Environment.NewLine, "<br/>");
            }

            return dest;
        }

        /// <summary>
        /// 对编码过的HTML进行解码
        /// </summary>
        /// <param name="source">待解码的字符串</param>
        /// <returns>解码后的HTML代码</returns>
        public static String HtmlDecode(String source)
        {
            if (String.IsNullOrEmpty(source))
            {
                return String.Empty;
            }

            String dest = source;

            dest = dest.Replace("<br/>", Environment.NewLine);
            dest = dest.Replace("&nbsp;", " ");
            dest = dest.Replace("&quot;", "\"");
            dest = dest.Replace("&gt;", ">");
            dest = dest.Replace("&lt;", "<");
            dest = dest.Replace("&amp;", "&");
            
            return dest;
        }
    }
}