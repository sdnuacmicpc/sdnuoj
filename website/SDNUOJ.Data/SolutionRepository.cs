using System;
using System.Collections.Generic;

using DotMaysWind.Data;
using DotMaysWind.Data.Command;
using DotMaysWind.Data.Command.Condition;
using DotMaysWind.Data.Orm;

using SDNUOJ.Configuration;
using SDNUOJ.Data.SolutionDataProviderExtensions;
using SDNUOJ.Entity;
using SDNUOJ.Entity.Complex;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 提交表格操作类
    /// </summary>
    public class SolutionRepository: AbstractDatabaseTable<SolutionEntity>
    {
        #region Instance
        private static SolutionRepository _instance;

        public static SolutionRepository Instance
        {
            get { return _instance; }
        }

        static SolutionRepository()
        {
            _instance = new SolutionRepository();
        }
        #endregion

        #region Const
        internal const String SOLUTIONID = "SOLU_SOLUTIONID";
        internal const String PROBLEMID = "SOLU_PROBLEMID";
        internal const String USERNAME = "SOLU_USERNAME";
        internal const String SOURCECODE = "SOLU_SOURCECODE";
        internal const String LANGUAGE = "SOLU_LANGUAGE";
        internal const String RESULT = "SOLU_RESULT";
        internal const String CODELENGTH = "SOLU_CODELENGTH";
        internal const String CONTESTID = "SOLU_CONTESTID";
        internal const String CONTESTPROBLEMID = "SOLU_CONTESTPROBLEMID";
        internal const String TIMECOST = "SOLU_TIMECOST";
        internal const String MEMORYCOST = "SOLU_MEMORYCOST";
        internal const String SUBMITTIME = "SOLU_SUBMITTIME";
        internal const String JUDGETIME = "SOLU_JUDGETIME";
        internal const String SUBMITIP = "SOLU_SUBMITIP";

        public const Int32 SOURCECODE_MINLEN = 2;
        public const Int32 SOURCECODE_MAXLEN = 32767;
        #endregion

        #region 重载属性和方法
        public override String TableName
        {
            get { return "SDNUOJ_SOLUTION"; }
        }

        private SolutionRepository() : base(MainDatabase.Instance) { }

        protected override SolutionEntity CreateEntity(Object sender, EntityCreatingArgs args)
        {
            SolutionEntity entity = new SolutionEntity();

            entity.SolutionID = this.LoadInt32(args, SOLUTIONID);
            entity.ProblemID = this.LoadInt32(args, PROBLEMID);
            entity.UserName = this.LoadString(args, USERNAME);
            entity.SourceCode = this.LoadString(args, SOURCECODE);
            entity.LanguageType = LanguageType.FromLanguageID(this.LoadByte(args, LANGUAGE));
            entity.Result = (ResultType)this.LoadByte(args, RESULT);
            entity.CodeLength = this.LoadInt32(args, CODELENGTH);
            entity.ContestID = this.LoadInt32(args, CONTESTID);
            entity.ContestProblemID = this.LoadInt32(args, CONTESTPROBLEMID);
            entity.TimeCost = this.LoadInt32(args, TIMECOST);
            entity.MemoryCost = this.LoadInt32(args, MEMORYCOST);
            entity.SubmitTime = this.LoadDateTime(args, SUBMITTIME);
            entity.JudgeTime = this.LoadDateTime(args, JUDGETIME);

            return entity;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 InsertEntity(SolutionEntity entity)
        {
            return this.Sequence()
                .Insert(i =>
                {
                    i.Set(PROBLEMID, entity.ProblemID)
                        .Set(USERNAME, entity.UserName)
                        .Set(SOURCECODE, entity.SourceCode)
                        .Set(LANGUAGE, entity.LanguageType.ID)
                        .Set(RESULT, (Byte)ResultType.Pending)
                        .Set(CODELENGTH, entity.SourceCode.Length)
                        .Set(CONTESTID, entity.ContestID)
                        .Set(CONTESTPROBLEMID, entity.ContestProblemID)
                        .Set(SUBMITTIME, entity.SubmitTime)
                        .Set(SUBMITIP, entity.SubmitIP);
                })//提交插入
                .Add(ProblemRepository.Instance.InternalGetIncreaseSubmitCountCommand(entity.ProblemID))//更新题目提交数
                .Add(UserRepository.Instance.InternalGetIncreaseSubmitCountCommand(entity.UserName))//更新用户提交数
                .Result();
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新一条数据(更新评测相关信息)
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <param name="error">编译错误</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntity(SolutionEntity entity, String error)
        {
            return this.UsingConnection<Int32>(conn =>
            {
                Int32 result = this.Update()
                    .Set(RESULT, (Byte)entity.Result)
                    .Set(TIMECOST, entity.TimeCost)
                    .Set(MEMORYCOST, entity.MemoryCost)
                    .Set(JUDGETIME, entity.JudgeTime)
                    .Where(c => c.Equal(SOLUTIONID, entity.SolutionID) & c.LessThanOrEqual(RESULT, (Byte)ResultType.Judging))
                    .Result(conn);

                if (result > 0)
                {
                    try
                    {
                        ProblemRepository.Instance.InternalGetUpdateSolvedCountCommand(entity.ProblemID).Result(conn);//更新题目完成数
                    }
                    catch { }

                    try
                    {
                        UserRepository.Instance.InternalGetUpdateSolvedCountCommand(entity.UserName).Result(conn);//更新用户完成数
                    }
                    catch { }

                    try
                    {
                        if (entity.Result == ResultType.CompileError || entity.Result == ResultType.RuntimeError)//如果编译错误或运行错误
                        {
                            SolutionErrorRepository.Instance.InternalGetDeleteEntityCommand(entity.SolutionID).Result(conn);//删除已有编译错误
                            SolutionErrorRepository.Instance.InternalGetInsertEntityCommand(entity.SolutionID, error).Result(conn);//插入编译错误
                        }
                    }
                    catch { }
                }

                return result;
            });
        }

        /// <summary>
        /// 更新一条数据(只更新评测信息)
        /// </summary>
        /// <param name="ids">逗号分隔的实体ID</param>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="userName">用户名</param>
        /// <param name="languageType">提交语言</param>
        /// <param name="resultType">提交结果</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntityToRejudge(String ids, Int32 cid, Int32 pid, String userName, LanguageType languageType, ResultType? resultType, DateTime? startDate, DateTime? endDate)
        {
            return this.Update()
                .Set(RESULT, (Byte)ResultType.RejudgePending)
                .Where(ids, cid, pid, userName, languageType, resultType, startDate, endDate)
                .Result();
        }
        #endregion

        #region Select
        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>对象实体</returns>
        public SolutionEntity GetEntity(Int32 id)
        {
            return this.Select()
                .Querys(SOLUTIONID, PROBLEMID, CONTESTID, CONTESTPROBLEMID, USERNAME, SOURCECODE, LANGUAGE, RESULT, CODELENGTH, TIMECOST, MEMORYCOST, SUBMITTIME, JUDGETIME)
                .Where(c => c.Equal(SOLUTIONID, id))
                .ToEntity(this);
        }

        /// <summary>
        /// 获取最早提交等待评测的一个实体
        /// </summary>
        /// <param name="count">获取个数</param>
        /// <param name="getErrorTime">获取异常提交时间</param>
        /// <param name="languageSupport">支持语言</param>
        /// <returns>对象实体</returns>
        public List<SolutionEntity> GetPendingEntities(Int32 count, Int32 getErrorTime, LanguageType[] languageSupport)
        {
            return this.UsingTransaction<List<SolutionEntity>>(trans =>
            {
                List<SolutionEntity> list = this.Select()
                    .Top(count)
                    .Querys(SOLUTIONID, PROBLEMID, USERNAME, SOURCECODE, LANGUAGE, CODELENGTH, SUBMITTIME)
                    .Where(c => 
                    {
                        AbstractSqlCondition condition = c.LessThanOrEqual(RESULT, (Byte)ResultType.RejudgePending);
                        AbstractSqlCondition temp = null;

                        if (getErrorTime > 0)
                        {
                            temp = c.Equal(RESULT, (Byte)ResultType.Judging) & c.LessThanOrEqual(JUDGETIME, DateTime.Now.AddSeconds(-getErrorTime));
                            condition = (condition == null ? temp : condition | temp);
                        }

                        if (languageSupport.Length > 0 & languageSupport.Length < LanguageManager.LanguageSupportedCount)//没有和全部时不设定条件
                        {
                            temp = c.In(LANGUAGE, () =>
                            {
                                Byte[] langIds = new Byte[languageSupport.Length];

                                for (Int32 i = 0; i < langIds.Length; i++)
                                {
                                    langIds[i] = languageSupport[i].ID;
                                }

                                return langIds;
                            });
                            condition = (condition == null ? temp : condition & temp);
                        }

                        return condition;
                    })
                    .OrderByAsc(SOLUTIONID)
                    .ToEntityList(this, trans);

                //提交获取到的题目状态为Juding且评测时间为当前
                if (list != null && list.Count > 0)
                {
                    Int32 updated = 0;

                    for (Int32 i = 0; i < list.Count; i++)
                    {
                        updated += this.InternalGetUpdateSolutionResultCommand(list[i].SolutionID, ResultType.Judging, DateTime.Now)
                            .Result(trans);
                    }
                }

                trans.Commit();

                return list;
            });
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="userName">用户名</param>
        /// <param name="languageType">提交语言</param>
        /// <param name="resultType">提交结果</param>
        /// <param name="order">排序顺序</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <returns>实体列表</returns>
        public List<SolutionEntity> GetEntities(Int32 cid, Int32 pid, String userName, LanguageType languageType, ResultType? resultType, Int32 order, Int32 pageIndex, Int32 pageSize, Int32 recordCount)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(SOLUTIONID, PROBLEMID, USERNAME, LANGUAGE, RESULT, CODELENGTH, CONTESTPROBLEMID, TIMECOST, MEMORYCOST, SUBMITTIME, JUDGETIME)
                .Where(cid, pid, userName, languageType, resultType)
                .Then(it => 
                {
                    //排序顺序 order=0为按时间 =1为按内存 =2为按大小
                    if (order == 0) it.OrderByAsc(TIMECOST);
                    else if (order == 1) it.OrderByAsc(MEMORYCOST);
                    else if (order == 2) it.OrderByAsc(CODELENGTH);
                    
                    it.OrderByDesc(SOLUTIONID);
                })
                .ToEntityList(this);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="ids">逗号分隔的实体ID</param>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="userName">用户名</param>
        /// <param name="languageType">提交语言</param>
        /// <param name="resultType">提交结果</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="order">排序顺序</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <returns>实体列表</returns>
        public List<SolutionEntity> GetEntities(String ids, Int32 cid, Int32 pid, String userName, LanguageType languageType, ResultType? resultType, DateTime? startDate, DateTime? endDate, Int32 order, Int32 pageIndex, Int32 pageSize, Int32 recordCount)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(SOLUTIONID, PROBLEMID, USERNAME, LANGUAGE, RESULT, CODELENGTH, CONTESTID, CONTESTPROBLEMID, TIMECOST, MEMORYCOST, SUBMITTIME, JUDGETIME)
                .Where(ids, cid, pid, userName, languageType, resultType, startDate, endDate)
                .Then(it =>
                {
                    //排序顺序 order=0为按用户名 =1为按竞赛ID =2为按题目ID =3为按提交结果 =4为按用时 =5为按内存 =6为按长度 =7为按语言
                    if (order == 0) it.OrderByAsc(USERNAME);
                    else if (order == 1) it.OrderByAsc(CONTESTID);
                    else if (order == 2) it.OrderByAsc(PROBLEMID);
                    else if (order == 3) it.OrderByAsc(RESULT);
                    else if (order == 4) it.OrderByAsc(TIMECOST);
                    else if (order == 5) it.OrderByAsc(MEMORYCOST);
                    else if (order == 6) it.OrderByAsc(CODELENGTH);
                    else if (order == 7) it.OrderByAsc(LANGUAGE);

                    it.OrderByDesc(SOLUTIONID);
                })
                .ToEntityList(this);
        }

        /// <summary>
        /// 根据用户名获取AC代码列表
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>AC代码列表</returns>
        public List<SolutionEntity> GetEntitiesForAcceptedSourceCode(String userName)
        {
            return this.Select()
                .Querys(SOLUTIONID, PROBLEMID, LANGUAGE, SOURCECODE)
                .Where(c => c.Equal(USERNAME, userName) & c.Equal(RESULT, (Byte)ResultType.Accepted))
                .ToEntityList(this);
        }

        /// <summary>
        /// 根据用户名和提交结果获取题目ID列表
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="contestID">竞赛ID</param>
        /// <param name="resultType">提交结果</param>
        /// <returns>题目ID列表</returns>
        public List<Int32> GetEntityIDsByUserAndResultType(String userName, Int32 contestID, ResultType resultType)
        {
            String problemIDColumn = (contestID < 0 ? PROBLEMID : CONTESTPROBLEMID);

            return this.Select()
                .Query(problemIDColumn)
                .Distinct()
                .Where(c =>
                {
                    AbstractSqlCondition condition = c.Equal(USERNAME, userName) & c.Equal(RESULT, (Byte)resultType);

                    if (contestID >= 0)
                    {
                        condition = condition & c.Equal(CONTESTID, contestID);
                    }

                    return condition;
                })
                .OrderByAsc(problemIDColumn)
                .ToDataTable()
                .EachToList<Int32>((list, args) => list.Add((Int32)args.Row[problemIDColumn]));
        }

        /// <summary>
        /// 根据用户名和小于提交结果获取题目ID列表
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="contestID">竞赛ID</param>
        /// <param name="resultType">提交结果</param>
        /// <returns>题目ID列表</returns>
        public List<Int32> GetEntityIDsByUserAndLessResultType(String userName, Int32 contestID, ResultType resultType)
        {
            String problemIDColumn = (contestID < 0 ? PROBLEMID : CONTESTPROBLEMID);

            return this.Select()
                .Query(problemIDColumn)
                .Distinct()
                .Where(c =>
                {
                    AbstractSqlCondition condition = c.Equal(USERNAME, userName) & c.LessThan(RESULT, (Byte)resultType);

                    if (contestID >= 0)
                    {
                        condition = condition & c.Equal(CONTESTID, contestID);
                    }

                    return condition;
                })
                .OrderByAsc(problemIDColumn)
                .ToDataTable()
                .EachToList<Int32>((list, args) => list.Add((Int32)args.Row[problemIDColumn]));
        }
        #endregion

        #region GetProblemStatistic
        /// <summary>
        /// 获取题目统计信息
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <returns>题目统计信息实体</returns>
        public ProblemStatistic GetProblemStatistic(Int32 cid, Int32 pid)
        {
            return this.UsingConnection<ProblemStatistic>(conn =>
            {
                ProblemStatistic entity = new ProblemStatistic();
                entity.ProblemID = pid;
                entity.SolvedCount = this.Select("T1", from =>
                    {
                        from.Querys(USERNAME)
                            .Distinct()
                            .Where(c =>
                            {
                                AbstractSqlCondition condition = c.Equal((cid > 0 ? CONTESTPROBLEMID : PROBLEMID), pid) & c.Equal(RESULT, (Byte)ResultType.Accepted);

                                if (cid >= 0)//竞赛ID，非竞赛为0
                                {
                                    condition = condition & c.Equal(CONTESTID, cid);
                                }

                                return condition;
                            });
                    })
                    .Count(conn);//计算AC的用户数

                return this.Select()
                    .Query(SqlAggregateFunction.Count, SOLUTIONID, "SOLU_COUNT")
                    .Query(RESULT)
                    .Where(c =>
                    {
                        AbstractSqlCondition condition = c.Equal((cid >= 0 ? CONTESTPROBLEMID : PROBLEMID), pid);

                        if (cid >= 0)//竞赛ID，非竞赛为0
                        {
                            condition = condition & c.Equal(CONTESTID, cid);
                        }

                        return condition;
                    })
                    .GroupBy(RESULT)
                    .ToDataTable(conn)
                    .Each(args =>
                    {
                        Int32 count = this.LoadInt32(args, "SOLU_COUNT");
                        ResultType type = (ResultType)this.LoadByte(args, RESULT);

                        entity.SubmitCount += count;

                        switch (type)
                        {
                            case ResultType.Pending: entity.PendingCount = count; break;
                            case ResultType.RejudgePending: entity.RejudgePendingCount = count; break;
                            case ResultType.Judging: entity.JudgingCount = count; break;
                            case ResultType.CompileError: entity.CompileErrorCount = count; break;
                            case ResultType.RuntimeError: entity.RuntimeErrorCount = count; break;
                            case ResultType.TimeLimitExceeded: entity.TimeLimitExceededCount = count; break;
                            case ResultType.MemoryLimitExceeded: entity.MemoryLimitExceededCount = count; break;
                            case ResultType.OutputLimitExceeded: entity.OutputLimitExceededCount = count; break;
                            case ResultType.WrongAnswer: entity.WrongAnswerCount = count; break;
                            case ResultType.PresentationError: entity.PresentationErrorCount = count; break;
                            case ResultType.Accepted: entity.AcceptedCount = count; break;
                        }
                    })
                    .Done(entity);
            });
        }
        #endregion

        #region GetSubmitStatus
        /// <summary>
        /// 获取提交统计信息
        /// </summary>
        /// <param name="dtStart">开始日期</param>
        /// <param name="dtEnd">结束日期</param>
        /// <param name="acceptedOnly">是否仅有AC题目</param>
        /// <returns>提交统计信息</returns>
        public IDictionary<Int32, Int32> GetSubmitStatus(DateTime dtStart, DateTime dtEnd, Boolean acceptedOnly)
        {
            //SELECT COUNT(*) AS SOLU_COUNT, #DatePart(SOLU_SUBMITTIME)# AS SOLU_SUBMITDATE 
            //FROM SDNUOJ_SOLUTION 
            //WHERE CONDITION 
            //GROUP BY #DatePart(SOLU_SUBMITTIME)#

            SortedDictionary<Int32, Int32> submits = new SortedDictionary<Int32, Int32>();

            return this.Select()
                .Query(SqlAggregateFunction.Count, SOLUTIONID, "SOLU_COUNT")
                .Query(this.Functions.DatePart(SUBMITTIME, SqlDatePartType.DayOfMonth), "SOLU_SUBMITDATE")
                .Where(c =>
                {
                    AbstractSqlCondition condition = c.Between(SUBMITTIME, dtStart, dtEnd);

                    if (acceptedOnly)
                    {
                        condition = condition & c.Equal(RESULT, (Byte)ResultType.Accepted);
                    }

                    return condition;
                })
                .GroupBy(this.Functions.DatePart(SUBMITTIME, SqlDatePartType.DayOfMonth))
                .ToDataTable()
                .Each(args =>
                {
                    if (!DbConvert.IsDBNull(args.Row["SOLU_SUBMITDATE"]))
                    {
                        Int32 dayOfMonth = this.LoadInt32(args, "SOLU_SUBMITDATE");
                        Int32 count = this.LoadInt32(args, "SOLU_COUNT");

                        submits[dayOfMonth] = count;
                    }
                })
                .Done(submits);
        }
        #endregion

        #region GetContestRanklist
        /// <summary>
        /// 获取竞赛用户排名数据
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="start">竞赛开始时间</param>
        /// <returns>用户排名数据</returns>
        public Dictionary<String, RankItem> GetContestRanklist(Int32 cid, DateTime start)
        {
            return this.UsingConnection<Dictionary<String, RankItem>>(conn => 
            {
                Dictionary<String, RankItem> rank = new Dictionary<String, RankItem>();
                HashSet<Int32> firstBlooded = new HashSet<Int32>();

                //获取AC用户列表
                this.Select()
                    .Querys(USERNAME, CONTESTPROBLEMID, SUBMITTIME)
                    .Where(c => c.Equal(CONTESTID, cid) & c.Equal(RESULT, (Byte)ResultType.Accepted))
                    .OrderByAsc(SUBMITTIME)
                    .ToDataTable(conn)
                    .Each(args => 
                    {
                        if (DbConvert.IsDBNull(args.Row[CONTESTPROBLEMID]))
                        {
                            return;
                        }

                        String userName = this.LoadString(args, USERNAME);
                        Int32 contestProblemID = this.LoadInt32(args, CONTESTPROBLEMID);
                        DateTime submitTime = this.LoadDateTime(args, SUBMITTIME);

                        RankItem userRank = null;

                        if (!rank.TryGetValue(userName, out userRank))
                        {
                            userRank = new RankItem(userName);
                        }

                        if (!userRank.Penaltys.ContainsKey(contestProblemID))
                        {
                            Boolean firstBlood = false;

                            if (!firstBlooded.Contains(contestProblemID))
                            {
                                firstBlood = true;
                                firstBlooded.Add(contestProblemID);
                            }

                            userRank.AddAcceptedProblem(contestProblemID, submitTime - start, firstBlood);
                            rank[userName] = userRank;
                        }
                    });

                //获取错误用户列表
                this.Select()
                    .Querys(USERNAME, CONTESTPROBLEMID)
                    .Query(SqlAggregateFunction.Count, SOLUTIONID, "SOLU_WRONGCOUNT")
                    .Where(c => c.Equal(CONTESTID, cid) & c.LessThan(RESULT, (Byte)ResultType.Accepted))
                    .GroupByThese(USERNAME, CONTESTPROBLEMID)
                    .ToDataTable(conn)
                    .Each(args =>
                    {
                        if (DbConvert.IsDBNull(args.Row[CONTESTPROBLEMID]))
                        {
                            return;
                        }

                        String userName = this.LoadString(args, USERNAME);
                        Int32 contestProblemID = this.LoadInt32(args, CONTESTPROBLEMID);
                        Int32 wrongCount = this.LoadInt32(args, "SOLU_WRONGCOUNT");

                        RankItem userRank = null;
                        if (!rank.TryGetValue(userName, out userRank))
                        {
                            userRank = new RankItem(userName);
                        }

                        userRank.AddWrongProblemCount(contestProblemID, wrongCount);
                        rank[userName] = userRank;
                    });

                return rank;
            });
        }
        #endregion

        #region GetContestStatistic
        /// <summary>
        /// 获取竞赛题目统计信息
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>竞赛题目统计信息实体列表</returns>
        public IDictionary<Int32, ContestProblemStatistic> GetContestStatistic(Int32 cid)
        {
            return this.UsingConnection<IDictionary<Int32, ContestProblemStatistic>>(conn => 
            {
                SortedDictionary<Int32, ContestProblemStatistic> statistics = new SortedDictionary<Int32, ContestProblemStatistic>();

                //计算每种RESULT的提交数
                this.Select()
                    .Querys(CONTESTPROBLEMID, RESULT)
                    .Query(SqlAggregateFunction.Count, SOLUTIONID, "SOLU_COUNT")
                    .Where(c => c.Equal(CONTESTID, cid))
                    .GroupByThese(CONTESTPROBLEMID, RESULT)
                    .ToDataTable(conn)
                    .Each(args => 
                    {
                        Int32 pid = this.LoadInt32(args, CONTESTPROBLEMID);
                        Int32 count = this.LoadInt32(args, "SOLU_COUNT");
                        ResultType type = (ResultType)this.LoadByte(args, RESULT);

                        ContestProblemStatistic entity = null;

                        if (!statistics.TryGetValue(pid, out entity))
                        {
                            entity = new ContestProblemStatistic() { ProblemID = pid };
                            statistics[pid] = entity;
                        }

                        entity.SubmitCount += count;

                        switch (type)
                        {
                            case ResultType.Pending: entity.PendingCount = count; break;
                            case ResultType.RejudgePending: entity.RejudgePendingCount = count; break;
                            case ResultType.Judging: entity.JudgingCount = count; break;
                            case ResultType.CompileError: entity.CompileErrorCount = count; break;
                            case ResultType.RuntimeError: entity.RuntimeErrorCount = count; break;
                            case ResultType.TimeLimitExceeded: entity.TimeLimitExceededCount = count; break;
                            case ResultType.MemoryLimitExceeded: entity.MemoryLimitExceededCount = count; break;
                            case ResultType.OutputLimitExceeded: entity.OutputLimitExceededCount = count; break;
                            case ResultType.WrongAnswer: entity.WrongAnswerCount = count; break;
                            case ResultType.PresentationError: entity.PresentationErrorCount = count; break;
                            case ResultType.Accepted: entity.AcceptedCount = count; break;
                        }
                    });

                //计算每种LANGID的提交数
                this.Select()
                    .Querys(CONTESTPROBLEMID, LANGUAGE)
                    .Query(SqlAggregateFunction.Count, SOLUTIONID, "SOLU_COUNT")
                    .Where(c => c.Equal(CONTESTID, cid))
                    .GroupByThese(CONTESTPROBLEMID, LANGUAGE)
                    .ToDataTable(conn)
                    .Each(args =>
                    {
                        Int32 pid = this.LoadInt32(args, CONTESTPROBLEMID);
                        Int32 count = this.LoadInt32(args, "SOLU_COUNT");
                        Byte type = this.LoadByte(args, LANGUAGE);

                        ContestProblemStatistic entity = null;

                        if (!statistics.TryGetValue(pid, out entity))
                        {
                            entity = new ContestProblemStatistic() { ProblemID = pid };
                            statistics[pid] = entity;
                        }

                        entity.SetLanguageStatistic(type, count);
                    });

                return statistics;
            });
        }
        #endregion

        #region Count/Exists
        /// <summary>
        /// 获取实体总数
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="userName">用户名</param>
        /// <param name="languageType">提交语言</param>
        /// <param name="resultType">提交结果</param>
        /// <returns>实体总数</returns>
        public Int32 CountEntities(Int32 cid, Int32 pid, String userName, LanguageType languageType, ResultType? resultType)
        {
            return this.Select()
                .Where(cid, pid, userName, languageType, resultType)
                .Count();
        }

        /// <summary>
        /// 获取实体总数
        /// </summary>
        /// <param name="ids">逗号分隔的实体ID</param>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="userName">用户名</param>
        /// <param name="languageType">提交语言</param>
        /// <param name="resultType">提交结果</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>实体总数</returns>
        public Int32 CountEntities(String ids, Int32 cid, Int32 pid, String userName, LanguageType languageType, ResultType? resultType, DateTime? startDate, DateTime? endDate)
        {
            return this.Select()
                .Where(ids, cid, pid, userName, languageType, resultType, startDate, endDate)
                .Count();
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 获取更新评测信息的SQL语句
        /// </summary>
        /// <param name="sid">实体ID</param>
        /// <param name="type">评测状态</param>
        /// <param name="judgeTime">评测时间</param>
        /// <returns>SQL语句</returns>
        internal UpdateCommand InternalGetUpdateSolutionResultCommand(Int32 sid, ResultType type, DateTime judgeTime)
        {
            return this.Update()
                .Set(RESULT, (Byte)type)
                .Set(JUDGETIME, judgeTime)
                .Where(c => c.Equal(SOLUTIONID, sid));
        }

        /// <summary>
        /// 获取指定时间内用户排名的SQL语句
        /// </summary>
        /// <param name="dtStart">开始日期</param>
        /// <param name="dtEnd">结束日期</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns>指定时间内用户排名的SQL语句</returns>
        internal SelectCommand InternalGetTopUserCommand(DateTime dtStart, DateTime dtEnd, Int32 pageSize)
        {
            //ACCESS: SELECT TOP 10 COUNT(*) AS USER_SOLVEDCOUNT, SOLU_USERNAME AS USER_USERNAME 
            //        FROM (SELECT DISTINCT SOLU_PROBLEMID, SOLU_USERNAME 
            //              FROM SDNUOJ_SOLUTION 
            //              WHERE CONDITION) 
            //        GROUP BY SOLU_USERNAME 
            //        ORDER BY COUNT(*) DESC,SOLU_USERNAME ASC

            //OTHER:  SELECT TOP 10 COUNT(DISTINCT SOLU_PROBLEMID) AS USER_SOLVEDCOUNT, SOLU_USERNAME AS USER_USERNAME 
            //        FROM SDNUOJ_SOLUTION 
            //        WHERE CONDITION 
            //        GROUP BY SOLU_USERNAME 
            //        ORDER BY COUNT(*) DESC,SOLU_USERNAME ASC

            Boolean notSupportCountDistinct = (this.Database.DatabaseType == DatabaseType.Access || this.Database.DatabaseType == DatabaseType.SqlServerCe);

            if (notSupportCountDistinct)
            {
                return this.Select("T1", from =>
                    {
                        from.Querys(PROBLEMID, USERNAME)
                            .Distinct()
                            .Where(c => c.Between(SUBMITTIME, dtStart, dtEnd) & c.Equal(RESULT, (Byte)ResultType.Accepted));
                    })
                    .Top(pageSize)
                    .Query(SqlAggregateFunction.Count, false, UserRepository.SOLVEDCOUNT)
                    .Query(USERNAME, UserRepository.USERNAME)
                    .GroupBy(USERNAME)
                    .OrderBy(SqlAggregateFunction.Count, false, SqlOrderType.Desc)
                    .OrderByAsc(USERNAME);
            }
            else
            {
                return this.Select()
                    .Top(pageSize)
                    .Query(SqlAggregateFunction.Count, PROBLEMID, true, UserRepository.SOLVEDCOUNT)
                    .Query(USERNAME, UserRepository.USERNAME)
                    .Where(c => c.Between(SUBMITTIME, dtStart, dtEnd) & c.Equal(RESULT, (Byte)ResultType.Accepted))
                    .GroupBy(USERNAME)
                    .OrderBy(SqlAggregateFunction.Count, PROBLEMID, true, SqlOrderType.Desc)
                    .OrderByAsc(USERNAME);
            }
        }
        #endregion
    }
}

namespace SDNUOJ.Data.SolutionDataProviderExtensions
{
    #region SolutionProviderExtension
    internal static class SolutionDataProviderExtension
    {
        /// <summary>
        /// 设置指定的查询语句并返回当前语句
        /// </summary>
        /// <param name="command">相关语句</param>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="userName">用户名</param>
        /// <param name="languageType">提交语言</param>
        /// <param name="resultType">提交结果</param>
        /// <returns>当前语句</returns>
        internal static SelectCommand Where(this SelectCommand command, Int32 cid, Int32 pid, String userName, LanguageType languageType, ResultType? resultType)
        {
            SqlConditionBuilder c = command.ConditionBuilder;
            AbstractSqlCondition condition = command.WhereCondition as AbstractSqlCondition;
            AbstractSqlCondition temp = null;

            //竞赛ID，非竞赛为-1
            if (cid >= 0)
            {
                temp = c.Equal(SolutionRepository.CONTESTID, cid);
                condition = (condition == null ? temp : condition & temp);
            }

            //题目ID，如果为竞赛从竞赛题目顺序查找，否则从题目ID查找
            if (pid >= 0)
            {
                temp = c.Equal((cid >= 0 ? SolutionRepository.CONTESTPROBLEMID : SolutionRepository.PROBLEMID), pid);
                condition = (condition == null ? temp : condition & temp);
            }

            //用户名
            if (!String.IsNullOrEmpty(userName))
            {
                temp = c.Equal(SolutionRepository.USERNAME, userName);
                condition = (condition == null ? temp : condition & temp);
            }

            //提交程序语言
            if (!LanguageType.IsNull(languageType))
            {
                temp = c.Equal(SolutionRepository.LANGUAGE, languageType.ID);
                condition = (condition == null ? temp : condition & temp);
            }

            //提交结果
            if (resultType.HasValue)
            {
                temp = c.Equal(SolutionRepository.RESULT, (Byte)resultType.Value);
                condition = (condition == null ? temp : condition & temp);
            }

            return command.Where(condition);
        }

        /// <summary>
        /// 设置指定的查询语句并返回当前语句
        /// </summary>
        /// <param name="command">相关语句</param>
        /// <param name="ids">逗号分隔的实体ID</param>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="userName">用户名</param>
        /// <param name="languageType">提交语言</param>
        /// <param name="resultType">提交结果</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>当前语句</returns>
        internal static T Where<T>(this T command, String ids, Int32 cid, Int32 pid, String userName, LanguageType languageType, ResultType? resultType, DateTime? startDate, DateTime? endDate) where T : AbstractSqlCommandWithWhere
        {
            SqlConditionBuilder c = command.ConditionBuilder;
            AbstractSqlCondition condition = command.WhereCondition as AbstractSqlCondition;
            AbstractSqlCondition temp = null;

            //提交ID
            if (!String.IsNullOrEmpty(ids))
            {
                temp = c.InInt32(SolutionRepository.SOLUTIONID, ids, ',');
                condition = (condition == null ? temp : condition & temp);
            }

            //竞赛ID
            if (cid >= 0)
            {
                temp = c.Equal(SolutionRepository.CONTESTID, cid);
                condition = (condition == null ? temp : condition & temp);
            }

            //题目ID
            if (pid >= 0)
            {
                temp = c.Equal(SolutionRepository.PROBLEMID, pid);
                condition = (condition == null ? temp : condition & temp);
            }

            //用户名
            if (!String.IsNullOrEmpty(userName))
            {
                temp = c.Equal(SolutionRepository.USERNAME, userName);
                condition = (condition == null ? temp : condition & temp);
            }

            //提交程序语言
            if (!LanguageType.IsNull(languageType))
            {
                temp = c.Equal(SolutionRepository.LANGUAGE, languageType.ID);
                condition = (condition == null ? temp : condition & temp);
            }

            //提交结果
            if (resultType.HasValue)
            {
                temp = c.Equal(SolutionRepository.RESULT, (Byte)resultType.Value);
                condition = (condition == null ? temp : condition & temp);
            }

            //开始时间
            if (startDate.HasValue || endDate.HasValue)
            {
                temp = c.BetweenNullable<DateTime>(SolutionRepository.SUBMITTIME, startDate, endDate);
                condition = (condition == null ? temp : condition & temp);
            }

            command.Where(condition);

            return command;
        }
    }
    #endregion
}