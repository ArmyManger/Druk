using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Druk.Common
{
    /// <summary>
    /// 地图工具类
    /// </summary>
    public class DoMap
    {
        private const double EARTH_RADIUS = 6371.393;
        private static double Rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        /// <summary>
        /// 根据经纬度获取两点间距离，单位m
        /// </summary>
        /// <param name="lat1">第一点纬度值</param>
        /// <param name="lng1">第一点经度值</param>
        /// <param name="lat2">第二点纬度值</param>
        /// <param name="lng2">第二点经度值</param>
        /// <returns></returns>
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = Rad(lat1);
            double radLat2 = Rad(lat2);
            double a = radLat1 - radLat2;
            double b = Rad(lng1) - Rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10;
            return s;
        }


        //火星转bd
        public static Hashtable bd_encrypt(double gg_lat, double gg_lon)
        {
            double x = gg_lon, y = gg_lat;
            double z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * Math.PI);
            double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * Math.PI);
            Hashtable retMap = new Hashtable();
            retMap.Add("bdlat", z * Math.Sin(theta) + 0.006);
            retMap.Add("bdlng", z * Math.Cos(theta) + 0.0065);
            return retMap;
        }

        //bd转火星
        public static Hashtable bd_decrypt(double bd_lat, double bd_lon)
        {
            double x = bd_lon - 0.0065, y = bd_lat - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * Math.PI);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * Math.PI);
            Hashtable retMap = new Hashtable();
            retMap.Add("gdlat", z * Math.Sin(theta));
            retMap.Add("gdlng", z * Math.Cos(theta));
            return retMap;
        }

    }
}
