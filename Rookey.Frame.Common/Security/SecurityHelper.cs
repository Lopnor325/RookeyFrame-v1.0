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
using System.Security.Cryptography;
using System.Text;

namespace Rookey.Frame.Common
{
    public class SecurityHelper
    {
        /// <summary>
        /// 获取混淆码
        /// </summary>
        /// <returns></returns>
        public static string GenerateSalt()
        {
            byte[] data = new byte[0x10];
            new RNGCryptoServiceProvider().GetBytes(data);
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// 获取通过混淆码混淆后的密码
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="passwordFormat"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string EncodePassword(string pass, string salt)
        {
            // 将密码和salt值转换成字节形式并连接起来
            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] src = Convert.FromBase64String(salt);
            byte[] dst = new byte[src.Length + bytes.Length];
            Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);

            //使用SHA-256算法加密
            byte[] hashBytes = new System.Security.Cryptography.SHA256Managed().ComputeHash(dst);

            // 以字符串形式返回散列值
            return Convert.ToBase64String(hashBytes);
        }
    }
}
