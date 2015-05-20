using System;
using System.Collections.Generic;

using SDNUOJ.Entity;

namespace SDNUOJ.Caching
{
    /// <summary>
    /// 题目类别种类信息缓存类
    /// </summary>
    public static class ProblemCategoryCache
    {
        #region 所有题目类别种类信息
        /// <summary>
        /// 缓存中存储题目类别种类信息的KEY
        /// </summary>
        private const String PROBLEM_CATEGORY_LIST_CACHE_KEY = "oj.p.categorys";

        /// <summary>
        /// 向缓存中写入题目类别种类信息
        /// </summary>
        /// <param name="list">题目类别种类信息</param>
        public static void SetProblemCategoryListCache(List<ProblemCategoryEntity> list)
        {
            if (list != null) CacheManager.Set(PROBLEM_CATEGORY_LIST_CACHE_KEY, list);
        }

        /// <summary>
        /// 从缓存中读取题目类别种类信息
        /// </summary>
        /// <returns>题目类别种类信息</returns>
        public static List<ProblemCategoryEntity> GetProblemCategoryListCache()
        {
            return CacheManager.Get<List<ProblemCategoryEntity>>(PROBLEM_CATEGORY_LIST_CACHE_KEY);
        }

        /// <summary>
        /// 从缓存中删除题目类别种类信息
        /// </summary>
        public static void RemoveProblemCategoryListCache()
        {
            CacheManager.Remove(PROBLEM_CATEGORY_LIST_CACHE_KEY);
        }
        #endregion
    }
}