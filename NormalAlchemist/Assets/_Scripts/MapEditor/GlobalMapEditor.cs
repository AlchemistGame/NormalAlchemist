using UnityEngine;

public class GlobalMapEditor
{
    public static bool canBuild = false;
    public static int mapObjectCount = 0;
    // 是否检测重叠
    public static bool OverlapDetection = true;
    public static int UndoSession = 0;
    public static string RootMapDir = Application.streamingAssetsPath + "/Maps/";
    public static string TemplatesDir = RootMapDir + "Templates/";
    public static string MapTilesPath = RootMapDir + "MapTiles.json";
    // 关卡地图路径
    public static string MapLevelsDir = RootMapDir + "MapLevels/";
}
