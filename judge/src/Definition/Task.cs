using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudgeClient.Definition
{
    public class Task
    {
        public int Id { get; set; }
        public Problem Problem { get; set; }
        public string LanguageAndSpecial { get; set; }
        public string SourceCode { get; set; }
        public string Author { get; set; }
        public int MemoryLimit { get; set; }
        public int TimeLimit { get; set; }
        public IFetcher Fetcher;
    }
}
