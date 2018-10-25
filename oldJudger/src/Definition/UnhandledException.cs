using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudgeClient.Definition
{
    public class UnhandledException : Exception
    {
        public UnhandledException(string Message, Exception InnerException)
            : base(Message, InnerException)
        { }
    }
}
