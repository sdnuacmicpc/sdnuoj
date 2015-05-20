using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 题目类型对实体类
    /// </summary>
    [Serializable]
    public class ProblemCategoryItemEntity
    {
        /// <summary>
        /// 获取或设置题目类型对ID
        /// </summary>
        public Int32 PTID { get; set; }

        /// <summary>
        /// 获取或设置题目类型种类ID
        /// </summary>
        public Int32 TypeID { get; set; }

        /// <summary>
        /// 获取或设置题目ID
        /// </summary>
        public Int32 ProblemID { get; set; }
    }
}