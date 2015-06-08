using System;

namespace SDNUOJ.Logging
{
    /// <summary>
    /// 异常日志上下文
    /// </summary>
    [Serializable]
    public class ExceptionLogContext : LogContext
    {
        #region 字段
        private Exception _exception;
        #endregion

        #region 属性
        /// <summary>
        /// 获取日志级别
        /// </summary>
        public override LogLevel Level
        {
            get { return LogLevel.Error; }
        }

        /// <summary>
        /// 获取日志类型
        /// </summary>
        public override String Type
        {
            get { return "error"; }
        }

        /// <summary>
        /// 获取日志内容
        /// </summary>
        public override String Message
        {
            get { return this._exception.Message; }
        }

        /// <summary>
        /// 获取当前异常
        /// </summary>
        public Exception Exception
        {
            get { return this._exception; }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 初始化新的异常日志上下文
        /// </summary>
        /// <param name="ex">异常</param>
        public ExceptionLogContext(Exception ex)
        {
            this._exception = ex;
        }
        #endregion
    }
}