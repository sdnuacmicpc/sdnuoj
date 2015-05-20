using System;
using System.Security.Principal;

using SDNUOJ.Entity;

namespace SDNUOJ.Controllers.Status
{
    /// <summary>
    /// 用户状态类
    /// </summary>
    public class UserStatus : IPrincipal, IIdentity
    {
        #region 常量
        /// <summary>
        /// 用户状态实体类版本
        /// </summary>
        internal const Int32 USER_STAUTS_VERSION = 1;
        #endregion

        #region 字段
        private String _userName;
        private PermissionType _permissionType;
        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置用户名
        /// </summary>
        public String UserName
        {
            get { return this._userName; }
            set { this._userName = value; }
        }

        /// <summary>
        /// 获取或设置权限类型
        /// </summary>
        public PermissionType Permission
        {
            get { return this._permissionType; }
            set { this._permissionType = value; }
        }

        /// <summary>
        /// 获取验证验证类型
        /// </summary>
        public String AuthenticationType
        {
            get { return "Forms"; }
        }

        /// <summary>
        /// 获取是否通过验证
        /// </summary>
        public Boolean IsAuthenticated
        {
            get { return true; }
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        String IIdentity.Name
        {
            get { return this.UserName; }
        }

        /// <summary>
        /// 获取标志对象实体
        /// </summary>
        public IIdentity Identity
        {
            get { return this; }
        }

        /// <summary>
        /// 判断是否拥有指定权限
        /// </summary>
        /// <param name="role">指定权限</param>
        /// <returns>是否拥有指定权限</returns>
        public Boolean IsInRole(String role)
        {
            PermissionType type = (PermissionType)Enum.Parse(typeof(PermissionType), role, true);
            return ((this._permissionType & type) == type);
        }

        /// <summary>
        /// 判断是否拥有指定权限
        /// </summary>
        /// <param name="role">指定权限</param>
        /// <returns>是否拥有指定权限</returns>
        public Boolean IsInRole(PermissionType type)
        {
            return ((this._permissionType & type) == type);
        }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化新的用户状态实体类
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="permission">用户权限</param>
        public UserStatus(String userName, PermissionType permission)
        {
            this._userName = userName;
            this._permissionType = permission;
        }
        #endregion
    }
}