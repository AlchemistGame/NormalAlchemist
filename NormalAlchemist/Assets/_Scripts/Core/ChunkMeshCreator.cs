using System.Collections.Generic;
using UnityEngine;

// Handles mesh creation and all related functions.
public enum MeshRotation
{
    none, back, left, right
}

public class ChunkMeshCreator : MonoBehaviour
{
    private Chunk chunk;
    private int SideLength;
    private GameObject noCollideCollider;

    /// <summary>
    /// 提供 Cube 顶点数和面数的依据
    /// </summary>
    public Mesh Cube;

    // variables for storing the mesh data
    private List<Vector3> Vertices = new List<Vector3>();
    private List<List<int>> Faces = new List<List<int>>();
    private int FaceCount;

    // variables for storing collider data
    private List<Vector3> SolidColliderVertices = new List<Vector3>();
    private List<int> SolidColliderFaces = new List<int>();
    private int SolidFaceCount;
    private List<Vector3> NoCollideVertices = new List<Vector3>();
    private List<int> NoCollideFaces = new List<int>();
    private int NoCollideFaceCount;

    private bool initialized;

    public void Initialize()
    {
        // set variables
        chunk = GetComponent<Chunk>();
        SideLength = chunk.SideLength;

        // make a list for each material (each material is a submesh)
        for (int i = 0; i < GetComponent<Renderer>().materials.Length; i++)
        {
            Faces.Add(new List<int>());
        }

        initialized = true;
    }


    // ==== Voxel updates =====================================================================================

    //public void RebuildMesh()
    //{
    //    if (!initialized)
    //    {
    //        Initialize();
    //    }

    //    // destroy additional mesh containers
    //    foreach (Transform child in transform)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    int x = 0, y = 0, z = 0;

    //    // Refresh neighbor chunks
    //    chunk.GetNeighbors();

    //    // for each voxel in Voxels, check if any of the voxel's faces are exposed,
    //    // and if so, add their faces to the main mesh arrays (named Vertices and Faces)
    //    while (x < SideLength)
    //    {
    //        while (y < SideLength)
    //        {
    //            while (z < SideLength)
    //            {
    //                ushort voxel = chunk.GetVoxelSimple(x, y, z);

    //                // don't render empty blocks.
    //                if (voxel != 0)
    //                {
    //                    Voxel voxelType = MapEngine.GetVoxelType(voxel);
    //                    // 非特殊 mesh( 标准方块 )
    //                    if (voxelType.VCustomMesh == false)
    //                    {
    //                        //Transparency transparency = Engine.GetVoxelType (chunk.GetVoxel(x,y,z)).VTransparency;
    //                        Transparency transparency = voxelType.VTransparency;
    //                        ColliderType colliderType = voxelType.VColliderType;

    //                        if (CheckAdjacent(x, y, z, Direction.forward, transparency) == true)
    //                            CreateFace(voxel, Facing.forward, colliderType, x, y, z);

    //                        if (CheckAdjacent(x, y, z, Direction.back, transparency) == true)
    //                            CreateFace(voxel, Facing.back, colliderType, x, y, z);

    //                        if (CheckAdjacent(x, y, z, Direction.up, transparency) == true)
    //                            CreateFace(voxel, Facing.up, colliderType, x, y, z);

    //                        if (CheckAdjacent(x, y, z, Direction.down, transparency) == true)
    //                            CreateFace(voxel, Facing.down, colliderType, x, y, z);

    //                        if (CheckAdjacent(x, y, z, Direction.right, transparency) == true)
    //                            CreateFace(voxel, Facing.right, colliderType, x, y, z);

    //                        if (CheckAdjacent(x, y, z, Direction.left, transparency) == true)
    //                            CreateFace(voxel, Facing.left, colliderType, x, y, z);

