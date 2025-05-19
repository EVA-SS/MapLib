using System;
using System.Collections.Generic;

namespace MapLib
{
    static partial class Map
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
            var TotalDistance = lines.Distance();//总里程
            var HighDensityLine = LineDense(lines, TotalDistance);//高密度线
            if (station_div != null && station_div.Count > 1)
            {
                var tempos = new List<RoadStations>(station_div.Count);
                var start_station = sm;
                int start_index = 0;
                foreach (var item in station_div)
                {
                    int end = FindNearest(HighDensityLine, item.lng, item.lat, start_index);
                    if (start_index == end) continue;
                    var line_tmp = HighDensityLine.LineSubstring(start_index, end);
                    if (line_tmp.Count > 2) tempos.Add(new RoadStations(line_tmp, start_station, item.m));
                    start_station = item.m;
                    start_index = end - 1;
                }
                //int xc = HighDensityLine.Count - start_index;
                //if (xc > 2)
                //{
                //    var line_tmp = HighDensityLine.LineSubstring(start_index, HighDensityLine.Count);
                //    if (line_tmp.Count > 2) tempos.Add(new RoadStations(line_tmp, -1, start_station, -1));
                //}
                //开始平均
                return StationAuto(tempos, TotalDistance, m, sm);
            }
            else
            {
                if (direction && sm > TotalDistance) return StationAuto(new List<RoadStations> { new RoadStations(HighDensityLine, sm, sm - (int)Math.Ceiling(TotalDistance)) }, TotalDistance, m, sm);
                return StationAuto(new List<RoadStations> { new RoadStations(HighDensityLine, sm, sm + (int)Math.Ceiling(TotalDistance)) }, TotalDistance, m, sm);
            }
        }

        /// <summary>
        /// 查找最近的点
        /// </summary>
        static int FindNearest(this List<double[]> line, double lng, double lat, int start)
        {
            var list = new List<MaxDistance>(line.Count - start);
            for (int i = start; i < line.Count; i++) list.Add(new MaxDistance(i, Distance(lng, lat, line[i][0], line[i][1])));
            list.Sort((x, y) => x.distance.CompareTo(y.distance));
            return list[0].index;
        }

        /// <summary>
        /// 让线变得很密
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="td">总长</param>
        static List<double[]> LineDense(double[][] lines, double td)
        {
            double m = td / 3;
            if (m > 1) m = 1;
            else m = Math.Round(m, 4);
            var s = lines[0];
            var gpss = new List<double[]>((int)Math.Ceiling(td / m)) { s };
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
            var lins = new List<double[]>(et);
            for (int i = st; i < et; i++) lins.Add(lines[i]);
            return lins;
        }

        #region 平均分割

        /// <summary>
        /// 自动处理桩号
        /// </summary>
        /// <param name="line">数据包</param>
        /// <param name="td">总里程</param>
        /// <param name="m">间隔</param>
        /// <param name="sm">开始桩号</param>
        static List<RoadStation> StationAuto(List<RoadStations> line, double td, int m, int sm)
        {
            var list = new List<RoadStation>(line.Count + 2) { new RoadStation
            {
                m = sm,
                lng = line[0].lines[0][0],
                lat = line[0].lines[0][1]
            }};
            bool add = line[0].length > 0;
            for (int i = 0; i < line.Count; i++)
            {
                list.AddRange(StationAVG(line[i], line[i].length, m, add));
                list.Add(new RoadStation
                {
                    m = line[i].et,
                    lng = line[i].lines[line[i].lines.Count - 1][0],
                    lat = line[i].lines[line[i].lines.Count - 1][1]
                });
            }
            return list;
        }
        static List<RoadStation> StationAVG(RoadStations line, int total, int _m, bool add)
        {
            int sm = line.st;
            var list = new List<RoadStation>(line.lines.Count);
            double old = 0, t = line.lines.Distance(), stationCount = Math.Abs(total * 1D) / _m, m = t / stationCount;
            double[] s = line.lines[0];
            for (int i = 1; i < line.lines.Count; i++)
            {
                var e = line.lines[i];
                old += Distance(s, e);
                if (old >= m)
                {
                    old = 0;
                    if (add) sm += _m;
                    else sm -= _m;
                    list.Add(new RoadStation(e, sm));
                }
                s = e;
            }
            return list;
        }

        class MaxDistance
        {
            public MaxDistance(int i, double d)
            {
                index = i;
                distance = d;
            }
            public int index { get; set; }
            public double distance { get; set; }
        }
        class RoadStations
        {
            public RoadStations(List<double[]> _lines, int _st, int _et)
            {
                lines = _lines;
                length = _et - _st;
                st = _st;
                et = _et;
            }

            public RoadStations(List<double[]> _lines, int _length, int _st, int _et)
            {
                lines = _lines;
                length = _length;
                st = _st;
                et = _et;
            }

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
                if (_m.Contains("+")) index = _m.LastIndexOf("+");
                else if (_m.Contains("-")) index = _m.LastIndexOf("-");
                else index = _m.LastIndexOf(".");
                if (int.TryParse(_m.Substring(0, index), out int _km_) && int.TryParse(_m.Substring(index + 1), out int _m_)) return _km_ * 1000 + _m_;
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
            string mstr = Math.Round(m / 1000.0, 3).ToString();
            if (mstr.Contains("."))
            {
                int index = mstr.LastIndexOf(".");
                return "K" + mstr.Substring(0, index) + join + (mstr.Substring(index + 1).PadRight(3, '0'));
            }
            else return "K" + mstr + join + "000";
        }

        /// <summary>
        /// 数字转桩号文本
        /// </summary>
        /// <param name="m">桩号米</param>
        /// <param name="join">拼接符号</param>
        /// <returns>桩号文本</returns>
        public static string StationToStr(this double m, string join = ".")
        {
            string mstr = Math.Round(m / 1000.0, 3).ToString();
            if (mstr.Contains("."))
            {
                int index = mstr.LastIndexOf(".");
                return "K" + mstr.Substring(0, index) + join + (mstr.Substring(index + 1).PadRight(3, '0'));
            }
            else return "K" + mstr + join + "000";
        }

        #endregion
    }
}