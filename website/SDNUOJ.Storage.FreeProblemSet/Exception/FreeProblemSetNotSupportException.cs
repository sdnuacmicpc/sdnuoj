using System;
using System.Runtime.Serialization;

namespace SDNUOJ.Storage.FreeProblemSet.Exception
{
    /// <summary>
    /// Free Problem Set不支持异常
    /// </summary>
    [Serializable]
    public class FreeProblemSetNotSupportException : FreeProblemSetException
    {
        #region 方法
        /// <summary>
        /// 初始化新的异常
        /// </summary>
        public FreeProblemSetNotSupportException() : base("This free problem set is not supported!") { }
        #endregion
    }
}