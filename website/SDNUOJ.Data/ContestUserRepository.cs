using System;
using System.Collections.Generic;

using DotMaysWind.Data;
using DotMaysWind.Data.Command;
using DotMaysWind.Data.Orm;

using SDNUOJ.Entity;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 竞赛用户表格操作类
    /// </summary>
    public class ContestUserRepository : AbstractDatabaseTable<ContestUserEntity>
    {
        #region Instance
        private static ContestUserRepository _instance;

        public static ContestUserRepository Instance
        {
            get { return _instance; }
        }

        static ContestUserRepository()
        {
            _instance = new ContestUserRepository();
        }
        #endregion

        #region Const
        internal const String CONTESTID = "CTUR_CONTESTID";
        internal const String USERNAME = "CTUR_USERNAME";
        internal const String REALNAME = "CTUR_REALNAME";
        internal const String REGISTERTIME = "CTUR_REGISTERTIME";
        internal const String ISENABLE = "CTUR_ISENABLE";

        public const Int32 REALNAME_MAXLEN = 255;
        #endregion

        #region 重载属性和方法
        public override String TableName
        {
            get { return "SDNUOJ_CONTESTUSER"; }
        }

        private ContestUserRepository() : base(MainDatabase.Instance) { }

        protected override ContestUserEntity CreateEntity(Object sender, EntityCreatingArgs args)
        {
            ContestUserEntity entity = new ContestUserEntity();

            entity.ContestID = this.LoadInt32(args, CONTESTID);
            entity.UserName = this.LoadString(args, USERNAME);
            entity.RealName = this.LoadString(args, REALNAME);
            entity.RegisterTime = this.LoadDateTime(args, REGISTERTIME);
            entity.IsEnable = this.LoadBoolean(args, ISENABLE);

            return entity;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 InsertEntity(ContestUserEntity entity)
        {
            return this.Insert()
                .Set(CONTESTID, entity.ContestID)
                .Set(USERNAME, entity.UserName)
                .Set(REALNAME, entity.RealName)
                .Set(REGISTERTIME, entity.RegisterTime)
                .Set(ISENABLE, entity.IsEnable)
                .Result();
        }

        /// <summary>
        /// 增加多条数据
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="usernames">逗号分隔的用户名</param>
        /// <param name="entities">对象实体列表</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 InsertEntities(Int32 cid, String usernames, Dictionary<String, ContestUserEntity> entities)
        {
            return this.UsingTransaction<Int32>(trans => 
            {
                this.Delete()
                    .Where(c => c.Equal(CONTESTID, cid) & c.InString(USERNAME, usernames, ','))
                    .Result(trans);

                Int32 result = this.Sequence()
                    .AddSome(entities.Values, item =>
                    {
                        return this.Insert()
                            .Set(CONTESTID, item.Value.ContestID)
                            .Set(USERNAME, item.Value.UserName)
                            .Set(REALNAME, item.Value.RealName)
                            .Set(REGISTERTIME, item.Value.RegisterTime)
                            .Set(ISENABLE, item.Value.IsEnable);
                    })
                    .Result(trans);

                trans.Commit();

                return result;
            });
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新指定数据(只更新是否启用)
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="usernames">逗号分隔的用户名</param>
        /// <param name="isEnabled">是否启用</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntityIsEnabled(Int32 cid, String usernames, Boolean isEnabled)
        {
            return this.Update()
                .Set(ISENABLE, isEnabled)
                .Where(c => c.Equal(CONTESTID, cid) & c.InString(USERNAME, usernames, ','))
                .Result();
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="usernames">逗号分隔的用户名</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 DeleteEntities(Int32 cid, String usernames)
        {
            return this.Delete()
                .Where(c => c.Equal(CONTESTID, cid) & c.InString(USERNAME, usernames, ','))
                .Result();
        }
        #endregion

        #region Select
        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>实体列表</returns>
        public Dictionary<String, ContestUserEntity> GetEntities(Int32 cid)
        {
            return this.Select()
                .Querys(CONTESTID, USERNAME, REALNAME, REGISTERTIME, ISENABLE)
                .Where(c => c.Equal(CONTESTID, cid))
                .OrderByAsc(USERNAME)
                .ToEntityDictionary<String, ContestUserEntity>(this, USERNAME);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <returns>实体列表</returns>
        public List<ContestUserEntity> GetEntities(Int32 cid, Int32 pageIndex, Int32 pageSize, Int32 recordCount)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(CONTESTID, USERNAME, REALNAME, REGISTERTIME, ISENABLE)
                .Where(c => c.Equal(CONTESTID, cid))
                .OrderByDesc(REGISTERTIME)
                .OrderByAsc(USERNAME)
                .ToEntityList(this);
        }
        #endregion

        #region Count/Exists
        /// <summary>
        /// 获取实体总数
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>实体总数</returns>
        public Int32 CountEntities(Int32 cid)
        {
            return this.Select()
                .Where(c => c.Equal(CONTESTID, cid))
                .Count();
        }

        /// <summary>
        /// 判断是否存在指定记录
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="userName">用户名</param>
        /// <returns>是否存在指定记录</returns>
        public Boolean ExistsEntity(Int32 cid, String userName)
        {
            return this.Select()
                .Where(c => c.Equal(CONTESTID, cid) & c.Equal(USERNAME, userName))
                .Count() > 0;
        }
        #endregion
    }
}