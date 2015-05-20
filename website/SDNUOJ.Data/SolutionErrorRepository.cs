using System;

using DotMaysWind.Data.Command;
using DotMaysWind.Data.Orm;

using SDNUOJ.Entity;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 提交错误表格操作类
    /// </summary>
    public class SolutionErrorRepository : AbstractDatabaseTable<SolutionErrorEntity>
    {
        #region Instance
        private static SolutionErrorRepository _instance;

        public static SolutionErrorRepository Instance
        {
            get { return _instance; }
        }

        static SolutionErrorRepository()
        {
            _instance = new SolutionErrorRepository();
        }
        #endregion

        #region Const
        internal const String SOLUTIONID = "SOCE_SOLUTIONID";
        internal const String ERRORINFO = "SOCE_ERRORINFO";
        #endregion

        #region 重载属性和方法
        public override String TableName
        {
            get { return "SDNUOJ_SOLUTIONERROR"; }
        }

        private SolutionErrorRepository() : base(MainDatabase.Instance) { }

        protected override SolutionErrorEntity CreateEntity(Object sender, EntityCreatingArgs args)
        {
            SolutionErrorEntity entity = new SolutionErrorEntity();

            entity.SolutionID = this.LoadInt32(args, SOLUTIONID);
            entity.ErrorInfo = this.LoadString(args, ERRORINFO);

            return entity;
        }
        #endregion

        #region Select
        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>对象实体</returns>
        public SolutionErrorEntity GetEntity(Int32 id)
        {
            return this.Select()
                .Querys(SOLUTIONID, ERRORINFO)
                .Where(c => c.Equal(SOLUTIONID, id))
                .ToEntity(this);
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 增加一条数据的SQL语句
        /// </summary>
        /// <param name="sid">提交ID</param>
        /// <param name="error">编译错误</param>
        /// <returns>SQL语句</returns>
        internal InsertCommand InternalGetInsertEntityCommand(Int32 sid, String error)
        {
            return this.Insert()
                .Set(SOLUTIONID, sid)
                .Set(ERRORINFO, error);
        }

        ///// <summary>
        ///// 更新一条数据的SQL语句
        ///// </summary>
        ///// <param name="sid">提交ID</param>
        ///// <param name="error">编译错误</param>
        ///// <returns>SQL语句</returns>
        //internal UpdateCommand InternalGetUpdateEntityCommand(Int32 sid, String error)
        //{
        //    return this.Update()
        //        .Set(ERRORINFO, error)
        //        .Where(c => c.Equal(SOLUTIONID, sid));
        //}

        /// <summary>
        /// 更新一条数据的SQL语句
        /// </summary>
        /// <param name="sid">提交ID</param>
        /// <returns>SQL语句</returns>
        internal DeleteCommand InternalGetDeleteEntityCommand(Int32 sid)
        {
            return this.Delete()
                .Where(c => c.Equal(SOLUTIONID, sid));
        }
        #endregion
    }
}