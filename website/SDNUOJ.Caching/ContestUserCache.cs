using System;
using System.Collections.Generic;

using SDNUOJ.Entity;

namespace SDNUOJ.Caching
{
    /// <summary>
    /// 竞赛用户缓存类
    /// </summary>
    public static class ContestUserCache
    {
        #region 竞赛用户列表信息
        /// <summary>
        /// 缓存中存储指定用户列表的KEY
        /// </summary>
        private const String CONTEST_USER_SET_CACHE_KEY = "oj.c.user";

        /// <summary>
        /// 向缓存中写入指定竞赛用户列表信息
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="dict">竞赛用户列表信息</param>
        public static void SetContestUserListCache(Int32 cid, Dictionary<String, ContestUserEntity> dict)
        {
            if (dict != null) CacheManager.Set(GetContestUserListCacheKey(cid), dict);
        }

        /// <summary>
        /// 从缓存中读取指定竞赛用户列表信息
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>竞赛用户列表信息</returns>
        public static Dictionary<String, ContestUserEntity> GetContestUserListCache(Int32 cid)
        {
            return CacheManager.Get<Dictionary<String, ContestUserEntity>>(GetContestUserListCacheKey(cid));
        }

        /// <summary>
        /// 从缓存中删除指定竞赛用户列表信息
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        public static void RemoveContestUserListCache(Int32 cid)
        {
            CacheManager.Remove(GetContestUserListCacheKey(cid));
        }

        /// <summary>
        /// 获取指定竞赛用户列表缓存KEY
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>缓存KEY</returns>
        private static String GetContestUserListCacheKey(Int32 cid)
        {
            return String.Format("{0}:cid=", CONTEST_USER_SET_CACHE_KEY, cid);
        }
        #endregion
    }
}