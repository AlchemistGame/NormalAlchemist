using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Controls spawning and destroying chunks.
public class ChunkManager : MonoBehaviour
{
    public GameObject ChunkObject; // Chunk prefab

    // chunk lists
    public static Dictionary<string, Chunk> Chunks;
    private static List<Chunk> ChunkUpdateQueue; // stores chunks ordered by update priority. Processed in the ProcessChunkQueue loop
    private static List<Chunk> ChunksToDestroy; // chunks to be destroyed at the end of SpawnChunks

    public static int SavesThisFrame;


    // global flags
    public static bool SpawningChunks; // true if the ChunkManager is currently spawning chunks
    public static bool StopSpawning; // when true, the current SpawnChunks sequence is aborted (and this is set back to false afterwards)
    public static bool Initialized;

    // local flags	
    private bool Done;
    private VoxelPos LastRequest;
    private float targetFrameDuration;
    private Stopwatch frameStopwatch;
    private int SpawnQueue;

    void Start()
    {
        targetFrameDuration = 1f / MapEngine.TargetFPS;

        ChunkManager.Chunks = new Dictionary<string, Chunk>();
        ChunkManager.ChunkUpdateQueue = new List<Chunk>();
        frameStopwatch = new Stopwatch();

        MapEngine.ChunkScale = ChunkObject.transform.localScale;
        // set correct scale of trigger collider and additional mesh collider
        ChunkObject.GetComponent<Chunk>().MeshContainer.transform.localScale = ChunkObject.transform.localScale;
        ChunkObject.GetComponent<Chunk>().ChunkCollider.transform.localScale = ChunkObject.transform.localScale;

        Done = true;
        ChunkManager.SpawningChunks = false;

        ChunkManager.Initialized = true;
    }

    private void ResetFrameStopwatch()
    {
        frameStopwatch.Stop();
        frameStopwatch.Reset();
        frameStopwatch.Start();
    }

    public static void AddChunkToUpdateQueue(Chunk chunk)
    {
        if (ChunkUpdateQueue.Contains(chunk) == false)
        {
            ChunkUpdateQueue.Add(chunk);
        }
    }

    // called from the SpawnChunks loop to update chunk meshes
    private void ProcessChunkQueue()
    {
        // update the first chunk and remove it from the queue
        Chunk currentChunk = ChunkUpdateQueue[0];

        if (!currentChunk.Empty && !currentChunk.DisableMesh)
        {
            currentChunk.RebuildMesh();
        }
        currentChunk.Fresh = false;
        ChunkUpdateQueue.RemoveAt(0);
    }

