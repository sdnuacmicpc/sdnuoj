using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 用户密码找回实体类
    /// </summary>
    [Serializable]
    public class UserForgetPasswordEntity
    {
        /// <summary>
        /// 获取或设置找回密码的用户名
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// 获取或设置哈希值
        /// </summary>
        public String HashKey { get; set; }

        /// <summary>
        /// 获取或设置申请找回的日期
        /// </summary>
        public DateTime SubmitDate { get; set; }

        /// <summary>
        /// 获取或设置申请找回的IP
        /// </summary>
        public String SubmitIP { get; set; }

        /// <summary>
        /// 获取或设置找回的日期
        /// </summary>
        public DateTime AccessDate { get; set; }

        /// <summary>
        /// 获取或设置找回的IP
        /// </summary>
        public String AccessIP { get; set; }
    }
}