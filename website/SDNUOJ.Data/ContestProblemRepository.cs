using System;
using System.Collections.Generic;

using DotMaysWind.Data.Command;
using DotMaysWind.Data.Orm;

using SDNUOJ.Entity;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 竞赛题目表格操作类
    /// </summary>
    public class ContestProblemRepository : AbstractDatabaseTable<ContestProblemEntity>
    {
        #region Instance
        private static ContestProblemRepository _instance;

        public static ContestProblemRepository Instance
        {
            get { return _instance; }
        }

        static ContestProblemRepository()
        {
            _instance = new ContestProblemRepository();
        }
        #endregion

        #region Const
        public const Int32 CONTESTPROBLEMIDSTART = 1001;

        internal const String CONTESTID = "CTPB_CONTESTID";
        internal const String PROBLEMID = "CTPB_PROBLEMID";
        internal const String CONTESTPROBLEMID = "CTPB_CONTESTPROBLEMID";
        #endregion

        #region 重载属性和方法
        public override String TableName
        {
            get { return "SDNUOJ_CONTESTPROBLEM"; }
        }

        private ContestProblemRepository() : base(MainDatabase.Instance) { }

        protected override ContestProblemEntity CreateEntity(Object sender, EntityCreatingArgs args)
        {
            ContestProblemEntity entity = new ContestProblemEntity();

            entity.ContestID = this.LoadInt32(args, CONTESTID);
            entity.ProblemID = this.LoadInt32(args, PROBLEMID);
            entity.ContestProblemID = this.LoadInt32(args, CONTESTPROBLEMID);

            return entity;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 增加竞赛题目ID
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pids">题目ID列表</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 InsertEntities(Int32 cid, List<Int32> pids)
        {
            return this.UsingTransaction<Int32>(trans =>
            {
                this.Delete()
                    .Where(c => c.Equal(CONTESTID, cid))
                    .Result(trans);

                Int32 result = this.Sequence()
                    .AddSome(pids, item =>
                    {
                        return this.Insert()
                            .Set(CONTESTID, cid)
                            .Set(PROBLEMID, item.Value)
                            .Set(CONTESTPROBLEMID, CONTESTPROBLEMIDSTART + item.Index);
                    })
                    .Result(trans);

                trans.Commit();

                return result;
            });
        }
        #endregion

        #region Select
        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>实体列表</returns>
        public List<ContestProblemEntity> GetEntities(Int32 cid)
        {
            return this.Select()
                .Querys(CONTESTID, PROBLEMID, CONTESTPROBLEMID)
                .Where(c => c.Equal(CONTESTID, cid))
                .OrderByAsc(CONTESTPROBLEMID)
                .ToEntityList(this);
        }
        #endregion
    }
}