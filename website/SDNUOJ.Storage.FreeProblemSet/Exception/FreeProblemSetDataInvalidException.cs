using System;
using System.Runtime.Serialization;

namespace SDNUOJ.Storage.FreeProblemSet.Exception
{
    /// <summary>
    /// Free Problem Set题目数据无效异常
    /// </summary>
    [Serializable]
    public class FreeProblemSetDataInvalidException : FreeProblemSetException
    {
        #region 方法
        /// <summary>
        /// 初始化新的异常
        /// </summary>
        public FreeProblemSetDataInvalidException() : base("The problem data in this free problem set is INVALID!") { }
        #endregion
    }
}