using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 专题页面实体类
    /// </summary>
    [Serializable]
    public class TopicPageEntity
    {
        /// <summary>
        /// 获取或设置专题页面名称
        /// </summary>
        public String PageName { get; set; }

        /// <summary>
        /// 获取或设置专题页面标题
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// 获取或设置专题页面描述
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// 获取或设置专题页面内容
        /// </summary>
        public String Content { get; set; }

        /// <summary>
        /// 获取或设置最后更新日期
        /// </summary>
        public DateTime LastDate { get; set; }

        /// <summary>
        /// 获取或设置创建用户名
        /// </summary>
        public String CreateUser { get; set; }

        /// <summary>
        /// 获取或设置是否隐藏
        /// </summary>
        public Boolean IsHide { get; set; }
    }
}