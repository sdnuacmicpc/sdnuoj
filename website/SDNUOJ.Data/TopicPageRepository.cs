using System;
using System.Collections.Generic;

using DotMaysWind.Data;
using DotMaysWind.Data.Orm;

using SDNUOJ.Entity;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 专题页面表格操作类
    /// </summary>
    public class TopicPageRepository: AbstractDatabaseTable<TopicPageEntity>
    {
        #region Instance
        private static TopicPageRepository _instance;

        public static TopicPageRepository Instance
        {
            get { return _instance; }
        }

        static TopicPageRepository()
        {
            _instance = new TopicPageRepository();
        }
        #endregion

        #region Const
        internal const String PAGENAME = "TPPG_PAGENAME";
        internal const String TITLE = "TPPG_TITLE";
        internal const String DESCRIPTION = "TPPG_DESCRIPTION";
        internal const String CONTENT = "TPPG_CONTENT";
        internal const String LASTDATE = "TPPG_LASTDATE";
        internal const String CREATEUSER = "TPPG_CREATEUSER";
        internal const String ISHIDE = "TPPG_ISHIDE";
        #endregion

        #region 重载属性和方法
        public override String TableName
        {
            get { return "SDNUOJ_TOPICPAGE"; }
        }

        private TopicPageRepository() : base(MainDatabase.Instance) { }

        protected override TopicPageEntity CreateEntity(Object sender, EntityCreatingArgs args)
        {
            TopicPageEntity entity = new TopicPageEntity();

            entity.PageName = this.LoadString(args, PAGENAME);
            entity.Title = this.LoadString(args, TITLE);
            entity.Description = this.LoadString(args, DESCRIPTION);
            entity.Content = this.LoadString(args, CONTENT);
            entity.LastDate = this.LoadDateTime(args, LASTDATE);
            entity.CreateUser = this.LoadString(args, CREATEUSER);
            entity.IsHide = this.LoadBoolean(args, ISHIDE);

            return entity;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 InsertEntity(TopicPageEntity entity)
        {
            return this.Insert()
                .Set(PAGENAME, entity.PageName)
                .Set(TITLE, entity.Title)
                .Set(DESCRIPTION, entity.Description)
                .Set(CONTENT, entity.Content)
                .Set(LASTDATE, entity.LastDate)
                .Set(CREATEUSER, entity.CreateUser)
                .Set(ISHIDE, entity.IsHide)
                .Result();
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <param name="oldname">旧的实体名称</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntity(TopicPageEntity entity, String oldName)
        {
            return this.Update()
                .Set(PAGENAME, entity.PageName)
                .Set(TITLE, entity.Title)
                .Set(DESCRIPTION, entity.Description)
                .Set(CONTENT, entity.Content)
                .Set(LASTDATE, entity.LastDate)
                .Where(c => c.Equal(PAGENAME, oldName))
                .Result();
        }

        /// <summary>
        /// 更新一条数据(只更新隐藏状态)
        /// </summary>
        /// <param name="name">实体名称</param>
        /// <param name="isHide">隐藏状态</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntityIsHide(String name, Boolean isHide)
        {
            return this.Update()
                .Set(ISHIDE, isHide)
                .Where(c => c.Equal(PAGENAME, name))
                .Result();
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除指定名称的数据
        /// </summary>
        /// <param name="ids">逗号分隔的实体名称</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 DeleteEntities(String names)
        {
            return this.Delete()
                .Where(c => c.InString(PAGENAME, names, ','))
                .Result();
        }
        #endregion

        #region Select
        /// <summary>
        /// 根据名称得到一个对象实体
        /// </summary>
        /// <param name="name">实体名称</param>
        /// <returns>对象实体</returns>
        public TopicPageEntity GetEntity(String name)
        {
            return this.Select()
                .Querys(PAGENAME, TITLE, DESCRIPTION, CONTENT, LASTDATE, CREATEUSER, ISHIDE)
                .Where(c => c.Equal(PAGENAME, name))
                .ToEntity(this);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <returns>实体列表</returns>
        public List<TopicPageEntity> GetEntities(Int32 pageIndex, Int32 pageSize, Int32 recordCount)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(PAGENAME, TITLE, LASTDATE, CREATEUSER, ISHIDE)
                .OrderByDesc(LASTDATE)
                .ToEntityList(this);
        }
        #endregion

        #region Count/Exists
        /// <summary>
        /// 获取实体总数
        /// </summary>
        /// <returns>实体总数</returns>
        public Int32 CountEntities()
        {
            return this.Select().Count();
        }
        #endregion
    }
}