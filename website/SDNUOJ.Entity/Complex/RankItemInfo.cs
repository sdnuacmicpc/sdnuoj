using System;

namespace SDNUOJ.Entity.Complex
{
    /// <summary>
    /// 排名信息类
    /// </summary>
    public class RankItemInfo
    {
        #region 字段
        private String _userName;
        private Int32 _problemID;
        private RankItemInfoType _type;
        private TimeSpan _penalty;
        private Int32 _wrongCount;
        #endregion

        #region 属性
        /// <summary>
        /// 获取用户名
        /// </summary>
        public String UserName
        {
            get { return this._userName; }
        }

        /// <summary>
        /// 获取题目ID
        /// </summary>
        public Int32 ProblemID
        {
            get { return this._problemID; }
        }

        /// <summary>
        /// 获取排名信息类型
        /// </summary>
        public RankItemInfoType Type
        {
            get { return this._type; }
        }

        /// <summary>
        /// 获取题目完成时间
        /// </summary>
        public TimeSpan Penalty
        {
            get { return this._penalty; }
        }

        /// <summary>
        /// 获取题目出错次数
        /// </summary>
        public Int32 WrongCount
        {
            get { return this._wrongCount; }
        }
        #endregion

        #region 构造方法
        public RankItemInfo(String userName, Int32 pid, RankItemInfoType type, TimeSpan penalty, Int32 wrongCount)
        {
            this._userName = userName;
            this._problemID = pid;
            this._type = type;
            this._penalty = penalty;
            this._wrongCount = wrongCount;
        }
        #endregion
    }
}