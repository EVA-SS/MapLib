//------------------------------------------------------------------------------
//  此代码版权（除特别声明或在MapLib命名空间的代码）归作者本人Tom所有
//  源代码使用协议遵循本仓库的开源协议及附加协议，若本仓库没有设置，则按MIT开源协议授权
//  Github源代码仓库：https://github.com/Haku-Men
//  Gitee源代码仓库：https://gitee.com/HakuMen
//  QQ：17379620
//  参考公式：http://www.movable-type.co.uk/scripts/latlong.html
//  感谢您的下载和使用
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MapLib
{
    /// <summary>
    /// 地图类
    /// </summary>
    public static partial class Map
    {
        /// <summary>
        /// 线转区域
        /// </summary>
        /// <param name="line">线</param>
        /// <param name="range">范围米</param>
        /// <returns>返回区域</returns>
        public static double[][] LineToRegion(this double[][] line, double range = 10)
        {
            List<double[]> result = new List<double[]>(), result_l = new List<double[]>(), result_r = new List<double[]>();
            for (int i = 0; i < line.Length; i++)
            {
                if (i == line.Length - 1)
                {
                    double[] lineold = line[i - 1], linenew = line[i];
                    LngLat _old = new LngLat(lineold), _new = new LngLat(linenew);
                    var angle = Azimuth(_old, _new);
                    result_l.Add(Destination(_new, angle - 90, range).ToDouble());
                    result_r.Insert(0, Destination(_new, angle + 90, range).ToDouble());
                }
                else
                {
                    double[] lineold = line[i], linenew = line[i + 1];
                    LngLat _old = new LngLat(lineold), _new = new LngLat(linenew);
                    var angle = Azimuth(_old, _new);
                    result_l.Add(Destination(_old, angle - 90, range).ToDouble());
                    result_r.Insert(0, Destination(_old, angle + 90, range).ToDouble());
                }
            }
            result.AddRange(result_l);
            result.AddRange(result_r);
            return result.ToArray();
        }

        #region 坐标转换

        static double pi = 3.1415926535897932384626;
        static double a = 6378245.0;
        static double ee = 0.00669342162296594323;

        #region 世界坐标 转 火星坐标

        /// <summary>
        /// 世界坐标 转 火星坐标
        /// </summary>
        /// <param name="lnglat">经纬度</param>
        /// <returns>WGS84 转 GCJ-02</returns>
        public static LngLat WGS84_To_GCJ02(this LngLat lnglat)
        {
            return WGS84_To_GCJ02(lnglat.lng, lnglat.lat);
        }

        /// <summary>
        /// 世界坐标 转 火星坐标
        /// </summary>
        /// <param name="lnglat">经纬度</param>
        /// <returns>WGS84 转 GCJ-02</returns>
        public static LngLat WGS84_To_GCJ02(this double[] lnglat)
        {
            return WGS84_To_GCJ02(lnglat[0], lnglat[1]);
        }

        /// <summary>
        /// 世界坐标 转 火星坐标
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns>WGS84 转 GCJ-02</returns>
        public static LngLat WGS84_To_GCJ02(double lng, double lat)
        {
            double dLat = transformLat(lng - 105.0, lat - 35.0);
            double dLon = transformLng(lng - 105.0, lat - 35.0);
            double radLat = lat / 180.0 * pi;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * pi);
            dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * pi);
            double mgLat = lat + dLat;
            double mgLon = lng + dLon;
            return new LngLat(mgLon, mgLat);
        }

        #endregion

        #region 火星坐标 转 世界坐标

        /// <summary>
        /// 火星坐标 转 世界坐标
        /// </summary>
        /// <param name="lnglat">经纬度</param>
        /// <returns>GCJ-02 转 WGS84</returns>
        public static LngLat GCJ02_To_WGS84(this LngLat lnglat)
        {
            return GCJ02_To_WGS84(lnglat.lng, lnglat.lat);
        }

        /// <summary>
        /// 火星坐标 转 世界坐标
        /// </summary>
        /// <param name="lnglat">经纬度</param>
        /// <returns>GCJ-02 转 WGS84</returns>
        public static LngLat GCJ02_To_WGS84(this double[] lnglat)
        {
            return GCJ02_To_WGS84(lnglat[0], lnglat[1]);
        }

        /// <summary>
        /// 火星坐标 转 世界坐标
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns>GCJ-02 转 WGS84</returns>
        public static LngLat GCJ02_To_WGS84(double lng, double lat)
        {
            LngLat gps = transform(lng, lat);
            return new LngLat(lng * 2 - gps.lng, lat * 2 - gps.lat);
        }

        #endregion

        #region 火星坐标 转 百度坐标

        /// <summary>
        /// 火星坐标 转 百度坐标
        /// </summary>
        /// <param name="lnglat">经纬度</param>
        /// <returns>GCJ-02 转 BD-09</returns>
        public static LngLat GCJ02_To_BD09(this LngLat lnglat)
        {
            return GCJ02_To_BD09(lnglat.lng, lnglat.lat);
        }

        /// <summary>
        /// 火星坐标 转 百度坐标
        /// </summary>
        /// <param name="lnglat">经纬度</param>
        /// <returns>GCJ-02 转 BD-09</returns>
        public static LngLat GCJ02_To_BD09(this double[] lnglat)
        {
            return GCJ02_To_BD09(lnglat[0], lnglat[1]);
        }

        /// <summary>
        /// 火星坐标 转 百度坐标
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns>GCJ-02 转 BD-09</returns>
        public static LngLat GCJ02_To_BD09(double lng, double lat)
        {
            double x = lng, y = lat;
            double z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * pi);
            double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * pi);
            double bd_lon = z * Math.Cos(theta) + 0.0065;
            double bd_lat = z * Math.Sin(theta) + 0.006;
            return new LngLat(bd_lon, bd_lat);
        }

        #endregion

        #region 百度坐标 转 火星坐标

        /// <summary>
        /// 百度坐标 转 火星坐标
        /// </summary>
        /// <param name="lnglat">经纬度</param>
        /// <returns>BD-09 转 GCJ-02</returns>
        public static LngLat BD09_To_GCJ02(this LngLat lnglat)
        {
            return BD09_To_GCJ02(lnglat.lng, lnglat.lat);
        }

        /// <summary>
        /// 百度坐标 转 火星坐标
        /// </summary>
        /// <param name="lnglat">经纬度</param>
        /// <returns>BD-09 转 GCJ-02</returns>
        public static LngLat BD09_To_GCJ02(this double[] lnglat)
        {
            return BD09_To_GCJ02(lnglat[0], lnglat[1]);
        }
        /// <summary>
        /// 百度坐标 转 火星坐标
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns>BD-09 转 GCJ-02</returns>
        public static LngLat BD09_To_GCJ02(double lng, double lat)
        {
            double x = lng - 0.0065, y = lat - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * pi);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * pi);
            double gg_lon = z * Math.Cos(theta);
            double gg_lat = z * Math.Sin(theta);
            return new LngLat(gg_lon, gg_lat);
        }

        #endregion

        #region 百度坐标 转 世界坐标

        /// <summary>
        /// 百度坐标 转 世界坐标
        /// </summary>
        /// <param name="lnglat">经纬度</param>
        /// <returns>BD-09 转 WGS84</returns>
        public static LngLat BD09_To_WGS84(this LngLat lnglat)
        {
            return BD09_To_WGS84(lnglat.lng, lnglat.lat);
        }

        /// <summary>
        /// 百度坐标 转 世界坐标
        /// </summary>
        /// <param name="lnglat">经纬度</param>
        /// <returns>BD-09 转 WGS84</returns>
        public static LngLat BD09_To_WGS84(this double[] lnglat)
        {
            return BD09_To_WGS84(lnglat[0], lnglat[1]);
        }

        /// <summary>
        /// 百度坐标 转 世界坐标
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns>BD-09 转 WGS84</returns>
        public static LngLat BD09_To_WGS84(double lng, double lat)
        {
            LngLat gcj02 = BD09_To_GCJ02(lng, lat);
            LngLat map84 = GCJ02_To_WGS84(gcj02.lng, gcj02.lat);
            return map84;

        }

        #endregion

        /// <summary>
        /// 是否在中国
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        public static bool IsInChina(double lng, double lat)
        {
            if ((lng < 72.004 || lng > 137.8347) ||(lat < 0.8293 || lat > 55.8271))
                return true;
            return false;
        }

        static LngLat transform(double lng, double lat)
        {
            double dLat = transformLat(lng - 105.0, lat - 35.0);
            double dLon = transformLng(lng - 105.0, lat - 35.0);
            double radLat = lat / 180.0 * pi;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * pi);
            dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * pi);
            double mgLat = lat + dLat;
            double mgLon = lng + dLon;
            return new LngLat(mgLon, mgLat);
        }

        static double transformLat(double x, double y)
        {
            double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y
                    + 0.2 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(y * pi) + 40.0 * Math.Sin(y / 3.0 * pi)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(y / 12.0 * pi) + 320 * Math.Sin(y * pi / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        static double transformLng(double x, double y)
        {
            double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1
                    * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(x * pi) + 40.0 * Math.Sin(x / 3.0 * pi)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(x / 12.0 * pi) + 300.0 * Math.Sin(x / 30.0
                    * pi)) * 2.0 / 3.0;
            return ret;
        }

        #endregion
    }
}
