using UnityEngine;

// Triggers chunk spawning around the player.
public class ChunkLoader : MonoBehaviour
{
    private VoxelPos LastPos;
    private VoxelPos currentPos;

    public void Update()
    {
        // don't load chunks if engine isn't initialized yet
        if (!MapEngine.Initialized || !ChunkManager.Initialized)
        {
            return;
        }

        // track which chunk we're currently in. If it's different from previous frame, spawn chunks at current position.
        currentPos = MapEngine.PositionToChunkIndex(transform.position);

        if (currentPos.IsEqual(LastPos) == false)
        {
            ChunkManager.SpawnChunks(currentPos.x, currentPos.y, currentPos.z);
        }

        LastPos = currentPos;

    }
}