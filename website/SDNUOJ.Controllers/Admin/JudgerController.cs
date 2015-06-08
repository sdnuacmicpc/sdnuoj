using System;
using System.Collections.Generic;
using System.Web.Mvc;

using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;

namespace SDNUOJ.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdministrator")]
    public class JudgerController : AdminBaseController
    {
        /// <summary>
        /// 评测机管理页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult List()
        {
            List<UserEntity> list = JudgeServerManager.AdminGetJudgers();

            return View(list);
        }

        /// <summary>
        /// 评测机添加页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// 评测机添加
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(FormCollection form)
        {
            return ResultToMessagePage(JudgeServerManager.CreateJudgeAccount, form["username"], "Your have created judger successfully!");
        }

        /// <summary>
        /// 评测机删除
        /// </summary>
        /// <param name="id">评测机用户名</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Delete(String id)
        {
            return ResultToJson(JudgeServerManager.DeleteJudgeAccount, id);
        }
    }
}