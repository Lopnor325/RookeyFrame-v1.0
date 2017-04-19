using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.EntityBase.Attr
{
    /// <summary>
    /// 非字段标识，属性加上该标识后将不会添加到字段表中
    /// </summary>
    public class NoFieldAttribute : Attribute
    {
    }
}
