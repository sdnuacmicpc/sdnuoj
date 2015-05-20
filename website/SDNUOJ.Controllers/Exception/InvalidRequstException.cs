using System;
using System.Runtime.Serialization;

namespace SDNUOJ.Controllers.Exception
{
    /// <summary>
    /// 非法请求异常
    /// </summary>
    [Serializable]
    public class InvalidRequstException : UserException, ISerializable
    {
        #region 属性
        /// <summary>
        /// 获取是否需要记录
        /// </summary>
        public override Boolean IsNeedLog { get { return false; } }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化新的非法请求异常
        /// </summary>
        /// <param name="type">非法请求项目类型</param>
        public InvalidRequstException(RequestType type)
            : base(GetExceptionMessage(type) + " is INVALID!") { }
        #endregion

        #region 静态方法
        private static String GetExceptionMessage(RequestType type)
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
}