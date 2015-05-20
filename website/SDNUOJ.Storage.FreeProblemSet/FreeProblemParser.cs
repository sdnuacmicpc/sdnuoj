using System;
using System.Collections.Generic;

using SDNUOJ.Entity;
using SDNUOJ.Storage.Problem;

namespace SDNUOJ.Storage.FreeProblemSet
{
    /// <summary>
    /// Free Problem转换器
    /// </summary>
    public static class FreeProblemParser
    {
        /// <summary>
        /// 将Free Problem转换为Problem实体
        /// </summary>
        /// <param name="fp">Free Problem</param>
        /// <returns>Problem实体</returns>
        public static ProblemEntity ConvertFreeProblemToProblem(FreeProblem fp)
        {
            if (fp == null)
            {
                return null;
            }

            ProblemEntity problem = new ProblemEntity();

            problem.Title = fp.Title;
            problem.Description = fp.Description;
            problem.Input = fp.Input;
            problem.Output = fp.Output;
            problem.SampleInput = fp.SampleInput;
            problem.SampleOutput = fp.SampleOutput;
            problem.Hint = fp.Hint;
            problem.Source = fp.Source;
            problem.TimeLimit = fp.TimeLimit;
            problem.MemoryLimit = fp.MemoryLimit;

            return problem;
        }

        /// <summary>
        /// 将Free Problem Data转换为题目数据包
        /// </summary>
        /// <param name="fpd">Free Problem Data</param>
        /// <returns>题目数据包</returns>
        public static Byte[] ConvertFreeProblemDataToZipFile(List<FreeProblemDataPair> fpds)
        {
            if (fpds == null || fpds.Count < 1)
            {
                return null;
            }

            ProblemDataWriter writer = new ProblemDataWriter();

            for (Int32 i = 0; i < fpds.Count; i++)
            {
                writer.WriteData(fpds[i].Input, fpds[i].Output);
            }

            return writer.WriteTo();
        }

        /// <summary>
        /// 将Free Problem Image转换为图形文件数组
        /// </summary>
        /// <param name="images">Free Problem Image</param>
        /// <returns>图形文件数组</returns>
        public static Dictionary<String, Byte[]> ConvertFreeProblemImagesToBytes(List<FreeProblemImage> images)
        {
            if (images == null || images.Count < 1)
            {
                return null;
            }

            Dictionary<String, Byte[]> files = new Dictionary<String, Byte[]>();

            for (Int32 j = 0; j < images.Count; j++)
            {
                files.Add(images[j].SourceUrl, images[j].Content);
            }

            return files;
        }
    }
}