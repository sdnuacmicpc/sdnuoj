using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using JudgeClient.Definition;

namespace JudgeClient.Fetcher
{
    public class TestFetcher : IFetcher
    {
        private TestDataAccessor data_accessor = new TestDataAccessor();
        public IDataAccessor DataAccessor
        {
            get { return data_accessor; }
        }

        public bool FetchData(string ProblemId)
        {
            Thread.Sleep(1000);
            return true;
        }

        public void ConfigureSupportedLanguages(IEnumerable<JudgerProfile> Judgers)
        {
        }

        public List<Task> FetchTask()
        {
            var res = new List<Task>();
            for (int i = 0; i < 5; ++i)
                res.Add(new Task()
                {
                    Fetcher = this,
                    Id = 3434,
                    LanguageAndSpecial = "g++[]",
                    MemoryLimit = 65536,
                    TimeLimit = 1000,
                    Problem = new Problem() { Id = "1000" },
                    SourceCode = "#include<iostream>\nusing namespace std;\n\nint main()\n{\nint a, b;\ncin>>a>>b;\ncout<<a + b<<endl;\nreturn 0;\n}\n"
                });
            return res;
        }

        public bool Submit(Result Result)
        {
            return true;
        }

        public void Configure(IProfile Profile)
        {
        }
    }
}
