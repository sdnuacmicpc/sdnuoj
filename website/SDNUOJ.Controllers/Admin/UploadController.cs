using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Utilities;

namespace SDNUOJ.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UploadController : AdminBaseController
    {
        /// <summary>
        /// 上传管理页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <returns>操作后的结果</returns>
        public ActionResult List(Int32 id = 1)
        {
            PagedList<FileInfo> list = UploadsManager.AdminGetUploadFiles(id);

            ViewBag.RootPath = ConfigurationManager.UploadDirectoryUrl;

            ViewBag.PageSize = list.PageSize;
            ViewBag.RecordCount = list.RecordCount;
            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = id;

            return View(list);
        }

        /// <summary>
        /// 上传文件页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file">上传的文件</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New(HttpPostedFileBase file)
        {
            return ResultToMessagePage(() => 
            {
                IMethodResult result = UploadsManager.AdminSaveUploadFile(file);

                if (!result.IsSuccess)
                {
                    return new Tuple<IMethodResult, String>(result, String.Empty);
                }

                String fileUrl = UploadsManager.AdminPreviewUploadFileUrl(result.ResultObject.ToString());
                String successInfo = String.Format("Your have uploaded file successfully!<br/>Your file url:<a href=\"{0}\" target=\"_blank\">{0}</a>", fileUrl);

                return new Tuple<IMethodResult, String>(result, successInfo);
            });
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Delete(String name)
        {
            return ResultToJson(UploadsManager.AdminDeleteUploadFile, name);
        }
    }
}