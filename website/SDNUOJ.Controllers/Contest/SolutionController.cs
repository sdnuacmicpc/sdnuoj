using System;
using System.Collections.Generic;
using System.Web.Mvc;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Attributes;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Entity;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Areas.Contest.Controllers
{
    public class SolutionController : ContestBaseController
    {
        /// <summary>
        /// 代码提交
        /// </summary>
        /// <param name="id">竞赛题目ID</param>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [Authorize]
        [ContestSubmit]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Submit(Int32 id, FormCollection form)
        {
            ContestEntity contest = ViewData["Contest"] as ContestEntity;
            ProblemEntity problem = ContestProblemManager.GetProblem(contest.ContestID, id);

            SolutionEntity entity = new SolutionEntity()
            {
                ProblemID = problem.ProblemID,
                ContestID = contest.ContestID,
                ContestProblemID = id,
                SourceCode = form["code"],
                LanguageType = LanguageType.FromLanguageID(form["lang"])
            };

            Dictionary<String, Byte> supportLanguages = LanguageManager.GetSupportLanguages(contest.SupportLanguage);

            if (!supportLanguages.ContainsValue(entity.LanguageType.ID))
            {
                throw new InvalidInputException("This contest does not support this programming language.");
            }

            String userip = this.GetCurrentUserIP();

            if (!SolutionManager.InsertSolution(entity, userip))
            {
                throw new OperationFailedException("Failed to submit your solution!");
            }

            return RedirectToAction("List", "Status", new { area = "Contest", cid = contest.ContestID });
        }
    }
}