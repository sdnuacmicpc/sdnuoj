using System;

using SDNUOJ.Configuration;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 程序语言类型
    /// </summary>
    [Serializable]
    public struct LanguageType
    {
        #region 常量
        public static readonly LanguageType Null = new LanguageType(LanguageManager.NullLangID);
        #endregion

        #region 字段
        private Byte _langID;
        #endregion

        #region 属性
        /// <summary>
        /// 获取程序语言代码
        /// </summary>
        public Byte ID
        {
            get { return this._langID; }
        }

        /// <summary>
        /// 获取程序语言名称
        /// </summary>
        public String Name
        {
            get { return LanguageManager.GetLanguageName(this._langID); }
        }

        /// <summary>
        /// 获取程序语言类型
        /// </summary>
        public String Type
        {
            get { return LanguageManager.GetLanguageType(this._langID); }
        }

        /// <summary>
        /// 获取程序语言文件扩展名
        /// </summary>
        public String FileExtension
        {
            get { return LanguageManager.GetLanguageFileExtension(this._langID); }
        }

        /// <summary>
        /// 获取程序语言加乘
        /// </summary>
        public Double Scale
        {
            get { return LanguageManager.GetLanguageScale(this._langID); }
        }
        #endregion

        #region 方法
        private LanguageType(Byte langID)
        {
            this._langID = langID;
        }

        public override Boolean Equals(Object obj)
        {
            try
            {
                LanguageType type = (LanguageType)obj;

                return (this._langID == type._langID);
            }
            catch
            {
                return false;
            }
        }

        public override Int32 GetHashCode()
        {
            return this._langID.GetHashCode();
        }

        /// <summary>
        /// 获取程序语言名称
        /// </summary>
        /// <returns>程序语言名称</returns>
        public override String ToString()
        {
            return this.Name;
        }
        #endregion

        #region 静态方法
        /// <summary>
        /// 从语言ID中创建语言类型
        /// </summary>
        /// <param name="langID">语言ID</param>
        /// <returns>语言类型</returns>
        public static LanguageType FromLanguageID(Byte langID)
        {
            LanguageType langType = new LanguageType(langID);

            return langType;
        }

        /// <summary>
        /// 从语言ID中创建语言类型
        /// </summary>
        /// <param name="langID">语言ID</param>
        /// <returns>语言类型</returns>
        public static LanguageType FromLanguageID(String langID)
        {
            Byte type = 0;

            if (!Byte.TryParse(langID, out type))
            {
                throw new InvalidCastException("Language ID is INVALID!");
            }

            return LanguageType.FromLanguageID(type);
        }

        /// <summary>
        /// 从语言ID中创建语言类型
        /// </summary>
        /// <param name="type">语言类型</param>
        /// <returns>语言类型</returns>
        public static LanguageType FromLanguagType(String type)
        {
            Byte langID = LanguageManager.GetLanguageID(type);
            LanguageType langType = new LanguageType(langID);

            return langType;
        }

        /// <summary>
        /// 获取程序语言类型是否为空
        /// </summary>
        /// <param name="type">程序语言类型</param>
        /// <returns>是否为空</returns>
        public static Boolean IsNull(LanguageType type)
        {
            return !LanguageManager.ContainsLanguageID(type._langID);
        }

        public static Boolean operator ==(LanguageType type1, LanguageType type2)
        {
            return type1.Equals(type2);
        }

        public static Boolean operator !=(LanguageType type1, LanguageType type2)
        {
            return !type1.Equals(type2);
        }
        #endregion
    }
}