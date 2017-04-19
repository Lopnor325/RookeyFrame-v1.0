/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Common;
using Rookey.Frame.Controllers.Attr;
using System.Web.Mvc;

namespace Rookey.Frame.Controllers
{
    /// <summary>
    /// 安全处理控制器
    /// </summary>
    public class SecurityController : BaseController
    {
        /// <summary>
        /// 验证码标记字符串
        /// </summary>
        public const string VALIDATECODE = "ValidateCode";

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Anonymous]
        public FileResult ValidateCode()
        {
            ValidateCodeHelper validateCode = new ValidateCodeHelper();
            string code = validateCode.GetRandomNumberString(4);
            TempData[VALIDATECODE] = code;
            byte[] bytes = validateCode.CreateImage(code);
            return File(bytes, @"image/jpeg");
        }
    }
}
