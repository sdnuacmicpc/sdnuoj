using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudgeClient.Definition
{
    [Serializable]
    public class JudgerProfile : IProfile
    {
        public string Type { get; set; }
        public string SourceCodeFileName { get; set; }
        public string CompilerPath { get; set; }
        public string CompilerWorkingDirectory { get; set; }
        public string CompileParameters { get; set; }
        public int CompilerWaitTime { get; set; }
        public string RunnerFileName { get; set; }
        public string RunnerParameters { get; set; }
        public string RunnerWorkingDirectory { get; set; }
        public string Language { get; set; }
        public string Special { get; set; }
        public double TimeLimitScale { get; set; }
        public int OutputLimit { get; set; }
        public string JudgeDirectory { get; set; }
        public bool SeeNoCompilerAsCompileError { get; set; }
        public bool NeedCompile { get; set; }
        public double TimeCompensation { get; set; }
    }
}
