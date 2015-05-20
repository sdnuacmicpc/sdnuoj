using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;

using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;

namespace SDNUOJ.Areas.Admin.Controllers
{
    [Authorize(Roles = "ProblemManage")]
    public class ProblemController : AdminBaseController
    {
        /// <summary>
        /// 题目管理页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <returns>操作后的结果</returns>
        public ActionResult List(Int32 id = 1)
        {
            PagedList<ProblemEntity> list = ProblemManager.AdminGetProblemList(id);

            ViewBag.PageSize = list.PageSize;
            ViewBag.RecordCount = list.RecordCount;
            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = id;

            return View(list);
        }

        /// <summary>
        /// 题目添加页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult Add()
        {
            ProblemEntity entity = new ProblemEntity()
            {
                TimeLimit = 1000,
                MemoryLimit = 32768
            };

            return View("Edit", entity);
        }

        /// <summary>
        /// 题目添加
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Add(FormCollection form)
        {
            ProblemEntity entity = new ProblemEntity()
            {
                Title = form["title"],
                Description = form["description"],
                Input = form["input"],
                Output = form["output"],
                SampleInput = form["samplein"],
                SampleOutput = form["sampleout"],
                Hint = form["hint"],
                Source = form["source"],
                TimeLimit = form["timelimit"].ToInt32(1000),
                MemoryLimit = form["memorylimit"].ToInt32(32768)
            };

            if (ProblemManager.AdminInsertProblem(entity))
            {
                return RedirectToSuccessMessagePage("Your have added problem successfully!");
            }
            else
            {
                return RedirectToErrorMessagePage("Failed to add problem!");
            }
        }

        /// <summary>
        /// 题目编辑页面
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Edit(Int32 id = -1)
        {
            return View("Edit", ProblemManager.AdminGetProblem(id));
        }

        /// <summary>
        /// 题目编辑
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Int32 id, FormCollection form)
        {
            ProblemEntity entity = new ProblemEntity()
            {
                ProblemID = id,
                Title = form["title"],
                Description = form["description"],
                Input = form["input"],
                Output = form["output"],
                SampleInput = form["samplein"],
                SampleOutput = form["sampleout"],
                Hint = form["hint"],
                Source = form["source"],
                TimeLimit = form["timelimit"].ToInt32(1000),
                MemoryLimit = form["memorylimit"].ToInt32(32768)
            };

            if (ProblemManager.AdminUpdateProblem(entity))
            {
                return RedirectToSuccessMessagePage("Your have edited problem successfully!");
            }
            else
            {
                return RedirectToErrorMessagePage("Failed to edit problem!");
            }
        }

        /// <summary>
        /// 题目导入页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        [Authorize(Roles = "SuperAdministrator")]
        public ActionResult Import()
        {
            return View();
        }

