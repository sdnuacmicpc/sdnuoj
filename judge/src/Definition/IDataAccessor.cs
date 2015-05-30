using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudgeClient.Definition
{
    public interface IDataAccessor : IModule
    {
        bool CheckValid(Problem Problem);
        int GetDataCount(Problem Problem);
        IEnumerator<TestData> GetDataEnumerator(Problem Problem);
        void Update(Problem Problem, IEnumerator<TestData> DataEnumerator);
    }
}
