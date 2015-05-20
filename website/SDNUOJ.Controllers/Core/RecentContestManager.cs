using System;
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
        /// 获取信息超时时间
        /// </summary>
        private const Int32 REQUEST_TIMEOUT = 10000;
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
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(REQUEST_TIMEOUT);

                content = await client.GetStringAsync(ConfigurationManager.RecentContestURL);
                content = content.Replace("\"Rigister\"", "\"Register\"");//修正HDU Rigister
                content = content.Replace("\"Open\"", "\"Public\"");//修正XMU Open
                content = content.Replace("\"open\"", "\"Public\"");//修正JLU open
                content = content.Replace("\"\\u221a\"", "\"Public\"");//修正NBUT √
                content = content.Replace("\"PUBLIC\"", "\"Public\"");//修正HUNNU PUBLIC
                content = content.Replace("\"PRIVATE\"", "\"Public\"");//修正HUNNU PRIVATE

                RecentContestCache.SetRecentContestCache(content);//设置缓存
            }

            return content;
        }
        #endregion
    }
}