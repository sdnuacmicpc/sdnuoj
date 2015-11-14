using System;
using System.Collections.Generic;
using System.Threading;

namespace SDNUOJ.Caching
{
    /// <summary>
    /// 定时任务管理器
    /// </summary>
    public static class ScheduledTaskManager
    {
        #region 字段
        private static Dictionary<String, Timer> _tasks;
        #endregion

        #region 构造方法
        static ScheduledTaskManager()
        {
            _tasks = new Dictionary<String, Timer>();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 设置定时任务
        /// </summary>
        /// <param name="taskName">任务名称</param>
        /// <param name="startDelay">开始延时（秒）</param>
        /// <param name="interval">间隔时间（秒）</param>
        /// <param name="action">执行内容</param>
        public static void Schedule(String taskName, Int32 startDelay, Int32 interval, Action action)
        {
            Timer timer = new Timer((state) => action(), null, TimeSpan.FromSeconds(startDelay), TimeSpan.FromSeconds(interval));
            _tasks[taskName] = timer;
        }
        #endregion
    }
}