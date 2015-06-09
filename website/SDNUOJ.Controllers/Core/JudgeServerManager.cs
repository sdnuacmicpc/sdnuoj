using System;
using System.Collections.Generic;

using SDNUOJ.Controllers.Exception;
using SDNUOJ.Controllers.Status;
using SDNUOJ.Entity;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 评测机管理类
    /// </summary>
    internal static class JudgeServerManager
    {
        #region 管理方法
        /// <summary>
        /// 创建评测机账号
        /// </summary>
        /// <param name="userName">评测机ID</param>
        /// <returns>是否成功创建</returns>
        public static IMethodResult AdminCreateJudgeAccount(String userName)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            Boolean success = UserManager.InternalAdminUpdatePermision(userName, PermissionType.HttpJudge);

            if (!success)
            {
                return MethodResult.FailedAndLog("No judger was created!");
            }

            return MethodResult.SuccessAndLog("Admin create judger, name = {0}", userName);
        }

        /// <summary>
        /// 删除评测机账号
        /// </summary>
        /// <param name="userName">评测机ID</param>
        /// <returns>是否成功删除</returns>
        public static IMethodResult AdminDeleteJudgeAccount(String userName)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            Boolean success = UserManager.InternalAdminUpdatePermision(userName, PermissionType.None);

            if (!success)
            {
                return MethodResult.FailedAndLog("No judger was deleted!");
            }

            return MethodResult.SuccessAndLog("Admin delete judger, name = {0}", userName);
        }

        /// <summary>
        /// 当前评测机列表
        /// </summary>
        /// <returns>当前评测机列表</returns>
        public static List<UserEntity> AdminGetJudgers()
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            List<UserEntity> list = UserManager.InternalAdminGetJudgerList();

            if (list != null && list.Count > 0)
            {
                for (Int32 i = 0; i < list.Count; i++)
                {
                    list[i].LastOnline = JudgeOnlineStatus.GetJudgeLastTime(list[i].UserName);
                }
            }

            return list;
        }
        #endregion
    }
}