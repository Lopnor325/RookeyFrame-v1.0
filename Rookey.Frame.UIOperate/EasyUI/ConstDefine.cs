/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Base;
using Rookey.Frame.Base.Set;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.UIOperate
{
    /// <summary>
    /// 常量定义类
    /// </summary>
    internal static class ConstDefine
    {
        /// <summary>
        /// 默认皮肤样式为扁平灰
        /// </summary>
        public const string DEFAULT_THEME = "metro";

        /// <summary>
        /// 滚动条宽度
        /// </summary>
        public static int SCROLL_WIDTH = 16;

        /// <summary>
        /// 北方布局高度（LOGO高度）
        /// </summary>
        public static int TOP_NORTH_REGION_HEIGHT = GlobalSet.IsHorizontalMenu ? 65 : 60;

        /// <summary>
        /// TAB头的高度
        /// </summary>
        public static int TAB_HEAD_HEIGHT = 26;

        /// <summary>
        /// panel标题栏高度
        /// </summary>
        public const int PANEL_HEAD_HEIGHT = 28;

        /// <summary>
        /// 底部状态栏区域高度
        /// </summary>
        public const int BOTTOM_STATUS_REGON_HEIGHT = 25;

        /// <summary>
        /// 表单控件高度
        /// </summary>
        public const int FORM_CONTROL_HEIGHT = 24;

        /// <summary>
        /// 文本域表单控件高度
        /// </summary>
        public const int FORM_TEXTAREA_CONTROL_HEIGHT = 45;

        /// <summary>
        /// 表单行高
        /// </summary>
        public const int FORM_ROW_HEIGHT = 30;

        /// <summary>
        /// 文本域表单行高
        /// </summary>
        public const int FORM_TEXTAREA_ROW_HEIGHT = 50;

        /// <summary>
        /// 表单panel的padding
        /// </summary>
        public const int FORM_PANEL_PADDING = 10;

        /// <summary>
        /// 弹出框表单最大宽度限制
        /// </summary>
        public static int DIALOG_FORM_MAX_WIDTH = 900;

        /// <summary>
        /// 弹出框表单最大高度限制
        /// </summary>
        public static int DIALOG_FORM_MAX_HEIGHT = 500;

        /// <summary>
        /// 弹出框表单最小宽度
        /// </summary>
        public const int DIALOG_FORM_MIN_WIDTH = 400;

        /// <summary>
        /// 弹出框表单最小高度
        /// </summary>
        public const int DIALOG_FORM_MIN_HEIGHT = 250;

        /// <summary>
        /// 弹出框底部工具栏高度
        /// </summary>
        public const int DIALOG_TOOLBAR_HEIGHT = 36;

        /// <summary>
        /// 弹出框边框宽度
        /// </summary>
        public const int DIALOG_BORDER_WIDTH = 8;

        /// <summary>
        /// 主界面左侧菜单默认宽度
        /// </summary>
        public static int MAIN_LEFT_MENU_WIDTH = GlobalSet.IsHorizontalMenu ? 0 : 180;

        /// <summary>
        /// 网格左侧菜单默认宽度
        /// </summary>
        public const int GRID_LEFT_MENU_WIDTH = 180;

        /// <summary>
        /// 网格工具栏高度
        /// </summary>
        public const int GRID_TOOLBAR_HEIGHT = 36;

        /// <summary>
        /// 网格高度修正值
        /// </summary>
        public const int GRID_FIX_HEIGHT = 4;

        /// <summary>
        /// 标准lable宽度
        /// </summary>
        public const int STANDARD_LABEL = 100;

        /// <summary>
        /// 标准控件宽度
        /// </summary>
        public const int STANDARD_CONTROL = 180;

        /// <summary>
        /// 标准标签加标准控件总宽度
        /// </summary>
        public const int STANDARD_INPUTWIDTH = 300;
    }
}
