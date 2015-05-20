using System;
using System.Collections.Generic;

using DotMaysWind.Data.Orm;

using SDNUOJ.Entity;

namespace SDNUOJ.Data
{
    /// <summary>
    /// 用户邮件表格操作类
    /// </summary>
    public class UserMailRepository : AbstractDatabaseTable<UserMailEntity>
    {
        #region Instance
        private static UserMailRepository _instance;

        public static UserMailRepository Instance
        {
            get { return _instance; }
        }

        static UserMailRepository()
        {
            _instance = new UserMailRepository();
        }
        #endregion

        #region Const
        internal const String MAILID = "MAIL_MAILID";
        internal const String FROMUSERNAME = "MAIL_FROMUSERNAME";
        internal const String TOUSERNAME = "MAIL_TOUSERNAME";
        internal const String TITLE = "MAIL_TITLE";
        internal const String CONTENT = "MAIL_CONTENT";
        internal const String SENDDATE = "MAIL_SENDDATE";
        internal const String ISREAD = "MAIL_ISREAD";
        internal const String ISDELETED = "MAIL_ISDELETED";

        public const Int32 TITLE_MAXLEN = 100;
        public const Int32 CONTENT_MINLEN = 2;
        public const Int32 CONTENT_MAXLEN = 32767;
        #endregion

        #region 重载属性和方法
        public override String TableName
        {
            get { return "SDNUOJ_USERMAIL"; }
        }

        private UserMailRepository() : base(MainDatabase.Instance) { }

        protected override UserMailEntity CreateEntity(Object sender, EntityCreatingArgs args)
        {
            UserMailEntity entity = new UserMailEntity();

            entity.MailID = this.LoadInt32(args, MAILID);
            entity.FromUserName = this.LoadString(args, FROMUSERNAME);
            entity.ToUserName = this.LoadString(args, TOUSERNAME);
            entity.Title = this.LoadString(args, TITLE);
            entity.Content = this.LoadString(args, CONTENT);
            entity.SendDate = this.LoadDateTime(args, SENDDATE);
            entity.IsRead = this.LoadBoolean(args, ISREAD);
            entity.IsDeleted = this.LoadBoolean(args, ISDELETED);

            return entity;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 InsertEntity(UserMailEntity entity)
        {
            return this.Insert()
                .Set(FROMUSERNAME, entity.FromUserName)
                .Set(TOUSERNAME, entity.ToUserName)
                .Set(TITLE, entity.Title)
                .Set(CONTENT, entity.Content)
                .Set(SENDDATE, entity.SendDate)
                .Set(ISREAD, entity.IsRead)
                .Set(ISDELETED, entity.IsDeleted)
                .Result();
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新一条数据(用户邮件状态)
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 UpdateEntityIsRead(UserMailEntity entity)
        {
            return this.Update()
                .Set(ISREAD, entity.IsRead)
                .Where(c => c.Equal(MAILID, entity.MailID))
                .Result();
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="ids">逗号分隔的实体ID</param>
        /// <returns>操作影响的记录数</returns>
        public Int32 DeleteEntities(String userName, String ids)
        {
            return this.Update()
                .Set(ISDELETED, true)
                .Where(c => c.Equal(TOUSERNAME, userName) & c.InInt32(MAILID, ids, ','))
                .Result();
        }
        #endregion

        #region Select
        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="mailID">实体ID</param>
        /// <returns>对象实体</returns>
        public UserMailEntity GetEntity(String userName, Int32 mailID)
        {
            return this.Select()
                .Querys(MAILID, FROMUSERNAME, TOUSERNAME, TITLE, CONTENT, SENDDATE, ISREAD, ISDELETED)
                .Where(c => c.Equal(ISDELETED, false) & c.Equal(TOUSERNAME, userName) & c.Equal(MAILID, mailID))
                .ToEntity(this);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录总数</param>
        /// <returns>实体列表</returns>
        public List<UserMailEntity> GetEntities(String userName, Int32 pageIndex, Int32 pageSize, Int32 recordCount)
        {
            return this.Select()
                .Paged(pageSize, pageIndex, recordCount)
                .Querys(MAILID, FROMUSERNAME, TOUSERNAME, TITLE, SENDDATE, ISREAD, ISDELETED)
                .Where(c => c.Equal(ISDELETED, false) & c.Equal(TOUSERNAME, userName))
                .OrderByDesc(MAILID)
                .ToEntityList(this);
        }
        #endregion

        #region Count/Exists
        /// <summary>
        /// 获取邮件总数
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>邮件总数</returns>
        public Int32 CountEntities(String userName)
        {
            return this.Select()
                .Where(c => c.Equal(ISDELETED, false) & c.Equal(TOUSERNAME, userName))
                .Count();
        }

        /// <summary>
        /// 获取未读邮件总数
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>未读邮件总数</returns>
        public Int32 CountUnReadEntities(String userName)
        {
            return this.Select()
                .Where(c => c.Equal(ISDELETED, false) & c.Equal(TOUSERNAME, userName) & c.Equal(ISREAD, false))
                .Count();
        }
        #endregion
    }
}