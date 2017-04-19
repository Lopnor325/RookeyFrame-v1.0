using FluentValidation;
using Rookey.Frame.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.ModelValidator
{
    /// <summary>
    /// 菜单验证
    /// </summary>
    public class Sys_MenuValidator : AbstractValidator<Sys_Menu>
    {
        /// <summary>
        /// 菜单验证构造函数
        /// </summary>
        public Sys_MenuValidator()
        {
            //RuleFor(x => x.Name).NotEqual("test").WithMessage("菜单名称不能为test");
        }
    }
}
