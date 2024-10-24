//------------------------------------------------------------------------------
//  此代码版权（除特别声明或在MapLib.Model命名空间的代码）归作者本人Tom所有
//  源代码使用协议遵循本仓库的开源协议及附加协议，若本仓库没有设置，则按MIT开源协议授权
//  Github源代码仓库：https://github.com/EVA-SS/MapLib
//  Gitee源代码仓库：https://gitee.com/EVA-SS/MapLib
//  QQ：17379620
//  参考公式：http://www.movable-type.co.uk/scripts/latlong.html
//  感谢您的下载和使用
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------

namespace MapLib
{
    public class LngLat
    {
        /// <summary>
        /// 经纬度
        /// </summary>
        /// <param name="_lng">经度</param>
        /// <param name="_lat">纬度</param>
        public LngLat(double _lng, double _lat)
        {
            lng = _lng;
            lat = _lat;
        }

        /// <summary>
        /// 经纬度
        /// </summary>
        /// <param name="lnglat">经纬度</param>
        public LngLat(double[] lnglat)
        {
            lng = lnglat[0];
            lat = lnglat[1];
        }

        /// <summary>
        /// 经纬度
        /// </summary>
        /// <param name="lnglat">经纬度</param>
        public LngLat(LngLat lnglat)
        {
            lng = lnglat.lng;
            lat = lnglat.lat;
        }

        /// <summary>
        /// 经度
        /// </summary>
        public double lng { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double lat { get; set; }

        public override string ToString() => lng + "," + lat;

        public double[] ToDouble() => new double[] { lng, lat };
    }

    public class LngLatTag : LngLat
    {
        /// <summary>
        /// 经纬度附加
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="_m">桩号</param>
        /// <param name="_tag">附加</param>
        public LngLatTag(double lng, double lat, int _m, object _tag) : base(lng, lat)
        {
            m = _m;
            tag = _tag;
        }

        public LngLatTag(LngLat lnglat, int _m, object _tag) : base(lnglat)
        {
            m = _m;
            tag = _tag;
        }

        public int m { get; set; }

        public object tag { get; set; }
    }

    public class RoadStation
    {
        public RoadStation()
        {
        }

        public RoadStation(double[] _lnglat, int _m)
        {
            lng = _lnglat[0];
            lat = _lnglat[1];
            m = _m;
        }

        public RoadStation(LngLat _lnglat, int _m)
        {
            lng = _lnglat.lng;
            lat = _lnglat.lat;
            m = _m;
        }

        public RoadStation(RoadStation _lnglat, int _m)
        {
            lng = _lnglat.lng;
            lat = _lnglat.lat;
            m = _m;
        }

        public RoadStation(double _lng, double _lat, int _m)
        {
            lng = _lng;
            lat = _lat;
            m = _m;
        }

        public double lng { get; set; }
        public double lat { get; set; }
        public int m { get; set; }
    }
}