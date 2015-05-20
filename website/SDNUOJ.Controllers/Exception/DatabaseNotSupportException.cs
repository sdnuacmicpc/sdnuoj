using System;
using System.Runtime.Serialization;

namespace SDNUOJ.Controllers.Exception
{
    /// <summary>
    /// 数据库不支持异常
    /// </summary>
    [Serializable]
    public class DatabaseNotSupportException : UserException, ISerializable
    {
        #region 属性
        /// <summary>
        /// 获取是否需要记录
        /// </summary>
        public override Boolean IsNeedLog { get { return false; } }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化新的数据库不支持异常
        /// </summary>
        public DatabaseNotSupportException()
            : base("Current database does not support this function!") { }
        #endregion
    }
}