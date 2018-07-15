using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace VoxelFramework
{

    // enums
    public enum Facing
    {
        up, down, right, left, forward, back
    }
    public enum Direction
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


    public class Engine : MonoBehaviour
    {

        // file paths
        public static string WorldName, WorldPath;
        public string lWorldName = "TestWorld";

        // voxels
        public static GameObject[] Blocks;
        public GameObject[] lBlocks;

        /// <summary>
        /// The maximum positive and negative vertical chunk index of spawned chunks.
        /// ��ֵ�� chunk index Ϊ��λ, ��ֵΪ 3 ��ʾ�� Y ���귽��, ���ɵ� chunk �϶��� [-3, 3] ֮��
        /// </summary>
        public static int HeightRange;

        /// <summary>
        /// Χ�� spawn point, �� ChunkSpawnDistance Ϊ�뾶, ���� terrain chunks
        /// ��ֵ�� chunk Ϊ��λ, ��ֵΪ 7 ��ʾ�� origin point �� 4 ��ˮƽ��������� 7 �� chunk
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
        public static bool ShowBorderFaces, GenerateColliders, SendCameraLookEvents,
        SendCursorEvents, SaveVoxelData;

        public bool lShowBorderFaces, lGenerateColliders, lSendCameraLookEvents,
        lSendCursorEvents, lSaveVoxelData;

        // other
        public static int SquaredSideLength;
        public static Engine EngineInstance;
        public static ChunkManager ChunkManagerInstance;

        public static Vector3 ChunkScale;

        public static bool Initialized;



        // ==== initialization ====
        public void Awake()
        {
            Engine.EngineInstance = this;
            Engine.ChunkManagerInstance = GetComponent<ChunkManager>();

            WorldName = lWorldName;
            UpdateWorldPath();
            
            Engine.Blocks = lBlocks;

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

            SendCameraLookEvents = lSendCameraLookEvents;
            SendCursorEvents = lSendCursorEvents;

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


            // check block array
            if (Engine.Blocks.Length < 1)
            {
                Debug.LogError("Uniblocks: The blocks array is empty! Use the Block Editor to update the blocks array.");
                Debug.Break();
            }

            if (Engine.Blocks[0] == null)
            {
                Debug.LogError("Uniblocks: Cannot find the empty block prefab (id 0)!");
                Debug.Break();
            }
            else if (Engine.Blocks[0].GetComponent<Voxel>() == null)
            {
                Debug.LogError("Uniblocks: Voxel id 0 does not have the Voxel component attached!");
                Debug.Break();
            }

            // check settings
            if (Engine.ChunkSideLength < 1)
            {
                Debug.LogError("Uniblocks: Chunk side length must be greater than 0!");
                Debug.Break();
            }

            if (Engine.ChunkSpawnDistance < 1)
            {
                Engine.ChunkSpawnDistance = 0;
                Debug.LogWarning("Uniblocks: Chunk spawn distance is 0. No chunks will spawn!");
            }

            if (Engine.HeightRange < 0)
            {
                Engine.HeightRange = 0;
                Debug.LogWarning("Uniblocks: Chunk height range can't be a negative number! Setting chunk height range to 0.");
            }

            // check materials
            GameObject chunkPrefab = GetComponent<ChunkManager>().ChunkObject;
            int materialCount = chunkPrefab.GetComponent<Renderer>().sharedMaterials.Length - 1;

            for (ushort i = 0; i < Engine.Blocks.Length; i++)
            {

                if (Engine.Blocks[i] != null)
                {
                    Voxel voxel = Engine.Blocks[i].GetComponent<Voxel>();

                    if (voxel.VSubmeshIndex < 0)
                    {
                        Debug.LogError("Uniblocks: Voxel " + i + " has a material index lower than 0! Material index must be 0 or greater.");
                        Debug.Break();
                    }

                    if (voxel.VSubmeshIndex > materialCount)
                    {
                        Debug.LogError("Uniblocks: Voxel " + i + " uses material index " + voxel.VSubmeshIndex + ", but the chunk prefab only has " + (materialCount + 1) + " material(s) attached. Set a lower material index or attach more materials to the chunk prefab.");
                        Debug.Break();
                    }
                }
            }

            // check anti-aliasing
            if (QualitySettings.antiAliasing > 0)
            {
                Debug.LogWarning("Uniblocks: Anti-aliasing is enabled. This may cause seam lines to appear between blocks. If you see lines between blocks, try disabling anti-aliasing, switching to deferred rendering path, or adding some texture padding in the engine settings.");
            }


            Engine.Initialized = true;

        }

        // ==== world data ====

        // �洢��ͼ���ݵ�·��
        private static void UpdateWorldPath()
        {
            WorldPath = Application.dataPath + "/../Worlds/" + Engine.WorldName + "/";
        }

        public static void SetWorldName(string worldName)
        {
            Engine.WorldName = worldName;
            UpdateWorldPath();
        }

        // һ֡��һ�� chunk, ��������ͼ�洢���̷ֳɺü�֡�����, ��ֹ��Ϸ����
        public static void SaveWorld()
        {
            Engine.EngineInstance.StartCoroutine(ChunkDataFiles.SaveAllChunks());
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
                GameObject voxelObject = Engine.Blocks[voxelId];
                if (voxelObject.GetComponent<Voxel>() == null)
                {
                    Debug.LogError("Uniblocks: Voxel id " + voxelId + " does not have the Voxel component attached!");
                    return Engine.Blocks[0];
                }
                else
                {
                    return voxelObject;
                }

            }
            catch (System.Exception)
            {
                Debug.LogError("Uniblocks: Invalid voxel id: " + voxelId);
                return Engine.Blocks[0];
            }
        }

        public static Voxel GetVoxelType(ushort voxelId)
        {
            try
            {
                if (voxelId == ushort.MaxValue) voxelId = 0;
                Voxel voxel = Engine.Blocks[(int)voxelId].GetComponent<Voxel>();
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



        // a raycast which returns the index of the hit voxel and the gameobject of the hit chunk
        public static VoxelInfo VoxelRaycast(Vector3 origin, Vector3 direction, float range, bool ignoreTransparent)
        {

            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(origin, direction, out hit, range))
            {

                if (hit.collider.GetComponent<Chunk>() != null
                    || hit.collider.GetComponent<ChunkExtension>() != null)
                { // check if we're actually hitting a chunk

                    GameObject hitObject = hit.collider.gameObject;

                    if (hitObject.GetComponent<ChunkExtension>() != null)
                    { // if we hit a mesh container instead of a chunk
                        hitObject = hitObject.transform.parent.gameObject; // swap the mesh container for the actual chunk object
                    }

                    Index hitIndex = hitObject.GetComponent<Chunk>().PositionToVoxelIndex(hit.point, hit.normal, false);

                    if (ignoreTransparent)
                    { // punch through transparent voxels by raycasting again when a transparent voxel is hit
                        ushort hitVoxel = hitObject.GetComponent<Chunk>().GetVoxel(hitIndex.x, hitIndex.y, hitIndex.z);
                        if (Engine.GetVoxelType(hitVoxel).VTransparency != Transparency.solid)
                        { // if the hit voxel is transparent
                            Vector3 newOrigin = hit.point;
                            newOrigin.y -= 0.5f; // push the new raycast down a bit
                            return Engine.VoxelRaycast(newOrigin, Vector3.down, range - hit.distance, true);
                        }
                    }


                    return new VoxelInfo(
                                         hitObject.GetComponent<Chunk>().PositionToVoxelIndex(hit.point, hit.normal, false), // get hit voxel index
                                         hitObject.GetComponent<Chunk>().PositionToVoxelIndex(hit.point, hit.normal, true), // get adjacent voxel index
                                         hitObject.GetComponent<Chunk>()); // get chunk
                }
            }

            // else 
            return null;
        }

        public static VoxelInfo VoxelRaycast(Ray ray, float range, bool ignoreTransparent)
        {
            return Engine.VoxelRaycast(ray.origin, ray.direction, range, ignoreTransparent);
        }




        public static Index PositionToChunkIndex(Vector3 position)
        {
            Index chunkIndex = new Index(Mathf.RoundToInt(position.x / Engine.ChunkScale.x) / Engine.ChunkSideLength,
                                          Mathf.RoundToInt(position.y / Engine.ChunkScale.y) / Engine.ChunkSideLength,
                                          Mathf.RoundToInt(position.z / Engine.ChunkScale.z) / Engine.ChunkSideLength);
            return chunkIndex;
        }

        public static GameObject PositionToChunk(Vector3 position)
        {
            Index chunkIndex = new Index(Mathf.RoundToInt(position.x / Engine.ChunkScale.x) / Engine.ChunkSideLength,
                                          Mathf.RoundToInt(position.y / Engine.ChunkScale.y) / Engine.ChunkSideLength,
                                          Mathf.RoundToInt(position.z / Engine.ChunkScale.z) / Engine.ChunkSideLength);
            return ChunkManager.GetChunk(chunkIndex);

        }

        public static VoxelInfo PositionToVoxelInfo(Vector3 position)
        {
            GameObject chunkObject = Engine.PositionToChunk(position);
            if (chunkObject != null)
            {
                Chunk chunk = chunkObject.GetComponent<Chunk>();
                Index voxelIndex = chunk.PositionToVoxelIndex(position);
                return new VoxelInfo(voxelIndex, chunk);
            }
            else
            {
                return null;
            }

        }

        public static Vector3 VoxelInfoToPosition(VoxelInfo voxelInfo)
        {
            return voxelInfo.chunk.GetComponent<Chunk>().VoxelIndexToPosition(voxelInfo.index);
        }




        // ==== mesh creator ====
        public static Vector2 GetTextureOffset(ushort voxel, Facing facing)
        {
            Voxel voxelType = Engine.GetVoxelType(voxel);
            Vector2[] textureArray = voxelType.VTexture;

            // in case there are no textures defined, return a default texture
            if (textureArray.Length == 0)
            {
                Debug.LogWarning("Uniblocks: Block " + voxel.ToString() + " has no defined textures! Using default texture.");
                return new Vector2(0, 0);
            }
            else if (voxelType.VCustomSides == false)
            {
                return textureArray[0];
            }
            // if we're asking for a texture that's not defined, grab the last defined texture instead
            else if ((int)facing > textureArray.Length - 1)
            {
                return textureArray[textureArray.Length - 1];
            }
            else
            {
                return textureArray[(int)facing];
            }
        }


    }

}