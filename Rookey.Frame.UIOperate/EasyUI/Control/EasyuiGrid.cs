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

namespace Rookey.Frame.UIOperate.Control
{
    /// <summary>
    /// Easyui网格
    /// </summary>
    public class EasyuiGrid
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public EasyuiGrid()
        {
            columns = new List<EasyuiGridField>();
            frozenColumns = new List<EasyuiGridField>();
            autoRowHeight = true;
            idField = "Id";
            method = "get";
            rownumbers = true;
            sortName = "Id";
            sortOrder = "desc";
        }

        /// <summary>
        /// 字段
        /// </summary>
        public List<EasyuiGridField> columns { get; set; }

        /// <summary>
        /// 冻结字段
        /// </summary>
        public List<EasyuiGridField> frozenColumns { get; set; }

        /// <summary>
        /// 宽度自适应
        /// </summary>
        public bool fitColumns { get; set; }

        /// <summary>
        /// 行高自适应，默认为true
        /// </summary>
        public bool autoRowHeight { get; set; }

        /// <summary>
        /// 工具栏
        /// </summary>
        public string toolbar { get; set; }

        /// <summary>
        /// 指明哪一个字段是标识字段，默认为Id
        /// </summary>
        public string idField { get; set; }

        /// <summary>
        /// 请求远程数据类型，默认为get
        /// </summary>
        public string method { get; set; }

        /// <summary>
        /// 数据加载url
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 是否分页
        /// </summary>
        public bool pagination { get; set; }

        /// <summary>
        /// 是否显示行号，默认显示
        /// </summary>
        public bool rownumbers { get; set; }

        /// <summary>
        /// 只允许选择一行，默认为true
        /// </summary>
        public bool singleSelect { get; set; }

        /// <summary>
        /// 排序字段，默认为Id
        /// </summary>
        public string sortName { get; set; }

        /// <summary>
        /// 排序方式，默认为desc
        /// </summary>
        public string sortOrder { get; set; }
    }

    /// <summary>
    /// Easyui网格字段
    /// </summary>
    public class EasyuiGridField
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// 字段显示名称
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 字段宽度
        /// </summary>
        public int width { get; set; }

        /// <summary>
        /// 是否显示复选框
        /// </summary>
        public bool checkbox { get; set; }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool hidden { get; set; }

        /// <summary>
        /// 格式化函数
        /// </summary>
        public string formatter { get; set; }

        /// <summary>
        /// 编辑器
        /// </summary>
        public string editor { set; get; }

        /// <summary>
        /// 标题对齐方式
        /// </summary>
        public string halign { get; set; }

        /// <summary>
        /// 数据对齐方式
        /// </summary>
        public string align { get; set; }
    }
}
