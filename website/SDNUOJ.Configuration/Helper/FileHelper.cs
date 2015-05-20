using System;
using System.IO;
using System.Web;

namespace SDNUOJ.Configuration.Helper
{
    /// <summary>
    /// 文件辅助类
    /// </summary>
    internal static class FileHelper
    {
        /// <summary>
        /// 获取文件绝对路径
        /// </summary>
        /// <param name="relativePath">文件相对路径</param>
        /// <returns>文件绝对路径</returns>
        internal static String GetRealPath(String relativePath)
        {
            String realPath = Path.Combine(HttpRuntime.AppDomainAppPath, relativePath);

            return realPath;
        }
    }
}