using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 论坛主题实体类
    /// </summary>
    [Serializable]
    public class ForumTopicEntity
    {
        /// <summary>
        /// 获取或设置主题ID
        /// </summary>
        public Int32 TopicID { get; set; }

        /// <summary>
        /// 获取或设置发布用户名
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// 获取或设置主题标题
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// 获取或设置主题类型
        /// </summary>
        public ForumTopicType Type { get; set; }

        /// <summary>
        /// 获取或设置是否锁定主题
        /// </summary>
        public Boolean IsLocked { get; set; }

        /// <summary>
        /// 获取或设置是否隐藏主题
        /// </summary>
        public Boolean IsHide { get; set; }

        /// <summary>
        /// 获取或设置关联题目/比赛ID
        /// </summary>
        public Int32 RelativeID { get; set; }

        /// <summary>
        /// 获取或设置主题的最后帖子日期
        /// </summary>
        public DateTime LastDate { get; set; }
    }
}