        /// <summary>
        /// 题目导入
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <param name="file">上传的文件</param>
        /// <returns>操作后的结果</returns>
        [Authorize(Roles = "SuperAdministrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Import(FormCollection form, HttpPostedFileBase file)
        {
            Dictionary<Int32, Boolean> result = ProblemManager.AdminImportProblem(Request, form["filetype"], form["uploadtype"], form["content"], file);

            if (result != null)
            {
                StringBuilder sb = new StringBuilder();
                Int32 nodatacount = 0;

                foreach (KeyValuePair<Int32, Boolean> pair in result)
                {
                    if (!pair.Value)
                    {
                        sb.Append(sb.Length > 0 ? "," : "").Append(pair.Key.ToString());
                        nodatacount++;
                    }
                }

                return RedirectToSuccessMessagePage(String.Format("{0} problem(s) have benn successfully imported!{1}",
                    result.Count.ToString(),
                    nodatacount > 0 ? String.Format("<br/>{0} problem(s) ({1}) have no data or fail to import these data!", nodatacount.ToString(), sb.ToString()) : ""));
            }
            else
            {
                return RedirectToErrorMessagePage("Failed to import problem!");
            }
        }

        /// <summary>
        /// 题目隐藏
        /// </summary>
        /// <param name="ids">题目ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Hide(String ids)
        {
            return ResultToJson(() =>
            {
                ProblemManager.AdminUpdateProblemIsHide(ids, true);
            });
        }

        /// <summary>
        /// 题目显示
        /// </summary>
        /// <param name="ids">题目ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Show(String ids)
        {
            return ResultToJson(() =>
            {
                ProblemManager.AdminUpdateProblemIsHide(ids, false);
            });
        }

        /// <summary>
        /// 题目统计计算
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Recalculate(Int32 id = -1)
        {
            return ResultToJson(() =>
            {
                ProblemManager.AdminUpdateProblemSolvedCount(id);
                ProblemManager.AdminUpdateProblemSubmitCount(id);
            });
        }

        /// <summary>
        /// 题目数据删除
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult DataDelete(Int32 id = -1)
        {
            return ResultToJson(() =>
            {
                ProblemDataManager.AdminDeleteProblemDataRealPath(id);
            });
        }

        /// <summary>
        /// 题目数据下载
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult DataDownload(Int32 id = -1)
        {
            String dataPath = ProblemDataManager.AdminGetProblemDataRealPath(id);

            if (!String.IsNullOrEmpty(dataPath))
            {
                return File(dataPath, "application/zip", id.ToString() + ".zip");
            }
            else
            {
                return RedirectToErrorMessagePage("This problem doesn't have data!");
            }
        }

        /// <summary>
        /// 题目分类管理页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <returns>操作后的结果</returns>
        public ActionResult CategoryList(Int32 id = 1)
        {
            PagedList<ProblemCategoryEntity> list = ProblemCategoryManager.AdminGetProblemCategoryList(id);

            ViewBag.PageSize = list.PageSize;
            ViewBag.RecordCount = list.RecordCount;
            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = id;

            return View(list);
        }

        /// <summary>
        /// 题目分类添加页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult CategoryAdd()
        {
            ProblemCategoryEntity entity = new ProblemCategoryEntity()
            {
                Order = 255
            };

            return View("CategoryEdit", entity);
        }

        /// <summary>
        /// 题目分类添加
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryAdd(FormCollection form)
        {
            ProblemCategoryEntity entity = new ProblemCategoryEntity()
            {
                Title = form["title"],
                Order = form["order"].ToInt32(0)
            };

            if (ProblemCategoryManager.AdminInsertProblemCategory(entity))
            {
                return RedirectToSuccessMessagePage("Your have added problem category successfully!");
            }
            else
            {
                return RedirectToErrorMessagePage("Failed to add problem category!");
            }
        }

        /// <summary>
        /// 题目分类编辑页面
        /// </summary>
        /// <param name="id">题目分类ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult CategoryEdit(Int32 id = -1)
        {
            return View("CategoryEdit", ProblemCategoryManager.AdminGetProblemCategory(id));
        }

        /// <summary>
        /// 题目分类编辑
        /// </summary>
        /// <param name="id">题目分类ID</param>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryEdit(Int32 id, FormCollection form)
        {
            ProblemCategoryEntity entity = new ProblemCategoryEntity()
            {
                TypeID = id,
                Title = form["title"],
                Order = form["order"].ToInt32(0)
            };

            if (ProblemCategoryManager.AdminUpdateProblemCategory(entity))
            {
                return RedirectToSuccessMessagePage("Your have edited problem category successfully!");
            }
            else
            {
                return RedirectToErrorMessagePage("Failed to edit problem category!");
            }
        }

        /// <summary>
        /// 题目分类删除
        /// </summary>
        /// <param name="id">题目分类ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult CategoryDelete(Int32 id)
        {
            return ResultToJson(() =>
            {
                ProblemCategoryManager.AdminDeleteProblemCategory(id);
            });
        }

        /// <summary>
        /// 题目分类编辑页面
        /// </summary>
        /// <param name="id">题目分类ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult CategorySet(Int32 id = -1)
        {
            List<ProblemCategoryEntity> selected, unselected;

            ViewBag.Source = ProblemCategoryItemManager.AdminGetProblemCategoryItemList(id, out selected, out unselected);
            ViewBag.ProblemID = (id >= 0 ? id.ToString() : "");

            return View(new Tuple<List<ProblemCategoryEntity>, List<ProblemCategoryEntity>>(unselected, selected));
        }

        /// <summary>
        /// 题目分类设置
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategorySet(Int32 id, FormCollection form)
        {
            if (ProblemCategoryItemManager.AdminUpdateProblemCategoryItems(id, form["source"], form["target"]))
            {
                return RedirectToSuccessMessagePage("Your have updated problem type successfully!");
            }
            else
            {
                return RedirectToErrorMessagePage("Failed to update problem type!");
            }
        }

        /// <summary>
        /// 题目数据创建页面
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult DataCreate(String id)
        {
            ViewBag.ProblemID = id;

            return View();
        }

        /// <summary>
        /// 题目数据创建
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult DataCreate(Int32 id, FormCollection form)
        {
            if (ProblemDataManager.AdminSaveProblemData(id, form, Request.Files))
            {
                return RedirectToSuccessMessagePage("Your have created problem data successfully!");
            }
            else
            {
                return RedirectToErrorMessagePage("Failed to create problem data!");
            }
        }

        /// <summary>
        /// 题目数据导入页面
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult DataUpload(String id)
        {
            ViewBag.ProblemID = id;

            return View();
        }

        /// <summary>
        /// 题目数据导入
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <param name="file">上传文件</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DataUpload(Int32 id, HttpPostedFileBase file)
        {
            if (ProblemDataManager.AdminSaveProblemData(id, file))
            {
                return RedirectToSuccessMessagePage("Your have uploaded problem data successfully!");
            }
            else
            {
                return RedirectToErrorMessagePage("Failed to upload problem data!");
            }
        }
    }
}