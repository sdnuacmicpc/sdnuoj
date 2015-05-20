using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 资源实体类
    /// </summary>
    [Serializable]
    public class ResourceEntity
    {
        /// <summary>
        /// 获取或设置资源ID
        /// </summary>
        public Int32 ResourceID { get; set; }

        /// <summary>
        /// 获取或设置资源标题
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// 获取或设置资源URL
        /// </summary>
        public String Url { get; set; }

        /// <summary>
        /// 获取或设置资源类型
        /// </summary>
        public String Type { get; set; }
    }
}