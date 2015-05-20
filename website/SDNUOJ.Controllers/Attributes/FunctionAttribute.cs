using System;
using System.Collections.Generic;
using System.Web.Mvc;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Exception;

namespace SDNUOJ.Controllers.Attributes
{
    /// <summary>
    /// 模块类型特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class FunctionAttribute : ActionFilterAttribute
    {
        #region 静态字段
        private static Dictionary<PageType, Boolean> _functionEnableTable;
        #endregion

        #region 字段
        private PageType _type;
        #endregion

        #region 静态构造方法
        static FunctionAttribute()
        {
            _functionEnableTable = new Dictionary<PageType, Boolean>()
            {
                { PageType.Register,            ConfigurationManager.AllowRegister },
                { PageType.ForgetPassword,      ConfigurationManager.AllowForgetPassword },
                { PageType.UserControl,         ConfigurationManager.AllowUserControl },
                { PageType.UserMail,            ConfigurationManager.AllowUserMail },
                { PageType.UserInfo,            ConfigurationManager.AllowUserInfo },
                { PageType.SourceView,          ConfigurationManager.AllowSourceView },
                { PageType.MainSubmit,          ConfigurationManager.AllowMainSubmit },
                { PageType.MainForum,           ConfigurationManager.AllowMainForum },
                { PageType.MainProblem,         ConfigurationManager.AllowMainProblem },
                { PageType.MainRanklist,        ConfigurationManager.AllowMainRanklist },
                { PageType.MainStatus,          ConfigurationManager.AllowMainStatus },
                { PageType.Resource,            ConfigurationManager.AllowResource },
                { PageType.Contest,             ConfigurationManager.AllowContest }
            };
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 初始化新的模块特性
        /// </summary>
        /// <param name="type">页面类型</param>
        public FunctionAttribute(PageType type)
        {
            this._type = type;
        }
        #endregion

        #region 方法
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Boolean value = false;

            if (!_functionEnableTable.TryGetValue(this._type, out value) || !value)
            {
                throw new FunctionDisabledException(this._type);
            }
        }
        #endregion
    }
}