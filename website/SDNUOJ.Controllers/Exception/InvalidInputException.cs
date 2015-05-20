using System;
using System.Runtime.Serialization;

namespace SDNUOJ.Controllers.Exception
{
    /// <summary>
    /// 非法输入异常
    /// </summary>
    [Serializable]
    public class InvalidInputException : UserException, ISerializable
    {
        #region 属性
        /// <summary>
        /// 获取是否需要记录
        /// </summary>
        public override Boolean IsNeedLog { get { return false; } }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化新的非法输入异常
        /// </summary>
        /// <param name="message">异常信息</param>
        public InvalidInputException(String message)
            : base(message) { }
        #endregion
    }
}