using System;
using System.Collections.Generic;

using SDNUOJ.Entity;
using SDNUOJ.Entity.Complex;

namespace SDNUOJ.Caching
{
    /// <summary>
    /// 竞赛信息缓存类
    /// </summary>
    public static class ContestCache
    {
        #region 竞赛总数缓存
        /// <summary>
        /// 缓存中存储竞赛总数的KEY
        /// </summary>
        private const String CONTEST_COUNT_CACHE_KEY = "oj.c.cnt";

        /// <summary>
        /// 缓存时间
        /// </summary>
        private const Int32 CONTEST_COUNT_CACHE_TIME = 60;

        /// <summary>
        /// 向缓存中写入竞赛总数
        /// </summary>
        /// <param name="passed">是否已过去的竞赛</param>
        /// <param name="count">竞赛总数</param>
        public static void SetContestListCountCache(Boolean passed, Int32 count)
        {
            CacheManager.Set(GetContestListCountCacheKey(passed), count, CONTEST_COUNT_CACHE_TIME);
        }

        /// <summary>
        /// 从缓存中读取竞赛总数
        /// </summary>
        /// <param name="passed">是否已过去的竞赛</param>
        /// <returns>竞赛总数</returns>
        public static Int32 GetContestListCountCache(Boolean passed)
        {
            return CacheManager.GetInt32(GetContestListCountCacheKey(passed));
        }

        /// <summary>
        /// 从缓存中删除竞赛总数
        /// </summary>
        /// <param name="passed">是否已过去的竞赛</param>
        public static void RemoveContestListCountCache(Boolean passed)
        {
            CacheManager.Remove(GetContestListCountCacheKey(passed));
        }

        /// <summary>
        /// 从缓存中删除所有竞赛总数
        /// </summary>
        public static void RemoveContestListCountCache()
        {
            RemoveContestListCountCache(true);
            RemoveContestListCountCache(false);
        }

        /// <summary>
        /// 获取指定竞赛排名缓存KEY
        /// </summary>
        /// <param name="id">指定竞赛ID</param>
        /// <returns>缓存KEY</returns>
        private static String GetContestListCountCacheKey(Boolean passed)
        {
            return String.Format("{0}:{1}", CONTEST_COUNT_CACHE_KEY, (passed ? "passed" : "running"));
        }
        #endregion

        #region 指定竞赛信息
        /// <summary>
        /// 缓存中存储指定竞赛的KEY
        /// </summary>
        private const String CONTEST_CACHE_KEY = "oj.c.item";

        /// <summary>
        /// 向缓存中写入指定竞赛信息
        /// </summary>
        /// <param name="contest">指定竞赛信息</param>
        public static void SetContestCache(ContestEntity contest)
        {
            if (contest != null) CacheManager.Set(GetContestCacheKey(contest.ContestID), contest);
        }

        /// <summary>
        /// 从缓存中读取指定竞赛信息
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <returns>指定竞赛信息</returns>
        public static ContestEntity GetContestCache(Int32 id)
        {
            return CacheManager.Get<ContestEntity>(GetContestCacheKey(id));
        }

        /// <summary>
        /// 从缓存中删除指定竞赛信息
        /// </summary>
        /// <param name="id">竞赛ID</param>
        public static void RemoveContestCache(Int32 id)
        {
            CacheManager.Remove(GetContestCacheKey(id));
        }

        /// <summary>
        /// 获取指定竞赛缓存KEY
        /// </summary>
        /// <param name="id">指定竞赛ID</param>
        /// <returns>缓存KEY</returns>
        private static String GetContestCacheKey(Int32 id)
        {
            return String.Format("{0}:cid={1}", CONTEST_CACHE_KEY, id);
        }
        #endregion

        #region 指定竞赛排名
        /// <summary>
        /// 缓存中存储指定竞赛排名的KEY
        /// </summary>
        private const String CONTEST_RANK_CACHE_KEY = "oj.c.rank";

        /// <summary>
        /// 缓存时间
        /// </summary>
        private const Int32 CONTEST_RANK_CACHE_TIME = 10;

        /// <summary>
        /// 向缓存中写入指定竞赛排名
        /// </summary>
        /// <param name="contestID">竞赛ID</param>
        /// <param name="rank">指定竞赛排名</param>
        public static void SetContestRankCache(Int32 contestID, List<RankItem> rank)
        {
            if (rank != null) CacheManager.Set(GetContestRankCacheKey(contestID), rank, CONTEST_RANK_CACHE_TIME);
        }

        /// <summary>
        /// 从缓存中读取指定竞赛排名
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <returns>指定竞赛排名</returns>
        public static List<RankItem> GetContestRankCache(Int32 id)
        {
            return CacheManager.Get<List<RankItem>>(GetContestRankCacheKey(id));
        }

        /// <summary>
        /// 从缓存中删除指定竞赛排名
        /// </summary>
        /// <param name="id">竞赛ID</param>
        public static void RemoveContestRankCache(Int32 id)
        {
            CacheManager.Remove(GetContestRankCacheKey(id));
        }

        /// <summary>
        /// 获取指定竞赛排名缓存KEY
        /// </summary>
        /// <param name="id">指定竞赛ID</param>
        /// <returns>缓存KEY</returns>
        private static String GetContestRankCacheKey(Int32 id)
        {
            return String.Format("{0}:cid={1}", CONTEST_RANK_CACHE_KEY, id);
        }
        #endregion
    }
}