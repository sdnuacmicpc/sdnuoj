using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Core;
using SDNUOJ.Controllers.Exception;

namespace SDNUOJ.Controllers.Attributes
{
    /// <summary>
    /// 仅限Access数据库特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AccessDatabaseOnlyAttribute : ActionFilterAttribute
    {
        #region 方法
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!DatabaseManager.IsAccessDB)
            {
                throw new DatabaseNotSupportException();
            }
        }
        #endregion
    }
}