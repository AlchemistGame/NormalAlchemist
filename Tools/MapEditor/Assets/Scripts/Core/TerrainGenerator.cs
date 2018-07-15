using UnityEngine;
using System.Collections;

namespace VoxelFramework
{
    /// <summary>
    /// 用于在新建地图时初始化一大块平地
    /// </summary>
    public class TerrainGenerator : MonoBehaviour
    {

        private Chunk chunk;

        public void InitializeGenerator()
        {
            // get chunk component
            chunk = GetComponent<Chunk>();

            // generate data
            GenerateVoxelData();

            // set empty
            chunk.Empty = true;
            foreach (ushort voxel in chunk.VoxelData)
            {
                if (voxel != 0)
                {
                    chunk.Empty = false;
                    break;
                }
            }

            // flag as done
            chunk.VoxelsDone = true;
        }

        public void GenerateVoxelData()
        {
            //int chunky = chunk.ChunkIndex.y;
            // 只有最底一层的 chunk 初始化一个平台
            //if (chunky > 0)
            //{
            //    return;
            //}

            int SideLength = Engine.ChunkSideLength;

            // for all voxels in the chunk
            for (int x = 0; x < SideLength; x++)
            {
                for (int z = 0; z < SideLength; z++)
                {
                    // 铺一层方块
                    chunk.SetVoxelSimple(x, 0, z, 1);
                }
            }
        }

    }
}