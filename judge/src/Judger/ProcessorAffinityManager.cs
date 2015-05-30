using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudgeClient.Judger
{
    internal class ProcessorAffinityManager
    {
        private object _processor_affinity_lock = new object();
        private object _processor_affinity_modify_lock = new object();
        private int _processor_status = 0;
        internal protected ProcessorAffinityUsage GetUsage(IntPtr default_affinity)
        {
            lock (_processor_affinity_lock)
            {
                var aff = ((_processor_status + 1) & ~_processor_status & (int)default_affinity);
                while (aff == 0)
                {
                    System.Threading.Thread.Sleep(100);
                    aff = ((_processor_status + 1) & ~_processor_status & (int)default_affinity);
                }
                lock (_processor_affinity_modify_lock)
                    _processor_status |= aff;
                return new ProcessorAffinityUsage()
                {
                    Manager = this,
                    Affinity = (IntPtr)aff
                };
            }
        }
        internal protected void ReleaseProcessor(IntPtr processor_affinity)
        {
            lock (_processor_affinity_modify_lock)
            {
                _processor_status &= ~(int)processor_affinity;
            }
        }
    }
}
