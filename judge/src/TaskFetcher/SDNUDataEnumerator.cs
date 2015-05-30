using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JudgeClient.Definition;

namespace JudgeClient.SDNU
{
    public class SDNUDataEnumerator : IEnumerator<TestData>
    {
        protected int index;
        protected Func<int, TestData> callback;
        protected Action dispose;
        public SDNUDataEnumerator(Func<int, TestData> callback)
        {
            this.index = 0;
            this.callback = callback;
        }

        protected TestData current;
        public TestData Current
        {
            get { return current; }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { return current; }
        }

        public bool MoveNext()
        {
            current = callback(index++);
            return current != null;
        }

        public void Reset()
        {
            index = 0;
        }
    }
}
