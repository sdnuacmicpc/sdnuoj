using System;
using System.Runtime.Serialization;

namespace SDNUOJ.Controllers.Exception
{
    /// <summary>
    /// 功能禁止异常
    /// </summary>
    [Serializable]
    public class FunctionDisabledException : UserException, ISerializable
    {
        #region 属性
        /// <summary>
        /// 获取是否需要记录
        /// </summary>
        public override Boolean IsNeedLog { get { return false; } }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化新的功能禁止异常
        /// </summary>
        /// <param name="type">页面执行功能类别</param>
        public FunctionDisabledException(PageType type)
            : base(GetExceptionMessage(type) + " is DISABLED!") { }

        /// <summary>
        /// 初始化新的功能禁止异常
        /// </summary>
        /// <param name="message">出错信息</param>
        public FunctionDisabledException(String message) : base(message) { }
        #endregion

        #region 静态方法
        private static String GetExceptionMessage(PageType type)
        {
            switch (type)
            {
                case PageType.Register: return "User Registration";
                case PageType.ForgetPassword: return "Forget Password";
                case PageType.UserControl: return "User Control";
                case PageType.UserMail: return "User Mail";
                case PageType.UserInfo: return "User Info";
                case PageType.SourceView: return "Source View";
                case PageType.MainSubmit: return "Problem Submit";
                case PageType.MainForum: return "Forum";
                case PageType.MainProblem: return "Problem";
                case PageType.MainRanklist: return "Ranklist";
                case PageType.MainStatus: return "Submit Status";
                case PageType.Resource: return "Resource";
                case PageType.Contest: return "Contest";
                default: return String.Empty;
            }
        }
        #endregion
    }
}