using System;
using System.Configuration;
using System.Web;

using NLog;

using SDNUOJ.Configuration;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Controllers.Logging
{
    /// <summary>
    /// 日志操作管理器
    /// </summary>
    public static class LogManager
    {
        #region Exception
        /// <summary>
        /// 记录异常事件
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <param name="action">命令</param>
        /// <param name="exception">抛出的异常对象</param>
        public static void LogException(System.Exception exception, String controller, String action)
        {
            if (exception is ConfigurationException)
            {
                return;
            }

            try
            {
                Logger logger = NLog.LogManager.GetLogger("Error");

                if (ConfigurationManager.LoggingEnable && logger.IsErrorEnabled)
                {
                    LogEventInfo log = new LogEventInfo(LogLevel.Error, "", "");

                    log.Properties["controller"] = controller;
                    log.Properties["action"] = action;
                    log.Message = exception.Message;
                    log.Exception = exception;

                    logger.Log(log);
                }
            }
            catch { }
        }
        #endregion

        #region Login
        /// <summary>
        /// 记录用户登陆成功事件
        /// </summary>
        /// <param name="context">Http上下文</param>
        /// <param name="userName">用户名</param>
        public static void LogLoginSuccess(HttpContext context, String userName)
        {
            try
            {
                Logger logger = NLog.LogManager.GetLogger("Operation");

                if (ConfigurationManager.LoggingEnable && logger.IsInfoEnabled)
                {
                    LogEventInfo log = new LogEventInfo(LogLevel.Info, "", "");

                    log.Properties["type"] = "Login";
                    log.Properties["username"] = userName;
                    log.Properties["userip"] = RemoteClient.GetRemoteClientIPv4(context);
                    log.Message = "Success";

                    logger.Log(log);
                }
            }
            catch { }
        }

        /// <summary>
        /// 记录用户登陆失败事件
        /// </summary>
        /// <param name="context">Http上下文</param>
        /// <param name="userName">用户名</param>
        /// <param name="error">错误内容</param>
        public static void LogLoginFailed(HttpContext context, String userName, String error)
        {
            try
            {
                Logger logger = NLog.LogManager.GetLogger("Operation");
                
                if (ConfigurationManager.LoggingEnable && logger.IsWarnEnabled)
                {
                    LogEventInfo log = new LogEventInfo(LogLevel.Warn, "", "");

                    log.Properties["type"] = "Login";
                    log.Properties["username"] = userName;
                    log.Properties["userip"] = RemoteClient.GetRemoteClientIPv4(context);
                    log.Message = error;

                    logger.Log(log);
                }
            }
            catch { }
        }
        #endregion

        #region Operation
        /// <summary>
        /// 记录用户重要操作事件
        /// </summary>
        /// <param name="context">Http上下文</param>
        /// <param name="userName">用户名</param>
        /// <param name="message">操作相关消息</param>
        public static void LogOperation(HttpContext context, String userName, String message)
        {
            try
            {
                Logger logger = NLog.LogManager.GetLogger("Operation");
                
                if (ConfigurationManager.LoggingEnable && logger.IsInfoEnabled)
                {
                    LogEventInfo log = new LogEventInfo(LogLevel.Info, "", "");

                    log.Properties["type"] = "Operation";
                    log.Properties["username"] = userName;
                    log.Properties["userip"] = RemoteClient.GetRemoteClientIPv4(context);
                    log.Message = message;

                    logger.Log(log);
                }
            }
            catch { }
        }
        #endregion
    }
}