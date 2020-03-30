using UnityEngine;

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

    /// <summary>
    /// ChunkIndex.y 有正负之分, [-HeightRange, HeightRange]
    /// 而一个 chunk 内的方块坐标, 都是从 0 起步向上
    /// </summary>
    public void GenerateVoxelData()
    {
        // 只有最底一层的 chunk 初始化一个平台
        int chunky = chunk.ChunkIndex.y;
        int lowestY = -MapEngine.HeightRange;
        if (chunky > lowestY)
        {
            return;
        }

        int SideLength = MapEngine.ChunkSideLength;

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