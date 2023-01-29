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

namespace MapLib
{
    /// <summary>
    /// 地图类
    /// </summary>
    public static partial class Map
    {
        /// <summary>
        /// 计算方位角
        /// </summary>
        /// <param name="lnglat_start">起始点</param>
        /// <param name="lnglat_end">目标点</param>
        /// <remarks>根据起始点经纬度、目标点经纬度计算它们之间的方位角</remarks>
        /// <returns>方位角</returns>
        public static double Azimuth(this LngLat lnglat_start, LngLat lnglat_end)
        {
            return Azimuth(lnglat_start.lng, lnglat_start.lat, lnglat_end.lng, lnglat_end.lat);
        }

        /// <summary>
        /// 计算方位角
        /// </summary>
        /// <param name="lnglat_start">起始点</param>
        /// <param name="lnglat_end">目标点</param>
        /// <remarks>根据起始点经纬度、目标点经纬度计算它们之间的方位角</remarks>
        /// <returns>方位角</returns>
        public static double Azimuth(this double[] lnglat_start, double[] lnglat_end)
        {
            return Azimuth(lnglat_start[0], lnglat_start[1], lnglat_end[0], lnglat_end[1]);
        }

        /// <summary>
        /// 计算方位角
        /// </summary>
        /// <param name="lng_start">起始 经度</param>
        /// <param name="lat_start">起始 纬度</param>
        /// <param name="lng_end">目标 经度</param>
        /// <param name="lat_end">目标 纬度</param>
        /// <remarks>根据起始点经纬度、目标点经纬度计算它们之间的方位角</remarks>
        /// <returns>方位角</returns>
        public static double Azimuth(double lng_start, double lat_start, double lng_end, double lat_end)
        {
            double pi = Math.PI / 180;
            double lng_start_rad = lng_start * pi, lat_start_rad = lat_start * pi, lng_end_rad = lng_end * pi, lat_end_rad = lat_end * pi;

            var y = Math.Sin(lng_end_rad - lng_start_rad) * Math.Cos(lat_end_rad);
            var x = Math.Cos(lat_start_rad) * Math.Sin(lat_end_rad) -
                   Math.Sin(lat_start_rad) * Math.Cos(lat_end_rad) * Math.Cos(lng_end_rad - lng_start_rad);
            var brng = Math.Atan2(y, x) * 180 / Math.PI;
            return (brng + 360.0) % 360.0;
        }

        /// <summary>
        /// 计算目标点
        /// </summary>
        /// <param name="current">当前经纬度</param>
        /// <param name="bearing">角度</param>
        /// <param name="m">里程（米）</param>
        /// <returns>经纬度</returns>
        public static LngLat Destination(this LngLat current, double bearing, double m)
        {
            return Destination(current.lng, current.lat, bearing, m);
        }

        /// <summary>
        /// 计算目标点
        /// </summary>
        /// <param name="current">当前经纬度</param>
        /// <param name="bearing">角度</param>
        /// <param name="m">里程（米）</param>
        /// <returns>经纬度</returns>
        public static LngLat Destination(this double[] current, double bearing, double m)
        {
            return Destination(current[0], current[1], bearing, m);
        }

        /// <summary>
        /// 计算目标点
        /// </summary>
        /// <param name="lng">当前经度</param>
        /// <param name="lat">当前纬度</param>
        /// <param name="bearing">角度</param>
        /// <param name="m">里程（米）</param>
        /// <returns>经纬度</returns>
        public static LngLat Destination(double lng, double lat, double bearing, double m)
        {
            double pi = Math.PI / 180;
            var radius = 6371e3;
            double lng_rad = lng * pi, lat_rad = lat * pi;
            double brng = bearing * pi;
            double _lat = Math.Asin(Math.Sin(lat_rad) * Math.Cos(m / radius) + Math.Cos(lat_rad) * Math.Sin(m / radius) * Math.Cos(brng)),
                _lng = lng_rad + Math.Atan2(Math.Sin(brng) * Math.Sin(m / radius) * Math.Cos(lat_rad), Math.Cos(m / radius) - Math.Sin(lat_rad) * Math.Sin(_lat));
            return new LngLat(_lng * (180 / Math.PI), _lat * (180 / Math.PI));
        }
    }
}
