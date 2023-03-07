// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var valstr = MapLib.Map.StationToStr(1010, "+");
Console.WriteLine("1010桩号：" + valstr);
var st2 = 1010 + 990;
var valstr2 = MapLib.Map.StationToStr(st2);
Console.WriteLine(st2 + "桩号：" + valstr2);
var val1 = MapLib.Map.Distance(102.570619, 24.964126, 102.575351, 24.960976);
Console.WriteLine("两点距离【云南省昆明市】：" + val1 + "米");
var val2 = MapLib.Map.Azimuth(new MapLib.LngLat(102.570619, 24.964126), new MapLib.LngLat(102.575351, 24.960976));
Console.WriteLine("方向角：" + val2);