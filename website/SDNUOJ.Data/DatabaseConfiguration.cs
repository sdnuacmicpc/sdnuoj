using System;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 数据库管理类
    /// </summary>
    public static class DatabaseConfiguration
    {
        /// <summary>
        /// 获取当前数据库类型
        /// </summary>
        public static String DataBaseType { get { return MainDatabase.Instance.DatabaseType.ToString(); } }

        /// <summary>
        /// 获取当前数据库连接字符串
        /// </summary>
        public static String DataBaseConnectionString { get { return MainDatabase.Instance.ConnectionString; } }
    }
}