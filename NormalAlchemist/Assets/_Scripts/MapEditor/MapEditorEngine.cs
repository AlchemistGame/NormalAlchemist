using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapEditorEngine : MonoBehaviour
{
    #region UI界面
    public MapTile prefabTile;
    public Transform transTileContainer;
    public Dropdown dropdownTileCategories;
    public InputField inputFieldSearchBox;
    private int selectedCategoryIndex;
    private List<MapTile> listMapTiles = new List<MapTile>();
    public Text txtMapDescription;
    public Text txtStatus;
    #endregion

    // states
    [HideInInspector]
    public bool notReady = true;

    enum MapEditorMode
    {
        Place,
        Erase,
        Find
    }
    private MapEditorMode currentMode;

    private int isRotated;
    private int isRotatedH;
    private bool is2D;
    private bool isOrtho;
    private bool canBuild;
    private bool isCamGridMoving;
    private bool isMouseDown;
    private bool isInTopView;
    [HideInInspector]
    public string newProjectName;
    [HideInInspector]
    public bool isItLoad;
    [HideInInspector]
    public bool isMapLoaded;
    [HideInInspector]
    public GameObject mapLightGO;

    // info
    private float globalYSize;
    private string cameraType;
    private int globalGridSizeX;
    private int globalGridSizeZ;
    private Vector3 gridsize;
    private static readonly float ZERO_F = .0f;
    private float rotationOffset;
    private string currentObjectID;
    private MassBuildEngine uMBE;
    private UndoSystem UndoSystem;
    private int currentLayer;
    private string currentObjectGUID;
    private Vector3 startingCameraTransform;
    private List<int> rotationList = new List<int>();
    private bool passSaveA;
    private bool passSaveB;

    // load camera info
    private Vector3 loadCameraPosition;
    private Vector3 loadCameraRotation;

    // map objects
    private GameObject MAP;
    private GameObject MAIN;
    private GameObject cameraGO;
    private List<CatInfo> allTiles = new List<CatInfo>();
    private List<GameObject> currentCategoryGoes = new List<GameObject>();
    private List<Texture2D> currentCategoryPrevs = new List<Texture2D>();
    private List<string> currentCategoryNames = new List<string>();
    private List<string> currentCategoryGuids = new List<string>();
    private GameObject currentTile;
    private GameObject lastTile;
    [HideInInspector]
    public GameObject grid;
    private GameObject hitObject;
    private float cameraSensitivity;

    // UI objects
    private List<GameObject> undo_objs = new List<GameObject>();
    private List<Vector3> undo_poss = new List<Vector3>();
    private List<Vector3> undo_rots = new List<Vector3>();
    private List<string> undo_guids = new List<string>();

    // helpers
    private GameObject helpers_CANTBUILD;
    private GameObject helpers_CANBUILD;

    // other
    private CameraMoveController cameraMove;
    private bool isCastShadows;

    public class CatInfo : System.IDisposable
    {
        public string catName;
        public List<GameObject> catObjs = new List<GameObject>();
        public List<Texture2D> catObjsPrevs = new List<Texture2D>();
        public List<string> catObjsNames = new List<string>();
        public List<string> catIDNames = new List<string>();
        public List<string> catLoadPaths = new List<string>();

        public CatInfo(string _catName)
        {
            // for clone
            catName = _catName;
        }

        public CatInfo(string _catName, List<string> _catObjsNames, List<GameObject> _catObjs, List<Texture2D> _catObjsPrevs, List<string> _catIDNames, List<string> _catLoadPaths)
        {
            catName = _catName;
            catObjs = _catObjs;
            catObjsPrevs = _catObjsPrevs;
            catObjsNames = _catObjsNames;
            catIDNames = _catIDNames;
            catLoadPaths = _catLoadPaths;
        }

        public void Dispose()
        {
        }
    }

    private void Awake()
    {
        EventManager.Register(EventsEnum.SaveMap, this, "SaveMapFoo");
    }

    private void Start()
    {
        dropdownTileCategories.onValueChanged.AddListener(categoryIndex =>
        {
            selectedCategoryIndex = categoryIndex;

            RefreshCategoryTiles();
        });

        inputFieldSearchBox.onValueChanged.AddListener(searchContent =>
        {
            RefreshCategoryTiles();
        });
    }

    public IEnumerator InitMapEditorEngine()
    {
        EventManager.Broadcast(EventsEnum.EnterEditMode);

        rotationList.Add(0);
        rotationList.Add(90);
        rotationList.Add(180);
        rotationList.Add(270);

        currentMode = MapEditorMode.Place;
        passSaveA = false;
        passSaveB = false;
        isInTopView = false;
        lastTile = null;
        currentLayer = 0;
        isMouseDown = false;
        isCamGridMoving = false;
        notReady = true;
        canBuild = false;
        cameraGO = GameObject.Find("MapEditorCamera");
        gridsize = new Vector3(1000.0f, 0.1f, 1000.0f);
        currentObjectGUID = "";

        MAP = new GameObject("MAP");
        mapLightGO = Instantiate(Resources.Load<GameObject>("uteForEditor/uteMapLight"));
        mapLightGO.name = "MapLight";
        mapLightGO.SetActive(true);
        GameObject tempGO = new GameObject();
        tempGO.name = "TEMP";

        SetCastShadows(false);

        GameObject help = Instantiate((GameObject)Resources.Load("uteForEditor/uteHELPERS"), Vector3.zero, Quaternion.identity);
        help.name = "uteHELPERS";
        helpers_CANTBUILD = GameObject.Find("uteHELPERS/CANTBUILD");
        helpers_CANBUILD = GameObject.Find("uteHELPERS/CANBUILD");
        uMBE = this.gameObject.AddComponent<MassBuildEngine>();

        yield return StartCoroutine(ReloadTileAssets());

        UndoSystem = this.gameObject.AddComponent<UndoSystem>();

        if (isItLoad)
        {
            yield return StartCoroutine(LoadMap(newProjectName, MAP));
            yield return StartCoroutine(LoadCameraInfo());
            notReady = false;
        }
        else
        {
            notReady = false;
        }

        if (!isItLoad)
        {
            yield return StartCoroutine(SaveMap(newProjectName));
        }

        yield return 0;
    }

    public IEnumerator LoadMap(string name, GameObject mapObject)
    {
        isMapLoaded = false;
        string path = GlobalSettings.MapLevelsDir + name + ".map";
        StreamReader sr = new StreamReader(path);
        string info = sr.ReadToEnd();
        sr.Close();
        MapData myData = JsonUtility.FromJson<MapData>(info);
        string[] allparts = myData.blocks.Split("$"[0]);

        for (int i = 0; i < allparts.Length; i++)
        {
            if (!allparts[i].Equals(""))
            {
                if (i % 2000 == 0) yield return 0;

                string[] allinfo = allparts[i].Split(":"[0]);

                string id = allinfo[0].ToString();
                float pX = System.Convert.ToSingle(allinfo[1].ToString());
                float pY = System.Convert.ToSingle(allinfo[2].ToString());
                float pZ = System.Convert.ToSingle(allinfo[3].ToString());
                int rX = System.Convert.ToInt32(allinfo[4].ToString());
                int rY = System.Convert.ToInt32(allinfo[5].ToString());
                int rZ = System.Convert.ToInt32(allinfo[6].ToString());
                string familyName = allinfo[9].ToString();

                GameObject tGO = Resources.Load<GameObject>(GetTileAssetPathByID(id));
                if (tGO)
                {
                    GameObject newGO = Instantiate(tGO, Vector3.zero, new Quaternion(0, 0, 0, 0));
                    newGO.name = tGO.name;
                    GameObject behindGO = CreateColliderToObject(newGO);
                    behindGO.layer = 0;
                    behindGO.transform.position = new Vector3(pX, pY, pZ);
                    behindGO.transform.localEulerAngles = new Vector3(rX, rY, rZ);
                    behindGO.GetComponent<Collider>().isTrigger = false;
                    TagMapObject uTO = behindGO.AddComponent<TagMapObject>();
                    uTO.objGUID = id;
                    behindGO.transform.parent = mapObject.transform;

                    GlobalSettings.mapObjectCount++;
                }
            }
        }

        isMapLoaded = true;

        yield return 0;
    }

    private IEnumerator LoadCameraInfo()
    {
        if (!System.IO.File.Exists(GlobalSettings.MapLevelsDir + newProjectName + ".map"))
        {
            yield return StartCoroutine(SaveMap(newProjectName));
        }

        if (System.IO.File.Exists(GlobalSettings.MapLevelsDir + newProjectName + ".map"))
        {
            StreamReader sr = new StreamReader(GlobalSettings.MapLevelsDir + newProjectName + ".map");
            string info = sr.ReadToEnd();
            sr.Close();
            MapData myData = JsonUtility.FromJson<MapData>(info);

            GameObject _YArea = GameObject.Find("MAIN/YArea");

            if (myData.settings != "")
            {
                string[] allinfo = myData.settings.Split(":"[0]);
                // main pos
                float pX = System.Convert.ToSingle(allinfo[0]);
                float pY = System.Convert.ToSingle(allinfo[1]);
                float pZ = System.Convert.ToSingle(allinfo[2]);
                // main rot
                float _main_rX = System.Convert.ToSingle(allinfo[3]);
                float _main_rY = System.Convert.ToSingle(allinfo[4]);
                float _main_rZ = System.Convert.ToSingle(allinfo[5]);
                // yarea rot
                float _yarea_rX = System.Convert.ToSingle(allinfo[6]);
                float _yarea_rY = System.Convert.ToSingle(allinfo[7]);
                float _yarea_rZ = System.Convert.ToSingle(allinfo[8]);
                // mapeditorcamera rot
                float _mapeditorcamera_rX = System.Convert.ToSingle(allinfo[9]);
                float _mapeditorcamera_rY = System.Convert.ToSingle(allinfo[10]);
                float _mapeditorcamera_rZ = System.Convert.ToSingle(allinfo[11]);

                MAIN.transform.position = new Vector3(pX, pY, pZ);
                MAIN.transform.localEulerAngles = new Vector3(_main_rX, _main_rY, _main_rZ);
                _YArea.transform.localEulerAngles = new Vector3(_yarea_rX, _yarea_rY, _yarea_rZ);
                cameraGO.transform.localEulerAngles = new Vector3(_mapeditorcamera_rX, _mapeditorcamera_rY, _mapeditorcamera_rZ);
            }
        }

        yield return 0;
    }

    private void Update()
    {
        if (notReady || EventSystem.current.IsPointerOverGameObject()/*防止鼠标操作穿透 UI*/)
            return;

        if (isItLoad)
        {
            if (!isMapLoaded)
            {
                return;
            }
        }

        if (isCamGridMoving)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            currentMode = MapEditorMode.Find;
        }
        if (currentMode == MapEditorMode.Find)
        {
            if (currentTile)
            {
                Destroy(currentTile);
            }

            Ray findRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit findHit;
            GameObject currentFindObject;
            if (Physics.Raycast(findRay, out findHit, 1000))
            {
                if (findHit.collider.gameObject.GetComponent<TagMapObject>())
                {
                    currentFindObject = findHit.collider.gameObject;
                }
                else
                {
                    currentFindObject = null;
                }
            }
            else
            {
                currentFindObject = null;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (currentFindObject)
                {
                    FindAndSelectTileInAllCats(currentFindObject);
                }
                else
                {
                    inputFieldSearchBox.text = "";
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inputFieldSearchBox.text = "";

            if (currentTile)
            {
                Destroy(currentTile);
                helpers_CANTBUILD.transform.position = new Vector3(-1000000.0f, 0.0f, -1000000.0f);
                helpers_CANBUILD.transform.position = new Vector3(-1000000.0f, 0.0f, -1000000.0f);
            }

            currentMode = MapEditorMode.Place;
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            grid.SetActive(!grid.activeInHierarchy);
        }
        if (Input.GetKeyUp(KeyCode.Alpha0))
        {
            mapLightGO.SetActive(!mapLightGO.activeInHierarchy);
        }
        if (Input.GetKeyUp(KeyCode.Alpha9))
        {
            SetCastShadows(!isCastShadows);
        }

        if (currentMode == MapEditorMode.Erase)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray eraseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(eraseRay, out RaycastHit eraseHit, 1000))
                {
                    if (!eraseHit.collider.gameObject.name.Equals("Grid"))
                    {
                        MapManager.Instance.DestroyBlock(eraseHit.collider.gameObject.GetComponent<TagMapObject>());
                        GlobalSettings.mapObjectCount--;
                        hitObject = null;
                        helpers_CANTBUILD.transform.position = new Vector3(-1000000.0f, 0.0f, -1000000.0f);
                    }
                }
            }
        }

        if (!isCamGridMoving)
        {
            if (Input.GetKeyUp(KeyCode.C))
            {
                StartCoroutine(gridSmoothMove(grid, true, cameraMove.gameObject));
            }
            else if (Input.GetKeyUp(KeyCode.X))
            {
                StartCoroutine(gridSmoothMove(grid, false, cameraMove.gameObject));
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetCamera();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                if (currentTile)
                {
                    Destroy(currentTile);
                    helpers_CANTBUILD.transform.position = new Vector3(-1000000.0f, 0.0f, -1000000.0f);
                }

                currentMode = MapEditorMode.Erase;
            }
        }

        if (isCamGridMoving)
            return;

        if (Input.GetKeyDown(KeyCode.LeftCommand))
        {
            passSaveA = true;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            passSaveB = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftCommand))
        {
            passSaveA = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            passSaveA = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            passSaveA = false;
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            passSaveB = false;
        }

        if (passSaveA && passSaveB)
        {
            passSaveB = false;

            EventManager.Broadcast(EventsEnum.SaveMap);
        }

        if (Input.GetMouseButtonUp(1) && currentMode == MapEditorMode.Place)
        {
            if (currentTile)
            {
                StartCoroutine(smoothRotate(currentTile, new Vector3(0.0f, -10.0f, 0.0f), true));
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastTile = null;
            isMouseDown = false;
        }

        if (currentMode == MapEditorMode.Erase && hitObject && !hitObject.name.Equals("Grid"))
        {
            helpers_CANTBUILD.transform.position = hitObject.transform.position + new Vector3(hitObject.GetComponent<BoxCollider>().center.x * hitObject.transform.localScale.x, hitObject.GetComponent<BoxCollider>().center.y * hitObject.transform.localScale.y, hitObject.GetComponent<BoxCollider>().center.z * hitObject.transform.localScale.z);
            helpers_CANTBUILD.transform.localScale = hitObject.GetComponent<Collider>().bounds.size + new Vector3(0.1f, 0.1f, 0.1f);
            helpers_CANBUILD.transform.position = new Vector3(-10000, 0, -10000);
        }
        else if (hitObject && !hitObject.name.Equals("Grid"))
        {
            if (hitObject.GetComponent<BoxCollider>())
            {
                helpers_CANBUILD.transform.position = hitObject.transform.position + new Vector3(hitObject.GetComponent<BoxCollider>().center.x * hitObject.transform.localScale.x, hitObject.GetComponent<BoxCollider>().center.y * hitObject.transform.localScale.y, hitObject.GetComponent<BoxCollider>().center.z * hitObject.transform.localScale.z);
                helpers_CANBUILD.transform.localScale = hitObject.GetComponent<Collider>().bounds.size + new Vector3(0.1f, 0.1f, 0.1f);
            }
        }
        else
        {
            helpers_CANBUILD.transform.position = new Vector3(-10000, 0, -10000);
        }

        if (currentTile && currentMode == MapEditorMode.Place)
        {
            canBuild = false;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit buildHit, 1000) && buildHit.collider)
            {
                hitObject = buildHit.collider.gameObject;

                if (!buildHit.collider.gameObject.name.Equals("Grid"))
                {
                    if (isMouseDown)
                    {
                        if (buildHit.collider.gameObject.name.Equals("uteTcDummy(Clone)"))
                        {
                            Destroy(buildHit.collider.gameObject);
                        }
                    }

                    if (buildHit.normal != Vector3.up)
                    {
                        return;
                    }
                }

                float sizeX = currentTile.GetComponent<Collider>().bounds.size.x;
                float sizeY = currentTile.GetComponent<Collider>().bounds.size.y;
                float sizeZ = currentTile.GetComponent<Collider>().bounds.size.z;
                float centerX = sizeX / 2.0f;
                float centerY = sizeY / 2.0f;
                float centerZ = sizeZ / 2.0f;
                float centerPosZ = centerZ + (currentTile.transform.position.z - (sizeZ / 2.0f));
                int castSizeX = (int)currentTile.GetComponent<Collider>().bounds.size.x;
                int castSizeZ = (int)currentTile.GetComponent<Collider>().bounds.size.z;
                int castSizeSide;

                if (castSizeX == castSizeZ)
                {
                    castSizeSide = castSizeX;
                }
                else if (castSizeX > castSizeZ)
                {
                    castSizeSide = castSizeX;
                }
                else
                {
                    castSizeSide = castSizeZ;
                }

                float normalX = ZERO_F;
                float normalZ = ZERO_F;
                float normalY = ZERO_F;


                if (buildHit.normal.y == ZERO_F)
                {
                    if ((int)buildHit.normal.x > ZERO_F)
                    {
                        normalX = 0.5f;
                    }
                    else if ((int)buildHit.normal.x < ZERO_F)
                    {
                        normalX = -0.5f;
                    }

                    if ((int)buildHit.normal.z > ZERO_F)
                    {
                        normalZ = 0.5f;
                    }
                    else if ((int)buildHit.normal.z < ZERO_F)
                    {
                        normalZ = -0.5f;
                    }
                }


                if (buildHit.normal.y > ZERO_F)
                {
                    normalY = 0.5f;
                }
                else if (buildHit.normal.y < ZERO_F)
                {
                    normalY = -0.5f;
                }

                float internalOffsetX = ZERO_F;
                float internalOffsetZ = ZERO_F;
                float internalOffsetY = ZERO_F;

                if (Mathf.Round(currentTile.GetComponent<Collider>().bounds.size.z) % 2 == 0)
                {
                    internalOffsetZ = 0.5f;
                }

                if (Mathf.Round(currentTile.GetComponent<Collider>().bounds.size.x) % 2 == 0)
                {
                    internalOffsetX = 0.5f;
                }

                if (Mathf.Round(currentTile.GetComponent<Collider>().bounds.size.y) % 2 == 0)
                {
                    internalOffsetY = 0.5f;
                }

                float posY = 0.0f;

                if (buildHit.normal == Vector3.up)
                {
                    posY = (buildHit.point.y + (currentTile.GetComponent<Collider>().bounds.size.y / 2.0f)) - (currentTile.GetComponent<BoxCollider>().center.y * currentTile.transform.localScale.y) + 0.000001f;
                }
                else
                {
                    if (currentTile.name.Equals(buildHit.collider.gameObject.name))
                    {
                        posY = buildHit.collider.gameObject.transform.position.y;
                    }
                    else
                    {
                        posY = 0.1f * ((Mathf.Round((buildHit.point.y + normalY) * 10.0f)) + internalOffsetY);
                    }
                }

                VoxelPos finalPos = new VoxelPos(buildHit.point.x, posY, buildHit.point.z);
                currentTile.transform.position = finalPos.ToVector3();

                // TODO: 鼠标所指位置不存在已有方块
                canBuild = true;
            }

            if (canBuild)
            {
                helpers_CANTBUILD.transform.position = new Vector3(-1000, 0, -1000);

                if (Input.GetMouseButtonDown(0))
                {
                    isMouseDown = true;

                    uMBE.massBuildStart(currentTile, currentObjectGUID);
                }
            }
            else
            {
                helpers_CANTBUILD.transform.position = currentTile.transform.position + new Vector3(currentTile.GetComponent<BoxCollider>().center.x * currentTile.transform.localScale.x, currentTile.GetComponent<BoxCollider>().center.y * currentTile.transform.localScale.y, currentTile.GetComponent<BoxCollider>().center.z * currentTile.transform.localScale.z);
                helpers_CANTBUILD.transform.localScale = currentTile.GetComponent<Collider>().bounds.size + new Vector3(0.1f, 0.1f, 0.1f);
            }
        }

        txtMapDescription.text = "Project [<color=green>" + newProjectName +
            "</color>] Object Count [<color=green>" + GlobalSettings.mapObjectCount +
            "</color>] Tile [<color=green>" + ReturnGameObjectNameIfExists(currentTile) +
            "</color>] Grid [<color=green>" + globalGridSizeX + "x" + globalGridSizeZ + "</color>]";
        if (currentTile != null)
        {
            txtMapDescription.text += " Current Tile Position: " + currentTile.transform.position.x.ToString("0.0") + ", " + currentTile.transform.position.y.ToString("0.0") + ", " + currentTile.transform.position.z.ToString("0.0");
        }
    }

    private void FixedUpdate()
    {
        if (isMouseDown && currentMode == MapEditorMode.Place && canBuild)
        {
            if (currentTile)
            {
                StartCoroutine(uMBE.AddTile(currentTile));
            }
        }
    }

    private IEnumerator SmoothObjInit(GameObject obj)
    {
        Vector3 wholeScale = obj.transform.localScale;
        float objSizeY = obj.transform.localScale.y;
        float objSizeYDiv = objSizeY / 5.0f;

        obj.transform.localScale = new Vector3(obj.transform.localScale.x, 0, obj.transform.localScale.z);
        obj.transform.position -= new Vector3(0, objSizeY, 0);

        for (int i = 0; i < 5; i++)
        {
            obj.transform.localScale += new Vector3(0, objSizeYDiv, 0);
            obj.transform.position += new Vector3(0, objSizeYDiv, 0);
            yield return 0;
        }

        obj.transform.localScale = wholeScale;

        yield return 0;
    }

    private IEnumerator ReloadTileAssets()
    {
        LoadGlobalSettings();
        LoadTools();
        yield return StartCoroutine(LoadTiles());
        LoadTilesIntoGUI();
        FinalizeGridAndCamera();
    }

    private IEnumerator LoadTiles()
    {
        GameObject TEMP_DYNAMIC = new GameObject("dynamic_objs");
        GameObject TEMP = GameObject.Find("TEMP");
        TEMP_DYNAMIC.transform.parent = TEMP.transform;
        TEMP.transform.position -= new Vector3(-1000000000.0f, 100000000.0f, -1000000000.0f);

        StreamReader sr = new StreamReader(GlobalSettings.MapTilesPath);
        string json = sr.ReadToEnd();
        sr.Close();
        MapTilesData mapTilesData = JsonUtility.FromJson<MapTilesData>(json);

        for (int i = 0; i < mapTilesData.categories.Length; i++)
        {
            CategoryTilesData categoryTilesData = mapTilesData.categories[i];
            string cName = categoryTilesData.CategoryName;
            List<GameObject> cObjs = new List<GameObject>();
            List<Texture2D> cObjsP = new List<Texture2D>();
            List<string> cObjsNames = new List<string>();
            List<string> cObjsIDs = new List<string>();
            List<string> cObjsPaths = new List<string>();

            for (int j = 0; j < categoryTilesData.tiles.Length; j++)
            {
                TileData tileData = categoryTilesData.tiles[j];
                GameObject tGO = Resources.Load<GameObject>(tileData.LoadPath);
                cObjsP.Add(RuntimePreviewGenerator.GenerateModelPreview(tGO.transform, 60, 60));
                cObjsNames.Add(tGO.name);
                cObjsIDs.Add(tileData.id);
                cObjsPaths.Add(tileData.LoadPath);
                GameObject tmp_tGO = Instantiate(tGO, Vector3.zero, new Quaternion(0, 0, 0, 0));
                tmp_tGO.name = tileData.id;
                GameObject behindGO = CreateColliderToObject(tmp_tGO);
                behindGO.layer = 2;
                cObjs.Add(behindGO);
                behindGO.transform.parent = TEMP_DYNAMIC.transform;
            }

            #region 按名称进行排序
            List<string> oldNames = new List<string>();
            int foundSame = 1;
            for (int z = 0; z < cObjsNames.Count; z++)
            {
                for (int zx = 0; zx < cObjsNames.Count; zx++)
                {
                    if (cObjsNames[z].Equals(cObjsNames[zx]) && z != zx)
                    {
                        foundSame++;
                        cObjsNames[zx] = cObjsNames[zx] + "(" + foundSame + ")";
                    }
                }

                foundSame = 1;
            }

            for (int z = 0; z < cObjsNames.Count; z++)
            {
                oldNames.Add(cObjsNames[z]);
            }

            cObjsNames.Sort();

            List<int> newIndexs = new List<int>();

            for (int z = 0; z < cObjsNames.Count; z++)
            {
                if (!cObjsNames[z].Equals(oldNames[z]))
                {
                    int foundIndex = 0;
                    for (int x = 0; x < oldNames.Count; x++)
                    {
                        if (oldNames[x].Equals(cObjsNames[z]))
                        {
                            foundIndex = x;
                            break;
                        }
                    }

                    newIndexs.Add(foundIndex);
                }
                else
                {
                    newIndexs.Add(z);
                }
            }

            List<Texture2D> cObjsP_newlist = new List<Texture2D>();
            List<string> cObjsIDs_newlist = new List<string>();
            List<string> cObjsPaths_newlist = new List<string>();
            List<GameObject> cObjs_newlist = new List<GameObject>();

            for (int x = 0; x < newIndexs.Count; x++)
            {
                cObjsP_newlist.Add(cObjsP[newIndexs[x]]);
                cObjsIDs_newlist.Add(cObjsIDs[newIndexs[x]]);
                cObjsPaths_newlist.Add(cObjsPaths[newIndexs[x]]);
                cObjs_newlist.Add(cObjs[newIndexs[x]]);
            }

            allTiles.Add(new CatInfo(cName, cObjsNames, cObjs_newlist, cObjsP_newlist, cObjsIDs_newlist, cObjsPaths_newlist));
            #endregion
        }

        yield return 0;
    }

    private void LoadTools()
    {
        MAIN = new GameObject("MAIN");
        cameraMove = MAIN.AddComponent<CameraMoveController>();
        cameraMove.cameraSensitivity = cameraSensitivity;

        GameObject _grid = (GameObject)Resources.Load("uteForEditor/uteLayer");
        grid = Instantiate(_grid, new Vector3((gridsize.x / 2) + 0.5f, -0.5f, (gridsize.z / 2) + 0.5f), _grid.transform.rotation);
        grid.name = "Grid";
        grid.SetActive(true);
        if (globalGridSizeX % 2.0f != 0.0f)
        {
            grid.transform.position -= new Vector3(0.5f, 0, 0);
        }
        if (globalGridSizeZ % 2.0f != 0.0f)
        {
            grid.transform.position -= new Vector3(0, 0, 0.5f);
        }
        grid.transform.localScale = new Vector3(globalGridSizeX, 0.01f, globalGridSizeZ);
        grid.GetComponent<MeshFilter>().mesh.uv = new Vector2[]
        {
            new Vector2(0,0), new Vector2(globalGridSizeX,0), new Vector2(0,globalGridSizeZ), new Vector2(globalGridSizeX,globalGridSizeZ),
            new Vector2(0,globalGridSizeZ), new Vector2(globalGridSizeX,globalGridSizeZ), new Vector2(0,globalGridSizeZ), new Vector2(globalGridSizeX,globalGridSizeZ),
            new Vector2(0,0), new Vector2(globalGridSizeX,0), new Vector2(0,0), new Vector2(globalGridSizeX,0),
            new Vector2(0,0), new Vector2(0,globalGridSizeZ), new Vector2(globalGridSizeX,globalGridSizeZ), new Vector2(globalGridSizeX,0),
            new Vector2(0,0), new Vector2(0,globalGridSizeZ), new Vector2(globalGridSizeX,globalGridSizeZ), new Vector2(globalGridSizeX,0),
            new Vector2(0,0), new Vector2(0,globalGridSizeZ), new Vector2(globalGridSizeX,globalGridSizeZ), new Vector2(globalGridSizeX,0),
        };

        cameraMove.gameObject.transform.position = Vector3.zero;
        MAP = GameObject.Find("MAP");
        GameObject.Find("MapEditorCamera").transform.parent = MAIN.transform;
        GameObject.Find("MapEditorCamera").transform.position = Vector3.zero;

        SetCamera(cameraType);
    }

    private void LoadTilesIntoGUI()
    {
        dropdownTileCategories.ClearOptions();
        List<string> listCategories = new List<string>();
        for (int i = 0; i < allTiles.Count; i++)
        {
            listCategories.Add(allTiles[i].catName);
        }
        dropdownTileCategories.AddOptions(listCategories);
        dropdownTileCategories.value = 0;
        RefreshCategoryTiles();
    }

    private void LoadGlobalSettings()
    {
        globalGridSizeX = 1024;
        globalGridSizeZ = 1024;

        //Add("isometric-perspective");
        //Add("isometric-ortho");
        //Add("2d-perspective");
        //Add("2d-ortho");
        cameraType = "isometric-perspective";

        globalYSize = 1.0f;
        cameraSensitivity = 1.0f;
    }

    private void SetCamera(string camType)
    {
        GameObject rotationArea = new GameObject("YArea");
        cameraGO.transform.parent = rotationArea.transform;
        rotationArea.transform.parent = MAIN.transform;
        cameraGO.transform.position = Vector3.zero;
        Camera camTemp = cameraGO.GetComponent<Camera>();

        if (camType.Equals("isometric-perspective"))
        {
            MAIN.transform.localEulerAngles = new Vector3(0, 45, 0);
            cameraGO.transform.localEulerAngles = new Vector3(30, 0, 0);
            camTemp.orthographic = false;
            camTemp.fieldOfView = 60;
            camTemp.farClipPlane = 1000.0f;
            isOrtho = false;
            is2D = false;
        }
        else if (camType.Equals("isometric-ortho"))
        {
            MAIN.transform.localEulerAngles = new Vector3(0, 45, 0);
            cameraGO.transform.localEulerAngles = new Vector3(30, 0, 0);
            camTemp.orthographic = true;
            camTemp.orthographicSize = 5;
            camTemp.nearClipPlane = -100.0f;
            camTemp.farClipPlane = 1000.0f;
            is2D = false;
            isOrtho = true;
        }
        else if (camType.Equals("2d-perspective"))
        {
            MAIN.transform.localEulerAngles = new Vector3(0, 0, 0);
            camTemp.orthographic = false;
            camTemp.nearClipPlane = 0.1f;
            camTemp.farClipPlane = 1000.0f;
            isOrtho = false;
            is2D = true;
        }
        else if (camType.Equals("2d-ortho"))
        {
            MAIN.transform.localEulerAngles = new Vector3(0, 0, 0);
            camTemp.orthographic = true;
            camTemp.orthographicSize = 5;
            camTemp.nearClipPlane = -10.0f;
            camTemp.farClipPlane = 300.0f;
            isOrtho = true;
            is2D = true;
        }
    }

    private void FinalizeGridAndCamera()
    {
        if (is2D)
        {
            cameraMove.is2D = true;
            cameraGO.transform.Rotate(new Vector3(90, 0, 0));
            MAIN.transform.position = new Vector3(500, 14, 490);
        }
        else
        {
            cameraMove.is2D = false;

            if (isOrtho)
            {
                MAIN.transform.position = new Vector3(492, 8, 492);
            }
            else
            {
                MAIN.transform.position = new Vector3(493, 8, 493);
            }
        }
    }

    // 给 Instantiate 出来的 GameObject 加上一个 Box Collider (计算这个 model 在 x, y, z 三个方向的最大长度)
    public GameObject CreateColliderToObject(GameObject obj)
    {
        float lowestPointY = 10000.0f;
        float highestPointY = -10000.0f;
        float lowestPointZ = 10000.0f;
        float highestPointZ = -10000.0f;
        float lowestPointX = 10000.0f;
        float highestPointX = -10000.0f;
        float finalYSize = 1.0f;
        float finalZSize = 1.0f;
        float finalXSize = 1.0f;
        float divX = 2.0f;
        float divY = 2.0f;
        float divZ = 2.0f;

        Vector3 objScale = obj.transform.localScale;
        obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        MeshFilter mfs = obj.GetComponent<MeshFilter>();
        MeshFilter[] mfs_arr = obj.GetComponentsInChildren<MeshFilter>();
        SkinnedMeshRenderer smfs = (SkinnedMeshRenderer)obj.GetComponent(typeof(SkinnedMeshRenderer));
        SkinnedMeshRenderer[] smfs_arr = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
        Transform[] trms = obj.GetComponentsInChildren<Transform>();

        if (mfs && mfs.GetComponent<Renderer>())
        {
            lowestPointY = mfs.GetComponent<Renderer>().bounds.min.y;
            highestPointY = mfs.GetComponent<Renderer>().bounds.max.y;
        }

        if (mfs_arr.Length > 0)
        {
            for (int i = 0; i < mfs_arr.Length; i++)
            {
                MeshFilter mf_c = mfs_arr[i];

                if (mf_c && mf_c.GetComponent<Renderer>())
                {
                    if (mf_c.GetComponent<Renderer>().bounds.min.y < lowestPointY)
                    {
                        lowestPointY = mf_c.GetComponent<Renderer>().bounds.min.y;
                    }

                    if (mf_c.GetComponent<Renderer>().bounds.max.y > highestPointY)
                    {
                        highestPointY = mf_c.GetComponent<Renderer>().bounds.max.y;
                    }

                    if (mf_c.GetComponent<Renderer>().bounds.min.x < lowestPointX)
                    {
                        lowestPointX = mf_c.GetComponent<Renderer>().bounds.min.x;
                    }

                    if (mf_c.GetComponent<Renderer>().bounds.max.x > highestPointX)
                    {
                        highestPointX = mf_c.GetComponent<Renderer>().bounds.max.x;
                    }

                    if (mf_c.GetComponent<Renderer>().bounds.min.z < lowestPointZ)
                    {
                        lowestPointZ = mf_c.GetComponent<Renderer>().bounds.min.z;
                    }

                    if (mf_c.GetComponent<Renderer>().bounds.max.z > highestPointZ)
                    {
                        highestPointZ = mf_c.GetComponent<Renderer>().bounds.max.z;
                    }
                }
            }
        }

        if (smfs)
        {
            lowestPointY = smfs.GetComponent<Renderer>().bounds.min.y;
            highestPointY = smfs.GetComponent<Renderer>().bounds.max.y;
        }

        if (smfs_arr.Length > 0)
        {
            for (int i = 0; i < smfs_arr.Length; i++)
            {
                SkinnedMeshRenderer smfs_c = smfs_arr[i];

                if (smfs_c)
                {
                    if (smfs_c.GetComponent<Renderer>().bounds.min.y < lowestPointY)
                    {
                        lowestPointY = smfs_c.GetComponent<Renderer>().bounds.min.y;
                    }

                    if (smfs_c.GetComponent<Renderer>().bounds.max.y > highestPointY)
                    {
                        highestPointY = smfs_c.GetComponent<Renderer>().bounds.max.y;
                    }

                    if (smfs_c.GetComponent<Renderer>().bounds.min.x < lowestPointX)
                    {
                        lowestPointX = smfs_c.GetComponent<Renderer>().bounds.min.x;
                    }

                    if (smfs_c.GetComponent<Renderer>().bounds.max.x > highestPointX)
                    {
                        highestPointX = smfs_c.GetComponent<Renderer>().bounds.max.x;
                    }

                    if (smfs_c.GetComponent<Renderer>().bounds.min.z < lowestPointZ)
                    {
                        lowestPointZ = smfs_c.GetComponent<Renderer>().bounds.min.z;
                    }

                    if (smfs_c.GetComponent<Renderer>().bounds.max.z > highestPointZ)
                    {
                        highestPointZ = smfs_c.GetComponent<Renderer>().bounds.max.z;
                    }
                }
            }
        }

        if (highestPointX - lowestPointX != -20000)
        {
            finalXSize = highestPointX - lowestPointX;
        }
        else
        {
            finalXSize = 1.0f; divX = 1.0f; lowestPointX = 0; Debug.Log("X Something wrong with " + obj.name);
        }

        if (highestPointY - lowestPointY != -20000)
        {
            finalYSize = highestPointY - lowestPointY;
        }
        else { finalYSize = globalYSize; divY = 1.0f; lowestPointY = 0; Debug.Log("Y Something wrong with " + obj.name); }

        if (highestPointZ - lowestPointZ != -20000)
        {
            finalZSize = highestPointZ - lowestPointZ;
        }
        else { finalZSize = 1.0f; divZ = 1.0f; lowestPointZ = 0; Debug.Log("Z Something wrong with " + obj.name); }

        for (int i = 0; i < trms.Length; i++)
        {
            GameObject trm_go = trms[i].gameObject;
            trm_go.layer = 2;
        }

        GameObject behindGO = new GameObject(obj.name);
        behindGO.AddComponent<BoxCollider>();
        obj.transform.parent = behindGO.transform;

        if (Mathf.Approximately(finalXSize, 1.0f) || finalXSize < 1.0f)
        {
            if (finalXSize < 1.0f)
            {
                divX = 1.0f;
                lowestPointX = -1.0f;
            }

            finalXSize = 1.0f;
        }

        if (Mathf.Approximately(finalYSize, 0.0f)) { finalYSize = 0.01f; divY = 0.1f; lowestPointY = 0.0f; }

        if (Mathf.Approximately(finalZSize, 1.0f) || finalZSize < 1.0f)
        {
            if (finalZSize < 1.0f)
            {
                divZ = 1.0f;
                lowestPointZ = -1.0f;
            }

            finalZSize = 1.0f;
        }
        behindGO.transform.localScale = objScale;
        ((BoxCollider)behindGO.GetComponent(typeof(BoxCollider))).size = new Vector3(finalXSize, finalYSize, finalZSize);
        ((BoxCollider)behindGO.GetComponent(typeof(BoxCollider))).center = new Vector3(finalXSize / divX + lowestPointX, finalYSize / divY + lowestPointY, finalZSize / divZ + lowestPointZ);

        DisableAllExternalColliders(obj);

        // behindGO 只是一个 collider, obj 才是实际模型
        return behindGO;
    }

    private void DisableAllExternalColliders(GameObject obj)
    {
        BoxCollider[] boxColls = obj.GetComponentsInChildren<BoxCollider>();

        for (int i = 0; i < boxColls.Length; i++)
        {
            BoxCollider coll = boxColls[i];
            if (coll) coll.enabled = false;
        }

        MeshCollider[] mrColls = obj.GetComponentsInChildren<MeshCollider>();

        for (int i = 0; i < mrColls.Length; i++)
        {
            MeshCollider coll = mrColls[i];
            if (coll) coll.enabled = false;
        }

        SphereCollider[] spColls = obj.GetComponentsInChildren<SphereCollider>();

        for (int i = 0; i < spColls.Length; i++)
        {
            SphereCollider coll = spColls[i];
            if (coll) coll.enabled = false;
        }

        CapsuleCollider[] cpColls = obj.GetComponentsInChildren<CapsuleCollider>();

        for (int i = 0; i < cpColls.Length; i++)
        {
            CapsuleCollider coll = cpColls[i];
            if (coll) coll.enabled = false;
        }
    }

    private void ShowTopView()
    {
        if (!isInTopView)
        {
            MAIN.transform.position += new Vector3(0, 5, 0);
        }

        isInTopView = true;
        cameraMove.isInTopView = true;
        MAIN.transform.localEulerAngles = new Vector3(MAIN.transform.localEulerAngles.x, 0, MAIN.transform.localEulerAngles.z);
        GameObject cameraYRot = GameObject.Find("MAIN/YArea");
        cameraYRot.transform.localEulerAngles = new Vector3(0, 0, 0);
        cameraGO.transform.localEulerAngles = new Vector3(90, cameraGO.transform.localEulerAngles.y, cameraGO.transform.localEulerAngles.z);
    }

    private IEnumerator TurnCamera90(int iternation, int count)
    {
        for (int i = 0; i < iternation; i++)
        {
            MAIN.transform.Rotate(new Vector3(0, count, 0));
            yield return 0;
        }

        yield return 0;
    }

    private string ReturnGameObjectNameIfExists(GameObject obj)
    {
        if (obj)
        {
            return obj.name;
        }
        else
        {
            return "none";
        }
    }

    private IEnumerator smoothRotate(GameObject obj, Vector3 dir, bool isHorizontal)
    {
        if (isHorizontal)
        {
            isRotated++;
        }
        else
        {
            isRotatedH++;
        }

        int counter = 0;

        while (counter++ != 9)
        {
            obj.transform.Rotate(dir);
            yield return null;
        }

        if (isHorizontal)
        {
            if (isRotated >= 4)
            {
                isRotated = 0;
            }
        }
        else
        {
            if (isRotatedH >= 4)
            {
                isRotatedH = 0;
            }
        }
    }

    private IEnumerator gridSmoothMove(GameObject gridObj, bool isUp, GameObject cam)
    {
        canBuild = false;
        isCamGridMoving = true;

        Vector3 endP = gridObj.transform.position;
        Vector3 camEP = cam.transform.position;
        Vector3 startP = gridObj.transform.position;
        Vector3 startEP = cam.transform.position;

        if (isUp)
        {
            currentLayer++;
            endP += new Vector3(0.0f, globalYSize, 0.0f);
            camEP += new Vector3(0.0f, globalYSize, 0.0f);
        }
        else
        {
            currentLayer--;
            endP -= new Vector3(0.0f, globalYSize, 0.0f);
            camEP -= new Vector3(0.0f, globalYSize, 0.0f);
        }

        while (true)
        {
            gridObj.transform.position = Vector3.Lerp(gridObj.transform.position, endP, Time.deltaTime * 20.0f);
            cam.transform.position = Vector3.Lerp(cam.transform.position, camEP, Time.deltaTime * 20.0f);

            float dist = Vector3.Distance(gridObj.transform.position, endP);

            if (Mathf.Abs(dist) <= 0.1f)
            {
                gridObj.transform.position = endP;
                cam.transform.position = camEP;
                break;
            }

            yield return null;
        }

        if (isUp)
        {
            gridObj.transform.position = startP + new Vector3(0.0f, globalYSize, 0.0f);
            cam.transform.position = startEP + new Vector3(0.0f, globalYSize, 0.0f);
        }
        else
        {
            gridObj.transform.position = startP - new Vector3(0.0f, globalYSize, 0.0f);
            cam.transform.position = startEP - new Vector3(0.0f, globalYSize, 0.0f);
        }

        yield return 0;
        canBuild = true;
        isCamGridMoving = false;
    }

    private void ResetCamera()
    {
        if (isInTopView)
        {
            cameraMove.isInTopView = false;
            isInTopView = false;
            MAIN.transform.position -= new Vector3(0, 5, 0);
        }

        GameObject cameraYRot = GameObject.Find("MAIN/YArea");
        cameraYRot.transform.localEulerAngles = Vector3.zero;

        if (is2D)
        {
            cameraGO.transform.localEulerAngles = new Vector3(90, 0, 0);
            MAIN.transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            MAIN.transform.localEulerAngles = new Vector3(0, 45, 0);
            cameraGO.transform.localEulerAngles = new Vector3(30, 0, 0);
        }
    }

    private float RoundTo(float point, float toRound = 2.0f)
    {
        point *= toRound;
        point = Mathf.Round(point);
        point /= toRound;

        return point;
    }

    private string ReturnCondition(bool isTrue)
    {
        if (isTrue)
        {
            return "on";
        }
        else
        {
            return "off";
        }
    }

    public void SaveMapFoo()
    {
        StartCoroutine(SaveMap(newProjectName));
    }

    public IEnumerator SaveMap(string mapName)
    {
        txtStatus.text = "正在保存地图...";

        MapData myData = new MapData();

        GameObject main = GameObject.Find("MAP");
        TagMapObject[] allObjects = main.GetComponentsInChildren<TagMapObject>();
        for (int i = 0; i < allObjects.Length; i++)
        {
            if (i % 2000 == 0) yield return 0;

            GameObject obj = allObjects[i].gameObject;
            string objGUID = allObjects[i].objGUID;

            myData.blocks += objGUID + ":" + obj.transform.position.x + ":" + obj.transform.position.y + ":" + obj.transform.position.z + ":" + ((int)obj.transform.localEulerAngles.x) + ":" + ((int)obj.transform.localEulerAngles.y) + ":" + ((int)obj.transform.localEulerAngles.z) + ":0:0:-:default:$";
        }

        yield return 0;

        GameObject MAIN = GameObject.Find("MAIN");
        GameObject YArea = GameObject.Find("MAIN/YArea");
        GameObject MapEditorCamera = GameObject.Find("MAIN/YArea/MapEditorCamera");
        myData.settings = MAIN.transform.position.x + ":" + MAIN.transform.position.y + ":" + MAIN.transform.position.z + ":" + MAIN.transform.localEulerAngles.x + ":" + MAIN.transform.localEulerAngles.y + ":" + MAIN.transform.localEulerAngles.z + ":" + YArea.transform.localEulerAngles.x + ":" + YArea.transform.localEulerAngles.y + ":" + YArea.transform.localEulerAngles.z + ":" + MapEditorCamera.transform.localEulerAngles.x + ":" + MapEditorCamera.transform.localEulerAngles.y + ":" + MapEditorCamera.transform.localEulerAngles.z + ":";

        yield return 0;

        #region GetMapBoundsInfo
        float mostLeft = 100000000.0f;
        float mostRight = -100000000.0f;
        float mostForward = -100000000.0f;
        float mostBack = 100000000.0f;
        float mostBottom = 100000000.0f;
        float mostUp = -100000000.0f;

        TagMapObject[] tS = MAP.GetComponentsInChildren<TagMapObject>();

        for (int i = 0; i < tS.Length; i++)
        {
            Vector3 tP = tS[i].gameObject.transform.position;
            Bounds bounds = tS[i].gameObject.GetComponent<Collider>().bounds;
            float vMLminus = (tP.x - (bounds.size.x / 2.0f));
            float vMLplus = (tP.x + (bounds.size.x / 2.0f));
            float vMFminus = tP.z - (bounds.size.z / 2.0f);
            float vMFplus = tP.z + (bounds.size.z / 2.0f);
            float vMUminus = (tP.y - (bounds.size.y / 2.0f));
            float vMUplus = (tP.y + (bounds.size.y / 2.0f));

            if (vMLminus < mostLeft)
            {
                mostLeft = vMLminus;
            }

            if (vMLplus > mostRight)
            {
                mostRight = vMLplus;
            }

            if (vMFminus < mostBack)
            {
                mostBack = vMFminus;
            }

            if (vMFplus > mostForward)
            {
                mostForward = vMFplus;
            }

            if (vMUminus < mostBottom)
            {
                mostBottom = vMUminus;
            }

            if (vMUplus > mostUp)
            {
                mostUp = vMUplus;
            }
        }

        myData.mostLeft = (float)System.Math.Round(mostLeft, 2);
        myData.mostRight = (float)System.Math.Round(mostRight, 2);
        myData.mostForward = (float)System.Math.Round(mostForward, 2);
        myData.mostBack = (float)System.Math.Round(mostBack, 2);
        myData.mostUp = (float)System.Math.Round(mostUp, 2);
        myData.mostBottom = (float)System.Math.Round(mostBottom, 2);
        #endregion

        yield return 0;

        StreamWriter sw = new StreamWriter(GlobalSettings.MapLevelsDir + mapName + ".map");
        sw.Write("");
        sw.Write(JsonUtility.ToJson(myData));
        sw.Flush();
        sw.Close();

        txtStatus.text = "地图保存成功!";
    }

    public void RefreshCategoryTiles()
    {
        if (allTiles.Count > 0)
        {
            uMBE.StepOne();

            FilterTilesBySearchBox();

            for (int i = 0; i < listMapTiles.Count; i++)
            {
                Destroy(listMapTiles[i].gameObject);
            }
            listMapTiles.Clear();
            for (int i = 0; i < currentCategoryGoes.Count; i++)
            {
                GameObject currentCatGo = currentCategoryGoes[i];
                string currentCatGuid = currentCategoryGuids[i];
                string currentCatName = currentCategoryNames[i];

                MapTile tile = Instantiate(prefabTile);

                Texture2D previewObjTexture = currentCategoryPrevs[i];
                if (previewObjTexture)
                {
                    tile.imgPreview.sprite = Sprite.Create(previewObjTexture,
                        new Rect(0.0f, 0.0f, previewObjTexture.width, previewObjTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                }

                tile.txtTile.text = currentCatName;

                tile.onClickCallback = () =>
                {
                    uMBE.StepOne();

                    if (currentTile)
                    {
                        Destroy(currentTile);
                    }

                    currentTile = Instantiate(currentCatGo, new Vector3(0.0f, 0.0f, 0.0f), currentCatGo.transform.rotation);
                    TagMapObject tempTag = currentTile.AddComponent<TagMapObject>();
                    tempTag.objGUID = currentCatGuid.ToString();
                    currentTile.AddComponent<Rigidbody>();
                    currentTile.GetComponent<Rigidbody>().useGravity = false;
                    currentTile.GetComponent<BoxCollider>().size -= new Vector3(0.0000001f, 0.0000001f, 0.0000001f);
                    currentTile.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                    currentTile.GetComponent<Collider>().isTrigger = true;
                    // ignore raycast
                    currentTile.layer = 2;
                    currentTile.name = currentCatName.ToString();
                    currentObjectGUID = currentCatGuid.ToString();

                    helpers_CANTBUILD.transform.position = new Vector3(-1000, 0, -1000);
                    helpers_CANTBUILD.transform.localScale = currentTile.GetComponent<Collider>().bounds.size + new Vector3(0.1f, 0.1f, 0.1f);
                    helpers_CANTBUILD.transform.localRotation = currentTile.transform.localRotation;

                    isRotated = 0;
                    isRotatedH = 0;
                    currentMode = MapEditorMode.Place;
                };

                tile.transform.SetParent(transTileContainer);
                listMapTiles.Add(tile);
            }
        }
    }

    private void FilterTilesBySearchBox()
    {
        string searchContent = inputFieldSearchBox.text;

        if (searchContent.Length <= 0)
        {
            currentCategoryGoes = allTiles[selectedCategoryIndex].catObjs;
            currentCategoryPrevs = allTiles[selectedCategoryIndex].catObjsPrevs;
            currentCategoryNames = allTiles[selectedCategoryIndex].catObjsNames;
            currentCategoryGuids = allTiles[selectedCategoryIndex].catIDNames;
        }
        else
        {
            using (CatInfo FilteredCatInfo = new CatInfo(allTiles[selectedCategoryIndex].catName))
            {
                FilteredCatInfo.catObjs = new List<GameObject>();
                FilteredCatInfo.catObjsPrevs = new List<Texture2D>();
                FilteredCatInfo.catObjsNames = new List<string>();
                FilteredCatInfo.catIDNames = new List<string>();

                for (int i = 0; i < allTiles[selectedCategoryIndex].catObjs.Count; i++)
                {
                    FilteredCatInfo.catObjs.Add(allTiles[selectedCategoryIndex].catObjs[i]);
                }

                for (int i = 0; i < allTiles[selectedCategoryIndex].catObjsPrevs.Count; i++)
                {
                    FilteredCatInfo.catObjsPrevs.Add(allTiles[selectedCategoryIndex].catObjsPrevs[i]);
                }

                for (int i = 0; i < allTiles[selectedCategoryIndex].catObjsNames.Count; i++)
                {
                    FilteredCatInfo.catObjsNames.Add(allTiles[selectedCategoryIndex].catObjsNames[i]);
                }

                for (int i = 0; i < allTiles[selectedCategoryIndex].catIDNames.Count; i++)
                {
                    FilteredCatInfo.catIDNames.Add(allTiles[selectedCategoryIndex].catIDNames[i]);
                }

                for (int i = FilteredCatInfo.catObjsNames.Count - 1; i >= 0; i--)
                {
                    if (!FilteredCatInfo.catObjsNames[i].Contains(searchContent))
                    {
                        FilteredCatInfo.catObjs.RemoveAt(i);
                        FilteredCatInfo.catObjsPrevs.RemoveAt(i);
                        FilteredCatInfo.catObjsNames.RemoveAt(i);
                        FilteredCatInfo.catIDNames.RemoveAt(i);
                    }
                }

                currentCategoryGoes = FilteredCatInfo.catObjs;
                currentCategoryPrevs = FilteredCatInfo.catObjsPrevs;
                currentCategoryNames = FilteredCatInfo.catObjsNames;
                currentCategoryGuids = FilteredCatInfo.catIDNames;
            }
        }
    }

    private void FindAndSelectTileInAllCats(GameObject findObj)
    {
        bool isFound = false;

        for (int i = 0; i < allTiles.Count; i++)
        {
            for (int j = 0; j < allTiles[i].catObjsNames.Count; j++)
            {
                if (allTiles[i].catObjsNames[j].Equals(findObj.name))
                {
                    isFound = true;
                    dropdownTileCategories.value = i;
                    inputFieldSearchBox.text = findObj.name;
                    break;
                }
            }
        }

        if (!isFound)
        {
            Debug.Log("NOT FOUND");
        }
    }

    public string GetTileAssetPathByID(string id)
    {
        string resultPath = null;

        for (int i = 0; i < allTiles.Count; i++)
        {
            for (int j = 0; j < allTiles[i].catIDNames.Count; j++)
            {
                if (allTiles[i].catIDNames[j].Equals(id))
                {
                    resultPath = allTiles[i].catLoadPaths[j];
                }
            }
        }

        return resultPath;
    }

    private void SetCastShadows(bool isTrue)
    {
        RenderSettings.ambientLight = new Color(0.5f, 0.5f, 0.5f, 1f);

        if (isTrue)
        {
            mapLightGO.GetComponent<Light>().shadows = LightShadows.Hard;
            mapLightGO.GetComponent<Light>().shadowStrength = 0.8f;
            mapLightGO.GetComponent<Light>().shadowBias = 0.01f;

            QualitySettings.shadowDistance = 100;
            QualitySettings.shadowCascades = 4;
            QualitySettings.shadowProjection = ShadowProjection.CloseFit;
            isCastShadows = true;
        }
        else
        {
            isCastShadows = false;
            mapLightGO.GetComponent<Light>().shadows = LightShadows.None;
        }
    }
}
