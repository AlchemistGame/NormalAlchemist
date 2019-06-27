using UnityEngine;

// 体素地图里的坐标用 int 来代替 float
public class VoxelPos
{
    public int x, y, z;

    public VoxelPos(int setX, int setY, int setZ)
    {
        this.x = setX;
        this.y = setY;
        this.z = setZ;
    }
    public VoxelPos(float setX, float setY, float setZ)
    {
        this.x = (int)setX;
        this.y = (int)setY;
        this.z = (int)setZ;
    }
    public VoxelPos(Vector3 setIndex)
    {
        this.x = (int)setIndex.x;
        this.y = (int)setIndex.y;
        this.z = (int)setIndex.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public override string ToString()
    {
        return (x.ToString() + "," + y.ToString() + "," + z.ToString());
    }

    public bool IsEqual(VoxelPos to)
    {
        if (to == null)
        {
            return false;
        }
        else if (this.x == to.x &&
            this.y == to.y &&
            this.z == to.z)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public VoxelPos GetAdjacentIndex(Direction direction)
    {
        if (direction == Direction.down) return new VoxelPos(x, y - 1, z);
        else if (direction == Direction.up) return new VoxelPos(x, y + 1, z);
        else if (direction == Direction.left) return new VoxelPos(x - 1, y, z);
        else if (direction == Direction.right) return new VoxelPos(x + 1, y, z);
        else if (direction == Direction.back) return new VoxelPos(x, y, z - 1);
        else if (direction == Direction.forward) return new VoxelPos(x, y, z + 1);
        else return null;
    }

    public static bool Compare(VoxelPos a, VoxelPos b)
    {
        if (a == null || b == null)
        {
            return false;
        }
        else if (a.x == b.x && a.y == b.y && a.z == b.z)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static VoxelPos FromString(string posString)
    {
        string[] splitString = posString.Split(',');

        try
        {
            return new VoxelPos(int.Parse(splitString[0]), int.Parse(splitString[1]), int.Parse(splitString[2]));
        }
        catch (System.Exception)
        {
            Debug.LogError("VoxelPos.FromString: Invalid format. String must be in \"x,y,z\" format.");
            return null;
        }
    }
}
