using System;

namespace SDNUOJ.Entity.Complex
{
    /// <summary>
    /// 题目语言统计信息实体类
    /// </summary>
    [Serializable]
    public class LanguageStatistic
    {
        /// <summary>
        /// 获取或设置题目ID
        /// </summary>
        public Int32 ProblemID { get; set; }

        /// <summary>
        /// 获取或设置语言ID
        /// </summary>
        public Byte LanguageID { get; set; }

        /// <summary>
        /// 获取或设置数量
        /// </summary>
        public Int32 Count { get; set; }
    }
}