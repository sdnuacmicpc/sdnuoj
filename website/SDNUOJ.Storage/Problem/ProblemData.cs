using System;
using System.Collections.Generic;

namespace SDNUOJ.Storage.Problem
{
    /// <summary>
    /// 题目数据类
    /// </summary>
    public class ProblemData
    {
        #region 常量
        internal const String NO_PROBLEM_DATA_VERSION = "0001-01-01 00:00:00";
        internal const String DATA_VERSION_FILENAME = "last_modified";
        internal const String INPUT_DATA_DIRECTORY = "input";
        internal const String OUTPUT_DATA_DIRECTORY = "output";
        #endregion

        #region 字段
        private String _lastModified;
        private List<ProblemDataPair> _datas;
        #endregion

        #region 属性
        /// <summary>
        /// 获取最后更新日期
        /// </summary>
        public String LastModified
        {
            get { return this._lastModified; }
        }

        /// <summary>
        /// 获取题目数据个数
        /// </summary>
        public Int32 DataCount
        {
            get { return (this._datas != null ? this._datas.Count : 0); }
        }

        /// <summary>
        /// 获取指定题目数据
        /// </summary>
        /// <param name="index">题目数据索引</param>
        /// <returns>指定题目数据</returns>
        public ProblemDataPair this[Int32 index]
        {
            get { return (this._datas != null ? this._datas[index] : null); }
        }
        #endregion

        #region 构造方法
        public ProblemData(String lastMod)
        {
            this._lastModified = lastMod;
            this._datas = new List<ProblemDataPair>();
        }

        public ProblemData() : this(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) { }
        #endregion

        #region 方法
        /// <summary>
        /// 添加题目数据对
        /// </summary>
        /// <param name="pair">题目数据对</param>
        public void AddData(ProblemDataPair pair)
        {
            this._datas.Add(pair);
        }
        #endregion
    }
}