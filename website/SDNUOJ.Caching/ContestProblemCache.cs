using System;
using System.Collections.Generic;

using SDNUOJ.Entity;

namespace SDNUOJ.Caching
{
    /// <summary>
    /// 竞赛题目缓存类
    /// </summary>
    public static class ContestProblemCache
    {
        #region 竞赛题目列表
        /// <summary>
        /// 缓存中存储指定竞赛题目列表的KEY
        /// </summary>
        private const String CONTEST_PROBLEM_LIST_CACHE_KEY = "oj.c.cp";

        /// <summary>
        /// 向缓存中写入指定竞赛题目列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="list">竞赛题目列表</param>
        public static void SetContestProblemListCache(Int32 cid, List<ContestProblemEntity> list)
        {
            if (list != null) CacheManager.Set(GetContestProblemListCacheKey(cid), list);
        }

        /// <summary>
        /// 从缓存中读取指定竞赛题目列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>指定竞赛题目列表</returns>
        public static List<ContestProblemEntity> GetContestProblemListCache(Int32 cid)
        {
            return CacheManager.Get<List<ContestProblemEntity>>(GetContestProblemListCacheKey(cid));
        }

        /// <summary>
        /// 从缓存中删除指定竞赛题目列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        public static void RemoveContestProblemListCache(Int32 cid)
        {
            CacheManager.Remove(GetContestProblemListCacheKey(cid));
        }

        /// <summary>
        /// 获取指定竞赛题目列表缓存KEY
        /// </summary>
        /// <param name="cid">指定竞赛ID</param>
        /// <returns>缓存KEY</returns>
        private static String GetContestProblemListCacheKey(Int32 cid)
        {
            return String.Format("{0}:cid={1}", CONTEST_PROBLEM_LIST_CACHE_KEY, cid);
        }
        #endregion

        #region 竞赛题目列表信息
        /// <summary>
        /// 缓存中存储指定题目列表的KEY
        /// </summary>
        private const String CONTEST_PROBLEM_SET_CACHE_KEY = "oj.c.pb";

        /// <summary>
        /// 缓存时间
        /// </summary>
        private const Int32 CONTEST_PROBLEM_SET_CACHE_TIME = 10;

        /// <summary>
        /// 向缓存中写入指定竞赛题目列表信息
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="list">竞赛题目列表信息</param>
        public static void SetContestProblemSetCache(Int32 cid, List<ProblemEntity> list)
        {
            if (list != null) CacheManager.Set(GetContestProblemSetCacheKey(cid), list, CONTEST_PROBLEM_SET_CACHE_TIME);
        }

        /// <summary>
        /// 从缓存中读取指定竞赛题目列表信息
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>竞赛题目列表信息</returns>
        public static List<ProblemEntity> GetContestProblemSetCache(Int32 cid)
        {
            return CacheManager.Get<List<ProblemEntity>>(GetContestProblemSetCacheKey(cid));
        }

        /// <summary>
        /// 获取指定竞赛题目列表缓存KEY
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>缓存KEY</returns>
        private static String GetContestProblemSetCacheKey(Int32 cid)
        {
            return String.Format("{0}:cid={1}", CONTEST_PROBLEM_SET_CACHE_KEY, cid);
        }
        #endregion

        #region 指定竞赛题目信息
        /// <summary>
        /// 缓存中存储指定竞赛题目的KEY
        /// </summary>
        private const String CONTEST_PROBLEM_CACHE_KEY = "oj.c.pb.item";

        /// <summary>
        /// 缓存时间
        /// </summary>
        private const Int32 CONTEST_PROBLEM_CACHE_TIME = 10;

        /// <summary>
        /// 向缓存中写入指定竞赛题目信息
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="problem">竞赛题目信息</param>
        public static void SetContestProblemCache(Int32 cid, Int32 pid, ProblemEntity problem)
        {
            if (problem != null) CacheManager.Set(GetContestProblemCacheKey(cid, pid), problem, CONTEST_PROBLEM_CACHE_TIME);
        }

        /// <summary>
        /// 从缓存中读取指定竞赛题目信息
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <returns>竞赛题目信息</returns>
        public static ProblemEntity GetContestProblemCache(Int32 cid, Int32 pid)
        {
            return CacheManager.Get<ProblemEntity>(GetContestProblemCacheKey(cid, pid));
        }

        /// <summary>
        /// 获取指定竞赛题目信息缓存KEY
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <returns>缓存KEY</returns>
        private static String GetContestProblemCacheKey(Int32 cid, Int32 pid)
        {
            return String.Format("{0}:cid={1};pid={2}", CONTEST_PROBLEM_CACHE_KEY, cid, pid);
        }
        #endregion
    }
}