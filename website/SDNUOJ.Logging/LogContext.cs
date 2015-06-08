using System;

namespace SDNUOJ.Logging
{
    /// <summary>
    /// 日志上下文类
    /// </summary>
    [Serializable]
    public class LogContext
    {
        /// <summary>
        /// 获取或设置日志级别
        /// </summary>
        public virtual LogLevel Level { get; set; }

        /// <summary>
        /// 获取或设置日志类型
        /// </summary>
        public virtual String Type { get; set; }

        /// <summary>
        /// 获取或设置日志内容
        /// </summary>
        public virtual String Message { get; set; }

        /// <summary>
        /// 获取或设置相关用户
        /// </summary>
        public String Username { get; set; }

        /// <summary>
        /// 获取或设置用户IP
        /// </summary>
        public String UserIP { get; set; }

        /// <summary>
        /// 获取或设置用户代理
        /// </summary>
        public String UserAgent { get; set; }

        /// <summary>
        /// 获取或设置请求地址
        /// </summary>
        public String RequestUrl { get; set; }

        /// <summary>
        /// 获取或设置来源地址
        /// </summary>
        public String RefererUrl { get; set; }

        /// <summary>
        /// 获取或设置控制器名称
        /// </summary>
        public String Controller { get; set; }

        /// <summary>
        /// 获取或设置操作名称
        /// </summary>
        public String Action { get; set; }

        /// <summary>
        /// 获取或设置日志时间戳
        /// </summary>
        public DateTime TimeStamp { get; set; }
    }
}