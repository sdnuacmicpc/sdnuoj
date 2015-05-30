using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JudgeClient.Definition;

namespace JudgeClient.Fetcher
{
    public class TestDataAccessor : IDataAccessor
    {
        public int GetDataCount(Problem Problem)
        {
            return 1;
        }

        int _c_v_count = 0;
        public bool CheckValid(Problem Problem)
        {
            return _c_v_count++ % 5 != 0;
        }

        public IEnumerator<TestData> GetDataEnumerator(Problem Problem)
        {
            return (new TestData[] {
                new TestData() { Input = "1 2", Output = "3", Name = "sample" }
            } as IEnumerable<TestData>).GetEnumerator();
        }

        public void Update(Problem Problem, IEnumerator<TestData> DataEnumerator)
        {
            throw new NotImplementedException();
        }

        public void Configure(IProfile Profile)
        {
            
        }
    }
}
