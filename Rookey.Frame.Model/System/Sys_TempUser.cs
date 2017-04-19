/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase;
using Rookey.Frame.EntityBase.Attr;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 临时用户
    /// </summary>
    [NoModule]
    public class Sys_TempUser : BaseSysEntity
    {
        public string FieldInfo1 { get; set; }

        public string FieldInfo2 { get; set; }
    }
}
