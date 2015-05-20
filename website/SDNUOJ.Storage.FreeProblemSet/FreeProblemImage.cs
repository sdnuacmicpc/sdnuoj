using System;

namespace SDNUOJ.Storage.FreeProblemSet
{
    /// <summary>
    /// Free Problem Image
    /// </summary>
    public class FreeProblemImage
    {
        #region 字段
        private String _src;
        private String _base64;
        #endregion

        #region 属性
        /// <summary>
        /// 获取图像来源URL
        /// </summary>
        public String SourceUrl
        {
            get { return this._src; }
        }

        /// <summary>
        /// 获取图像Base64内容
        /// </summary>
        public String Base64Content
        {
            get { return this._base64; }
        }

        /// <summary>
        /// 获取图像二进制内容数组
        /// </summary>
        public Byte[] Content
        {
            get { return Convert.FromBase64String(this._base64); }
        }
        #endregion

        #region 构造方法
        public FreeProblemImage(String src, String base64)
        {
            this._src = src;
            this._base64 = base64;
        }
        #endregion
    }
}