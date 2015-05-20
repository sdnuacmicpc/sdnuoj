using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 竞赛用户实体类
    /// </summary>
    [Serializable]
    public class ContestUserEntity
    {
        /// <summary>
        /// 获取或设置竞赛ID
        /// </summary>
        public Int32 ContestID { get; set; }

        /// <summary>
        /// 获取或设置用户名称
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// 获取或设置真实姓名
        /// </summary>
        public String RealName { get; set; }

        /// <summary>
        /// 获取或设置注册时间
        /// </summary>
        public DateTime RegisterTime { get; set; }

        /// <summary>
        /// 获取或设置是否启用该用户
        /// </summary>
        public Boolean IsEnable { get; set; }
    }
}