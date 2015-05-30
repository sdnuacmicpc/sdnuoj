using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudgeClient.Definition
{
    public class CreateAndConfigureModuleException : Exception
    {
        public CreateAndConfigureModuleException(string Message, Exception InnerException)
            : base(Message, InnerException)
        { }
    }
}
