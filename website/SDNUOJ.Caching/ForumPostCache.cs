using System;

using SDNUOJ.Entity;

namespace SDNUOJ.Caching
{
    /// <summary>
    /// 论坛帖子缓存类
    /// </summary>
    public static class ForumPostCache
    {
        #region 指定帖子信息
        /// <summary>
        /// 缓存中存储指定题目的KEY
        /// </summary>
        private const String FORUM_POST_CACHE_KEY = "oj.f.post";

        /// <summary>
        /// 向缓存中写入指定帖子信息
        /// </summary>
        /// <param name="Post">指定帖子信息</param>
        public static void SetForumPostCache(ForumPostEntity post)
        {
            if (post != null) CacheManager.Set(GetForumPostCacheKey(post.PostID), post);
        }

        /// <summary>
        /// 从缓存中读取指定帖子信息
        /// </summary>
        /// <param name="id">指定帖子ID</param>
        /// <returns>指定帖子信息</returns>
        public static ForumPostEntity GetForumPostCache(Int32 id)
        {
            return CacheManager.Get<ForumPostEntity>(GetForumPostCacheKey(id));
        }

        /// <summary>
        /// 从缓存中删除指定帖子信息
        /// </summary>
        /// <param name="id">指定帖子ID</param>
        public static void RemoveForumPostCache(Int32 id)
        {
            CacheManager.Remove(GetForumPostCacheKey(id));
        }

        /// <summary>
        /// 获取指定帖子缓存KEY
        /// </summary>
        /// <param name="id">指定帖子ID</param>
        /// <returns>缓存KEY</returns>
        private static String GetForumPostCacheKey(Int32 id)
        {
            return String.Format("{0}:id={1}", FORUM_POST_CACHE_KEY, id);
        }
        #endregion
    }
}