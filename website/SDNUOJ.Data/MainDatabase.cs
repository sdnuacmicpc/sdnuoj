using System;

using DotMaysWind.Data;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 数据库类
    /// </summary>
    internal static class MainDatabase
    {
        #region 字段
        private static IDatabase _database;
        #endregion

        #region 属性
        /// <summary>
        /// 获取当前数据库实例
        /// </summary>
        internal static IDatabase Instance
        {
            get { return _database; }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 初始化新的数据库类
        /// </summary>
        static MainDatabase()
        {
            _database = DatabaseFactory.CreateDatabase();
        }
        #endregion
    }
}