using System.Collections.Generic;
using UnityEngine;

namespace MyBattle
{
    public class GridMapManager : MonoBehaviour
    {
        /// <summary>
        /// 整个地图以方块为单位进行划分
        /// </summary>
        public const float gridUnitWidth = 1;
        public const float gridUnitDepth = 1;
        public const float gridUnitHeight = 1;
        public const int gridMapWidth = 50;
        public const int gridMapDepth = 1;
        public const int gridMapHeight = 50;
        public static Vector3 gridMapOriginPos = Vector3.zero;

        private GridUnitData[,,] gridUnitMapData;
        public static GridMapManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void GenerateGridMap()
        {
            gridUnitMapData = new GridUnitData[gridMapWidth, gridMapDepth, gridMapHeight];

            for (int x = 0; x < gridMapWidth; x++)
            {
                for (int y = 0; y < gridMapDepth; y++)
                {
                    for (int z = 0; z < gridMapHeight; z++)
                    {
                        GridUnitData gud = new GridUnitData();
                        gud.gridCoord = new Int3(x, y, z);
                        gud.gridType = (GridType)Random.Range(0, (int)GridType.Max);
                        gud.gridState = GridState.normal;
                        gridUnitMapData[x, y, z] = gud;
                    }
                }
            }

            DisplayGridMap();
        }

        private void DisplayGridMap()
        {
            for (int x = 0; x < gridMapWidth; x++)
            {
                for (int y = 0; y < gridMapDepth; y++)
                {
                    for (int z = 0; z < gridMapHeight; z++)
                    {
                        gridUnitMapData[x, y, z].CreateGridUnit();
                    }
                }
            }
        }

        public List<GridUnitData> GetPathFromStartToEnd(Int3 startCoord, Int3 endCoord)
        {
            GridUnitData startNode = GetGridUnitDataFrom2DCoord(startCoord.x, startCoord.z);
            GridUnitData endNode = GetGridUnitDataFrom2DCoord(endCoord.x, endCoord.z);
            return PathFinder.RequestPathfind(startNode, endNode);
        }

        #region public API
        /// <summary>
        /// 根据 grid 坐标计算 world 坐标
        /// 整个地图左下角为原点
        /// 每一个 grid 方块, 其中心点为 (0,0,0)
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <param name="gridZ"></param>
        /// <returns></returns>
        public static Vector3 GridCoordToWorldPos(Int3 gridCoord)
        {
            float x = gridMapOriginPos.x + gridCoord.x * gridUnitWidth;
            float y = gridMapOriginPos.y + gridCoord.y * gridUnitDepth;
            float z = gridMapOriginPos.z + gridCoord.z * gridUnitHeight;
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// 根据 world 坐标计算 grid 坐标
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public static Int3 WorldPosToGridCoord(Vector3 worldPos)
        {
            float x = (worldPos.x - gridMapOriginPos.x) / gridUnitWidth;
            float y = (worldPos.y - gridMapOriginPos.y) / gridUnitDepth;
            float z = (worldPos.z - gridMapOriginPos.z) / gridUnitHeight;

            int gridX = Mathf.RoundToInt(x);
            int gridY = Mathf.RoundToInt(y);
            int gridZ = Mathf.RoundToInt(z);

            return new Int3(gridX, 0, gridZ);
        }

        /// <summary>
        /// 根据 grid 坐标获取 GridUnitData 数据
        /// </summary>
        public static GridUnitData GetGridUnitDataFromGridCoord(Int3 coord)
        {
            GridUnitData retVal = null;

            if (coord.x >= 0 && coord.x < Instance.gridUnitMapData.GetLength(0) &&
                coord.y >= 0 && coord.y < Instance.gridUnitMapData.GetLength(1) &&
                coord.z >= 0 && coord.z < Instance.gridUnitMapData.GetLength(2))
            {
                retVal = Instance.gridUnitMapData[coord.x, coord.y, coord.z];
            }

            return retVal;
        }

        public GridUnitData GetGridUnitDataFromWorldPos(Vector3 pos)
        {
            Int3 gridCoord = WorldPosToGridCoord(pos);
            return GetGridUnitDataFromGridCoord(gridCoord);
        }

        public GridUnitData GetGridUnitDataFrom2DCoord(int x, int z)
        {
            GridUnitData retVal = null;

            if (x >= 0 && x < Instance.gridUnitMapData.GetLength(0) &&
                z >= 0 && z < Instance.gridUnitMapData.GetLength(2))
            {
                retVal = Instance.gridUnitMapData[x, Instance.gridUnitMapData.GetLength(1) - 1, z];
            }

            return retVal;
        }

        public static GridUnitData GridUnitRaycast(Ray ray, float range)
        {
            return GridUnitRaycast(ray.origin, ray.direction, range);
        }

        public static GridUnitData GridUnitRaycast(Vector3 origin, Vector3 direction, float range)
        {
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(origin, direction, out hit, range))
            {
                if (hit.collider.GetComponent<GridUnit>() != null)
                {
                    return Instance.GetGridUnitDataFromWorldPos(hit.transform.position);
                }
            }

            return null;
        }
        #endregion
    }
}