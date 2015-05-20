using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 竞赛题目实体类
    /// </summary>
    [Serializable]
    public class ContestProblemEntity
    {
        /// <summary>
        /// 获取或设置竞赛ID
        /// </summary>
        public Int32 ContestID { get; set; }

        /// <summary>
        /// 获取或设置题目ID
        /// </summary>
        public Int32 ProblemID { get; set; }

        /// <summary>
        /// 获取或设置竞赛题目ID
        /// </summary>
        public Int32 ContestProblemID { get; set; }
    }
}