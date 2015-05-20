using System;
using System.Web;
using System.Web.Security;

using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Utilities.Security;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Controllers.Status
{
    /// <summary>
    /// 验证码状态类
    /// </summary>
    public static class CheckCodeStatus
    {
        #region 常量
        private static readonly String CHECK_CODE_COOKIE_NAME = FormsAuthentication.FormsCookieName + "cc";
        #endregion

        #region 方法
        /// <summary>
        /// 设置验证码信息
        /// </summary>
        /// <param name="checkCode">验证码信息</param>
        public static void SetCheckCode(String checkCode)
        {
            String hashed = CheckCodeStatus.EncryptCode(checkCode);

            Cookies.SetValue(CHECK_CODE_COOKIE_NAME, hashed, true, DateTime.Now.AddSeconds(ConfigurationManager.CheckCodeTimeout));
        }

        /// <summary>
        /// 验证用户输入的验证码是否正确
        /// </summary>
        /// <param name="code">用户输入的验证码</param>
        /// <returns>用户输入的验证码是否正确</returns>
        public static Boolean VerifyCheckCode(String code)
        {
            if (String.IsNullOrEmpty(code))
            {
                throw new InvalidInputException("The verification code can not be NULL!");
            }

            HttpCookie checkCode = CheckCodeStatus.GetCheckCodeCookie();

            if (checkCode == null || String.IsNullOrEmpty(checkCode.Value))
            {
                throw new InvalidInputException(String.Format("The verification codes are only valid for a maximum of {0} seconds!", ConfigurationManager.CheckCodeTimeout.ToString()));
            }

            String hashed = CheckCodeStatus.EncryptCode(code);

            CheckCodeStatus.RemoveCheckCode();

            return String.Equals(checkCode.Value, hashed, StringComparison.Ordinal);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 加密验证码
        /// </summary>
        /// <param name="checkCode">验证码信息</param>
        /// <returns>加密后的验证码</returns>
        private static String EncryptCode(String checkCode)
        {
            return PassWordEncrypt.Encrypt(CHECK_CODE_COOKIE_NAME, checkCode);
        }

        /// <summary>
        /// 获取验证码Cookie信息
        /// </summary>
        /// <returns>验证码Cookie信息</returns>
        private static HttpCookie GetCheckCodeCookie()
        {
            return Cookies.GetCookie(CHECK_CODE_COOKIE_NAME);
        }

        /// <summary>
        /// 删除验证码状态
        /// </summary>
        private static void RemoveCheckCode()
        {
            Cookies.Remove(CHECK_CODE_COOKIE_NAME);
        }
        #endregion
    }
}