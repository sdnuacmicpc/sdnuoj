using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudgeClient.Definition
{
    public interface IFetcher : IModule
    {
        IDataAccessor DataAccessor { get; }
        bool FetchData(string ProblemId);
        void ConfigureSupportedLanguages(IEnumerable<JudgerProfile> Judgers);
        List<Task> FetchTask();
        bool Submit(Result Result);
    }
}
