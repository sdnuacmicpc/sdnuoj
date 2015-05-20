using System;

namespace SDNUOJ.Utilities.Text
{
    /// <summary>
    /// 分隔辅助类
    /// </summary>
    public static class SplitHelper
    {
        /// <summary>
        /// 获取优化后的字符串
        /// </summary>
        /// <param name="origin">原始字符串</param>
        /// <returns>优化后的字符串</returns>
        public static String GetOptimizedString(String origin)
        {
            if (String.IsNullOrEmpty(origin))
            {
                return origin;
            }

            origin = origin.Replace('，', ',');
            origin = origin.Replace('；', ',');
            origin = origin.Replace(';', ',');
            origin = origin.Replace(' ', ',');
            origin = origin.Replace('\t', ',');

            return origin;
        }

        /// <summary>
        /// 获取分行后的字符串
        /// </summary>
        /// <param name="origin">原始字符串</param>
        /// <returns>分行后的字符串</returns>
        public static String[] GetLinesFromString(String origin)
        {
            String[] lines = origin.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

            return lines;
        }
    }
}