using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 用户实体类
    /// </summary>
    [Serializable]
    public class UserEntity
    {
        /// <summary>
        /// 获取或设置用户名
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// 获取或设置密码
        /// </summary>
        public String PassWord { get; set; }

        /// <summary>
        /// 获取或设置用户昵称
        /// </summary>
        public String NickName { get; set; }

        /// <summary>
        /// 获取或设置电子邮箱地址
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// 获取或设置学校
        /// </summary>
        public String School { get; set; }

        /// <summary>
        /// 获取或设置用户权限
        /// </summary>
        public PermissionType Permission { get; set; }

        /// <summary>
        /// 获取或设置题目提交数
        /// </summary>
        public Int32 SubmitCount { get; set; }

        /// <summary>
        /// 获取或设置题目通过数
        /// </summary>
        public Int32 SolvedCount { get; set; }

        /// <summary>
        /// 获取或设置是否锁定账户
        /// </summary>
        public Boolean IsLocked { get; set; }

        /// <summary>
        /// 获取或设置注册IP地址
        /// </summary>
        public String CreateIP { get; set; }

        /// <summary>
        /// 获取或设置注册时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 获取或设置最后登录IP
        /// </summary>
        public String LastIP { get; set; }

        /// <summary>
        /// 获取或设置最后登录时间
        /// </summary>
        public DateTime LastDate { get; set; }

        /// <summary>
        /// 获取或设置用户排名
        /// </summary>
        public virtual Double Rank { get; set; }

        /// <summary>
        /// 获取或设置最后在线时间
        /// </summary>
        public virtual DateTime? LastOnline { get; set; }

        /// <summary>
        /// 获取AC比率
        /// </summary>
        public virtual Double Ratio
        {
            get { return 100 * (this.SubmitCount > 0 ? (Double)this.SolvedCount / (Double)this.SubmitCount : 0); }
        }
    }
}