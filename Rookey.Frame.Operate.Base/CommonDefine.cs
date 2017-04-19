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

namespace Rookey.Frame.Operate.Base
{
    /// <summary>
    /// 常用变量定义
    /// </summary>
    public static class CommonDefine
    {
        /// <summary>
        /// 基类字段集合
        /// </summary>
        public static readonly List<string> BaseEntityFields = new List<string>() { "CreateUserId", "ModifyUserId", "CreateUserName", "ModifyUserName", "CreateDate", "ModifyDate", "IsDeleted", "DeleteTime", "IsDraft", "OrgId", "AutoIncrmId" };

        /// <summary>
        /// 包含Id的基类字段集合
        /// </summary>
        public static readonly List<string> BaseEntityFieldsContainId = new List<string>() { "Id", "CreateUserId", "ModifyUserId", "CreateUserName", "ModifyUserName", "CreateDate", "ModifyDate", "IsDeleted", "DeleteTime", "IsDraft", "OrgId", "AutoIncrmId" };

        /// <summary>
        /// 不需要更新字段集合
        /// </summary>
        public static readonly List<string> NoUpdateFields = new List<string>() { "CreateUserId", "CreateUserName", "CreateDate", "IsDeleted", "DeleteTime", "IsDraft", "AutoIncrmId" };

        /// <summary>
        /// 网格通用按钮
        /// </summary>
        public static readonly List<string> GridCommonBtns = new List<string>() { "新增", "编辑", "删除", "查看", "导入", "导出", "复制", "批量编辑", "打印" };

        /// <summary>
        /// 加载外键Name字段的最小记录数
        /// </summary>
        public static readonly int MaxLoadForeignNameFieldsCount = 50;

        /// <summary>
        /// 无框架标识
        /// </summary>
        public static readonly string NoFrameFlag = "nfm";

        /// <summary>
        /// 多选Checkbox框真值
        /// </summary>
        public static readonly string MutiCheckboxTrueValue = "1";
    }
}
