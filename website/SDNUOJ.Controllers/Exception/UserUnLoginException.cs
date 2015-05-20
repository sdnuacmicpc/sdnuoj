using System;
using System.Runtime.Serialization;

namespace SDNUOJ.Controllers.Exception
{
    /// <summary>
    /// 用户未登录异常
    /// </summary>
    [Serializable]
    public class UserUnLoginException : UserException, ISerializable
    {
        #region 属性
        /// <summary>
        /// 获取是否需要记录
        /// </summary>
        public override Boolean IsNeedLog { get { return false; } }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化新的用户未登录异常
        /// </summary>
        public UserUnLoginException()
            : base("Please login first!") { }
        #endregion
    }
}