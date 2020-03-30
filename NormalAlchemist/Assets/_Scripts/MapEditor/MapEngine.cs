using System.Collections.Generic;
using UnityEngine;

// enums
public enum Facing
{
    up, down, right, left, forward, back
}

public enum Transparency
{
    solid, semiTransparent, transparent
}
/// <summary>
/// 如 Box Collider, Sphere Collider, Capsule Collider 都被称为 primitive collider,
/// 将任意个数 primitive collider 添加到同一物体上, 组成一个完整的 compound collider,
/// 一个物体的 collider, 通常不需要与 mesh 保持完全相同的形状, 一个粗糙的近似就够用了.
/// 只有当 compound collider 不够用时, 才会转用 mesh collider 来完全模拟 mesh 的形状.
/// </summary>
public enum ColliderType
{
    // 在体素地图里, 一般 1x1x1 的方块加上贴图就行了
    cube,
    // 特殊情况下可能需要特殊的 mesh 来代替默认 block
    mesh,
    // 生成一个 cube Trigger( 一种特殊的 collider ), 仍能监测到其他 collider enters its space 的情况, 并触发脚本里的 OnTriggerEnter 事件,
    // 但不会产生物理层面的碰撞
    none,
}


public class MapEngine : MonoBehaviour
{
    // file paths
    public static string WorldName, WorldPath;
    public string lWorldName = "TestWorld";

    // voxels
    public static GameObject[] Blocks;

    /// <summary>
    /// The maximum positive and negative vertical chunk index of spawned chunks.
    /// 此值以 chunk index 为单位, 如值为 3 表示在 Y 坐标方向, 生成的 chunk 肯定在 [-3, 3] 之间
    /// </summary>
    public static int HeightRange;

    /// <summary>
    /// 围绕 spawn point, 以 ChunkSpawnDistance 为半径, 生成 terrain chunks
    /// 此值以 chunk 为单位, 如值为 7 表示在以 origin point 为中心, 2x7 chunks 为边长的立方体范围内生成 chunk( Y 方向上受到 HeightRange 的限制 )
    /// </summary>
    public static int ChunkSpawnDistance;

    /// <summary>
    /// 当之前已生成的一个 chunk 距离 origin point 超过 (ChunkSpawnDistance + AdditionalDespawnDistance)
    /// 就销毁这个 chunk( 只是销毁场景里的 mesh, 该 chunk 的数据会保留下来 )
    /// </summary>
    public static int ChunkDespawnDistance;

    /// <summary>
    /// the length of the side of each chunk (in voxels).
    /// 如值为 8 表示 1 chunk = 8x8x8 cubes
    /// </summary>
    public static int ChunkSideLength;
    public int lHeightRange, lChunkSpawnDistance, lChunkSideLength, lChunkDespawnDistance;

    /// <summary>
    /// 现在 chunk 所用的材质上, texture 大小为 512x512, 里面按 blocks 划分为 8x8
    /// 一个 block 单元占整张 texture 的比例为 1/8=0.125
    /// </summary>
    public static float TextureUnit;
    public static float TexturePadding;
    public float lTextureUnit, lTexturePadding;

    // performance settings
    public static int TargetFPS, MaxChunkSaves;
    public int lTargetFPS, lMaxChunkSaves;

    // global settings
    public static bool ShowBorderFaces, GenerateColliders, SaveVoxelData;
    public bool lShowBorderFaces, lGenerateColliders, lSaveVoxelData;

    // other
    public static int SquaredSideLength;
    public static MapEngine EngineInstance;
    public static ChunkManager ChunkManagerInstance;

    public static Vector3 ChunkScale;

    public static bool Initialized;