    private bool ProcessChunkQueueLoopActive;
    private IEnumerator ProcessChunkQueueLoop()
    { // called from Update when SpawnChunks is not running
        ProcessChunkQueueLoopActive = true;
        while (ChunkUpdateQueue.Count > 0 && !SpawningChunks && !StopSpawning)
        {
            ProcessChunkQueue();
            if (frameStopwatch.Elapsed.TotalSeconds >= targetFrameDuration)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        ProcessChunkQueueLoopActive = false;
    }

    public static void RegisterChunk(Chunk chunk)
    { // adds a reference to the chunk to the global chunk list
        ChunkManager.Chunks.Add(chunk.ChunkIndex.ToString(), chunk);
    }

    public static void UnregisterChunk(Chunk chunk)
    {
        ChunkManager.Chunks.Remove(chunk.ChunkIndex.ToString());
    }

    public static GameObject GetChunk(int x, int y, int z)
    { // returns the gameObject of the chunk with the specified x,y,z, or returns null if the object is not instantiated

        return GetChunk(new VoxelPos(x, y, z));
    }

    public static GameObject GetChunk(VoxelPos index)
    {

        Chunk chunk = ChunkManager.GetChunkComponent(index);
        if (chunk == null)
        {
            return null;
        }
        else
        {
            return chunk.gameObject;
        }

    }

    public static Chunk GetChunkComponent(int x, int y, int z)
    {

        return GetChunkComponent(new VoxelPos(x, y, z));
    }

    public static Chunk GetChunkComponent(VoxelPos index)
    {

        string indexString = index.ToString();
        if (ChunkManager.Chunks.ContainsKey(indexString))
        {
            return ChunkManager.Chunks[indexString];
        }
        else
        {
            return null;
        }
    }


    // ==== spawn chunks functions ====

    public static GameObject SpawnChunk(int x, int y, int z)
    { // spawns a single chunk (only if it's not already spawned)

        GameObject chunk = ChunkManager.GetChunk(x, y, z);
        if (chunk == null)
        {
            return MapEngine.ChunkManagerInstance.DoSpawnChunk(new VoxelPos(x, y, z));
        }
        else return chunk;
    }

    public static GameObject SpawnChunk(VoxelPos index)
    { // spawns a single chunk (only if it's not already spawned)

        GameObject chunk = ChunkManager.GetChunk(index);
        if (chunk == null)
        {
            return MapEngine.ChunkManagerInstance.DoSpawnChunk(index);
        }
        else return chunk;
    }

    GameObject DoSpawnChunk(VoxelPos index)
    {
        GameObject chunkObject = Instantiate(ChunkObject, index.ToVector3(), transform.rotation) as GameObject;
        Chunk chunk = chunkObject.GetComponent<Chunk>();
        AddChunkToUpdateQueue(chunk);
        return chunkObject;
    }

    public static void SpawnChunks(float x, float y, float z)
    { // take world pos, convert to chunk index
        VoxelPos index = MapEngine.PositionToChunkIndex(new Vector3(x, y, z));
        MapEngine.ChunkManagerInstance.TrySpawnChunks(index);
    }
    public static void SpawnChunks(Vector3 position)
    {
        VoxelPos index = MapEngine.PositionToChunkIndex(position);
        MapEngine.ChunkManagerInstance.TrySpawnChunks(index);
    }

    // take chunk index, no conversion needed
    public static void SpawnChunks(int x, int y, int z)
    {
        MapEngine.ChunkManagerInstance.TrySpawnChunks(x, y, z);
    }

    public static void SpawnChunks(VoxelPos index)
    {
        MapEngine.ChunkManagerInstance.TrySpawnChunks(index.x, index.y, index.z);
    }

    private void TrySpawnChunks(VoxelPos index)
    {
        TrySpawnChunks(index.x, index.y, index.z);
    }

    private void TrySpawnChunks(int x, int y, int z)
    {
        // if we're not spawning chunks at the moment, just spawn them normally
        if (Done == true)
        {
            StartSpawnChunks(x, y, z);
        }
        // if we are spawning chunks already, flag to spawn again once the previous round is finished using the last requested position as origin.
        else
        {
            LastRequest = new VoxelPos(x, y, z);
            SpawnQueue = 1;
            StopSpawning = true;
            ChunkUpdateQueue.Clear();
        }
    }

    public void Update()
    {

        if (SpawnQueue == 1 && Done == true)
        { // if there is a chunk spawn queued up, and if the previous one has finished, run the queued chunk spawn.
            SpawnQueue = 0;
            StartSpawnChunks(LastRequest.x, LastRequest.y, LastRequest.z);
        }

        // if not currently spawning chunks, process any queued chunks here instead
        if (!SpawningChunks && !ProcessChunkQueueLoopActive && ChunkUpdateQueue != null && ChunkUpdateQueue.Count > 0)
        {
            StartCoroutine(ProcessChunkQueueLoop());
        }


        ResetFrameStopwatch();
    }


    private void StartSpawnChunks(int originX, int originY, int originZ)
    {
        ChunkManager.SpawningChunks = true;
        Done = false;

        int range = MapEngine.ChunkSpawnDistance;

        StartCoroutine(SpawnMissingChunks(originX, originY, originZ, range));

    }


    private IEnumerator SpawnMissingChunks(int originX, int originY, int originZ, int range)
    {

        int heightRange = MapEngine.HeightRange;

        // clear update queue - it will be repopulated again in the correct order in the following loop
        ChunkUpdateQueue = new List<Chunk>();

        // flag chunks not in range for removal
        ChunksToDestroy = new List<Chunk>();
        foreach (Chunk chunk in Chunks.Values)
        {
            if (Vector2.Distance(new Vector2(chunk.ChunkIndex.x, chunk.ChunkIndex.z), new Vector2(originX, originZ)) > range + MapEngine.ChunkDespawnDistance)
            {
                ChunksToDestroy.Add(chunk);
            }
            // destroy chunks outside of vertical range
            else if (Mathf.Abs(chunk.ChunkIndex.y - originY) > range + MapEngine.ChunkDespawnDistance)
            {
                ChunksToDestroy.Add(chunk);
            }
        }


        // main loop
        for (int currentLoop = 0; currentLoop <= range; currentLoop++)
        {
            for (var x = originX - currentLoop; x <= originX + currentLoop; x++)
            { // iterate through all potential chunk indexes within range
                for (var y = originY - currentLoop; y <= originY + currentLoop; y++)
                {
                    for (var z = originZ - currentLoop; z <= originZ + currentLoop; z++)
                    {
                        // skip chunks outside of height range
                        if (Mathf.Abs(y) <= heightRange)
                        {
                            // skip corners
                            if (Mathf.Abs(originX - x) + Mathf.Abs(originZ - z) < range * 1.3f)
                            {
                                // pause loop while the queue is not empty
                                while (ChunkUpdateQueue.Count > 0)
                                {
                                    ProcessChunkQueue();
                                    if (frameStopwatch.Elapsed.TotalSeconds >= targetFrameDuration)
                                    {
                                        yield return new WaitForEndOfFrame();
                                    }
                                }

                                Chunk currentChunk = ChunkManager.GetChunkComponent(x, y, z);


                                // chunks that already exist but haven't had their mesh built yet should be added to the update queue
                                if (currentChunk != null)
                                {
                                    if (currentChunk.Fresh)
                                    {
                                        // spawn neighbor chunks
                                        for (int d = 0; d < 6; d++)
                                        {
                                            VoxelPos neighborIndex = currentChunk.ChunkIndex.GetAdjacentIndex((Direction)d);
                                            GameObject neighborChunk = GetChunk(neighborIndex);
                                            if (neighborChunk == null)
                                            {
                                                neighborChunk = Instantiate(ChunkObject, neighborIndex.ToVector3(), transform.rotation) as GameObject;
                                            }
                                            currentChunk.NeighborChunks[d] = neighborChunk.GetComponent<Chunk>(); // always add the neighbor to NeighborChunks, in case it's not there already

                                            // continue loop in next frame if the current frame time is exceeded
                                            if (frameStopwatch.Elapsed.TotalSeconds >= targetFrameDuration)
                                            {
                                                yield return new WaitForEndOfFrame();
                                            }
                                            if (StopSpawning)
                                            {
                                                EndSequence();
                                                yield break;
                                            }
                                        }

                                        if (currentChunk != null)
                                            currentChunk.AddToQueueWhenReady();
                                    }
                                }
                                // if chunk doesn't exist, create new chunk (it adds itself to the update queue when its data is ready)
                                else
                                {
                                    // spawn chunk
                                    GameObject newChunk = Instantiate(ChunkObject, new Vector3(x, y, z), transform.rotation) as GameObject; // Spawn a new chunk.
                                    currentChunk = newChunk.GetComponent<Chunk>();

                                    // spawn neighbor chunks if they're not spawned yet
                                    for (int d = 0; d < 6; d++)
                                    {
                                        VoxelPos neighborIndex = currentChunk.ChunkIndex.GetAdjacentIndex((Direction)d);
                                        GameObject neighborChunk = GetChunk(neighborIndex);
                                        if (neighborChunk == null)
                                        {
                                            neighborChunk = Instantiate(ChunkObject, neighborIndex.ToVector3(), transform.rotation) as GameObject;
                                        }
                                        currentChunk.NeighborChunks[d] = neighborChunk.GetComponent<Chunk>(); // always add the neighbor to NeighborChunks, in case it's not there already

                                        // continue loop in next frame if the current frame time is exceeded
                                        if (frameStopwatch.Elapsed.TotalSeconds >= targetFrameDuration)
                                        {
                                            yield return new WaitForEndOfFrame();
                                        }
                                        if (StopSpawning)
                                        {
                                            EndSequence();
                                            yield break;
                                        }
                                    }

                                    if (currentChunk != null)
                                        currentChunk.AddToQueueWhenReady();

                                }

                            }
                        }


                        // continue loop in next frame if the current frame time is exceeded
                        if (frameStopwatch.Elapsed.TotalSeconds >= targetFrameDuration)
                        {
                            yield return new WaitForEndOfFrame();
                        }
                        if (StopSpawning)
                        {
                            EndSequence();
                            yield break;
                        }


                    }
                }
            }
        }

        yield return new WaitForEndOfFrame();
        EndSequence();
    }





    private void EndSequence()
    {

        ChunkManager.SpawningChunks = false;
        Resources.UnloadUnusedAssets();
        Done = true;
        StopSpawning = false;

        foreach (Chunk chunk in ChunksToDestroy)
        {
            chunk.FlagToRemove();
        }

    }

}

