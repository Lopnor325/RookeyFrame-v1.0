/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System.ComponentModel;

namespace Rookey.Frame.Model.EnumSpace
{
    /// <summary>
    /// 表单类型
    /// </summary>
    public enum FormTypeEnum
    {
        /// <summary>
        /// 编辑表单
        /// </summary>
        [Description("编辑表单")]
        EditForm = 0,

        /// <summary>
        /// 查看表单
        /// </summary>
        [Description("查看表单")]
        ViewForm = 1
    }

    /// <summary>
    /// 验证类型
    /// </summary>
    public enum ValidateTypeEnum
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        No = 0,

        /// <summary>
        /// Eamil
        /// </summary>
        [Description("邮箱")]
        email = 1,
        /// <summary>
        /// URL
        /// </summary>
        [Description("网址")]
        url = 2,

        /// <summary>
        /// 整型
        /// </summary>
        [Description("整型")]
        intNum = 3,

        /// <summary>
        /// 浮点型
        /// </summary>
        [Description("浮点型")]
        floatNum = 4,

        /// <summary>
        /// 手机号码
        /// </summary>
        [Description("手机")]
        mobile = 5,

        /// <summary>
        /// QQ
        /// </summary>
        [Description("QQ")]
        qq = 6,

        /// <summary>
        /// 邮政编码
        /// </summary>
        [Description("邮政编码")]
        zip = 7,
        /// <summary>
        /// 电话号码
        /// </summary>
        [Description("电话")]
        phone = 8,

        /// <summary>
        /// 传真
        /// </summary>
        [Description("传真")]
        faxno = 9,

