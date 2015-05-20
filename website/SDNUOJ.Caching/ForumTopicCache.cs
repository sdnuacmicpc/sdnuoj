using System;

using SDNUOJ.Entity;

namespace SDNUOJ.Caching
{
    /// <summary>
    /// 论坛主题缓存类
    /// </summary>
    public static class ForumTopicCache
    {
        #region 主题总数缓存
        /// <summary>
        /// 缓存中存储主题总数的KEY
        /// </summary>
        private const String FORUM_TOPIC_COUNT_CACHE_KEY = "oj.f.count";

        /// <summary>
        /// 缓存时间
        /// </summary>
        private const Int32 FORUM_TOPIC_COUNT_CACHE_TIME = 10;

        /// <summary>
        /// 向缓存中写入主题总数
        /// </summary>
        /// <param name="type">主题类型</param>
        /// <param name="relativeID">相关ID</param>
        /// <param name="count">主题总数</param>
        public static void SetForumTopicCountCache(ForumTopicType type, Int32 relativeID, Int32 count)
        {
            CacheManager.Set(GetForumTopicCountCacheKey(type, relativeID), count, FORUM_TOPIC_COUNT_CACHE_TIME);
        }

        /// <summary>
        /// 从缓存中读取主题总数
        /// </summary>
        /// <param name="type">主题类型</param>
        /// <param name="relativeID">相关ID</param>
        /// <returns>主题总数</returns>
        public static Int32 GetForumTopicCountCache(ForumTopicType type, Int32 relativeID)
        {
            return CacheManager.GetInt32(GetForumTopicCountCacheKey(type, relativeID));
        }

        /// <summary>
        /// 从缓存中删除主题总数
        /// </summary>
        /// <param name="type">主题类型</param>
        /// <param name="relativeID">相关ID</param>
        public static void RemoveForumTopicCountCache(ForumTopicType type, Int32 relativeID)
        {
            CacheManager.Remove(GetForumTopicCountCacheKey(type, relativeID));
        }

        /// <summary>
        /// 增加缓存中主题总数
        /// </summary>
        /// <param name="type">主题类型</param>
        /// <param name="relativeID">相关ID</param>
        public static void IncreaseForumTopicCountCache(ForumTopicType type, Int32 relativeID)
        {
            Int32 count = GetForumTopicCountCache(type, relativeID);
            if (count > 0) SetForumTopicCountCache(type, relativeID, count + 1);
        }

        /// <summary>
        /// 获取主题总数缓存KEY
        /// </summary>
        /// <param name="type">主题类型</param>
        /// <param name="relativeID">相关ID</param>
        /// <returns>缓存KEY</returns>
        private static String GetForumTopicCountCacheKey(ForumTopicType type, Int32 relativeID)
        {
            return String.Format("{0}.{1}:id={2}", FORUM_TOPIC_COUNT_CACHE_KEY, type.ToString(), relativeID.ToString());
        }
        #endregion

        #region 指定主题信息
        /// <summary>
        /// 缓存中存储指定题目的KEY
        /// </summary>
        private const String FORUM_TOPIC_CACHE_KEY = "oj.f.topic.item";

        /// <summary>
        /// 向缓存中写入指定主题信息
        /// </summary>
        /// <param name="topic">指定主题信息</param>
        public static void SetForumTopicCache(ForumTopicEntity topic)
        {
            if (topic != null) CacheManager.Set(GetForumTopicCacheKey(topic.TopicID), topic);
        }

        /// <summary>
        /// 从缓存中读取指定主题信息
        /// </summary>
        /// <param name="id">指定主题ID</param>
        /// <returns>指定主题信息</returns>
        public static ForumTopicEntity GetForumTopicCache(Int32 id)
        {
            return CacheManager.Get<ForumTopicEntity>(GetForumTopicCacheKey(id));
        }

        /// <summary>
        /// 从缓存中删除指定主题信息
        /// </summary>
        /// <param name="id">指定主题ID</param>
        public static void RemoveForumTopicCache(Int32 id)
        {
            CacheManager.Remove(GetForumTopicCacheKey(id));
        }

        /// <summary>
        /// 获取指定主题缓存KEY
        /// </summary>
        /// <param name="id">指定主题ID</param>
        /// <returns>缓存KEY</returns>
        private static String GetForumTopicCacheKey(Int32 id)
        {
            return String.Format("{0}:id={1}", FORUM_TOPIC_CACHE_KEY, id);
        }
        #endregion
    }
}