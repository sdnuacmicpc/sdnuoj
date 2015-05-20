using System;

using DotMaysWind.Data.Orm;

using SDNUOJ.Entity;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 用户密码找回表格操作类
    /// </summary>
    public class UserForgetPasswordRepository: AbstractDatabaseTable<UserForgetPasswordEntity>
    {
        #region Instance
        private static UserForgetPasswordRepository _instance;

        public static UserForgetPasswordRepository Instance
        {
            get { return _instance; }
        }

        static UserForgetPasswordRepository()
        {
            _instance = new UserForgetPasswordRepository();
        }
        #endregion

        #region Const
        internal const String USERNAME = "FRGT_USERNAME";
        internal const String HASHKEY = "FRGT_HASHKEY";
        internal const String SUBMITDATE = "FRGT_SUBMITDATE";
        internal const String SUBMITIP = "FRGT_SUBMITIP";
        internal const String ACCESSDATE = "FRGT_ACCESSDATE";
        internal const String ACCESSIP = "FRGT_ACCESSIP";
        #endregion

        #region 重载属性和方法
        public override String TableName
        {
            get { return "SDNUOJ_USERFORGETPASS"; }
        }

        private UserForgetPasswordRepository() : base(MainDatabase.Instance) { }

        protected override UserForgetPasswordEntity CreateEntity(Object sender, EntityCreatingArgs args)
        {
            UserForgetPasswordEntity entity = new UserForgetPasswordEntity();

            entity.UserName = this.LoadString(args, USERNAME);
            entity.HashKey = this.LoadString(args, HASHKEY);
            entity.SubmitDate = this.LoadDateTime(args, SUBMITDATE);
            entity.SubmitIP = this.LoadString(args, SUBMITIP);
            entity.AccessDate = this.LoadDateTime(args, ACCESSDATE);
            entity.AccessIP = this.LoadString(args, ACCESSIP);

            return entity;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 InsertEntity(UserForgetPasswordEntity entity)
        {
            return this.Insert()
                .Set(USERNAME, entity.UserName)
                .Set(HASHKEY, entity.HashKey)
                .Set(SUBMITDATE, entity.SubmitDate)
                .Set(SUBMITIP, entity.SubmitIP)
                .Result();
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新一条数据(更新找回时间)
        /// </summary>
        /// <param name="hashKey">哈希KEY</param>
        /// <param name="accessDate">找回时间</param>
        /// <param name="accessIP">找回IP</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntityStatus(String hashKey, DateTime accessDate, String accessIP)
        {
            return this.Update()
                .Set(ACCESSDATE, accessDate)
                .Set(ACCESSIP, accessIP)
                .Where(c => c.Equal(HASHKEY, hashKey))
                .Result();
        }
        #endregion

        #region Select
        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="hashKey">哈希KEY</param>
        /// <returns>对象实体</returns>
        public UserForgetPasswordEntity GetEntity(String hashKey)
        {
            return this.Select()
                .Querys(USERNAME, HASHKEY, SUBMITDATE, USERNAME, SUBMITIP, ACCESSDATE, ACCESSIP)
                .Where(c => c.Equal(HASHKEY, hashKey))
                .ToEntity(this);
        }
        #endregion
    }
}