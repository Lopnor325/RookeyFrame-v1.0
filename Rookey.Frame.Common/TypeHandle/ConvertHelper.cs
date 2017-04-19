/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;

namespace Rookey.Frame.Common
{
    /// <summary>
    /// 强制转化辅助类(无异常抛出)
    /// </summary>
    public static class ConvertHelper
    {
        #region 强制转化

        /// <summary>
        /// object转化为Bool类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool ObjToBool(this object obj)
        {
            bool flag;
            if (obj == null)
            {
                return false;
            }
            if (obj.Equals(DBNull.Value))
            {
                return false;
            }
            return (bool.TryParse(obj.ToString(), out flag) && flag);
        }

        /// <summary>
        /// object强制转化为DateTime类型(吃掉异常)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime? ObjToDateNull(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            try
            {
                return new DateTime?(Convert.ToDateTime(obj));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// byte强制转化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte ObjToByte(this object obj)
        {
            if (obj != null)
            {
                byte num;
                if (obj.Equals(DBNull.Value))
                {
                    return 0;
                }
                if (Byte.TryParse(obj.ToString(), out num))
                {
                    return num;
                }
            }
            return 0;
        }

        /// <summary>
        /// int强制转化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ObjToInt(this object obj)
        {
            if (obj != null)
            {
                int num;
                if (obj.Equals(DBNull.Value))
                {
                    return 0;
                }
                if (int.TryParse(obj.ToString(), out num))
                {
                    return num;
                }
            }
            return 0;
        }

        /// <summary>
        /// 强制转化为long
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long ObjToLong(this object obj)
        {
            if (obj != null)
            {
                long num;
                if (obj.Equals(DBNull.Value))
                {
                    return 0;
                }
                if (long.TryParse(obj.ToString(), out num))
                {
                    return num;
                }
            }
            return 0;
        }

        /// <summary>
        /// 强制转化可空int类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int? ObjToIntNull(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj.Equals(DBNull.Value))
            {
                return null;
            }
            return new int?(ObjToInt(obj));
        }

        /// <summary>
        /// 强制转化可空long类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long? ObjToLongNull(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj.Equals(DBNull.Value))
            {
                return null;
            }
            return new long?(ObjToLong(obj));
        }

        /// <summary>
        /// 强制转化为string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjToStr(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            if (obj.Equals(DBNull.Value))
            {
                return string.Empty;
            }
            return Convert.ToString(obj);
        }

        /// <summary>
        /// double强制转化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double ObjToDouble(this object obj)
        {
            if (obj != null)
            {
                double num;
                if (obj.Equals(DBNull.Value))
                {
                    return 0;
                }
                if (double.TryParse(obj.ToString(), out num))
                {
                    return num;
                }
            }
            return 0;
        }

        /// <summary>
        /// Decimal转化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal ObjToDecimal(this object obj)
        {
            if (obj == null)
            {
                return 0M;
            }
            if (obj.Equals(DBNull.Value))
            {
                return 0M;
            }
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch
            {
                return 0M;
            }
        }

        /// <summary>
        /// Decimal可空类型转化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal? ObjToDecimalNull(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj.Equals(DBNull.Value))
            {
                return null;
            }
            return new decimal?(ObjToDecimal(obj));
        }

        /// <summary>
        /// 强制转化为Guid
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Guid ObjToGuid(this object obj)
        {
            if (obj == null)
            {
                return Guid.Empty;
            }
            if (obj.Equals(DBNull.Value))
            {
                return Guid.Empty;
            }
            try
            {
                return new Guid(obj.ToString());
            }
            catch
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// 强制转化为Guid?
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Guid? ObjToGuidNull(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj.Equals(DBNull.Value))
            {
                return null;
            }
            return new Guid?(ObjToGuid(obj));
        }

        /// <summary>
        /// 强制转化为DateTime?
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime? ObjToDateTimeNull(this object obj)
        {
            if (obj != null)
            {
                DateTime dt;
                if (obj.Equals(DBNull.Value))
                {
                    return null;
                }
                if (DateTime.TryParse(obj.ToString(), out dt))
                {
                    return dt;
                }
            }
            return null;
        }

        #endregion
    }
}
