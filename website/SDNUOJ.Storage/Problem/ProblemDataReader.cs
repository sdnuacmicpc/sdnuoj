using System;
using System.IO;
using System.Text;

using Ionic.Zip;

namespace SDNUOJ.Storage.Problem
{
    /// <summary>
    /// 题目数据读取器
    /// </summary>
    public class ProblemDataReader
    {
        #region 字段
        private String _filePath;
        private ProblemData _data;
        #endregion

        #region 构造方法
        /// <summary>
        /// 初始化新的题目数据读取器
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public ProblemDataReader(String filePath)
        {
            this._filePath = filePath;

            String lastMod = ProblemData.NO_PROBLEM_DATA_VERSION;

            using (ZipFile zip = new ZipFile(this._filePath))
            {
                ZipEntry entry = zip[ProblemData.DATA_VERSION_FILENAME];

                if (entry != null)
                {
                    MemoryStream ms = new MemoryStream();
                    entry.Extract(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    StreamReader reader = new StreamReader(ms, Encoding.UTF8);
                    lastMod = reader.ReadToEnd();

                    reader.Close();
                    reader.Dispose();

                    ms.Close();
                    ms.Dispose();
                }
            }

            this._data = new ProblemData(lastMod);
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获取最后修改日期
        /// </summary>
        /// <returns>最后修改日期</returns>
        public String ReadLastModified()
        {
            return (this._data != null ? this._data.LastModified : ProblemData.NO_PROBLEM_DATA_VERSION);
        }
        #endregion
    }
}