using System;
using System.Web;
using System.Web.Mvc;

namespace SDNUOJ.Controllers.Attributes
{
    /// <summary>
    /// 禁用客户端本地缓存特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class NoClientCacheAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpResponseBase Response = filterContext.HttpContext.Response;

            Response.Expires = 0;
            Response.AppendHeader("Pragma", "No-Cache");
            Response.CacheControl = "No-Cache";
        }
    }
}