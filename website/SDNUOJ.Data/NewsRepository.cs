using System;
using System.Collections.Generic;

using DotMaysWind.Data.Orm;

using SDNUOJ.Entity;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 公告表格操作类
    /// </summary>
    public class NewsRepository : AbstractDatabaseTable<NewsEntity>
    {
        #region Instance
        private static NewsRepository _instance;

        public static NewsRepository Instance
        {
            get { return _instance; }
        }

        static NewsRepository()
        {
            _instance = new NewsRepository();
        }
        #endregion

        #region Const
        public const Int32 DEFAULTID = 1;

        internal const String ANNOUNCEID = "ANNO_ANNOUNCEID";
        internal const String TITLE = "ANNO_TITLE";
        internal const String DESCRIPTION = "ANNO_DESCRIPTION";
        internal const String PUBLISHDATE = "ANNO_PUBLISHDATE";
        #endregion

        #region 重载属性和方法
        public override String TableName
        {
            get { return "SDNUOJ_ANNOUNCEMENT"; }
        }

        private NewsRepository() : base(MainDatabase.Instance) { }

        protected override NewsEntity CreateEntity(Object sender, EntityCreatingArgs args)
        {
            NewsEntity entity = new NewsEntity();

            entity.AnnounceID = this.LoadInt32(args, ANNOUNCEID);
            entity.Title = this.LoadString(args, TITLE);
            entity.Description = this.LoadString(args, DESCRIPTION);
            entity.PublishDate = this.LoadDateTime(args, PUBLISHDATE);

            return entity;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 InsertEntity(NewsEntity entity)
        {
            return this.Insert()
                .Set(TITLE, entity.Title)
                .Set(DESCRIPTION, entity.Description)
                .Set(PUBLISHDATE, entity.PublishDate)
                .Result();
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntity(NewsEntity entity)
        {
            return this.Update()
                .Set(TITLE, entity.Title)
                .Set(DESCRIPTION, entity.Description)
                .Set(PUBLISHDATE, entity.PublishDate)
                .Where(c => c.Equal(ANNOUNCEID, entity.AnnounceID))
                .Result();
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="ids">逗号分隔的实体ID</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 DeleteEntities(String ids)
        {
            return this.Delete()
                .Where(c => 
                {
                    List<Int32> listids = new List<Int32>();
                    String[] arrids = ids.Split(',');
                    String defID = NewsRepository.DEFAULTID.ToString();

                    for (Int32 i = 0; i < arrids.Length; i++)
                    {
                        Int32 id = 0;

                        if (!String.Equals(arrids[i], defID) && Int32.TryParse(arrids[i], out id))
                        {
                            listids.Add(id);
                        }
                    }

                    return c.In<Int32>(ANNOUNCEID, listids);
                })
                .Result();
        }
        #endregion

        #region Select
        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>对象实体</returns>
        public NewsEntity GetEntity(Int32 id)
        {
            return this.Select()
                .Querys(ANNOUNCEID, TITLE, DESCRIPTION, PUBLISHDATE)
                .Where(c => c.Equal(ANNOUNCEID, id))
                .ToEntity(this);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <param name="includeDefault">是否包含默认公告</param>
        /// <returns>实体列表</returns>
        public List<NewsEntity> GetEntities(Int32 pageIndex, Int32 pageSize, Int32 recordCount, Boolean includeDefault)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(ANNOUNCEID, TITLE, PUBLISHDATE)
                .Where(c => includeDefault ? null : c.GreaterThan(ANNOUNCEID, DEFAULTID))
                .OrderByDesc(ANNOUNCEID)
                .ToEntityList(this);
        }

        /// <summary>
        /// 获取最近公告
        /// </summary>
        /// <param name="count">获取公告数</param>
        /// <returns>最近公告</returns>
        public List<NewsEntity> GetLastEntities(Int32 count)
        {
            return this.Select()
                .Top(count)
                .Querys(ANNOUNCEID, TITLE, DESCRIPTION, PUBLISHDATE)
                .Where(c => c.GreaterThan(ANNOUNCEID, DEFAULTID))
                .OrderByDesc(ANNOUNCEID)
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