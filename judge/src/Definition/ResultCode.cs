using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudgeClient.Definition
{
    public enum ResultCode
    {
        CompileError = 3,
        RuntimeError = 4,
        TimeLimitExceeded = 5,
        MemoryLimitExceeded = 6,
        OutputLimitExcceeded = 7,
        WrongAnswer = 8,
        PresentationError = 9,
        Accepted = 10,
        UnJudgable = 255
    }
}
