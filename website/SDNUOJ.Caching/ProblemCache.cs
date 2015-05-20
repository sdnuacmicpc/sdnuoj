using System;
using System.Collections.Generic;

using SDNUOJ.Entity;

namespace SDNUOJ.Caching
{
    /// <summary>
    /// 题目信息缓存类
    /// </summary>
    public static class ProblemCache
    {
        #region 题目总数缓存
        /// <summary>
        /// 缓存中存储题目总数的KEY
        /// </summary>
        private const String PROBLEM_COUNT_CACHE_KEY = "oj.p.cnt";

        /// <summary>
        /// 向缓存中写入题目总数
        /// </summary>
        /// <param name="count">题目总数</param>
        public static void SetProblemSetCountCache(Int32 count)
        {
            CacheManager.Set(PROBLEM_COUNT_CACHE_KEY, count);
        }

        /// <summary>
        /// 从缓存中读取题目总数
        /// </summary>
        /// <returns>题目总数</returns>
        public static Int32 GetProblemSetCountCache()
        {
            return CacheManager.GetInt32(PROBLEM_COUNT_CACHE_KEY);
        }

        /// <summary>
        /// 更新缓存中题目总数
        /// </summary>
        public static void IncreaseProblemSetCountCache()
        {
            Int32 count = GetProblemSetCountCache();
            if (count > 0) SetProblemSetCountCache(count + 1);
        }

        /// <summary>
        /// 从缓存中删除题目总数
        /// </summary>
        public static void RemoveProblemSetCountCache()
        {
            CacheManager.Remove(PROBLEM_COUNT_CACHE_KEY);
        }
        #endregion

        #region 题目ID最大值缓存
        /// <summary>
        /// 缓存中存储题目ID最大值的KEY
        /// </summary>
        private const String PROBLEM_MAX_CACHE_KEY = "oj.p.max";

        /// <summary>
        /// 向缓存中写入题目ID最大值
        /// </summary>
        /// <param name="max">题目ID最大值</param>
        public static void SetProblemIDMaxCache(Int32 max)
        {
            CacheManager.Set(PROBLEM_MAX_CACHE_KEY, max);
        }

        /// <summary>
        /// 从缓存中读取题目ID最大值
        /// </summary>
        /// <returns>题目ID最大值</returns>
        public static Int32 GetProblemIDMaxCache()
        {
            return CacheManager.GetInt32(PROBLEM_MAX_CACHE_KEY);
        }

        /// <summary>
        /// 更新缓存中题目ID最大值
        /// </summary>
        public static void IncreaseProblemIDMaxCache()
        {
            Int32 max = GetProblemIDMaxCache();
            if (max > 0) SetProblemIDMaxCache(max + 1);
        }

        /// <summary>
        /// 从缓存中删除题目ID最大值
        /// </summary>
        public static void RemoveProblemIDMaxCache()
        {
            CacheManager.Remove(PROBLEM_MAX_CACHE_KEY);
        }
        #endregion

        #region 题目列表信息
        /// <summary>
        /// 缓存中存储指定题目的KEY
        /// </summary>
        private const String PROBLEM_SET_CACHE_KEY = "oj.p.page";

        /// <summary>
        /// 缓存时间
        /// </summary>
        private const Int32 PROBLEM_SET_CACHE_TIME = 30;

        /// <summary>
        /// 向缓存中写入指定题目列表信息
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="list">题目列表信息</param>
        public static void SetProblemSetCache(Int32 pageIndex, List<ProblemEntity> list)
        {
            if (list != null) CacheManager.Set(GetProblemSetCacheKey(pageIndex), list, PROBLEM_SET_CACHE_TIME);
        }

        /// <summary>
        /// 从缓存中读取指定题目列表信息
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>题目列表信息</returns>
        public static List<ProblemEntity> GetProblemSetCache(Int32 pageIndex)
        {
            return CacheManager.Get<List<ProblemEntity>>(GetProblemSetCacheKey(pageIndex));
        }

        /// <summary>
        /// 从缓存中删除指定题目列表信息
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        public static void RemoveProblemSetCache(Int32 pageIndex)
        {
            CacheManager.Remove(GetProblemSetCacheKey(pageIndex));
        }

        /// <summary>
        /// 获取指定题目缓存KEY
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>缓存KEY</returns>
        private static String GetProblemSetCacheKey(Int32 pageIndex)
        {
            return String.Format("{0}:{1}", PROBLEM_SET_CACHE_KEY, pageIndex);
        }
        #endregion

        #region 指定题目信息
        /// <summary>
        /// 缓存中存储指定题目的KEY
        /// </summary>
        private const String PROBLEM_CACHE_KEY = "oj.p.item";

        /// <summary>
        /// 缓存时间
        /// </summary>
        private const Int32 PROBLEM_CACHE_TIME = 30;

        /// <summary>
        /// 向缓存中写入指定题目信息
        /// </summary>
        /// <param name="problem">指定题目信息</param>
        public static void SetProblemCache(ProblemEntity problem)
        {
            if (problem != null) CacheManager.Set(GetProblemCacheKey(problem.ProblemID), problem, PROBLEM_CACHE_TIME);
        }

        /// <summary>
        /// 从缓存中读取指定题目信息
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <returns>指定题目信息</returns>
        public static ProblemEntity GetProblemCache(Int32 id)
        {
            return CacheManager.Get<ProblemEntity>(GetProblemCacheKey(id));
        }

        /// <summary>
        /// 从缓存中删除指定题目信息
        /// </summary>
        /// <param name="id">题目ID</param>
        public static void RemoveProblemCache(Int32 id)
        {
            CacheManager.Remove(GetProblemCacheKey(id));
        }

        /// <summary>
        /// 更新缓存中题目提交数
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <param name="count">提交数(自增为-1)</param>
        public static void UpdateProblemCacheSubmitCount(Int32 id, Int32 count)
        {
            ProblemEntity problem = GetProblemCache(id);
            if (problem != null)
            {
                problem.SubmitCount = (count < 0 ? problem.SubmitCount + 1 : count);
                SetProblemCache(problem);
            }
        }

        /// <summary>
        /// 更新缓存中题目通过数
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <param name="count">通过数(自增为-1)</param>
        public static void UpdateProblemCacheSolvedCount(Int32 id, Int32 count)
        {
            ProblemEntity problem = GetProblemCache(id);
            if (problem != null)
            {
                problem.SolvedCount = (count < 0 ? problem.SolvedCount + 1 : count);
                SetProblemCache(problem);
            }
        }

        /// <summary>
        /// 获取指定题目缓存KEY
        /// </summary>
        /// <param name="id">指定题目ID</param>
        /// <returns>缓存KEY</returns>
        private static String GetProblemCacheKey(Int32 id)
        {
            return String.Format("{0}:pid={1}", PROBLEM_CACHE_KEY, id);
        }
        #endregion
    }
}