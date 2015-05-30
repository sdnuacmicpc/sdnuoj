using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudgeClient.Definition
{
    public class Result
    {
        public ResultCode ResultCode { get; set; }
        public string Detail { get; set; }
        public int TimeCost { get; set; }
        public int MemoryCost { get; set; }
        public double PassRate { get; set; }
        public Task Task { get; set; }

        public bool Submit()
        {
            return Task.Fetcher.Submit(this);
        }
    }
}
