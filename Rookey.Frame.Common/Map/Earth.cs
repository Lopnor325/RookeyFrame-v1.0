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
    public static class Earth
    {
        /// <summary>  
        /// 地球的半径（公里）
        /// </summary>  
        public const double EARTH_RADIUS = 6378.137;

        /// <summary>  
        /// 计算经纬度坐标点的距离  
        /// </summary>  
        /// <param name="begin">开始的经度纬度</param>  
        /// <param name="end">结束的经度纬度</param>  
        /// <returns>距离(公里)</returns>  
        public static double GetDistance(LongLatPoint begin, LongLatPoint end)
        {
            double lat = begin.RadLat - end.RadLat;
            double lng = begin.RadLng - end.RadLng;

            double dis = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(lat / 2), 2) + Math.Cos(begin.RadLat) * Math.Cos(end.RadLat) * Math.Pow(Math.Sin(lng / 2), 2)));
            dis = dis * EARTH_RADIUS;
            dis = Math.Round(dis * 1e4) / 1e4;

            return dis;
        }
    }

    /// <summary>  
    /// 代表经度, 纬度  
    /// </summary>  
    public class LongLatPoint
    {
        /// <param name="lat">纬度 X</param>  
        /// <param name="lng">经度 Y</param>  
        public LongLatPoint(double lat, double lng)
        {
            this.lat = lat;
            this.lng = lng;
        }

        //  纬度 X  
        private double lat;

        // 经度 Y  
        private double lng;

        /// <summary>  
        /// 代表纬度 X轴  
        /// </summary>  
        public double Lat 
        {
            get { return this.lat; }
        }

        /// <summary>  
        /// 代表经度 Y轴  
        /// </summary>  
        public double Lng
        {
            get { return this.lng; }
        }

        public double RadLat { get { return lat * Math.PI / 180; } }

        public double RadLng { get { return lng * Math.PI / 180; } }
    }  
}
