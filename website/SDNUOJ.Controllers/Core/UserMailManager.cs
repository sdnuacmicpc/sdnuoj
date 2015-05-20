using System;

using SDNUOJ.Caching;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Controllers.Status;
using SDNUOJ.Configuration;
using SDNUOJ.Data;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;
using SDNUOJ.Utilities.Text;
using SDNUOJ.Utilities.Text.RegularExpressions;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 公告数据管理类
    /// </summary>
    /// <remarks>
    /// HtmlEncode : 邮件标题 / 保存在数据库中
    /// HtmlEncode : 邮件内容 / 保存在数据库中
    /// </remarks>
    internal static class UserMailManager
    {
        #region 常量
        /// <summary>
        /// 用户邮件每页显示的数量
        /// </summary>
        private const Int32 MAILBOX_PAGE_SIZE = 10;
        #endregion

        #region 发送邮件方法
        /// <summary>
        /// 尝试发送邮件
        /// </summary>
        /// <param name="model">邮件实体</param>
        /// <param name="error">出错信息</param>
        /// <returns>是否发送成功</returns>
        public static Boolean TrySendUserMail(UserMailEntity entity, out String error)
        {
            if (!UserManager.IsUserLogined)
            {
                error = "Please login first!";
                return false;
            }

            if (String.IsNullOrEmpty(entity.Title))
            {
                error = "Title can not be NULL!";
                return false;
            }

            if (entity.Title.Length > UserMailRepository.TITLE_MAXLEN)
            {
                error = "Title is too long!";
                return false;
            }

            if (String.IsNullOrEmpty(entity.Content) || entity.Content.Length < UserMailRepository.CONTENT_MINLEN)
            {
                error = "Content is too short!";
                return false;
            }

            if (entity.Content.Length > UserMailRepository.CONTENT_MAXLEN)
            {
                error = "Content is too long!";
                return false;
            }

            if (String.IsNullOrEmpty(entity.ToUserName))
            {
                error = "Username can not be NULL!";
                return false;
            }

            if (!RegexVerify.IsUserName(entity.ToUserName))
            {
                error = "Username is INVALID!";
                return false;
            }

            if (String.Equals(ConfigurationManager.SystemAccount, entity.ToUserName, StringComparison.OrdinalIgnoreCase))
            {
                error = "You can not send mail to system account!";
                return false;
            }

            if (String.Equals(UserManager.CurrentUserName, entity.ToUserName, StringComparison.OrdinalIgnoreCase))
            {
                error = "You can not send mail to yourself!";
                return false;
            }

            if (!UserSubmitStatus.CheckLastSubmitUserMailTime(UserManager.CurrentUserName))
            {
                throw new InvalidInputException(String.Format("You can not submit user mail more than twice in {0} seconds!", ConfigurationManager.SubmitInterval.ToString()));
            }

            if (!UserManager.InternalExistsUser(entity.ToUserName))
            {
                error = String.Format("The username \"{0}\" doesn't exist!", entity.ToUserName);
                return false;
            }

            entity.Title = HtmlEncoder.HtmlEncode(entity.Title);
            entity.Content = HtmlEncoder.HtmlEncode(entity.Content);
            entity.FromUserName = UserManager.CurrentUserName;

            if (!UserMailManager.InternalSendUserMail(entity))
            {
                error = "Failed to send your mail";
                return false;
            }

            error = String.Empty;
            return true;
        }
        #endregion

        #region 用户方法
        /// <summary>
        /// 删除指定ID的邮件
        /// </summary>
        /// <param name="ids">逗号分隔的邮件ID</param>
        /// <returns>是否成功删除</returns>
        public static Boolean DeleteUserMails(String ids)
        {
            if (!UserManager.IsUserLogined)
            {
                throw new UserUnLoginException();
            }

            if (!RegexVerify.IsNumericIDs(ids))
            {
                throw new InvalidRequstException(RequestType.UserMail);
            }

            String userName = UserManager.CurrentUserName;
            Boolean success = UserMailRepository.Instance.DeleteEntities(userName, ids) > 0;

            if (success)
            {
                UserMailCache.RemoveUserUnReadMailCountCache(userName);//删除缓存
            }

            return success;
        }

        /// <summary>
        /// 根据ID得到一个邮件实体
        /// </summary>
        /// <param name="id">邮件ID</param>
        /// <returns>邮件实体</returns>
        public static UserMailEntity GetUserMail(Int32 id)
        {
            if (!UserManager.IsUserLogined)
            {
                throw new UserUnLoginException();
            }

            if (id <= 0)
            {
                throw new InvalidRequstException(RequestType.UserMail);
            }

            String userName = UserManager.CurrentUserName;
            UserMailEntity entity = UserMailRepository.Instance.GetEntity(userName, id);

            if (entity == null)
            {
                throw new NullResponseException(RequestType.UserMail);
            }

            if (!entity.IsRead)
            {
                UserMailManager.UpdateUserMailRead(entity);
            }

            return entity;
        }

        /// <summary>
        /// 获取用户邮件
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>用户邮件列表</returns>
        public static PagedList<UserMailEntity> GetUserMails(Int32 pageIndex)
        {
            if (!UserManager.IsUserLogined)
            {
                throw new UserUnLoginException();
            }

            Int32 pageSize = UserMailManager.MAILBOX_PAGE_SIZE;
            Int32 recordCount = UserMailManager.CountUserMails();

            String userName = UserManager.CurrentUserName;

            UserMailCache.RemoveUserUnReadMailCountCache(userName);//删除缓存

            return UserMailRepository.Instance
                .GetEntities(userName, pageIndex, pageSize, recordCount)
                .ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 发送用户邮件
        /// </summary>
        /// <param name="model">邮件实体</param>
        /// <returns>是否发送成功</returns>
        internal static Boolean InternalSendUserMail(UserMailEntity entity)
        {
            entity.SendDate = DateTime.Now;
            entity.IsRead = false;
            entity.IsDeleted = false;

            Int32 result = UserMailRepository.Instance.InsertEntity(entity);

            if (result > 0)
            {
                UserMailCache.RemoveUserUnReadMailCountCache(entity.ToUserName);//删除缓存
            }

            return (result > 0);
        }

        /// <summary>
        /// 获取未读邮件总数(有缓存)
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>未读邮件总数</returns>
        internal static Int32 InternalCountUserUnReadMails(String userName)
        {
            if (ConfigurationManager.AllowUserMail)
            {
                Int32 recordCount = UserMailCache.GetUserUnReadMailCountCache(userName);//获取缓存

                if (recordCount < 0)
                {
                    recordCount = UserMailRepository.Instance.CountUnReadEntities(userName);
                    UserMailCache.SetUserUnReadMailCountCache(userName, recordCount);//设置缓存
                }

                return recordCount;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取邮件总数
        /// </summary>
        /// <returns>邮件总数</returns>
        private static Int32 CountUserMails()
        {
            if (!UserManager.IsUserLogined)
            {
                throw new UserUnLoginException();
            }

            String userName = UserManager.CurrentUserName;

            return UserMailRepository.Instance.CountEntities(userName);
        }

        /// <summary>
        /// 更新一条邮件的状态
        /// </summary>
        /// <param name="model">对象实体</param>
        /// <returns>是否成功更新</returns>
        private static Boolean UpdateUserMailRead(UserMailEntity entity)
        {
            Boolean success = false;

            try
            {
                entity.IsRead = true;
                success = UserMailRepository.Instance.UpdateEntityIsRead(entity) > 0;
            }
            finally
            {
                if (success)
                {
                    UserMailCache.RemoveUserUnReadMailCountCache(entity.ToUserName);//删除缓存
                }
            }

            return success;
        }
        #endregion
    }
}