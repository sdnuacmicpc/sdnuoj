using System;
using System.Collections.Generic;
using System.Web.Mvc;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Attributes;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Entity;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Controllers
{
    public class SolutionController : BaseController
    {
        /// <summary>
        /// 代码显示页面
        /// </summary>
        /// <param name="id">提交ID</param>
        /// <returns>操作后的结果</returns>
        [Function(PageType.SourceView)]
        [Authorize]
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "id", VaryByCustom = "nm")]
        public ActionResult View(Int32 id = -1)
        {
            return View(SolutionManager.GetSourceCode(id));
        }

        /// <summary>
        /// 代码错误信息显示页面
        /// </summary>
        /// <param name="id">提交ID</param>
        /// <returns>操作后的结果</returns>
        [Function(PageType.MainStatus)]
        [OutputCache(CacheProfile = "DynamicPageCache", VaryByParam = "id")]
        public ActionResult Info(Int32 id = -1)
        {
            return View(SolutionErrorManager.GetSolutionError(id));
        }

        /// <summary>
        /// 获取当前用户所有题目提交列表
        /// </summary>
        /// <returns>题目列表Json</returns>
        [Authorize]
        public ActionResult SubmitList()
        {
            Tuple<List<Int32>, List<Int32>> list = SolutionManager.GetUserSubmitList();

            var obj = (list == null ? null : new
            {
                solved = list.Item1,
                unsolved = list.Item2
            });

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取当前用户所有通过的源代码下载
        /// </summary>
        /// <returns>源代码压缩包</returns>
        [Authorize]
        public ActionResult SourceCodes()
        {
            Byte[] data = SolutionManager.GetAcceptedSourceCodeList();

            return File(data, "application/zip", "SourceCodes.zip");
        }

        /// <summary>
        /// 代码提交
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [Function(PageType.MainSubmit)]
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Submit(Int32 id, FormCollection form)
        {
            SolutionEntity entity = new SolutionEntity()
            {
                ProblemID = id,
                SourceCode = form["code"],
                LanguageType = LanguageType.FromLanguageID(form["lang"])
            };
            
            Dictionary<String, Byte> supportLanguages = LanguageManager.MainSubmitSupportLanguages;

            if (!supportLanguages.ContainsValue(entity.LanguageType.ID))
            {
                throw new InvalidInputException("This problem does not support this programming language.");
            }

            String userip = this.GetCurrentUserIP();

            if (!SolutionManager.InsertSolution(entity, userip))
            {
                throw new OperationFailedException("Failed to submit your solution!");
            }

            return RedirectToAction("List", "Status");
        }
    }
}