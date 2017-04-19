/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Operate.Base.EnumDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.TempModel
{
    /// <summary>
    /// 表单按钮
    /// </summary>
    public class FormButton
    {
        /// <summary>
        /// 标签Id
        /// </summary>
        public string TagId { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// 按钮图标类型
        /// </summary>
        public ButtonIconType IconType { get; set; }

        /// <summary>
        /// 按钮调用方法
        /// </summary>
        public string ClickMethod { get; set; }

        /// <summary>
        /// 按钮图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 父待办ID，针对子流程
        /// </summary>
        public string ParentToDoId { get; set; }
    }
}
