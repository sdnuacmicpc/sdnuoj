using System;
using System.Collections.Generic;

namespace SDNUOJ.Configuration.Caching
{
    /// <summary>
    /// 语言过滤器缓存
    /// </summary>
    internal static class LanguageFilterCache
    {
        #region 字段
        private static Dictionary<String, Dictionary<String, Byte>> _cache;
        #endregion

        #region 构造方法
        static LanguageFilterCache()
        {
            _cache = new Dictionary<String, Dictionary<String, Byte>>();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 设置语言过滤器结果缓存
        /// </summary>
        /// <param name="filter">过滤器</param>
        /// <param name="result">过滤器结果</param>
        internal static void SetLanguageFilterResultCache(String filter, Dictionary<String, Byte> result)
        {
            _cache[filter] = result;
        }

        /// <summary>
        /// 获取语言过滤器结果缓存
        /// </summary>
        /// <param name="filter">过滤器</param>
        /// <returns>过滤器结果</returns>
        internal static Dictionary<String, Byte> GetLanguageFilterResultCache(String filter)
        {
            Dictionary<String, Byte> result = null;

            return (_cache.TryGetValue(filter, out result) ? result : null);
        }
        #endregion
    }
}