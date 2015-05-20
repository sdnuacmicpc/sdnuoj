using System;
using System.Runtime.Serialization;

namespace SDNUOJ.Controllers.Exception
{
    /// <summary>
    /// 系统未开放异常
    /// </summary>
    [Serializable]
    public class SystemNotOpenException : UserException, ISerializable
    {
        #region 属性
        /// <summary>
        /// 获取是否需要记录
        /// </summary>
        public override Boolean IsNeedLog { get { return false; } }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化新的系统未开放异常
        /// </summary>
        public SystemNotOpenException()
            : base("System is not open.") { }

        /// <summary>
        /// 初始化新的系统未开放异常
        /// </summary>
        /// <param name="message">提示信息</param>
        public SystemNotOpenException(String message)
            : base(message) { }
        #endregion
    }
}