// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var val1 = MapLib.Map.Distance(102.570619, 24.964126, 102.575351, 24.960976);
Console.WriteLine("两点距离【云南省昆明市】：" + val1 + "米");
var val2 = MapLib.Map.Azimuth(new MapLib.LngLat(102.570619, 24.964126), new MapLib.LngLat(102.575351, 24.960976));
Console.WriteLine("方向角：" + val2);