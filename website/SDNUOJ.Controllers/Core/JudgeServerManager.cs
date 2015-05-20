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
        public static Boolean CreateJudgeAccount(String userName)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            return UserManager.AdminUpdatePermision(userName, PermissionType.HttpJudge);
        }

        /// <summary>
        /// 删除评测机账号
        /// </summary>
        /// <param name="userName">评测机ID</param>
        /// <returns>是否成功删除</returns>
        public static Boolean DeleteJudgeAccount(String userName)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            return UserManager.AdminUpdatePermision(userName, PermissionType.None);
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