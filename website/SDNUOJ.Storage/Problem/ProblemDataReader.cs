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
        private ProblemData _data;
        #endregion

        #region 属性
        /// <summary>
        /// 获取最后修改日期
        /// </summary>
        public String LastModified
        {
            get { return (this._data != null ? this._data.LastModified : ProblemData.NO_PROBLEM_DATA_VERSION); }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 初始化新的题目数据读取器
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public ProblemDataReader(String filePath)
        {
            using (ZipFile zip = new ZipFile(filePath))
            {
                ZipEntry entry = zip[ProblemData.DATA_VERSION_FILENAME];
                String lastModified = this.ReadLastModified(entry);

                this._data = new ProblemData(lastModified);
            }
        }

        /// <summary>
        /// 初始化新的题目数据读取器
        /// </summary>
        /// <param name="stream">文件流</param>
        public ProblemDataReader(Stream stream)
        {
            using (ZipFile zip = ZipFile.Read(stream))
            {
                ZipEntry entry = zip[ProblemData.DATA_VERSION_FILENAME];
                String lastModified = this.ReadLastModified(entry);

                this._data = new ProblemData(lastModified);
            }
        }
        #endregion

        #region 私有方法
        private String ReadLastModified(ZipEntry entry)
        {
            if (entry == null)
            {
                return ProblemData.NO_PROBLEM_DATA_VERSION;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                entry.Extract(ms);
                ms.Seek(0, SeekOrigin.Begin);

                StreamReader reader = new StreamReader(ms, Encoding.UTF8);
                String lastModified = reader.ReadToEnd();

                reader.Close();
                reader.Dispose();

                ms.Close();

                return lastModified;
            }
        }
        #endregion

        #region 静态方法
        /// <summary>
        /// 检查题目数据文件是否正确
        /// </summary>
        /// <param name="stream">题目数据文件流</param>
        /// <returns>题目数据文件是否正确</returns>
        public static Boolean CheckProblemDataFile(Stream stream)
        {
            try
            {
                ProblemDataReader reader = new ProblemDataReader(stream);
                
                if (String.Equals(reader.LastModified, ProblemData.NO_PROBLEM_DATA_VERSION))
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}