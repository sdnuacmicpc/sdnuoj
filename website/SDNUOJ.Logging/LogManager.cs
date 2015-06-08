using System;

using NLog;

using SDNUOJ.Configuration;

namespace SDNUOJ.Logging
{
    /// <summary>
    /// 日志操作管理器
    /// </summary>
    public static class LogManager
    {
        #region Exception
        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="context">日志上下文</param>
        public static void LogException(ExceptionLogContext context)
        {
            try
            {
                Logger logger = NLog.LogManager.GetLogger("Error");

                if (ConfigurationManager.LoggingEnable && logger.IsErrorEnabled)
                {
                    LogEventInfo log = new LogEventInfo(LogLevelConverter.Convert(context.Level), "", "");

                    log.Properties["type"] = context.Type;
                    log.Properties["url"] = context.RequestUrl;
                    log.Properties["referer"] = context.RefererUrl;
                    log.Properties["controller"] = context.Controller;
                    log.Properties["action"] = context.Action;
                    log.Properties["username"] = context.Username;
                    log.Properties["userip"] = context.UserIP;
                    log.Properties["useragent"] = context.UserAgent;
                    log.Message = context.Message;
                    log.Exception = context.Exception;
                    log.TimeStamp = context.TimeStamp;

                    logger.Log(log);
                }
            }
            catch { }
        }
        #endregion

        #region Operation
        /// <summary>
        /// 记录用户操作日志
        /// </summary>
        /// <param name="context">日志上下文</param>
        public static void LogOperation(LogContext context)
        {
            try
            {
                Logger logger = NLog.LogManager.GetLogger("Operation");

                if (ConfigurationManager.LoggingEnable && logger.IsInfoEnabled)
                {
                    LogEventInfo log = new LogEventInfo(LogLevelConverter.Convert(context.Level), "", "");

                    log.Properties["type"] = context.Type;
                    log.Properties["url"] = context.RequestUrl;
                    log.Properties["referer"] = context.RefererUrl;
                    log.Properties["controller"] = context.Controller;
                    log.Properties["action"] = context.Action;
                    log.Properties["username"] = context.Username;
                    log.Properties["userip"] = context.UserIP;
                    log.Properties["useragent"] = context.UserAgent;
                    log.Message = context.Message;
                    log.TimeStamp = context.TimeStamp;

                    logger.Log(log);
                }
            }
            catch { }
        }
        #endregion
    }
}