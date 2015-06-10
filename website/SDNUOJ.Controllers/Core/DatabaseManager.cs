using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

using SDNUOJ.Caching;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Data;
using SDNUOJ.Entity;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 数据库管理类
    /// </summary>
    internal static class DatabaseManager
    {
        #region 属性
        /// <summary>
        /// 获取当前数据库类型
        /// </summary>
        public static String DataBaseType { get { return DatabaseConfiguration.DataBaseType.ToUpper(); } }

        /// <summary>
        /// 获取当前数据库连接字符串
        /// </summary>
        public static String DataBaseConnectionString { get { return DatabaseConfiguration.DataBaseConnectionString; } }
        #endregion

        #region ACCESS属性
        /// <summary>
        /// 获取是否是ACCESS数据库
        /// </summary>
        public static Boolean IsAccessDB { get { return String.Equals("ACCESS", DataBaseType, StringComparison.OrdinalIgnoreCase); } }

        /// <summary>
        /// 获取Access数据库存放路径
        /// </summary>
        public static String AccessDataDirectory
        {
            get { return Path.GetDirectoryName(DatabaseManager.AccessDBFullPath); }
        }

        /// <summary>
        /// 获取Access数据库文件名
        /// </summary>
        public static String AccessDBFileName
        {
            get { return Path.GetFileName(DatabaseManager.AccessDBFullPath); }
        }

        /// <summary>
        /// 获取Access数据库完整路径
        /// </summary>
        public static String AccessDBFullPath
        {
            get
            {
                if (!IsAccessDB)
                {
                    return String.Empty;
                }

                String connectionString = DataBaseConnectionString;
                String[] s = connectionString.Split(';');
                String dbPath = null;

                for (Int32 i = 0; i < s.Length; i++)
                {
                    if (s[i].IndexOf("Data Source", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        dbPath = s[i].Replace("Data Source=", "").Trim();
                        dbPath = dbPath.Replace("|DataDirectory|", AppDomain.CurrentDomain.GetData("DataDirectory") as String);
                        break;
                    }
                }

                return dbPath;
            }
        }

        /// <summary>
        /// 获取Access数据库大小
        /// </summary>
        public static Int64 AccessDBSize
        {
            get
            {
                if (!IsAccessDB)
                {
                    return -1;
                }

                FileInfo fi = new FileInfo(AccessDBFullPath);
                return (fi.Exists ? fi.Length : -1);
            }
        }
        #endregion

        #region ACCESS数据库操作方法
        /// <summary>
        /// 保存数据库到磁盘
        /// </summary>
        /// <param name="file">上传文件</param>
        /// <returns>是否保存成功</returns>
        public static IMethodResult AdminSaveUploadDataBase(HttpPostedFileBase file)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!DatabaseManager.IsAccessDB)
            {
                throw new DatabaseNotSupportException();
            }

            if (file == null)
            {
                return MethodResult.FailedAndLog("No file was uploaded!");
            }

            if (String.IsNullOrEmpty(file.FileName))
            {
                return MethodResult.FailedAndLog("Filename can not be NULL!");
            }

            FileInfo fi = new FileInfo(file.FileName);
            if (!String.Equals(fi.Extension, ".resx", StringComparison.OrdinalIgnoreCase))
            {
                return MethodResult.FailedAndLog("Filename is INVALID!");
            }

            if (file.ContentLength <= 0)
            {
                return MethodResult.FailedAndLog("You can not upload empty file!");
            }

            String newName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".resx";
            String savePath = Path.Combine(DatabaseManager.AccessDataDirectory, newName);
            file.SaveAs(savePath);

            return MethodResult.SuccessAndLog("Admin upload database, name = {0}", newName);
        }

        /// <summary>
        /// 压缩指定数据库
        /// </summary>
        /// <param name="fileName">数据库文件名</param>
        /// <returns>是否压缩成功</returns>
        public static IMethodResult AdminCompactDataBase(String fileName)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!DatabaseManager.IsAccessDB)
            {
                throw new DatabaseNotSupportException();
            }

            String sourcePath = Path.Combine(DatabaseManager.AccessDataDirectory, fileName);
            String tempPath = Path.Combine(DatabaseManager.AccessDataDirectory, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".resx");

            if (!File.Exists(sourcePath))
            {
                return MethodResult.FailedAndLog("Database does not exist!");
            }

            if (DatabaseManager.DataBaseConnectionString.IndexOf("Microsoft.Jet.OLEDB.4.0", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                Object objJRO = Activator.CreateInstance(Type.GetTypeFromProgID("JRO.JetEngine"));
                String connSource = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + sourcePath;
                String connTemp = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + tempPath;

                Object[] args = new Object[] { connSource, connTemp };
                objJRO.GetType().InvokeMember("CompactDatabase", System.Reflection.BindingFlags.InvokeMethod, null, objJRO, args);
            }
            else if (DatabaseManager.DataBaseConnectionString.IndexOf("Microsoft.ACE.OLEDB.12.0", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                Object objDAO = Activator.CreateInstance(Type.GetTypeFromProgID("Dao.DBEngine.120"));
                Object[] args = new Object[] { sourcePath, tempPath };

                objDAO.GetType().InvokeMember("CompactDatabase", System.Reflection.BindingFlags.InvokeMethod, null, objDAO, args);
            }
            else
            {
                return MethodResult.FailedAndLog("Your database engine doesn't support database compacting!");
            }

            File.Copy(tempPath, sourcePath, true);
            File.Delete(tempPath);

            return MethodResult.SuccessAndLog("Admin compact database, name = {0}", fileName);
        }

        /// <summary>
        /// 备份指定数据库
        /// </summary>
        /// <param name="fileName">数据库文件名</param>
        /// <returns>是否备份成功</returns>
        public static IMethodResult AdminBackupDataBase(String fileName)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!DatabaseManager.IsAccessDB)
            {
                throw new DatabaseNotSupportException();
            }

            String sourcePath = DatabaseManager.AccessDBFullPath;
            String destPath = Path.Combine(DatabaseManager.AccessDataDirectory, fileName);

            if (String.Equals(sourcePath, destPath, StringComparison.OrdinalIgnoreCase))
            {
                return MethodResult.FailedAndLog("You can not use this filename!");
            }

            File.Copy(sourcePath, destPath, true);

            return MethodResult.SuccessAndLog("Admin backup database, name = {0}", fileName);
        }

        /// <summary>
        /// 还原指定数据库
        /// </summary>
        /// <param name="fileName">数据库文件名</param>
        /// <returns>是否还原成功</returns>
        public static IMethodResult AdminRestoreDataBase(String fileName)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!DatabaseManager.IsAccessDB)
            {
                throw new DatabaseNotSupportException();
            }

            String sourcePath = Path.Combine(DatabaseManager.AccessDataDirectory, fileName);
            String destPath = DatabaseManager.AccessDBFullPath;

            if (!File.Exists(sourcePath))
            {
                return MethodResult.FailedAndLog("Database does not exist!");
            }

            if (String.Equals(destPath, sourcePath, StringComparison.OrdinalIgnoreCase))
            {
                return MethodResult.FailedAndLog("You can not restore the current database!");
            }

            File.Copy(sourcePath, destPath, true);
            CacheManager.RemoveAll();

            return MethodResult.SuccessAndLog("Admin restore database, name = {0}", fileName);
        }

        /// <summary>
        /// 删除指定数据库
        /// </summary>
        /// <param name="did">数据库ID</param>
        /// <returns>是否删除成功</returns>
        public static IMethodResult AdminDeleteDataBase(Int32 did)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!DatabaseManager.IsAccessDB)
            {
                throw new DatabaseNotSupportException();
            }

            FileInfo fi = new FileInfo(DatabaseManager.AdminGetDataBaseFileName(did));
            String filePath = Path.Combine(DatabaseManager.AccessDataDirectory, fi.Name);

            if (!File.Exists(filePath))
            {
                return MethodResult.FailedAndLog("Database does not exist!");
            }

            if (String.Equals(DatabaseManager.AccessDBFullPath, filePath, StringComparison.OrdinalIgnoreCase))
            {
                return MethodResult.FailedAndLog("You can not delete the current database!");
            }

            File.Delete(filePath);

            return MethodResult.SuccessAndLog("Admin delete database, name = {0}", fi.Name);
        }

        /// <summary>
        /// 获取数据库真实路径
        /// </summary>
        /// <param name="did">数据库ID</param>
        /// <returns>数据库真实路径</returns>
        public static IMethodResult AdminGetDataBaseDownloadPath(Int32 did)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!DatabaseManager.IsAccessDB)
            {
                throw new DatabaseNotSupportException();
            }

            FileInfo fi = new FileInfo(DatabaseManager.AdminGetDataBaseFileName(did));
            String filePath = Path.Combine(DatabaseManager.AccessDataDirectory, fi.Name);

            if (!File.Exists(filePath))
            {
                return MethodResult.FailedAndLog("Database does not exist!");
            }

            return MethodResult.SuccessAndLog<String>(filePath, "Admin download database, name = {0}", fi.Name);
        }

        /// <summary>
        /// 获取数据库列表
        /// </summary>
        /// <returns>数据库列表</returns>
        public static IMethodResult AdminGetDataBases()
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!DatabaseManager.IsAccessDB)
            {
                throw new DatabaseNotSupportException();
            }

            String[] files = Directory.GetFiles(DatabaseManager.AccessDataDirectory);
            Array.Sort<String>(files, Comparer<String>.Default);
            //recordCount = files.Length;

            List<FileInfo> fis = new List<FileInfo>();
            Int32 startIndex = 0;
            Int32 endIndex = files.Length - 1;

            //分页处理
            //Int32 startIndex = (pageIndex - 1) * pageSize;
            //Int32 endIndex = pageIndex * pageSize;

            //if (startIndex >= files.Length)
            //{
            //    return null;
            //}

            //if (endIndex >= files.Length)
            //{
            //    endIndex = files.Length - 1;
            //}

            for (Int32 i = startIndex; i <= endIndex; i++)
            {
                fis.Add(new FileInfo(files[i]));
            }

            return MethodResult.Success(fis);
        }

        /// <summary>
        /// 根据数据库ID获取数据库文件名
        /// </summary>
        /// <param name="did">数据库ID</param>
        /// <returns>数据库文件名</returns>
        public static String AdminGetDataBaseFileName(Int32 did)
        {
            String[] files = Directory.GetFiles(DatabaseManager.AccessDataDirectory);

            if (files.Length <= did)
            {
                throw new InvalidInputException("Database does not exist!");
            }

            Array.Sort<String>(files, Comparer<String>.Default);
            return Path.GetFileName(files[did]);
        }
        #endregion
    }
}