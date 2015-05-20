using System;
using System.Collections.Generic;

using DotMaysWind.Data.Orm;

using SDNUOJ.Entity;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 资源表格操作类
    /// </summary>
    public class ResourceRepository: AbstractDatabaseTable<ResourceEntity>
    {
        #region Instance
        private static ResourceRepository _instance;

        public static ResourceRepository Instance
        {
            get { return _instance; }
        }

        static ResourceRepository()
        {
            _instance = new ResourceRepository();
        }
        #endregion

        #region Const
        internal const String RESOURCEID = "RESC_RESOURCEID";
        internal const String TITLE = "RESC_TITLE";
        internal const String URL = "RESC_URL";
        internal const String TYPE = "RESC_TYPE";
        #endregion

        #region 重载属性和方法
        public override String TableName
        {
            get { return "SDNUOJ_RESOURCE"; }
        }

        private ResourceRepository() : base(MainDatabase.Instance) { }

        protected override ResourceEntity CreateEntity(Object sender, EntityCreatingArgs args)
        {
            ResourceEntity entity = new ResourceEntity();

            entity.ResourceID = this.LoadInt32(args, RESOURCEID);
            entity.Title = this.LoadString(args, TITLE);
            entity.Url = this.LoadString(args, URL);
            entity.Type = this.LoadString(args, TYPE);

            return entity;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 InsertEntity(ResourceEntity entity)
        {
            return this.Insert()
                .Set(TITLE, entity.Title)
                .Set(URL, entity.Url)
                .Set(TYPE, entity.Type)
                .Result();
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntity(ResourceEntity entity)
        {
            return this.Update()
                .Set(TITLE, entity.Title)
                .Set(URL, entity.Url)
                .Set(TYPE, entity.Type)
                .Where(c => c.Equal(RESOURCEID, entity.ResourceID))
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
                .Where(c => c.InInt32(RESOURCEID, ids, ','))
                .Result();
        }
        #endregion

        #region Select
        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>对象实体</returns>
        public ResourceEntity GetEntity(Int32 id)
        {
            return this.Select()
                .Querys(RESOURCEID, TITLE, URL, TYPE)
                .Where(c => c.Equal(RESOURCEID, id))
                .ToEntity(this);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <returns>实体列表</returns>
        public List<ResourceEntity> GetAllEntities()
        {
            return this.Select()
                .Querys(RESOURCEID, TITLE, URL, TYPE)
                .OrderByAsc(TYPE, TITLE)
                .ToEntityList(this);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <returns>实体列表</returns>
        public List<ResourceEntity> GetEntities(Int32 pageIndex, Int32 pageSize, Int32 recordCount)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(RESOURCEID, TITLE, URL, TYPE)
                .OrderByDesc(RESOURCEID)
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