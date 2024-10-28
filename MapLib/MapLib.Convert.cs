using System;
using System.Collections.Generic;

namespace MapLib
{
    /// <summary>
    /// 地图类
    /// </summary>
    static partial class Map
    {
        /// <summary>
        /// 线转区域
        /// </summary>
        /// <param name="line">线</param>
        /// <param name="range">范围米</param>
        /// <returns>返回区域</returns>
        public static double[][] LineToRegion(this double[][] line, double range = 10)
        {
            List<double[]> result = new List<double[]>(),
                result_l = new List<double[]>(),
                result_r = new List<double[]>();
            double[]? old = null;
            for (int i = 1; i < line.Length; i++)
            {
                double[] lineold = line[i - 1], linenew = line[i];
                if (old == null || (old[0] != linenew[0] || old[1] != linenew[1]))
                {
                    old = linenew;
                    LngLat _old = new LngLat(lineold), _new = new LngLat(linenew);
                    var angle = Azimuth(_old, _new);
                    result_l.Add(Destination(_old, angle - 90, range).ToDouble());
                    result_r.Insert(0, Destination(_old, angle + 90, range).ToDouble());

                    if (i == line.Length - 1)
                    {
                        result_l.Add(Destination(_new, angle - 90, range).ToDouble());
                        result_r.Insert(0, Destination(_new, angle + 90, range).ToDouble());
                    }
                }
            }

            result.AddRange(result_l);
            result.AddRange(result_r);
            return result.ToArray();
        }

        #region 坐标转换

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
            double radLat = lat / 180.0 * PI;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * PI);
            dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * PI);
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
            double z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * PI);
            double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * PI);
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
        public static LngLat BD09_To_GCJ02(this LngLat lnglat) => BD09_To_GCJ02(lnglat.lng, lnglat.lat);

        /// <summary>
        /// 百度坐标 转 火星坐标
        /// </summary>
        /// <param name="lnglat">经纬度</param>
        /// <returns>BD-09 转 GCJ-02</returns>
        public static LngLat BD09_To_GCJ02(this double[] lnglat) => BD09_To_GCJ02(lnglat[0], lnglat[1]);

        /// <summary>
        /// 百度坐标 转 火星坐标
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns>BD-09 转 GCJ-02</returns>
        public static LngLat BD09_To_GCJ02(double lng, double lat)
        {
            double x = lng - 0.0065, y = lat - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * PI);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * PI);
            double gg_lon = z * Math.Cos(theta), gg_lat = z * Math.Sin(theta);
            return new LngLat(gg_lon, gg_lat);
        }

        #endregion

        #region 百度坐标 转 世界坐标

        /// <summary>
        /// 百度坐标 转 世界坐标
        /// </summary>
        /// <param name="lnglat">经纬度</param>
        /// <returns>BD-09 转 WGS84</returns>
        public static LngLat BD09_To_WGS84(this LngLat lnglat) => BD09_To_WGS84(lnglat.lng, lnglat.lat);

        /// <summary>
        /// 百度坐标 转 世界坐标
        /// </summary>
        /// <param name="lnglat">经纬度</param>
        /// <returns>BD-09 转 WGS84</returns>
        public static LngLat BD09_To_WGS84(this double[] lnglat) => BD09_To_WGS84(lnglat[0], lnglat[1]);

        /// <summary>
        /// 百度坐标 转 世界坐标
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns>BD-09 转 WGS84</returns>
        public static LngLat BD09_To_WGS84(double lng, double lat) => GCJ02_To_WGS84(BD09_To_GCJ02(lng, lat));

        #endregion

        /// <summary>
        /// 是否在中国
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        public static bool IsInChina(double lng, double lat)
        {
            if ((lng < 72.004 || lng > 137.8347) || (lat < 0.8293 || lat > 55.8271)) return true;
            return false;
        }

        static LngLat transform(double lng, double lat)
        {
            double dLat = transformLat(lng - 105.0, lat - 35.0);
            double dLon = transformLng(lng - 105.0, lat - 35.0);
            double radLat = lat / 180.0 * PI;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * PI);
            dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * PI);
            double mgLat = lat + dLat;
            double mgLon = lng + dLon;
            return new LngLat(mgLon, mgLat);
        }

        static double transformLat(double x, double y)
        {
            double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * PI) + 20.0 * Math.Sin(2.0 * x * PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(y * PI) + 40.0 * Math.Sin(y / 3.0 * PI)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(y / 12.0 * PI) + 320 * Math.Sin(y * PI / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        static double transformLng(double x, double y)
        {
            double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * PI) + 20.0 * Math.Sin(2.0 * x * PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(x * PI) + 40.0 * Math.Sin(x / 3.0 * PI)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(x / 12.0 * PI) + 300.0 * Math.Sin(x / 30.0 * PI)) * 2.0 / 3.0;
            return ret;
        }

        #endregion
    }
}