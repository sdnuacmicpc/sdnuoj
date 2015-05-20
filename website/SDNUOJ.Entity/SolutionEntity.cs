using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 提交实体类
    /// </summary>
    [Serializable]
    public class SolutionEntity
    {
        /// <summary>
        /// 获取或设置提交ID
        /// </summary>
        public Int32 SolutionID { get; set; }

        /// <summary>
        /// 获取或设置题目ID
        /// </summary>
        public Int32 ProblemID { get; set; }

        /// <summary>
        /// 获取或设置用户名
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// 获取或设置源代码
        /// </summary>
        public String SourceCode { get; set; }

        /// <summary>
        /// 获取或设置语言类型
        /// </summary>
        public LanguageType LanguageType { get; set; }

        /// <summary>
        /// 获取或设置结果类型
        /// </summary>
        public ResultType Result { get; set; }

        /// <summary>
        /// 获取或设置代码长度(Byte)
        /// </summary>
        public Int32 CodeLength { get; set; }

        /// <summary>
        /// 获取或设置竞赛ID
        /// </summary>
        public Int32 ContestID { get; set; }

        /// <summary>
        /// 获取或设置竞赛题目ID
        /// </summary>
        public Int32 ContestProblemID { get; set; }

        /// <summary>
        /// 获取或设置花费时间(MS)
        /// </summary>
        public Int32 TimeCost { get; set; }

        /// <summary>
        /// 获取或设置占用内存(KB)
        /// </summary>
        public Int32 MemoryCost { get; set; }

        /// <summary>
        /// 获取或设置提交时间
        /// </summary>
        public DateTime SubmitTime { get; set; }

        /// <summary>
        /// 获取或设置评测时间
        /// </summary>
        public DateTime JudgeTime { get; set; }

        /// <summary>
        /// 获取或设提交IP
        /// </summary>
        public String SubmitIP { get; set; }

        /// <summary>
        /// 获取用来输出的结果
        /// </summary>
        public virtual String ResultString
        {
            get
            {
                switch (this.Result)
                {
                    case ResultType.Pending: return "Pending";
                    case ResultType.RejudgePending: return "Rejudge Pending";
                    case ResultType.Judging: return "Judging";
                    case ResultType.CompileError: return "Compile Error";
                    case ResultType.RuntimeError: return "Runtime Error";
                    case ResultType.TimeLimitExceeded: return "Time Limit Exceeded";
                    case ResultType.MemoryLimitExceeded: return "Memory Limit Exceeded";
                    case ResultType.OutputLimitExceeded: return "Output Limit Exceeded";
                    case ResultType.WrongAnswer: return "Wrong Answer";
                    case ResultType.PresentationError: return "Presentation Error";
                    case ResultType.Accepted: return "Accepted";
                    case ResultType.JudgeFailed: return "Judge Failed";
                    default: return String.Empty;
                }
            }
        }
    }
}