using System;
using System.Collections.Generic;
using System.Linq;

namespace MapLib
{
    static partial class Map
    {
        /// <summary>
        /// 获取两点之间的距离
        /// </summary>
        /// <param name="lnglat">第一个坐标</param>
        /// <param name="lnglat2">第二个坐标</param>
        /// <returns>两个坐标之间的距离</returns>
        public static double Distance(this LngLat lnglat, LngLat lnglat2) => Distance(lnglat.lng, lnglat.lat, lnglat2.lng, lnglat2.lat);

        /// <summary>
        /// 获取两点之间的距离
        /// </summary>
        /// <param name="lnglat">第一个经纬度</param>
        /// <param name="lnglat2">第二个经纬度</param>
        /// <returns>米</returns>
        public static double Distance(this double[] lnglat, double[] lnglat2) => Distance(lnglat[0], lnglat[1], lnglat2[0], lnglat2[1]);

        /// <summary>
        /// 获取两点之间的距离
        /// </summary>
        /// <param name="lng">第一个经度</param>
        /// <param name="lat">第一个纬度</param>
        /// <param name="lng2">第二个经度</param>
        /// <param name="lat2">第二个纬度</param>
        /// <returns>米</returns>
        public static double Distance(double lng, double lat, double lng2, double lat2)
        {
            try
            {
                var c = Math.Sin((lat2 - lat) * PI180 / 2);
                var d = Math.Sin((lng2 - lng) * PI180 / 2);
                var a = c * c + d * d * Math.Cos(lat2 * PI180) * Math.Cos(lat2 * PI180);
                return 12756274 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取多点总长度
        /// </summary>
        /// <param name="lines">线</param>
        /// <returns>米</returns>
        public static double Distance(this List<double[]> lines)
        {
            if (lines.Count > 0)
            {
                var s = lines[0];
                double total = 0;
                for (int i = 1; i < lines.Count; i++)
                {
                    total += Distance(s, lines[i]);
                    s = lines[i];
                }
                return total;
            }
            return 0;
        }

        /// <summary>
        /// 获取多点总长度
        /// </summary>
        /// <param name="lines">线</param>
        /// <returns>米</returns>
        public static double Distance(this double[][] lines)
        {
            var s = lines[0];
            double total = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                total += Distance(s, lines[i]);
                s = lines[i];
            }
            return total;
        }

        /// <summary>
        /// 计算点与最接近的线
        /// </summary>
        /// <param name="current">当前点</param>
        /// <param name="line"></param>
        /// <param name="station">桩号</param>
        public static LngLatTag? Distance(this LngLat current, IList<LngLatTag> line, out int? station)
        {
            try
            {
                var lnglatMs = new List<LngLatM>();
                foreach (var item in line) lnglatMs.Add(new LngLatM(current.Distance(item), item));
                var _lnglatMs = lnglatMs.OrderBy(x => x.m);
                var find = _lnglatMs.First(); //最近点
                var find2 = _lnglatMs.Take(2).Last(); //第二点
                if (find.lnglat.m > find2.lnglat.m) station = (int)Math.Round(find.lnglat.m - find.m);
                else station = (int)Math.Round(find.lnglat.m + find.m);
                return find.lnglat;
            }
            catch
            {
            }
            station = null;
            return null;
        }

        class LngLatM
        {
            public LngLatM(double _m, LngLatTag _lnglat)
            {
                m = _m;
                lnglat = _lnglat;
            }

            public double m { get; set; }
            public LngLatTag lnglat { get; set; }
        }
    }
}