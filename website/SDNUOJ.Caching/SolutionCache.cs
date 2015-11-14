using System;
using System.Collections.Generic;

using SDNUOJ.Entity;
using SDNUOJ.Entity.Complex;

namespace SDNUOJ.Caching
{
    /// <summary>
    /// 提交信息缓存类
    /// </summary>
    public static class SolutionCache
    {
        #region 提交总数缓存
        /// <summary>
        /// 缓存中存储提交总数的KEY
        /// </summary>
        private const String SOLUTION_COUNT_CACHE_KEY = "oj.s.cnt";

        /// <summary>
        /// 缓存时间
        /// </summary>
        private const Int32 SOLUTION_COUNT_CACHE_TIME = 10;

        /// <summary>
        /// 向缓存中写入提交总数
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="count">提交总数</param>
        public static void SetSolutionCountCache(Int32 cid, Int32 count)
        {
            CacheManager.Set(GetSolutionCountCacheKey(cid), count, SOLUTION_COUNT_CACHE_TIME);
        }

        /// <summary>
        /// 从缓存中读取提交总数
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>提交总数</returns>
        public static Int32 GetSolutionCountCache(Int32 cid)
        {
            return CacheManager.GetInt32(GetSolutionCountCacheKey(cid));
        }

        /// <summary>
        /// 获取指定竞赛ID的缓存KEY
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>缓存KEY</returns>
        private static String GetSolutionCountCacheKey(Int32 cid)
        {
            if (cid >= 0)
            {
                return String.Format("{0}:cid={1}", SOLUTION_COUNT_CACHE_KEY, cid.ToString());
            }
            else
            {
                return SOLUTION_COUNT_CACHE_KEY;
            }
        }
        #endregion

        #region 用户AC代码打包文件
        /// <summary>
        /// 缓存中存储AC代码打包文件的KEY
        /// </summary>
        private const String ACCEPTED_CODES_CACHE_KEY = "oj.s.codes";

        /// <summary>
        /// 缓存时间
        /// </summary>
        private const Int32 ACCEPTED_CODES_CACHE_TIME = 600;

        /// <summary>
        /// 向缓存中写入AC代码打包文件
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="file">AC代码打包文件</param>
        public static void SetAcceptedCodesCache(String userName, Byte[] file)
        {
            CacheManager.Set(GetAcceptedCodesKey(userName), file, ACCEPTED_CODES_CACHE_TIME);
        }

        /// <summary>
        /// 从缓存中读取AC代码打包文件
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>AC代码打包文件</returns>
        public static Byte[] GetAcceptedCodesCache(String userName)
        {
            return CacheManager.Get<Byte[]>(GetAcceptedCodesKey(userName));
        }

        /// <summary>
        /// 从缓存中删除AC代码打包文件
        /// </summary>
        /// <param name="userName">用户名</param>
        public static void RemoveAcceptedCodesCache(String userName)
        {
            CacheManager.Remove(GetAcceptedCodesKey(userName));
        }

        /// <summary>
        /// 获取指定用户名缓存KEY
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>缓存KEY</returns>
        private static String GetAcceptedCodesKey(String userName)
        {
            return String.Format("{0}:name={1}", ACCEPTED_CODES_CACHE_KEY, userName);
        }
        #endregion

        #region 用户完成/未完成题目ID列表
        /// <summary>
        /// 缓存中存储题目ID列表的KEY
        /// </summary>
        private const String PROBLEMID_LIST_CACHE_KEY = "oj.s.part";

        /// <summary>
        /// 缓存时间
        /// </summary>
        private const Int32 PROBLEMID_LIST_CACHE_TIME = 600;

        /// <summary>
        /// 向缓存中写入题目ID列表
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="isUnsolved">是否非完成列表</param>
        /// <param name="list">题目ID列表</param>
        public static void SetProblemIDListCache(String userName, Boolean isUnsolved, List<Int32> list)
        {
            CacheManager.Set(GetProblemIDListCacheKey(userName, isUnsolved), list ?? new List<Int32>(), PROBLEMID_LIST_CACHE_TIME);
        }

