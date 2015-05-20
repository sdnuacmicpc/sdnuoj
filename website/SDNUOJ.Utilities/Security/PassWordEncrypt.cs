using System;

namespace SDNUOJ.Utilities.Security
{
    /// <summary>
    /// 用户密码加密类
    /// </summary>
    public static class PassWordEncrypt
    {
        private const String SALT = "__SDNU ACM-ICPC__";

        /// <summary>
        /// 对指定用户名和密码进行加密
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">原始密码</param>
        /// <returns>加密后的密码</returns>
        public static String Encrypt(String userName, String passWord)
        {
            String temp = MD5Encrypt.EncryptToHexString(userName + passWord, true) + SALT;

            return MD5Encrypt.EncryptToHexString(temp, true);
        }
    }
}