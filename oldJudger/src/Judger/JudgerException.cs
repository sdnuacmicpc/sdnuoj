using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudgeClient.Judger
{
    public class JudgerException : Exception
    {
        public JudgerException(string Message, Exception InnerException)
            : base(Message, InnerException)
        { }
    }
}
