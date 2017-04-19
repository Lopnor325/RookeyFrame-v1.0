using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.TempModel
{
    /// <summary>
    /// 权限保存时用此类反序列化
    /// </summary>
    public class PermissionModel
    {
        /// <summary>
        /// 模块Id
        /// </summary>
        public Guid? ModuleId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 允许浏览时的模块对应菜单Id
        /// </summary>
        public Guid? CanOpMeuId { get; set; }

        /// <summary>
        /// 允许操作的按钮Id集合
        /// </summary>
        public List<Guid> CanOpBtnIds { get; set; }

        /// <summary>
        /// 允许浏览数据范围的组织Id集合
        /// </summary>
        public List<string> CanViewDataOrgIds { get; set; }

        /// <summary>
        /// 允许编辑数据范围的组织Id集合
        /// </summary>
        public List<string> CanEditDataOrgIds { get; set; }

        /// <summary>
        /// 允许删除数据范围的组织Id集合
        /// </summary>
        public List<string> CanDelDataOrgIds { get; set; }

        /// <summary>
        /// 允许查看的字段集合
        /// </summary>
        public List<string> CanViewFields { get; set; }

        /// <summary>
        /// 允许新增的字段集合
        /// </summary>
        public List<string> CanAddFields { get; set; }

        /// <summary>
        /// 允许编辑的字段集合
        /// </summary>
        public List<string> CanEditFields { get; set; }
    }
}
