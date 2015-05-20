using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Caching;

using SDNUOJ.Configuration;

namespace SDNUOJ.Caching
{
    /// <summary>
    /// 缓存管理器
    /// </summary>
    public static class CacheManager
    {
        #region IsEmpty
        /// <summary>
        /// 判断缓存变量是否为空
        /// </summary>
        /// <param name="key">缓存变量名</param>
        /// <returns>返回缓存变量是否为空</returns>
        public static Boolean IsEmpty(String key)
        {
            return (ConfigurationManager.ContentCacheEnable ? MemoryCache.Default.Contains(key) : true);
        }
        #endregion

        #region Set
        /// <summary>
        /// 设置缓存变量
        /// </summary>
        /// <param name="key">缓存变量名</param>
        /// <param name="value">缓存变量值</param>
        public static void Set(String key, Object value)
        {
            if (ConfigurationManager.ContentCacheEnable)
            {
                MemoryCache.Default.Set(key, value, new CacheItemPolicy() { Priority = CacheItemPriority.NotRemovable });
            }
        }

        /// <summary>
        /// 设置缓存变量
        /// </summary>
        /// <param name="key">缓存变量名</param>
        /// <param name="value">缓存变量值</param>
        /// <param name="dt">缓存结束时间</param>
        public static void Set(String key, Object value, DateTime dt)
        {
            if (ConfigurationManager.ContentCacheEnable)
            {
                MemoryCache.Default.Set(key, value, new CacheItemPolicy() { Priority = CacheItemPriority.Default, AbsoluteExpiration = dt });
            }
        }

        /// <summary>
        /// 设置缓存变量
        /// </summary>
        /// <param name="key">缓存变量名</param>
        /// <param name="value">缓存变量值</param>
        /// <param name="second">缓存持续时间(秒)</param>
        public static void Set(String key, Object value, Int32 second)
        {
            if (ConfigurationManager.ContentCacheEnable)
            {
                MemoryCache.Default.Set(key, value, new CacheItemPolicy() { Priority = CacheItemPriority.Default, SlidingExpiration = TimeSpan.FromSeconds(second) });
            }
        }

        /// <summary>
        /// 设置缓存变量
        /// </summary>
        /// <param name="key">缓存变量名</param>
        /// <param name="value">缓存变量值</param>
        /// <param name="ts">缓存未使用的时间</param>
        public static void Set(String key, Object value, TimeSpan ts)
        {
            MemoryCache.Default.Set(key, value, new CacheItemPolicy() { Priority = CacheItemPriority.Default, SlidingExpiration = ts });
        }
        #endregion

        #region Get
        /// <summary>
        /// 获取缓存变量值
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">缓存变量名</param>
        /// <returns>缓存变量值</returns>
        public static String Get(String key)
        {
            return (ConfigurationManager.ContentCacheEnable ? MemoryCache.Default.Get(key) as String : String.Empty);
        }

        /// <summary>
        /// 获取缓存变量值
        /// </summary>
        /// <param name="key">缓存变量名</param>
        /// <returns>缓存变量值</returns>
        public static T Get<T>(String key) where T : class
        {
            return (ConfigurationManager.ContentCacheEnable ? MemoryCache.Default.Get(key) as T : null);
        }

        /// <summary>
        /// 获取缓存变量值
        /// </summary>
        /// <param name="key">缓存变量名</param>
        /// <returns>缓存变量值，若不存在返回-1</returns>
        public static Int32 GetInt32(String key)
        {
            Object obj;
            return (ConfigurationManager.ContentCacheEnable && (obj = MemoryCache.Default.Get(key)) != null ? (Int32)obj : -1);
        }

        /// <summary>
        /// 获取缓存变量值
        /// </summary>
        /// <param name="key">缓存变量名</param>
        /// <returns>缓存变量值</returns>
        public static Object GetObject(String key)
        {
            return (ConfigurationManager.ContentCacheEnable ? MemoryCache.Default.Get(key) : null);
        }

        /// <summary>
        /// 获取指定缓存变量（可为null）
        /// </summary>
        /// <param name="key">缓存变量名</param>
        /// <returns>指定缓存变量（可为null）</returns>
        public static List<KeyValuePair<String, String>> GetDetail(String key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return null;
            }

