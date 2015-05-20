using System;

namespace SDNUOJ.Utilities.Security
{
    /// <summary>
    /// SQL语句验证器
    /// </summary>
    public static class SQLValidator
    {
        #region 常量
        /// <summary>
        /// 防注入判断字符
        /// </summary>
        private static readonly String[] BADSQLWORDS = { "\"", "'", ";", "*", "%", "=", ">", "<", "insert", "update", "delete", "drop", "alter", "create", "master", "truncate", "declare" };
        #endregion

        #region 方法
        /// <summary>
        /// 判断字符串是否非空且是否安全
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>是否非空且是否安全</returns>
        public static Boolean IsNonNullANDSafe(String s)
        {
            return (!String.IsNullOrEmpty(s) && IsSQLLegal(s));
        }

        /// <summary>
        /// 判断字符串是否安全
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>是否非空且是否安全</returns>
        public static Boolean IsSafe(String s)
        {
            return SQLValidator.IsSQLLegal(s);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 判断字符串是否合法
        /// </summary>
        /// <param name="sql">输入的字符串</param>
        /// <returns>返回字符串是否合法</returns>
        private static Boolean IsSQLLegal(String sql)
        {
            String temp = sql.ToLower();

            foreach (String badWord in BADSQLWORDS)
            {
                if (temp.IndexOf(badWord) > -1) return false;
            }

            return true;
        }

        /// <summary>
        /// 通用修改注入内容
        /// </summary>
        /// <param name="sql">要修改的SQL语句</param>
        /// <returns>修改后的SQL语句</returns>
        private static String CorrectSQL(String sql)
        {
            String temp = sql.ToLower();

            foreach (String badWord in BADSQLWORDS)
            {
                temp = temp.Replace(badWord, "");
            }

            return temp;
        }
        #endregion
    }
}