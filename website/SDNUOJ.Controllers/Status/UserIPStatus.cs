using System;
using System.Web;
using System.Web.Caching;

using SDNUOJ.Configuration;

namespace SDNUOJ.Controllers.Status
{
    /// <summary>
    /// 用户IP管理器
    /// </summary>
    public static class UserIPStatus
    {
        #region 字段
        private static Cache _ips;
        #endregion

        #region 构造方法
        static UserIPStatus()
        {
            _ips = HttpRuntime.Cache;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 判断注册间隔是否满足
        /// </summary>
        /// <param name="ip">IP</param>
        /// <returns>IP是否存在</returns>
        public static Boolean CheckLastRegisterTime(String ip)
        {
            if (ConfigurationManager.RegisterInterval <= 0)
            {
                return true;
            }

            if (_ips.Get(ip) != null)
            {
                return false;
            }

            _ips.Add(ip, true, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, ConfigurationManager.RegisterInterval), CacheItemPriority.NotRemovable, null);

            return true;
        }
        #endregion
    }
}