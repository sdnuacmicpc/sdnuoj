using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Burst.Json;

using JudgeClient.Definition;

namespace JudgeClient.Fetcher
{
    public class ResultJsonSerializer : JsonObjectSerializerBase
    {
        public override string SerializeToJsonString(object ori, int layer, JsonFormatOption option)
        {
            var res = ori as Result;
            return string.Format(@"{{""task"":{{""id"":{0}}},""result"":{1},""detail"":""{2}"",""time_cost"":{3},""memory_cost"":{4},""pass_rate"":{5}}}",
                res.Task.Id, (int)res.ResultCode, JsonUtils.EncodeCRLFTabQuote(res.Detail), res.TimeCost, res.MemoryCost, res.PassRate);
        }
    }
}
