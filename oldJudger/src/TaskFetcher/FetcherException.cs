using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudgeClient.Fetcher
{
    public class FetcherException : Exception
    {
        public FetcherException(string Message, Exception InnerException)
            : base(Message, InnerException)
        { }
    }
}
