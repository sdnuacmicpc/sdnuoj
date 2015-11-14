using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using SDNUOJ.Caching;
using SDNUOJ.Configuration;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 最近比赛业务逻辑层
    /// </summary>
    internal static class RecentContestManager
    {
        #region 常量
        /// <summary>
        /// 定时自动获取间隔时间
        /// </summary>
        private const Int32 AUTO_REQUEST_INTERVAL = 1800;//s = 30min

        /// <summary>
        /// 获取信息超时时间
        /// </summary>
        private const Int32 REQUEST_TIMEOUT = 20000;//ms
        #endregion

        #region 用户方法
        /// <summary>
        /// 根据比赛信息地址获取最近比赛信息
        /// </summary>
        /// <param name="url">比赛信息地址</param>
        /// <returns>最近比赛信息</returns>
        public static async Task<String> GetAllRecentContestsJsonAsync()
        {
            String content = RecentContestCache.GetRecentContestCache();//获取缓存

            if (String.IsNullOrEmpty(content))
            {
                content = await RecentContestManager.GetAllRecentContestsJsonFromWebAsync();
                RecentContestCache.SetRecentContestCache(content);//设置缓存
            }

            return content;
        }
        #endregion

        #region 定时方法
        /// <summary>
        /// 定时获取比赛信息
        /// </summary>
        /// <returns>异步任务</returns>
        public static void ScheduleGetAllRecentContestsJsonFromWeb()
        {
            ScheduledTaskManager.Schedule("RequestRecentContest", 0, AUTO_REQUEST_INTERVAL, 
                RecentContestManager.ScheduleGetAllRecentContestsJsonCallback);
        }
        #endregion

        #region 私有方法
        private static void ScheduleGetAllRecentContestsJsonCallback(Object state)
        {
            try
            {
                HttpWebRequest request = HttpWebRequest.CreateHttp(ConfigurationManager.RecentContestURL);
                request.Timeout = REQUEST_TIMEOUT;
                request.BeginGetResponse(ar => 
                {
                    HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);

                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader sr = new StreamReader(stream);
                        String content = RecentContestManager.ProcessRecentContestsJsonContent(sr.ReadToEnd());

                        if (!String.IsNullOrEmpty(content))
                        {
                            RecentContestCache.SetRecentContestCache(content);//设置缓存
                        }
                    }
                }, request);
            }
            catch (System.Exception)
            {
                ;//Do Nothing
            }
        }

        private static async Task<String> GetAllRecentContestsJsonFromWebAsync()
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(REQUEST_TIMEOUT);

            String content = await client.GetStringAsync(ConfigurationManager.RecentContestURL);
            return RecentContestManager.ProcessRecentContestsJsonContent(content);
        }

        private static String ProcessRecentContestsJsonContent(String content)
        {
            content = content.Replace("\"Rigister\"", "\"Register\"");//修正HDU Rigister
            content = content.Replace("\"Open\"", "\"Public\"");//修正XMU Open
            content = content.Replace("\"open\"", "\"Public\"");//修正JLU open
            content = content.Replace("\"\\u221a\"", "\"Public\"");//修正NBUT √
            content = content.Replace("\"PUBLIC\"", "\"Public\"");//修正HUNNU PUBLIC
            content = content.Replace("\"PRIVATE\"", "\"Public\"");//修正HUNNU PRIVATE

            return content;
        }
        #endregion
    }
}