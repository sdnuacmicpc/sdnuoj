using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

using SDNUOJ.Utilities;

namespace SDNUOJ
{
    /// <summary>
    /// 静态文件扩展类
    /// </summary>
    public static class StaticFileExtension
    {
        #region 常量
        private static readonly DateTime UNIXEPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, 0);
        #endregion

        #region 字段
        private static Dictionary<String, String> _fileVersionCache;
        #endregion

        #region 构造方法
        static StaticFileExtension()
        {
            _fileVersionCache = new Dictionary<String, String>();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获取静态文件引用路径
        /// </summary>
        /// <param name="page">页面文件</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>静态文件引用路径</returns>
        public static String StaticFile(this WebViewPage page, String filePath)
        {
            return String.Format("{0}?v={1}", filePath, GetFileVersion(page.Context, filePath));
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取文件版本
        /// </summary>
        /// <param name="context">Http上下文</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件版本</returns>
        private static String GetFileVersion(HttpContextBase context, String filePath)
        {
            String version = String.Empty;

            if (_fileVersionCache.TryGetValue(filePath, out version) && !String.IsNullOrEmpty(version))
            {
                return version;
            }

            String realPath = context.Server.MapPath(filePath);

            if (!File.Exists(realPath))
            {
                return String.Empty;
            }

            FileInfo fi = new FileInfo(realPath);

            Int64 seconds = (Int64)(fi.LastWriteTimeUtc - UNIXEPOCH).TotalMilliseconds;
            version = seconds.ToSixtyTwoRadix();

            _fileVersionCache[filePath] = version;

            return version;
        }
        #endregion
    }
}