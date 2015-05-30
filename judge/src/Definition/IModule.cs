using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudgeClient.Definition
{
    public interface IModule
    {
        void Configure(IProfile Profile);
    }
}
