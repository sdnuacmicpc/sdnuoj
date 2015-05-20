using System;
using System.Collections.Generic;

namespace SDNUOJ.Configuration
{
    /// <summary>
    /// 日期管理器
    /// </summary>
    public static class DateManager
    {
        #region 字段
        private static List<Int32> _years = null;
        private static List<Int32> _months = null;
        private static DateTime _cacheDate;
        #endregion

        #region 属性
        /// <summary>
        /// 获取所有年份列表
        /// </summary>
        public static List<Int32> Years
        {
            get
            {
                if (DateTime.Today.Year != _cacheDate.Year || DateTime.Today.Month != _cacheDate.Month)
                {
                    Init();
                }

                return _years; 
            }
        }

        /// <summary>
        /// 获取所有月份列表
        /// </summary>
        public static List<Int32> Months
        {
            get
            {
                if (DateTime.Today.Year != _cacheDate.Year || DateTime.Today.Month != _cacheDate.Month)
                {
                    Init();
                }

                return _months;
            }
        }
        #endregion

        #region 构造方法
        static DateManager()
        {
            Init();
        }
        #endregion

        #region 方法
        private static void Init()
        {
            _years = new List<Int32>();
            for (Int32 i = 2012; i <= DateTime.Now.Year; i++)
            {
                _years.Add(i);
            }

            _months = new List<Int32>();
            for (Int32 i = 1; i <= 12; i++)
            {
                _months.Add(i);
            }

            _cacheDate = DateTime.Today;
        }
        #endregion
    }
}