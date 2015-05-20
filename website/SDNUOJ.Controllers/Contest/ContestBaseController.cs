using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Attributes;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Controllers;
using SDNUOJ.Entity;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Areas.Contest.Controllers
{
    [Function(PageType.Contest)]   
    public class ContestBaseController : BaseController
    {
        #region 方法
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            Int32 cid = filterContext.RouteData.GetInt32("cid", -1);

            if (cid <= 0)
            {
                throw new InvalidRequstException(RequestType.Contest);
            }

            ContestEntity contest = ContestManager.GetContest(cid);
            Boolean hasPermission = AdminManager.HasPermission(PermissionType.ContestManage);

            if (!hasPermission && contest.IsHide)
            {
                throw new NoPermissionException("You have no privilege to view this contest!");
            }

            if (!hasPermission && contest.StartTime > DateTime.Now)
            {
                throw new NoPermissionException("This contest has not been started yet!");
            }

            if (!hasPermission && (contest.ContestType == ContestType.Private || contest.ContestType == ContestType.RegisterPrivate))
            {
                if (!ContestUserManager.ContestContainsUser(contest.ContestID, UserManager.CurrentUserName))
                {
                    throw new NoPermissionException("You have no privilege to view this contest!");
                }
            }

            filterContext.ActionParameters["Contest"] = contest;
            ViewData["Contest"] = contest;
        }
        #endregion
    }
}