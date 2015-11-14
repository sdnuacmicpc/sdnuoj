using System;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Status;
using SDNUOJ.Entity;

namespace SDNUOJ.Controllers.Core.Judge
{
    internal static class JudgeStatusManager
    {
        #region 属性
        /// <summary>
        /// 获取当前评测机用户名
        /// </summary>
        public static String JudgeUserName
        {
            get { return UserManager.CurrentUserName; }
        }
        #endregion

        #region 评测机登录
        /// <summary>
        /// 尝试评测机登录
        /// </summary>
        /// <param name="serverID">评测机ID</param>
        /// <param name="secretKey">评测机密钥</param>
        /// <param name="userip">用户IP</param>
        /// <param name="error">错误信息</param>
        /// <returns>是否登录成功</returns>
        public static Boolean TryJudgeServerLogin(String serverID, String secretKey, String userip, out String error)
        {
            if (!ConfigurationManager.EnableJudgerInterface)
            {
                error = "Judger interface is disabled!";
                return false;
            }

            UserEntity user = null;
            error = UserManager.TryGetUserByUsernameAndPassword(serverID, secretKey, out user);

            if (!String.IsNullOrEmpty(error))
            {
                return false;
            }

            if (user.Permission != PermissionType.HttpJudge)
            {
                error = "You do not have httpjudge privilege!";
                return false;
            }

            try
            {
                UserManager.UpdateLoginInfomation(serverID, userip);
                UserCurrentStatus.SetCurrentUserStatus(user);
                JudgeOnlineStatus.SetJudgeStatus(serverID);
            }
            catch { }

            return true;
        }
        #endregion

        #region 评测机状态
        /// <summary>
        /// 获取用户登录状态
        /// </summary>
        /// <returns>若用户已登录则返回空，否则返回出错状态</returns>
        public static String GetJudgeServerLoginStatus()
        {
            if (!ConfigurationManager.EnableJudgerInterface)
            {
                return "disabled";
            }

            if (!UserManager.IsUserLogined)
            {
                return "unlogin";
            }

            if (UserManager.CurrentUser.Permission != PermissionType.HttpJudge)
            {
                return "no privilege";
            }

            try
            {
                JudgeOnlineStatus.SetJudgeStatus(UserManager.CurrentUserName);
            }
            catch { }

            return String.Empty;
        }
        #endregion
    }
}