using System;
using System.Web;

using SDNUOJ.Utilities.Text.RegularExpressions;

namespace SDNUOJ.Utilities.Web
{
    /// <summary>
    /// Http上下文扩展类
    /// </summary>
    public static class HttpRequestExtension
    {
        /// <summary>
        /// 获取用户IPv4地址
        /// </summary>
        /// <param name="request">Http请求</param>
        /// <returns>IPv4地址</returns>
        public static String GetRemoteClientIPv4(this HttpRequestBase request)
        {
            if (request == null)
            {
                return String.Empty;
            }

            String ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (RegexVerify.IsIPv4(ip))
            {
                if (ip.IndexOf(':') >= 0)//有端口的情况
                {
                    ip = ip.Substring(0, ip.IndexOf(':'));
                }

                return ip;//如果只有一个IP则返回
            }

            if (String.IsNullOrEmpty(ip))
            {
                ip = request.ServerVariables["REMOTE_ADDR"];
            }

            if (String.IsNullOrEmpty(ip))
            {
                ip = request.UserHostAddress;
            }

            if (!String.IsNullOrEmpty(ip) && ip.IndexOf(',') >= 0)//有代理的情况
            {
                String[] ips = ip.Replace(" ", "").Replace("'", "").Split(new Char[] { ',', ';' });

                for (Int32 i = 0; i < ips.Length; i++)
                {
                    if ((RegexVerify.IsIPv4(ips[i])) && (ips[i].Substring(0, 3) != "10.") && (ips[i].Substring(0, 7) != "192.168") && (ips[i].Substring(0, 7) != "172.16."))
                    {
                        ip = ips[i];//获取不是内网的地址
                        break;
                    }
                }
            }

            if (String.Equals(ip, "::1"))
            {
                ip = "127.0.0.1";
            }

            if (!String.IsNullOrEmpty(ip) && ip.IndexOf(':') >= 0)//有端口的情况
            {
                ip = ip.Substring(0, ip.IndexOf(':'));
            }

            return ip;
        }
    }
}