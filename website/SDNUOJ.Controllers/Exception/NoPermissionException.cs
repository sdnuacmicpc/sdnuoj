using System;
using System.Runtime.Serialization;

namespace SDNUOJ.Controllers.Exception
{
    /// <summary>
    /// 没有权限异常
    /// </summary>
    [Serializable]
    public class NoPermissionException : UserException, ISerializable
    {
        #region 属性
        /// <summary>
        /// 获取是否需要记录
        /// </summary>
        public override Boolean IsNeedLog { get { return false; } }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化新的没有权限异常
        /// </summary>
        /// <param name="message">异常信息</param>
        public NoPermissionException(String message)
            : base(message) { }

        /// <summary>
        /// 初始化新的没有权限异常
        /// </summary>
        public NoPermissionException()
            : this("You do not have privileges to access this page!") { }
        #endregion
    }
}