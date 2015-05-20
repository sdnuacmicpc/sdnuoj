using System;
using System.Web;

namespace SDNUOJ.Configuration
{
    /// <summary>
    /// 网络信息管理器
    /// </summary>
    public static class WebManager
    {
        /// <summary>
        /// 获取系统域名路径（末尾有“/”）
        /// </summary>
        /// <param name="request">Http请求</param>
        /// <returns>系统域名路径（末尾有“/”）</returns>
        public static String GetDomainUrl(HttpRequestBase request)
        {
            if (!String.IsNullOrEmpty(ConfigurationManager.DomainUrl))
            {
                return ConfigurationManager.DomainUrl;
            }

            if (request != null && !String.IsNullOrEmpty(request.Url.AbsoluteUri))
            {
                String url = request.Url.AbsoluteUri.Replace(request.Url.AbsolutePath, "");
                if (url[url.Length - 1] != '/') url += '/';

                return url;
            }

            return String.Empty;
        }
    }
}