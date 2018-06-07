using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridMathLib
{
    /// <summary>
    /// 六角网格的边长
    /// </summary>
    public const float sideLength = 10.0f;

    /// <summary>
    /// 相邻网格单元的中心距离
    /// </summary>
    public static readonly float neighborDistance = sideLength * Mathf.Sqrt(3f);
}
