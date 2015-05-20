using System;


namespace SDNUOJ.Caching
{
    /// <summary>
    /// 最近比赛缓存类
    /// </summary>
    public static class RecentContestCache
    {
        #region 最近比赛信息
        /// <summary>
        /// 缓存中存储最近比赛信息的KEY
        /// </summary>
        private const String RECENTCONTEST_CACHE_KEY = "oj.c.recent";

        /// <summary>
        /// 缓存时间
        /// </summary>
        private const Int32 RECENTCONTEST_CACHE_TIME = 600;

        /// <summary>
        /// 向缓存中写入最近比赛信息
        /// </summary>
        /// <param name="recentContest">最近比赛信息</param>
        public static void SetRecentContestCache(String recentContest)
        {
            if (!String.IsNullOrEmpty(recentContest))
            {
                CacheManager.Set(RECENTCONTEST_CACHE_KEY, recentContest);
            }
        }

        /// <summary>
        /// 从缓存中读取最近比赛信息
        /// </summary>
        /// <returns>最近比赛信息</returns>
        public static String GetRecentContestCache()
        {
            return CacheManager.Get<String>(RECENTCONTEST_CACHE_KEY);
        }
        #endregion
    }
}