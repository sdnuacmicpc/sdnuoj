using System;
using System.Text;

namespace SDNUOJ.Utilities
{
    /// <summary>
    /// 数值扩展类
    /// </summary>
    public static class NumericExtension
    {
        /// <summary>
        /// 转换为62进制
        /// </summary>
        /// <param name="value">原数字</param>
        /// <returns>62进制数字字符串</returns>
        public static String ToSixtyTwoRadix(this Int64 value)
        {
            if (value == 0)
            {
                return "0";
            }

            StringBuilder result = new StringBuilder();

            while (value > 0)
            {
                Int64 num = value % 62;
                result.Insert(0, GetChar(num));
                value = (Int64)(value / 62);
            }

            return result.ToString();
        }

        private static Char GetChar(Int64 num)
        {
            if (0 <= num && num <= 9)
            {
                return (Char)('0' + num);
            }
            else if (10 <= num && num <= 35)
            {
                return (Char)('a' + num - 10);
            }
            else if (36 <= num && num <= 61)
            {
                return (Char)('A' + num - 36);
            }
            else
            {
                return '\0';
            }
        }
    }
}