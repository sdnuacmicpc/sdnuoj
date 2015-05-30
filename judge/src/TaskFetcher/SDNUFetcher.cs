using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

using Burst.Json;

using JudgeClient.Definition;
using JudgeClient.Fetcher;

namespace JudgeClient.SDNU
{
    public class SDNUFetcher : IFetcher
    {
        protected FetcherProfile _profile;
        internal FetcherProfile Profile
        {
            get { return _profile; }
        }

        protected CookieContainer _cookie_container;
        internal CookieContainer CookieContainer
        {
            get { return _cookie_container; }
        }

        protected int _task_count_per_fetch;
        protected JsonSerializer _json_serializer;

        protected void Authenticate()
        {
            var res = RequestJson<ServerMessage>(_profile.AuthenticationURL, Encoding.UTF8.GetBytes(string.Format("username={0}&password={1}", _profile.Username, _profile.Password)));
            if (res.Status != "success")
                throw new CreateAndConfigureModuleException("Authentication failed, message:\r\n" + res.Message, null);
        }

        void IModule.Configure(IProfile Profile)
        {
            try
            {
                _profile = Profile as FetcherProfile;
                _cookie_container = new CookieContainer();
                _data_accessor = Factory.CreateAndConfigure<SDNUDataAccessor>(_profile.DataAccessorProfile);
                _json_serializer = new JsonSerializer();
                _json_serializer.ParserRegistry.Add(typeof(Task), new TaskJsonParser());
                _json_serializer.SerializerRegistry.Add(typeof(Result), new ResultJsonSerializer());
                _task_count_per_fetch = Configuration.Singleton.TaskCountPerFetch;
                Authenticate();
            }
            catch (Exception ex)
            {
                ExceptionManager.Throw(new CreateAndConfigureModuleException("Configure SDNUFetcher failed.", ex));
            }
        }

        protected IDataAccessor _data_accessor;
        public IDataAccessor DataAccessor
        {
            get { return _data_accessor; }
        }

        protected HttpWebResponse Request(string url, byte[] data)
        {
            StringBuilder query = new StringBuilder();
            HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
            req.Method = "POST";
            req.CookieContainer = CookieContainer;
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = data.Length;
            req.Timeout = req.ReadWriteTimeout = _profile.FetchTimeout;
            using (var s = req.GetRequestStream())
                s.Write(data, 0, data.Length);
            return req.GetResponse() as HttpWebResponse;
        }
        protected T RequestJson<T>(string url, byte[] data)
        {
            return ReadJson<T>(Request(url, data));
        }
        protected string ReadString(HttpWebResponse response)
        {
            return EncodingUtils.DetectAndReadToEndAndDispose(response.GetResponseStream(), Encoding.UTF8);
        }
        protected T ReadJson<T>(HttpWebResponse response)
        {
            var res = ReadString(response);
            try
            {
                return _json_serializer.ParseAs<T>(res);
            }
            catch (Exception ex)
            {
                ServerMessage msg = null;
                try
                {
                    msg = _json_serializer.ParseAs<ServerMessage>(res);
                }
                catch { }
                if (msg != null)
                {
                    if (msg.Status == "error" && msg.Message == "unlogin")
                    {
                        try
                        {
                            Authenticate();
                        }
                        catch (Exception auth_ex)
                        {
                            throw new FetcherException("Reauthentication", auth_ex);
                        }
                        throw new FetcherException("Reauthentication", null);
                    }
                }
                throw new FetcherException(string.Format(
                    "Parse json as {0} failed, content:\r\n{1}\r\n", typeof(T), res), ex);
            }
        }

