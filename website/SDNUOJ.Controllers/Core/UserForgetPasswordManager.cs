using System;
using System.Text;
using System.Threading.Tasks;

using SDNUOJ.Controllers.Exception;
using SDNUOJ.Controllers.Status;
using SDNUOJ.Configuration;
using SDNUOJ.Data;
using SDNUOJ.Entity;
using SDNUOJ.Utilities.Net;
using SDNUOJ.Utilities.Security;
using SDNUOJ.Utilities.Text.RegularExpressions;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 用户找回密码数据管理类
    /// </summary>
    internal static class UserForgetPasswordManager
    {
        #region 用户方法
        /// <summary>
        /// 申请找回密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="email">电子邮箱</param>
        /// <param name="userip">用户IP</param>
        /// <param name="checkCode">验证码</param>
        /// <param name="link">找回密码链接</param>
        /// <returns>是否可以申请</returns>
        public static async Task<IMethodResult> RequestResetUserPassword(String userName, String email, String userip, String checkCode, String link)
        {
            if (!CheckCodeStatus.VerifyCheckCode(checkCode))
            {
                return MethodResult.Failed("The verification code you input didn't match the picture, Please try again!");
            }

            if (!RegexVerify.IsUserName(userName))
            {
                return MethodResult.InvalidRequest(RequestType.User);
            }

            if (!RegexVerify.IsEmail(email))
            {
                return MethodResult.Failed("Email address is INVALID!");
            }

            UserEntity user = UserManager.InternalGetUserByNameAndEmail(userName, email);

            if (user == null)
            {
                return MethodResult.Failed("The username \"{0}\" doesn't exist or the email is wrong!", userName);
            }

            if (user.IsLocked)
            {
                return MethodResult.Failed("The user is locked, please contact the administrator!");
            }

            if (String.IsNullOrEmpty(user.Email) || "NULL".Equals(user.Email, StringComparison.OrdinalIgnoreCase))
            {
                return MethodResult.Failed("The user has no email, please contact the administrator!");
            }

            Random rand = new Random(DateTime.Now.Millisecond);

            UserForgetPasswordEntity ufp = new UserForgetPasswordEntity()
            {
                UserName = userName,
                SubmitDate = DateTime.Now,
                SubmitIP = userip,
                HashKey = MD5Encrypt.EncryptToHexString(String.Format("{0}-{1}-{2}", userName, DateTime.Now.Ticks.ToString(), rand.Next(DateTime.Now.Millisecond)), true)
            };

            Boolean success = UserForgetPasswordRepository.Instance.InsertEntity(ufp) > 0;

            if (!success)
            {
                return MethodResult.Failed("Failed to process your request!");
            }

            String url = ConfigurationManager.DomainUrl + ((link[0] == '/') ? link.Substring(1) : link);
            String mailSubject = ConfigurationManager.OnlineJudgeName + " Password Recovery";
            String mailContent = UserForgetPasswordManager.GetMailContent(userName, url + ufp.HashKey.ToLowerInvariant());

            try
            {
                await MailClient.SendMailAsync(ConfigurationManager.EmailSMTPServer, ConfigurationManager.EmailAddresser, email, mailSubject, mailContent, true, true, ConfigurationManager.EmailUsername, ConfigurationManager.EmailPassword);
            }
            catch
            {
                return MethodResult.Failed("Failed to send a password reset link to your email address.");
            }

            return MethodResult.SuccessAndLog("User forget password, name = {0}", userName);
        }

        /// <summary>
        /// 获取用户忘记密码请求
        /// </summary>
        /// <param name="hashKey">哈希KEY</param>
        /// <returns>是否可以重置密码</returns>
        public static UserForgetPasswordEntity GetUserForgetRequest(String hashKey)
        {
            if (!SQLValidator.IsNonNullANDSafe(hashKey))
            {
                throw new InvalidRequstException(RequestType.UserForgetPassword);
            }

            UserForgetPasswordEntity entity = UserForgetPasswordRepository.Instance.GetEntity(hashKey);

            if (entity == null)
            {
                throw new NullResponseException(RequestType.UserForgetPassword);
            }

            if (!String.IsNullOrEmpty(entity.AccessIP))
            {
                throw new InvalidRequstException(RequestType.UserForgetPassword);
            }

            TimeSpan ts = DateTime.Now - entity.SubmitDate;

            if (ts.TotalHours >= 24)
            {
                throw new InvalidRequstException(RequestType.UserForgetPassword);
            }

            return entity;
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="hashKey">哈希KEY</param>
        /// <param name="password">新密码</param>
        /// <param name="password2">确认新密码</param>
        /// <param name="userip">用户IP</param>
        /// <returns>是否成功重置密码</returns>
        public static IMethodResult ResetUserPassword(String hashKey, String userName, String password, String password2, String userip)
        {
            if (String.IsNullOrEmpty(password))
            {
                return MethodResult.Failed("Password can not be NULL!");
            }

            if (!String.Equals(password, password2, StringComparison.OrdinalIgnoreCase))
            {
                return MethodResult.Failed("Two passwords are not match!");
            }

            UserForgetPasswordEntity request = UserForgetPasswordManager.GetUserForgetRequest(hashKey);
            String realUserName = request.UserName;

            if (!String.Equals(userName, realUserName, StringComparison.OrdinalIgnoreCase))
            {
                return MethodResult.Failed("Username is INVALID!");
            }

            Boolean success = UserManager.InternalResetUserPassword(userName, password);

            if (!success)
            {
                return MethodResult.Failed("Failed to reset your password!");
            }

            DateTime accessDate = DateTime.Now;
            String accessIP = userip;
            UserForgetPasswordRepository.Instance.UpdateEntityStatus(hashKey, accessDate, accessIP);

            return MethodResult.SuccessAndLog("User reset password, name = {0}, rid = {1}", userName, hashKey);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取邮件正文
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="url">找回密码地址</param>
        /// <returns>邮件正文</returns>
        private static String GetMailContent(String userName, String url)
        {
            StringBuilder mailContent = new StringBuilder();
            mailContent.Append("Dear ").Append(userName).AppendLine(":");
            mailContent.AppendLine("To initiate the password reset process for your account, click the link below:");
            mailContent.AppendLine();
            mailContent.AppendFormat("<a href=\"{0}\">{0}<a/>", url).AppendLine();
            mailContent.AppendLine();
            mailContent.AppendLine("If clicking the link above doesn't work, please copy and paste the URL in a new browser window instead.");
            mailContent.AppendLine();
            mailContent.AppendLine("If you've received this mail in error, it's likely that another user entered your email address by mistake while trying to reset a password. If you didn't initiate the request, you don't need to take any further action and can safely disregard this email.");
            mailContent.AppendLine();
            mailContent.AppendLine("Sincerely,");
            mailContent.Append("The ").Append(ConfigurationManager.OnlineJudgeName).AppendLine(" Team");
            mailContent.AppendLine();
            mailContent.AppendLine("Note: This email address cannot accept replies. If you have any questions, please contact administrators.");

            mailContent.Replace(Environment.NewLine, "<br/>");

            return mailContent.ToString();
        }
        #endregion
    }
}