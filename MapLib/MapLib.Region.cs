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
    public static partial class Map
    {
        #region 判断经纬度是否在范围类

        /// <summary>
        /// 判断经纬度是否在范围类
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="paths">多边形边界点集合</param>
        public static bool IsInRegion(double lng, double lat, List<LngLatTag> paths)
        {
            if (paths.Count < 3)//点小于3无法构成多边形
                return false;
            int iSum = 0;
            int pathCount = paths.Count;
            for (int i = 0; i < pathCount; i++)
            {
                int nextIndex = i + 1;
                if (i == pathCount - 1)
                    nextIndex = 0;
                double lng_s = paths[i].lng, lat_s = paths[i].lat, lng_e = paths[nextIndex].lng, lat_e = paths[nextIndex].lat;
                //判断纬度即Y坐标是否在2点的Y坐标内，只有在其内水平线才会相交
                if ((lat >= lat_s && lat < lat_e) || (lat >= lat_e && lat < lat_s))
                {
                    if (Math.Abs(lat_s - lat_e) > 0)
                    {
                        double dLong = lng_s - ((lng_s - lng_e) * (lat_s - lat)) / (lat_s - lat_e);
                        if (dLong < lng)
                            iSum++;
                    }
                }
            }
            if ((iSum % 2) != 0) return true;
            return false;
        }

        /// <summary>
        /// 判断经纬度是否在范围类
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="paths">多边形边界点集合</param>
        public static bool IsInRegion(double lng, double lat, List<LngLat> paths)
        {
            if (paths.Count < 3)//点小于3无法构成多边形
                return false;
            int iSum = 0;
            int pathCount = paths.Count;
            for (int i = 0; i < pathCount; i++)
            {
                int nextIndex = i + 1;
                if (i == pathCount - 1)
                    nextIndex = 0;
                double lng_s = paths[i].lng, lat_s = paths[i].lat, lng_e = paths[nextIndex].lng, lat_e = paths[nextIndex].lat;
                //判断纬度即Y坐标是否在2点的Y坐标内，只有在其内水平线才会相交
                if ((lat >= lat_s && lat < lat_e) || (lat >= lat_e && lat < lat_s))
                {
                    if (Math.Abs(lat_s - lat_e) > 0)
                    {
                        double dLong = lng_s - ((lng_s - lng_e) * (lat_s - lat)) / (lat_s - lat_e);
                        if (dLong < lng)
                            iSum++;
                    }
                }
            }
            if ((iSum % 2) != 0) return true;
            return false;
        }

        #endregion
    }
}
