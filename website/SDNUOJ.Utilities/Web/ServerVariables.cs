using System;
using System.Web;

namespace SDNUOJ.Utilities.Web
{
    /// <summary>
    /// 服务器变量类
    /// </summary>
    public static class ServerVariables
    {
        /// <summary>
        /// 获取当前站点物理路径
        /// </summary>
        /// <param name="request">Http请求</param>
        /// <returns>当前站点物理路径</returns>
        public static String ApplicationPath(HttpRequestBase request)
        {
            return (request == null ? String.Empty : request.ServerVariables["APPL_PHYSICAL_PATH"]);
        }

        /// <summary>
        /// 获取当前文件物理路径
        /// </summary>
        /// <param name="request">Http请求</param>
        /// <returns>当前文件物理路径</returns>
        public static String CurrentFilePhysicalPath(HttpRequestBase request)
        {
            return (request == null ? String.Empty : request.ServerVariables["PATH_TRANSLATED"]);
        }

        /// <summary>
        /// 获取当前文件相对路径
        /// </summary>
        /// <param name="request">Http请求</param>
        /// <returns>当前文件相对路径</returns>
        public static String CurrentFileRelativePath(HttpRequestBase request)
        {
            return (request == null ? String.Empty : request.ServerVariables["SCRIPT_NAME"]);
        }
    }
}