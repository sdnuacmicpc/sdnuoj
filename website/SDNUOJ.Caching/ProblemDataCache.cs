using System;
using System.Collections.Generic;

namespace SDNUOJ.Caching
{
    /// <summary>
    /// 题目数据缓存类
    /// </summary>
    public static class ProblemDataCache
    {
        #region 指定题目数据版本信息
        private static Dictionary<Int32, String> _lastModified = null;

        static ProblemDataCache()
        {
            _lastModified = new Dictionary<Int32, String>();
        }

        /// <summary>
        /// 向缓存中写入指定题目数据版本
        /// </summary>
        /// <param name="pid">题目ID</param>
        /// <param name="version">数据版本</param>
        public static void SetProblemDataVersionCache(Int32 pid, String version)
        {
            _lastModified[pid] = version;
        }

        /// <summary>
        /// 从缓存中读取指定题目数据版本
        /// </summary>
        /// <param name="pid">题目ID</param>
        /// <returns>数据版本</returns>
        public static String GetProblemDataVersionCache(Int32 pid)
        {
            String version = String.Empty;
            return _lastModified.TryGetValue(pid, out version) ? version : String.Empty;
        }

        /// <summary>
        /// 从缓存中删除指定题目数据版本
        /// </summary>
        /// <param name="pid">题目ID</param>
        public static void RemoveProblemDataVersionCache(Int32 pid)
        {
            _lastModified.Remove(pid);
        }
        #endregion
    }
}