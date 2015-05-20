using System;
using System.Collections.Generic;

using SDNUOJ.Entity;

namespace SDNUOJ.Caching
{
    /// <summary>
    /// 用户信息缓存类
    /// </summary>
    public static class UserCache
    {
        #region 用户TOP10列表
        /// <summary>
        /// 缓存中存储用户TOP10列表的KEY
        /// </summary>
        private const String USER_TOP10_CACHE_KEY = "oj.u.top10";

        /// <summary>
        /// 缓存时间
        /// </summary>
        private const Int32 USER_TOP10_CACHE_TIME = 60;

        /// <summary>
        /// 向缓存中写入用户TOP10列表
        /// </summary>
        /// <param name="list">用户TOP10列表</param>
        public static void SetUserTop10Cache(List<UserEntity> list)
        {
            CacheManager.Set(USER_TOP10_CACHE_KEY, list ?? new List<UserEntity>(), USER_TOP10_CACHE_TIME);
        }

        /// <summary>
        /// 从缓存中读取用户TOP10列表
        /// </summary>
        /// <returns>用户TOP10列表</returns>
        public static List<UserEntity> GetUserTop10Cache()
        {
            return CacheManager.Get<List<UserEntity>>(USER_TOP10_CACHE_KEY);
        }

        /// <summary>
        /// 从缓存中删除用户TOP10列表
        /// </summary>
        public static void RemoveUserTop10Cache()
        {
            CacheManager.Remove(USER_TOP10_CACHE_KEY);
        }
        #endregion

        #region 用户排名总数
        /// <summary>
        /// 缓存中存储用户排名总数的KEY
        /// </summary>
        private const String RANKLIST_COUNT_CACHE_KEY = "oj.u.count";

        /// <summary>
        /// 向缓存中写入用户排名总数
        /// </summary>
        /// <param name="count">用户排名总数</param>
        public static void SetRanklistUserCountCache(Int32 count)
        {
            CacheManager.Set(RANKLIST_COUNT_CACHE_KEY, count);
        }

        /// <summary>
        /// 从缓存中读取用户排名总数
        /// </summary>
        /// <returns>用户排名总数</returns>
        public static Int32 GetRanklistUserCountCache()
        {
            return CacheManager.GetInt32(RANKLIST_COUNT_CACHE_KEY);
        }

        /// <summary>
        /// 从缓存中删除用户排名总数
        /// </summary>
        public static void RemoveRanklistUserCountCache()
        {
            CacheManager.Remove(RANKLIST_COUNT_CACHE_KEY);
        }
        #endregion
    }
}