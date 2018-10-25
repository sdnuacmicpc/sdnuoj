using System;
using System.Collections.Generic;

using DotMaysWind.Data;
using DotMaysWind.Data.Command;
using DotMaysWind.Data.Command.Condition;
using DotMaysWind.Data.Orm;

using SDNUOJ.Data.Functions;
using SDNUOJ.Data.UserDataProviderExtensions;
using SDNUOJ.Entity;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 用户表格操作类
    /// </summary>
    public class UserRepository : AbstractDatabaseTable<UserEntity>
    {
        #region Instance
        private static UserRepository _instance;

        public static UserRepository Instance
        {
            get { return _instance; }
        }

        static UserRepository()
        {
            _instance = new UserRepository();
        }
        #endregion

        #region Const
        internal const String USERNAME = "USER_USERNAME";
        internal const String PASSWORD = "USER_PASSWORD";
        internal const String NICKNAME = "USER_NICKNAME";
        internal const String EMAIL = "USER_EMAIL";
        internal const String SCHOOL = "USER_SCHOOL";
        internal const String PERMISSION = "USER_PERMISSION";
        internal const String SUBMITCOUNT = "USER_SUBMITCOUNT";
        internal const String SOLVEDCOUNT = "USER_SOLVEDCOUNT";
        internal const String ISLOCKED = "USER_ISLOCKED";
        internal const String CREATEIP = "USER_CREATEIP";
        internal const String CREATEDATE = "USER_CREATEDATE";
        internal const String LASTIP = "USER_LASTIP";
        internal const String LASTDATE = "USER_LASTDATE";
        internal const String RANK = "USER_RANK";

        public const Int32 USERNAME_MAXLEN = 20;
        public const Int32 NICKNAME_MAXLEN = 40;
        public const Int32 EMAIL_MAXLEN = 40;
        public const Int32 SCHOOL_MAXLEN = 40;
        #endregion

        #region 重载属性和方法
        public override String TableName
        {
            get { return "SDNUOJ_USER"; }
        }

        private UserRepository() : base(MainDatabase.Instance) { }

        protected override UserEntity CreateEntity(Object sender, EntityCreatingArgs args)
        {
            UserEntity entity = new UserEntity();

            entity.UserName = this.LoadString(args, USERNAME);
            entity.PassWord = this.LoadString(args, PASSWORD);
            entity.NickName = this.LoadString(args, NICKNAME);
            entity.Email = this.LoadString(args, EMAIL);
            entity.School = this.LoadString(args, SCHOOL);
            entity.Permission = (PermissionType)this.LoadInt32(args, PERMISSION);
            entity.SubmitCount = this.LoadInt32(args, SUBMITCOUNT);
            entity.SolvedCount = this.LoadInt32(args, SOLVEDCOUNT);
            entity.IsLocked = this.LoadBoolean(args, ISLOCKED);
            entity.CreateIP = this.LoadString(args, CREATEIP);
            entity.CreateDate = this.LoadDateTime(args, CREATEDATE);
            entity.LastIP = this.LoadString(args, LASTIP);
            entity.LastDate = this.LoadDateTime(args, LASTDATE);
            entity.Rank = this.LoadDouble(args, RANK);

            if (String.IsNullOrEmpty(entity.NickName) && args.ExtraArgument is Boolean && ((Boolean)args.ExtraArgument))
            {
                entity.NickName = entity.UserName;
            }

            return entity;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 InsertEntity(UserEntity entity)
        {
            return this.Insert()
                .Set(USERNAME, entity.UserName)
                .Set(PASSWORD, entity.PassWord)
                .Set(NICKNAME, entity.NickName)
                .Set(EMAIL, entity.Email)
                .Set(SCHOOL, entity.School)
                .Set(PERMISSION, (Int32)entity.Permission)
                .Set(CREATEIP, entity.CreateIP)
                .Set(CREATEDATE, entity.CreateDate)
                .Result();
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新一条数据(只更新用户信息)
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntityForUser(UserEntity entity, String currentPassword)
        {
            return this.Update()
                .Set(() => !String.IsNullOrEmpty(entity.PassWord), PASSWORD, entity.PassWord)
                .Set(NICKNAME, entity.NickName)
                .Set(EMAIL, entity.Email)
                .Set(SCHOOL, entity.School)
                .Where(c => c.Equal(USERNAME, entity.UserName) & c.Equal(PASSWORD, currentPassword))
                .Result();
        }

        ///// <summary>
        ///// 更新一条数据(只更新评测机信息)
        ///// </summary>
        ///// <param name="entity">对象实体</param>
        ///// <returns>操作影响的记录数</returns>
        //public Int32 UpdateEntityForJudge(UserEntity entity)
        //{
        //    return this.Update()
        //        .Set(() => !String.IsNullOrEmpty(entity.PassWord), PASSWORD, entity.PassWord)
        //        .Set(NICKNAME, entity.NickName)
        //        .Set(EMAIL, entity.Email)
        //        .Set(SCHOOL, entity.School)
        //        .Where(c => c.Equal(USERNAME, entity.UserName))
        //        .Result();
        //}

        /// <summary>
        /// 更新一条数据(只更新用户权限)
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="permission">权限类型</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntityPermision(String userName, PermissionType permision)
        {
            return this.Update()
                .Set(PERMISSION, (Int32)permision)
                .Where(c => c.Equal(USERNAME, userName))
                .Result();
        }

        /// <summary>
        /// 更新指定数据(只更新是否锁定)
        /// </summary>
        /// <param name="userNames">用户名列表</param>
        /// <param name="isLocked">是否锁定</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntityIsLocked(String userNames, Boolean isLocked)
        {
            return this.Update()
                .Set(ISLOCKED, isLocked)
                .Where(c => c.InString(USERNAME, userNames, ','))
                .Result();
        }

        /// <summary>
        /// 更新一条数据(只更新最近登陆信息)
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="lastip">最后登录IP</param>
        /// <param name="lastDate">最后登录时间</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntityLoginInfomation(String userName, String lastip, DateTime lastDate)
        {
            return this.Update()
                .Set(LASTIP, lastip)
                .Set(LASTDATE, lastDate)
                .Where(c => c.Equal(USERNAME, userName))
                .Result();
        }

        /// <summary>
        /// 更新一条数据(只更新用户密码)
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntityPassword(String userName, String passWord)
        {
            return this.Update()
                .Set(PASSWORD, passWord)
                .Where(c => c.Equal(USERNAME, userName))
                .Result();
        }

        /// <summary>
        /// 更新一条数据(只更新提交题目信息)
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntitySubmitCount(String userName)
        {
            return this.InternalGetUpdateSubmitCountCommand(userName)
                .Result();
        }

        /// <summary>
        /// 更新一条数据(只更新完成题目信息)
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntitySolvedCount(String userName)
        {
            return this.InternalGetUpdateSolvedCountCommand(userName)
                .Result();
        }
        #endregion

        #region Select
        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>对象实体</returns>
        public UserEntity GetEntityWithRank(String userName)
        {
            String table2 = "tb";

            //SELECT #COLUMN_NAMES#,
            //  (SELECT ISNULL(SUM(1), 0) + 1 
            //   FROM SDNUOJ_USER 
            //   WHERE USER_SOLVEDCOUNT > tb.USER_SOLVEDCOUNT OR (USER_SOLVEDCOUNT = tb.USER_SOLVEDCOUNT AND USER_SUBMITCOUNT < tb.USER_SUBMITCOUNT)
            //  ) AS USER_RANK
            //FROM SDNUOJ_USER tb 
            //WHERE USER_USERNAME=@PC_USER_USERNAME

            return this.Select(table2)
                .Querys(USERNAME, NICKNAME, EMAIL, SCHOOL, PERMISSION, ISLOCKED, CREATEDATE, SUBMITCOUNT, SOLVEDCOUNT)
                .Query(this.TableName, s =>
                {
                    s.Query(this.Functions.IsNull("SUM(1)", "0") + " + 1")
                        .Where(c => c.GreaterThanColumn(SOLVEDCOUNT, table2, SOLVEDCOUNT) | (c.EqualColumn(SOLVEDCOUNT, table2, SOLVEDCOUNT) & c.LessThanColumn(SUBMITCOUNT, table2, SUBMITCOUNT)));
                }, RANK)
                .Where(c => c.Equal(USERNAME, userName))
                .ToEntityWithArgs(this, true);
        }

        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>对象实体</returns>
        public UserEntity GetEntityWithBasicInfo(String userName)
        {
            return this.Select()
                .Querys(USERNAME, NICKNAME, EMAIL, SCHOOL)
                .Where(c => c.Equal(USERNAME, userName))
                .ToEntityWithArgs(this, true);
        }

        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>对象实体</returns>
        public UserEntity GetEntityWithAllInfo(String userName)
        {
            return this.Select()
                .Querys(USERNAME, PASSWORD, NICKNAME, EMAIL, SCHOOL, PERMISSION, SUBMITCOUNT, SOLVEDCOUNT, ISLOCKED, CREATEIP, CREATEDATE, LASTIP, LASTDATE)
                .Where(c => c.Equal(USERNAME, userName))
                .ToEntity(this);
        }

        /// <summary>
        /// 根据用户名和密码得到一个对象实体
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">MD5后的密码</param>
        /// <returns>若成功则得到一个对象实体，否则返回null</returns>
        public UserEntity GetEntityByNameAndPassword(String userName, String passWord)
        {
            return this.Select()
                .Querys(USERNAME, PASSWORD, PERMISSION, ISLOCKED)
                .Where(c => c.Equal(USERNAME, userName) & c.Equal(PASSWORD, passWord))
                .ToEntityWithArgs(this, true);
        }

        /// <summary>
        /// 根据用户名和电子邮箱得到一个对象实体
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="email">电子邮箱</param>
        /// <returns>若成功则得到一个对象实体，否则返回null</returns>
        public UserEntity GetEntityByNameAndEmail(String userName, String email)
        {
            return this.Select()
                .Querys(USERNAME, EMAIL, ISLOCKED)
                .Where(c => c.Equal(USERNAME, userName) & c.Equal(EMAIL, email))
                .ToEntityWithArgs(this, true);
        }

        /// <summary>
        /// 根据用户名获取用户的权限类型
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>权限类型</returns>
        public PermissionType GetPermissionByName(String userName)
        {
            return (PermissionType)this.Select()
                .Query(PERMISSION)
                .Where(c => c.Equal(USERNAME, userName))
                .Result();
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <returns>实体列表</returns>
        public List<UserEntity> GetEntities(Int32 pageIndex, Int32 pageSize, Int32 recordCount)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(USERNAME, NICKNAME, SOLVEDCOUNT, SUBMITCOUNT, CREATEDATE, ISLOCKED)
                .OrderByDesc(SOLVEDCOUNT)
                .OrderByAsc(SUBMITCOUNT)
                .OrderByDesc(CREATEDATE)
                .OrderByAsc(USERNAME)
                .ToEntityListWithArgs(this, true);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="names">用户名包含</param>
        /// <param name="nickname">昵称包含</param>
        /// <param name="email">邮箱包含</param>
        /// <param name="school">学校包含</param>
        /// <param name="lastIP">最后登录IP</param>
        /// <param name="islocked">是否锁定</param>
        /// <param name="regStartDate">注册日期开始</param>
        /// <param name="regEndDate">注册日期结束</param>
        /// <param name="loginStartDate">最后登录日期开始</param>
        /// <param name="loginEndDate">最后登录日期结束</param>
        /// <param name="order">排序顺序</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <returns>实体列表</returns>
        public List<UserEntity> GetEntities(String names, String nickname, String email, String school, String lastIP, Boolean? islocked, DateTime? regStartDate, DateTime? regEndDate, DateTime? loginStartDate, DateTime? loginEndDate, Int32 order, Int32 pageIndex, Int32 pageSize, Int32 recordCount)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(USERNAME, SOLVEDCOUNT, SUBMITCOUNT, LASTDATE, LASTIP, ISLOCKED, CREATEDATE)
                .Where(names, nickname, email, school, lastIP, islocked, regStartDate, regEndDate, loginStartDate, loginEndDate)
                .Then(it =>
                {
                    //排序顺序 order=0为按通过率排名 =1为按注册日期 =2为按最后登录日期 =3为按最后登录IP
                    if (order == 0) it.OrderByDesc(SOLVEDCOUNT).OrderByAsc(SUBMITCOUNT).OrderByDesc(CREATEDATE);
                    else if (order == 1) it.OrderByDesc(CREATEDATE);
                    else if (order == 2) it.OrderByDesc(LASTDATE);
                    else if (order == 3) it.OrderByAsc(LASTIP);
                    
                    it.OrderByAsc(USERNAME);
                })
                .ToEntityListWithArgs(this, true);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <returns>实体列表</returns>
        public List<UserEntity> GetEntitiesHavePermission(Int32 pageIndex, Int32 pageSize, Int32 recordCount)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(USERNAME, PERMISSION)
                .Where(c => c.GreaterThan(PERMISSION, (Int32)PermissionType.HttpJudge))
                .OrderByAsc(USERNAME)
                .ToEntityListWithArgs(this, true);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <returns>实体列表</returns>
        public List<UserEntity> GetEntitiesForJudgers()
        {
            return this.Select()
                .Querys(USERNAME, NICKNAME, LASTDATE, LASTIP)
                .Where(c => c.Equal(PERMISSION, (Int32)PermissionType.HttpJudge))
                .OrderByAsc(USERNAME)
                .ToEntityListWithArgs(this, true);
        }

        /// <summary>
        /// 获取首页的用户TOP排名
        /// </summary>
        /// <param name="dtStart">开始日期</param>
        /// <param name="dtEnd">结束日期</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns>首页的用户TOP排名列表</returns>
        public List<UserEntity> GetEntitiesForUserTopRanklist(DateTime dtStart, DateTime dtEnd, Int32 pageSize)
        {
            return SolutionRepository.Instance.InternalGetTopUserCommand(dtStart, dtEnd, pageSize)
                .ToEntityListWithArgs(this, true);
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
        /// <param name="names">用户名包含</param>
        /// <param name="nickname">昵称包含</param>
        /// <param name="email">邮箱包含</param>
        /// <param name="school">学校包含</param>
        /// <param name="lastIP">最后登录IP</param>
        /// <param name="islocked">是否锁定</param>
        /// <param name="regStartDate">注册日期开始</param>
        /// <param name="regEndDate">注册日期结束</param>
        /// <param name="loginStartDate">最后登录日期开始</param>
        /// <param name="loginEndDate">最后登录日期结束</param>
        /// <returns>实体总数</returns>
        public Int32 CountEntities(String names, String nickname, String email, String school, String lastIP, Boolean? islocked, DateTime? regStartDate, DateTime? regEndDate, DateTime? loginStartDate, DateTime? loginEndDate)
        {
            return this.Select()
                .Where(names, nickname, email, school, lastIP, islocked, regStartDate, regEndDate, loginStartDate, loginEndDate)
                .Count();
        }

        /// <summary>
        /// 获取实体总数
        /// </summary>
        /// <returns>实体总数</returns>
        public Int32 CountEntitiesHavePermission()
        {
            return this.Select()
                .Where(c => c.GreaterThan(PERMISSION, (Int32)PermissionType.HttpJudge))
                .Count();
        }

        /// <summary>
        /// 判断是否存在指定用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>是否存在指定用户</returns>
        public Boolean ExistsEntity(String userName)
        {
            return this.Select()
                .Where(c => c.Equal(USERNAME, userName))
                .Count() > 0;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 获取增加提交题目数的SQL语句
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>增加提交题目数的SQL语句</returns>
        internal UpdateCommand InternalGetIncreaseSubmitCountCommand(String userName)
        {
            return this.Update()
                .Increase(SUBMITCOUNT)
                .Where(c => c.Equal(USERNAME, userName));
        }

        /// <summary>
        /// 获取增加完成题目信息的SQL语句
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>增加完成题目信息的SQL语句</returns>
        internal UpdateCommand InternalGetIncreaseSolvedCountCommand(String userName)
        {
            return this.Update()
                 .Increase(SOLVEDCOUNT)
                 .Where(c => c.Equal(USERNAME, userName));
        }

        /// <summary>
        /// 获取更新完成题目信息的SQL语句
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>更新完成题目信息的SQL语句</returns>
        internal UpdateCommand InternalGetUpdateSolvedCountCommand(String userName)
        {
            return this.Update()
                .Set(() => this.Database.DatabaseType == DatabaseType.Access, SOLVEDCOUNT,
                    new DCountFunction(
                        ProblemRepository.PROBLEMID,
                        ProblemRepository.Instance.TableName,
                        String.Format("{0} IN (SELECT {1} FROM {2} WHERE {3} = '{4}' AND {5} = {6})",
                            ProblemRepository.PROBLEMID,
                            SolutionRepository.PROBLEMID,
                            SolutionRepository.Instance.TableName,
                            SolutionRepository.USERNAME, userName,
                            SolutionRepository.RESULT, ((Byte)ResultType.Accepted).ToString()
                        )))
                .Set(() => this.Database.DatabaseType != DatabaseType.Access, SOLVEDCOUNT,
                    SolutionRepository.Instance.TableName, s =>
                    {
                        s.Query(SqlAggregateFunction.Count, SolutionRepository.PROBLEMID, true)
                            .Where(c => c.Equal(SolutionRepository.USERNAME, userName) & c.Equal(SolutionRepository.RESULT, (Byte)ResultType.Accepted));
                    })
                .Where(c => c.Equal(USERNAME, userName));
        }

        /// <summary>
        /// 获取更新提交题目信息的SQL语句
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>更新提交题目信息的SQL语句</returns>
        internal UpdateCommand InternalGetUpdateSubmitCountCommand(String userName)
        {
            return this.Update()
                .Set(() => this.Database.DatabaseType == DatabaseType.Access, SUBMITCOUNT, 
                    new DCountFunction(
                        SolutionRepository.PROBLEMID, 
                        SolutionRepository.Instance.TableName,
                        String.Format("{0} = '{1}'", SolutionRepository.USERNAME, userName)))
                .Set(() => this.Database.DatabaseType != DatabaseType.Access, SUBMITCOUNT,
                    SolutionRepository.Instance.TableName, s =>
                    {
                        s.Query(SqlAggregateFunction.Count)
                            .Where(c => c.Equal(SolutionRepository.USERNAME, userName));
                    })
                .Where(c => c.Equal(USERNAME, userName));
        }
        #endregion
    }
}

namespace SDNUOJ.Data.UserDataProviderExtensions
{
    #region UserDataProviderExtension
    internal static class UserDataProviderExtension
    {
        /// <summary>
        /// 设置指定的查询语句并返回当前语句
        /// </summary>
        /// <param name="command">选择语句</param>
        /// <param name="names">用户名包含</param>
        /// <param name="nickname">昵称包含</param>
        /// <param name="email">邮箱包含</param>
        /// <param name="school">学校包含</param>
        /// <param name="lastIP">最后登录IP</param>
        /// <param name="islocked">是否锁定</param>
        /// <param name="regStartDate">注册日期开始</param>
        /// <param name="regEndDate">注册日期结束</param>
        /// <param name="loginStartDate">最后登录日期开始</param>
        /// <param name="loginEndDate">最后登录日期结束</param>
        /// <returns>当前语句</returns>
        internal static SelectCommand Where(this SelectCommand command, String names, String nickname, String email, String school, String lastIP, Boolean? islocked, DateTime? regStartDate, DateTime? regEndDate, DateTime? loginStartDate, DateTime? loginEndDate)
        {
            SqlConditionBuilder c = command.ConditionBuilder;
            AbstractSqlCondition condition = command.WhereCondition as AbstractSqlCondition;
            AbstractSqlCondition temp = null;

            //用户名列表模糊
            if (!String.IsNullOrEmpty(names))
            {
                String[] arrnames = names.Split(',');
                AbstractSqlCondition innertemp = null;
                temp = null;

                for (Int32 i = 0; i < arrnames.Length; i++)
                {
                    if (!String.IsNullOrEmpty(arrnames[i]))
                    {
                        innertemp = c.LikeAll(UserRepository.USERNAME, arrnames[i]);
                        temp = (temp == null ? innertemp : innertemp | temp);
                    }
                }

                condition = (condition == null ? temp : condition & temp);
            }

            //昵称模糊
            if (!String.IsNullOrEmpty(nickname))
            {
                temp = c.LikeAll(UserRepository.NICKNAME, nickname);
                condition = (condition == null ? temp : condition & temp);
            }

            //邮箱模糊
            if (!String.IsNullOrEmpty(email))
            {
                temp = c.LikeAll(UserRepository.EMAIL, email);
                condition = (condition == null ? temp : condition & temp);
            }

            //学校模糊
            if (!String.IsNullOrEmpty(school))
            {
                temp = c.LikeAll(UserRepository.SCHOOL, school);
                condition = (condition == null ? temp : condition & temp);
            }

            //最后登录IP模糊
            if (!String.IsNullOrEmpty(lastIP))
            {
                temp = c.LikeAll(UserRepository.LASTIP, lastIP);
                condition = (condition == null ? temp : condition & temp);
            }

            //是否锁定
            if (islocked.HasValue)
            {
                temp = c.Equal(UserRepository.ISLOCKED, islocked.Value);
                condition = (condition == null ? temp : condition & temp);
            }

            //注册日期范围
            if (regStartDate.HasValue || regEndDate.HasValue)
            {
                temp = c.BetweenNullable<DateTime>(UserRepository.CREATEDATE, regStartDate, regEndDate);
                condition = (condition == null ? temp : condition & temp);
            }

            //登陆日期范围
            if (loginStartDate.HasValue || loginEndDate.HasValue)
            {
                temp = c.BetweenNullable<DateTime>(UserRepository.LASTDATE, loginStartDate, loginEndDate);
                condition = (condition == null ? temp : condition & temp);
            }

            return command.Where(condition);
        }
    }
    #endregion
}