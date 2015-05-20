using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 题目类型种类实体类
    /// </summary>
    [Serializable]
    public class ProblemCategoryEntity
    {
        /// <summary>
        /// 获取或设置题目类型种类ID
        /// </summary>
        public Int32 TypeID { get; set; }

        /// <summary>
        /// 获取或设置题目类型种类名称
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// 获取或设置显示顺序
        /// </summary>
        public Int32 Order { get; set; }
    }
}