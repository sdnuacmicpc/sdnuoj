using System;
using System.Runtime.Serialization;

namespace SDNUOJ.Storage.FreeProblemSet.Exception
{
    /// <summary>
    /// Free Problem Set异常抽象类
    /// </summary>
    [Serializable]
    public abstract class FreeProblemSetException : System.Exception, ISerializable
    {
        #region 字段
        protected String _newmessage;
        #endregion

        #region 属性
        /// <summary>
        /// 获取描述当前异常的消息
        /// </summary>
        public override String Message
        {
            get { return this._newmessage; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化新的异常
        /// </summary>
        public FreeProblemSetException(String message)
        {
            this._newmessage = message;
        }
        #endregion
    }
}