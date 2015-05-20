using System;
using System.Collections.Generic;

namespace SDNUOJ.Controllers.Status
{
    /// <summary>
    /// 评测机状态管理器
    /// </summary>
    public static class JudgeOnlineStatus
    {
        #region 状态存储
        private static Dictionary<String, DateTime> _lastDate = null;

        static JudgeOnlineStatus()
        {
            _lastDate = new Dictionary<String, DateTime>();
        }

        /// <summary>
        /// 向缓存中写入评测机最后登录时间
        /// </summary>
        /// <param name="serverID">评测机账号</param>
        public static void SetJudgeStatus(String serverID)
        {
            _lastDate[serverID] = DateTime.Now;
        }

        /// <summary>
        /// 获取当前在线评测机列表
        /// </summary>
        /// <returns>当前在线评测机列表</returns>
        public static List<KeyValuePair<String, DateTime>> GetOnlineJudges()
        {
            List<KeyValuePair<String, DateTime>> lstJudges = new List<KeyValuePair<String, DateTime>>();

            foreach (KeyValuePair<String, DateTime> pair in _lastDate)
            {
                lstJudges.Add(pair);
            }

            return lstJudges;
        }

        /// <summary>
        /// 获取评测机最后登录时间
        /// </summary>
        /// <param name="serverID">评测机账号</param>
        /// <returns>评测机最后登录时间</returns>
        public static DateTime? GetJudgeLastTime(String serverID)
        {
            DateTime lastDate;
            return _lastDate.TryGetValue(serverID, out lastDate) ? lastDate : new Nullable<DateTime>();
        }
        #endregion
    }
}