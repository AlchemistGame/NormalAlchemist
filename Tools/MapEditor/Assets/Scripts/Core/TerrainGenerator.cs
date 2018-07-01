using UnityEngine;
using System.Collections;

namespace VoxelFramework
{

    /// <summary>
    /// 用于在新建地图时初始化一大块平地
    /// </summary>
    public class TerrainGenerator : MonoBehaviour
    {

        protected Chunk chunk;

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

        public virtual void GenerateVoxelData()
        {

        }
    }

}