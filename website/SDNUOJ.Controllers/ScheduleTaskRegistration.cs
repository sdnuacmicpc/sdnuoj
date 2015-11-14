using System;
using SDNUOJ.Controllers.Core;

namespace SDNUOJ.Controllers
{
    public static class ScheduleTaskRegistration
    {
        /// <summary>
        /// 注册全部定时任务
        /// </summary>
        public static void RegisterAllScheduleTasks()
        {
            RecentContestManager.ScheduleGetAllRecentContestsJsonFromWeb();
        }
    }
}