using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudgeClient.Judger
{
    internal class ProcessorAffinityUsage : IDisposable
    {
        internal protected IntPtr Affinity;

        internal protected ProcessorAffinityManager Manager;

        public void Dispose()
        {
            Manager.ReleaseProcessor(Affinity);
        }
    }
}
