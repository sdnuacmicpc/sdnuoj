using System;
using System.Web;

using SDNUOJ.Controllers.Core;
using SDNUOJ.Controllers.Status;
using SDNUOJ.Entity;

namespace SDNUOJ.Controllers
{
    public static class GlobalAuthenticateModule
    {
        /// <summary>
        /// 替换系统认证模型
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <remarks>应放在Global.asax.cs的Application_AuthenticateRequest时执行</remarks>
        public static void ReplaceFormAuthenticateModel(HttpContext context)
        {
            UserStatus user = UserCurrentStatus.ReplaceFormAuthenticateModel(context);

            if (user != null && !AdminManager.InternalCheckPermission(user.Permission, PermissionType.HttpJudge))
            {
                Int32 unreadMailCount = UserMailManager.InternalCountUserUnReadMails(user.UserName);

                UserBrowserStatus.SetCurrentUserBrowserStatus(user.UserName, user.Permission, unreadMailCount);
            }
        }
    }
}
