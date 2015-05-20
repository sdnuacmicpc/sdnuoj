using System;
using System.Collections.Generic;

using DotMaysWind.Data.Command;
using DotMaysWind.Data.Command.Condition;
using DotMaysWind.Data.Orm;

using SDNUOJ.Data.ForumTopicDataProviderExtensions;
using SDNUOJ.Entity;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 论坛主题表格操作类
    /// </summary>
    public class ForumTopicRepository : AbstractDatabaseTable<ForumTopicEntity>
    {
        #region Instance
        private static ForumTopicRepository _instance;

        public static ForumTopicRepository Instance
        {
            get { return _instance; }
        }

        static ForumTopicRepository()
        {
            _instance = new ForumTopicRepository();
        }
        #endregion

        #region Const
        internal const String TOPICID = "TOPC_TOPICID";
        internal const String USERNAME = "TOPC_USERNAME";
        internal const String TITLE = "TOPC_TITLE";
        internal const String TYPE = "TOPC_TYPE";
        internal const String ISLOCKED = "TOPC_ISLOCKED";
        internal const String ISHIDE = "TOPC_ISHIDE";
        internal const String RELATIVEID = "TOPC_RELATIVEID";
        internal const String LASTDATE = "TOPC_LASTDATE";
        #endregion

        #region 重载属性和方法
        public override String TableName
        {
            get { return "SDNUOJ_FORUMTOPIC"; }
        }

        private ForumTopicRepository() : base(MainDatabase.Instance) { }

        protected override ForumTopicEntity CreateEntity(Object sender, EntityCreatingArgs args)
        {
            ForumTopicEntity entity = new ForumTopicEntity();

            entity.TopicID = this.LoadInt32(args, TOPICID);
            entity.UserName = this.LoadString(args, USERNAME);
            entity.Title = this.LoadString(args, TITLE);
            entity.Type = (ForumTopicType)this.LoadByte(args, TYPE);
            entity.IsLocked = this.LoadBoolean(args, ISLOCKED);
            entity.IsHide = this.LoadBoolean(args, ISHIDE);
            entity.RelativeID = this.LoadInt32(args, RELATIVEID);
            entity.LastDate = this.LoadDateTime(args, LASTDATE);

            return entity;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="topic">主题实体</param>
        /// <param name="content">主题内容</param>
        /// <param name="ip">发布IP</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 InsertEntity(ForumTopicEntity topic, String content, String ip)
        {
            return this.UsingTransaction<Int32>((trans) =>
            {
                Int32 result = this.Insert()
                    .Set(USERNAME, topic.UserName)
                    .Set(TITLE, topic.Title)
                    .Set(TYPE, (Byte)topic.Type)
                    .Set(RELATIVEID, topic.RelativeID)
                    .Set(LASTDATE, topic.LastDate)
                    .Result(trans);

                if (result <= 0)
                {
                    trans.Rollback();
                    return 0;
                }

                ForumPostEntity post = new ForumPostEntity();
                post.TopicID = this.Select().QueryIdentity().Result(trans);
                post.ParentPostID = 0;
                post.Deepth = ForumPostRepository.DEEPTH_MIN;
                post.UserName = topic.UserName;
                post.Title = topic.Title;
                post.Content = content;
                post.PostDate = topic.LastDate;
                post.PostIP = ip;

                result = ForumPostRepository.Instance.InternalGetInsertEntityCommand(post)
                    .Result(trans);//提交主题帖内容

                trans.Commit();

                return result;
            });
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新主题隐藏状态
        /// </summary>
        /// <param name="ids">实体ID列表</param>
        /// <param name="isHide">是否隐藏</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntityIsHide(String ids, Boolean isHide)
        {
            return this.Update()
                .Set(ISHIDE, isHide)
                .Where(c => c.InInt32(TOPICID, ids, ','))
                .Result();
        }

        /// <summary>
        /// 更新主题锁定状态
        /// </summary>
        /// <param name="ids">实体ID列表</param>
        /// <param name="isLocked">是否锁定</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntityIsLocked(String ids, Boolean isLocked)
        {
            return this.Update()
                .Set(ISLOCKED, isLocked)
                .Where(c => c.InInt32(TOPICID, ids, ','))
                .Result();
        }
        #endregion

        #region Select
        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>对象实体</returns>
        public ForumTopicEntity GetEntity(Int32 id)
        {
            return this.Select()
                .Querys(TOPICID, USERNAME, TITLE, TYPE, ISLOCKED, ISHIDE, RELATIVEID, LASTDATE)
                .Where(c => c.Equal(TOPICID, id))
                .ToEntity(this);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <param name="type">主题类型</param>
        /// <param name="relativeID">相关ID</param>
        /// <param name="includeHide">是否包含隐藏主题</param>
        /// <returns>实体列表</returns>
        public List<ForumTopicEntity> GetEntities(Int32 pageIndex, Int32 pageSize, Int32 recordCount, ForumTopicType type, Int32 relativeID, Boolean includeHide)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(TOPICID, USERNAME, TITLE, TYPE, ISLOCKED, ISHIDE, RELATIVEID, LASTDATE)
                .Where(type, relativeID, includeHide)
                .OrderByDesc(LASTDATE, TOPICID)
                .ToEntityList(this);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <param name="ftids">主题ID列表</param>
        /// <param name="userName">发帖人</param>
        /// <param name="title">主题标题</param>
        /// <param name="type">主题类型</param>
        /// <param name="relativeID">相关ID</param>
        /// <param name="isLocked">是否锁定</param>
        /// <param name="isHide">是否隐藏</param>
        /// <param name="startDate">发帖开始时间</param>
        /// <param name="endDate">发帖结束时间</param>
        /// <returns>实体列表</returns>
        public List<ForumTopicEntity> GetEntities(Int32 pageIndex, Int32 pageSize, Int32 recordCount, String ftids, String userName, String title, ForumTopicType? type, Int32 relativeID, Boolean? isLocked, Boolean? isHide, DateTime? startDate, DateTime? endDate)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(TOPICID, USERNAME, TITLE, TYPE, ISLOCKED, ISHIDE, RELATIVEID, LASTDATE)
                .Where(ftids, userName, title, type, relativeID, isLocked, isHide, startDate, endDate)
                .OrderByDesc(LASTDATE)
                .ToEntityList(this);
        }
        #endregion

        #region Count/Exists
        /// <summary>
        /// 获取实体总数
        /// </summary>
        /// <param name="type">主题类型</param>
        /// <param name="relativeID">相关ID</param>
        /// <param name="includeHide">是否包含隐藏主题</param>
        /// <returns>实体总数</returns>
        public Int32 CountEntities(ForumTopicType type, Int32 relativeID, Boolean includeHide)
        {
            return this.Select()
                .Where(type, relativeID, includeHide)
                .Count();
        }

        /// <summary>
        /// 获取实体总数
        /// </summary>
        /// <param name="ftids">主题ID列表</param>
        /// <param name="userName">发帖人</param>
        /// <param name="title">主题标题</param>
        /// <param name="type">主题类型</param>
        /// <param name="relativeID">相关ID</param>
        /// <param name="isLocked">是否锁定</param>
        /// <param name="isHide">是否隐藏</param>
        /// <param name="startDate">发帖开始时间</param>
        /// <param name="endDate">发帖结束时间</param>
        /// <returns>实体总数</returns>
        public Int32 CountEntities(String ftids, String userName, String title, ForumTopicType? type, Int32 relativeID, Boolean? isLocked, Boolean? isHide, DateTime? startDate, DateTime? endDate)
        {
            return this.Select()
                .Where(ftids, userName, title, type, relativeID, isLocked, isHide, startDate, endDate)
                .Count();
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 获取更新最后帖子时间的SQL语句
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <param name="lastdate">最后帖子时间</param>
        /// <returns>操作影响的记录数</returns>
        internal UpdateCommand InternalGetUpdateEntityLastDateCommand(Int32 id, DateTime lastdate)
        {
            return this.Update()
                .Set(LASTDATE, lastdate)
                .Where(c => c.Equal(TOPICID, id));
        }
        #endregion
    }
}

namespace SDNUOJ.Data.ForumTopicDataProviderExtensions
{
    #region ForumTopicDataProviderExtension
    internal static class ForumTopicDataProviderExtension
    {
        /// <summary>
        /// 设置指定的查询语句并返回当前语句
        /// </summary>
        /// <param name="command">选择语句</param>
        /// <param name="type">主题类型</param>
        /// <param name="relativeID">相关ID</param>
        /// <param name="includeHide">是否包含隐藏主题</param>
        /// <returns>当前语句</returns>
        internal static SelectCommand Where(this SelectCommand command, ForumTopicType type, Int32 relativeID, Boolean includeHide)
        {
            SqlConditionBuilder c = command.ConditionBuilder;
            AbstractSqlCondition condition = command.WhereCondition as AbstractSqlCondition;
            AbstractSqlCondition temp = null;

            //不包含隐藏
            if (!includeHide)
            {
                temp = c.Equal(ForumTopicRepository.ISHIDE, false);
                condition = (condition == null ? temp : condition & temp);
            }

            if (type == ForumTopicType.Default)//普通讨论板中包括题目专属讨论板的帖子
            {
                temp = c.LessThan(ForumTopicRepository.TYPE, (Byte)ForumTopicType.Contest);//普通0，题目1，竞赛2
                condition = (condition == null ? temp : condition & temp);
            }
            else//竞赛或题目专属讨论板只有专属帖子
            {
                temp = c.Equal(ForumTopicRepository.TYPE, (Byte)type) & c.Equal(ForumTopicRepository.RELATIVEID, relativeID);
                condition = (condition == null ? temp : condition & temp);
            }

            return command.Where(condition);
        }

        /// <summary>
        /// 设置指定的查询语句并返回当前语句
        /// </summary>
        /// <param name="command">选择语句</param>
        /// <param name="ftids">主题ID列表</param>
        /// <param name="userName">发帖人</param>
        /// <param name="title">主题标题</param>
        /// <param name="type">主题类型</param>
        /// <param name="relativeID">相关ID</param>
        /// <param name="isLocked">是否锁定</param>
        /// <param name="isHide">是否隐藏</param>
        /// <param name="startDate">发帖开始时间</param>
        /// <param name="endDate">发帖结束时间</param>
        /// <returns>当前语句</returns>
        internal static SelectCommand Where(this SelectCommand command, String ftids, String userName, String title, ForumTopicType? type, Int32 relativeID, Boolean? isLocked, Boolean? isHide, DateTime? startDate, DateTime? endDate)
        {
            SqlConditionBuilder c = command.ConditionBuilder;
            AbstractSqlCondition condition = command.WhereCondition as AbstractSqlCondition;
            AbstractSqlCondition temp = null;

            //主题ID
            if (!String.IsNullOrEmpty(ftids))
            {
                temp = c.InInt32(ForumTopicRepository.TOPICID, ftids, ',');
                condition = (condition == null ? temp : condition & temp);
            }

            //用户名
            if (!String.IsNullOrEmpty(userName))
            {
                temp = c.LikeAll(ForumTopicRepository.USERNAME, userName);
                condition = (condition == null ? temp : condition & temp);
            }

            //主题标题
            if (!String.IsNullOrEmpty(title))
            {
                temp = c.LikeAll(ForumTopicRepository.TITLE, title);
                condition = (condition == null ? temp : condition & temp);
            }

            //主题类型
            if (type.HasValue)
            {
                temp = c.Equal(ForumTopicRepository.TYPE, (Byte)type.Value);
                condition = (condition == null ? temp : condition & temp);
            }

            //相关ID
            if (relativeID >= 0)
            {
                temp = c.Equal(ForumTopicRepository.RELATIVEID, relativeID);
                condition = (condition == null ? temp : condition & temp);
            }

            //是否锁定
            if (isLocked.HasValue)
            {
                temp = c.Equal(ForumTopicRepository.ISLOCKED, isLocked.Value);
                condition = (condition == null ? temp : condition & temp);
            }

            //是否隐藏
            if (isHide.HasValue)
            {
                temp = c.Equal(ForumTopicRepository.ISHIDE, isHide.Value);
                condition = (condition == null ? temp : condition & temp);
            }

            //发布日期范围
            if (startDate.HasValue || endDate.HasValue)
            {
                temp = c.BetweenNullable<DateTime>(ForumTopicRepository.LASTDATE, startDate, endDate);
                condition = (condition == null ? temp : condition & temp);
            }

            return command.Where(condition);
        }
    }
    #endregion
}