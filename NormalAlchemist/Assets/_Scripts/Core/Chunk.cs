using System.Collections;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    /// <summary>
    /// 0 - empty block( 空的方块 )
    /// 1 - grass block
    /// ...
    /// </summary>
    public ushort[] VoxelData;

    public VoxelPos ChunkIndex; // corresponds to the position of the chunk
    public Chunk[] NeighborChunks; // references to GameObjects of neighbor chunks

    /// <summary>
    /// 这个 chunk 是不是空的( 没有创建任何方块 )
    /// 设为 true 时将不参与 mesh 的重建
    /// </summary>
    public bool Empty;

    // Settings & flags
    public bool Fresh = true;
    public bool DisableMesh; // for chunks spawned from UniblocksServer; if true, the chunk will not build a mesh
    private bool FlaggedToRemove;

    // update queue
    public bool FlaggedToUpdate,
    InUpdateQueue,
    VoxelsDone; // true when this chunk has finished generating or loading voxel data


    // Semi-constants
    public int SideLength;
    private int SquaredSideLength;

    private ChunkMeshCreator MeshCreator;

    // object prefabs
    public GameObject MeshContainer, ChunkCollider;



    // ==== maintenance ===========================================================================================

    public void Awake()
    { // chunk initialization (load/generate data, set position, etc.)

        // Set variables
        ChunkIndex = new VoxelPos(transform.position);

        NeighborChunks = new Chunk[6]; // 0 = up, 1 = down, 2 = right, 3 = left, 4 = forward, 5 = back
        MeshCreator = GetComponent<ChunkMeshCreator>();
        Fresh = true;

        // Register chunk
        ChunkManager.RegisterChunk(this);

        // Set actual position
        transform.position = ChunkIndex.ToVector3() * SideLength;

        // multiply by scale
        transform.position = new Vector3(transform.position.x * transform.localScale.x, transform.position.y * transform.localScale.y, transform.position.z * transform.localScale.z);

        // Grab voxel data
        if (MapEngine.SaveVoxelData && TryLoadVoxelData() == true)
        {
            // data is loaded through TryLoadVoxelData()
        }
        else
        {
            GenerateVoxelData();
        }

    }

    public bool TryLoadVoxelData()
    { // returns true if data was loaded successfully, false if data was not found
        return GetComponent<ChunkDataFiles>().LoadData();
    }

    public void GenerateVoxelData()
    {
        GetComponent<TerrainGenerator>().InitializeGenerator();
    }

    public void AddToQueueWhenReady()
    { // adds chunk to the UpdateQueue when this chunk and all known neighbors have their data ready
        StartCoroutine(DoAddToQueueWhenReady());
    }
    private IEnumerator DoAddToQueueWhenReady()
    {
        while (VoxelsDone == false || AllNeighborsHaveData() == false)
        {
            if (ChunkManager.StopSpawning)
            { // interrupt if the chunk spawn sequence is stopped. This will be restarted in the correct order from ChunkManager
                yield break;
            }
            yield return new WaitForEndOfFrame();

        }
        ChunkManager.AddChunkToUpdateQueue(this);
    }

    private bool AllNeighborsHaveData()
    { // returns false if at least one neighbor is known but doesn't have data ready yet
        foreach (Chunk neighbor in NeighborChunks)
        {
            if (neighbor != null)
            {
                if (neighbor.VoxelsDone == false)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void OnDestroy()
    {
        ChunkManager.UnregisterChunk(this);
    }


    // ==== data =======================================================================================

    public void ClearVoxelData()
    {
        VoxelData = new ushort[SideLength * SideLength * SideLength];
    }

    public int GetDataLength()
    {
        return VoxelData.Length;
    }


    // ushort data 表示 Engine 类 Blocks 列表中的 index
    public void SetVoxelSimple(int rawIndex, ushort data)
    {
        VoxelData[rawIndex] = data;
    }
    public void SetVoxelSimple(int x, int y, int z, ushort data)
    {
        VoxelData[(z * SquaredSideLength) + (y * SideLength) + x] = data;
    }
    public void SetVoxelSimple(VoxelPos index, ushort data)
    {
        VoxelData[(index.z * SquaredSideLength) + (index.y * SideLength) + index.x] = data;
    }

    // 获取 chunk 中指定体素方块的数据
    public ushort GetVoxelSimple(int rawIndex)
    {
        return VoxelData[rawIndex];
    }
    public ushort GetVoxelSimple(int x, int y, int z)
    {
        return VoxelData[(z * SquaredSideLength) + (y * SideLength) + x];
    }
    public ushort GetVoxelSimple(VoxelPos index)
    {
        return VoxelData[(index.z * SquaredSideLength) + (index.y * SideLength) + index.x];
    }

    // ==== Flags =======================================================================================

    public void FlagToRemove()
    {
        FlaggedToRemove = true;
    }

    // ==== Update ====

    public void Update()
    {
        ChunkManager.SavesThisFrame = 0;
    }

    public void LateUpdate()
    {
        // 当距离中心点较远时, 该 chunk 将被销毁( 省得渲染占资源 )
        // 销毁之前先存下该 chunk 的数据
        if (FlaggedToRemove)
        {
            if (MapEngine.SaveVoxelData)
            {
                // 保存流程走完了( 可以销毁了 )
                if (ChunkDataFiles.SavingChunks == false)
                {
                    if (ChunkManager.SavesThisFrame < MapEngine.MaxChunkSaves)
                    {
                        ChunkManager.SavesThisFrame++;
                        SaveData();
                        Destroy(this.gameObject);
                    }
                }
            }
            // if saving is disabled, destroy immediately
            else
            {
                Destroy(this.gameObject);
            }

        }
    }

    public void RebuildMesh()
    {
        //MeshCreator.RebuildMesh();
        ConnectNeighbors();
    }


    private void SaveData()
    {
        if (MapEngine.SaveVoxelData == false)
        {
            Debug.LogWarning("Uniblocks: Saving is disabled. You can enable it in the Engine Settings.");
            return;
        }

        GetComponent<ChunkDataFiles>().SaveData();
    }



    // ==== Neighbors =======================================================================================

    public void ConnectNeighbors()
    { // update the mesh on all neighbors that have a mesh but don't know about this chunk yet, and also pass them the reference to this chunk

        int loop = 0;
        int i = loop;

        while (loop < 6)
        {
            if (loop % 2 == 0)
            { // for even indexes, add one; for odd, subtract one (because the neighbors are in opposite direction to this chunk)
                i = loop + 1;
            }
            else
            {
                i = loop - 1;
            }

            if (NeighborChunks[loop] != null && NeighborChunks[loop].gameObject.GetComponent<MeshFilter>().sharedMesh != null)
            {
                if (NeighborChunks[loop].NeighborChunks[i] == null)
                {
                    NeighborChunks[loop].AddToQueueWhenReady();
                    NeighborChunks[loop].NeighborChunks[i] = this;
                }
            }

            loop++;
        }
    }

    // assign the neighbor chunk gameobjects to the NeighborChunks array
    public void GetNeighbors()
    {
        int x = ChunkIndex.x;
        int y = ChunkIndex.y;
        int z = ChunkIndex.z;

        if (NeighborChunks[0] == null) NeighborChunks[0] = ChunkManager.GetChunkComponent(x, y + 1, z);
        if (NeighborChunks[1] == null) NeighborChunks[1] = ChunkManager.GetChunkComponent(x, y - 1, z);
        if (NeighborChunks[2] == null) NeighborChunks[2] = ChunkManager.GetChunkComponent(x + 1, y, z);
        if (NeighborChunks[3] == null) NeighborChunks[3] = ChunkManager.GetChunkComponent(x - 1, y, z);
        if (NeighborChunks[4] == null) NeighborChunks[4] = ChunkManager.GetChunkComponent(x, y, z + 1);
        if (NeighborChunks[5] == null) NeighborChunks[5] = ChunkManager.GetChunkComponent(x, y, z - 1);

    }

    public VoxelPos GetAdjacentIndex(VoxelPos index, Direction direction)
    {
        return GetAdjacentIndex(index.x, index.y, index.z, direction);
    }

    public VoxelPos GetAdjacentIndex(int x, int y, int z, Direction direction)
    { // converts x,y,z, direction into a specific index

        if (direction == Direction.down) return new VoxelPos(x, y - 1, z);
        else if (direction == Direction.up) return new VoxelPos(x, y + 1, z);
        else if (direction == Direction.left) return new VoxelPos(x - 1, y, z);
        else if (direction == Direction.right) return new VoxelPos(x + 1, y, z);
        else if (direction == Direction.back) return new VoxelPos(x, y, z - 1);
        else if (direction == Direction.forward) return new VoxelPos(x, y, z + 1);


        else
        {
            Debug.LogError("Chunk.GetAdjacentIndex failed! Returning default index.");
            return new VoxelPos(x, y, z);
        }
    }
}
