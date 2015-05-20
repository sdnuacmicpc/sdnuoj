using System;
using System.Collections.Generic;
using System.Web.UI;

namespace SDNUOJ.Utilities
{
    /// <summary>
    /// 树形节点
    /// </summary>
    public class TreeNode<T> : Control, INamingContainer
    {
        #region 字段
        private T _dataSource;
        private String _value;
        private Int32 _deepth;
        private IList<TreeNode<T>> _childNodes;
        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置数据绑定
        /// </summary>
        public T DataSource
        {
            get { return this._dataSource; }
            set { this._dataSource = value; }
        }

        /// <summary>
        /// 获取或设置节点值
        /// </summary>
        public String Value
        {
            get { return this._value; }
            set { this._value = value; }
        }

        /// <summary>
        /// 获取或设置节点深度
        /// </summary>
        public Int32 Deepth
        {
            get { return this._deepth; }
            set { this._deepth = value; }
        }

        /// <summary>
        /// 获取或设置子节点集合
        /// </summary>
        public IList<TreeNode<T>> ChildNodes
        {
            get { return this._childNodes; }
            set { this._childNodes = value; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化新的树形节点类
        /// </summary>
        /// <param name="value">节点值</param>
        public TreeNode(String value, T data)
        {
            this._childNodes = new List<TreeNode<T>>();
            this._value = value;
            this._dataSource = data;
        }

        /// <summary>
        /// 向子集合中增加新的节点
        /// </summary>
        /// <param name="node">子节点</param>
        public void AddNote(TreeNode<T> node)
        {
            node.Deepth = this._deepth + 1;
            this._childNodes.Add(node);
        }
        #endregion
    }
}