using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Entity;
using SDNUOJ.Logging;
using SDNUOJ.Utilities;
using SDNUOJ.Utilities.Security;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 上传文件管理器
    /// </summary>
    internal static class UploadsManager
    {
        #region 常量
        private static readonly String[] ALLOW_EXTENSTIONS = new String[] { ".zip", ".rar", ".7z", ".bmp", ".jpg", ".png", ".gif", ".txt", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf" };
        #endregion

        #region 管理方法
        /// <summary>
        /// 保存单个文件到磁盘
        /// </summary>
        /// <param name="file">上传的文件</param>
        /// <param name="fileNewName">文件新名称</param>
        /// <returns>是否保存成功</returns>
        public static Boolean AdminSaveUploadFile(HttpPostedFileBase file, out String fileNewName)
        {
            if (!AdminManager.HasPermission(PermissionType.Administrator))
            {
                throw new NoPermissionException();
            }

            if (file == null)
            {
                throw new InvalidInputException("No file uploaded");
            }

            if (String.IsNullOrEmpty(file.FileName))
            {
                throw new InvalidInputException("Filename can not be NULL!");
            }
            
            FileInfo fi = new FileInfo(file.FileName);

            if (!UploadsManager.CheckFileExtension(fi.Extension, ALLOW_EXTENSTIONS))
            {
                throw new InvalidInputException("Filename is INVALID!");
            }

            if (file.ContentLength <= 0)
            {
                throw new InvalidInputException("You can not upload empty file!");
            }

            fileNewName = MD5Encrypt.EncryptToHexString(fi.Name + DateTime.Now.ToString("yyyyMMddHHmmssffff"), true) + fi.Extension;
            String savePath = Path.Combine(ConfigurationManager.UploadDirectoryPath, fileNewName);

            if (File.Exists(savePath))
            {
                throw new InvalidInputException("Filename exists!");
            }

            file.SaveAs(savePath);

            LogManager.LogOperation(HttpContext.Current, UserManager.CurrentUserName, String.Format("Admin Upload File, Filename = \"{0}\"", fi.Name));

            return true;
        }

        /// <summary>
        /// 保存单个文件到磁盘
        /// </summary>
        /// <param name="fileContent">文件内容</param>
        /// <param name="fileNewName">文件新名称</param>
        /// <returns>是否保存成功</returns>
        internal static Boolean InternalAdminSaveUploadFile(Byte[] fileContent, String fileNewName)
        {
            if (!AdminManager.HasPermission(PermissionType.Administrator))
            {
                throw new NoPermissionException();
            }

            if (String.IsNullOrEmpty(fileNewName))
            {
                throw new InvalidInputException("Filename can not be NULL!");
            }

            FileInfo fi = new FileInfo(fileNewName);

            if (!UploadsManager.CheckFileExtension(fi.Extension, ALLOW_EXTENSTIONS))
            {
                throw new InvalidInputException("Filename is INVALID!");
            }

            String savePath = Path.Combine(ConfigurationManager.UploadDirectoryPath, fileNewName);

            if (File.Exists(savePath))
            {
                throw new InvalidInputException("Filename exists!");
            }

            File.WriteAllBytes(savePath, fileContent);

            LogManager.LogOperation(HttpContext.Current, UserManager.CurrentUserName, String.Format("Admin Upload File, Filename = \"{0}\"", fi.Name));

            return true;
        }

        /// <summary>
        /// 删除上传文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>是否删除成功</returns>
        public static Boolean AdminDeleteUploadFile(String fileName)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            String filePath = Path.Combine(ConfigurationManager.UploadDirectoryPath, fileName);

            if (!File.Exists(filePath))
            {
                throw new InvalidInputException("File does not exist!");
            }

            File.Delete(filePath);

            LogManager.LogOperation(HttpContext.Current, UserManager.CurrentUserName, String.Format("Admin Delete File, Filename = \"{0}\"", fileName));

            return true;
        }

        /// <summary>
        /// 获取上传文件真实路径
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>上传文件真实路径</returns>
        public static String AdminGetUploadFileRealPath(String fileName)
        {
            if (!AdminManager.HasPermission(PermissionType.Administrator))
            {
                throw new NoPermissionException();
            }

            String filePath = Path.Combine(ConfigurationManager.UploadDirectoryPath, fileName);

            if (!File.Exists(filePath))
            {
                throw new InvalidInputException("File does not exist!");
            }

            return filePath;
        }

        /// <summary>
        /// 获取上传文件预览Url
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>上传文件真实路径</returns>
        public static String AdminPreviewUploadFileUrl(String fileName)
        {
            if (!AdminManager.HasPermission(PermissionType.Administrator))
            {
                throw new NoPermissionException();
            }

            String fileUrl = String.Format("{0}{1}", ConfigurationManager.UploadDirectoryUrl, fileName);

            return fileUrl;
        }

        /// <summary>
        /// 获取上传文件列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>上传文件列表</returns>
        public static PagedList<FileInfo> AdminGetUploadFiles(Int32 pageIndex)
        {
            if (!AdminManager.HasPermission(PermissionType.Administrator))
            {
                throw new NoPermissionException();
            }

            DirectoryInfo dirUploadPath = new DirectoryInfo(ConfigurationManager.UploadDirectoryPath);

            if (!dirUploadPath.Exists)
            {
                throw new InvalidInputException("Upload path does not exist!");
            }

            FileInfo[] files = dirUploadPath.GetFiles();
            Comparison<FileInfo> comparison = delegate(FileInfo a, FileInfo b) { return b.LastWriteTime.CompareTo(a.LastWriteTime); };
            Array.Sort<FileInfo>(files, comparison);
            Int32 pageSize = AdminManager.ADMIN_LIST_PAGE_SIZE;
            Int32 recordCount = files.Length;
            
            //String[] files = Directory.GetFiles(ConfigurationManager.UploadFilePath);
            //Comparison<String> comparison = delegate(String a, String b) { return b.CompareTo(a); };
            //Array.Sort<String>(files, comparison);
            //Int32 recordCount = files.Length;

            List<FileInfo> fis = new List<FileInfo>(pageSize);
            Int32 startIndex = (pageIndex - 1) * pageSize;
            Int32 endIndex = pageIndex * pageSize;

            if (startIndex >= files.Length)
            {
                return PagedList<FileInfo>.Empty;
            }

            if (endIndex >= files.Length)
            {
                endIndex = files.Length - 1;
            }

            for (Int32 i = startIndex; i <= endIndex; i++)
            {
                fis.Add(files[i]);
            }

            return fis.ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 获取上传文件数量
        /// </summary>
        /// <returns>上传文件数量</returns>
        private static Int32 AdminCountUploadFiles()
        {
            if (!AdminManager.HasPermission(PermissionType.Administrator))
            {
                throw new NoPermissionException();
            }

            String[] files = Directory.GetFiles(ConfigurationManager.UploadDirectoryPath);

            return files.Length;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 检查文件扩展名是否有效
        /// </summary>
        /// <param name="ext">文件扩展名</param>
        /// <param name="allowExts">允许的扩展名集合</param>
        /// <returns>文件扩展名是否有效</returns>
        private static Boolean CheckFileExtension(String ext, String[] allowExts)
        {
            if (allowExts == null || allowExts.Length == 0)
            {
                return true;
            }

            for (Int32 i = 0; i < allowExts.Length; i++)
            {
                if (allowExts[i].Equals(ext, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}