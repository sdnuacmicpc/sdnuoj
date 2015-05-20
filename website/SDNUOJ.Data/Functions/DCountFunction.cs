using System;

using DotMaysWind.Data;
using DotMaysWind.Data.Command;

namespace SDNUOJ.Data.Functions
{
    public class DCountFunction : ISqlFunction
    {
        #region 字段
        private String _expr;
        private String _domain;
        private String _criteria;
        #endregion

        #region 属性
        /// <summary>
        /// 获取是否需要提交参数
        /// </summary>
        public Boolean HasParameters
        {
            get { return false; }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 初始化新的DCount函数
        /// </summary>
        /// <param name="expr">表达式，用于标识将统计其记录数的字段</param>
        /// <param name="domain">字符串表达式，代表组成域的记录集</param>
        /// <param name="criteria">可选的字符串表达式，用于限制 DCount 函数执行的数据范围</param>
        public DCountFunction(String expr, String domain, String criteria)
        {
            this._expr = expr;
            this._domain = domain;
            this._criteria = criteria;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获取查询函数参数集合
        /// </summary>
        /// <returns>查询函数参数集合</returns>
        public DataParameter[] GetAllParameters()
        {
            return null;
        }

        /// <summary>
        /// 获取函数拼接后字符串
        /// </summary>
        /// <returns>函数拼接后字符串</returns>
        public String GetCommandText()
        {
            return String.Format("DCount(\"{0}\", \"{1}\", \"{2}\")", this._expr, this._domain, this._criteria);
        }
        #endregion
    }
}