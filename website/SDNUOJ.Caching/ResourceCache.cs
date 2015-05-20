using System;
using System.Collections.Generic;

using SDNUOJ.Entity;

namespace SDNUOJ.Caching
{
    /// <summary>
    /// 资源信息缓存类
    /// </summary>
    public static class ResourceCache
    {
        #region 资源信息列表
        /// <summary>
        /// 缓存中存储资源信息的KEY
        /// </summary>
        private const String RESOURCE_LIST_CACHE_KEY = "oj.r.all";

        /// <summary>
        /// 向缓存中写入资源信息
        /// </summary>
        /// <param name="list">资源信息</param>
        public static void SetResourceListCache(List<ResourceEntity> list)
        {
            if (list != null) CacheManager.Set(RESOURCE_LIST_CACHE_KEY, list);
        }

        /// <summary>
        /// 从缓存中读取资源信息
        /// </summary>
        /// <returns>资源信息</returns>
        public static List<ResourceEntity> GetResourceListCache()
        {
            return CacheManager.Get<List<ResourceEntity>>(RESOURCE_LIST_CACHE_KEY);
        }

        /// <summary>
        /// 从缓存中删除资源信息
        /// </summary>
        public static void RemoveResourceListCache()
        {
            CacheManager.Remove(RESOURCE_LIST_CACHE_KEY);
        }
        #endregion
    }
}