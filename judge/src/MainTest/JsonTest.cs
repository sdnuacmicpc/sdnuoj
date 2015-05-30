using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Burst.Json;

using JudgeClient.Definition;
using JudgeClient.Fetcher;

namespace MainTest
{
    [TestClass]
    public class JsonTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var _parser_dic = new Dictionary<Type, JsonObjectParserBase>();
            _parser_dic.Add(typeof(Task), new TaskJsonParser());
            //_parser_dic.Add(typeof(Problem), new ProblemJsonParser());

            var res = JsonUtils.ParseAs<List<Task>>("[{'id':2,'problem':{'id':34,'last_modified_date':'2012-1-1'},'language_and_special':'g++[]'}]", _parser_dic);
            Assert.AreEqual(2, res[0].Id);
        }
    }
}
