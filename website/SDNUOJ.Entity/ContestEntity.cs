using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 竞赛实体类
    /// </summary>
    [Serializable]
    public class ContestEntity
    {
        /// <summary>
        /// 获取或设置竞赛ID
        /// </summary>
        public Int32 ContestID { get; set; }

        /// <summary>
        /// 获取或设置竞赛标题
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// 获取或设置竞赛描述
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// 获取或设置竞赛类型
        /// </summary>
        public ContestType ContestType { get; set; }

        /// <summary>
        /// 获取或设置竞赛开始日期
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 获取或设置竞赛结束日期
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 获取或设置竞赛注册开始日期
        /// </summary>
        public DateTime? RegisterStartTime { get; set; }

        /// <summary>
        /// 获取或设置竞赛注册结束日期
        /// </summary>
        public DateTime? RegisterEndTime { get; set; }

        /// <summary>
        /// 获取或设置最后更新日期
        /// </summary>
        public DateTime LastDate { get; set; }

        /// <summary>
        /// 获取或设置支持语言
        /// </summary>
        public String SupportLanguage { get; set; }

        /// <summary>
        /// 获取或设置是否隐藏
        /// </summary>
        public Boolean IsHide { get; set; }

        /// <summary>
        /// 获取竞赛类型名称
        /// </summary>
        public String ContestTypeString
        {
            get
            {
                switch (this.ContestType)
                {
                    case ContestType.Private:
                        return "Private";
                    case ContestType.Public:
                        return "Public";
                    case ContestType.RegisterPrivate:
                        return "Register Private";
                    case ContestType.RegisterPublic:
                        return "Register Public";
                    default:
                        return String.Empty;
                }
            }
        }

        /// <summary>
        /// 获取竞赛状态
        /// </summary>
        public ContestStatus ContestStatus
        {
            get
            {
                if (this.ContestType == ContestType.RegisterPrivate || this.ContestType == ContestType.RegisterPublic)
                {
                    if (DateTime.Now >= this.RegisterStartTime && DateTime.Now <= this.RegisterEndTime)
                    {
                        return ContestStatus.Registering;
                    }
                }

                if (DateTime.Now < this.StartTime)
                {
                    return ContestStatus.Pending;
                }

                if (DateTime.Now >= this.StartTime && DateTime.Now <= this.EndTime)
                {
                    return ContestStatus.Running;
                }

                return ContestStatus.Ended;
            }
        }
    }
}