    //                        if (colliderType == ColliderType.none && MapEngine.GenerateColliders)
    //                        {
    //                            AddCubeMesh(x, y, z, false);
    //                        }
    //                    }
    //                    // 非 1x1x1 方块
    //                    else
    //                    {
    //                        // if any adjacent voxel isn't opaque, we render the mesh
    //                        if (CheckAllAdjacent(x, y, z) == false)
    //                        {
    //                            CreateCustomMesh(voxel, x, y, z, voxelType.VMesh);
    //                        }
    //                    }
    //                }
    //                z += 1;
    //            }
    //            z = 0;
    //            y += 1;

    //        }
    //        y = 0;
    //        x += 1;
    //    }

    //    UpdateMesh(GetComponent<MeshFilter>().mesh);
    //}

    //private bool CheckAdjacent(int x, int y, int z, Direction direction, Transparency transparency)
    //{ // returns true if a face should be spawned

    //    VoxelPos index = chunk.GetAdjacentIndex(x, y, z, direction);
    //    ushort adjacentVoxel = chunk.GetVoxel(index.x, index.y, index.z);

    //    if (adjacentVoxel == ushort.MaxValue)
    //    { // if the neighbor chunk is missing

    //        if (MapEngine.ShowBorderFaces || direction == Direction.up)
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }

    //    }

    //    Transparency result = MapEngine.GetVoxelType(adjacentVoxel).VTransparency; // get the transparency of the adjacent voxel

    //    // parse the result (taking into account the transparency of the adjacent block as well as the one doing this check)
    //    if (transparency == Transparency.transparent)
    //    {
    //        if (result == Transparency.transparent)
    //            return false; // don't draw a transparent block next to another transparent block
    //        else
    //            return true; // draw a transparent block next to a solid or semi-transparent
    //    }
    //    else
    //    {
    //        if (result == Transparency.solid)
    //            return false; // don't draw a solid block or a semi-transparent block next to a solid block
    //        else
    //            return true; // draw a solid block or a semi-transparent block next to both transparent and semi-transparent
    //    }
    //}

    //public bool CheckAllAdjacent(int x, int y, int z)
    //{ // returns true if all adjacent voxels are solid

    //    for (int direction = 0; direction < 6; direction++)
    //    {
    //        if (MapEngine.GetVoxelType(chunk.GetVoxel(chunk.GetAdjacentIndex(x, y, z, (Direction)direction))).VTransparency != Transparency.solid)
    //        {
    //            return false;
    //        }
    //    }
    //    return true;
    //}


    // ==== mesh generation =======================================================================================

    private void CreateFace(ushort voxel, Facing facing, ColliderType colliderType, int x, int y, int z)
    {
        Voxel voxelComponent = MapEngine.GetVoxelType(voxel);
        List<int> FacesList = Faces[voxelComponent.VSubmeshIndex];

        // ==== Vertices ====

        // add the positions of the vertices depending on the facing of the face
        if (facing == Facing.forward)
        {
            Vertices.Add(new Vector3(x + 0.5001f, y + 0.5001f, z + 0.5f));
            Vertices.Add(new Vector3(x - 0.5001f, y + 0.5001f, z + 0.5f));
            Vertices.Add(new Vector3(x - 0.5001f, y - 0.5001f, z + 0.5f));
            Vertices.Add(new Vector3(x + 0.5001f, y - 0.5001f, z + 0.5f));
            if (colliderType == ColliderType.cube && MapEngine.GenerateColliders)
            {
                SolidColliderVertices.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                SolidColliderVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                SolidColliderVertices.Add(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
                SolidColliderVertices.Add(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
            }
        }
        else if (facing == Facing.up)
        {
            Vertices.Add(new Vector3(x - 0.5001f, y + 0.5f, z + 0.5001f));
            Vertices.Add(new Vector3(x + 0.5001f, y + 0.5f, z + 0.5001f));
            Vertices.Add(new Vector3(x + 0.5001f, y + 0.5f, z - 0.5001f));
            Vertices.Add(new Vector3(x - 0.5001f, y + 0.5f, z - 0.5001f));
            if (colliderType == ColliderType.cube && MapEngine.GenerateColliders)
            {
                SolidColliderVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                SolidColliderVertices.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                SolidColliderVertices.Add(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                SolidColliderVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
            }
        }
        else if (facing == Facing.right)
        {
            Vertices.Add(new Vector3(x + 0.5f, y + 0.5001f, z - 0.5001f));
            Vertices.Add(new Vector3(x + 0.5f, y + 0.5001f, z + 0.5001f));
            Vertices.Add(new Vector3(x + 0.5f, y - 0.5001f, z + 0.5001f));
            Vertices.Add(new Vector3(x + 0.5f, y - 0.5001f, z - 0.5001f));
            if (colliderType == ColliderType.cube && MapEngine.GenerateColliders)
            {
                SolidColliderVertices.Add(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                SolidColliderVertices.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                SolidColliderVertices.Add(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                SolidColliderVertices.Add(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
            }
        }
        else if (facing == Facing.back)
        {
            Vertices.Add(new Vector3(x - 0.5001f, y + 0.5001f, z - 0.5f));
            Vertices.Add(new Vector3(x + 0.5001f, y + 0.5001f, z - 0.5f));
            Vertices.Add(new Vector3(x + 0.5001f, y - 0.5001f, z - 0.5f));
            Vertices.Add(new Vector3(x - 0.5001f, y - 0.5001f, z - 0.5f));
            if (colliderType == ColliderType.cube && MapEngine.GenerateColliders)
            {
                SolidColliderVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                SolidColliderVertices.Add(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                SolidColliderVertices.Add(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                SolidColliderVertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
            }
        }
        else if (facing == Facing.down)
        {
            Vertices.Add(new Vector3(x - 0.5001f, y - 0.5f, z - 0.5001f));
            Vertices.Add(new Vector3(x + 0.5001f, y - 0.5f, z - 0.5001f));
            Vertices.Add(new Vector3(x + 0.5001f, y - 0.5f, z + 0.5001f));
            Vertices.Add(new Vector3(x - 0.5001f, y - 0.5f, z + 0.5001f));
            if (colliderType == ColliderType.cube && MapEngine.GenerateColliders)
            {
                SolidColliderVertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                SolidColliderVertices.Add(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                SolidColliderVertices.Add(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                SolidColliderVertices.Add(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
            }
        }
        else if (facing == Facing.left)
        {
            Vertices.Add(new Vector3(x - 0.5f, y + 0.5001f, z + 0.5001f));
            Vertices.Add(new Vector3(x - 0.5f, y + 0.5001f, z - 0.5001f));
            Vertices.Add(new Vector3(x - 0.5f, y - 0.5001f, z - 0.5001f));
            Vertices.Add(new Vector3(x - 0.5f, y - 0.5001f, z + 0.5001f));
            if (colliderType == ColliderType.cube && MapEngine.GenerateColliders)
            {
                SolidColliderVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                SolidColliderVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                SolidColliderVertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                SolidColliderVertices.Add(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
            }
        }

        // ==== Faces ====

        // add the faces
        FacesList.Add(FaceCount + 0);
        FacesList.Add(FaceCount + 1);
        FacesList.Add(FaceCount + 3);
        FacesList.Add(FaceCount + 1);
        FacesList.Add(FaceCount + 2);
        FacesList.Add(FaceCount + 3);
        if (colliderType == ColliderType.cube && MapEngine.GenerateColliders)
        {
            SolidColliderFaces.Add(SolidFaceCount + 0);
            SolidColliderFaces.Add(SolidFaceCount + 1);
            SolidColliderFaces.Add(SolidFaceCount + 3);
            SolidColliderFaces.Add(SolidFaceCount + 1);
            SolidColliderFaces.Add(SolidFaceCount + 2);
            SolidColliderFaces.Add(SolidFaceCount + 3);
        }

        // Add to the face count
        FaceCount += 4; // we're adding 4 because there are 4 vertices in each face.
        if (colliderType == ColliderType.cube && MapEngine.GenerateColliders)
        {
            SolidFaceCount += 4;
        }

        // Check the amount of vertices so far and create a new mesh if necessary
        if (Vertices.Count > 65530)
        {
            CreateNewMeshObject();
        }
    }

    private void CreateCustomMesh(ushort voxel, int x, int y, int z, Mesh mesh)
    {

        Voxel voxelComponent = MapEngine.GetVoxelType(voxel);
        List<int> FacesList = Faces[voxelComponent.VSubmeshIndex];

        // check if mesh exists
        if (mesh == null)
        {
            Debug.LogError("Uniblocks: The voxel id " + voxel + " uses a custom mesh, but no mesh has been assigned!");
            return;
        }


        // === mesh
        // check if we still have room for more vertices in the mesh
        if (Vertices.Count + mesh.vertices.Length > 65534)
        {
            CreateNewMeshObject();
        }

        // rotate vertices depending on the mesh rotation setting
        List<Vector3> rotatedVertices = new List<Vector3>();
        MeshRotation rotation = voxelComponent.VRotation;

        // 180 horizontal (reverse all x and z)
        if (rotation == MeshRotation.back)
        {
            foreach (Vector3 vertex in mesh.vertices)
            {
                rotatedVertices.Add(new Vector3(-vertex.x, vertex.y, -vertex.z));
            }
        }

        // 90 right
        else if (rotation == MeshRotation.right)
        {
            foreach (Vector3 vertex in mesh.vertices)
            {
                rotatedVertices.Add(new Vector3(vertex.z, vertex.y, -vertex.x));
            }
        }

        // 90 left
        else if (rotation == MeshRotation.left)
        {
            foreach (Vector3 vertex in mesh.vertices)
            {
                rotatedVertices.Add(new Vector3(-vertex.z, vertex.y, vertex.x));
            }
        }

        // no rotation
        else
        {
            foreach (Vector3 vertex in mesh.vertices)
            {
                rotatedVertices.Add(vertex);
            }
        }

        // vertices
        foreach (Vector3 vertex in rotatedVertices)
        {
            Vertices.Add(vertex + new Vector3(x, y, z)); // add all vertices from the mesh
        }

        // faces
        foreach (int face in mesh.triangles)
        {
            FacesList.Add(FaceCount + face);
        }

        // Add to the face count
        FaceCount += mesh.vertexCount;


        // === collider
        if (MapEngine.GenerateColliders)
        {
            ColliderType colliderType = MapEngine.GetVoxelType(voxel).VColliderType;

            // mesh collider
            if (colliderType == ColliderType.mesh)
            {
                foreach (Vector3 vertex1 in rotatedVertices)
                {
                    // if mesh collider, just add the vertices & faces from this mesh to the solid collider mesh
                    SolidColliderVertices.Add(vertex1 + new Vector3(x, y, z));
                }
                foreach (int face1 in mesh.triangles)
                {
                    SolidColliderFaces.Add(SolidFaceCount + face1);
                }
                SolidFaceCount += mesh.vertexCount;
            }

            if (colliderType == ColliderType.cube)
            {
                // 产生一个参与物理碰撞的 collider
                AddCubeMesh(x, y, z, true);
            }
            // 对于 non-empty voxels, 不管其 collider 类型( ColliderType.mesh 或 ColliderType.none ), 都要再产生一个 trigger cube collider
            // 使其能够正常响应 raycasts, 从而玩家可以对其进行删除或修改
            // 但 ColliderType.cube 本身已经生成了一个 cube collider, 所以无需重复生成( mesh collider 因其形状不规则, 可能在 raycasts 时出现意外 )
            else if (voxel != 0)
            {
                AddCubeMesh(x, y, z, false);
            }
        }
    }

    // 虽说是产生 cube 类型的 collider,
    // 但本质还是加到 mesh collider 中, 并不是构建 primitive collider
    private void AddCubeMesh(int x, int y, int z, bool solid)
    {
        if (solid)
        {
            // vertices
            foreach (Vector3 vertex in Cube.vertices)
            {
                SolidColliderVertices.Add(vertex + new Vector3(x, y, z)); // add all vertices from the mesh
            }

            // faces
            foreach (int face in Cube.triangles)
            {
                SolidColliderFaces.Add(SolidFaceCount + face);
            }

            // Add to the face count 
            SolidFaceCount += Cube.vertexCount;
        }
        else
        {
            // vertices
            foreach (Vector3 vertex1 in Cube.vertices)
            {
                NoCollideVertices.Add(vertex1 + new Vector3(x, y, z));
            }

            // faces
            foreach (int face1 in Cube.triangles)
            {
                NoCollideFaces.Add(NoCollideFaceCount + face1);
            }

            // Add to the face count 
            NoCollideFaceCount += Cube.vertexCount;
        }
    }

    /// <summary>
    /// 将构建好的 Mesh 赋给指定 chunk
    /// </summary>
    /// <param name="mesh"></param>
    private void UpdateMesh(Mesh mesh)
    {
        // Update the mesh
        mesh.Clear();
        mesh.vertices = Vertices.ToArray();
        mesh.subMeshCount = GetComponent<Renderer>().materials.Length;

        for (int i = 0; i < Faces.Count; ++i)
        {
            mesh.SetTriangles(Faces[i].ToArray(), i);
        }

        //UVs.ToBuiltin(Vector2) as Vector2[];
        mesh.RecalculateNormals();

        if (MapEngine.GenerateColliders)
        {
            // Mesh Collider 以一个 Mesh Asset 为参数, 基于其 Mesh 数据直接构建 collider.
            Mesh colMesh = new Mesh();
            colMesh.vertices = SolidColliderVertices.ToArray();
            colMesh.triangles = SolidColliderFaces.ToArray();
            colMesh.RecalculateNormals();

            GetComponent<MeshCollider>().sharedMesh = null;
            GetComponent<MeshCollider>().sharedMesh = colMesh;

            // Update nocollide collider
            if (NoCollideVertices.Count > 0)
            {
                // make mesh
                Mesh nocolMesh = new Mesh();
                nocolMesh.vertices = NoCollideVertices.ToArray();
                nocolMesh.triangles = NoCollideFaces.ToArray();
                nocolMesh.RecalculateNormals();

                noCollideCollider = Instantiate(chunk.ChunkCollider, transform.position, transform.rotation) as GameObject;
                noCollideCollider.transform.parent = this.transform;
                noCollideCollider.GetComponent<MeshCollider>().sharedMesh = nocolMesh;
            }
            else if (noCollideCollider != null)
            {
                Destroy(noCollideCollider); // destroy the existing collider if there is no NoCollide vertices
            }
        }


        // clear the main arrays for future use.
        Vertices.Clear();
        foreach (List<int> faceList in Faces)
        {
            faceList.Clear();
        }

        SolidColliderVertices.Clear();
        SolidColliderFaces.Clear();

        NoCollideVertices.Clear();
        NoCollideFaces.Clear();


        FaceCount = 0;
        SolidFaceCount = 0;
        NoCollideFaceCount = 0;
    }



    private void CreateNewMeshObject()
    { // in case the amount of vertices exceeds the maximum for one mesh, we need to create a new mesh

        GameObject meshContainer = Instantiate(chunk.MeshContainer, transform.position, transform.rotation) as GameObject;
        meshContainer.transform.parent = this.transform;

        UpdateMesh(meshContainer.GetComponent<MeshFilter>().mesh);
    }


}