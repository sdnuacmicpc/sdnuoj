using System;

using SDNUOJ.Utilities;

namespace SDNUOJ.Controllers.Core.Judge
{
    internal static class JudgeProblemManager
    {
        #region 评测机获取题目数据
        /// <summary>
        /// 获取题目数据
        /// </summary>
        /// <param name="pid">题目ID</param>
        /// <param name="dataPath">题目数据路径</param>
        /// <param name="error">错误信息</param>
        /// <returns>获取是否成功</returns>
        public static Boolean TryGetProblemDataPath(String pid, out String dataPath, out String error)
        {
            dataPath = String.Empty;

            try
            {
                error = JudgeStatusManager.GetJudgeServerLoginStatus();

                if (!String.IsNullOrEmpty(error))
                {
                    return false;
                }

                Int32 problemID = pid.ToInt32(0);
                String path = ProblemDataManager.GetProblemDataRealPath(problemID);

                if (String.IsNullOrEmpty(path))
                {
                    error = "Problem data does not exist!";
                    return false;
                }

                dataPath = path;
                return true;
            }
            catch (System.Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        #endregion
    }
}