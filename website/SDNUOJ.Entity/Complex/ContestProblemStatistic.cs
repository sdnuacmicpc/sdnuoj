using System;
using System.Collections.Generic;

namespace SDNUOJ.Entity.Complex
{
    /// <summary>
    /// 竞赛题目统计信息实体类
    /// </summary>
    [Serializable]
    public class ContestProblemStatistic : ProblemStatistic
    {
        #region 字段
        private Dictionary<Byte, LanguageStatistic> _langStatistic;
        #endregion

        #region 方法
        public ContestProblemStatistic()
        {
            this._langStatistic = new Dictionary<Byte, LanguageStatistic>();
        }

        public void SetLanguageStatistic(Byte langID, Int32 count)
        {
            this._langStatistic[langID] = new LanguageStatistic() { ProblemID = this.ProblemID, LanguageID = langID, Count = count };
        }

        public LanguageStatistic GetLanguageStatistic(Byte langID)
        {
            LanguageStatistic statistic = null;

            return this._langStatistic.TryGetValue(langID, out statistic) ? statistic : new LanguageStatistic() { ProblemID = this.ProblemID, LanguageID = langID, Count = 0 };
        }
        #endregion
    }
}