using System;
using System.Collections.Generic;

using SDNUOJ.Entity;

namespace SDNUOJ.Caching
{
    /// <summary>
    /// 公告信息缓存类
    /// </summary>
    public static class NewsCache
    {
        #region 最近公告信息列表
        /// <summary>
        /// 缓存中存储公告信息的KEY
        /// </summary>
        private const String NEWS_LASTLIST_CACHE_KEY = "oj.n.lastest";

        /// <summary>
        /// 向缓存中写入最近公告信息
        /// </summary>
        /// <param name="list">最近公告信息</param>
        public static void SetLastestNewsListCache(List<NewsEntity> list)
        {
            if (list != null) CacheManager.Set(NEWS_LASTLIST_CACHE_KEY, list);
        }

        /// <summary>
        /// 从缓存中读取最近公告信息
        /// </summary>
        /// <returns>最近公告信息</returns>
        public static List<NewsEntity> GetLastestNewsListCache()
        {
            return CacheManager.Get<List<NewsEntity>>(NEWS_LASTLIST_CACHE_KEY);
        }

        /// <summary>
        /// 从缓存中删除最近公告信息
        /// </summary>
        public static void RemoveLastestNewsListCache()
        {
            CacheManager.Remove(NEWS_LASTLIST_CACHE_KEY);
        }
        #endregion

        #region 指定公告信息
        /// <summary>
        /// 缓存中存储指定公告的KEY
        /// </summary>
        private const String NEWS_CACHE_KEY = "oj.n.item";

        /// <summary>
        /// 向缓存中写入指定公告信息
        /// </summary>
        /// <param name="news">指定公告信息</param>
        public static void SetNewsCache(NewsEntity news)
        {
            if (news != null) CacheManager.Set(GetNewsCacheKey(news.AnnounceID), news);
        }

        /// <summary>
        /// 从缓存中读取指定公告信息
        /// </summary>
        /// <param name="id">公告ID</param>
        /// <returns>指定公告信息</returns>
        public static NewsEntity GetNewsCache(Int32 id)
        {
            return CacheManager.Get<NewsEntity>(GetNewsCacheKey(id));
        }

        /// <summary>
        /// 从缓存中删除指定公告信息
        /// </summary>
        public static void RemoveNewsCache(Int32 id)
        {
            CacheManager.Remove(GetNewsCacheKey(id));
        }

        /// <summary>
        /// 获取指定公告缓存KEY
        /// </summary>
        /// <param name="id">指定公告ID</param>
        /// <returns>缓存KEY</returns>
        private static String GetNewsCacheKey(Int32 id)
        {
            return String.Format("{0}:id={1}", NEWS_CACHE_KEY, id);
        }
        #endregion

        #region 公告总数
        /// <summary>
        /// 缓存中存储公告总数的KEY
        /// </summary>
        private const String NEWS_COUNT_CACHE_KEY = "oj.n.count";

        /// <summary>
        /// 向缓存中写入公告总数
        /// </summary>
        /// <param name="count">公告总数</param>
        public static void SetNewsCountCache(Int32 count)
        {
            CacheManager.Set(NEWS_COUNT_CACHE_KEY, count);
        }

        /// <summary>
        /// 从缓存中读取公告总数
        /// </summary>
        /// <returns>公告总数</returns>
        public static Int32 GetNewsCountCache()
        {
            return CacheManager.GetInt32(NEWS_COUNT_CACHE_KEY);
        }

        /// <summary>
        /// 从缓存中删除公告总数
        /// </summary>
        public static void RemoveNewsCountCache()
        {
            CacheManager.Remove(NEWS_COUNT_CACHE_KEY);
        }
        #endregion
    }
}