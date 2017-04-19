/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System.ComponentModel;

namespace Rookey.Frame.EntityBase
{
    /// <summary>
    /// 表单控件类型
    /// </summary>
    public enum ControlTypeEnum
    {
        /// <summary>
        /// 文本框
        /// </summary>
        [Description("文本框")]
        TextBox = 0,

        /// <summary>
        /// 单选CheckBox
        /// </summary>
        [Description("单选CheckBox")]
        SingleCheckBox = 1,

        /// <summary>
        /// 多选CheckBox
        /// </summary>
        [Description("多选CheckBox")]
        MutiCheckBox = 2,

        /// <summary>
        /// 下拉列表框
        /// </summary>
        [Description("下拉列表")]
        ComboBox = 3,

        /// <summary>
        /// 下拉弹出列表框
        /// </summary>
        [Description("下拉弹出列表")]
        ComboGrid = 4,

        /// <summary>
        /// 下拉树
        /// </summary>
        [Description("下拉树")]
        ComboTree = 5,

        /// <summary>
        /// 浮点数值输入框
        /// </summary>
        [Description("浮点数值")]
        NumberBox = 6,

        /// <summary>
        /// 整型数值输入框
        /// </summary>
        [Description("整型数值")]
        IntegerBox = 7,

        /// <summary>
        /// 弹出列表框
        /// </summary>
        [Description("弹出列表框")]
        DialogGrid = 8,

        /// <summary>
        /// 单选框组
        /// </summary>
        [Description("单选框组")]
        RadioList = 9,

        /// <summary>
        /// 日期输入框
        /// </summary>
        [Description("日期")]
        DateBox = 10,

        /// <summary>
        /// 日期时间
        /// </summary>
        [Description("日期时间")]
        DateTimeBox = 11,

        /// <summary>
        /// 文本域
        /// </summary>
        [Description("文本域")]
        TextAreaBox = 12,

        /// <summary>
        /// 富文本框
        /// </summary>
        [Description("富文本框")]
        RichTextBox = 13,

        /// <summary>
        /// 密码输入框
        /// </summary>
        [Description("密码输入框")]
        PasswordBox = 15,

        /// <summary>
        /// 图标控件
        /// </summary>
        [Description("图标控件")]
        IconBox = 16,

        /// <summary>
        /// 弹出树控件
        /// </summary>
        [Description("弹出树控件")]
        DialogTree = 17,

        /// <summary>
        /// 文件上传
        /// </summary>
        [Description("文件上传")]
        FileUpload = 25,

        /// <summary>
        /// 图片上传控件
        /// </summary>
        [Description("图片上传")]
        ImageUpload = 26,

        /// <summary>
        /// 隐藏控件
        /// </summary>
        [Description("隐藏控件")]
        HiddenBox = 30,

        /// <summary>
        /// 显示标签
        /// </summary>
        [Description("显示标签")]
        LabelBox = 100,
    }
}
