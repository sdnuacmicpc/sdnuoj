using System;
using System.Collections.Generic;
using System.Text;

using SDNUOJ.Configuration.Caching;

namespace SDNUOJ.Configuration
{
    /// <summary>
    /// 程序语言管理器
    /// </summary>
    public static class LanguageManager
    {
        #region 常量
        public const Byte NullLangID = Byte.MaxValue;
        public const Double DefaultScale = 1.0;
        #endregion

        #region 字段
        private static Int32 _languageSupportedCount = 0;

        private static Dictionary<String, Byte> _languageNamesAndIDs = null;
        private static Dictionary<String, Byte> _languageTypesAndIDs = null;
        private static Dictionary<Byte, String> _languageIDAndNames = null;
        private static Dictionary<Byte, String> _languageIDAndTypes = null;
        private static Dictionary<Byte, String> _languageIDAndExtensions = null;
        private static Dictionary<Byte, Double> _languageIDAndScale = null;

        private static Dictionary<String, Byte> _languageNamesAndIDsForMainSubmit = null;
        #endregion

        #region 属性
        /// <summary>
        /// 获取支持提交的语言个数
        /// </summary>
        public static Int32 LanguageSupportedCount
        {
            get { return _languageSupportedCount; }
        }

        /// <summary>
        /// 获取主提交支持语言
        /// </summary>
        public static Dictionary<String, Byte> MainSubmitSupportLanguages
        {
            get { return _languageNamesAndIDsForMainSubmit; }
        }

        /// <summary>
        /// 获取所有支持语言
        /// </summary>
        public static Dictionary<String, Byte> AllSupportLanguages
        {
            get { return _languageNamesAndIDs; }
        }
        #endregion

        #region 构造方法
        static LanguageManager()
        {
            if (String.IsNullOrEmpty(ConfigurationManager.SupportLanguages))
            {
                return;
            }

            _languageSupportedCount = 0;

            _languageNamesAndIDs = new Dictionary<String, Byte>();
            _languageTypesAndIDs = new Dictionary<String, Byte>();
            _languageIDAndNames = new Dictionary<Byte, String>();
            _languageIDAndTypes = new Dictionary<Byte, String>();
            _languageIDAndExtensions = new Dictionary<Byte, String>();
            _languageIDAndScale = new Dictionary<Byte, Double>();

            _languageNamesAndIDsForMainSubmit = new Dictionary<String, Byte>();

            String[] langs = ConfigurationManager.SupportLanguages.Split('|');

            for (Int32 i = 0; i < langs.Length; i++)
            {
                String[] lang = langs[i].Split('=');
                Byte langid = LanguageManager.NullLangID;
                Double scale = LanguageManager.DefaultScale;

                if (lang.Length == 6 && Byte.TryParse(lang[0], out langid) && Double.TryParse(lang[4], out scale))
                {
                    _languageNamesAndIDs.Add(lang[1], langid);
                    _languageIDAndNames.Add(langid, lang[1]);

                    _languageTypesAndIDs.Add(lang[2], langid);
                    _languageIDAndTypes.Add(langid, lang[2]);

                    _languageIDAndExtensions.Add(langid, lang[3]);
                    _languageIDAndScale.Add(langid, scale);

                    if ("true".Equals(lang[5], StringComparison.OrdinalIgnoreCase))
                    {
                        _languageNamesAndIDsForMainSubmit.Add(lang[1], langid);
                    }

                    _languageSupportedCount++;
                }
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获取全部语言名称
        /// </summary>
        /// <returns>全部语言名称</returns>
        public static String GetAllLanguageNames()
        {
            StringBuilder names = new StringBuilder();

            if (_languageNamesAndIDs != null)
            {
                foreach (String name in _languageNamesAndIDs.Keys)
                {
                    if (names.Length > 0)
                    {
                        names.Append("、");
                    }

                    names.Append(name);
                }
            }

            return names.ToString();
        }

        /// <summary>
        /// 获取是否存在指定程序语言ID
        /// </summary>
        /// <param name="langid">语言ID</param>
        /// <returns>是否存在</returns>
        public static Boolean ContainsLanguageID(Byte langid)
        {
            return _languageIDAndNames.ContainsKey(langid);
        }

        /// <summary>
        /// 获取指定程序语言的ID
        /// </summary>
        /// <param name="langType">语言类型</param>
        /// <returns>语言ID</returns>
        public static Byte GetLanguageID(String langType)
        {
            if (_languageTypesAndIDs.ContainsKey(langType))
            {
                return _languageTypesAndIDs[langType];
            }
            else
            {
                return LanguageManager.NullLangID;
            }
        }

        /// <summary>
        /// 获取指定程序语言的类型
        /// </summary>
        /// <param name="langID">语言ID</param>
        /// <returns>语言名称</returns>
        public static String GetLanguageName(Byte langid)
        {
            if (_languageIDAndNames.ContainsKey(langid))
            {
                return _languageIDAndNames[langid];
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 获取指定程序语言的类型
        /// </summary>
        /// <param name="langID">语言ID</param>
        /// <returns>语言类型</returns>
        public static String GetLanguageType(Byte langid)
        {
            if (_languageIDAndTypes.ContainsKey(langid))
            {
                return _languageIDAndTypes[langid];
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 获取指定程序语言的文件扩展名
        /// </summary>
        /// <param name="langID">语言ID</param>
        /// <returns>文件扩展名</returns>
        public static String GetLanguageFileExtension(Byte langid)
        {
            if (_languageIDAndExtensions.ContainsKey(langid))
            {
                return _languageIDAndExtensions[langid];
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 获取指定程序语言的加乘系数
        /// </summary>
        /// <param name="langID">语言ID</param>
        /// <returns>加乘系数</returns>
        public static Double GetLanguageScale(Byte langid)
        {
            if (_languageIDAndScale.ContainsKey(langid))
            {
                return _languageIDAndScale[langid];
            }
            else
            {
                return LanguageManager.DefaultScale;
            }
        }

        /// <summary>
        /// 根据过滤器获取支持的语言
        /// </summary>
        /// <param name="filter">过滤器</param>
        /// <returns>支持的语言</returns>
        public static Dictionary<String, Byte> GetSupportLanguages(String filter)
        {
            if (String.IsNullOrEmpty(filter))
            {
                return _languageNamesAndIDsForMainSubmit;
            }

            Dictionary<String, Byte> result = LanguageFilterCache.GetLanguageFilterResultCache(filter);

            if (result == null)
            {
                String[] filters = filter.Split(',');

                if (filters.Length < 1)
                {
                    return _languageNamesAndIDsForMainSubmit;
                }

                result = new Dictionary<String, Byte>();

                for (Int32 i = 0; i < filters.Length; i++)
                {
                    Byte id = 0;

                    if (_languageNamesAndIDs.TryGetValue(filters[i], out id) && !result.ContainsKey(filters[i]))
                    {
                        result.Add(filters[i], id);
                    }
                }

                LanguageFilterCache.SetLanguageFilterResultCache(filter, result);
            }

            return result;
        }
        #endregion
    }
}