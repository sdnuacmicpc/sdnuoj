using System;
using System.Collections;
using System.Collections.Generic;

namespace SDNUOJ.Caching
{
    /// <summary>
    /// 用户邮件缓存类
    /// </summary>
    public static class UserMailCache
    {
        #region 存储用户未读邮件总数
        /// <summary>
        /// 缓存中存储用户未读邮件总数的KEY
        /// </summary>
        private const String USERUNREADMAIL_COUNT_CACHE_KEY = "oj.m.ucnt";

        /// <summary>
        /// 缓存时间
        /// </summary>
        private const Int32 USERUNREADMAIL_COUNT_CACHE_TIME = 300;

        /// <summary>
        /// 向缓存中写入用户未读邮件总数
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="count">用户未读邮件总数</param>
        public static void SetUserUnReadMailCountCache(String userName, Int32 count)
        {
            CacheManager.Set(GetUserUnReadMailCountCacheKey(userName), count, USERUNREADMAIL_COUNT_CACHE_TIME);
        }

        /// <summary>
        /// 从缓存中读取用户未读邮件总数
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>用户未读邮件总数</returns>
        public static Int32 GetUserUnReadMailCountCache(String userName)
        {
            return CacheManager.GetInt32(GetUserUnReadMailCountCacheKey(userName));
        }

        /// <summary>
        /// 获取当前在线用户列表
        /// </summary>
        /// <returns>当前在线用户列表</returns>
        public static List<String> GetOnlineUserNames()
        {
            IEnumerator<KeyValuePair<String, Object>> items = CacheManager.GetAll();
            List<String> lstUserNames = new List<String>();

            if (items != null)
            {
                String emptyKey = GetUserUnReadMailCountCacheKey("");

                do
                {
                    if (!String.IsNullOrEmpty(items.Current.Key) && (items.Current.Key.IndexOf(emptyKey) >= 0))
                    {
                        lstUserNames.Add(items.Current.Key.Replace(emptyKey, ""));
                    }
                }
                while (items.MoveNext());
            }

            return lstUserNames;
        }

        /// <summary>
        /// 从缓存中删除用户未读邮件总数
        /// </summary>
        /// <param name="userName">用户名</param>
        public static void RemoveUserUnReadMailCountCache(String userName)
        {
            CacheManager.Remove(GetUserUnReadMailCountCacheKey(userName));
        }

        /// <summary>
        /// 获取未读邮件总数缓存KEY
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>缓存KEY</returns>
        private static String GetUserUnReadMailCountCacheKey(String userName)
        {
            return String.Format("{0}:name={1}", USERUNREADMAIL_COUNT_CACHE_KEY, userName);
        }
        #endregion
    }
}