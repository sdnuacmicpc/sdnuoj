using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 提交错误实体类
    /// </summary>
    [Serializable]
    public class SolutionErrorEntity
    {
        /// <summary>
        /// 获取或设置提交ID
        /// </summary>
        public Int32 SolutionID { get; set; }

        /// <summary>
        /// 获取或设置错误信息
        /// </summary>
        public String ErrorInfo { get; set; }
    }
}