using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudgeClient.Definition
{
    public interface IJudger : IModule
    {
        Result Judge(Task task);
    }
}
