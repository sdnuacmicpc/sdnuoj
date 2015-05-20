using System;
using System.IO;
using System.Text;

using Ionic.Zip;

namespace SDNUOJ.Storage.Problem
{
    /// <summary>
    /// 题目数据写入器
    /// </summary>
    public class ProblemDataWriter
    {
        #region 字段
        private ProblemData _data;
        #endregion

        #region 构造方法
        /// <summary>
        /// 初始化新的题目数据写入器
        /// </summary>
        public ProblemDataWriter()
        {
            this._data = new ProblemData();
        }

        /// <summary>
        /// 初始化新的题目数据写入器
        /// </summary>
        /// <param name="data">题目数据</param>
        public ProblemDataWriter(ProblemData data)
        {
            this._data = data;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="input">输入数据</param>
        /// <param name="output">输出数据</param>
        public void WriteData(String input, String output)
        {
            this._data.AddData(new ProblemDataPair(input, output));
        }

        /// <summary>
        /// 写入题目数据
        /// </summary>
        /// <returns></returns>
        public Byte[] WriteTo()
        {
            MemoryStream ms = new MemoryStream();
            ZipFile zip = new ZipFile();
            zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;

            for (Int32 i = 0; i < this._data.DataCount; i++)
            {
                zip.AddEntry(String.Format("{0}/{1}.in", ProblemData.INPUT_DATA_DIRECTORY, (i + 1).ToString()), this._data[i].Input, Encoding.UTF8);
                zip.AddEntry(String.Format("{0}/{1}.out", ProblemData.OUTPUT_DATA_DIRECTORY, (i + 1).ToString()), this._data[i].Output, Encoding.UTF8);
            }

            zip.AddEntry(ProblemData.DATA_VERSION_FILENAME, this._data.LastModified, Encoding.UTF8);
            zip.Save(ms);

            return ms.ToArray();
        }
        #endregion
    }
}