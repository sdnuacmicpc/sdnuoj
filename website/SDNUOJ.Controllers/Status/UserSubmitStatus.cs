using System;
using System.Collections.Generic;

using SDNUOJ.Configuration;

namespace SDNUOJ.Controllers.Status
{
    /// <summary>
    /// 用户提交状态类
    /// </summary>
    public static class UserSubmitStatus
    {
        #region 常量
        private static readonly Int64 SUBMIT_INTERVAL_TICKS;
        #endregion

        #region 字段
        private static Dictionary<String, Int64> _solutionSubmitTime;
        private static Dictionary<String, Int64> _forumSubmitTime;
        private static Dictionary<String, Int64> _mailSubmitTime;
        #endregion

        #region 构造方法
        static UserSubmitStatus()
        {
            TimeSpan ts = new TimeSpan(0, 0, ConfigurationManager.SubmitInterval);
            UserSubmitStatus.SUBMIT_INTERVAL_TICKS = ts.Ticks;

            _solutionSubmitTime = new Dictionary<String, Int64>();
            _forumSubmitTime = new Dictionary<String, Int64>();
            _mailSubmitTime = new Dictionary<String, Int64>();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化最后提交时间
        /// </summary>
        /// <param name="userName">用户名</param>
        public static void InitLastSubmitTime(String userName)
        {
            Int64 submitTicks = DateTime.Now.Ticks - SUBMIT_INTERVAL_TICKS;

            _solutionSubmitTime[userName] = submitTicks;
            _forumSubmitTime[userName] = submitTicks;
            _mailSubmitTime[userName] = submitTicks;
        }

        /// <summary>
        /// 检查最后提交代码的时间
        /// </summary>
        /// <param name="userName">用户名</param>
        public static Boolean CheckLastSubmitSolutionTime(String userName)
        {
            return UserSubmitStatus.CheckLastSubmitTime(userName, _solutionSubmitTime);
        }

        /// <summary>
        /// 检查最后提交帖子的时间
        /// </summary>
        /// <param name="userName">用户名</param>
        public static Boolean CheckLastSubmitForumPostTime(String userName)
        {
            return UserSubmitStatus.CheckLastSubmitTime(userName, _forumSubmitTime);
        }

        /// <summary>
        /// 检查最后发送站内信的时间
        /// </summary>
        /// <param name="userName">用户名</param>
        public static Boolean CheckLastSubmitUserMailTime(String userName)
        {
            return UserSubmitStatus.CheckLastSubmitTime(userName, _mailSubmitTime);
        }

        /// <summary>
        /// 从缓存中删除后提交时间
        /// </summary>
        /// <param name="userName">用户名</param>
        public static void RemoveLastSubmitTime(String userName)
        {
            _solutionSubmitTime.Remove(userName);
            _forumSubmitTime.Remove(userName);
            _mailSubmitTime.Remove(userName);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 检查指定最后提交时间
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="ticksList">最后提交时间列表</param>
        /// <returns>检查时候成功</returns>
        private static Boolean CheckLastSubmitTime(String userName, Dictionary<String, Int64> ticksList)
        {
            if (ConfigurationManager.SubmitInterval <= 0)
            {
                return true;
            }

            Int64 lastTicks = 0;

            if (!ticksList.TryGetValue(userName, out lastTicks))
            {
                ticksList[userName] = DateTime.Now.Ticks;
                return false;
            }
            else
            {
                if (DateTime.Now.Ticks - lastTicks > SUBMIT_INTERVAL_TICKS)
                {
                    ticksList[userName] = DateTime.Now.Ticks;
                    return true;
                }
                else
                {
                    return false;//如果此次间隔时间不够不再重新设置间隔时间
                }
            }
        }
        #endregion
    }
}