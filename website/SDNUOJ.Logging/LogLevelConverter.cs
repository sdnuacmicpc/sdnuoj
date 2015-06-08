using System;
using System.Collections.Generic;

namespace SDNUOJ.Logging
{
    /// <summary>
    /// 日志级别转换器
    /// </summary>
    internal static class LogLevelConverter
    {
        #region 常量
        private static readonly Dictionary<LogLevel, NLog.LogLevel> LevelMappingTable = new Dictionary<LogLevel, NLog.LogLevel>()
        {
            { LogLevel.Debug, NLog.LogLevel.Debug },
            { LogLevel.Verbose, NLog.LogLevel.Trace },
            { LogLevel.Information, NLog.LogLevel.Info },
            { LogLevel.Warning, NLog.LogLevel.Warn },
            { LogLevel.Error, NLog.LogLevel.Error },
            { LogLevel.Critical, NLog.LogLevel.Fatal }
        };
        #endregion

        #region 方法
        internal static NLog.LogLevel Convert(LogLevel level)
        {
            return LevelMappingTable[level];
        }
        #endregion
    }
}