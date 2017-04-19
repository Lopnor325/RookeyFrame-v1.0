using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.TempModel
{
    /// <summary>
    /// 表单工具标签按钮
    /// </summary>
    public class FormToolTag
    {
        /// <summary>
        /// 标签Id
        /// </summary>
        public string TagId { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 单击事件
        /// </summary>
        public string ClickMethod { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// tooltip提示
        /// </summary>
        public string Title { get; set; }
    }
}
