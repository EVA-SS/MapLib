using System;
using System.Collections.Generic;
using System.Linq;

namespace MapLib
{
    static partial class Map
    {
        #region 判断经纬度是否在范围类

        /// <summary>
        /// 判断经纬度是否在范围类
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="paths">多边形边界点集合</param>
        public static bool IsInRegion(double lng, double lat, IList<LngLatTag> paths)
        {
            if (paths.Count < 3) return false;//点小于3无法构成多边形
            int iSum = 0;
            int pathCount = paths.Count;
            for (int i = 0; i < pathCount; i++)
            {
                int nextIndex = i + 1;
                if (i == pathCount - 1) nextIndex = 0;
                double lng_s = paths[i].lng, lat_s = paths[i].lat, lng_e = paths[nextIndex].lng, lat_e = paths[nextIndex].lat;
                //判断纬度即Y坐标是否在2点的Y坐标内，只有在其内水平线才会相交
                if ((lat >= lat_s && lat < lat_e) || (lat >= lat_e && lat < lat_s))
                {
                    if (Math.Abs(lat_s - lat_e) > 0)
                    {
                        double dLong = lng_s - ((lng_s - lng_e) * (lat_s - lat)) / (lat_s - lat_e);
                        if (dLong < lng) iSum++;
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
        public static bool IsInRegion(double lng, double lat, IList<LngLat> paths)
        {
            if (paths.Count < 3) return false;//点小于3无法构成多边形
            int iSum = 0;
            int pathCount = paths.Count;
            for (int i = 0; i < pathCount; i++)
            {
                int nextIndex = i + 1;
                if (i == pathCount - 1) nextIndex = 0;
                double lng_s = paths[i].lng, lat_s = paths[i].lat, lng_e = paths[nextIndex].lng, lat_e = paths[nextIndex].lat;
                //判断纬度即Y坐标是否在2点的Y坐标内，只有在其内水平线才会相交
                if ((lat >= lat_s && lat < lat_e) || (lat >= lat_e && lat < lat_s))
                {
                    if (Math.Abs(lat_s - lat_e) > 0)
                    {
                        double dLong = lng_s - ((lng_s - lng_e) * (lat_s - lat)) / (lat_s - lat_e);
                        if (dLong < lng) iSum++;
                    }
                }
            }
            if ((iSum % 2) != 0) return true;
            return false;
        }

        #endregion

        public static double DistanceFromPointToPolygon(LngLat point, IList<LngLat> polygon)
        {
            double minDistance = double.MaxValue;
            //LngLat closestPoint = default;
            foreach (var edge in polygon.Zip(polygon.Skip(1).Concat(new[] { polygon.First() }), (p1, p2) => new { p1, p2 }))
            {
                var edgeVector = new LngLat(edge.p2.lng - edge.p1.lng, edge.p2.lat - edge.p1.lat);
                var normalVector = new LngLat(-edgeVector.lat, edgeVector.lng);
                var denominator = Math.Sqrt(edgeVector.lng * edgeVector.lng + edgeVector.lat * edgeVector.lat);
                var t = ((point.lng - edge.p1.lng) * edgeVector.lng + (point.lat - edge.p1.lat) * edgeVector.lat) / denominator;
                var intersectionPoint = new LngLat(edge.p1.lng + edgeVector.lng * t, edge.p1.lat + edgeVector.lat * t);

                if (t >= 0 && t <= 1) // Intersection point on the edge
                {
                    var distance = Math.Sqrt(Math.Pow(point.lng - intersectionPoint.lng, 2) + Math.Pow(point.lat - intersectionPoint.lat, 2));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        //closestPoint = intersectionPoint;
                    }
                }
                else // Intersection point on the edge's extension
                {
                    var distance = Math.Abs(edgeVector.lng * (point.lat - edge.p1.lat) - edgeVector.lat * (point.lng - edge.p1.lng)) / denominator;
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        //closestPoint = new LngLat(point.lng + normalVector.lng * distance, point.lat + normalVector.lat * distance);
                    }
                }
            }
            return minDistance;
        }
    }
}