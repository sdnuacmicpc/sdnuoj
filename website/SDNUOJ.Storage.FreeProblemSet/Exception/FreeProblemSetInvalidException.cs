using System;
using System.Runtime.Serialization;

namespace SDNUOJ.Storage.FreeProblemSet.Exception
{
    /// <summary>
    /// Free Problem Set无效异常
    /// </summary>
    [Serializable]
    public class FreeProblemSetInvalidException : FreeProblemSetException
    {
        #region 方法
        /// <summary>
        /// 初始化新的异常
        /// </summary>
        public FreeProblemSetInvalidException() : base("This free problem set is INVALID!") { }
        #endregion
    }
}