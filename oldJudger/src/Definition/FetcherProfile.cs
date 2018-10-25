using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Burst.Crypt;
using Burst.Json;

namespace JudgeClient.Definition
{
    [Serializable]
    public class FetcherProfile : IProfile
    {
        public string Type { get; set; }

        public string AuthenticationURL { get; set; }
        public string Username { get; set; }
        //[JsonIgnore]
        //[XmlIgnore]
        public string Password { get; set; }
        //private static string Key = "key : sdnu acm.";
        //public string EncryptedPassword
        //{
        //    get { return CryptUtils.EncryptDES(Password, Key); }
        //    set { Password = CryptUtils.DecryptDES(value, Key); }
        //}

        public string TaskFetchURL { get; set; }
        public string ResultSubmitURL { get; set; }
        public string DataFetchURL { get; set; }

        public int FetchInterval { get; set; }

        public int FetchTimeout { get; set; }

        public DataAccessorProfile DataAccessorProfile { get; set; }
    }
}
