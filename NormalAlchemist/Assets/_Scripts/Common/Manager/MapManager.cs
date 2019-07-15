using UnityEngine;

public class MapManager : MonoBehaviour
{
    private MapManager() { }
    public static MapManager Instance { get; private set; }

    // 地图大小
    public int SideLength;
    private int SquaredSideLength;
    // tile id
    public string[] VoxelData;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        Init();
    }

    private void Init()
    {
        SideLength = GlobalSettings.MapSideLength;
        SquaredSideLength = SideLength * SideLength;
        VoxelData = new string[SideLength * SideLength * SideLength];
    }

    public Vector3 VoxelIndexToPosition(int x, int y, int z)
    {
        Vector3 localPoint = new Vector3(x, y, z);
        return transform.TransformPoint(localPoint);
    }

    public string GetVoxel(VoxelPos index)
    {
        return GetVoxel(index.x, index.y, index.z);
    }

    public string GetVoxel(int x, int y, int z)
    {
        return VoxelData[(z * SquaredSideLength) + (y * SideLength) + x];
    }

    public void SetVoxel(VoxelPos index, string data)
    {
        SetVoxel(index.x, index.y, index.z, data);
    }

    public void SetVoxel(int x, int y, int z, string data)
    {
        // 三维转一维
        VoxelData[(z * SquaredSideLength) + (y * SideLength) + x] = data;

        // 更新场景
    }

    public void PlaceBlock(VoxelInfo voxelPos, TagMapObject templateObject)
    {
        MapManager.Instance.SetVoxel(voxelPos.index, templateObject.objGUID);

        GameObject newObj = Instantiate(templateObject.gameObject, voxelPos.index.ToVector3(), templateObject.transform.rotation);
        newObj.transform.localEulerAngles = templateObject.transform.localEulerAngles;
        newObj.layer = 0;
        Destroy(newObj.GetComponent<Rigidbody>());
        newObj.GetComponent<Collider>().isTrigger = false;
        RoundTo90(newObj);
        GameObject mapObject = GameObject.Find("MAP");
        newObj.transform.parent = mapObject.transform;
        newObj.name = templateObject.name;
    }

    public void DestroyBlock(TagMapObject eraseObject)
    {
        VoxelPos erasePos = new VoxelPos(eraseObject.transform.position);
        MapManager.Instance.SetVoxel(erasePos, "");
        Destroy(eraseObject.gameObject);
    }

    private void RoundTo90(GameObject go)
    {
        Vector3 vec = go.transform.eulerAngles;
        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;
        go.transform.eulerAngles = vec;
    }
}