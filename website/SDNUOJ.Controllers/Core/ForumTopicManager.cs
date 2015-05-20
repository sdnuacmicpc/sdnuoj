using System;
using System.Web;

using SDNUOJ.Caching;
using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Controllers.Logging;
using SDNUOJ.Controllers.Status;
using SDNUOJ.Data;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;
using SDNUOJ.Utilities.Text;
using SDNUOJ.Utilities.Text.RegularExpressions;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 论坛主题数据管理类
    /// </summary>
    /// <remarks>
    /// HtmlEncode : 主题标题 / 保存在数据库中
    /// HtmlEncode : 帖子标题 / 保存在数据库中
    /// HtmlEncode : 帖子内容 / 保存在数据库中
    /// </remarks>
    internal static class ForumTopicManager
    {
        #region 常量
        /// <summary>
        /// 论坛每页显示的主题数
        /// </summary>
        private const Int32 FORUM_PAGE_SIZE = 10;
        #endregion

        #region 用户方法
        /// <summary>
        /// 发布新主题
        /// </summary>
        /// <param name="topic">主题实体</param>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="content">主题帖内容</param>
        /// <returns>是否成功发布</returns>
        public static Boolean InsertForumTopic(ForumTopicEntity topic, String cid, String pid, String content)
        {
            if (!UserManager.IsUserLogined)
            {
                throw new UserUnLoginException();
            }

            if (String.IsNullOrEmpty(topic.Title))
            {
                throw new InvalidInputException("Topic title can not be NULL!");
            }

            if (topic.Title.Length > ForumPostRepository.TITLE_MAXLEN)
            {
                throw new InvalidInputException("Topic title is too long!");
            }

            if (!KeywordsFilterManager.IsForumPostContentLegal(topic.Title))
            {
                throw new InvalidInputException("Topic title can not contain illegal keywords!");
            }

            if (String.IsNullOrEmpty(content) || content.Length < ForumPostRepository.POST_MINLEN)
            {
                throw new InvalidInputException("Topic content is too short!");
            }

            if (content.Length > ForumPostRepository.POST_MAXLEN)
            {
                throw new InvalidInputException("Topic content is too long!");
            }

            if (!KeywordsFilterManager.IsForumPostContentLegal(content))
            {
                throw new InvalidInputException("Topic content can not contain illegal keywords!");
            }

            if (!UserSubmitStatus.CheckLastSubmitForumPostTime(UserManager.CurrentUserName))
            {
                throw new InvalidInputException(String.Format("You can not submit post more than twice in {0} seconds!", ConfigurationManager.SubmitInterval.ToString()));
            }

            topic.Type = ForumTopicManager.GetForumTopicType(cid, pid);
            topic.RelativeID = (topic.Type == ForumTopicType.Default ? 0 : ForumTopicManager.GetRelativeID(cid, pid));

            if (topic.Type == ForumTopicType.Problem && !ProblemManager.InternalExistsProblem(topic.RelativeID))
            {
                throw new InvalidRequstException(RequestType.Problem);
            }
            else if (topic.Type == ForumTopicType.Contest && !ContestManager.InternalExistsContest(topic.RelativeID))
            {
                throw new InvalidRequstException(RequestType.Contest);
            }

            topic.UserName = UserManager.CurrentUserName;
            topic.LastDate = DateTime.Now;
            topic.Title = HtmlEncoder.HtmlEncode(topic.Title);
            content = HtmlEncoder.HtmlEncode(content);

            Boolean success = ForumTopicRepository.Instance.InsertEntity(topic, content, RemoteClient.GetRemoteClientIPv4(HttpContext.Current)) > 0;

            if (success)
            {
                ForumTopicCache.IncreaseForumTopicCountCache(topic.Type, topic.RelativeID);//更新缓存

                if (topic.Type == ForumTopicType.Problem)
                {
                    ForumTopicCache.IncreaseForumTopicCountCache(ForumTopicType.Default, 0);//更新缓存
                }
            }

            return success;
        }

        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>对象实体</returns>
        public static ForumTopicEntity GetForumTopic(Int32 id)
        {
            if (id <= 0)
            {
                throw new InvalidRequstException(RequestType.ForumTopic);
            }

            ForumTopicEntity topic = ForumTopicCache.GetForumTopicCache(id);

            if (topic == null)
            {
                topic = ForumTopicRepository.Instance.GetEntity(id);
                ForumTopicCache.SetForumTopicCache(topic);
            }

            if (topic == null)
            {
                throw new NullResponseException(RequestType.ForumTopic);
            }

            if (topic.IsHide && !AdminManager.HasPermission(PermissionType.ForumManage))
            {
                throw new NoPermissionException("You have no privilege to view the topic!");
            }

            return topic;
        }

        /// <summary>
        /// 获取主题列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <returns>主题列表</returns>
        public static PagedList<ForumTopicEntity> GetForumTopicList(Int32 pageIndex, String cid, String pid)
        {
            ForumTopicType type = ForumTopicManager.GetForumTopicType(cid, pid);
            Int32 relativeID = ForumTopicManager.GetRelativeID(cid, pid);

            if (type == ForumTopicType.Problem && !ProblemManager.InternalExistsProblem(relativeID))
            {
                throw new InvalidRequstException(RequestType.Problem);
            }
            else if (type == ForumTopicType.Contest && !ContestManager.InternalExistsContest(relativeID))
            {
                throw new InvalidRequstException(RequestType.Contest);
            }

            Int32 pageSize = ForumTopicManager.FORUM_PAGE_SIZE;
            Int32 recordCount = ForumTopicManager.CountForumTopics(cid, pid);

            return ForumTopicRepository.Instance
                .GetEntities(pageIndex, pageSize, recordCount, type, relativeID, false)
                .ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 获取主题总数(有缓存)
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <returns>主题总数</returns>
        private static Int32 CountForumTopics(String cid, String pid)
        {
            ForumTopicType type = ForumTopicManager.GetForumTopicType(cid, pid);
            Int32 relativeID = ForumTopicManager.GetRelativeID(cid, pid);

            Int32 recordCount = ForumTopicCache.GetForumTopicCountCache(type, relativeID);//获取缓存

            if (recordCount < 0)
            {
                recordCount = ForumTopicRepository.Instance.CountEntities(type, relativeID, false);
                ForumTopicCache.SetForumTopicCountCache(type, relativeID, recordCount);//设置缓存
            }

            return recordCount;
        }
        #endregion

        #region 管理方法
        /// <summary>
        /// 更新主题隐藏状态
        /// </summary>
        /// <param name="ids">主题ID列表</param>
        /// <param name="isHide">是否隐藏</param>
        /// <returns>是否成功更新</returns>
        public static Boolean AdminUpdateForumTopicHideStatus(String ids, Boolean isHide)
        {
            if (!AdminManager.HasPermission(PermissionType.ForumManage))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsNumericIDs(ids))
            {
                throw new InvalidRequstException(RequestType.ForumTopic);
            }

            Boolean success = ForumTopicRepository.Instance.UpdateEntityIsHide(ids, isHide) > 0;

            if (success)
            {
                LogManager.LogOperation(HttpContext.Current, UserManager.CurrentUserName, String.Format("Admin {0} Forum Topic, ID in ({1})", (isHide ? "Hide" : "Unhide"), ids));

                String[] arrids = ids.Split(',');

                for (Int32 i = 0; i < arrids.Length; i++)
                {
                    if (String.IsNullOrEmpty(arrids[i])) continue;

                    Int32 id = Convert.ToInt32(arrids[i]);
                    ForumTopicCache.RemoveForumTopicCache(id);//删除缓存
                }
            }

            return success;
        }

        /// <summary>
        /// 更新主题锁定状态
        /// </summary>
        /// <param name="ids">主题ID列表</param>
        /// <param name="isLock">是否锁定</param>
        /// <returns>是否成功更新</returns>
        public static Boolean AdminUpdateForumTopicLockStatus(String ids, Boolean isLock)
        {
            if (!AdminManager.HasPermission(PermissionType.ForumManage))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsNumericIDs(ids))
            {
                throw new InvalidRequstException(RequestType.ForumTopic);
            }

            Boolean success = ForumTopicRepository.Instance.UpdateEntityIsLocked(ids, isLock) > 0;

            if (success)
            {
                LogManager.LogOperation(HttpContext.Current, UserManager.CurrentUserName, String.Format("Admin {0} Forum Topic, ID in ({1})", (isLock ? "Lock" : "Unlock"), ids));

                String[] arrids = ids.Split(',');

                for (Int32 i = 0; i < arrids.Length; i++)
                {
                    if (String.IsNullOrEmpty(arrids[i])) continue;

                    Int32 id = Convert.ToInt32(arrids[i]);
                    ForumTopicCache.RemoveForumTopicCache(id);//删除缓存
                }
            }

            return success;
        }

        /// <summary>
        /// 获取主题列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="ftids">主题ID列表</param>
        /// <param name="username">发帖人</param>
        /// <param name="title">主题标题</param>
        /// <param name="type">主题类型</param>
        /// <param name="relativeID">相关ID</param>
        /// <param name="isLocked">是否锁定</param>
        /// <param name="isHide">是否隐藏</param>
        /// <param name="startDate">发帖开始时间</param>
        /// <param name="endDate">发帖结束时间</param>
        /// <returns>主题列表</returns>
        public static PagedList<ForumTopicEntity> AdminGetForumTopicList(Int32 pageIndex, String ftids, String username, String title, String type, String relativeID, String isLocked, String isHide, String startDate, String endDate)
        {
            if (!AdminManager.HasPermission(PermissionType.ForumManage))
            {
                throw new NoPermissionException();
            }

            Int32 pageSize = AdminManager.ADMIN_LIST_PAGE_SIZE;
            Int32 recordCount = ForumTopicManager.AdminCountForumTopics(ftids, username, title, type, relativeID, isLocked, isHide, startDate, endDate);

            Byte topictype = 0;
            Int32 rid = -1;
            DateTime dtStart = DateTime.MinValue, dtEnd = DateTime.MinValue;

            ftids = SplitHelper.GetOptimizedString(ftids);

            if (!String.IsNullOrEmpty(ftids) && !RegexVerify.IsNumericIDs(ftids))
            {
                throw new InvalidRequstException(RequestType.ForumTopic);
            }

            if (!String.IsNullOrEmpty(relativeID) && !Int32.TryParse(relativeID, out rid))
            {
                throw new InvalidInputException("Relative ID is INVALID!");
            }

            return ForumTopicRepository.Instance
                .GetEntities(pageIndex, pageSize, recordCount, ftids, username, title,
                    (!String.IsNullOrEmpty(type) && Byte.TryParse(type, out topictype) ? (ForumTopicType)topictype : new Nullable<ForumTopicType>()), rid,
                    (!String.IsNullOrEmpty(isLocked) ? "1".Equals(isLocked, StringComparison.OrdinalIgnoreCase) : new Nullable<Boolean>()),
                    (!String.IsNullOrEmpty(isHide) ? "1".Equals(isHide, StringComparison.OrdinalIgnoreCase) : new Nullable<Boolean>()),
                    (!String.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out dtStart) ? dtStart : new Nullable<DateTime>()),
                    (!String.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out dtEnd) ? dtEnd : new Nullable<DateTime>()))
                    .ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 获取主题总数(无缓存)
        /// </summary>
        /// <param name="ftids">主题ID列表</param>
        /// <param name="username">发帖人</param>
        /// <param name="title">主题标题</param>
        /// <param name="type">主题类型</param>
        /// <param name="relativeID">相关ID</param>
        /// <param name="isLocked">是否锁定</param>
        /// <param name="isHide">是否隐藏</param>
        /// <param name="startDate">发帖开始时间</param>
        /// <param name="endDate">发帖结束时间</param>
        /// <returns>主题总数</returns>
        private static Int32 AdminCountForumTopics(String ftids, String username, String title, String type, String relativeID, String isLocked, String isHide, String startDate, String endDate)
        {
            Byte topictype = 0;
            Int32 rid = -1;
            DateTime dtStart = DateTime.MinValue, dtEnd = DateTime.MinValue;

            ftids = SplitHelper.GetOptimizedString(ftids);

            if (!String.IsNullOrEmpty(ftids) && !RegexVerify.IsNumericIDs(ftids))
            {
                throw new InvalidRequstException(RequestType.ForumTopic);
            }

            if (!String.IsNullOrEmpty(relativeID) && !Int32.TryParse(relativeID, out rid))
            {
                throw new InvalidInputException("Relative ID is INVALID!");
            }

            return ForumTopicRepository.Instance
                .CountEntities(ftids, username, title,
                    (!String.IsNullOrEmpty(type) && Byte.TryParse(type, out topictype) ? (ForumTopicType)topictype : new Nullable<ForumTopicType>()), rid,
                    (!String.IsNullOrEmpty(isLocked) ? "1".Equals(isLocked, StringComparison.OrdinalIgnoreCase) : new Nullable<Boolean>()),
                    (!String.IsNullOrEmpty(isHide) ? "1".Equals(isHide, StringComparison.OrdinalIgnoreCase) : new Nullable<Boolean>()),
                    (!String.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out dtStart) ? dtStart : new Nullable<DateTime>()),
                    (!String.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out dtEnd) ? dtEnd : new Nullable<DateTime>()));
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取主题类型
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <returns>主题类型</returns>
        private static ForumTopicType GetForumTopicType(String cid, String pid)
        {
            if (!String.IsNullOrEmpty(cid)) return ForumTopicType.Contest;
            else if (!String.IsNullOrEmpty(pid)) return ForumTopicType.Problem;
            return ForumTopicType.Default;
        }

        /// <summary>
        /// 获取相关ID
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <returns>相关ID</returns>
        private static Int32 GetRelativeID(String cid, String pid)
        {
            if (!String.IsNullOrEmpty(cid)) return Convert.ToInt32(cid);
            else if (!String.IsNullOrEmpty(pid)) return Convert.ToInt32(pid);
            return 0;
        }
        #endregion
    }
}