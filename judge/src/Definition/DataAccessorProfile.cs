using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JudgeClient.Definition
{
    public class DataAccessorProfile : IProfile
    {
        public string Type { get; set; }
        public string TestDataSavePath { get; set; }
    }
}
