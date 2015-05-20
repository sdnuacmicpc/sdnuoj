using System;

using SDNUOJ.Entity;

namespace SDNUOJ.Caching
{
    /// <summary>
    /// 主题页面缓存类
    /// </summary>
    public static class TopicPageCache
    {
        #region 指定页面信息
        /// <summary>
        /// 缓存中存储指定页面的KEY
        /// </summary>
        private const String TOPICPAGE_CACHE_KEY = "oj.page";

        /// <summary>
        /// 向缓存中写入指定页面信息
        /// </summary>
        /// <param name="topicpage">指定页面信息</param>
        public static void SetTopicPageCache(TopicPageEntity topicpage)
        {
            if (topicpage != null) CacheManager.Set(GetTopicPageCacheKey(topicpage.PageName), topicpage);
        }

        /// <summary>
        /// 从缓存中读取指定页面信息
        /// </summary>
        /// <param name="name">页面名称</param>
        /// <returns>指定页面信息</returns>
        public static TopicPageEntity GetTopicPageCache(String name)
        {
            return CacheManager.Get<TopicPageEntity>(GetTopicPageCacheKey(name));
        }

        /// <summary>
        /// 从缓存中删除指定页面信息
        /// </summary>
        /// <param name="name">页面名称</param>
        public static void RemoveTopicPageCache(String name)
        {
            CacheManager.Remove(GetTopicPageCacheKey(name));
        }

        /// <summary>
        /// 获取指定页面缓存KEY
        /// </summary>
        /// <param name="name">页面名称</param>
        /// <returns>缓存KEY</returns>
        private static String GetTopicPageCacheKey(String name)
        {
            return String.Format("{0}.{1}", TOPICPAGE_CACHE_KEY, name);
        }
        #endregion
    }
}