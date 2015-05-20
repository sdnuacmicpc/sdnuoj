using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 题目实体类
    /// </summary>
    [Serializable]
    public class ProblemEntity
    {
        /// <summary>
        /// 获取或设置题目ID
        /// </summary>
        public Int32 ProblemID { get; set; }

        /// <summary>
        /// 获取或设置题目标题
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// 获取或设置题目描述
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// 获取或设置输入描述
        /// </summary>
        public String Input { get; set; }

        /// <summary>
        /// 获取或设置输出描述
        /// </summary>
        public String Output { get; set; }

        /// <summary>
        /// 获取或设置样例输入
        /// </summary>
        public String SampleInput { get; set; }

        /// <summary>
        /// 获取或设置样例输出
        /// </summary>
        public String SampleOutput { get; set; }

        /// <summary>
        /// 获取或设置题目提示
        /// </summary>
        public String Hint { get; set; }

        /// <summary>
        /// 获取或设置题目来源
        /// </summary>
        public String Source { get; set; }

        /// <summary>
        /// 获取或设置时间限制(MS)
        /// </summary>
        public Int32 TimeLimit { get; set; }

        /// <summary>
        /// 获取或设置内存限制(KB)
        /// </summary>
        public Int32 MemoryLimit { get; set; }

        /// <summary>
        /// 获取或设置用户提交数
        /// </summary>
        public Int32 SubmitCount { get; set; }

        /// <summary>
        /// 获取或设置用户通过数
        /// </summary>
        public Int32 SolvedCount { get; set; }

        /// <summary>
        /// 获取或设置最后更新日期
        /// </summary>
        public DateTime LastDate { get; set; }

        /// <summary>
        /// 获取或设置是否隐藏
        /// </summary>
        public Boolean IsHide { get; set; }

        /// <summary>
        /// 获取AC比率
        /// </summary>
        public virtual Double Ratio
        {
            get { return 100 * (this.SubmitCount > 0 ? (Double)this.SolvedCount / (Double)this.SubmitCount : 0); }
        }
    }
}