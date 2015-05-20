using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SDNUOJ.Utilities.Net
{
    /// <summary>
    /// 电子邮件客户端
    /// </summary>
    public static class MailClient
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailServer">SMTP服务器地址</param>
        /// <param name="mailFrom">发件人邮箱</param>
        /// <param name="mailTo">收件人信箱</param>
        /// <param name="mailSubject">信件主题</param>
        /// <param name="mailBody">信件内容</param>
        /// <param name="isBodyHtml">信件内容是否是Html</param>
        /// <param name="isHighPriority">是否高优先级</param>
        /// <param name="userName">SMTP登录用户名</param>
        /// <param name="userPass">SMTP登录密码</param>
        public static async Task SendMail(String mailServer, String mailFrom, String mailTo, String mailSubject, String mailBody, Boolean isBodyHtml, Boolean isHighPriority, String userName, String userPass)
        {
            using (MailMessage message = new MailMessage(mailFrom, mailTo, mailSubject, mailBody))
            {
                message.IsBodyHtml = isBodyHtml;
                message.Priority = (isHighPriority ? MailPriority.High : MailPriority.Normal);

                SmtpClient client = new SmtpClient(mailServer);
                client.Credentials = new NetworkCredential(userName, userPass);
                await client.SendMailAsync(message);
            }
        }
    }
}