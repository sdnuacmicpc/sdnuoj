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

        /// <summary>
        /// 对字符串中的ID进行遍历操作
        /// </summary>
        /// <param name="s">原字符串</param>
        /// <param name="separator">分隔字符</param>
        /// <param name="action">遍历操作</param>
        public static void ForEachInIDs(this String s, Char separator, Action<Int32> action)
        {
            if (String.IsNullOrEmpty(s))
            {
                return;
            }

            String[] arr = s.Split(separator);

            for (Int32 i = 0; i < arr.Length; i++)
            {
                if (String.IsNullOrEmpty(arr[i]))
                {
                    continue;
                }

                Int32 id = Convert.ToInt32(arr[i]);
                action(id);
            }
        }

        /// <summary>
        /// 对字符串中的ID进行遍历操作
        /// </summary>
        /// <param name="s">原字符串</param>
        /// <param name="separator">分隔字符</param>
        /// <param name="action">遍历操作</param>
        public static void ForEachInIDs(this String s, Char separator, Action<String> action)
        {
            if (String.IsNullOrEmpty(s))
            {
                return;
            }

            String[] arr = s.Split(separator);

            for (Int32 i = 0; i < arr.Length; i++)
            {
                if (String.IsNullOrEmpty(arr[i]))
                {
                    continue;
                }

                action(arr[i]);
            }
        }

        /// <summary>
        /// 获取分行后的字符串
        /// </summary>
        /// <param name="s">原始字符串</param>
        /// <returns>分行后的字符串</returns>
        public static String[] Lines(this String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return new String[0];
            }

            String[] lines = s.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

            return lines;
        }

        /// <summary>
        /// 获取查询优化后的字符串
        /// </summary>
        /// <param name="s">原始字符串</param>
        /// <returns>优化后的字符串</returns>
        public static String SearchOptimized(this String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return s;
            }

            s = s.Replace('，', ',');
            s = s.Replace('；', ',');
            s = s.Replace(';', ',');
            s = s.Replace(' ', ',');
            s = s.Replace('\t', ',');

            return s;
        }
    }
}