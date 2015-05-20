using System;
using System.Collections.Generic;

using DotMaysWind.Data.Command;
using DotMaysWind.Data.Orm;

using SDNUOJ.Entity;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 题目类型表格操作类
    /// </summary>
    public class ProblemCategoryItemRepository : AbstractDatabaseTable<ProblemCategoryItemEntity>
    {
        #region Instance
        private static ProblemCategoryItemRepository _instance;

        public static ProblemCategoryItemRepository Instance
        {
            get { return _instance; }
        }

        static ProblemCategoryItemRepository()
        {
            _instance = new ProblemCategoryItemRepository();
        }
        #endregion

        #region Const
        internal const String PTID = "TYPE_PTID";
        internal const String PROBLEMID = "TYPE_PROBLEMID";
        internal const String TYPEID = "TYPE_TYPEID";
        #endregion

        #region 重载属性和方法
        public override String TableName
        {
            get { return "SDNUOJ_PROBLEMTYPE"; }
        }

        private ProblemCategoryItemRepository() : base(MainDatabase.Instance) { }

        protected override ProblemCategoryItemEntity CreateEntity(Object sender, EntityCreatingArgs args)
        {
            ProblemCategoryItemEntity entity = new ProblemCategoryItemEntity();

            entity.PTID = this.LoadInt32(args, PTID);
            entity.ProblemID = this.LoadInt32(args, PROBLEMID);
            entity.TypeID = this.LoadInt32(args, TYPEID);

            return entity;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="problemID">题目ID</param>
        /// <param name="typesids">逗号分隔的分类ID</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 InsertEntity(Int32 problemID, String typesids)
        {
            return this.Sequence()
                .AddSome(typesids.Split(','), item => 
                {
                    if (!String.IsNullOrEmpty(item.Value))
                    {
                        return this.Insert()
                            .Set(PROBLEMID, problemID)
                            .Set(TYPEID, Convert.ToInt32(item.Value));
                    }
                    else
                    {
                        return null;
                    }
                })
                .Result();
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="problemID">题目ID</param>
        /// <param name="typesids">逗号分隔的分类ID</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 DeleteEntities(Int32 problemID, String typesids)
        {
            return this.Delete()
                .Where(c => c.Equal(PROBLEMID, problemID) & c.InInt32(TYPEID, typesids, ','))
                .Result();
        }
        #endregion

        #region Select
        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="problemID">题目ID</param>
        /// <returns>实体列表</returns>
        public List<ProblemCategoryItemEntity> GetEntities(Int32 problemID)
        {
            return this.Select()
                .Querys(PROBLEMID, TYPEID)
                .Where(c => c.Equal(PROBLEMID, problemID))
                .OrderByAsc(TYPEID)
                .ToEntityList(this);
        }
        #endregion

        #region Count
        /// <summary>
        /// 获取实体数量
        /// </summary>
        /// <param name="typeID">分类ID</param>
        /// <returns>实体数量</returns>
        public Int32 CountEntities(Int32 typeID)
        {
            return this.Select()
                .Where(c => c.Equal(TYPEID, typeID))
                .Count();
        }
        #endregion
    }
}