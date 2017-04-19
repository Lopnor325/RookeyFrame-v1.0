/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.TempModel
{
    /// <summary>
    /// 树结点类
    /// </summary>
    public class TreeNode
    {
        public TreeNode()
        {
            state = "open";
        }

        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 节点文本
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string iconCls { get; set; }

        /// <summary>
        /// 节点状态，异步用到
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public IEnumerable<TreeNode> children { get; set; }

        /// <summary>
        /// 节点其他属性
        /// </summary>
        public TreeAttributes attribute { get; set; }

        /// <summary>
        /// 将其他树节点类型转换到TreeNode类型
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="obj">实体对象</param>
        /// <param name="parseAction">转换方法</param>
        /// <param name="childsProperty">子节点属性</param>
        /// <returns>转换后的节点信息</returns>
        public static TreeNode Parse<T>(T obj, Action<T, TreeNode> parseAction, Func<T, IEnumerable<T>> childsProperty)
        {
            if (obj == null) return null;
            TreeNode node = new TreeNode();
            parseAction(obj, node);
            if (childsProperty != null)
            {
                var childs = childsProperty(obj);
                if (childs != null && childs.Count() > 0)
                {
                    List<TreeNode> nodes = new List<TreeNode>();
                    foreach (var child in childs)
                    {
                        nodes.Add(Parse<T>(child, parseAction, childsProperty));
                    }
                    node.children = nodes;
                }
            }
            return node;
        }
    }

    /// <summary>
    /// 树属性
    /// </summary>
    public class TreeAttributes
    {
        /// <summary>
        /// URL
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 扩展对象
        /// </summary>
        public object obj { get; set; }
    }
}
