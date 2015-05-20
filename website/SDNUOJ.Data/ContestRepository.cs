using System;
using System.Collections.Generic;

using DotMaysWind.Data.Orm;

using SDNUOJ.Entity;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 竞赛表格操作类
    /// </summary>
    public class ContestRepository : AbstractDatabaseTable<ContestEntity>
    {
        #region Instance
        private static ContestRepository _instance;

        public static ContestRepository Instance
        {
            get { return _instance; }
        }

        static ContestRepository()
        {
            _instance = new ContestRepository();
        }
        #endregion

        #region Const
        public const Int32 NONECONTEST = 0;

        internal const String CONTESTID = "CNTS_CONTESTID";
        internal const String TITLE = "CNTS_TITLE";
        internal const String DESCRIPTION = "CNTS_DESCRIPTION";
        internal const String TYPE = "CNTS_TYPE";
        internal const String STARTTIME = "CNTS_STARTTIME";
        internal const String ENDTIME = "CNTS_ENDTIME";
        internal const String REGSTARTTIME = "CNTS_REGSTARTTIME";
        internal const String REGENDTIME = "CNTS_REGENDTIME";
        internal const String LASTDATE = "CNTS_LASTDATE";
        internal const String SUPPORTLANGUAGE = "CNTS_SUPPORTLANGUAGE";
        internal const String ISHIDE = "CNTS_ISHIDE";
        #endregion

        #region 重载属性和方法
        public override String TableName
        {
            get { return "SDNUOJ_CONTEST"; }
        }

        private ContestRepository() : base(MainDatabase.Instance) { }

        protected override ContestEntity CreateEntity(Object sender, EntityCreatingArgs args)
        {
            ContestEntity entity = new ContestEntity();

            entity.ContestID = this.LoadInt32(args, CONTESTID);
            entity.Title = this.LoadString(args, TITLE);
            entity.Description = this.LoadString(args, DESCRIPTION);
            entity.ContestType = (ContestType)this.LoadByte(args, TYPE);
            entity.StartTime = this.LoadDateTime(args, STARTTIME);
            entity.EndTime = this.LoadDateTime(args, ENDTIME);
            entity.RegisterStartTime = this.LoadNullableDateTime(args, REGSTARTTIME);
            entity.RegisterEndTime = this.LoadNullableDateTime(args, REGENDTIME);
            entity.LastDate = this.LoadDateTime(args, LASTDATE);
            entity.SupportLanguage = this.LoadString(args, SUPPORTLANGUAGE);
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
        public Int32 InsertEntity(ContestEntity entity)
        {
            return this.Insert()
                .Set(TITLE, entity.Title)
                .Set(DESCRIPTION, entity.Description)
                .Set(TYPE, (Byte)entity.ContestType)
                .Set(STARTTIME, entity.StartTime)
                .Set(ENDTIME, entity.EndTime)
                .Set(REGSTARTTIME, entity.RegisterStartTime)
                .Set(REGENDTIME, entity.RegisterEndTime)
                .Set(LASTDATE, entity.LastDate)
                .Set(SUPPORTLANGUAGE, entity.SupportLanguage)
                .Set(ISHIDE, entity.IsHide)
                .Result();
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntity(ContestEntity entity)
        {
            return this.Update()
                .Set(TITLE, entity.Title)
                .Set(DESCRIPTION, entity.Description)
                .Set(TYPE, (Byte)entity.ContestType)
                .Set(STARTTIME, entity.StartTime)
                .Set(ENDTIME, entity.EndTime)
                .Set(REGSTARTTIME, entity.RegisterStartTime)
                .Set(REGENDTIME, entity.RegisterEndTime)
                .Set(LASTDATE, entity.LastDate)
                .Set(SUPPORTLANGUAGE, entity.SupportLanguage)
                .Where(c => c.Equal(CONTESTID, entity.ContestID))
                .Result();
        }

        /// <summary>
        /// 更新竞赛隐藏状态
        /// </summary>
        /// <param name="ids">实体ID列表</param>
        /// <param name="isHide">隐藏状态</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntityIsHide(String ids, Boolean isHide)
        {
            return this.Update()
                .Set(ISHIDE, isHide)
                .Where(c => c.InInt32(CONTESTID, ids, ','))
                .Result();
        }
        #endregion

        #region Select
        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>对象实体</returns>
        public ContestEntity GetEntity(Int32 id)
        {
            return this.Select()
                .Querys(CONTESTID, TITLE, DESCRIPTION, TYPE, STARTTIME, ENDTIME, REGSTARTTIME, REGENDTIME, LASTDATE, SUPPORTLANGUAGE, ISHIDE)
                .Where(c => c.Equal(CONTESTID, id))
                .ToEntity(this);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <returns>实体列表</returns>
        public List<ContestEntity> GetEntities(Int32 pageIndex, Int32 pageSize, Int32 recordCount)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(CONTESTID, TITLE, TYPE, STARTTIME, ENDTIME, ISHIDE)
                .OrderByDesc(CONTESTID)
                .ToEntityList(this);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <param name="passed">是否已过去的竞赛</param>
        /// <returns>实体列表</returns>
        public List<ContestEntity> GetEntities(Int32 pageIndex, Int32 pageSize, Int32 recordCount, Boolean passed)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(CONTESTID, TITLE, TYPE, STARTTIME, ENDTIME, REGSTARTTIME, REGENDTIME, ISHIDE)
                .Where(c => c.Equal(ISHIDE, false) & (passed ? c.LessThan(ENDTIME, DateTime.Now) : c.GreaterThanOrEqual(ENDTIME, DateTime.Now)))
                .OrderByDesc(CONTESTID)
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

        /// <summary>
        /// 获取实体总数
        /// </summary>
        /// <param name="passed">是否已过去的竞赛</param>
        /// <returns>实体总数</returns>
        public Int32 CountEntities(Boolean passed)
        {
            return this.Select()
                .Where(c => c.Equal(ISHIDE, false) & (passed ? c.LessThan(ENDTIME, DateTime.Now) : c.GreaterThanOrEqual(ENDTIME, DateTime.Now)))
                .Count();
        }
        #endregion
    }
}