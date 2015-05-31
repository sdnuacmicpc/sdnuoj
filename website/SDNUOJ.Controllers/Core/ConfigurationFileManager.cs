using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

using SDNUOJ.Controllers.Exception;
using SDNUOJ.Configuration;
using SDNUOJ.Entity;
using SDNUOJ.Logging;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 配置文件管理类
    /// </summary>
    internal static class ConfigurationFileManager
    {
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <returns>配置信息</returns>
        public static List<KeyValuePair<String, String>> GetConfigPairList()
        {
            NameValueCollection col = ConfigurationManager.GetConfigCollection();
            List<KeyValuePair<String, String>> list = new List<KeyValuePair<String, String>>();

            foreach (String key in col.AllKeys)
            {
                list.Add(new KeyValuePair<String, String>(key, col[key]));
            }

            return list;
        }

        /// <summary>
        /// 保存配置到配置文件
        /// </summary>
        /// <param name="col">配置信息</param>
        public static void SaveToConfig(NameValueCollection col)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            LogManager.LogOperation(HttpContext.Current, UserManager.CurrentUserName, String.Format("Admin Update Web.Config"));

            ConfigurationManager.SaveToConfig(col);
        }
    }
}