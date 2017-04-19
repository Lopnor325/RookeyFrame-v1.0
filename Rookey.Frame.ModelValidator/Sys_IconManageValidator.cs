using FluentValidation;
using Rookey.Frame.Model.Sys;
using System;
using System.Linq.Expressions;

namespace Rookey.Frame.ModelValidator
{
    /// <summary>
    /// 图标管理验证
    /// </summary>
    public class Sys_IconManageValidator : AbstractValidator<Sys_IconManage>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Sys_IconManageValidator()
        {
            Expression<Func<Sys_IconManage, bool>> exp = x => !x.StyleClassName.StartsWith("icon-");
            RuleFor(x => x.StyleClassName).Length(5, 30).WithMessage("样式类名字符长度在【5】至【30】之间！");
        }
    }
}
