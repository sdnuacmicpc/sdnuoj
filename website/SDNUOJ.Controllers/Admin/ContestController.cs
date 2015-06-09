using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;

namespace SDNUOJ.Areas.Admin.Controllers
{
    [Authorize(Roles = "ContestManage")]
    public class ContestController : AdminBaseController
    {
        /// <summary>
        /// 竞赛管理页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <returns>操作后的结果</returns>
        public ActionResult List(Int32 id = 1)
        {
            PagedList<ContestEntity> list = ContestManager.AdminGetContestList(id);

            return ViewWithPager(list, id);
        }

        /// <summary>
        /// 竞赛添加页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult Add()
        {
            DateTime now = DateTime.Today.AddHours(DateTime.Now.Hour + 1);
            ContestEntity entity = new ContestEntity()
            {
                StartTime = now,
                EndTime = now.AddHours(2)
            };

            ViewBag.AllSupportedLanguages = LanguageManager.GetAllLanguageNames();

            return View("Edit", entity);
        }

        /// <summary>
        /// 竞赛添加
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Add(FormCollection form)
        {
            ContestEntity entity = new ContestEntity()
            {
                Title = form["title"],
                ContestType = (ContestType)Byte.Parse(form["type"]),
                Description = form["description"],
                StartTime = DateTime.Parse(String.Format("{0} {1}:{2}:{3}", form["startdate"], form["starthour"], form["startminute"], form["startsecond"])),
                EndTime = DateTime.Parse(String.Format("{0} {1}:{2}:{3}", form["enddate"], form["endhour"], form["endminute"], form["endsecond"])),
                RegisterStartTime = String.IsNullOrEmpty(form["regstartdate"]) ? new Nullable<DateTime>() : DateTime.Parse(String.Format("{0} {1}:{2}:{3}", form["regstartdate"], form["regstarthour"], form["regstartminute"], form["regstartsecond"])),
                RegisterEndTime = String.IsNullOrEmpty(form["regenddate"]) ? new Nullable<DateTime>() : DateTime.Parse(String.Format("{0} {1}:{2}:{3}", form["regenddate"], form["regendhour"], form["regendminute"], form["regendsecond"])),
                SupportLanguage = form["supportlangs"]
            };

            return ResultToMessagePage(ContestManager.AdminInsertContest, entity, "Your have added contest successfully!");
        }

        /// <summary>
        /// 竞赛编辑页面
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Edit(Int32 id = -1)
        {
            ViewBag.AllSupportedLanguages = LanguageManager.GetAllLanguageNames();

            return View("Edit", ContestManager.AdminGetContest(id));
        }

        /// <summary>
        /// 竞赛编辑
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Int32 id, FormCollection form)
        {
            ContestEntity entity = new ContestEntity()
            {
                ContestID = id,
                Title = form["title"],
                ContestType = (ContestType)Byte.Parse(form["type"]),
                Description = form["description"],
                StartTime = DateTime.Parse(String.Format("{0} {1}:{2}:{3}", form["startdate"], form["starthour"], form["startminute"], form["startsecond"])),
                EndTime = DateTime.Parse(String.Format("{0} {1}:{2}:{3}", form["enddate"], form["endhour"], form["endminute"], form["endsecond"])),
                RegisterStartTime = String.IsNullOrEmpty(form["regstartdate"]) ? new Nullable<DateTime>() : DateTime.Parse(String.Format("{0} {1}:{2}:{3}", form["regstartdate"], form["regstarthour"], form["regstartminute"], form["regstartsecond"])),
                RegisterEndTime = String.IsNullOrEmpty(form["regenddate"]) ? new Nullable<DateTime>() : DateTime.Parse(String.Format("{0} {1}:{2}:{3}", form["regenddate"], form["regendhour"], form["regendminute"], form["regendsecond"])),
                SupportLanguage = form["supportlangs"]
            };

            return ResultToMessagePage(ContestManager.AdminUpdateContest, entity, "Your have edited contest successfully!");
        }

        /// <summary>
        /// 竞赛隐藏
        /// </summary>
        /// <param name="ids">竞赛ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Hide(String ids)
        {
            return ResultToJson(ContestManager.AdminUpdateContestIsHide, ids, true);
        }

