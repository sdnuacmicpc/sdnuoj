using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Burst.Json;

using JudgeClient.Definition;

namespace JudgeClient.Fetcher
{
    public class TaskJsonParser : JsonObjectParserBase
    {
        protected override Type GetFieldType(string field, object res, Type type)
        {
            switch (field)
            {
                case "sid":
                case "memorylimit":
                case "timelimit":
                    return typeof(int);
                case "language":
                case "sourcecode":
                case "pid":
                case "dataversion":
                case "username":
                    return typeof(string);
            }
            return typeof(object);
        }

        protected override void SetFieldValue(string field, object value, object res, Type type)
        {
            var task = res as Task;
            switch (field)
            {
                case "sid":
                    task.Id = (int)value;
                    break;
                case "memorylimit":
                    task.MemoryLimit = (int)value;
                    break;
                case "timelimit":
                    task.TimeLimit = (int)value;
                    break;
                case "pid":
                    task.Problem.Id = value as string;
                    break;
                case "language":
                    task.LanguageAndSpecial = value as string;
                    break;
                case "dataversion":
                    task.Problem.Version = value as string;
                    break;
                case "sourcecode":
                    task.SourceCode = value as string;
                    break;
                case "username":
                    task.Author = value as string;
                    break;
            }
        }

        protected override object CreateResult(Type type)
        {
            var res = new Task();
            res.Problem = new Problem();
            return res;
        }
    }
}
