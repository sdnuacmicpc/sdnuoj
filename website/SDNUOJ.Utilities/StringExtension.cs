using System;

namespace SDNUOJ.Utilities
{
    public static class StringExtension
    {
        /// <summary>
        /// 将字符串转换为指定数值
        /// </summary>
        /// <param name="s">原字符串</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns>转换后的数值</returns>
        public static Byte ToByte(this String s, Byte defaultValue)
        {
            if (String.IsNullOrEmpty(s))
            {
                return defaultValue;
            }

            Byte value = defaultValue;

            return (Byte.TryParse(s, out value) ? value : defaultValue);
        }

        /// <summary>
        /// 将字符串转换为指定数值
        /// </summary>
        /// <param name="s">原字符串</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns>转换后的数值</returns>
        public static Int32 ToInt32(this String s, Int32 defaultValue)
        {
            if (String.IsNullOrEmpty(s))
            {
                return defaultValue;
            }

            Int32 value = defaultValue;

            return (Int32.TryParse(s, out value) ? value : defaultValue);
        }

        /// <summary>
        /// 将字符串转换为指定枚举值
        /// </summary>
        /// <param name="s">原字符串</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns>转换后的枚举值</returns>
        public static T ToEnum<T>(this String s, Boolean ignoreCase, T defaultValue) where T : struct
        {
            if (String.IsNullOrEmpty(s))
            {
                return defaultValue;
            }

            T value = defaultValue;

            return (Enum.TryParse<T>(s, ignoreCase, out value) ? value : defaultValue);
        }
    }
}