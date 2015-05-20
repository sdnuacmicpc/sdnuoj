using System;
using System.Collections.Generic;

using DotMaysWind.Data;
using DotMaysWind.Data.Command;
using DotMaysWind.Data.Orm;

using SDNUOJ.Configuration;
using SDNUOJ.Data.Functions;
using SDNUOJ.Entity;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 题目表格操作类
    /// </summary>
    public class ProblemRepository : AbstractDatabaseTable<ProblemEntity>
    {
        #region Instance
        private static ProblemRepository _instance;

        public static ProblemRepository Instance
        {
            get { return _instance; }
        }

        static ProblemRepository()
        {
            _instance = new ProblemRepository();
        }
        #endregion

        #region Const
        internal const String PROBLEMID = "PROB_PROBLEMID";
        internal const String TITLE = "PROB_TITLE";
        internal const String DESCRIPTION = "PROB_DESCRIPTION";
        internal const String INPUT = "PROB_INPUT";
        internal const String OUTPUT = "PROB_OUTPUT";
        internal const String SAMPLEINPUT = "PROB_SAMPLEINPUT";
        internal const String SAMPLEOUT = "PROB_SAMPLEOUT";
        internal const String HINT = "PROB_HINT";
        internal const String SOURCE = "PROB_SOURCE";
        internal const String TIMELIMIT = "PROB_TIMELIMIT";
        internal const String MEMORYLIMIT = "PROB_MEMORYLIMIT";
        internal const String SUBMITCOUNT = "PROB_SUBMITCOUNT";
        internal const String SOLVEDCOUNT = "PROB_SOLVEDCOUNT";
        internal const String LASTDATE = "PROB_LASTDATE";
        internal const String ISHIDE = "PROB_ISHIDE";
        #endregion

        #region 重载属性和方法
        public override String TableName
        {
            get { return "SDNUOJ_PROBLEM"; }
        }

        private ProblemRepository() : base(MainDatabase.Instance) { }

        protected override ProblemEntity CreateEntity(Object sender, EntityCreatingArgs args)
        {
            ProblemEntity entity = new ProblemEntity();

            entity.ProblemID = this.LoadInt32(args, PROBLEMID);
            entity.Title = this.LoadString(args, TITLE);
            entity.Description = this.LoadString(args, DESCRIPTION);
            entity.Input = this.LoadString(args, INPUT);
            entity.Output = this.LoadString(args, OUTPUT);
            entity.SampleInput = this.LoadString(args, SAMPLEINPUT);
            entity.SampleOutput = this.LoadString(args, SAMPLEOUT);
            entity.Hint = this.LoadString(args, HINT);
            entity.Source = this.LoadString(args, SOURCE);
            entity.TimeLimit = this.LoadInt32(args, TIMELIMIT);
            entity.MemoryLimit = this.LoadInt32(args, MEMORYLIMIT);
            entity.SubmitCount = this.LoadInt32(args, SUBMITCOUNT);
            entity.SolvedCount = this.LoadInt32(args, SOLVEDCOUNT);
            entity.LastDate = this.LoadDateTime(args, LASTDATE);
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
        public Int32 InsertEntity(ProblemEntity entity)
        {
            return this.GetInsertCommand(entity)
                .Result();
        }

        /// <summary>
        /// 增加一条数据并获取实体ID
        /// </summary>
        /// <param name="entities">对象实体列表</param>
        /// <returns>实体ID,不成功则返回-1</returns>
        public List<Int32> InsertEntities(List<ProblemEntity> entities)
        {
            List<Int32> resultIDs = new List<Int32>();

            if (entities == null || entities.Count < 1)
            {
                return resultIDs;
            }

            this.UsingTransaction(trans =>
            {
                for (Int32 i = 0; i < entities.Count; i++)
                {
                    if (this.GetInsertCommand(entities[i]).Result(trans) > 0)
                    {
                        Int32 pid = this.Select().QueryIdentity().Result(trans);//获取刚插入的Problem ID
                        resultIDs.Add(pid);
                    }
                    else
                    {
                        resultIDs.Add(-1);
                        break;
                    }
                }

                trans.Commit();
            });

            return resultIDs;
        }

        /// <summary>
        /// 获取题目插入SQL语句
        /// </summary>
        /// <param name="entity">题目实体</param>
        /// <returns>插入SQL语句</returns>
        private InsertCommand GetInsertCommand(ProblemEntity entity)
        {
            return this.Insert()
                .Set(TITLE, entity.Title)
                .Set(DESCRIPTION, entity.Description)
                .Set(INPUT, entity.Input)
                .Set(OUTPUT, entity.Output)
                .Set(SAMPLEINPUT, entity.SampleInput)
                .Set(SAMPLEOUT, entity.SampleOutput)
                .Set(HINT, entity.Hint)
                .Set(SOURCE, entity.Source)
                .Set(TIMELIMIT, entity.TimeLimit)
                .Set(MEMORYLIMIT, entity.MemoryLimit)
                .Set(LASTDATE, entity.LastDate)
                .Set(ISHIDE, entity.IsHide);
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新一条数据(只更新题目信息)
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntity(ProblemEntity entity)
        {
            return this.Update()
                .Set(TITLE, entity.Title)
                .Set(DESCRIPTION, entity.Description)
                .Set(INPUT, entity.Input)
                .Set(OUTPUT, entity.Output)
                .Set(SAMPLEINPUT, entity.SampleInput)
                .Set(SAMPLEOUT, entity.SampleOutput)
                .Set(HINT, entity.Hint)
                .Set(SOURCE, entity.Source)
                .Set(TIMELIMIT, entity.TimeLimit)
                .Set(MEMORYLIMIT, entity.MemoryLimit)
                .Set(LASTDATE, entity.LastDate)
                .Where(c => c.Equal(PROBLEMID, entity.ProblemID))
                .Result();
        }

        /// <summary>
        /// 更新题目隐藏状态
        /// </summary>
        /// <param name="ids">实体ID列表</param>
        /// <param name="isHide">隐藏状态</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntityIsHide(String ids, Boolean isHide)
        {
            return this.Update()
                .Set(ISHIDE, isHide)
                .Where(c => c.InInt32(PROBLEMID, ids, ','))
                .Result();
        }

        /// <summary>
        /// 更新一条数据(只更新提交数)
        /// </summary>
        /// <param name="problemID">题目ID</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntitySubmitCount(Int32 problemID)
        {
            return this.InternalGetUpdateSubmitCountCommand(problemID)
                .Result();
        }

        /// <summary>
        /// 更新一条数据(只更新通过数)
        /// </summary>
        /// <param name="problemID">题目ID</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntitySolvedCount(Int32 problemID)
        {
            return this.InternalGetUpdateSolvedCountCommand(problemID)
                .Result();
        }
        #endregion

        #region Select
        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>对象实体</returns>
        public ProblemEntity GetEntity(Int32 id)
        {
            return this.Select()
                .Querys(PROBLEMID, TITLE, DESCRIPTION, INPUT, OUTPUT, SAMPLEINPUT, SAMPLEOUT, HINT, SOURCE, TIMELIMIT, MEMORYLIMIT, SUBMITCOUNT, SOLVEDCOUNT, LASTDATE, ISHIDE)
                .Where(c => c.Equal(PROBLEMID, id))
                .ToEntity(this);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <returns>实体列表</returns>
        public List<ProblemEntity> GetEntities(Int32 pageIndex, Int32 pageSize, Int32 recordCount)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(PROBLEMID, TITLE, ISHIDE, SOLVEDCOUNT, SUBMITCOUNT, LASTDATE)
                .OrderByDesc(PROBLEMID)
                .ToEntityList(this);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns>实体列表</returns>
        public List<ProblemEntity> GetEntitiesForProblemSet(Int32 pageIndex, Int32 pageSize)
        {
            return this.Select()
                .Top(pageSize)
                .Querys(PROBLEMID, TITLE, ISHIDE, SOLVEDCOUNT, SUBMITCOUNT)
                .Where(c => c.GreaterThanOrEqual(PROBLEMID, ConfigurationManager.ProblemSetStartID + (pageIndex - 1) * pageSize) & c.LessThan(PROBLEMID, ConfigurationManager.ProblemSetStartID + (pageIndex) * pageSize))
                .OrderByAsc(PROBLEMID)
                .ToEntityList(this);
        }

        /// <summary>
        /// 根据题目ID获取实体列表
        /// </summary>
        /// <param name="ids">逗号分隔的实体ID</param>
        /// <returns>实体列表</returns>
        public List<ProblemEntity> GetEntitiesByIDs(String ids)
        {
            return this.Select()
                .Querys(PROBLEMID, TITLE, ISHIDE, SOLVEDCOUNT, SUBMITCOUNT)
                .Where(c => c.InInt32(PROBLEMID, ids, ','))
                .OrderByAsc(PROBLEMID)
                .ToEntityList(this);
        }

        /// <summary>
        /// 根据题目类型ID获取实体列表
        /// </summary>
        /// <param name="typeID">题目类型ID</param>
        /// <returns>实体列表</returns>
        public List<ProblemEntity> GetEntitiesByTypeID(Int32 typeID)
        {
            return this.Select()
                .Querys(PROBLEMID, TITLE, ISHIDE, SOLVEDCOUNT, SUBMITCOUNT)
                .InnerJoin(PROBLEMID, ProblemCategoryItemRepository.Instance.TableName, ProblemCategoryItemRepository.PROBLEMID)
                .Where(c => c.Equal(ProblemCategoryItemRepository.TYPEID, typeID))
                .OrderByAsc(PROBLEMID)
                .ToEntityList(this);
        }

        /// <summary>
        /// 根据题目标题获取实体列表
        /// </summary>
        /// <param name="title">题目标题</param>
        /// <returns>实体列表</returns>
        public List<ProblemEntity> GetEntitiesByTitle(String title)
        {
            return this.Select()
                .Querys(PROBLEMID, TITLE, ISHIDE, SOLVEDCOUNT, SUBMITCOUNT)
                .Where(c => c.LikeAll(TITLE, title))
                .OrderByAsc(PROBLEMID)
                .ToEntityList(this);
        }

        /// <summary>
        /// 根据题目来源获取实体列表
        /// </summary>
        /// <param name="source">题目来源</param>
        /// <returns>实体列表</returns>
        public List<ProblemEntity> GetEntitiesBySource(String source)
        {
            return this.Select()
                .Querys(PROBLEMID, TITLE, ISHIDE, SOLVEDCOUNT, SUBMITCOUNT)
                .Where(c => c.LikeAll(SOURCE, source))
                .OrderByAsc(PROBLEMID)
                .ToEntityList(this);
        }
        #endregion

        #region Select4Contest
        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">竞赛题目ID</param>
        /// <returns>对象实体</returns>
        public ProblemEntity GetEntityForContest(Int32 cid, Int32 contestpid)
        {
            return this.Select()
                .Querys(PROBLEMID, TITLE, DESCRIPTION, INPUT, OUTPUT, SAMPLEINPUT, SAMPLEOUT, HINT, SOURCE, TIMELIMIT, MEMORYLIMIT)
                .Query(SolutionRepository.Instance.TableName, s =>
                {
                    s.Query(SqlAggregateFunction.Count)
                        .Where(sc => sc.Equal(SolutionRepository.CONTESTID, cid) & sc.Equal(SolutionRepository.CONTESTPROBLEMID, contestpid));
                }, SUBMITCOUNT)
                .Query(SolutionRepository.Instance.TableName, s =>
                {
                    s.Query(SqlAggregateFunction.Count)
                        .Where(sc => sc.Equal(SolutionRepository.CONTESTID, cid) & sc.Equal(SolutionRepository.CONTESTPROBLEMID, contestpid) & sc.Equal(SolutionRepository.RESULT, (Byte)ResultType.Accepted));
                }, SOLVEDCOUNT)
                .Where(c => c.Equal(PROBLEMID, ContestProblemRepository.Instance.TableName, s =>
                {
                    s.Top(1)
                        .Query(ContestProblemRepository.PROBLEMID)
                        .Where(sc => sc.Equal(ContestProblemRepository.CONTESTID, cid) & sc.Equal(ContestProblemRepository.CONTESTPROBLEMID, contestpid));
                }))
                .ToEntity(this);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>实体列表</returns>
        public List<ProblemEntity> GetEntitiesForContest(Int32 cid)
        {
            String CONTESTPROBLEM_SUBMITCOUNT = "CONTESTPROBLEM_SUBMITCOUNT";
            String CONTESTPROBLEM_SOLVEDCOUNT = "CONTESTPROBLEM_SOLVEDCOUNT";

            return this.Select()
                .Query(ContestProblemRepository.CONTESTPROBLEMID, PROBLEMID)
                .Querys(TITLE, ISHIDE)
                .Query(CONTESTPROBLEM_SUBMITCOUNT, SUBMITCOUNT)
                .Query(CONTESTPROBLEM_SOLVEDCOUNT, SOLVEDCOUNT)
                .Where(c => c.Equal(ContestProblemRepository.CONTESTID, cid))
                .LeftJoin(PROBLEMID, SolutionRepository.Instance.TableName, SolutionRepository.PROBLEMID, s =>
                {
                    s.Query(SolutionRepository.PROBLEMID)
                        .Query(SqlAggregateFunction.Count, false, CONTESTPROBLEM_SUBMITCOUNT)
                        .GroupBy(SolutionRepository.PROBLEMID)
                        .Where(sc => sc.Equal(SolutionRepository.CONTESTID, cid));
                })
                .LeftJoin(PROBLEMID, SolutionRepository.Instance.TableName, SolutionRepository.PROBLEMID, s =>
                {
                    s.Query(SolutionRepository.PROBLEMID)
                        .Query(SqlAggregateFunction.Count, false, CONTESTPROBLEM_SOLVEDCOUNT)
                        .GroupBy(SolutionRepository.PROBLEMID)
                        .Where(sc => sc.Equal(SolutionRepository.CONTESTID, cid) & sc.Equal(SolutionRepository.RESULT, (Byte)ResultType.Accepted));
                })
                .InnerJoin(PROBLEMID, ContestProblemRepository.Instance.TableName, ContestProblemRepository.PROBLEMID)
                .OrderByAsc(ContestProblemRepository.CONTESTPROBLEMID)
                .ToEntityList(this);
        }
        #endregion

        #region Count/Exists
        /// <summary>
        /// 获取实体ID最大值
        /// </summary>
        /// <returns>实体ID最大值数</returns>
        public Int32 GetMaxProblemID()
        {
            return this.Select().Max<Int32>(PROBLEMID);
        }

        /// <summary>
        /// 获取实体总数
        /// </summary>
        /// <returns>实体总数</returns>
        public Int32 CountEntities()
        {
            return this.Select().Count();
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 获取提交题目数的SQL语句
        /// </summary>
        /// <param name="problemID">题目ID</param>
        /// <returns>提交题目数的SQL语句</returns>
        internal UpdateCommand InternalGetIncreaseSubmitCountCommand(Int32 problemID)
        {
            return this.Update()
                .Increase(SUBMITCOUNT)
                .Where(c => c.Equal(PROBLEMID, problemID));
        }

        /// <summary>
        /// 获取增加完成题目信息的SQ语句
        /// </summary>
        /// <param name="problemID">题目ID</param>
        /// <returns>增加完成题目信息的SQL语句</returns>
        internal UpdateCommand InternalGetIncreaseSolvedCountCommand(Int32 problemID)
        {
            return this.Update()
                 .Increase(SOLVEDCOUNT)
                 .Where(c => c.Equal(PROBLEMID, problemID));
        }

        /// <summary>
        /// 获取更新完成题目信息的SQL语句
        /// </summary>
        /// <param name="problemID">题目ID</param>
        /// <returns>更新完成题目信息的SQL语句</returns>
        internal UpdateCommand InternalGetUpdateSolvedCountCommand(Int32 problemID)
        {
            return this.Update()
                .Set(() => this.Database.DatabaseType == DatabaseType.Access, SOLVEDCOUNT,
                    new DCountFunction(
                        SolutionRepository.SOLUTIONID,
                        SolutionRepository.Instance.TableName,
                        String.Format("{0} = {1} AND {2} = {3}",
                            SolutionRepository.PROBLEMID, problemID,
                            SolutionRepository.RESULT, ((Byte)ResultType.Accepted).ToString()
                        )))
                .Set(() => this.Database.DatabaseType != DatabaseType.Access, SOLVEDCOUNT,
                    SolutionRepository.Instance.TableName, s =>
                    {
                        s.Query(SqlAggregateFunction.Count, SolutionRepository.SOLUTIONID)
                            .Where(c => c.Equal(SolutionRepository.PROBLEMID, problemID) & c.Equal(SolutionRepository.RESULT, (Byte)ResultType.Accepted));
                    })
                .Where(c => c.Equal(PROBLEMID, problemID));
        }

        /// <summary>
        /// 获取更新提交题目信息的SQL语句
        /// </summary>
        /// <param name="problemID">题目ID</param>
        /// <returns>更新提交题目信息的SQL语句</returns>
        internal UpdateCommand InternalGetUpdateSubmitCountCommand(Int32 problemID)
        {
            return this.Update()
                .Set(() => this.Database.DatabaseType == DatabaseType.Access, SUBMITCOUNT,
                    new DCountFunction(
                        SolutionRepository.SOLUTIONID,
                        SolutionRepository.Instance.TableName,
                        String.Format("{0} = {1}", SolutionRepository.PROBLEMID, problemID)))
                .Set(() => this.Database.DatabaseType != DatabaseType.Access, SUBMITCOUNT,
                    SolutionRepository.Instance.TableName, s =>
                    {
                        s.Query(SqlAggregateFunction.Count, SolutionRepository.SOLUTIONID)
                            .Where(c => c.Equal(SolutionRepository.PROBLEMID, problemID));
                    })
                .Where(c => c.Equal(PROBLEMID, problemID));
        }
        #endregion
    }
}