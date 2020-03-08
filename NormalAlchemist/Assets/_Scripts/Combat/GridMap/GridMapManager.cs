using SimplexNoise;
using System.Collections.Generic;
using UnityEngine;

namespace MyBattle
{
    public enum BlockType
    {
        None,       // 空
        Mud,        // 土
        Grass,      // 草
        Rock,       // 石头
    }

    /// <summary>
    /// 整个地图以方块为单位进行划分
    /// </summary>
    public class GridMapManager : MonoBehaviour
    {
        [HideInInspector]
        public int seed;                                            // 随机种子
        public const float gridUnitWidth = 1;
        public const float gridUnitDepth = 1;
        public const float gridUnitHeight = 1;
        public const int gridMapWidth = 50;                         // x
        public const int gridMapDepth = 10;                         // y
        public const int gridMapHeight = 50;                        // z
        public const int minGridMapDepth = 1;
        public const float frequency = 0.025f;                      // 噪音频率 (噪音采样时用到)
        public const float amplitude = 5;                           // 噪音振幅
        public static Vector3 gridMapOriginPos = Vector3.zero;

        private GridUnitData[,,] gridUnitMapData;                   // 整个地图内所有方块的数据信息
        //噪音采样时会用到的偏移
        private Vector3Int offset0;
        private Vector3Int offset1;
        private Vector3Int offset2;

        public static GridMapManager Instance { get; private set; }

        #region Private Methods
        private void Awake()
        {
            Instance = this;

            EventManager.Register(EventsEnum.StartGenerateGridMap, this, "GenerateGridMap");
        }

        private int GenerateDepth(Vector3Int mapCoord)
        {
            // 让随机种子，振幅，频率，应用于噪音采样结果
            float x0 = (mapCoord.x + offset0.x) * frequency;
            float y0 = (mapCoord.y + offset0.y) * frequency;
            float z0 = (mapCoord.z + offset0.z) * frequency;

            float x1 = (mapCoord.x + offset1.x) * frequency * 2;
            float y1 = (mapCoord.y + offset1.y) * frequency * 2;
            float z1 = (mapCoord.z + offset1.z) * frequency * 2;

            float x2 = (mapCoord.x + offset2.x) * frequency / 4;
            float y2 = (mapCoord.y + offset2.y) * frequency / 4;
            float z2 = (mapCoord.z + offset2.z) * frequency / 4;

            float noise0 = Noise.Generate(x0, y0, z0) * amplitude;
            float noise1 = Noise.Generate(x1, y1, z1) * amplitude / 2;
            float noise2 = Noise.Generate(x2, y2, z2) * amplitude / 4;

            // 限制随机生成的地图深度下限
            int retHeight = Mathf.FloorToInt(noise0 + noise1 + noise2);
            return retHeight >= minGridMapDepth ? retHeight : minGridMapDepth;
        }

        private GridUnitData GenerateGridUnitData(Vector3Int mapCoord)
        {
            // 获取当前位置方块随机生成的高度值
            float genDepth = GenerateDepth(mapCoord);
            BlockType blockType;
            // 当前方块位置高于设定的地图最大深度, 或随机生成的高度值时，当前方块类型为空
            if (mapCoord.y > genDepth)
            {
                blockType = BlockType.None;
            }
            // 当前方块位置等于随机生成的高度值时，当前方块类型为草地
            else if (mapCoord.y == genDepth)
            {
                blockType = BlockType.Grass;
            }
            // 当前方块位置小于随机生成的高度值且大于 genDepth - 5时，当前方块类型为泥土
            else if (mapCoord.y < genDepth && mapCoord.y > genDepth - 5)
            {
                blockType = BlockType.Mud;
            }
            // 地下都是岩石
            else
            {
                blockType = BlockType.Rock;
            }

            GridUnitData gud = new GridUnitData
            {
                gridCoord = mapCoord,
                gridType = blockType,
                gridState = GridState.normal
            };
            return gud;
        }

        /// <summary>
        /// 根据生成数据, build 得到地图
        /// </summary>
        private void BuildGridMap()
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

