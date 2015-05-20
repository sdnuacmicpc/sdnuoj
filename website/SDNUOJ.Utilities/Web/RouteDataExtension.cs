using System;
using System.Web.Routing;

namespace SDNUOJ.Utilities.Web
{
    public static class RouteDataExtension
    {
        /// <summary>
        /// 从路由数据中获取指定信息
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="routeData">路由数据</param>
        /// <param name="key">信息键名</param>
        /// <param name="defaultValue">信息默认值</param>
        /// <returns>获取到的信息数据</returns>
        public static T GetValue<T>(this RouteData routeData, String key, T defaultValue) where T : IConvertible
        {
            if (routeData == null)
            {
                return defaultValue;
            }

            Object obj = null;
            
            if (!routeData.Values.TryGetValue(key, out obj) || obj == null)
            {
                return defaultValue;
            }

            return (T)Convert.ChangeType(obj, typeof(T));
        }

        /// <summary>
        /// 从路由数据中获取指定信息
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="routeData">路由数据</param>
        /// <param name="key">信息键名</param>
        /// <returns>获取到的信息数据</returns>
        public static T GetValue<T>(this RouteData routeData, String key) where T : IConvertible
        {
            return RouteDataExtension.GetValue<T>(routeData, key, default(T));
        }

        /// <summary>
        /// 从路由数据中获取指定信息
        /// </summary>
        /// <param name="routeData">路由数据</param>
        /// <param name="key">信息键名</param>
        /// <param name="defaultValue">信息默认值</param>
        /// <returns>获取到的信息数据</returns>
        public static String GetString(this RouteData routeData, String key, String defaultValue)
        {
            if (routeData == null)
            {
                return defaultValue;
            }

            Object obj = null;

            if (!routeData.Values.TryGetValue(key, out obj) || obj == null)
            {
                return defaultValue;
            }

            String value = obj as String;

            return value;
        }

        /// <summary>
        /// 从路由数据中获取指定信息
        /// </summary>
        /// <param name="routeData">路由数据</param>
        /// <param name="key">信息键名</param>
        /// <returns>获取到的信息数据</returns>
        public static String GetString(this RouteData routeData, String key)
        {
            return RouteDataExtension.GetString(routeData, key, String.Empty);
        }

        /// <summary>
        /// 从路由数据中获取指定信息
        /// </summary>
        /// <param name="routeData">路由数据</param>
        /// <param name="key">信息键名</param>
        /// <param name="defaultValue">信息默认值</param>
        /// <returns>获取到的信息数据</returns>
        public static Int32 GetInt32(this RouteData routeData, String key, Int32 defaultValue)
        {
            if (routeData == null)
            {
                return defaultValue;
            }

            Object obj = null;

            if (!routeData.Values.TryGetValue(key, out obj) || obj == null)
            {
                return defaultValue;
            }

            Int32 value = defaultValue;

            if (Int32.TryParse(obj as String, out value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从路由数据中获取指定信息
        /// </summary>
        /// <param name="routeData">路由数据</param>
        /// <param name="key">信息键名</param>
        /// <returns>获取到的信息数据</returns>
        public static Int32 GetInt32(this RouteData routeData, String key)
        {
            return RouteDataExtension.GetInt32(routeData, key, 0);
        }

        /// <summary>
        /// 从路由数据中获取页码索引
        /// </summary>
        /// <param name="routeData">路由数据</param>
        /// <param name="key">信息键名</param>
        /// <returns>获取到的页码索引</returns>
        public static Int32 GetPageIndex(this RouteData routeData, String key)
        {
            return RouteDataExtension.GetInt32(routeData, key, 1);
        }
    }
}