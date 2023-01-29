﻿//------------------------------------------------------------------------------
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
using System.Linq;

namespace MapLib
{
    public static partial class Map
    {
        /// <summary>
        /// 获取两点之间的距离
        /// </summary>
        /// <param name="lnglat">第一个坐标</param>
        /// <param name="lnglat2">第二个坐标</param>
        /// <returns>两个坐标之间的距离</returns>
        public static double Distance(this LngLat lnglat, LngLat lnglat2)
        {
            return Distance(lnglat.lng, lnglat.lat, lnglat2.lng, lnglat2.lat);
        }

        /// <summary>
        /// 获取两点之间的距离
        /// </summary>
        /// <param name="lnglat">第一个经纬度</param>
        /// <param name="lnglat2">第二个经纬度</param>
        /// <returns>米</returns>
        public static double Distance(this double[] lnglat, double[] lnglat2)
        {
            return Distance(lnglat[0], lnglat[1], lnglat2[0], lnglat2[1]);
        }

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
                var b = Math.PI / 180;
                var c = Math.Sin((lat2 - lat) * b / 2);
                var d = Math.Sin((lng2 - lng) * b / 2);
                var a = c * c + d * d * Math.Cos(lat2 * b) * Math.Cos(lat2 * b);
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
            var s = lines[0];
            double total = 0;
            for (int i = 1; i < lines.Count; i++)
            {
                total += Distance(s, lines[i]);
                s = lines[i];
            }
            return total;
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
        /// <param name="differ">相差（米）</param>
        /// <param name="line"></param>
        public static LngLatTag Distance(this LngLat current, out double differ, List<LngLatTag> line)
        {
            try
            {
                var lnglatMs = new List<LngLatM>();
                foreach (var item in line)
                {
                    lnglatMs.Add(new LngLatM(current.Distance(item), item));
                }
                if (lnglatMs.Count > 0)
                {
                    if (lnglatMs.Count > 1)
                    {
                        var find = lnglatMs.OrderBy(x => x.m).First();
                        differ = find.m;
                        return find.lnglat;
                    }
                    else
                    {
                        differ = lnglatMs[0].m;
                        return lnglatMs[0].lnglat;
                    }
                }
            }
            catch { }
            differ = -1;
            return null;
        }

        class LngLatM
        {
            public LngLatM(double m, LngLatTag _lnglat) { this.m = m; lnglat = _lnglat; }
            public double m { get; set; }
            public LngLatTag lnglat { get; set; }
        }
    }
}
