using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    /// <summary>
    /// 方块边长
    /// </summary>
    public const int gridEdge = 1;

    public struct GridInfo
    {
        int x, y, z;
    }

    public List<GridInfo> mapGridList = new List<GridInfo>();

    public void AddGrid(int x, int y, int z)
    {

    }
}
