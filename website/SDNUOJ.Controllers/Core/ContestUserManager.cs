using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

using SDNUOJ.Caching;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Data;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 竞赛用户数据管理类
    /// </summary>
    internal static class ContestUserManager
    {
        #region 用户方法
        /// <summary>
        /// 注册当前用户
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="realName">真实姓名</param>
        /// <returns>是否注册成功</returns>
        public static Boolean RegisterCurrentUser(Int32 cid, String realName)
        {
            if (!UserManager.IsUserLogined)
            {
                throw new UserUnLoginException();
            }

            if (String.IsNullOrEmpty(realName))
            {
                throw new InvalidInputException("Real Name can not be NULL!");
            }

            if (!String.IsNullOrEmpty(realName) && realName.Length > ContestUserRepository.REALNAME_MAXLEN)
            {
                throw new InvalidInputException("Real Name is too long!");
            }

            ContestEntity contest = ContestManager.GetRegisterContest(cid);
            String userName = UserManager.CurrentUserName;

            if (ContestUserRepository.Instance.ExistsEntity(contest.ContestID, userName))
            {
                throw new InvalidInputException("You have already registered this contest!");
            }

            ContestUserEntity entity = new ContestUserEntity()
            {
                ContestID = contest.ContestID,
                UserName = userName,
                RealName = realName,
                IsEnable = false,
                RegisterTime = DateTime.Now
            };
            
            return (ContestUserRepository.Instance.InsertEntity(entity) > 0);
        }

        /// <summary>
        /// 获取竞赛用户列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>竞赛用户列表</returns>
        public static Dictionary<String, ContestUserEntity> GetContestUserList(Int32 cid)
        {
            Dictionary<String, ContestUserEntity> dict = ContestUserCache.GetContestUserListCache(cid);//获取缓存

            if (dict == null)
            {
                dict = ContestUserRepository.Instance.GetEntities(cid);
                ContestUserCache.SetContestUserListCache(cid, dict);//设置缓存
            }

            return dict;
        }

        /// <summary>
        /// 判断指定竞赛是否包含指定用户
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="userName">用户名</param>
        /// <returns>是否包含指定用户</returns>
        public static Boolean ContestContainsUser(Int32 cid, String userName)
        {
            Dictionary<String, ContestUserEntity> dict = GetContestUserList(cid);
            
            if (dict == null)
            {
                return false;
            }

            ContestUserEntity entity = null;

            if (!dict.TryGetValue(userName, out entity))
            {
                return false;
            }

            return entity.IsEnable;
        }
        #endregion

        #region 管理方法
        /// <summary>
        /// 添加指定ID的竞赛用户
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="usernames">竞赛用户</param>
        /// <returns>是否成功添加</returns>
        public static IMethodResult AdminInsertContestUsers(Int32 cid, String usernames)
        {
            if (!AdminManager.HasPermission(PermissionType.ContestManage))
            {
                throw new NoPermissionException();
            }

            if (String.IsNullOrEmpty(usernames))
            {
                return MethodResult.InvalidRequst(RequestType.User);
            }

            Dictionary<String, ContestUserEntity> dict = new Dictionary<String, ContestUserEntity>();
            StringBuilder names = new StringBuilder();
            String[] namelist = usernames.Split('\n');

            for (Int32 i = 0; i < namelist.Length; i++)
            {
                String[] name = namelist[i].Replace('\t', ' ').Split(' ');

                if (name.Length < 1 || String.IsNullOrEmpty(name[0].Trim()))
                {
                    continue;
                }

                ContestUserEntity entity = new ContestUserEntity()
                {
                    ContestID = cid,
                    UserName = name[0].Replace("\r", "").Trim(),
                    RealName = (name.Length >= 2 ? name[1].Replace("\r", "").Trim() : ""),
                    IsEnable = true,
                    RegisterTime = DateTime.Now
                };

                if (!dict.ContainsKey(entity.UserName))
                {
                    names.Append(names.Length > 0 ? "," : "").Append(entity.UserName);
                }

                dict[entity.UserName] = entity;
            }

            try
            {
                Int32 count = ContestUserRepository.Instance.InsertEntities(cid, names.ToString(), dict);

                if (count <= 0)
                {
                    return MethodResult.FailedAndLog("No contest user was added!");
                }

                ContestUserCache.RemoveContestUserListCache(cid);

                return MethodResult.SuccessAndLog<Int32>(count, "Admin add contest user, cid = {0}, username = {1}", cid.ToString(), usernames);
            }
            catch (DbException)
            {
                return MethodResult.FailedAndLog("Failed to add these users, please check whether the names are all correct.");
            }
        }

        /// <summary>
        /// 更新指定ID的竞赛用户是否启用
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="usernames">逗号分隔的用户名</param>
        /// <param name="isEnabled">是否启用</param>
        /// <returns>是否成功更新</returns>
        public static IMethodResult AdminUpdateContestUsers(Int32 cid, String usernames, Boolean isEnabled)
        {
            if (!AdminManager.HasPermission(PermissionType.ContestManage))
            {
                throw new NoPermissionException();
            }

            if (String.IsNullOrEmpty(usernames))
            {
                return MethodResult.FailedAndLog("You must select at least one item!");
            }

            Boolean success = ContestUserRepository.Instance.UpdateEntityIsEnabled(cid, usernames, isEnabled) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No contest user was {0}!", isEnabled ? "enabled" : "disabled");
            }

            ContestUserCache.RemoveContestUserListCache(cid);

            return MethodResult.SuccessAndLog("Admin {2} contest user, cid = {0}, username = {1}", cid.ToString(), usernames, isEnabled ? "enable" : "disable");
        }

        /// <summary>
        /// 删除指定ID的竞赛用户
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="usernames">逗号分隔的用户名</param>
        /// <returns>是否成功删除</returns>
        public static IMethodResult AdminDeleteContestUsers(Int32 cid, String usernames)
        {
            if (!AdminManager.HasPermission(PermissionType.ContestManage))
            {
                throw new NoPermissionException();
            }

            if (String.IsNullOrEmpty(usernames))
            {
                return MethodResult.FailedAndLog("You must select at least one item!");
            }

            Boolean success = ContestUserRepository.Instance.DeleteEntities(cid, usernames) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No contest user was deleted!");
            }

            ContestUserCache.RemoveContestUserListCache(cid);

            return MethodResult.SuccessAndLog("Admin delete contest user, cid = {0}, username = {1}", cid.ToString(), usernames);
        }

        /// <summary>
        /// 导出竞赛用户列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="mask">导出内容掩码</param>
        /// <param name="withTitle">包括标题行</param>
        /// <returns>竞赛用户列表</returns>
        /// <remarks>
        /// 第1位-竞赛ID
        /// 第2位-用户名
        /// 第3位-真实姓名
        /// 第4位-注册时间
        /// 第5位-是否启用
        /// </remarks>
        public static String AdminExportContestUserList(Int32 cid, String mask, Boolean withTitle)
        {
            if (!AdminManager.HasPermission(PermissionType.ContestManage))
            {
                throw new NoPermissionException();
            }

            Int32 maskcode = 0;

            if (!String.IsNullOrEmpty(mask))
            {
                String[] codes = mask.Replace(" ", "").Split(',');

                foreach (String code in codes)
                {
                    Int32 num = 0;

                    if (Int32.TryParse(code, out num))
                    {
                        maskcode += num;
                    }
                }
            }

            if (maskcode <= 0)
            {
                throw new InvalidInputException("You must select at least one item to export!");
            }

            Dictionary<String, ContestUserEntity> dict = ContestUserRepository.Instance.GetEntities(cid);
            StringBuilder sb = new StringBuilder();

            if (dict != null)
            {
                StringBuilder line = new StringBuilder();

                if (withTitle)
                {
                    if ((maskcode & 0x1) == 0x1)
                    {
                        line.Append(line.Length > 0 ? "\t" : "").Append("Contest ID");
                    }

                    if ((maskcode & 0x2) == 0x2)
                    {
                        line.Append(line.Length > 0 ? "\t" : "").Append("User Name");
                    }

                    if ((maskcode & 0x4) == 0x4)
                    {
                        line.Append(line.Length > 0 ? "\t" : "").Append("Real Name");
                    }

                    if ((maskcode & 0x8) == 0x8)
                    {
                        line.Append(line.Length > 0 ? "\t" : "").Append("Register Time");
                    }

                    if ((maskcode & 0x10) == 0x10)
                    {
                        line.Append(line.Length > 0 ? "\t" : "").Append("Enabled");
                    }

                    sb.AppendLine(line.ToString());
                }

                foreach (ContestUserEntity user in dict.Values)
                {
                    line = new StringBuilder();

                    if ((maskcode & 0x1) == 0x1)
                    {
                        line.Append(line.Length > 0 ? "\t" : "").Append(user.ContestID.ToString());
                    }

                    if ((maskcode & 0x2) == 0x2)
                    {
                        line.Append(line.Length > 0 ? "\t" : "").Append(user.UserName);
                    }

                    if ((maskcode & 0x4) == 0x4)
                    {
                        line.Append(line.Length > 0 ? "\t" : "").Append(user.RealName);
                    }

                    if ((maskcode & 0x8) == 0x8)
                    {
                        line.Append(line.Length > 0 ? "\t" : "").Append(user.RegisterTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    }

                    if ((maskcode & 0x10) == 0x10)
                    {
                        line.Append(line.Length > 0 ? "\t" : "").Append(user.IsEnable ? "Y" : "N");
                    }

                    sb.AppendLine(line.ToString());
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取竞赛用户列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>竞赛用户列表</returns>
        public static PagedList<ContestUserEntity> AdminGetContestUserList(Int32 cid, Int32 pageIndex)
        {
            if (!AdminManager.HasPermission(PermissionType.ContestManage))
            {
                throw new NoPermissionException();
            }

            Int32 pageSize = AdminManager.ADMIN_LIST_PAGE_SIZE;
            Int32 recordCount = ContestUserManager.AdminCountContestUsers(cid);

            return ContestUserRepository.Instance
                .GetEntities(cid, pageIndex, pageSize, recordCount)
                .ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 获取竞赛用户总数
        /// </summary>
        /// <<param name="cid">竞赛用户ID</param>
        /// <returns>竞赛用户总数</returns>
        private static Int32 AdminCountContestUsers(Int32 cid)
        {
            return ContestUserRepository.Instance.CountEntities(cid);
        }
        #endregion
    }
}