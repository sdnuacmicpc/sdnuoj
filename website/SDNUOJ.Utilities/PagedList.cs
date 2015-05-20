using System;
using System.Collections;
using System.Collections.Generic;

namespace SDNUOJ.Utilities
{
    /// <summary>
    /// 支持分页的列表集合
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class PagedList<T> : IList<T>
    {
        #region 静态字段
        public readonly static PagedList<T> Empty = new PagedList<T>();
        #endregion

        #region 字段
        private List<T> _list;
        private Int32 _recordCount;
        private Int32 _pageSize;
        private Int32 _pageCount;
        #endregion

        #region 属性
        /// <summary>
        /// 获取记录数量
        /// </summary>
        public Int32 RecordCount
        {
            get { return this._recordCount; }
        }

        /// <summary>
        /// 获取页面大小
        /// </summary>
        public Int32 PageSize
        {
            get { return this._pageSize; }
        }

        /// <summary>
        /// 获取页面数量
        /// </summary>
        public Int32 PageCount
        {
            get { return this._pageCount; }
        }

        /// <summary>
        /// 获取当前页面记录数量
        /// </summary>
        public Int32 Count
        {
            get { return this._list.Count; }
        }

        /// <summary>
        /// 获取当前集合是否只读
        /// </summary>
        Boolean ICollection<T>.IsReadOnly
        {
            get { return ((ICollection<T>)this._list).IsReadOnly; }
        }
        #endregion

        #region 索引器
        /// <summary>
        /// 获取或设置指定索引的数据
        /// </summary>
        /// <param name="index">指定索引</param>
        /// <returns>指定索引的数据</returns>
        public T this[Int32 index]
        {
            get { return this._list[index];}
            set { this._list[index] = value; }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 初始化新的支持分页的列表集合
        /// </summary>
        /// <param name="list">当前页面集合</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录数量</param>
        public PagedList(List<T> list, Int32 pageSize, Int32 recordCount)
        {
            this._list = list ?? new List<T>();
            this._recordCount = recordCount;
            this._pageSize = pageSize;
            this._pageCount = (this._recordCount + this._pageSize - 1) / this._pageSize;
            this._pageCount = (this._recordCount < 1 ? 1 : this._pageCount);
        }

        /// <summary>
        /// 初始化空的支持分页的列表集合
        /// </summary>
        private PagedList()
        {
            this._list = new List<T>();
            this._recordCount = 0;
            this._pageSize = 0;
            this._pageCount = 1;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="item">指定数据</param>
        public void Add(T item)
        {
            this._list.Add(item);
        }

        /// <summary>
        /// 在指定索引处插入指定数据
        /// </summary>
        /// <param name="index">指定索引</param>
        /// <param name="item">指定数据</param>
        public void Insert(Int32 index, T item)
        {
            this._list.Insert(index, item);
        }

        /// <summary>
        /// 获取当前集合中是否包含指定数据
        /// </summary>
        /// <param name="item">指定数据</param>
        /// <returns>是否包含指定数据</returns>
        public Boolean Contains(T item)
        {
            return this._list.Contains(item);
        }

        /// <summary>
        /// 获取指定数据的索引
        /// </summary>
        /// <param name="item">指定数据</param>
        /// <returns>指定数据的索引</returns>
        public Int32 IndexOf(T item)
        {
            return this._list.IndexOf(item);
        }

        /// <summary>
        /// 删除指定索引处的数据
        /// </summary>
        /// <param name="index">指定索引</param>
        public void RemoveAt(Int32 index)
        {
            this._list.RemoveAt(index);
        }

        /// <summary>
        /// 删除指定数据
        /// </summary>
        /// <param name="item">指定数据</param>
        /// <returns>是否删除成功</returns>
        public Boolean Remove(T item)
        {
            return this._list.Remove(item);
        }

        /// <summary>
        /// 清空所有数据
        /// </summary>
        public void Clear()
        {
            this._list.Clear();
        }

        /// <summary>
        /// 将当前集合中的数据复制到指定数组中
        /// </summary>
        /// <param name="array">目标数组</param>
        /// <param name="arrayIndex">复制起始索引</param>
        public void CopyTo(T[] array, Int32 arrayIndex)
        {
            this._list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 获取当前集合的枚举器
        /// </summary>
        /// <returns>当前集合的枚举器</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this._list.GetEnumerator();
        }

        /// <summary>
        /// 获取当前集合的枚举器
        /// </summary>
        /// <returns>当前集合的枚举器</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this._list).GetEnumerator();
        }
        #endregion
    }

    public static class ListExtension
    {
        /// <summary>
        /// 从已有列表集合中创建支持分页的列表集合
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="list">已有集合</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="recordCount">记录数量</param>
        /// <returns>支持分页的列表集合</returns>
        public static PagedList<T> ToPagedList<T>(this List<T> list, Int32 pageSize, Int32 recordCount)
        {
            return new PagedList<T>(list, pageSize, recordCount);
        }
    }
}