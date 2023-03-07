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
        /// <summary>
        /// 计算桩号
        /// </summary>
        /// <param name="lines">线</param>
        /// <param name="m">间隔（米）</param>
        /// <param name="sm">开始桩号（米）</param>
        /// <param name="station_div">自定义桩号位置</param>
        /// <param name="direction">反向</param>
        /// <returns>桩号集合</returns>
        public static List<RoadStation> Station(this double[][] lines, int m, int sm = 0, List<RoadStation>? station_div = null, bool direction = false)
        {
            if (direction)
            {
                var _lines = new List<double[]>();
                foreach (var item in lines) _lines.Insert(0, item);
                lines = _lines.ToArray();
            }
            var readm = lines.Distance();
            var arrs = LineDense(lines, readm / 3);
            var list = new List<RoadStation>();
            var has_div = new List<RoadStation>();
            if (station_div != null && station_div.Count > 1)
            {
                var tempos = new List<RoadStations>();
                var start_station = sm;
                int start_index = 0;
                foreach (var item in station_div)
                {
                    var len = item.m - start_station;
                    var mins = new List<MaxDistance>();
                    for (int j = start_index; j < arrs.Count; j++)
                    {
                        var distance = Distance(item.lng, item.lat, arrs[j][0], arrs[j][1]);
                        mins.Add(new MaxDistance
                        {
                            distance = distance,
                            index = j
                        });
                    }
                    mins.Sort((x, y) =>
                    {
                        return x.distance.CompareTo(y.distance);
                    });
                    tempos.Add(new RoadStations
                    {
                        lines = arrs.LineSubstring(start_index, mins[0].index),
                        length = len,
                        st = start_station,
                        et = item.m
                    });
                    start_station = item.m;
                    start_index = mins[0].index - 1;
                }

                tempos.Add(new RoadStations
                {
                    lines = arrs.LineSubstring(start_index, arrs.Count),
                    length = -1,
                    st = start_station,
                    et = -1
                });

                //开始平均
                list.AddRange(StationAuto(tempos, m, sm));
            }
            else
            {
                var s = arrs[0];
                double old_distance = 0;
                int read_distance = sm;
                for (int i = 1; i < arrs.Count; i++)
                {
                    var distance = Distance(s, arrs[i]);
                    old_distance += distance;
                    if (old_distance >= m)
                    {
                        old_distance = 0;
                        read_distance += m;
                        list.Add(new RoadStation(arrs[i], read_distance));
                    }
                    s = arrs[i];
                }
            }
            return list;
        }

        /// <summary>
        /// 让线变得很密
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="m">密度（米）</param>
        static List<double[]> LineDense(double[][] lines, double m)
        {
            if (m > 1) { m = 1; }
            else { m = Math.Round(m, 4); }
            var s = lines[0];
            var gpss = new List<double[]> { s };
            for (int i = 1; i < lines.Length; i++)
            {
                var distance = Distance(s, lines[i]);
                double total = 0;
                double[] _s = s, _e = lines[i];
                var exs = new List<string>();
                while (distance >= total)
                {
                    var angle = Azimuth(new LngLat(_s), new LngLat(_e));
                    var gps_ = Destination(new LngLat(_s), angle, m);
                    var id = gps_.lng.ToString() + gps_.lat.ToString();
                    if (!exs.Contains(id))
                    {
                        exs.Add(id);
                        gpss.Add(new double[] { gps_.lng, gps_.lat });
                    }
                    total += Math.Round(Distance(_s[0], _s[1], gps_.lng, gps_.lat), 3);
                    _s = new double[] { gps_.lng, gps_.lat };
                }
                gpss.Add(_e);
                s = _e;
            }
            return gpss;
        }

        /// <summary>
        /// 截取线
        /// </summary>
        /// <param name="lines">线</param>
        /// <param name="st">开始位置</param>
        /// <param name="et">结束位置</param>
        static List<double[]> LineSubstring(this List<double[]> lines, int st, int et)
        {
            var lins = new List<double[]>();
            for (int i = st; i < et; i++) lins.Add(lines[i]);
            return lins;
        }

        #region 平均分割

        /// <summary>
        /// 自动处理桩号
        /// </summary>
        /// <param name="line">数据包</param>
        /// <param name="m">间隔</param>
        /// <param name="sm">开始桩号</param>
        static List<RoadStation> StationAuto(List<RoadStations> line, int m, int sm)
        {
            var list = new List<RoadStation>();
            for (int i = 0; i < line.Count - 1; i++)
            {
                list.AddRange(StationAVG(line[i], line[i].length, m, ref sm));
                list.Add(new RoadStation
                {
                    m = line[i].et,
                    lng = line[i].lines[line[i].lines.Count - 1][0],
                    lat = line[i].lines[line[i].lines.Count - 1][1]
                });
            }
            list.AddRange(StationPlain(line[line.Count - 1], m, ref sm));
            return list;
        }
        static List<RoadStation> StationAVG(RoadStations line, int total, int _m, ref int sm)
        {
            sm = line.st;
            var list = new List<RoadStation>();
            double old = 0, t = line.lines.Distance();
            var stationCount = total / _m;
            var m = t / stationCount;
            double[] s = line.lines[0];
            for (int i = 1; i < line.lines.Count; i++)
            {
                var e = line.lines[i];
                old += Distance(s, e);
                if (old >= m)
                {
                    old = 0;
                    sm += _m;
                    list.Add(new RoadStation(e, sm));
                }
                s = e;
            }
            sm = line.et;
            return list;
        }
        static List<RoadStation> StationPlain(RoadStations line, int m, ref int sm)
        {
            sm = line.st;
            var list = new List<RoadStation>();
            double old = 0;
            double[] s = line.lines[0];
            for (int i = 1; i < line.lines.Count; i++)
            {
                var e = line.lines[i];
                old += Distance(s, e);
                if (old >= m)
                {
                    old = 0;
                    sm += m;
                    list.Add(new RoadStation(e, sm));
                }
                s = e;
            }
            return list;
        }
        class MaxDistance
        {
            public int index { get; set; }
            public double distance { get; set; }
        }
        class RoadStations
        {
            public List<double[]> lines { get; set; }
            public int length { get; set; }
            public int st { get; set; }
            public int et { get; set; }
        }

        #endregion

        #region 桩号转换

        /// <summary>
        /// 桩号转换米
        /// </summary>
        /// <param name="m">桩号字符串</param>
        /// <param name="deVal">失败返回数字</param>
        public static int StationToNum(this string m, int deVal = -1)
        {
            var _m = m.TrimStart('K').TrimStart('k');
            if (_m.Contains("+") || _m.Contains("-") || _m.Contains("."))
            {
                int index = -1;
                if (_m.Contains("+"))
                    index = _m.LastIndexOf("+");
                else if (_m.Contains("-"))
                    index = _m.LastIndexOf("-");
                else
                    index = _m.LastIndexOf(".");
                if (int.TryParse(_m.Substring(0, index), out int _km_) && int.TryParse(_m.Substring(index + 1), out int _m_))
                    return _km_ * 1000 + _m_;
                else return deVal;
            }
            if (int.TryParse(_m, out int _value)) return _value;
            return deVal;
        }

        /// <summary>
        /// 数字转桩号文本
        /// </summary>
        /// <param name="m">桩号米</param>
        /// <param name="join">拼接符号</param>
        /// <returns>桩号文本</returns>
        public static string StationToStr(this int m, string join = ".")
        {
            if (m >= 1000)
            {
                string mstr = Math.Round(m / 1000.0, 3).ToString();
                if (mstr.Contains("."))
                {
                    int index = mstr.LastIndexOf(".");
                    return "K" + mstr.Substring(0, index) + join + (mstr.Substring(index + 1).PadRight(3, '0'));
                }
                else
                    return "K" + mstr + join + "000";
            }
            else
                return "K0" + join + m.ToString().PadLeft(3, '0');
        }

        /// <summary>
        /// 数字转桩号文本
        /// </summary>
        /// <param name="m">桩号米</param>
        /// <param name="join">拼接符号</param>
        /// <returns>桩号文本</returns>
        public static string StationToStr(this double m, string join = ".")
        {
            if (m >= 1000)
            {
                string mstr = Math.Round(m / 1000.0, 3).ToString();
                if (mstr.Contains("."))
                {
                    int index = mstr.LastIndexOf(".");
                    return "K" + mstr.Substring(0, index) + join + (mstr.Substring(index + 1).PadRight(3, '0'));
                }
                else
                    return "K" + mstr + join + "000";
            }
            else
                return "K0" + join + m.ToString().PadLeft(3, '0');
        }

        #endregion
    }
}
