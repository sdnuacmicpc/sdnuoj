using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Ionic.Zip;
using Ionic.Zlib;

using SDNUOJ.Entity;

namespace SDNUOJ.Controllers.Core.Exchange
{
    /// <summary>
    /// 提交代码导出类
    /// </summary>
    internal static class SolutionCodeExport
    {
        /// <summary>
        /// 将用户提交代码导出为Zip
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="ojName">OJ名称</param>
        /// <param name="solutions">提交结果</param>
        /// <returns>Zip文件</returns>
        public static Byte[] ExportSolutionAcceptedCodeToZip(String userName, String ojName, List<SolutionEntity> solutions)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                StringBuilder comment = new StringBuilder();
                comment.AppendFormat("The accpted codes of {0} in {1}.", userName, ojName).AppendLine().AppendLine();
                comment.AppendFormat("This file is created at {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                ZipFile file = new ZipFile();
                file.AlternateEncoding = Encoding.UTF8;
                file.AlternateEncodingUsage = ZipOption.AsNecessary;
                file.CompressionLevel = CompressionLevel.BestCompression;
                file.Comment = comment.ToString();

                if (solutions != null)
                {
                    for (Int32 i = 0; i < solutions.Count; i++)
                    {
                        String fileName = String.Format("P{0}(S{1}).{2}", solutions[i].ProblemID.ToString(), solutions[i].SolutionID.ToString(), String.IsNullOrEmpty(solutions[i].LanguageType.FileExtension) ? "txt" : solutions[i].LanguageType.FileExtension);
                        file.AddEntry(fileName, solutions[i].SourceCode, Encoding.UTF8);
                    }
                }

                file.Save(ms);
                return ms.ToArray();
            }
        }
    }
}