using System;

using SDNUOJ.Controllers.Status;
using SDNUOJ.Entity;

namespace SDNUOJ.Controllers.Core
{
    internal static class AdminManager
    {
        #region 常量
        /// <summary>
        /// 后台管理列表页面大小
        /// </summary>
        internal const Int32 ADMIN_LIST_PAGE_SIZE = 20;
        #endregion

        #region 管理方法
        /// <summary>
        /// 验证是否有指定权限
        /// </summary>
        /// <param name="type">指定权限</param>
        /// <returns>是否有指定权限</returns>
        public static Boolean HasPermission(PermissionType type)
        {
            UserStatus user = UserManager.CurrentUser;
            PermissionType userPermission = (user != null ? UserManager.InternalAdminGetPermissionByName(user.UserName) : PermissionType.None);

            return ((userPermission & type) == type);
        }

        /// <summary>
        /// 获取逗号分隔的权限
        /// </summary>
        /// <param name="permissions">逗号分隔的权限</param>
        /// <returns>指定权限</returns>
        public static PermissionType GetPermission(String permissions)
        {
            PermissionType permission = PermissionType.None;

            if (!String.IsNullOrEmpty(permissions))
            {
                String[] arr = permissions.Split(',');
                Int32 temp = 0;

                for (Int32 i = 0; i < arr.Length; i++)
                {
                    Int32.TryParse(arr[i].Trim(), out temp);
                    permission |= (PermissionType)temp;
                }
            }

            if (permission > PermissionType.Administrator)
            {
                permission |= PermissionType.Administrator;//管理员自动添加基本管理权限
            }

            return permission;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 验证是否有指定权限
        /// </summary>
        /// <param name="userPermission">用户权限</param>
        /// <param name="type">指定权限</param>
        /// <returns>是否有指定权限</returns>
        internal static Boolean InternalCheckPermission(PermissionType userPermission, PermissionType type)
        {
            return ((userPermission & type) == type);
        }
        #endregion
    }
}