        /// <summary>
        /// 竞赛显示
        /// </summary>
        /// <param name="ids">竞赛ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Show(String ids)
        {
            return ResultToJson(ContestManager.AdminUpdateContestIsHide, ids, false);
        }

        /// <summary>
        /// 竞赛导出页面
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Export(String id)
        {
            ViewBag.ContestID = id;

            return View();
        }

        /// <summary>
        /// 竞赛导出
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Export(Int32 id, FormCollection form)
        {
            String usernames = String.Empty;

            if (String.Equals(form["enablerealname"], "1"))
            {
                usernames = ContestUserManager.AdminExportContestUserList(id, "2,4", false);
            }
            else if (String.Equals(form["enablerealname"], "2"))
            {
                usernames = form["usernames"];
            }

            Byte[] data = ContestManager.AdminGetExportRanklist(id, usernames);

            return File(data, "application/vnd.ms-excel", id.ToString() + ".xls");
        }

        /// <summary>
        /// 竞赛题目页面
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Problem(Int32 id = -1)
        {
            List<ContestProblemEntity> list = ContestProblemManager.AdminGetContestProblemList(id);

            ViewBag.ContestID = (id >= 0 ? id.ToString() : "");

            return View(list);
        }

        /// <summary>
        /// 竞赛题目编辑
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProblemEdit(Int32 id, FormCollection form)
        {
            return ResultToMessagePage(ContestProblemManager.AdminSetContestProblemList, id, form["problemids"], "Your have edited contest problem list successfully!");
        }

        /// <summary>
        /// 竞赛用户管理页面
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="id">页面索引</param>
        /// <returns>操作后的结果</returns>
        public ActionResult UserList(Int32 cid, Int32 id = 1)
        {
            PagedList<ContestUserEntity> list = ContestUserManager.AdminGetContestUserList(cid, id);

            ViewBag.ContestID = cid;

            return ViewWithPager(list, id);
        }

        /// <summary>
        /// 竞赛用户添加页面
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult UserAdd(String id)
        {
            ViewBag.ContestID = id;

            return View();
        }

        /// <summary>
        /// 竞赛用户添加
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserAdd(Int32 id, FormCollection form)
        {
            return ResultToMessagePage(() =>
            {
                IMethodResult result = ContestUserManager.AdminInsertContestUsers(id, form["usernames"]);

                if (!result.IsSuccess)
                {
                    return new Tuple<IMethodResult, String>(result, String.Empty);
                }

                String successInfo = String.Format("{0} contest user(s) have been successfully added!", result.ResultObject.ToString());

                return new Tuple<IMethodResult, String>(result, successInfo);
            });
        }

        /// <summary>
        /// 竞赛用户导出页面
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult UserExport(String id)
        {
            ViewBag.ContestID = id;

            return View();
        }

        /// <summary>
        /// 竞赛用户导出
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserExport(Int32 id, FormCollection form)
        {
            String content = ContestUserManager.AdminExportContestUserList(id, form["maskcode"], String.Equals(form["withtitle"], "1"));
            Byte[] data = Encoding.UTF8.GetBytes(content);

            return File(data, "application/vnd.ms-excel", id.ToString() + ".txt");
        }

        /// <summary>
        /// 竞赛用户启用
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <param name="ids">用户名</param>
        /// <returns>操作后的结果</returns>
        public ActionResult UserEnable(Int32 id, String ids)
        {
            return ResultToJson(ContestUserManager.AdminUpdateContestUsers, id, ids, true);
        }

        /// <summary>
        /// 竞赛用户禁用
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <param name="ids">用户名</param>
        /// <returns>操作后的结果</returns>
        public ActionResult UserDisable(Int32 id, String ids)
        {
            return ResultToJson(ContestUserManager.AdminUpdateContestUsers, id, ids, false);
        }

        /// <summary>
        /// 竞赛用户删除
        /// </summary>
        /// <param name="id">竞赛ID</param>
        /// <param name="ids">用户名</param>
        /// <returns>操作后的结果</returns>
        public ActionResult UserDelete(Int32 id, String ids)
        {
            return ResultToJson(ContestUserManager.AdminDeleteContestUsers, id, ids);
        }
    }
}