        public bool FetchData(string ProblemId)
        {
            try
            {
                var problem = new Problem();
                problem.Id = ProblemId;
                var res = Request(_profile.DataFetchURL, Encoding.UTF8.GetBytes("pid=" + ProblemId));
                if (res.ContentType != "application/zip")
                {
                    #if DEBUG
                    #else

                    ExceptionManager.Throw(new FetcherException(string.Format(
                        "SDNUFetcher fetch data MIME validate failed, content:\r\n{0}\r\n", ReadString(res)), null));

                    #endif
                    return false;
                }
                Dictionary<string, TestData> data_dic = new Dictionary<string, TestData>();
                MemoryStream ms = new MemoryStream();
                using (var s = res.GetResponseStream())
                {
                    byte[] buffer = new byte[2048000];
                    int len;
                    while((len = s.Read(buffer, 0, 2048000)) > 0)
                        ms.Write(buffer, 0, len);
                }
                using (var file = new ZipFile(ms))
                {
                    foreach (ZipEntry entry in file)
                    {
                        entry.IsUnicodeText = true;
                        var tags = entry.Name.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

                        string content = EncodingUtils.DetectAndReadToEndAndDispose(file.GetInputStream(entry));

                        switch (tags[0])
                        {
                            case "last_modified":
                                problem.Version = content;
                                break;
                            case "input":
                            case "output":
                                if (tags.Length > 1)
                                {
                                    var dk = Path.GetFileNameWithoutExtension(tags[1]);
                                    if (!data_dic.ContainsKey(dk))
                                        data_dic.Add(dk, new TestData() { Name = dk });
                                    if (tags[0] == "input")
                                        data_dic[dk].Input = content;
                                    else
                                        data_dic[dk].Output = content;
                                }
                                break;
                        }
                    }
                    DataAccessor.Update(problem, (from kvp in data_dic orderby kvp.Key ascending select kvp.Value).GetEnumerator());
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionManager.Throw(new FetcherException(string.Format(
                    "SDNUFetcher fetch data failed, problem id: {0}.", ProblemId), ex));
                return false;
            }
        }

        protected byte[] supported_languages;
        public void ConfigureSupportedLanguages(IEnumerable<JudgerProfile> Judgers)
        {
            StringBuilder res = new StringBuilder();
            foreach (var j in Judgers)
                res.AppendFormat("{0}[{1}],", j.Language, j.Special);
            res.Remove(res.Length - 1, 1);
            supported_languages = Encoding.UTF8.GetBytes(string.Format("count={0}&supported_languages={1}",
                _task_count_per_fetch, System.Web.HttpUtility.UrlEncode(res.ToString())));
        }
        public List<Task> FetchTask()
        {
            try
            {
                var res = RequestJson<List<Task>>(_profile.TaskFetchURL, supported_languages);
                foreach (var t in res)
                    t.Fetcher = this;
                return res;
            }
            catch (Exception ex)
            {
                ExceptionManager.Throw(new FetcherException("SDNUFetcher fetch task failed.", ex));
            }
            return new List<Task>();
        }

        public bool Submit(Result result)
        {
            try
            {
                if (result.ResultCode == ResultCode.UnJudgable)
                    ExceptionManager.Throw(new FetcherException(string.Format(
                        "Unjudgable result, task id is {0}, detail:\r\n{1}", result.Task.Id, result.Detail), null));
                var res = RequestJson<ServerMessage>(_profile.ResultSubmitURL, Encoding.UTF8.GetBytes(
                    string.Format("sid={0}&resultcode={1}&detail={2}&timecost={3}&memorycost={4}&pid={5}&username={6}",
                        result.Task.Id,
                        (int)result.ResultCode,
                        System.Web.HttpUtility.UrlEncode(result.Detail),
                        result.TimeCost,
                        result.MemoryCost,
                        result.Task.Problem.Id,
                        System.Web.HttpUtility.UrlEncode(result.Task.Author)
                    )
                ));
                if (res.Status != "success")
                    throw new FetcherException(res.Message, null);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionManager.Throw(new FetcherException("SDNUFetcher submit results failed.", ex));
                return false;
            }
        }
    }
}
