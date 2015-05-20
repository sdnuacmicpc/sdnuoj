using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 公告实体类
    /// </summary>
    [Serializable]
    public class NewsEntity
    {
        /// <summary>
        /// 获取或设置公告ID
        /// </summary>
        public Int32 AnnounceID { get; set; }

        /// <summary>
        /// 获取或设置公告标题
        /// </summary>
        public String Title { get; set; }
        
        /// <summary>
        /// 获取或设置公告描述
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// 获取或设置公告日期
        /// </summary>
        public DateTime PublishDate { get; set; }
    }
}