        /// <summary>
        /// 身份证
        /// </summary>
        [Description("身份证")]
        idCard = 10
    }

    /// <summary>
    /// 模块编辑模式
    /// </summary>
    public enum ModuleEditModeEnum
    {
        /// <summary>
        /// 自适应模式
        /// </summary>
        [Description("自适应模式")]
        None = 0,

        /// <summary>
        /// 标签页表单编辑模式
        /// </summary>
        [Description("标签页表单编辑")]
        TabFormEdit = 1,

        /// <summary>
        /// 弹出框表单编辑模式
        /// </summary>
        [Description("弹出框表单编辑")]
        PopFormEdit = 2,

        /// <summary>
        /// 网格行编辑模式
        /// </summary>
        [Description("列表行编辑")]
        GridRowEdit = 3,

        /// <summary>
        /// 网格行下方展开表单编辑
        /// </summary>
        [Description("列表行下方展开表单编辑")]
        GridRowBottomFormEdit = 4,

    }

    /// <summary>
    /// 视图类型
    /// </summary>
    public enum GridTypeEnum
    {
        /// <summary>
        /// 列表视图
        /// </summary>
        [Description("列表视图")]
        System = 0,

        /// <summary>
        /// 弹出框视图
        /// </summary>
        [Description("弹出框视图")]
        Dialog = 1,

        /// <summary>
        /// 自定义
        /// </summary>
        [Description("自定义视图")]
        Custom = 2,

        /// <summary>
        /// 综合视图，包含外键模块字段的视图
        /// </summary>
        [Description("综合视图")]
        Comprehensive = 3,

        /// <summary>
        /// 综合明细视图，包含明细模块字段的视图
        /// </summary>
        [Description("综合明细视图")]
        ComprehensiveDetail = 4,
    }

    /// <summary>
    /// 操作按钮类型
    /// </summary>
    public enum OperateButtonTypeEnum
    {
        /// <summary>
        /// 普通按钮
        /// </summary>
        [Description("普通按钮")]
        CommonButton = 0,

        /// <summary>
        /// 文件菜单按钮
        /// </summary>
        [Description("文件菜单按钮")]
        FileMenuButton = 1
    }

    /// <summary>
    /// 列表操作按钮位置
    /// </summary>
    public enum GridButtonLocationEnum
    {
        /// <summary>
        /// 列表工具栏
        /// </summary>
        [Description("列表工具栏")]
        Toolbar = 0,

        /// <summary>
        /// 行头
        /// </summary>
        [Description("行头")]
        RowHead = 1,
    }

    /// <summary>
    /// 按钮组摆放位置
    /// </summary>
    public enum ButtonLocationEnum
    {
        /// <summary>
        /// 顶部
        /// </summary>
        [Description("顶部")]
        Top = 0,

        /// <summary>
        /// 底部
        /// </summary>
        [Description("底部")]
        Bottom = 1,

        /// <summary>
        /// 顶部和底部
        /// </summary>
        [Description("顶部和底部")]
        TopBottom = 2
    }

    /// <summary>
    /// 登录类型
    /// </summary>
    public enum LoginTypeEnum
    {
        /// <summary>
        /// 手持设备
        /// </summary>
        [Description("手持设备")]
        HandheldDevices = 0,

        /// <summary>
        /// 电脑
        /// </summary>
        [Description("电脑")]
        Computer = 1
    }

    /// <summary>
    /// 接口调用类型
    /// </summary>
    public enum InterfaceCallTypeEnum
    {
        /// <summary>
        /// 系统调用外部接口
        /// </summary>
        [Description("系统调用外部接口")]
        CallExternalInterface = 0,

        /// <summary>
        /// 外接程序调用系统接口
        /// </summary>
        [Description("外接程序调用系统接口")]
        ExternalCallSystemInterface = 1
    }

    /// <summary>
    /// 表单附件显示方式
    /// </summary>
    public enum FormAttachDisplayStyleEnum
    {
        /// <summary>
        /// 简单方式
        /// </summary>
        [Description("简单方式")]
        SimpleStyle = 0,

        /// <summary>
        /// 列表方式
        /// </summary>
        [Description("列表方式")]
        GridStype = 1
    }

    /// <summary>
    /// 条件操作方法
    /// </summary>
    public enum ConditionMethodEnum
    {
        /// <summary>
        /// 等于
        /// </summary>
        [Description("等于")]
        Equal = 0,

        /// <summary>
        /// 小于
        /// </summary>
        [Description("小于")]
        LessThan = 1,

        /// <summary>
        /// 大于
        /// </summary>
        [Description("大于")]
        GreaterThan = 2,

        /// <summary>
        /// 小于等于
        /// </summary>
        [Description("小于等于")]
        LessThanOrEqual = 3,

        /// <summary>
        /// 大于等于
        /// </summary>
        [Description("大于等于")]
        GreaterThanOrEqual = 4,

        /// <summary>
        /// 不等于
        /// </summary>
        [Description("不等于")]
        NotEqual = 9,

        /// <summary>
        /// 包含
        /// </summary>
        [Description("包含")]
        Contains = 12,

        /// <summary>
        /// 开头为
        /// </summary>
        [Description("开头为")]
        StartsWith = 10,

        /// <summary>
        /// 结尾为
        /// </summary>
        [Description("结尾为")]
        EndsWith = 11,

        /// <summary>
        /// 包含于
        /// </summary>
        [Description("包含于")]
        In = 7
    }

    /// <summary>
    /// 对齐方式
    /// </summary>
    public enum AlignTypeEnum
    {
        /// <summary>
        /// 左对齐
        /// </summary>
        [Description("左对齐")]
        Left = 0,

        /// <summary>
        /// 右对齐
        /// </summary>
        [Description("右对齐")]
        Right = 1,

        /// <summary>
        /// 居中对齐
        /// </summary>
        [Description("居中对齐")]
        Center = 2,
    }

    /// <summary>
    /// 图标类型
    /// </summary>
    public enum IconTypeEnum
    {
        /// <summary>
        /// 16×16
        /// </summary>
        [Description("16×16")]
        Piex16 = 1,

        /// <summary>
        /// 32×32
        /// </summary>
        [Description("32×32")]
        Piex32 = 2,

        /// <summary>
        /// 64×64
        /// </summary>
        [Description("64×64")]
        Piex64 = 3,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 10
    }

    /// <summary>
    /// 图标分类
    /// </summary>
    public enum IconClassTypeEnum
    {
        /// <summary>
        /// 系统图标
        /// </summary>
        [Description("系统图标")]
        SystemIcon = 2,

        /// <summary>
        /// 自定义图标
        /// </summary>
        [Description("自定义图标")]
        CustomerIcon = 1,

        /// <summary>
        /// 用户上传
        /// </summary>
        [Description("用户上传")]
        UserUploadIcon = 0,
    }

    /// <summary>
    /// 模块数据来源类型
    /// </summary>
    public enum ModuleDataSourceType
    {
        /// <summary>
        /// 数据库表
        /// </summary>
        [Description("数据库表")]
        DbTable = 0,

        /// <summary>
        /// 接口
        /// </summary>
        [Description("接口")]
        InterfaceData = 1,

        /// <summary>
        /// Xml
        /// </summary>
        [Description("Xml")]
        XmlData = 2,

        /// <summary>
        /// 其它
        /// </summary>
        [Description("其它")]
        Other = 3,
    }

    /// <summary>
    /// 缓存类型
    /// </summary>
    public enum TempCacheProviderType
    {
        /// <summary>
        /// 本地缓存
        /// </summary>
        [Description("本地缓存")]
        LOCALMEMORYCACHE = 0,

        /// <summary>
        /// MemcachedCache分布式缓存
        /// </summary>
        [Description("Memcached分布式缓存")]
        MEMCACHEDCACHE = 1,

        /// <summary>
        /// redis分布式缓存
        /// </summary>
        [Description("Redis分布式缓存")]
        REDIS = 2
    }

    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum TempDatabaseType
    {
        /// <summary>
        /// 微软MsSqlServer数据库
        /// </summary>
        [Description("MsSqlServer数据库")]
        MsSqlServer = 0,

        /// <summary>
        /// MySql数据库
        /// </summary>
        [Description("MySql数据库")]
        MySql = 1,

        /// <summary>
        /// Oracle数据库
        /// </summary>
        [Description("Oracle数据库")]
        Oracle = 2
    }

    /// <summary>
    /// 日期格式
    /// </summary>
    public enum DateFormatEnum
    {
        /// <summary>
        /// 年的前两位
        /// </summary>
        [Description("yy")]
        yy = 0,

        /// <summary>
        /// 年
        /// </summary>
        [Description("yyyy")]
        yyyy = 1,

        /// <summary>
        /// 年前两位+月
        /// </summary>
        [Description("yyMM")]
        yyMM = 2,

        /// <summary>
        /// 年+月
        /// </summary>
        [Description("yyyyMM")]
        yyyyMM = 3,

        /// <summary>
        /// 年前两位+月+天
        /// </summary>
        [Description("yyMMdd")]
        yyMMdd = 4,

        /// <summary>
        /// 年+月+天
        /// </summary>
        [Description("yyyyMMdd")]
        yyyyMMdd = 5,

        /// <summary>
        /// 月+年前两位
        /// </summary>
        [Description("MMyy")]
        MMyy = 6,

        /// <summary>
        /// 月+年
        /// </summary>
        [Description("MMyyyy")]
        MMyyyy = 7,

        /// <summary>
        /// 年前两位-月
        /// </summary>
        [Description("yy-MM")]
        yy_MM = 8,

        /// <summary>
        /// 年-月
        /// </summary>
        [Description("yyyy-MM")]
        yyyy_MM = 9,

        /// <summary>
        /// 年前两位-月-天
        /// </summary>
        [Description("yy-MM-dd")]
        yy_MM_dd = 10,

        /// <summary>
        /// 年-月-天
        /// </summary>
        [Description("yyyy-MM-dd")]
        yyyy_MM_dd = 11,

        /// <summary>
        /// 月-年前两位
        /// </summary>
        [Description("MM-yy")]
        MM_yy = 12,

        /// <summary>
        /// 月-年
        /// </summary>
        [Description("MM-yyyy")]
        MM_yyyy = 13,

        /// <summary>
        /// 月-天-年前两位
        /// </summary>
        [Description("MM-dd-yy")]
        MM_dd_yy = 14,

        /// <summary>
        /// 月-天-年
        /// </summary>
        [Description("MM-dd-yyyy")]
        MM_dd_yyyy = 15,

        /// <summary>
        /// 月/天/年前两位
        /// </summary>
        [Description("MM/dd/yy")]
        MMSddSyy = 16,

        /// <summary>
        /// 月/天/年
        /// </summary>
        [Description("MM/dd/yyyy")]
        MMSddSyyyy = 17,

        /// <summary>
        /// 年前两位/月/天
        /// </summary>
        [Description("yy/MM/dd")]
        yySMMSdd = 18,

        /// <summary>
        /// 年/月/天
        /// </summary>
        [Description("yyyy/MM/dd")]
        yyyySMMSdd = 19,
    }

    /// <summary>
    /// 资源类型
    /// </summary>
    public enum ResourceTypeEnum
    {
        /// <summary>
        /// 功能菜单
        /// </summary>
        [Description("功能菜单")]
        Menu = 0,

        /// <summary>
        /// 网格按钮
        /// </summary>
        [Description("网格按钮")]
        GridButton = 1,

        /// <summary>
        /// 字段编辑
        /// </summary>
        [Description("字段编辑")]
        FieldEdit = 2,

        /// <summary>
        /// 字段查看
        /// </summary>
        [Description("字段查看")]
        FieldView = 3,

        /// <summary>
        /// 数据查看范围
        /// </summary>
        [Description("数据查看范围")]
        DataViewRange = 4,

        /// <summary>
        /// 数据编辑范围
        /// </summary>
        [Description("数据编辑范围")]
        DataEditRange = 5,

        /// <summary>
        /// 数据删除范围
        /// </summary>
        [Description("数据删除范围")]
        DataDelRange = 6,

        ///// <summary>
        ///// 表单按钮
        ///// </summary>
        //[Description("表单按钮")]
        //FormButton = 5
    }

    /// <summary>
    /// 功能类型
    /// </summary>
    public enum FunctionTypeEnum
    {
        /// <summary>
        /// 菜单
        /// </summary>
        [Description("菜单")]
        Menu = 0,

        /// <summary>
        /// 按钮
        /// </summary>
        [Description("按钮")]
        Button = 1,
    }

    /// <summary>
    /// 参数设定类型
    /// </summary>
    public enum SystemSetTypeEnum
    {
        /// <summary>
        /// 全局设置
        /// </summary>
        [Description("全局设置")]
        GlobalSet = 0,

        /// <summary>
        /// 用户个人设置
        /// </summary>
        [Description("用户个人设置")]
        UserSet = 1
    }

    /// <summary>
    /// 缓存页面类型
    /// </summary>
    public enum CachePageTypeEnum
    {
        /// <summary>
        /// 网格页面
        /// </summary>
        [Description("网格页面")]
        GridPage = 0,

        /// <summary>
        /// 编辑表单页面
        /// </summary>
        [Description("编辑表单页面")]
        EditForm = 1,

        /// <summary>
        /// 查看表单页面
        /// </summary>
        [Description("查看表单页面")]
        ViewForm = 2,

        /// <summary>
        /// 其他页面
        /// </summary>
        [Description("其他页面")]
        OtherPage = 3
    }
}
