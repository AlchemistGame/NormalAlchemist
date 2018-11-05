using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding
{
    public class GridManager : MonoBehaviour
    {
        private struct Int3
        {
            public int x, y, z;

            public Int3(int width, int depth, int height)
            {
                x = width;
                y = depth;
                z = height;
            }
        }
        
        public GameObject groundGO;

        /// <summary>
        /// 将整个地图以方块为单位进行划分
        /// </summary>
        public float gridWidth = 1.0f;
        public float gridDepth = 1.0f;
        public float gridHeight = 1.0f;

        private float groundWidth;
        private float groundHeight;
        private Node[,,] gridData;

        public static GridManager _instance;
        public static GridManager Instance
        {
            get
            {
                return _instance;
            }
        }


        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            GetGroundSize();

            GenerateGridData();
        }

        private void Update()
        {
            RefreshGridData();
        }
        

        void GetGroundSize()
        {
            groundWidth = groundGO.GetComponent<Renderer>().bounds.size.x;
            groundHeight = groundGO.GetComponent<Renderer>().bounds.size.z;
        }

        void GenerateGridData()
        {
            Int3 gridSize = CalcGridSize();

            int maxX = gridSize.x;
            int maxY = gridSize.y;
            int maxZ = gridSize.z;

            //The typical way to create a grid
            gridData = new Node[maxX, maxY, maxZ];

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    for (int z = 0; z < maxZ; z++)
                    {
                        //Create a new node and update it's values
                        Node node = new Node();
                        node.x = x;
                        node.y = y;
                        node.z = z;
                        
                        //then place it to the grid
                        gridData[x, y, z] = node;
                    }
                }
            }
        }

        private void RefreshGridData()
        {
            GameObject[] blockObjects = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var blockObj in blockObjects)
            {
                Int3 gridCoord = CalcGridCoord(blockObj.transform.position);
                Node enemyNode = GetNode(gridCoord.x, gridCoord.y, gridCoord.z);
                if (enemyNode != null)
                {
                    enemyNode.isWalkable = false;
                }
            }
        }

        /// <summary>
        /// 以 Grid 为单位, 整个 ground 地图可以划分为多少行、多少列
        /// </summary>
        /// <returns></returns>
        Int3 CalcGridSize()
        {
            int gridWidthInGrids = (int)(groundWidth / gridWidth);
            int gridDepthInGrids = 1;   // 暂定 Y 方向只有一层
            int gridHeightInGrids = (int)(groundHeight / gridHeight);
            return new Int3(gridWidthInGrids, gridDepthInGrids, gridHeightInGrids);
        }
        
        // 根据 grid 坐标计算 world 坐标
        Vector3 CalcWorldCoord(int gridX, int gridY, int gridZ)
        {
            Vector3 initPos = CalcInitPos();

            float x = initPos.x + gridX * gridWidth;
            float y = initPos.y + gridY * gridDepth;
            float z = initPos.z + gridZ * gridHeight;
            return new Vector3(x, y, z);
        }

        // 根据 world 坐标计算 grid 坐标
        Int3 CalcGridCoord(Vector3 worldPos)
        {
            Vector3 initPos = CalcInitPos();

            float x = (worldPos.x - initPos.x) / gridWidth;
            float y = (worldPos.y - initPos.y) / gridDepth;
            float z = (worldPos.z - initPos.z) / gridHeight;

            int gridX = Mathf.RoundToInt(x);
            int gridY = Mathf.RoundToInt(y);
            int gridZ = Mathf.RoundToInt(z);

            return new Int3(gridX, 0, gridZ);
        }

        /// <summary>
        /// 以地图左下角为原点
        /// 每一个 grid 方块, 其中心点为 (0,0,0)
        /// </summary>
        /// <returns></returns>
        Vector3 CalcInitPos()
        {
            Vector3 initPos;
            initPos = new Vector3(-groundWidth / 2 + gridWidth / 2,
                0,
                -groundHeight / 2 + gridWidth / 2);

            return initPos;
        }


        /// <summary>
        /// 根据 grid 坐标获取 node 节点数据
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Node GetNode(int x, int y, int z)
        {
            Node retVal = null;

            if (x >= 0 && x < gridData.GetLength(0) &&
                y >= 0 && y < gridData.GetLength(1) &&
                z >= 0 && z < gridData.GetLength(2))
            {
                retVal = gridData[x, y, z];
            }

            return retVal;
        }

        public Node GetNodeFromVector3(Vector3 pos)
        {
            Int3 gridCoord = CalcGridCoord(pos);
            return GetNode(gridCoord.x, gridCoord.y, gridCoord.z);
        }

        public List<Vector3> GetPathFromStartToEnd(Vector3 startPosition, Vector3 endPosition)
        {
            Node startNode = GetNodeFromVector3(startPosition);
            Node endNode = GetNodeFromVector3(endPosition);

            List<Vector3> movingPath = new List<Vector3>();

            List<Node> movingNodeList = Pathfinder.RequestPathfind(startNode, endNode);
            foreach (var node in movingNodeList)
            {
                movingPath.Add(CalcWorldCoord(node.x, node.y, node.z));
            }

            return movingPath;
        }

        public Vector3 GetGridCenterPos(Vector3 groundPos)
        {
            Node curNode = GetNodeFromVector3(groundPos);
            Vector3 curPos = CalcWorldCoord(curNode.x, curNode.y, curNode.z);
            return curPos;
        }
    }
}
