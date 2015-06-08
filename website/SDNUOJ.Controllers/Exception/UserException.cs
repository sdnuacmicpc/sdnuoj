using System;
using System.Runtime.Serialization;

namespace SDNUOJ.Controllers.Exception
{
    /// <summary>
    /// 用户异常基类
    /// </summary>
    [Serializable]
    public abstract class UserException : ApplicationException, ISerializable
    {
        #region 属性
        /// <summary>
        /// 获取是否需要记录
        /// </summary>
        public abstract Boolean IsNeedLog { get; }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化新的用户异常
        /// </summary>
        public UserException(String message)
            : base(message) { }
        #endregion
    }
}