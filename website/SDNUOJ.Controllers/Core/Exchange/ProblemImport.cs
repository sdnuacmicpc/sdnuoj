using System;
using System.Collections.Generic;

using SDNUOJ.Entity;
using SDNUOJ.Storage.FreeProblemSet;

namespace SDNUOJ.Controllers.Core.Exchange
{
    /// <summary>
    /// 题目导入类
    /// </summary>
    internal static class ProblemImport
    {
        /// <summary>
        /// 尝试将Free Problem Set导出为题目实体和题目数据包
        /// </summary>
        /// <param name="fps">Free Problem Set</param>
        /// <param name="problems">题目实体</param>
        /// <param name="datas">题目数据包</param>
        /// <param name="images">图形文件列表</param>
        /// <returns>是否导入成功</returns>
        public static Boolean TryImportFreeProblemSet(FreeProblemSet fps, out List<ProblemEntity> problems, out List<Byte[]> datas, out List<Dictionary<String, Byte[]>> images)
        {
            if (fps == null || fps.Count < 1)
            {
                problems = null;
                datas = null;
                images = null;

                return false;
            }

            problems = new List<ProblemEntity>();
            datas = new List<Byte[]>();
            images = new List<Dictionary<String, Byte[]>>();

            for (Int32 i = 0; i < fps.Count; i++)
            {
                ProblemEntity problem = FreeProblemParser.ConvertFreeProblemToProblem(fps[i]);
                Byte[] data = FreeProblemParser.ConvertFreeProblemDataToZipFile(fps[i].TestData);
                Dictionary<String, Byte[]> fpimages = FreeProblemParser.ConvertFreeProblemImagesToBytes(fps[i].Images);

                problems.Add(problem);
                datas.Add(data);
                images.Add(fpimages);
            }

            return true;
        }

        /// <summary>
        /// 尝试将Free Problem Set导出为题目实体和题目数据包
        /// </summary>
        /// <param name="fps">Free Problem Set</param>
        /// <param name="problems">题目实体</param>
        /// <param name="datas">题目数据包</param>
        /// <param name="images">图形文件列表</param>
        /// <returns>是否导入成功</returns>
        public static Boolean TryImportFreeProblemSet(String fps, out List<ProblemEntity> problems, out List<Byte[]> datas, out List<Dictionary<String, Byte[]>> images)
        {
            try
            {
                FreeProblemSet set = new FreeProblemSet(fps);

                return TryImportFreeProblemSet(set, out problems, out datas, out images);
            }
            catch
            {
                problems = null;
                datas = null;
                images = null;

                return false;
            }
        }
    }
}