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
            const string startStr = "[Language=";
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

            StringBuilder sb = new StringBuilder();//去掉注释的待解析数据
            string[] dataArr = Regex.Split(data, "\r\n|\r|\n");
            foreach (string line in dataArr)
            {
                if (line.StartsWith("##") || String.IsNullOrEmpty(line))
                    continue;
                sb.Append(line + "\n");
            }
            data = sb.ToString();

            try
            {
                int startP = data.IndexOf(startStr);
                while (startP != -1)
                {
                    int langEndP = data.IndexOf("]", startP + startStr.Length);
                    if (langEndP == -1)
                        break;

                    int endP = data.IndexOf(startStr, langEndP);
                    if (endP == -1)
                        endP = data.Length - 1;

                    string nowLangName = data.Substring(startP + startStr.Length, langEndP - (startP + startStr.Length));
                    string nowLangData = data.Substring(langEndP + 1, endP - langEndP - 1);

                    string[] nowLangArr = nowLangData.Split('\n');
                    List<string> unsafeList = new List<string>();

                    foreach (string code in nowLangArr)
                    {
                        if (!String.IsNullOrEmpty(code))
                            unsafeList.Add(code);
                    }

                    if (!String.IsNullOrEmpty(nowLangName) && unsafeList.Count > 0)
                    {
                        unsafeCodeDic[nowLangName] = unsafeList;
                    }

                    startP = endP;
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
