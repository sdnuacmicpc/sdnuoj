using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

using SDNUOJ.Controllers.Attributes;
using SDNUOJ.Controllers.Core;

namespace SDNUOJ.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdministrator")]
    [AccessDatabaseOnlyAttribute]
    public class DatabaseController : AdminBaseController
    {
        /// <summary>
        /// 数据库管理页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult List()
        {
            List<FileInfo> list = DatabaseManager.AdminGetDataBases();

            ViewBag.DefaultFileName = DatabaseManager.AccessDBFileName;

            return View(list);
        }

        /// <summary>
        /// 数据库压缩页面
        /// </summary>
        /// <param name="id">数据库ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Compact(Int32 id = -1)
        {
            ViewBag.FileName = (id >= 0 ? DatabaseManager.AdminGetDataBaseFileName(id) : DatabaseManager.AccessDBFileName);

            return View();
        }

        /// <summary>
        /// 数据库压缩
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Compact(FormCollection form)
        {
            if (DatabaseManager.AdminCompactDataBase(form["filename"]))
            {
                return RedirectToSuccessMessagePage("Your have compacted database successfully!");
            }
            else
            {
                return RedirectToSuccessMessagePage("Failed to compact database!");
            }
        }

        /// <summary>
        /// 数据库备份页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult Backup()
        {
            return View();
        }

        /// <summary>
        /// 数据库备份
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Backup(FormCollection form)
        {
            if (DatabaseManager.AdminBackupDataBase(form["filename"]))
            {
                return RedirectToSuccessMessagePage("Your have backuped database successfully!");
            }
            else
            {
                return RedirectToSuccessMessagePage("Failed to backup database!");
            }
        }

        /// <summary>
        /// 数据库还原页面
        /// </summary>
        /// <param name="id">数据库ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Restore(Int32 id = -1)
        {
            ViewBag.FileName = (id >= 0 ? DatabaseManager.AdminGetDataBaseFileName(id) : DatabaseManager.AccessDBFileName);

            return View();
        }

        /// <summary>
        /// 数据库还原
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Restore(FormCollection form)
        {
            if (DatabaseManager.AdminRestoreDataBase(form["filename"]))
            {
                return RedirectToSuccessMessagePage("Your have restored database successfully!");
            }
            else
            {
                return RedirectToSuccessMessagePage("Failed to restore database!");
            }
        }

        /// <summary>
        /// 数据库上传页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult Upload()
        {
            return View();
        }

        /// <summary>
        /// 数据库上传
        /// </summary>
        /// <param name="file">上传的文件</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            String fileNewName = String.Empty;

            if (DatabaseManager.AdminSaveUploadDataBase(file))
            {
                return RedirectToSuccessMessagePage("Your have uploaded database successfully!");
            }
            else
            {
                return RedirectToErrorMessagePage("Failed to upload database!");
            }
        }

        /// <summary>
        /// 数据库删除
        /// </summary>
        /// <param name="id">数据库ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Delete(Int32 id = -1)
        {
            return ResultToJson(() =>
            {
                DatabaseManager.AdminDeleteDataBase(id);
            });
        }

        /// <summary>
        /// 数据库删除
        /// </summary>
        /// <param name="id">数据库ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Download(Int32 id = -1)
        {
            String filePath = DatabaseManager.AdminGetDataBaseRealPath(id);

            return File(filePath, "application/msaccess", Path.GetFileName(filePath));
        }
    }
}