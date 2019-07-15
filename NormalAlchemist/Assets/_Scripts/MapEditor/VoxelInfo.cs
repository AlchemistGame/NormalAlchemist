public class VoxelInfo
{
    public VoxelPos index;
    public VoxelPos adjacentIndex;

    public VoxelInfo(int setX, int setY, int setZ)
    {
        this.index.x = setX;
        this.index.y = setY;
        this.index.z = setZ;
    }

    public VoxelInfo(int setX, int setY, int setZ, int setXa, int setYa, int setZa)
    {
        this.index.x = setX;
        this.index.y = setY;
        this.index.z = setZ;

        this.adjacentIndex.x = setXa;
        this.adjacentIndex.y = setYa;
        this.adjacentIndex.z = setZa;
    }

    public VoxelInfo(VoxelPos setIndex)
    {
        this.index = setIndex;
    }

    public VoxelInfo(VoxelPos setIndex, VoxelPos setAdjacentIndex)
    {
        this.index = setIndex;
        this.adjacentIndex = setAdjacentIndex;
    }

    public string GetVoxel()
    {
        return MapManager.Instance.GetVoxel(index);
    }

    public string GetAdjacentVoxel()
    {
        return MapManager.Instance.GetVoxel(adjacentIndex);
    }

    public void SetVoxel(string data)
    {
        MapManager.Instance.SetVoxel(index, data);
    }

}