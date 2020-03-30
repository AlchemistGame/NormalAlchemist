using UnityEngine;

public class Voxel : MonoBehaviour
{
    public string VName;
    public Mesh VMesh;
    public bool VCustomMesh;

    public Transparency VTransparency;
    public ColliderType VColliderType;
    public int VSubmeshIndex;
    public MeshRotation VRotation;

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