            Object obj = CacheManager.GetObject(key);

            if (obj == null)
            {
                return null;
            }

            List<KeyValuePair<String, String>> list = new List<KeyValuePair<String, String>>();
            CacheManager.AddObjectDetail(obj, list, "");

            return list;
        }

        private static void AddObjectDetail(Object obj, List<KeyValuePair<String, String>> list, String prefix)
        {
            if (obj is IFormattable || obj is String)
            {
                CacheManager.AddSimpleObjectDetail(obj, list, prefix);
            }
            else if (obj is IEnumerable)
            {
                CacheManager.AddEnumerableObjectDetail(obj as IEnumerable, list, prefix);
            }
            else
            {
                CacheManager.AddNormalObjectDetail(obj, list, prefix);
            }
        }

        private static void AddSimpleObjectDetail(Object obj, List<KeyValuePair<String, String>> list, String prefix)
        {
            list.Add(new KeyValuePair<String, String>(prefix + "Value", obj.ToString()));
        }

        private static void AddEnumerableObjectDetail(IEnumerable collection, List<KeyValuePair<String, String>> list, String prefix)
        {
            Type type = collection.GetType();
            Int32 count = 0;
            String prefixFormat = String.Format("{0}Item[{{0}}].", prefix);
            

            foreach (Object obj in collection)
            {
                if (count < 100)
                {
                    CacheManager.AddObjectDetail(obj, list, String.Format(prefixFormat, count.ToString()));
                }

                count++;
            }

            if (count > 100)
            {
                list.Add(new KeyValuePair<String, String>("...", "There are too many items in this object."));
            }

            list.Add(new KeyValuePair<String, String>("Type", type.ToString()));
            list.Add(new KeyValuePair<String, String>("Count", count.ToString()));
        }
        
        private static void AddNormalObjectDetail(Object obj, List<KeyValuePair<String, String>> list, String prefix)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                if (property.GetIndexParameters().Length > 0)
                {
                    continue;//去除索引器
                }

                Object value = property.GetValue(obj, null);

                list.Add(new KeyValuePair<String, String>(prefix + property.Name, (value != null ? value.ToString() : "NULL")));
            }
        }
        #endregion

        #region GetAll
        /// <summary>
        /// 获取所有缓存变量
        /// </summary>
        /// <returns>所有缓存变量</returns>
        public static IEnumerator<KeyValuePair<String, Object>> GetAll()
        {
            if (!ConfigurationManager.ContentCacheEnable)
            {
                return null;
            }

            Type type = typeof(MemoryCache);
            MethodInfo method = type.GetMethod("GetEnumerator", BindingFlags.Instance | BindingFlags.NonPublic);
            IEnumerator<KeyValuePair<String, Object>> items = method.Invoke(MemoryCache.Default, null) as IEnumerator<KeyValuePair<String, Object>>;

            return items;
        }
        #endregion

        #region Remove
        /// <summary>
        /// 删除缓存变量
        /// </summary>
        /// <param name="key">缓存变量名</param>
        public static void Remove(String key)
        {
            if (ConfigurationManager.ContentCacheEnable)
            {
                MemoryCache.Default.Remove(key);
            }
        }

        /// <summary>
        /// 删除缓存变量
        /// </summary>
        /// <param name="keys">缓存变量名集合</param>
        public static void Remove(String[] keys)
        {
            if (!ConfigurationManager.ContentCacheEnable || keys == null || keys.Length == 0)
            {
                return;
            }

            for (Int32 i = 0; i < keys.Length; i++)
            {
                MemoryCache.Default.Remove(keys[i].Trim());
            }
        }
        #endregion

        #region RemoveAll
        /// <summary>
        /// 删除全部缓存变量
        /// </summary>
        public static void RemoveAll()
        {
            if (ConfigurationManager.ContentCacheEnable)
            {
                MemoryCache.Default.Trim(100);
            }
        }
        #endregion

        #region GetCount
        /// <summary>
        /// 获取所有缓存数量
        /// </summary>
        /// <returns>所有缓存数量</returns>
        public static Int64 GetCount()
        {
            return (ConfigurationManager.ContentCacheEnable ? MemoryCache.Default.GetCount() : 0);
        }
        #endregion
    }
}