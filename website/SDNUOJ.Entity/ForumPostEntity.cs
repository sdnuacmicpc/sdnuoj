using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 论坛帖子实体类
    /// </summary>
    [Serializable]
    public class ForumPostEntity
    {
        /// <summary>
        /// 获取或设置帖子ID
        /// </summary>
        public Int32 PostID { get; set; }

        /// <summary>
        /// 获取或设置主题ID
        /// </summary>
        public Int32 TopicID { get; set; }

        /// <summary>
        /// 获取或设置帖子父节点
        /// </summary>
        public Int32 ParentPostID { get; set; }

        /// <summary>
        /// 获取或设置帖子深度
        /// </summary>
        public Int32 Deepth { get; set; }

        /// <summary>
        /// 获取或设置发布用户名
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// 获取或设置帖子标题
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// 获取或设置帖子内容
        /// </summary>
        public String Content { get; set; }

        /// <summary>
        /// 获取或设置帖子长度
        /// </summary>
        public Int32 ContentLength { get; set; }

        /// <summary>
        /// 获取或设置是否隐藏帖子
        /// </summary>
        public Boolean IsHide { get; set; }

        /// <summary>
        /// 获取或设置帖子发布日期
        /// </summary>
        public DateTime PostDate { get; set; }

        /// <summary>
        /// 获取或设置帖子发布IP
        /// </summary>
        public String PostIP { get; set; }

        /// <summary>
        /// 获取或设置关联题目/竞赛类型
        /// </summary>
        public ForumTopicType RelativeType { get; set; }

        /// <summary>
        /// 获取或设置关联题目/竞赛ID
        /// </summary>
        public Int32 RelativeID { get; set; }
    }
}