            EventManager.Broadcast(EventsEnum.FinishGenerateGridMap);
        }
        #endregion

        #region public API
        public void GenerateGridMap(int seedParam)
        {
            // 初始化随机种子
            this.seed = seedParam;
            Random.InitState(seed);
            offset0 = new Vector3Int(Random.Range(0, 1000), Random.Range(0, 1000), Random.Range(0, 1000));
            offset1 = new Vector3Int(Random.Range(0, 1000), Random.Range(0, 1000), Random.Range(0, 1000));
            offset2 = new Vector3Int(Random.Range(0, 1000), Random.Range(0, 1000), Random.Range(0, 1000));

            gridUnitMapData = new GridUnitData[gridMapWidth, gridMapDepth, gridMapHeight];

            // 遍历整个地图, 生成其中每个方块的数据
            for (int x = 0; x < gridMapWidth; x++)
            {
                for (int y = 0; y < gridMapDepth; y++)
                {
                    for (int z = 0; z < gridMapHeight; z++)
                    {
                        gridUnitMapData[x, y, z] = GenerateGridUnitData(new Vector3Int(x, y, z));
                    }
                }
            }

            BuildGridMap();
        }

        public List<GridUnitData> GetPathFromStartToEnd(Vector2Int startCoord, Vector2Int endCoord)
        {
            GridUnitData startNode = GetTopGridUnitDataFrom2DCoord(startCoord);
            GridUnitData endNode = GetTopGridUnitDataFrom2DCoord(endCoord);
            return PathFinder.RequestPathfind(startNode, endNode);
        }

        /// <summary>
        /// 根据 grid 坐标计算 world 坐标
        /// 整个地图左下角为原点
        /// 每一个 grid 方块, 其中心点为 (0,0,0)
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <param name="gridZ"></param>
        /// <returns></returns>
        public static Vector3 GridCoordToWorldPos(Vector3Int gridCoord)
        {
            float x = gridMapOriginPos.x + gridCoord.x * gridUnitWidth;
            float y = gridMapOriginPos.y + gridCoord.y * gridUnitDepth;
            float z = gridMapOriginPos.z + gridCoord.z * gridUnitHeight;
            return new Vector3(x, y, z);
        }

        public static GridUnitData GetTopGridUnitDataFrom2DCoord(Vector2Int coord)
        {
            GridUnitData retVal = null;

            for (int i = Instance.gridUnitMapData.GetLength(1) - 1; i >= 0; i--)
            {
                retVal = Instance.gridUnitMapData[coord.x, i, coord.y];
                if (retVal.gridType != BlockType.None)
                {
                    break;
                }
            }

            return retVal;
        }

        // 获取脚下方块顶部面中心的位置
        public static Vector3 ActorCoordToWorldPos(Vector2Int actorCoord)
        {
            GridUnitData gu = GetTopGridUnitDataFrom2DCoord(actorCoord);
            float x = gridMapOriginPos.x + gu.gridCoord.x * gridUnitWidth;
            float y = gridMapOriginPos.y + gu.gridCoord.y * gridUnitDepth;
            float z = gridMapOriginPos.z + gu.gridCoord.z * gridUnitHeight;
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// 根据 world 坐标计算 grid 坐标
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public static Vector3Int WorldPosToGridCoord(Vector3 worldPos)
        {
            float x = (worldPos.x - gridMapOriginPos.x) / gridUnitWidth;
            float y = (worldPos.y - gridMapOriginPos.y) / gridUnitDepth;
            float z = (worldPos.z - gridMapOriginPos.z) / gridUnitHeight;

            int gridX = Mathf.RoundToInt(x);
            int gridY = Mathf.RoundToInt(y);
            int gridZ = Mathf.RoundToInt(z);

            return new Vector3Int(gridX, 0, gridZ);
        }

        /// <summary>
        /// 根据 grid 坐标获取 GridUnitData 数据
        /// </summary>
        public static GridUnitData GetGridUnitDataFromGridCoord(Vector3Int coord)
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
            Vector3Int gridCoord = WorldPosToGridCoord(pos);
            return GetGridUnitDataFromGridCoord(gridCoord);
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