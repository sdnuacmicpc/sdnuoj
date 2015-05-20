using System;
using System.Collections.Generic;

using DotMaysWind.Data.Command;
using DotMaysWind.Data.Command.Condition;
using DotMaysWind.Data.Orm;

using SDNUOJ.Data.ForumPostDataProviderExtensions;
using SDNUOJ.Entity;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 论坛帖子表格操作类
    /// </summary>
    public class ForumPostRepository : AbstractDatabaseTable<ForumPostEntity>
    {
        #region Instance
        private static ForumPostRepository _instance;

        public static ForumPostRepository Instance
        {
            get { return _instance; }
        }

        static ForumPostRepository()
        {
            _instance = new ForumPostRepository();
        }
        #endregion

        #region Const
        internal const String POSTID = "POST_POSTID";
        internal const String TOPICID = "POST_TOPICID";
        internal const String PARENTPOSTID = "POST_PARENTPOSTID";
        internal const String DEEPTH = "POST_DEEPTH";
        internal const String USERNAME = "POST_USERNAME";
        internal const String TITLE = "POST_TITLE";
        internal const String CONTENT = "POST_CONTENT";
        internal const String CONTENTLENGTH = "POST_CONTENTLENGTH";
        internal const String ISHIDE = "POST_ISHIDE";
        internal const String POSTDATE = "POST_POSTDATE";
        internal const String POSTIP = "POST_POSTIP";

        public const Int32 DEEPTH_MIN = 0;
        public const Int32 DEEPTH_MAX = 10;

        public const Int32 TITLE_MAXLEN = 100;
        public const Int32 POST_MINLEN = 2;
        public const Int32 POST_MAXLEN = 32767;
        #endregion

        #region 重载属性和方法
        public override String TableName
        {
            get { return "SDNUOJ_FORUMPOST"; }
        }

        private ForumPostRepository() : base(MainDatabase.Instance) { }

        protected override ForumPostEntity CreateEntity(Object sender, EntityCreatingArgs args)
        {
            ForumPostEntity entity = new ForumPostEntity();

            entity.PostID = this.LoadInt32(args, POSTID);
            entity.TopicID = this.LoadInt32(args, TOPICID);
            entity.ParentPostID = this.LoadInt32(args, PARENTPOSTID);
            entity.Deepth = this.LoadInt32(args, DEEPTH);
            entity.UserName = this.LoadString(args, USERNAME);
            entity.Title = this.LoadString(args, TITLE);
            entity.Content = this.LoadString(args, CONTENT);
            entity.ContentLength = this.LoadInt32(args, CONTENTLENGTH);
            entity.IsHide = this.LoadBoolean(args, ISHIDE);
            entity.PostDate = this.LoadDateTime(args, POSTDATE);
            entity.PostIP = this.LoadString(args, POSTIP);

            return entity;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 InsertEntity(ForumPostEntity entity)
        {
            return this.Sequence()
                .Add(this.InternalGetInsertEntityCommand(entity))
                .Add(ForumTopicRepository.Instance.InternalGetUpdateEntityLastDateCommand(entity.TopicID, entity.PostDate))//更新主题最后时间
                .Result();
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新帖子隐藏状态
        /// </summary>
        /// <param name="ids">实体ID列表</param>
        /// <param name="isHide">是否隐藏</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntityIsHide(String ids, Boolean isHide)
        {
            return this.Update()
                .Set(ISHIDE, isHide)
                .Where(c => c.InInt32(POSTID, ids, ','))
                .Result();
        }
        #endregion

        #region Select
        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>对象实体</returns>
        public ForumPostEntity GetEntity(Int32 id)
        {
            return this.Select()
                .Querys(POSTID, TOPICID, PARENTPOSTID, DEEPTH, USERNAME, TITLE, CONTENT, CONTENTLENGTH, ISHIDE, POSTDATE, POSTIP)
                .Where(c => c.Equal(POSTID, id))
                .ToEntity(this);
        }

        /// <summary>
        /// 根据主题ID得到一个对象实体
        /// </summary>
        /// <param name="tid">主题ID</param>
        /// <returns>对象实体</returns>
        public ForumPostEntity GetEntityByTopicID(Int32 tid)
        {
            return this.Select()
                .Querys(POSTID, TOPICID, PARENTPOSTID, DEEPTH, USERNAME, TITLE, CONTENT, CONTENTLENGTH, ISHIDE, POSTDATE, POSTIP)
                .Where(c => c.Equal(TOPICID, tid) & c.Equal(DEEPTH, 0))
                .ToEntity(this);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <param name="fpids">帖子ID列表</param>
        /// <param name="ftids">主题ID列表</param>
        /// <param name="userName">发帖用户名</param>
        /// <param name="title">帖子标题</param>
        /// <param name="content">帖子内容</param>
        /// <param name="isHide">是否隐藏</param>
        /// <param name="startDate">发帖开始时间</param>
        /// <param name="endDate">发帖结束时间</param>
        /// <param name="postip">发帖IP</param>
        /// <returns>实体列表</returns>
        public List<ForumPostEntity> GetEntities(Int32 pageIndex, Int32 pageSize, Int32 recordCount, String fpids, String ftids, String userName, String title, String content, Boolean? isHide, DateTime? startDate, DateTime? endDate, String postip)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(POSTID, TOPICID, PARENTPOSTID, DEEPTH, TITLE, USERNAME, ISHIDE, POSTDATE, POSTIP)
                .Where(fpids, ftids, userName, title, content, isHide, startDate, endDate, postip)
                .OrderByDesc(POSTID)
                .ToEntityList(this);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="topics">主题列表</param>
        /// <param name="includeHide">是否包含隐藏主题</param>
        /// <returns>实体列表</returns>
        public List<ForumPostEntity> GetEntities(IList<ForumTopicEntity> topics, Boolean includeHide)
        {
            if (topics == null || topics.Count == 0)
            {
                return null;
            }

            return this.Select()
                .Querys(POSTID, TOPICID, PARENTPOSTID, DEEPTH, USERNAME, TITLE, CONTENT, CONTENTLENGTH, ISHIDE, POSTDATE, POSTIP)
                .Where(topics, includeHide)
                .OrderByDesc(TOPICID)
                .OrderByAsc(DEEPTH)
                .OrderByAsc(POSTID)
                .ToEntityList(this);
        }
        #endregion

        #region Count/Exists
        /// <summary>
        /// 获取实体总数
        /// </summary>
        /// <param name="fpids">帖子ID列表</param>
        /// <param name="ftids">主题ID列表</param>
        /// <param name="userName">发帖用户名</param>
        /// <param name="title">帖子标题</param>
        /// <param name="content">帖子内容</param>
        /// <param name="isHide">是否隐藏</param>
        /// <param name="startDate">发帖开始时间</param>
        /// <param name="endDate">发帖结束时间</param>
        /// <param name="postip">发帖IP</param>
        /// <returns>实体总数</returns>
        public Int32 CountEntities(String fpids, String ftids, String userName, String title, String content, Boolean? isHide, DateTime? startDate, DateTime? endDate, String postip)
        {
            return this.Select()
                .Where(fpids, ftids, userName, title, content, isHide, startDate, endDate, postip)
                .Count();
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 获取增加数据的SQL语句
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>SQL语句</returns>
        internal InsertCommand InternalGetInsertEntityCommand(ForumPostEntity entity)
        {
            return this.Insert()
                .Set(TOPICID, entity.TopicID)
                .Set(PARENTPOSTID, entity.ParentPostID)
                .Set(DEEPTH, entity.Deepth)
                .Set(USERNAME, entity.UserName)
                .Set(TITLE, entity.Title)
                .Set(CONTENT, entity.Content)
                .Set(CONTENTLENGTH, entity.Content.Length)
                .Set(POSTDATE, entity.PostDate)
                .Set(POSTIP, entity.PostIP);
        }
        #endregion
    }
}

namespace SDNUOJ.Data.ForumPostDataProviderExtensions
{
    #region ForumPostDataProviderExtension
    internal static class ForumPostDataProviderExtension
    {
        /// <summary>
        /// 设置指定的查询语句并返回当前语句
        /// </summary>
        /// <param name="command">选择语句</param>
        /// <param name="fpids">帖子ID列表</param>
        /// <param name="ftids">主题ID列表</param>
        /// <param name="userName">发帖用户名</param>
        /// <param name="title">帖子标题</param>
        /// <param name="content">帖子内容</param>
        /// <param name="isHide">是否隐藏</param>
        /// <param name="startDate">发帖开始时间</param>
        /// <param name="endDate">发帖结束时间</param>
        /// <param name="postip">发帖IP</param>
        /// <returns>当前语句</returns>
        internal static SelectCommand Where(this SelectCommand command, String fpids, String ftids, String userName, String title, String content, Boolean? isHide, DateTime? startDate, DateTime? endDate, String postip)
        {
            SqlConditionBuilder c = command.ConditionBuilder;
            AbstractSqlCondition condition = command.WhereCondition as AbstractSqlCondition;
            AbstractSqlCondition temp = null;

            //帖子ID
            if (!String.IsNullOrEmpty(fpids))
            {
                temp = c.InInt32(ForumPostRepository.POSTID, fpids, ',');
                condition = (condition == null ? temp : condition & temp);
            }

            //主题ID
            if (!String.IsNullOrEmpty(ftids))
            {
                temp = c.InInt32(ForumPostRepository.TOPICID, ftids, ',');
                condition = (condition == null ? temp : condition & temp);
            }

            //用户名
            if (!String.IsNullOrEmpty(userName))
            {
                temp = c.LikeAll(ForumPostRepository.USERNAME, userName);
                condition = (condition == null ? temp : condition & temp);
            }

            //帖子标题
            if (!String.IsNullOrEmpty(title))
            {
                temp = c.LikeAll(ForumPostRepository.TITLE, title);
                condition = (condition == null ? temp : condition & temp);
            }

            //帖子内容
            if (!String.IsNullOrEmpty(content))
            {
                temp = c.LikeAll(ForumPostRepository.CONTENT, content);
                condition = (condition == null ? temp : condition & temp);
            }

            //是否隐藏
            if (isHide.HasValue)
            {
                temp = c.Equal(ForumPostRepository.ISHIDE, isHide.Value);
                condition = (condition == null ? temp : condition & temp);
            }

            //发布日期范围
            if (startDate.HasValue || endDate.HasValue)
            {
                temp = c.BetweenNullable<DateTime>(ForumPostRepository.POSTDATE, startDate, endDate);
                condition = (condition == null ? temp : condition & temp);
            }

            //发布IP
            if (!String.IsNullOrEmpty(postip))
            {
                temp = c.LikeAll(ForumPostRepository.POSTIP, postip);
                condition = (condition == null ? temp : condition & temp);
            }

            return command.Where(condition);
        }

        /// <summary>
        /// 设置指定的查询语句并返回当前语句
        /// </summary>
        /// <param name="command">选择语句</param>
        /// <param name="topics">主题列表</param>
        /// <param name="includeHide">是否包含隐藏主题</param>
        /// <returns>当前语句</returns>
        internal static SelectCommand Where(this SelectCommand command, IList<ForumTopicEntity> topics, Boolean includeHide)
        {
            SqlConditionBuilder c = command.ConditionBuilder;
            AbstractSqlCondition condition = command.WhereCondition as AbstractSqlCondition;
            AbstractSqlCondition temp = null;

            if (!includeHide)
            {
                temp = c.Equal(ForumPostRepository.ISHIDE, false);
                condition = (condition == null ? temp : condition & temp);
            }

            if (topics != null)
            {
                temp = c.In<Int32>(ForumPostRepository.TOPICID, () =>
                {
                    List<Int32> ids = new List<Int32>();

                    for (Int32 i = 0; i < topics.Count; i++)
                    {
                        ids.Add(topics[i].TopicID);
                    }

                    return ids;
                });

                condition = (condition == null ? temp : condition & temp);
            }

            return command.Where(condition);
        }
    }
    #endregion
}