    // ==== initialization ====
    public void Awake()
    {
        MapEngine.EngineInstance = this;
        MapEngine.ChunkManagerInstance = GetComponent<ChunkManager>();

        WorldName = lWorldName;
        UpdateWorldPath();

        TargetFPS = lTargetFPS;
        MaxChunkSaves = lMaxChunkSaves;

        TextureUnit = lTextureUnit;
        TexturePadding = lTexturePadding;
        GenerateColliders = lGenerateColliders;
        ShowBorderFaces = lShowBorderFaces;
        SaveVoxelData = lSaveVoxelData;

        ChunkSpawnDistance = lChunkSpawnDistance;
        HeightRange = lHeightRange;
        ChunkDespawnDistance = lChunkDespawnDistance;

        ChunkSideLength = lChunkSideLength;
        SquaredSideLength = lChunkSideLength * lChunkSideLength;

        ChunkDataFiles.LoadedRegions = new Dictionary<string, string[]>();
        ChunkDataFiles.TempChunkData = new Dictionary<string, string>();


        // set layer
        if (LayerMask.LayerToName(26) != "" && LayerMask.LayerToName(26) != "UniblocksNoCollide")
        {
            Debug.LogWarning("Uniblocks: Layer 26 is reserved for Uniblocks; it is automatically set to ignore collision with all layers.");
        }
        for (int i = 0; i < 31; i++)
        {
            Physics.IgnoreLayerCollision(i, 26);
        }

        // check settings
        if (MapEngine.ChunkSideLength < 1)
        {
            Debug.LogError("Uniblocks: Chunk side length must be greater than 0!");
            Debug.Break();
        }

        if (MapEngine.ChunkSpawnDistance < 1)
        {
            MapEngine.ChunkSpawnDistance = 0;
            Debug.LogWarning("Uniblocks: Chunk spawn distance is 0. No chunks will spawn!");
        }

        if (MapEngine.HeightRange < 0)
        {
            MapEngine.HeightRange = 0;
            Debug.LogWarning("Uniblocks: Chunk height range can't be a negative number! Setting chunk height range to 0.");
        }

        // check materials
        GameObject chunkPrefab = GetComponent<ChunkManager>().ChunkObject;
        int materialCount = chunkPrefab.GetComponent<Renderer>().sharedMaterials.Length - 1;

        MapEngine.Initialized = true;
    }

    // ==== world data ====

    // 存储地图数据的路径
    private static void UpdateWorldPath()
    {
        WorldPath = Application.dataPath + "/../Worlds/" + MapEngine.WorldName + "/";
    }

    public static void SetWorldName(string worldName)
    {
        MapEngine.WorldName = worldName;
        UpdateWorldPath();
    }

    // 一帧存一个 chunk, 将整个地图存储过程分成好几帧来完成, 防止游戏过卡
    public static void SaveWorld()
    {
        MapEngine.EngineInstance.StartCoroutine(ChunkDataFiles.SaveAllChunks());
    }

    // 一次性存完所有数据
    public static void SaveWorldInstant()
    {
        ChunkDataFiles.SaveAllChunksInstant();
    }

    // ==== other ====	

    public static GameObject GetVoxelGameObject(ushort voxelId)
    {
        try
        {
            if (voxelId == ushort.MaxValue) voxelId = 0;
            GameObject voxelObject = MapEngine.Blocks[voxelId];
            if (voxelObject.GetComponent<Voxel>() == null)
            {
                Debug.LogError("Uniblocks: Voxel id " + voxelId + " does not have the Voxel component attached!");
                return MapEngine.Blocks[0];
            }
            else
            {
                return voxelObject;
            }

        }
        catch (System.Exception)
        {
            Debug.LogError("Uniblocks: Invalid voxel id: " + voxelId);
            return MapEngine.Blocks[0];
        }
    }

    public static Voxel GetVoxelType(ushort voxelId)
    {
        try
        {
            if (voxelId == ushort.MaxValue) voxelId = 0;
            Voxel voxel = MapEngine.Blocks[voxelId].GetComponent<Voxel>();
            if (voxel == null)
            {
                Debug.LogError("Uniblocks: Voxel id " + voxelId + " does not have the Voxel component attached!");
                return null;
            }
            else
            {
                return voxel;
            }
        }
        catch (System.Exception)
        {
            Debug.LogError("Uniblocks: Invalid voxel id: " + voxelId);
            return null;
        }
    }

    public static VoxelPos PositionToChunkIndex(Vector3 position)
    {
        VoxelPos chunkIndex = new VoxelPos(Mathf.RoundToInt(position.x / MapEngine.ChunkScale.x) / MapEngine.ChunkSideLength,
                                      Mathf.RoundToInt(position.y / MapEngine.ChunkScale.y) / MapEngine.ChunkSideLength,
                                      Mathf.RoundToInt(position.z / MapEngine.ChunkScale.z) / MapEngine.ChunkSideLength);
        return chunkIndex;
    }

    public static GameObject PositionToChunk(Vector3 position)
    {
        VoxelPos chunkIndex = new VoxelPos(Mathf.RoundToInt(position.x / MapEngine.ChunkScale.x) / MapEngine.ChunkSideLength,
                                      Mathf.RoundToInt(position.y / MapEngine.ChunkScale.y) / MapEngine.ChunkSideLength,
                                      Mathf.RoundToInt(position.z / MapEngine.ChunkScale.z) / MapEngine.ChunkSideLength);
        return ChunkManager.GetChunk(chunkIndex);

    }
}