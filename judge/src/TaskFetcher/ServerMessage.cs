using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Burst.Json;

namespace JudgeClient.Fetcher
{
    public class ServerMessage : IJsonDeserializeObject
    {
        public string Message { get; set; }
        public string Status { get; set; }

        public void SetFieldValue(string fieldName, object value)
        {
            switch (fieldName)
            {
                case "status":
                    Status = value as string;
                    break;
                case "message":
                    Message = value as string;
                    break;
            }
        }

        public Type GetFieldType(string fieldName)
        {
            return typeof(string);
        }
    }
}
