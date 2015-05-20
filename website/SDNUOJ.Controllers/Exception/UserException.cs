using System;
using System.Runtime.Serialization;
using System.Web;

using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Controllers.Exception
{
    /// <summary>
    /// 用户异常基类
    /// </summary>
    [Serializable]
    public abstract class UserException : ApplicationException, ISerializable
    {
        #region 字段
        protected String _newmessage;
        private DateTime _dateTime;
        private String _requestURL;
        private String _userIP;
        private String _userName;
        #endregion

        #region 属性
        /// <summary>
        /// 获取是否需要记录
        /// </summary>
        public abstract Boolean IsNeedLog { get; }

        /// <summary>
        /// 获取异常发生的时间
        /// </summary>
        public DateTime DateTime
        {
            get { return this._dateTime; }
        }

        /// <summary>
        /// 获取请求的URL地址
        /// </summary>
        public String RequestURL
        {
            get { return this._requestURL; }
        }

        /// <summary>
        /// 获取用户的IP地址
        /// </summary>
        public String UserIP
        {
            get { return this._userIP; }
        }

        /// <summary>
        /// 获取发生异常的用户名
        /// </summary>
        public String UserName
        {
            get { return this._userName; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化新的用户异常
        /// </summary>
        public UserException(String message)
            : base (message)
        {
            this._dateTime = DateTime.Now;

            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                this._requestURL = HttpContext.Current.Request.RawUrl;
                this._userIP = RemoteClient.GetRemoteClientIPv4(HttpContext.Current);
            }

            if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null)
            {
                this._userName = HttpContext.Current.User.Identity.Name;
            }
        }
        #endregion
    }
}