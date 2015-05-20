using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 用户邮件实体类
    /// </summary>
    [Serializable]
    public class UserMailEntity
    {
        /// <summary>
        /// 获取或设置邮件ID
        /// </summary>
        public Int32 MailID { get; set; }

        /// <summary>
        /// 获取或设置来源用户名
        /// </summary>
        public String FromUserName { get; set; }

        /// <summary>
        /// 获取或设置去向用户名
        /// </summary>
        public String ToUserName { get; set; }

        /// <summary>
        /// 获取或设置邮件标题
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// 获取或设置邮件内容
        /// </summary>
        public String Content { get; set; }

        /// <summary>
        /// 获取或设置邮件发送日期
        /// </summary>
        public DateTime SendDate { get; set; }

        /// <summary>
        /// 获取或设置是否已读
        /// </summary>
        public Boolean IsRead { get; set; }

        /// <summary>
        /// 获取或设置是否已删除
        /// </summary>
        public Boolean IsDeleted { get; set; }
    }
}