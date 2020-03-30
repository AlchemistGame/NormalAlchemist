using UnityEngine;

public class GlobalSettings
{
    public static int MapSideLength = 1024;
    public static int mapObjectCount = 0;
    public static int UndoSession = 0;
    public static string RootMapDir = Application.streamingAssetsPath + "/Maps/";
    public static string TemplatesDir = RootMapDir + "Templates/";
    public static string MapTilesPath = RootMapDir + "MapTiles.json";
    // 关卡地图路径
    public static string MapLevelsDir = RootMapDir + "MapLevels/";
}
