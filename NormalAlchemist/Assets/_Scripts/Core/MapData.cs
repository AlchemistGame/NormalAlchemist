using System;

/// <summary>
/// 地图关卡数据
/// </summary>
[Serializable]
public class MapData
{
    public string blocks;
    public string settings;
    public float mostLeft;
    public float mostRight;
    public float mostForward;
    public float mostBack;
    public float mostUp;
    public float mostBottom;
}
