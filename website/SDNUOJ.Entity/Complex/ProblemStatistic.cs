using System;

namespace SDNUOJ.Entity.Complex
{
    /// <summary>
    /// 题目统计信息实体类
    /// </summary>
    [Serializable]
    public class ProblemStatistic
    {
        /// <summary>
        /// 获取或设置题目ID
        /// </summary>
        public Int32 ProblemID { get; set; }

        /// <summary>
        /// 获取或设置用户提交数
        /// </summary>
        public Int32 SubmitCount { get; set; }

        /// <summary>
        /// 获取或设置用户通过数
        /// </summary>
        public Int32 SolvedCount { get; set; }

        /// <summary>
        /// 获取或设置Pending数
        /// </summary>
        public Int32 PendingCount { get; set; }

        /// <summary>
        /// 获取或设置Rejudge Pending数
        /// </summary>
        public Int32 RejudgePendingCount { get; set; }

        /// <summary>
        /// 获取或设置Judging数
        /// </summary>
        public Int32 JudgingCount { get; set; }

        /// <summary>
        /// 获取或设置Compile Error数
        /// </summary>
        public Int32 CompileErrorCount { get; set; }

        /// <summary>
        /// 获取或设置Runtime Error数
        /// </summary>
        public Int32 RuntimeErrorCount { get; set; }

        /// <summary>
        /// 获取或设置Time Limit Exceeded数
        /// </summary>
        public Int32 TimeLimitExceededCount { get; set; }

        /// <summary>
        /// 获取或设置Memory Limit Exceeded数
        /// </summary>
        public Int32 MemoryLimitExceededCount { get; set; }

        /// <summary>
        /// 获取或设置Output Limit Exceeded数
        /// </summary>
        public Int32 OutputLimitExceededCount { get; set; }

        /// <summary>
        /// 获取或设置Wrong Answer数
        /// </summary>
        public Int32 WrongAnswerCount { get; set; }

        /// <summary>
        /// 获取或设置Presentation Error数
        /// </summary>
        public Int32 PresentationErrorCount { get; set; }

        /// <summary>
        /// 获取或设置Accepted数
        /// </summary>
        public Int32 AcceptedCount { get; set; }
    }
}