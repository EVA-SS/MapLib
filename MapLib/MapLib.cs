using System;

namespace MapLib
{
    /// <summary>
    /// 地图类
    /// </summary>
    public static partial class Map
    {
        public const double PI = 3.1415926535897932384626, PI180 = PI / 180, a = 6378245.0, ee = 0.00669342162296594323;

        /// <summary>
        /// 计算方位角
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <remarks>根据起点经纬度、终点经纬度计算它们之间的方位角</remarks>
        /// <returns>两点的方位角</returns>
        public static double Azimuth(this LngLat start, LngLat end) => Azimuth(start.lng, start.lat, end.lng, end.lat);

        /// <summary>
        /// 计算方位角
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <remarks>根据起点经纬度、终点经纬度计算它们之间的方位角</remarks>
        /// <returns>两点的方位角</returns>
        public static double Azimuth(this double[] start, double[] end) => Azimuth(start[0], start[1], end[0], end[1]);

        /// <summary>
        /// 计算方位角
        /// </summary>
        /// <param name="lng_start">起点 经度</param>
        /// <param name="lat_start">起点 纬度</param>
        /// <param name="lng_end">终点 经度</param>
        /// <param name="lat_end">终点 纬度</param>
        /// <remarks>根据起点经纬度、终点经纬度计算它们之间的方位角</remarks>
        /// <returns>两点的方位角</returns>
        public static double Azimuth(double lng_start, double lat_start, double lng_end, double lat_end)
        {
            double lng_start_rad = lng_start * PI180, lat_start_rad = lat_start * PI180, lng_end_rad = lng_end * PI180, lat_end_rad = lat_end * PI180;

            double y = Math.Sin(lng_end_rad - lng_start_rad) * Math.Cos(lat_end_rad), x = Math.Cos(lat_start_rad) * Math.Sin(lat_end_rad) - Math.Sin(lat_start_rad) * Math.Cos(lat_end_rad) * Math.Cos(lng_end_rad - lng_start_rad);
            var brng = Math.Atan2(y, x) * 180 / PI180;
            return (brng + 360.0) % 360.0;
        }

        /// <summary>
        /// 计算目标点
        /// </summary>
        /// <param name="current">当前经纬度</param>
        /// <param name="bearing">角度</param>
        /// <param name="m">里程（米）</param>
        /// <returns>通过目标点和角度得到经纬度</returns>
        public static LngLat Destination(this LngLat current, double bearing, double m) => Destination(current.lng, current.lat, bearing, m);

        /// <summary>
        /// 计算目标点
        /// </summary>
        /// <param name="current">当前经纬度</param>
        /// <param name="bearing">角度</param>
        /// <param name="m">里程（米）</param>
        /// <returns>通过目标点和角度得到经纬度</returns>
        public static LngLat Destination(this double[] current, double bearing, double m) => Destination(current[0], current[1], bearing, m);

        /// <summary>
        /// 计算目标点
        /// </summary>
        /// <param name="lng">当前经度</param>
        /// <param name="lat">当前纬度</param>
        /// <param name="bearing">角度</param>
        /// <param name="m">里程（米）</param>
        /// <returns>通过目标点和角度得到经纬度</returns>
        public static LngLat Destination(double lng, double lat, double bearing, double m)
        {
            var radius = 6371e3;
            double lng_rad = lng * PI180, lat_rad = lat * PI180;
            double brng = bearing * PI180;
            double _lat = Math.Asin(Math.Sin(lat_rad) * Math.Cos(m / radius) + Math.Cos(lat_rad) * Math.Sin(m / radius) * Math.Cos(brng)), _lng = lng_rad + Math.Atan2(Math.Sin(brng) * Math.Sin(m / radius) * Math.Cos(lat_rad), Math.Cos(m / radius) - Math.Sin(lat_rad) * Math.Sin(_lat));
            return new LngLat(_lng * (180 / PI180), _lat * (180 / PI180));
        }
    }
}