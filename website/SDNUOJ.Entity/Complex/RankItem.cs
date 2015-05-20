using System;
using System.Collections.Generic;

namespace SDNUOJ.Entity.Complex
{
    /// <summary>
    /// 用户排名类
    /// </summary>
    public class RankItem : IComparable<RankItem>
    {
        #region 字段
        private String _userName;
        private Int32 _penalty;
        private Int32 _allWrongCount;
        private Dictionary<Int32, TimeSpan> _penaltys;
        private Dictionary<Int32, Int32> _wrongCount;
        private Dictionary<Int32, Boolean> _firstBlood;
        #endregion

        #region 属性
        /// <summary>
        /// 获取用户名
        /// </summary>
        public String UserName { get { return this._userName; } }

        /// <summary>
        /// 获取指定题目罚时
        /// </summary>
        public Dictionary<Int32, TimeSpan> Penaltys { get { return this._penaltys; } }

        /// <summary>
        /// 获取指定题目错误次数
        /// </summary>
        public Dictionary<Int32, Int32> WrongCount { get { return this._wrongCount; } }

        /// <summary>
        /// 获取指定题目是否为第一个完成
        /// </summary>
        public Dictionary<Int32, Boolean> FirstBlood { get { return this._firstBlood; } }

        /// <summary>
        /// 获取完成题目数量
        /// </summary>
        public Int32 SolvedCount { get { return this._penaltys.Count; } }

        /// <summary>
        /// 获取总罚时秒数
        /// </summary>
        public Int32 PenaltySecond { get { return this._penalty; } }

        /// <summary>
        /// 获取总罚时
        /// </summary>
        public TimeSpan Penalty { get { return new TimeSpan(0, 0, this._penalty); } }
        #endregion

        #region 索引器
        /// <summary>
        /// 获取指定题目的竞赛信息类
        /// </summary>
        /// <param name="pid">题目ID</param>
        /// <returns>竞赛信息类/returns>
        public RankItemInfo this[Int32 pid]
        {
            get
            {
                return new RankItemInfo(this._userName,
                    pid,
                    this.GetProblemRankType(pid),
                    this._penaltys.ContainsKey(pid) ? this._penaltys[pid] : TimeSpan.Zero,
                    this._wrongCount.ContainsKey(pid) ? this._wrongCount[pid] : 0);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 创建新的竞赛排名类
        /// </summary>
        /// <param name="userName">用户名</param>
        public RankItem(String userName)
        {
            this._userName = userName;
            this._penalty = 0;
            this._allWrongCount = 0;

            this._penaltys = new Dictionary<Int32, TimeSpan>();
            this._wrongCount = new Dictionary<Int32, Int32>();
            this._firstBlood = new Dictionary<Int32, Boolean>();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 添加通过题目
        /// </summary>
        /// <param name="pid">题目ID</param>
        /// <param name="penalty">题目完成时间</param>
        /// <param name="firstBlood">是否第一个通过</param>
        public void AddAcceptedProblem(Int32 pid, TimeSpan penalty, Boolean firstBlood)
        {
            if (!this._penaltys.ContainsKey(pid))
            {
                this._penaltys[pid] = penalty;
                this._firstBlood[pid] = firstBlood;

                this._penalty += (Int32)penalty.TotalSeconds;
            }
        }

        /// <summary>
        /// 添加失败题目
        /// </summary>
        /// <param name="pid">题目ID</param>
        /// <param name="count">失败次数</param>
        public void AddWrongProblemCount(Int32 pid, Int32 count)
        {
            Int32 oldcount = 0;
            this._wrongCount.TryGetValue(pid, out oldcount);
            this._wrongCount[pid] = count + oldcount;
            this._allWrongCount += count;

            if (this._penaltys.ContainsKey(pid))
            {
                this._penalty += count * 20 * 60;
            }
        }

        /// <summary>
        /// 获取指定题目的类型
        /// </summary>
        /// <param name="pid">题目ID</param>
        /// <returns>题目的类型</returns>
        public RankItemInfoType GetProblemRankType(Int32 pid)
        {
            if (this._firstBlood.ContainsKey(pid) && this._firstBlood[pid])
            {
                return RankItemInfoType.FirstBlood;
            }
            else if (this._penaltys.ContainsKey(pid))
            {
                return RankItemInfoType.Accepted;
            }
            else if (this._wrongCount.ContainsKey(pid))
            {
                return RankItemInfoType.Wrong;
            }
            else
            {
                return RankItemInfoType.Normal;
            }
        }
        #endregion

        #region 接口方法
        public Int32 CompareTo(RankItem obj)
        {
            if (this._penaltys.Count != obj._penaltys.Count)
            {
                return -this._penaltys.Count.CompareTo(obj._penaltys.Count);
            }

            if (this._penalty != obj._penalty)
            {
                return this._penalty.CompareTo(obj._penalty);
            }

            return this._allWrongCount.CompareTo(obj._allWrongCount);
        }
        #endregion
    }
}