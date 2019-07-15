using Crosstales.FB;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MapMenu : MonoBehaviour
{
    #region 地图管理
    public Button btnNewMap;
    public GameObject panelMapCreate;
    public Button btnMapCreate;
    public InputField inputNewMapName;
    public Button btnOpenMap;
    public Button btnSaveMap;
    #endregion

    #region 帮助页面
    public Button btnHelp;
    public GameObject panelHelp;
    public Text txtHelp;
    #endregion

    #region 编辑界面
    public GameObject panelCategoryTiles;
    #endregion

    public MapEditorEngine mapEditorEngine;

    private void Awake()
    {
        EventManager.Register(EventsEnum.EnterLobbyMode, this, "EnterLobbyMode");
        EventManager.Register(EventsEnum.EnterEditMode, this, "EnterEditMode");
    }

    private void Start()
    {
        EventManager.Broadcast(EventsEnum.EnterLobbyMode);

        btnNewMap.onClick.AddListener(() =>
        {
            panelMapCreate.SetActive(true);
        });
        btnMapCreate.onClick.AddListener(() =>
        {
            string newMapName = inputNewMapName.text;
            if (CreateNewMap(newMapName))
            {
                // 是新建地图还是打开已有地图
                mapEditorEngine.isItLoad = false;
                // 地图名
                mapEditorEngine.newProjectName = newMapName;
                StartCoroutine(mapEditorEngine.InitMapEditorEngine());

                panelMapCreate.SetActive(false);
            }
        });
        btnOpenMap.onClick.AddListener(() =>
        {
            // 地图名
            string mapPath = FileBrowser.OpenSingleFile("打开地图", GlobalSettings.MapLevelsDir, "map");
            string mapName = Path.GetFileNameWithoutExtension(mapPath);
            if (mapName.Length > 0)
            {
                mapEditorEngine.newProjectName = mapName;
                // 是新建地图还是打开已有地图
                mapEditorEngine.isItLoad = true;
                StartCoroutine(mapEditorEngine.InitMapEditorEngine());
            }
        });

        btnSaveMap.onClick.AddListener(() =>
        {
            EventManager.Broadcast(EventsEnum.SaveMap);
        });

        btnHelp.onClick.AddListener(() =>
        {
            panelHelp.SetActive(!panelHelp.activeInHierarchy);
        });
        txtHelp.text =
            "先选中左侧资源栏里的一个方块, 然后在右侧场景里按住鼠标左键拖拉来快速铺开大量方块, 同时按 1 来降低一层高度, 按 2 增加一层高度, 按 3 来快速恢复高度为 1\n\n" +
            "鼠标右键点击 - 90度旋转物体\n\n" +
            "按键 G - 显示/隐藏网格 (Grid)\n\n" +
            "按键 0 - 打开/关闭光照\n\n" +
            "按住 ALT + 鼠标左键 - orbit 旋转摄像机\n\n" +
            "按住 ALT + 鼠标右键 - 拖拽摄像机\n\n" +
            "按住鼠标中键 - 拖拽摄像机\n\n" +
            "Keys W, S, D, A - 前后左右移动摄像机\n\n" +
            "Keys Q, E - 向左/右旋转摄像机\n\n" +
            "鼠标滚轮 - 放大缩小摄像机镜头\n\n" +
            "Key X, C - 使基准网格上升/下降\n\n" +
            "Key R - 重置摄像机的旋转方向\n\n" +
            "按 T 进入删除模式 - 按住鼠标左键拖动快速删除大量方块, 鼠标右键一次删一个\n\n" +
            "按 F 进入拣选模式 - 在拣选模式下, 鼠标左键点击方块, 将在 tile 窗口中快速定位此类型方块\n\n" +
            "按 Esc 恢复到放置模式\n\n" +
            "Ctrl/Cmd + Z" + " - 撤销上一个指令\n\n" +
            "Ctrl/Cmd + S - 保存地图";
    }

    private void SaveAsMap(string old_name, string new_name)
    {
        new_name = FilterName(new_name);

        if (!CheckIfMapExists(new_name) && !new_name.Equals(""))
        {
            string mapPath;
            string mapPathMetaPath = "";

            string mapPath_2 = GlobalSettings.MapLevelsDir + new_name + ".map";
            string mapPathMetaPath_2 = GlobalSettings.MapLevelsDir + new_name + ".map.meta";

            mapPath = GlobalSettings.MapLevelsDir + old_name + ".map";
            mapPathMetaPath = GlobalSettings.MapLevelsDir + old_name + ".map.meta";

            if (File.Exists(mapPath))
            {
                File.Copy(mapPath, mapPath_2);
            }

            if (File.Exists(mapPathMetaPath))
            {
                File.Copy(mapPathMetaPath, mapPathMetaPath_2);
            }

            ReadAllMaps();
        }
    }

    private void RenameMap(string old_name, string new_name)
    {
        new_name = FilterName(new_name);

        if (!CheckIfMapExists(new_name) && !new_name.Equals(""))
        {
            string mapPath;

            mapPath = GlobalSettings.MapLevelsDir + old_name + ".map";

            string new_map_info = "";

            bool mapPath_exists = false;

            if (File.Exists(mapPath))
            {
                mapPath_exists = true;
                StreamReader sr2 = new StreamReader(mapPath);
                new_map_info = sr2.ReadToEnd();
                sr2.Close();
            }

            if (mapPath_exists)
            {
                StreamWriter sw2 = new StreamWriter(GlobalSettings.MapLevelsDir + new_name + ".map");
                sw2.Write(new_map_info);
                sw2.Flush();
                sw2.Close();
            }

            DeleteMap(old_name);
        }
        else
        {
            Debug.Log("not ok");
        }
    }

    private string FilterName(string name)
    {
        if (name.Contains(" ") || name.Contains("/") || name.Contains("\"") || name.Contains(":") || name.Contains("$") || name.Contains("."))
        {
            Debug.Log("Warning: empty spaces or symbols: \" : / $ : . are forbiden. They will be stripped.");
        }

        name = name.Replace(" ", "");
        name = name.Replace("/", "");
        name = name.Replace("\"", "");
        name = name.Replace(":", "");
        name = name.Replace("$", "");
        name = name.Replace(".", "");

        return name;
    }

    private bool CreateNewMap(string name)
    {
        name = FilterName(name);

        if (!CheckIfMapExists(name) && !name.Equals(""))
        {
            ReadAllMaps();

            return true;
        }
        else
        {
            Debug.Log("Error: Map with this name already exists or map name is empty.");

            return false;
        }
    }

    private bool CheckIfMapExists(string name)
    {
        List<string> myMaps = ReadAllMaps();

        for (int i = 0; i < myMaps.Count; i++)
        {
            if (myMaps[i].ToString().Equals(name))
            {
                return true;
            }
        }

        return false;
    }

    private List<string> ReadAllMaps()
    {
        DirectoryInfo dirInfo = new DirectoryInfo(GlobalSettings.MapLevelsDir);
        FileInfo[] filesInfo = dirInfo.GetFiles("*.map");
        List<string> myMaps = new List<string>();
        for (int i = 0; i < filesInfo.Length; i++)
        {
            string fileName = Path.GetFileNameWithoutExtension(filesInfo[i].Name);
            myMaps.Add(fileName);
        }
        return myMaps;
    }

    private void DeleteMap(string name)
    {
        string mapPath = GlobalSettings.MapLevelsDir + name + ".map";
        if (File.Exists(mapPath))
        {
            File.Delete(mapPath);
        }
    }

    public void EnterLobbyMode()
    {
        panelCategoryTiles.SetActive(false);
        btnNewMap.gameObject.SetActive(true);
        btnOpenMap.gameObject.SetActive(true);
        btnSaveMap.gameObject.SetActive(false);
        btnHelp.gameObject.SetActive(true);
    }

    public void EnterEditMode()
    {
        btnNewMap.gameObject.SetActive(false);
        btnOpenMap.gameObject.SetActive(false);
        btnSaveMap.gameObject.SetActive(true);
        btnHelp.gameObject.SetActive(true);
        panelCategoryTiles.SetActive(true);
    }
}