        /// <summary>
        /// 从缓存中读取题目ID列表
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="isUnsolved">是否非完成列表</param>
        /// <returns>题目ID列表</returns>
        public static List<Int32> GetProblemIDListCache(String userName, Boolean isUnsolved)
        {
            return CacheManager.Get<List<Int32>>(GetProblemIDListCacheKey(userName, isUnsolved));
        }

        /// <summary>
        /// 从缓存中删除题目ID列表
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="isUnsolved">是否非完成列表</param>
        public static void RemoveProblemIDListCache(String userName, Boolean isUnsolved)
        {
            CacheManager.Remove(GetProblemIDListCacheKey(userName, isUnsolved));
        }

        /// <summary>
        /// 获取指定用户名和状态缓存KEY
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="isUnsolved">是否非完成列表</param>
        /// <returns>缓存KEY</returns>
        private static String GetProblemIDListCacheKey(String userName, Boolean isUnsolved)
        {
            return String.Format("{0}:name={1}|{2}", PROBLEMID_LIST_CACHE_KEY, userName, (isUnsolved ? "u" : "s"));
        }
        #endregion

        #region 题目统计信息
        /// <summary>
        /// 缓存中存储题目统计信息的KEY
        /// </summary>
        private const String PROBLEMSTATISTIC_CACHE_KEY = "oj.s.st.pb";

        /// <summary>
        /// 缓存时间
        /// </summary>
        private const Int32 PROBLEMSTATISTIC_CACHE_TIME = 10;

        /// <summary>
        /// 向缓存中写入题目统计
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <returns>题目统计信息实体</returns>
        public static void SetProblemStatisticCache(Int32 cid, Int32 pid, ProblemStatistic statistic)
        {
            CacheManager.Set(GetProblemStatisticCacheKey(cid, pid), statistic, PROBLEMSTATISTIC_CACHE_TIME);
        }

        /// <summary>
        /// 从缓存中读取题目统计
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <returns>题目统计信息实体</returns>
        public static ProblemStatistic GetProblemStatisticCache(Int32 cid, Int32 pid)
        {
            return CacheManager.Get<ProblemStatistic>(GetProblemStatisticCacheKey(cid, pid));
        }

        /// <summary>
        /// 获取题目统计缓存KEY
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <returns>缓存KEY</returns>
        private static String GetProblemStatisticCacheKey(Int32 cid, Int32 pid)
        {
            return String.Format("{0}:cid={1};pid={2}", PROBLEMSTATISTIC_CACHE_KEY, cid.ToString(), pid.ToString());
        }
        #endregion

        #region 竞赛题目统计信息
        /// <summary>
        /// 缓存中存储竞赛统计信息的KEY
        /// </summary>
        private const String CONTESTSTATISTIC_CACHE_KEY = "oj.s.st.ct";

        /// <summary>
        /// 缓存时间
        /// </summary>
        private const Int32 CONTESTSTATISTIC_CACHE_TIME = 10;

        /// <summary>
        /// 向缓存中写入竞赛题目统计列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>竞赛题目统计列表</returns>
        public static void SetContestStatisticCache(Int32 cid, IDictionary<Int32, ContestProblemStatistic> statistic)
        {
            CacheManager.Set(GetContestStatisticCacheKey(cid), statistic, CONTESTSTATISTIC_CACHE_TIME);
        }

        /// <summary>
        /// 从缓存中读取竞赛题目统计列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>竞赛题目统计列表</returns>
        public static IDictionary<Int32, ContestProblemStatistic> GetContestStatisticCache(Int32 cid)
        {
            return CacheManager.Get<IDictionary<Int32, ContestProblemStatistic>>(GetContestStatisticCacheKey(cid));
        }

        /// <summary>
        /// 获取竞赛题目统计列表缓存KEY
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>缓存KEY</returns>
        private static String GetContestStatisticCacheKey(Int32 cid)
        {
            return String.Format("{0}:cid={1}", CONTESTSTATISTIC_CACHE_KEY, cid.ToString());
        }
        #endregion
    }
}