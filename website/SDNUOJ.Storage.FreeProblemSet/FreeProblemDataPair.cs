using System;

namespace SDNUOJ.Storage.FreeProblemSet
{
    /// <summary>
    /// Free Problem Data Pair
    /// </summary>
    public class FreeProblemDataPair
    {
        #region 字段
        private String _input;
        private String _output;
        #endregion

        #region 属性
        /// <summary>
        /// 获取题目输入数据
        /// </summary>
        public String Input
        {
            get { return this._input; }
        }

        /// <summary>
        /// 获取题目输出数据
        /// </summary>
        public String Output
        {
            get { return this._output; }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 创建新的题目数据类
        /// </summary>
        /// <param name="input">输入数据</param>
        /// <param name="output">输出数据</param>
        public FreeProblemDataPair(String input, String output)
        {
            this._input = input;
            this._output = output;
        }
        #endregion
    }
}