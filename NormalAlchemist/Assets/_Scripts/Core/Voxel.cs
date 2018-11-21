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
        /// �˷����Ƿ��ÿһ�涼ָ����һ������, false-�˷�����������õ�ͬһ������-VTexture.Length==1
        /// </summary>
        public bool VCustomSides;

        /// <summary>
        /// Vector2: texture sheet �е�����
        /// �� block ��һ�����СΪ��λ( �������� )
        /// �����½ǿ�ʼ����, �� (0, 0) ��ʾ���½ǵ�һ��������
        /// ����������Ӧ������ط���ĸ�����, �� VTexture[0] is the up-facing texture
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