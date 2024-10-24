//------------------------------------------------------------------------------
//  此代码版权（除特别声明或在MapLib命名空间的代码）归作者本人Tom所有
//  源代码使用协议遵循本仓库的开源协议及附加协议，若本仓库没有设置，则按MIT开源协议授权
//  Github源代码仓库：https://github.com/EVA-SS/MapLib
//  Gitee源代码仓库：https://gitee.com/EVA-SS/MapLib
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
        /// <summary>
        /// 判断是否偏离航线
        /// </summary>
        /// <param name="point">实时点用于判断此点是否偏离航线</param>
        /// <param name="points">航线组成的点坐标</param>
        /// <returns>距离多少m（可自行判断阈值）</returns>
        public static double PointToPintLine(this LngLat point, List<LngLat> points)
        {
            double minDistance = -1;
            for (int i = 0;i < points.Count - 1;i++)
            {
                if (points[i].lng == points[i + 1].lng && points[i].lat == points[i + 1].lat) continue;
                //获取线段的x取值范围和Y的取值范围
                double[] rangeX = new double[2], rangeY = new double[2];
                if (points[i].lng > points[i + 1].lng)
                {
                    rangeX[0] = points[i + 1].lng;
                    rangeX[1] = points[i].lng;
                }
                else
                {
                    rangeX[0] = points[i].lng;
                    rangeX[1] = points[i + 1].lng;
                }

                if (points[i].lat > points[i + 1].lat)
                {
                    rangeY[0] = points[i + 1].lat;
                    rangeY[1] = points[i].lat;
                }
                else
                {
                    rangeY[0] = points[i].lat;
                    rangeY[1] = points[i + 1].lat;
                }

                //根据两点求出直线方程AX+BY+C=0中，A B C 的值
                double a = points[i + 1].lat - points[i].lat;
                double b = points[i].lng - points[i + 1].lng;
                double c = points[i + 1].lng * points[i].lat - points[i].lng * points[i + 1].lat;

                //求点到直线的垂足以及距离
                //得到垂足点
                var foot = getFootOfPerpendicular(point.lng, point.lat, a, b, c);
                if (foot == null) return -1;
                //得到距离
                double distance = Distance(point.lng, point.lat, foot.lng, foot.lat);

                //判断垂足是否在线段上
                if (foot.lng >= rangeX[0] && foot.lng <= rangeX[1] && foot.lat >= rangeY[0] && foot.lat <= rangeY[1])
                {
                    //1.如果在线段上则记录值
                    //2.跟minDistance比较，如果小于目前值则进行替换(若是初始值(-1)也进行替换)
                    if ((minDistance == -1) || (minDistance != -1 && distance < minDistance)) minDistance = distance;
                }
                else
                {
                    //计算点距离
                    double start = Distance(point, points[i]), end = Distance(point, points[i + 1]);
                    distance = (start <= end ? start : end);
                    if (minDistance == -1 || minDistance > distance) minDistance = distance;
                }
            }

            //1.看是否minDistance是否是初始值
            //2.如果是初始值则再次计算点到首末两点的距离，若均大于allowRange则认为偏离航线
            if (minDistance == -1)
            {
                //计算点到首末两点的距离
                LngLat startPoint = points[0], endPoint = points[points.Count - 1];
                double start = Distance(point.lng, point.lat, startPoint.lng, startPoint.lat), end = Distance(point.lng, point.lat, endPoint.lng, endPoint.lat);
                return start <= end ? start : end;
            }

            return minDistance;
        }

        static LngLat? getFootOfPerpendicular(double x1, double y1, double A, double B, double C)
        {
            if (A * A + B * B < 1e-13) return null;
            if (Math.Abs(A * x1 + B * y1 + C) < 1e-13) return new LngLat(x1, y1);
            else
            {
                double newX = (B * B * x1 - A * B * y1 - A * C) / (A * A + B * B), newY = (-A * B * x1 + A * A * y1 - B * C) / (A * A + B * B);
                return new LngLat(newX, newY);
            }
        }
    }
}