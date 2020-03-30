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
/// �� Box Collider, Sphere Collider, Capsule Collider ������Ϊ primitive collider,
/// ��������� primitive collider ��ӵ�ͬһ������, ���һ�������� compound collider,
/// һ������� collider, ͨ������Ҫ�� mesh ������ȫ��ͬ����״, һ���ֲڵĽ��ƾ͹�����.
/// ֻ�е� compound collider ������ʱ, �Ż�ת�� mesh collider ����ȫģ�� mesh ����״.
/// </summary>
public enum ColliderType
{
    // �����ص�ͼ��, һ�� 1x1x1 �ķ��������ͼ������
    cube,
    // ��������¿�����Ҫ����� mesh ������Ĭ�� block
    mesh,
    // ����һ�� cube Trigger( һ������� collider ), ���ܼ�⵽���� collider enters its space �����, �������ű���� OnTriggerEnter �¼�,
    // �������������������ײ
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
    /// ��ֵ�� chunk index Ϊ��λ, ��ֵΪ 3 ��ʾ�� Y ���귽��, ���ɵ� chunk �϶��� [-3, 3] ֮��
    /// </summary>
    public static int HeightRange;

    /// <summary>
    /// Χ�� spawn point, �� ChunkSpawnDistance Ϊ�뾶, ���� terrain chunks
    /// ��ֵ�� chunk Ϊ��λ, ��ֵΪ 7 ��ʾ���� origin point Ϊ����, 2x7 chunks Ϊ�߳��������巶Χ������ chunk( Y �������ܵ� HeightRange ������ )
    /// </summary>
    public static int ChunkSpawnDistance;

    /// <summary>
    /// ��֮ǰ�����ɵ�һ�� chunk ���� origin point ���� (ChunkSpawnDistance + AdditionalDespawnDistance)
    /// ��������� chunk( ֻ�����ٳ������ mesh, �� chunk �����ݻᱣ������ )
    /// </summary>
    public static int ChunkDespawnDistance;

    /// <summary>
    /// the length of the side of each chunk (in voxels).
    /// ��ֵΪ 8 ��ʾ 1 chunk = 8x8x8 cubes
    /// </summary>
    public static int ChunkSideLength;
    public int lHeightRange, lChunkSpawnDistance, lChunkSideLength, lChunkDespawnDistance;

    /// <summary>
    /// ���� chunk ���õĲ�����, texture ��СΪ 512x512, ���水 blocks ����Ϊ 8x8
    /// һ�� block ��Ԫռ���� texture �ı���Ϊ 1/8=0.125
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

    // �洢��ͼ���ݵ�·��
    private static void UpdateWorldPath()
    {
        WorldPath = Application.dataPath + "/../Worlds/" + MapEngine.WorldName + "/";
    }

    public static void SetWorldName(string worldName)
    {
        MapEngine.WorldName = worldName;
        UpdateWorldPath();
    }

    // һ֡��һ�� chunk, ��������ͼ�洢���̷ֳɺü�֡�����, ��ֹ��Ϸ����
    public static void SaveWorld()
    {
        MapEngine.EngineInstance.StartCoroutine(ChunkDataFiles.SaveAllChunks());
    }

    // һ���Դ�����������
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