using System;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 方法返回接口
    /// </summary>
    public interface IMethodResult
    {
        /// <summary>
        /// 获取是否执行成功
        /// </summary>
        Boolean IsSuccess { get; }

        /// <summary>
        /// 获取是否记录日志
        /// </summary>
        Boolean IsWriteLog { get; }

        /// <summary>
        /// 获取执行描述
        /// </summary>
        String Description { get; }

        /// <summary>
        /// 获取是否包含执行结果
        /// </summary>
        Boolean HasResult { get; }

        /// <summary>
        /// 获取执行结果
        /// </summary>
        Object ResultObject { get; }
    }

    /// <summary>
    /// 方法返回接口
    /// </summary>
    public interface IMethodResult<T> : IMethodResult
    {
        /// <summary>
        /// 获取执行结果
        /// </summary>
        T Result { get; }
    }

    #region MethodResult
    /// <summary>
    /// 无返回值的方法结果类
    /// </summary>
    public abstract class MethodResult : IMethodResult
    {
        /// <summary>
        /// 获取是否执行成功
        /// </summary>
        public abstract Boolean IsSuccess { get; }

        /// <summary>
        /// 获取是否记录日志
        /// </summary>
        public abstract Boolean IsWriteLog { get; }

        /// <summary>
        /// 获取执行描述
        /// </summary>
        public String Description { get; private set; }

        /// <summary>
        /// 获取是否包含执行结果
        /// </summary>
        public virtual Boolean HasResult { get { return false; } }

        /// <summary>
        /// 获取执行结果
        /// </summary>
        public virtual Object ResultObject { get { return null; } }

        /// <summary>
        /// 初始化新的方法结果类
        /// </summary>
        /// <param name="description">执行描述</param>
        protected MethodResult(String description)
        {
            this.Description = description;
        }

        /// <summary>
        /// 返回新的方法成功结果
        /// </summary>
        /// <returns>方法成功结果类</returns>
        public static MethodSuccessResult Success()
        {
            return MethodSuccessResult.CreateEmpty();
        }

        /// <summary>
        /// 返回新的方法成功结果
        /// </summary>
        /// <param name="result">请求结果</param>
        /// <returns>方法成功结果类</returns>
        public static MethodSuccessResult<T> Success<T>(T result)
        {
            return MethodSuccessResult<T>.CreateEmpty(result);
        }

        /// <summary>
        /// 返回新的方法成功结果
        /// </summary>
        /// <param name="format">执行描述格式</param>
        /// <param name="args">执行描述参数</param>
        /// <returns>方法成功结果类</returns>
        public static MethodSuccessResult SuccessAndLog(String format, params String[] args)
        {
            return MethodSuccessResult.CreateLog(format, args);
        }

        /// <summary>
        /// 返回新的方法成功结果
        /// </summary>
        /// <param name="result">请求结果</param>
        /// <param name="format">执行描述格式</param>
        /// <param name="args">执行描述参数</param>
        /// <returns>方法成功结果类</returns>
        public static MethodSuccessResult<T> SuccessAndLog<T>(T result, String format, params String[] args)
        {
            return MethodSuccessResult<T>.CreateLog(result, format, args);
        }

        /// <summary>
        /// 返回新的方法失败结果
        /// </summary>
        /// <param name="format">执行描述格式</param>
        /// <param name="args">执行描述参数</param>
        /// <returns>方法失败结果类</returns>
        public static MethodFailedResult Failed(String format, params String[] args)
        {
            return MethodFailedResult.Create(format, args);
        }

        /// <summary>
        /// 返回新的方法失败结果
        /// </summary>
        /// <param name="format">执行描述格式</param>
        /// <param name="args">执行描述参数</param>
        /// <returns>方法失败结果类</returns>
        public static MethodFailedResult FailedAndLog(String format, params String[] args)
        {
            return MethodFailedResult.CreateLog(format, args);
        }

        /// <summary>
        /// 返回新的非法请求异常结果
        /// </summary>
        /// <param name="type">请求类型</param>
        /// <returns>非法请求异常结果类</returns>
        public static InvalidRequstResult InvalidRequst(RequestType type)
        {
            return InvalidRequstResult.Create(type);
        }
    }

    /// <summary>
    /// 方法结果类
    /// </summary>
    /// <typeparam name="T">结果类型</typeparam>
    public abstract class MethodResult<T> : MethodResult, IMethodResult<T>
    {
        /// <summary>
        /// 获取执行结果
        /// </summary>
        public T Result { get; private set; }

        /// <summary>
        /// 获取是否包含执行结果
        /// </summary>
        public override Boolean HasResult { get { return true; } }

        /// <summary>
        /// 获取执行结果
        /// </summary>
        public override Object ResultObject { get { return this.Result; } }

        /// <summary>
        /// 初始化新的方法结果类
        /// </summary>
        /// <param name="result">执行结果</param>
        /// <param name="description">执行描述</param>
        protected MethodResult(T result, String description) : base(description)
        {
            this.Result = result;
        }
    }
    #endregion

    #region MethodSuccessResult
    /// <summary>
    /// 方法成功结果类
    /// </summary>
    public class MethodSuccessResult : MethodResult
    {
        private readonly Boolean _isWriteLog;

        /// <summary>
        /// 获取是否执行成功
        /// </summary>
        public override Boolean IsSuccess { get { return true; } }

        /// <summary>
        /// 获取是否记录日志
        /// </summary>
        public override Boolean IsWriteLog { get { return this._isWriteLog; } }

        /// <summary>
        /// 初始化新的方法成功结果类
        /// </summary>
        /// <param name="description">执行描述</param>
        /// <param name="isWriteLog">是否记录日志</param>
        protected MethodSuccessResult(String description, Boolean isWriteLog)
            : base(description)
        {
            this._isWriteLog = isWriteLog;
        }

        /// <summary>
        /// 返回新的方法成功结果类
        /// </summary>
        /// <returns>方法成功结果类</returns>
        public static MethodSuccessResult CreateEmpty()
        {
            return new MethodSuccessResult(String.Empty, false);
        }

        /// <summary>
        /// 返回新的方法成功结果类
        /// </summary>
        /// <param name="format">执行描述格式</param>
        /// <param name="args">执行描述参数</param>
        /// <returns>方法成功结果类</returns>
        public static MethodSuccessResult Create(String format, params String[] args)
        {
            return new MethodSuccessResult(String.Format(format, args), false);
        }

        /// <summary>
        /// 返回新的方法成功结果类
        /// </summary>
        /// <param name="format">执行描述格式</param>
        /// <param name="args">执行描述参数</param>
        /// <returns>方法成功结果类</returns>
        public static MethodSuccessResult CreateLog(String format, params String[] args)
        {
            return new MethodSuccessResult(String.Format(format, args), true);
        }
    }

    /// <summary>
    /// 方法成功结果类
    /// </summary>
    public class MethodSuccessResult<T> : MethodResult<T>
    {
        private readonly Boolean _isWriteLog;

        /// <summary>
        /// 获取是否执行成功
        /// </summary>
        public override Boolean IsSuccess { get { return true; } }

        /// <summary>
        /// 获取是否记录日志
        /// </summary>
        public override Boolean IsWriteLog { get { return this._isWriteLog; } }

        /// <summary>
        /// 初始化新的方法成功结果类
        /// </summary>
        /// <param name="result">执行结果</param>
        /// <param name="description">执行描述</param>
        /// <param name="isWriteLog">是否记录日志</param>
        protected MethodSuccessResult(T result, String description, Boolean isWriteLog)
            : base(result, description)
        {
            this._isWriteLog = isWriteLog;
        }

        /// <summary>
        /// 返回新的方法成功结果类
        /// </summary>
        /// <param name="result">执行结果</param>
        /// <exception cref="ArgumentNullException">执行结果不能为空</exception>
        /// <returns>方法成功结果类</returns>
        public static MethodSuccessResult<T> CreateEmpty(T result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            return new MethodSuccessResult<T>(result, String.Empty, false);
        }

        /// <summary>
        /// 返回新的方法成功结果类
        /// </summary>
        /// <param name="result">执行结果</param>
        /// <param name="format">执行描述格式</param>
        /// <param name="args">执行描述参数</param>
        /// <exception cref="ArgumentNullException">执行结果不能为空</exception>
        /// <returns>方法成功结果类</returns>
        public static MethodSuccessResult<T> CreateLog(T result, String format, params String[] args)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            return new MethodSuccessResult<T>(result, String.Format(format, args), true);
        }
    }
    #endregion

    #region MethodFailedResult
    /// <summary>
    /// 方法失败结果类
    /// </summary>
    public class MethodFailedResult : MethodResult
    {
        private readonly Boolean _isWriteLog;

        /// <summary>
        /// 获取是否执行成功
        /// </summary>
        public override Boolean IsSuccess { get { return false; } }

        /// <summary>
        /// 获取是否记录日志
        /// </summary>
        public override Boolean IsWriteLog { get { return this._isWriteLog; } }

        /// <summary>
        /// 初始化新的方法失败结果类
        /// </summary>
        /// <param name="description">执行描述</param>
        /// <param name="isWriteLog">是否记录日志</param>
        protected MethodFailedResult(String description, Boolean isWriteLog)
            : base(description)
        {
            this._isWriteLog = isWriteLog;
        }

        /// <summary>
        /// 返回新的方法失败结果类
        /// </summary>
        /// <param name="format">执行描述格式</param>
        /// <param name="args">执行描述参数</param>
        /// <returns>方法失败结果类</returns>
        public static MethodFailedResult Create(String format, params String[] args)
        {
            return new MethodFailedResult(String.Format(format, args), false);
        }

        /// <summary>
        /// 返回新的方法失败结果类
        /// </summary>
        /// <param name="format">执行描述格式</param>
        /// <param name="args">执行描述参数</param>
        /// <returns>方法失败结果类</returns>
        public static MethodFailedResult CreateLog(String format, params String[] args)
        {
            return new MethodFailedResult(String.Format(format, args), true);
        }
    }

    /// <summary>
    /// 非法请求异常结果类
    /// </summary>
    public sealed class InvalidRequstResult : MethodFailedResult
    {
        /// <summary>
        /// 初始化新的非法请求异常结果类
        /// </summary>
        /// <param name="type">请求类型</param>
        private InvalidRequstResult(RequestType type)
            : base(GetErrorMessage(type) + " is INVALID!", false) { }

        /// <summary>
        /// 返回新的方法失败结果类
        /// </summary>
        /// <param name="type">请求类型</param>
        /// <returns>方法失败结果类</returns>
        public static InvalidRequstResult Create(RequestType type)
        {
            return new InvalidRequstResult(type);
        }

        #region 静态方法
        private static String GetErrorMessage(RequestType type)
        {
            switch (type)
            {
                case RequestType.User: return "Username";
                case RequestType.Problem: return "Problem ID";
                case RequestType.ProblemCategory: return "Problem Category ID";
                case RequestType.Contest: return "Contest ID";
                case RequestType.Solution: return "Run ID";
                case RequestType.SolutionError: return "Solution ID";
                case RequestType.ForumTopic: return "Forum Topic ID";
                case RequestType.ForumPost: return "Forum Post ID";
                case RequestType.News: return "News ID";
                case RequestType.UserMail: return "UserMail ID";
                case RequestType.UserForgetPassword: return "Request ID";
                case RequestType.TopicPage: return "Page Name";
                case RequestType.Resource: return "Resource ID";
                default: return "Request";
            }
        }
        #endregion
    }
    #endregion
}