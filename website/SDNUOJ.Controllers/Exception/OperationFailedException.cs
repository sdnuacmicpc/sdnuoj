using System;
using System.Runtime.Serialization;

namespace SDNUOJ.Controllers.Exception
{
    /// <summary>
    /// 操作失败异常
    /// </summary>
    [Serializable]
    public class OperationFailedException : UserException, ISerializable
    {
        #region 属性
        /// <summary>
        /// 获取是否需要记录
        /// </summary>
        public override Boolean IsNeedLog { get { return false; } }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化新的操作失败异常
        /// </summary>
        /// <param name="message">异常信息</param>
        public OperationFailedException(String message)
            : base(message) { }
        #endregion
    }
}