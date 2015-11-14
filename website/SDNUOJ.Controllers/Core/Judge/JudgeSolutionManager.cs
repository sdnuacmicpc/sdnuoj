using System;
using System.Collections.Generic;
using System.Text;

using SDNUOJ.Entity;
using SDNUOJ.Utilities;
using SDNUOJ.Utilities.Text;

namespace SDNUOJ.Controllers.Core.Judge
{
    internal static class JudgeSolutionManager
    {
        #region 常量
        /// <summary>
        /// 自动重测最多尝试次数
        /// </summary>
        private const Int32 AUTO_REJUDGE_MAX_TIMES = 5;
        #endregion

        #region 字段
        private static Dictionary<Int32, Int32> _rejudgeTimesMap;
        #endregion

        #region 构造方法
        static JudgeSolutionManager()
        {
            _rejudgeTimesMap = new Dictionary<Int32, Int32>();
        }
        #endregion

        #region 评测机获取评测列表
        /// <summary>
        /// 获取评测列表的Json信息
        /// </summary>
        /// <param name="lanaugeSupport">评测机支持语言</param>
        /// <param name="count">评测机请求个数</param>
        /// <returns>评测列表Json信息</returns>
        public static Boolean TryGetPendingListJson(String lanaugeSupport, String count, out String result, out String error)
        {
            result = String.Empty;

            try
            {
                error = JudgeStatusManager.GetJudgeServerLoginStatus();

                if (!String.IsNullOrEmpty(error))
                {
                    return false;
                }

                StringBuilder ret = new StringBuilder();
                Int32 requestCount = Math.Max(1, count.ToInt32(1));
                List<SolutionEntity> pendingList = SolutionManager.JudgeGetPendingSolution(requestCount, GetJudgeSupportLanguages(lanaugeSupport));

                Dictionary<Int32, ProblemEntity> problemCache = new Dictionary<Int32, ProblemEntity>();
                Dictionary<Int32, String> problemVersionCache = new Dictionary<Int32, String>();

                ProblemEntity problem = null;
                String problemDataVersion = String.Empty;
                SolutionEntity solution = null;
                Int32 listCount = (pendingList == null ? 0 : pendingList.Count);

                ret.Append("[");

                for (Int32 i = 0; i < listCount; i++)
                {
                    if (i > 0) ret.Append(",");

                    solution = pendingList[i];

                    if (!problemCache.TryGetValue(solution.ProblemID, out problem))
                    {
                        problem = ProblemManager.GetJudgeProblem(solution.ProblemID);
                        problemCache[solution.ProblemID] = problem;
                    }

                    if (!problemVersionCache.TryGetValue(solution.ProblemID, out problemDataVersion))
                    {
                        problemDataVersion = ProblemDataManager.GetProblemDataVersion(solution.ProblemID);
                        problemVersionCache[solution.ProblemID] = problemDataVersion;
                    }

                    if (problem != null)
                    {
                        Double scale = solution.LanguageType.Scale;
                        Int32 timeLimit = (Int32)(problem.TimeLimit * scale);
                        Int32 memoryLimit = (Int32)(problem.MemoryLimit * scale);

                        ret.Append("{");
                        ret.Append("\"sid\":\"").Append(solution.SolutionID.ToString()).Append("\",");
                        ret.Append("\"pid\":\"").Append(solution.ProblemID.ToString()).Append("\",");
                        ret.Append("\"username\":\"").Append(solution.UserName).Append("\",");
                        ret.Append("\"dataversion\":\"").Append(problemDataVersion).Append("\",");
                        ret.Append("\"timelimit\":\"").Append(timeLimit.ToString()).Append("\",");
                        ret.Append("\"memorylimit\":\"").Append(memoryLimit.ToString()).Append("\",");
                        ret.Append("\"language\":\"").Append(solution.LanguageType.Type).Append("[]\",");
                        ret.Append("\"sourcecode\":\"").Append(JsonEncoder.JsonEncode(solution.SourceCode)).Append("\"");
                        ret.Append("}");
                    }
                }

                ret.Append("]");

                result = ret.ToString();
                return true;
            }
            catch (System.Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        #endregion

        #region 评测机更新评测状态
        /// <summary>
        /// 更新评测状态
        /// </summary>
        /// <param name="sid">提交ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="username">用户名</param>
        /// <param name="result">评测结果</param>
        /// <param name="detail">出错信息</param>
        /// <param name="tcost">花费时间</param>
        /// <param name="mcost">花费内存</param>
        /// <param name="error">错误信息</param>
        /// <returns>是否更新成功</returns>
        public static Boolean TryUpdateSolutionStatus(String sid, String pid, String username, String result, String detail, String tcost, String mcost, out String error)
        {
            try
            {
                error = JudgeStatusManager.GetJudgeServerLoginStatus();

                if (!String.IsNullOrEmpty(error))
                {
                    return false;
                }

                SolutionEntity entity = new SolutionEntity()
                {
                    SolutionID = Int32.Parse(sid),
                    ProblemID = pid.ToInt32(0),
                    UserName = username,
                    Result = (ResultType)result.ToByte(0),
                    TimeCost = tcost.ToInt32(0),
                    MemoryCost = mcost.ToInt32(0)
                };

                if (entity.Result > ResultType.Accepted)//评测失败
                {
                    Boolean hasProblemData = !String.IsNullOrEmpty(ProblemDataManager.GetProblemDataRealPath(entity.ProblemID));
                    
                    //没有题目的不重新评测
                    Boolean canAutoRejudge = hasProblemData;

                    Int32 triedTimes = 0;
                    if (!_rejudgeTimesMap.TryGetValue(entity.SolutionID, out triedTimes))
                    {
                        triedTimes = 0;
                    }

                    if (triedTimes > AUTO_REJUDGE_MAX_TIMES)
                    {
                        _rejudgeTimesMap.Remove(entity.SolutionID);
                        canAutoRejudge = false;
                    }
                    else
                    {
                        _rejudgeTimesMap[entity.SolutionID] = triedTimes + 1;
                    }

                    entity.Result = canAutoRejudge ? ResultType.RejudgePending : ResultType.JudgeFailed;
                }

                SolutionManager.JudgeUpdateSolutionAllResult(entity, detail);

                return true;
            }
            catch (System.Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取评测机支持的语言
        /// </summary>
        /// <param name="languageSupport">评测机支持的语言</param>
        /// <returns>评测机支持的语言</returns>
        private static LanguageType[] GetJudgeSupportLanguages(String languageSupport)
        {
            if (String.IsNullOrEmpty(languageSupport))
            {
                return new LanguageType[] { };
            }

            String[] languages = languageSupport.Split(',');
            List<LanguageType> languageTypes = new List<LanguageType>();

            for (Int32 i = 0; i < languages.Length; i++)
            {
                String type = (languages[i].IndexOf('[') > 0 ? languages[i].Substring(0, languages[i].IndexOf('[')) : languages[i]);
                LanguageType langType = LanguageType.FromLanguagType(type);

                if (!LanguageType.IsNull(langType) && !languageTypes.Contains(langType))
                {
                    languageTypes.Add(langType);
                }
            }

            return languageTypes.ToArray();
        }
        #endregion
    }
}