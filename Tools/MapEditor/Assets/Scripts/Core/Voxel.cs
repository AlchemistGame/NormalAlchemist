using UnityEngine;
using System.Collections;

namespace VoxelFramework
{

    public class Voxel : MonoBehaviour
    {

        public string VName;
        public Mesh VMesh;
        public bool VCustomMesh;

        /// <summary>
        /// 此方块是否给每一面都指定了一张纹理, false-此方块的六个面用的同一张纹理-VTexture.Length==1
        /// </summary>
        public bool VCustomSides;

        /// <summary>
        /// Vector2: texture sheet 中的坐标
        /// 以 block 的一个面大小为单位( 而非像素 )
        /// 从左下角开始计算, 如 (0, 0) 表示左下角第一个方块面
        /// 数组索引对应这个体素方块的各个面, 如 VTexture[0] is the up-facing texture
        /// </summary>
        public Vector2[] VTexture;

        public Transparency VTransparency;
        public ColliderType VColliderType;
        public int VSubmeshIndex;
        public MeshRotation VRotation;


        public static void DestroyBlock(VoxelInfo voxelInfo)
        {
            GameObject voxelObject = Instantiate(Engine.GetVoxelGameObject(voxelInfo.GetVoxel())) as GameObject;
            if (voxelObject.GetComponent<VoxelEvents>() != null)
            {
                voxelObject.GetComponent<VoxelEvents>().OnBlockDestroy(voxelInfo);
            }
            voxelInfo.chunk.SetVoxel(voxelInfo.index, 0, true);
            Destroy(voxelObject);
        }

        public static void PlaceBlock(VoxelInfo voxelInfo, ushort data)
        {
            voxelInfo.chunk.SetVoxel(voxelInfo.index, data, true);

            GameObject voxelObject = Instantiate(Engine.GetVoxelGameObject(data)) as GameObject;
            if (voxelObject.GetComponent<VoxelEvents>() != null)
            {
                voxelObject.GetComponent<VoxelEvents>().OnBlockPlace(voxelInfo);
            }
            Destroy(voxelObject);
        }

        public static void ChangeBlock(VoxelInfo voxelInfo, ushort data)
        {
            voxelInfo.chunk.SetVoxel(voxelInfo.index, data, true);

            GameObject voxelObject = Instantiate(Engine.GetVoxelGameObject(data)) as GameObject;
            if (voxelObject.GetComponent<VoxelEvents>() != null)
            {
                voxelObject.GetComponent<VoxelEvents>().OnBlockChange(voxelInfo);
            }
            Destroy(voxelObject);
        }


        // block editor functions
        public ushort GetID()
        {
            return ushort.Parse(this.gameObject.name.Split('_')[1]);

        }

        public void SetID(ushort id)
        {
            this.gameObject.name = "block_" + id.ToString();
        }

    }

}