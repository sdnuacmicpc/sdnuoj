using System;
using System.Collections.Generic;
using System.Web;

using SDNUOJ.Caching;
using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Controllers.Status;
using SDNUOJ.Data;
using SDNUOJ.Entity;
using SDNUOJ.Logging;
using SDNUOJ.Utilities;
using SDNUOJ.Utilities.Security;
using SDNUOJ.Utilities.Text;
using SDNUOJ.Utilities.Text.RegularExpressions;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 用户数据管理类
    /// </summary>
    /// <remarks>
    /// HtmlEncode : 用户昵称 / 保存在数据库中
    /// </remarks>
    internal static class UserManager
    {
        #region 常量
        /// <summary>
        /// 首页显示的用户数量
        /// </summary>
        private const Int32 HOME_PAGE_PAGE_SIZE = 10;

        /// <summary>
        /// 用户排行页面显示的用户数量
        /// </summary>
        private const Int32 RANKLIST_PAGE_SIZE = 50;
        #endregion

        #region 属性
        /// <summary>
        /// 获取当前是否登陆
        /// </summary>
        public static Boolean IsUserLogined
        {
            get { return UserCurrentStatus.GetIsUserLogined(); }
        }

        /// <summary>
        /// 获取当前登陆的用户名
        /// </summary>
        public static String CurrentUserName
        {
            get { return UserCurrentStatus.GetCurrentUserName(); }
        }

        /// <summary>
        /// 获取当前登陆的用户实体
        /// </summary>
        public static UserStatus CurrentUser
        {
            get { return UserCurrentStatus.GetCurrentUserStatus(); }
        }
        #endregion

        #region 登陆登出方法
        /// <summary>
        /// 尝试将使用用户名密码登陆系统
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">密码</param>
        /// <param name="user">若成功返回用户实体</param>
        /// <returns>失败则返回出错信息，成功则不返回任何信息</returns>
        public static String TryGetUserByUsernameAndPassword(String userName, String passWord, out UserEntity user)
        {
            user = null;

            try
            {
                if (String.IsNullOrEmpty(userName))
                {
                    return "Username can not be NULL!";
                }

                if (String.IsNullOrEmpty(passWord))
                {
                    return "Password can not be NULL!";
                }

                if (!RegexVerify.IsUserName(userName) || !SQLValidator.IsNonNullANDSafe(userName))
                {
                    return "Username is INVALID!";
                }

                passWord = PassWordEncrypt.Encrypt(userName, passWord);
                user = UserRepository.Instance.GetEntityByNameAndPassword(userName, passWord);

                if (user == null)
                {
                    return "No such user or wrong password!";
                }

                if (!String.Equals(user.PassWord, passWord, StringComparison.OrdinalIgnoreCase))
                {
                    return "Password is wrong!";
                }

                if (user.IsLocked)
                {
                    return "The user is locked, please contact the administrator!";
                }

                if ("NULL".Equals(user.PassWord, StringComparison.OrdinalIgnoreCase))
                {
                    return "The user's password is INVALID, please visit \"Forget Password\" and reset your password!";
                }

                return String.Empty;
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 更新用户最后登录信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="lastip">最后登录IP</param>
        public static void UpdateLoginInfomation(String userName, String lastip)
        {
            UserRepository.Instance.UpdateEntityLoginInfomation(userName, lastip, DateTime.Now);
        }

        /// <summary>
        /// 尝试将使用用户名密码登陆系统
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">密码</param>
        /// <param name="userip">用户IP</param>
        /// <returns>返回是否成功登陆，若失败则返回出错信息</returns>
        public static IMethodResult SignIn(String userName, String passWord, String userip)
        {
            UserEntity user = null;
            String error = TryGetUserByUsernameAndPassword(userName, passWord, out user);

            if (!String.IsNullOrEmpty(error))
            {
                return MethodResult.FailedAndLog(error);
            }

            if (AdminManager.InternalCheckPermission(user.Permission, PermissionType.HttpJudge))
            {
                return MethodResult.FailedAndLog("You can not login a httpjudge account!");
            }

            try
            {
                Int32 unreadMailCount = UserMailManager.InternalCountUserUnReadMails(user.UserName);

                UserBrowserStatus.SetCurrentUserBrowserStatus(user.UserName, user.Permission, unreadMailCount);
                UserSubmitStatus.InitLastSubmitTime(user.UserName);
                UserCurrentStatus.SetCurrentUserStatus(user);

                UpdateLoginInfomation(userName, userip);
            }
            catch (System.Exception ex)
            {
                return MethodResult.Failed(ex.Message);
            }

            return MethodResult.SuccessAndLog("User sign in");
        }

        /// <summary>
        /// 用户登出系统
        /// </summary>
        public static void SignOut()
        {
            String userName = UserManager.CurrentUserName;

            UserCurrentStatus.RemoveCurrentUserStatus();
            UserBrowserStatus.RemoveCurrentUserBrowserStatus();
            UserSubmitStatus.RemoveLastSubmitTime(userName);
            UserMailCache.RemoveUserUnReadMailCountCache(userName);
        }
        #endregion

        #region 注册方法
        /// <summary>
        /// 尝试注册用户
        /// </summary>
        /// <param name="entity">用户实体</param>
        /// <param name="password">密码</param>
        /// <param name="password2">重复密码</param>
        /// <param name="checkCode">验证码</param>
        /// <param name="userip">用户IP</param>
        /// <returns>执行结果</returns>
        public static IMethodResult SignUp(UserEntity entity, String password, String password2, String checkCode, String userip)
        {
            if (!CheckCodeStatus.VerifyCheckCode(checkCode))
            {
                return MethodResult.Failed("The verification code you input didn't match the picture, Please try again!");
            }

            if (String.IsNullOrEmpty(entity.UserName))
            {
                return MethodResult.Failed("Username can not be NULL!");
            }

            if (!RegexVerify.IsUserName(entity.UserName) || !SQLValidator.IsNonNullANDSafe(entity.UserName))
            {
                return MethodResult.Failed("Username can not contain illegal characters!");
            }

            if (!KeywordsFilterManager.IsUserNameLegal(entity.UserName))
            {
                return MethodResult.Failed("Username can not contain illegal keywords!");
            }

            if (entity.UserName.Length > UserRepository.USERNAME_MAXLEN)
            {
                return MethodResult.Failed("Username is too long!");
            }

            if (String.IsNullOrEmpty(password))
            {
                return MethodResult.Failed("Password can not be NULL!");
            }

            if (!String.Equals(password, password2))
            {
                return MethodResult.Failed("Two passwords are not match!");
            }

            if (String.IsNullOrEmpty(entity.Email))
            {
                return MethodResult.Failed("Email address can not be NULL!");
            }

            if (!RegexVerify.IsEmail(entity.Email))
            {
                return MethodResult.Failed("Email address is INVALID!");
            }

            if (entity.Email.Length > UserRepository.EMAIL_MAXLEN)
            {
                return MethodResult.Failed("Email address is too long!");
            }

            if (!String.IsNullOrEmpty(entity.NickName) && entity.NickName.Length > UserRepository.NICKNAME_MAXLEN)
            {
                return MethodResult.Failed("Nick Name is too long!");
            }

            if (!KeywordsFilterManager.IsUserNameLegal(entity.NickName))
            {
                return MethodResult.Failed("Nick Name can not contain illegal keywords!");
            }

            if (!String.IsNullOrEmpty(entity.School) && entity.School.Length > UserRepository.SCHOOL_MAXLEN)
            {
                return MethodResult.Failed("School Name is too long!");
            }

            if (UserRepository.Instance.ExistsEntity(entity.UserName))
            {
                return MethodResult.Failed("The username \"{0}\" has already existed!", entity.UserName);
            }

            if (!UserIPStatus.CheckLastRegisterTime(userip))
            {
                return MethodResult.Failed("You can only register one user from single ip in {0} seconds!", ConfigurationManager.RegisterInterval.ToString());
            }

            entity.PassWord = PassWordEncrypt.Encrypt(entity.UserName, password);
            entity.NickName = HtmlEncoder.HtmlEncode(entity.NickName);
            entity.Permission = PermissionType.None;
            entity.CreateIP = userip;
            entity.CreateDate = DateTime.Now;

            try
            {
                if (UserRepository.Instance.InsertEntity(entity) == 0)
                {
                    return MethodResult.Failed("User Registration Failed!");
                }
            }
            catch (System.Exception ex)
            {
                return MethodResult.Failed(ex.Message);
            }

            UserCache.RemoveRanklistUserCountCache();//删除缓存

            return MethodResult.SuccessAndLog("User sign up");
        }
        #endregion

        #region 更新用户信息方法
        /// <summary>
        /// 尝试更新用户信息
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <param name="currentPassword">当前密码</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="newPassword2">重复新密码</param>
        /// <param name="result">执行结果</param>
        /// <returns>执行结果</returns>
        public static IMethodResult UpdateUserInfo(UserEntity entity, String currentPassword, String newPassword, String newPassword2)
        {
            if (String.IsNullOrEmpty(currentPassword))
            {
                return MethodResult.Failed("Current password can not be NULL!");
            }
            else
            {
                entity.UserName = UserManager.CurrentUserName;
                entity.NickName = HtmlEncoder.HtmlEncode(entity.NickName);
                currentPassword = PassWordEncrypt.Encrypt(entity.UserName, currentPassword);
            }

            if (!String.Equals(newPassword, newPassword2))
            {
                return MethodResult.Failed("Two new passwords are not match!");
            }

            if (String.IsNullOrEmpty(entity.Email))
            {
                return MethodResult.Failed("Email address can not be NULL!");
            }

            if (!RegexVerify.IsEmail(entity.Email))
            {
                return MethodResult.Failed("Email address is INVALID!");
            }

            if (entity.Email.Length > UserRepository.EMAIL_MAXLEN)
            {
                return MethodResult.Failed("Email address is too long!");
            }

            if (!String.IsNullOrEmpty(entity.NickName) && entity.NickName.Length > UserRepository.NICKNAME_MAXLEN)
            {
                return MethodResult.Failed("Nick Name is too long!");
            }

            if (!KeywordsFilterManager.IsUserNameLegal(entity.NickName))
            {
                return MethodResult.Failed("Nick Name can not contain illegal keywords!");
            }

            if (!String.IsNullOrEmpty(entity.School) && entity.School.Length > UserRepository.SCHOOL_MAXLEN)
            {
                return MethodResult.Failed("School Name is too long!");
            }

            if (!String.IsNullOrEmpty(newPassword))
            {
                entity.PassWord = PassWordEncrypt.Encrypt(entity.UserName, newPassword);
            }

            try
            {
                if (UserRepository.Instance.UpdateEntityForUser(entity, currentPassword) <= 0)
                {
                    return MethodResult.Failed("Current password is wrong!");
                }
            }
            catch (System.Exception ex)
            {
                return MethodResult.Failed(ex.Message);
            }

            return MethodResult.SuccessAndLog("User update info");
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">新密码</param>
        /// <returns>是否成功更新</returns>
        internal static Boolean InternalResetUserPassword(String userName, String passWord)
        {
            passWord = PassWordEncrypt.Encrypt(userName, passWord);
            return (UserRepository.Instance.UpdateEntityPassword(userName, passWord) > 0);
        }

        /// <summary>
        /// 判断是否存在指定用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>是否存在指定用户</returns>
        internal static Boolean InternalExistsUser(String userName)
        {
            return UserRepository.Instance.ExistsEntity(userName);
        }

        /// <summary>
        /// 根据用户名和电子邮箱得到一个对象实体（不存在时返回null）
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="email">电子邮箱</param>
        /// <returns>若成功则得到一个对象实体，否则返回null</returns>
        internal static UserEntity InternalGetUserByNameAndEmail(String userName, String email)
        {
            return UserRepository.Instance.GetEntityByNameAndEmail(userName, email);
        }

        /// <summary>
        /// 根据用户名获取用户的权限类型
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>权限类型</returns>
        internal static PermissionType InternalAdminGetPermissionByName(String userName)
        {
            return UserRepository.Instance.GetPermissionByName(userName);
        }

        /// <summary>
        /// 获取评测机列表
        /// </summary>
        /// <returns>评测机列表</returns>
        internal static List<UserEntity> InternalAdminGetJudgerList()
        {
            return UserRepository.Instance.GetEntitiesForJudgers();
        }
        #endregion

        #region 其他方法
        /// <summary>
        /// 根据ID得到一个用户实体
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>用户实体</returns>
        public static UserEntity GetUserInfoWithRank(String userName)
        {
            if (!RegexVerify.IsUserName(userName))
            {
                throw new InvalidRequstException(RequestType.User);
            }

            UserEntity entity = UserRepository.Instance.GetEntityWithRank(userName);

            if (entity == null)
            {
                throw new NullResponseException(RequestType.User);
            }

            return entity;
        }

        /// <summary>
        /// 根据ID得到一个用户实体
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>用户实体</returns>
        public static UserEntity GetUserInfo(String userName)
        {
            if (!RegexVerify.IsUserName(userName))
            {
                throw new InvalidRequstException(RequestType.User);
            }

            UserEntity entity = UserRepository.Instance.GetEntityWithBasicInfo(userName);

            if (entity == null)
            {
                throw new NullResponseException(RequestType.User);
            }

            return entity;
        }

        /// <summary>
        /// 获取首页的用户TOP10排名列表
        /// </summary>
        /// <returns>首页的用户TOP10排名列表</returns>
        public static List<UserEntity> GetWeeklyUserTop10Ranklist()
        {
            List<UserEntity> list = UserCache.GetUserTop10Cache();//获取缓存

            if (list == null)
            {
                DateTime weekStart = DateTime.Today.AddDays(1 - (Int32)DateTime.Today.DayOfWeek);

                if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                {
                    weekStart = weekStart.AddDays(-7);//DayOfWeek.Sunday == 0
                }

                list = UserRepository.Instance.GetEntitiesForUserTopRanklist(weekStart, weekStart.AddDays(7), HOME_PAGE_PAGE_SIZE);

                UserCache.SetUserTop10Cache(list);//设置缓存
            }

            return list;
        }

        /// <summary>
        /// 获取用户排名列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>用户排名列表</returns>
        public static PagedList<UserEntity> GetUserRanklist(Int32 pageIndex)
        {
            Int32 pageSize = UserManager.RANKLIST_PAGE_SIZE;
            Int32 recordCount = UserManager.CountUserRanklist();

            List<UserEntity> users = UserRepository.Instance
                .GetEntities(pageIndex, pageSize, recordCount);

            foreach(UserEntity user in users)
            {
                if(user.IsLocked)
                {
                    user.UserName = "Blocked";
                    user.NickName = "**Blocked**";
                }
            }

            return users.ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 获取用户排行的总数(有缓存)
        /// </summary>
        /// <returns>用户排行的总数</returns>
        public static Int32 CountUserRanklist()
        {
            Int32 recordCount = UserCache.GetRanklistUserCountCache();//获取缓存

            if (recordCount < 0)
            {
                recordCount = UserRepository.Instance.CountEntities();
                UserCache.SetRanklistUserCountCache(recordCount);//设置缓存
            }

            return recordCount;
        }
        #endregion

        #region 管理方法
        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">新密码</param>
        /// <returns>是否成功更新</returns>
        public static IMethodResult AdminResetUserPassword(String userName, String passWord)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsUserName(userName))
            {
                return MethodResult.InvalidRequest(RequestType.User);
            }

            if (String.IsNullOrEmpty(passWord))
            {
                return MethodResult.FailedAndLog("New password can not be NULL!");
            }
            else
            {
                passWord = PassWordEncrypt.Encrypt(userName, passWord);
            }

            Boolean success = UserRepository.Instance.UpdateEntityPassword(userName, passWord) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No user's password was reset!");
            }

            return MethodResult.SuccessAndLog("User reset password, name = {0}", userName);
        }

        /// <summary>
        /// 更新指定ID的用户的用户权限
        /// </summary>
        /// <param name="userNames">用户名</param>
        /// <param name="permissions">权限类型</param>
        /// <returns>是否成功更新</returns>
        public static IMethodResult AdminUpdatePermision(String userName, String permissions)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsUserName(userName))
            {
                return MethodResult.InvalidRequest(RequestType.User);
            }

            PermissionType permission = AdminManager.GetPermission(permissions);
            Boolean success = UserRepository.Instance.UpdateEntityPermision(userName, permission) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No user's permission was updated!");
            }

            return MethodResult.SuccessAndLog("Admin update permission, name = {0}, permission = {1}", userName, ((Int32)permission).ToString());
        }

        /// <summary>
        /// 更新指定ID的用户的用户权限
        /// </summary>
        /// <param name="userNames">用户名</param>
        /// <param name="permission">权限类型</param>
        /// <returns>是否成功更新</returns>
        internal static IMethodResult InternalAdminUpdatePermission(String userName, PermissionType permission)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsUserName(userName))
            {
                return MethodResult.InvalidRequest(RequestType.User);
            }

            Boolean success = UserRepository.Instance.UpdateEntityPermision(userName, permission) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("Failed to update user's permission!");
            }

            return MethodResult.Success();
        }

        /// <summary>
        /// 更新指定ID的用户是否锁定
        /// </summary>
        /// <param name="userNames">用户名</param>
        /// <param name="isLocked">是否锁定</param>
        /// <returns>是否成功更新</returns>
        public static IMethodResult AdminUpdateUserIsLocked(String userNames, Boolean isLocked)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (String.IsNullOrEmpty(userNames))
            {
                return MethodResult.InvalidRequest(RequestType.User);
            }

            Boolean success = UserRepository.Instance.UpdateEntityIsLocked(userNames, isLocked) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No user was {0}!", isLocked ? "locked" : "unlocked");
            }

            return MethodResult.SuccessAndLog("Admin {1} user, name = {0}", userNames, isLocked ? "lock" : "unlock");
        }

        /// <summary>
        /// 更新指定ID的用户提交题目总数
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>是否成功更新</returns>
        public static IMethodResult AdminUpdateSubmitCount(String userName)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsUserName(userName))
            {
                return MethodResult.InvalidRequest(RequestType.User);
            }

            Boolean success = UserRepository.Instance.UpdateEntitySubmitCount(userName) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No user's submit count was recalculated!");
            }

            return MethodResult.SuccessAndLog("Admin update user's submit count, name = {0}", userName);
        }

        /// <summary>
        /// 更新指定ID的用户通过题目总数
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>是否成功更新</returns>
        public static IMethodResult AdminUpdateSolvedCount(String userName)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsUserName(userName))
            {
                return MethodResult.InvalidRequest(RequestType.User);
            }

            Boolean success = UserRepository.Instance.UpdateEntitySolvedCount(userName) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No user's solved count was recalculated!");
            }

            return MethodResult.SuccessAndLog("Admin update user's solved count, name = {0}", userName);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>用户实体</returns>
        public static IMethodResult AdminGetUserInfo(String userName)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsUserName(userName))
            {
                return MethodResult.InvalidRequest(RequestType.User);
            }

            UserEntity entity = UserRepository.Instance.GetEntityWithAllInfo(userName);

            if (entity == null)
            {
                return MethodResult.NotExist(RequestType.User);
            }

            return MethodResult.Success(entity);
        }

        /// <summary>
        /// 当前在线用户列表
        /// </summary>
        /// <returns>当前在线用户列表</returns>
        public static IMethodResult AdminGetOnlineUserNames()
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            List<String> list = UserMailCache.GetOnlineUserNames();

            return MethodResult.Success(list);
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="names">用户名包含</param>
        /// <param name="nickname">昵称包含</param>
        /// <param name="email">邮箱包含</param>
        /// <param name="school">学校包含</param>
        /// <param name="lastIP">最后登录IP包含</param>
        /// <param name="islocked">是否锁定</param>
        /// <param name="regStartDate">注册日期开始</param>
        /// <param name="regEndDate">注册日期结束</param>
        /// <param name="loginStartDate">最后登录日期开始</param>
        /// <param name="loginEndDate">最后登录日期结束</param>
        /// <param name="order">排序顺序</param>
        /// <returns>用户列表</returns>
        public static PagedList<UserEntity> AdminGetUserList(Int32 pageIndex, String names, String nickname, String email, String school, String lastIP, String islocked, String regStartDate, String regEndDate, String loginStartDate, String loginEndDate, String order)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            Int32 pageSize = AdminManager.ADMIN_LIST_PAGE_SIZE;
            Int32 recordCount = UserManager.AdminCountUserList(names, nickname, email, school, lastIP, islocked, regStartDate, regEndDate, loginStartDate, loginEndDate);

            DateTime regStart = DateTime.MinValue, regEnd = DateTime.MinValue, loginStart = DateTime.MinValue, loginEnd = DateTime.MinValue;

            return UserRepository.Instance
                .GetEntities(names.SearchOptimized(), nickname, email, school, lastIP,
                    (!String.IsNullOrEmpty(islocked) ? "1".Equals(islocked, StringComparison.OrdinalIgnoreCase) : new Nullable<Boolean>()),
                    (!String.IsNullOrEmpty(regStartDate) && DateTime.TryParse(regStartDate, out regStart) ? regStart : new Nullable<DateTime>()),
                    (!String.IsNullOrEmpty(regEndDate) && DateTime.TryParse(regEndDate, out regEnd) ? regEnd : new Nullable<DateTime>()),
                    (!String.IsNullOrEmpty(loginStartDate) && DateTime.TryParse(loginStartDate, out loginStart) ? loginStart : new Nullable<DateTime>()),
                    (!String.IsNullOrEmpty(loginEndDate) && DateTime.TryParse(loginEndDate, out loginEnd) ? loginEnd : new Nullable<DateTime>()),
                    (String.IsNullOrEmpty(order) ? -1 : Convert.ToInt32(order)),
                    pageIndex, pageSize, recordCount)
                .ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 获取有权限的用户列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>有权限的用户列表</returns>
        public static PagedList<UserEntity> AdminGetPermissionUsers(Int32 pageIndex)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            Int32 pageSize = AdminManager.ADMIN_LIST_PAGE_SIZE;
            Int32 recordCount = UserManager.AdminCountPermissionUsers();

            return UserRepository.Instance
                .GetEntitiesHavePermission(pageIndex, pageSize, recordCount)
                .ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 获取用户总数(无缓存)
        /// </summary>
        /// <param name="names">用户名包含</param>
        /// <param name="nickname">昵称包含</param>
        /// <param name="email">邮箱包含</param>
        /// <param name="school">学校包含</param>
        /// <param name="lastIP">最后登录IP包含</param>
        /// <param name="islocked">是否锁定</param>
        /// <param name="regStartDate">注册日期开始</param>
        /// <param name="regEndDate">注册日期结束</param>
        /// <param name="loginStartDate">最后登录日期开始</param>
        /// <param name="loginEndDate">最后登录日期结束</param>
        /// <param name="order">排序顺序</param>
        /// <returns>用户总数</returns>
        private static Int32 AdminCountUserList(String names, String nickname, String email, String school, String lastIP, String islocked, String regStartDate, String regEndDate, String loginStartDate, String loginEndDate)
        {
            DateTime regStart = DateTime.MinValue, regEnd = DateTime.MinValue, loginStart = DateTime.MinValue, loginEnd = DateTime.MinValue;
            names = names.SearchOptimized();

            return UserRepository.Instance
                .CountEntities(names.SearchOptimized(), nickname, email, school, lastIP,
                    (!String.IsNullOrEmpty(islocked) ? "1".Equals(islocked, StringComparison.OrdinalIgnoreCase) : new Nullable<Boolean>()),
                    (!String.IsNullOrEmpty(regStartDate) && DateTime.TryParse(regStartDate, out regStart) ? regStart : new Nullable<DateTime>()),
                    (!String.IsNullOrEmpty(regEndDate) && DateTime.TryParse(regEndDate, out regEnd) ? regEnd : new Nullable<DateTime>()),
                    (!String.IsNullOrEmpty(loginStartDate) && DateTime.TryParse(loginStartDate, out loginStart) ? loginStart : new Nullable<DateTime>()),
                    (!String.IsNullOrEmpty(loginEndDate) && DateTime.TryParse(loginEndDate, out loginEnd) ? loginEnd : new Nullable<DateTime>()));
        }

        /// <summary>
        /// 获取有权限的用户列表总数
        /// </summary>
        /// <returns>有权限的用户列表总数</returns>
        private static Int32 AdminCountPermissionUsers()
        {
            return UserRepository.Instance.CountEntitiesHavePermission();
        }
        #endregion
    }
}