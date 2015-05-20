using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Core;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Entity;

namespace SDNUOJ.Controllers.Attributes
{
    /// <summary>
    /// 竞赛需要提交特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ContestSubmitAttribute : ActionFilterAttribute
    {
        #region 方法
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ContestEntity contest = filterContext.ActionParameters["Contest"] as ContestEntity;
            Boolean hasPermission = AdminManager.HasPermission(PermissionType.ContestManage);

            if (contest.EndTime < DateTime.Now)
            {
                throw new NoPermissionException("This contest has ended!");
            }

            if (!hasPermission && contest.ContestType == ContestType.RegisterPublic)
            {
                if (!ContestUserManager.ContestContainsUser(contest.ContestID, UserManager.CurrentUserName))
                {
                    throw new NoPermissionException("You have no privilege to submit in this contest!");
                }
            }
        }
        #endregion
    }
}