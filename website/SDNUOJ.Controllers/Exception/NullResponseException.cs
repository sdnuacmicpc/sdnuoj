using System;
using System.Runtime.Serialization;

namespace SDNUOJ.Controllers.Exception
{
    /// <summary>
    /// 无结果异常
    /// </summary>
    [Serializable]
    public class NullResponseException : UserException, ISerializable
    {
        #region 属性
        /// <summary>
        /// 获取是否需要记录
        /// </summary>
        public override Boolean IsNeedLog { get { return false; } }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化新的无结果异常
        /// </summary>
        /// <param name="type">请求项目类型</param>
        public NullResponseException(RequestType type)
            : base("No such " + NullResponseException.GetExceptionMessage(type) + "!") { }
        #endregion

        #region 静态方法
        private static String GetExceptionMessage(RequestType type)
        {
            switch (type)
            {
                case RequestType.User: return "user";
                case RequestType.Problem: return "problem";
                case RequestType.ProblemCategory: return "problem category";
                case RequestType.Contest: return "contest";
                case RequestType.Solution: return "solution";
                case RequestType.SolutionError: return "solution error";
                case RequestType.ForumTopic: return "forum topic";
                case RequestType.ForumPost: return "forum post";
                case RequestType.News: return "news";
                case RequestType.UserMail: return "user mail";
                case RequestType.UserForgetPassword: return "forget request";
                case RequestType.TopicPage: return "topic page";
                case RequestType.Resource: return "resource";
                default: return "item";
            }
        }
        #endregion
    }
}