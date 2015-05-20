using System;
using System.Web;
using System.Web.Mvc;

using SDNUOJ.Controllers;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Entity;

namespace SDNUOJ.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminBaseController : BaseController
    {
        #region 属性
        /// <summary>
        /// 获取是否永远可以访问
        /// </summary>
        protected override Boolean IsAlwaysOpen { get { return true; } }

        /// <summary>
        /// 获取是否输出当前竞赛数量
        /// </summary>
        protected override Boolean OutputCurrentContestCount { get { return false; } }
        #endregion

        #region 方法
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            if (filterContext.Exception != null)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = RedirectToErrorMessagePage(filterContext.Exception.Message);
            }
        }
        
        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="msg">信息内容</param>
        /// <param name="backurl">返回转向页面</param>
        /// <param name="type">信息类型</param>
        protected override ActionResult RedirectToMessagePage(String msg, String backurl, MessageType type)
        {
            return RedirectToAction("Index", "Info", new
            {
                area = "Admin",
                c = HttpUtility.UrlEncode(msg),
                s = (type != MessageType.Generic ? type.ToString().ToLowerInvariant() : ""),
                b = backurl
            });
        }
        #endregion
    }
}