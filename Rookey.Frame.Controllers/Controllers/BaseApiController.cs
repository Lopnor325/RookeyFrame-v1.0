/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Controllers.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace Rookey.Frame.Controllers
{
    /// <summary>
    /// Api控制器基类
    /// </summary>
    [PermissionFilter]
    public class BaseApiController : ApiController
    {
    }
}
