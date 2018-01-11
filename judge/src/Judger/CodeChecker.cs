using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using JudgeClient.Definition;

namespace JudgeClient.Judger
{
    static class CodeChecker
    {
        private static Dictionary<string, List<string>> unsafeCodeDic = new Dictionary<string, List<string>>();
        private static bool inited = false;

        public static void InitChecker()
        {
            const string startStr = "LanguageStart:";
            const string langTypeStr = "LangType=";
            const string endStr = "LanguageEnd;";
            string data;
            try
            {
                StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "UnSafeCodeList.txt", Encoding.Default);
                data = sr.ReadToEnd();
                sr.Close();
            }
            catch
            {
                data = "";
            }

            try
            {
                int startP = data.IndexOf(startStr);
                while (startP != -1)
                {
                    int endP = data.IndexOf(endStr, startP + startStr.Length);
                    if (endP == -1)
                        break;

                    string nowLangData = data.Substring(startP + startStr.Length, endP - (startP + startStr.Length));
                    string[] nowLangArr = Regex.Split(nowLangData, "\r\n|\r|\n");
                    List<string> unsafeList = new List<string>();
                    string nowLangName = "";
                    foreach (string code in nowLangArr)
                    {
                        if (code.StartsWith("##") || String.IsNullOrEmpty(code))
                            continue;

                        if (code.StartsWith(langTypeStr))
                        {
                            nowLangName = code.Substring(langTypeStr.Length, code.Length - langTypeStr.Length);
                            continue;
                        }

                        unsafeList.Add(code);
                    }

                    if (!String.IsNullOrEmpty(nowLangName) && unsafeList.Count > 0)
                    {
                        unsafeCodeDic[nowLangName] = unsafeList;
                    }
                    startP = data.IndexOf(startStr, endP + endStr.Length);
                }
            }
            catch { }
            inited = true;
        }

        /// <summary>
        /// 检查不安全代码
        /// </summary>
        /// <param name="task"></param>
        /// <returns>true-通过检查, false-含有不安全代码</returns>
        public static bool CheckUnsafeCode(Task task)
        {
            if (!inited)
                InitChecker();

            string langKey = "";
            foreach(string key in unsafeCodeDic.Keys)
            {
                if (task.LanguageAndSpecial.IndexOf(key) != -1)
                {
                    langKey = key;
                    break;
                }
            }

            if (langKey == "")
                return true;

            List<string> unsafeCodeList = unsafeCodeDic[langKey];

            foreach (string nowusCode in unsafeCodeList)
            {
                if (Regex.IsMatch(task.SourceCode, nowusCode, RegexOptions.Singleline)) 
                {
                    return false;
                }
            }
            return true;
        }
    }
}
