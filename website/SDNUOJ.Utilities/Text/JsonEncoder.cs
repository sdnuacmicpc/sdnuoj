using System;
using System.Text;

namespace SDNUOJ.Utilities.Text
{
    /// <summary>
    /// Json编码类
    /// </summary>
    public static class JsonEncoder
    {
        /// <summary>
        /// 对Json进行编码
        /// </summary>
        /// <param name="source">待编码的字符串</param>
        /// <returns>编码后的Json代码</returns>
        public static String JsonEncode(String source)
        {
            if (String.IsNullOrEmpty(source))
            {
                return String.Empty;
            }

            StringBuilder dest = new StringBuilder();

            for (Int32 i = 0; i < source.Length; i++)
            {
                Char c = source[i];

                if (c == '\\')
                {
                    dest.Append("\\\\");
                }
                else if (c == '\n')
                {
                    dest.Append("\\n");
                }
                else if (c == '\r')
                {
                    dest.Append("\\r");
                }
                else if (c == '\t')
                {
                    dest.Append("\\t");
                }
                else if (c == '\"')
                {
                    dest.Append("\\\"");
                }
                else
                {
                    dest.Append(c);
                }
            }

            return dest.ToString();
        }

        /// <summary>
        /// 对编码过的Json进行解码
        /// </summary>
        /// <param name="source">代解码的字符串</param>
        /// <returns>解码后的Json代码</returns>
        public static String JsonDecode(String source)
        {
            if (String.IsNullOrEmpty(source))
            {
                return String.Empty;
            }

            StringBuilder dest = new StringBuilder();

            for (Int32 i = 0; i < source.Length; i++)
            {
                Char c = source[i];

                if (c == '\\')
                {
                    if (i + 1 < source.Length)
                    {
                        Char next = source[i + 1];

                        if (next == 'n')
                        {
                            dest.Append('\n');
                        }
                        else if (next == 'r')
                        {
                            dest.Append('\r');
                        }
                        else if (next == 't')
                        {
                            dest.Append('\t');
                        }
                        else if (next == '\"')
                        {
                            dest.Append('\"');
                        }
                        else if (next == '\\')
                        {
                            dest.Append('\\');
                        }
                        else
                        {
                            dest.Append(c).Append(next);
                        }

                        i++;
                    }
                    else
                    {
                        dest.Append(c);
                    }
                }
                else
                {
                    dest.Append(c);
                }
            }

            return dest.ToString();
        }
    }
}
