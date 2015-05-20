using System;
using System.Collections.Generic;

using DotMaysWind.Data.Orm;

using SDNUOJ.Entity;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 题目类型种类表格操作类
    /// </summary>
    public class ProblemCategoryRepository : AbstractDatabaseTable<ProblemCategoryEntity>
    {
        #region Instance
        private static ProblemCategoryRepository _instance;

        public static ProblemCategoryRepository Instance
        {
            get { return _instance; }
        }

        static ProblemCategoryRepository()
        {
            _instance = new ProblemCategoryRepository();
        }
        #endregion

        #region Const
        internal const String TYPEID = "TYPS_TYPEID";
        internal const String TITLE = "TYPS_TITLE";
        internal const String ORDER = "TYPS_ORDER";
        #endregion

        #region 重载属性和方法
        public override String TableName
        {
            get { return "SDNUOJ_PROBLEMTYPES"; }
        }

        private ProblemCategoryRepository() : base(MainDatabase.Instance) { }

        protected override ProblemCategoryEntity CreateEntity(Object sender, EntityCreatingArgs args)
        {
            ProblemCategoryEntity entity = new ProblemCategoryEntity();

            entity.TypeID = this.LoadInt32(args, TYPEID);
            entity.Title = this.LoadString(args, TITLE);
            entity.Order = this.LoadInt32(args, ORDER);

            return entity;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 InsertEntity(ProblemCategoryEntity entity)
        {
            return this.Insert()
                .Set(TITLE, entity.Title)
                .Set(ORDER, entity.Order)
                .Result();
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntity(ProblemCategoryEntity entity)
        {
            return this.Update()
                .Set(TITLE, entity.Title)
                .Set(ORDER, entity.Order)
                .Where(c => c.Equal(TYPEID, entity.TypeID))
                .Result();
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 DeleteEntity(Int32 typeID)
        {
            return this.Delete()
                .Where(c => c.Equal(TYPEID, typeID))
                .Result();
        }
        #endregion

        #region Select
        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>对象实体</returns>
        public ProblemCategoryEntity GetEntity(Int32 id)
        {
            return this.Select()
                .Querys(TYPEID, TITLE, ORDER)
                .Where(c => c.Equal(TYPEID, id))
                .ToEntity(this);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <returns>实体列表</returns>
        public List<ProblemCategoryEntity> GetEntities(Int32 pageIndex, Int32 pageSize, Int32 recordCount)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(TYPEID, TITLE, ORDER)
                .OrderByDesc(ORDER)
                .OrderByDesc(TYPEID)
                .ToEntityList(this);
        }

        /// <summary>
        /// 获取所有题目类型
        /// </summary>
        /// <returns>所有题目类型</returns>
        public List<ProblemCategoryEntity> GetAllEntities()
        {
            return this.Select()
                .Querys(TYPEID, TITLE, ORDER)
                .OrderByAsc(ORDER)
                .OrderByAsc(TYPEID)
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