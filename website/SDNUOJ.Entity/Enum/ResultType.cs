using System;
using System.ComponentModel;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 提交结果
    /// </summary>
    public enum ResultType : byte
    {
        [Description("Pending")]                Pending             = 0,
        [Description("Rejudge Pending")]        RejudgePending      = 1,
        [Description("Judging")]                Judging             = 2,
        [Description("Compile Error")]          CompileError        = 3,
        [Description("Runtime Error")]          RuntimeError        = 4,
        [Description("Time Limit Exceeded")]    TimeLimitExceeded   = 5,
        [Description("Memory Limit Exceeded")]  MemoryLimitExceeded = 6,
        [Description("Output Limit Exceeded")]  OutputLimitExceeded = 7,
        [Description("Wrong Answer")]           WrongAnswer         = 8,
        [Description("Presentation Error")]     PresentationError   = 9,
        [Description("Accepted")]               Accepted            = 10,
        [Description("Judge Failed")]           JudgeFailed         = 255
    }
}