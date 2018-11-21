using UnityEngine;
using System.Collections;

// Triggers chunk spawning around the player.

namespace VoxelFramework
{
    public class ChunkLoader : MonoBehaviour
    {
        private Index LastPos;
        private Index currentPos;
        
        public void Update()
        {
            // don't load chunks if engine isn't initialized yet
            if (!Engine.Initialized || !ChunkManager.Initialized)
            {
                return;
            }

            // track which chunk we're currently in. If it's different from previous frame, spawn chunks at current position.
            currentPos = Engine.PositionToChunkIndex(transform.position);

            if (currentPos.IsEqual(LastPos) == false)
            {
                ChunkManager.SpawnChunks(currentPos.x, currentPos.y, currentPos.z);
            }

            LastPos = currentPos;

        }
    }

}