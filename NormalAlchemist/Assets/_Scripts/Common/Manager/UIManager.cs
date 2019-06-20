using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : BaseManager
{
    private UIManager() { }
    protected static UIManager instance = null;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
                instance = new UIManager();
            return instance;
        }
    }

    private emUIStatus emStatus;
    private emUIStatus emTagerStatus;
    internal emUIStatus EmStatus
    {
        get
        {
            return emStatus;
        }

        set
        {
            emTagerStatus = value;
        }
    }

    private List<BaseUI> listUI = null;

    public List<GameObject> MsgBoxList = new List<GameObject>();

    private RectTransform canvas = null;
    public RectTransform MainCanvas
    {
        get
        {
            if (canvas== null)
            {
                canvas = GameMain.Instance.UIRender.GetComponent<UIRender>().UIMainCanvas.gameObject.GetComponent<RectTransform>();
            }
            return canvas;
        }
    }


    private const string strUIRootPath = "Prefabs/UIPrefabs/";

    private List<GameObject> ls_LayerList = null;
    private int i_LayerNum = 10;

    /// <summary>
    /// 初始化函数 UI注册
    /// </summary>
    public override void Init()
    {
        emStatus = 0;
        listUI = new List<BaseUI>();
        ls_LayerList = new List<GameObject>();
        //GameObject canvasPrefab = Resources.Load("Base/Canvas") as GameObject;
        //GameObject canvasPrefab = Resources.Load(strUIRootPath + "Canvas") as GameObject;
        //canvas = GetMainCanvas();         //GameObject.Instantiate(canvasPrefab);

        for (int i = 0; i < i_LayerNum; i++)
        {
            GameObject newLayer = new GameObject("Layer_" + i);
            newLayer.layer = LayerMask.NameToLayer("UI");
            newLayer.transform.SetParent(MainCanvas.transform, false);
            newLayer.AddComponent<Canvas>().overrideSorting = true;
            newLayer.GetComponent<Canvas>().sortingLayerName = ("Layer_" + i);
            newLayer.AddComponent<CanvasGroup>();
            newLayer.AddComponent<GraphicRaycaster>();
            if (!ls_LayerList.Contains(newLayer))
            {
                ls_LayerList.Add(newLayer);
            }
            RectTransform newLayerRect = newLayer.GetComponent<RectTransform>();
            newLayerRect.anchorMin = new Vector2(0, 0);
            newLayerRect.anchorMax = new Vector2(1, 1);
            newLayerRect.offsetMin = Vector2.zero;
            newLayerRect.offsetMax = Vector2.zero;
        }

        //MonoBehaviour.DontDestroyOnLoad(canvas);

        //if (KBEngine.Dbg.debugLevel == KBEngine.DEBUGLEVEL.DEBUG)
        //{
        //    GameObject go = GameObject.Instantiate(Resources.Load("Base/TestButton")) as GameObject;
        //    // GameObject uiPrefab = Resources.Load("Base/LoadingUI") as GameObject;
        //    go.transform.SetParent(UIManager.Instance.GetLayerByLv(9), false);
        //}

        //KBEngine.Event.registerOut("SearchNeedPreHeatSource", this, "SearchNeedPreHeatSource");
    }

    public void ReInit()
    {
        foreach (BaseUI ui in listUI)
        {
            ui.ReInit();
        }
    }

    public override void OnDestroy()
    {
        if (listUI != null)
        {
            listUI.Clear();
        }
        GameObject.DestroyObject(canvas);
    }
    public override void Update()
    {
        if (emStatus != emTagerStatus)
        {
            emStatus = emTagerStatus;
            ChangeUI();
        }
    }
    public override void FixedUpdate()
    {
    }

    /// <summary>
    /// 根据层级在对应的层下创建UI
    /// </summary>
    /// <param name="strName">UI资源名称</param>
    /// <param name="lv">层级（lv越大层级越靠外，0是最底层，保留）</param>
    void CreateUI(string strName, int lv)
    {
        GameObject uiPrefab = ResourceManager.Instance.GetResource(emAssetBundle.Bundle_UIPrefab, strName) as GameObject;

        if (uiPrefab == null)
        {
            Debug.LogError(strName + "is None");
            return;
        }
        if (uiPrefab.GetComponent<BaseUI>() == null)
            return;
        GameObject uiObj = GameObject.Instantiate(uiPrefab);

        uiObj.transform.SetParent(GetLayerByLv(lv), false);
        listUI.Add(uiObj.GetComponent<BaseUI>());
    }

    #region 施工中

    private BaseUI m_SwitchUI;

    //多UI并存,基础UI
    private int m_iNormalRoot = 4;
    private List<BaseUI> Ls_AloneUI = new List<BaseUI>();
    //单UI会排斥其他此类UI
    private int m_iFixRoot = 5;
    //弹出窗口,在其他UI上叠加
    private int m_iPopUpRoot = 6;
    private Stack<BaseUI> m_CoverUIStack = new Stack<BaseUI>();

    //将ui分组，切换状态时将删除对应分组的UI缓存,不在分组中的不受影响
    private Dictionary<emUIStatus, List<Type>> Dict_UIGroup = new Dictionary<emUIStatus, List<Type>>();

    Dictionary<Type, BaseUI> dict_ExceptUI = new Dictionary<Type, BaseUI>();
    Dictionary<Type, BaseUI> dict_AllUICache = new Dictionary<Type, BaseUI>();
    Dictionary<Type, BaseUI> dict_CurrentShowUI = new Dictionary<Type, BaseUI>();

    Dictionary<Type, UIInfo> Map_Type_Info = new Dictionary<Type, UIInfo>()
    {

    };
    struct UIInfo
    {
        public string path;
        public Enum_BaseUIType uiType;
        public int lv;
        public UIInfo(string curPath, Enum_BaseUIType curUIType, int curLv)
        {
            path = curPath;
            uiType = curUIType;
            lv = curLv;
        }
    }

    /// <summary>
    /// 根据层级在对应的层下创建UI
    /// </summary>
    /// <param name="strName">UI资源名称</param>
    /// <param name=" uiType">ui类型 </param>
    void CreateUI(string strName, Enum_BaseUIType uiType, int lv = -1)
    {
        GameObject uiPrefab = ResourceManager.Instance.GetResource(emAssetBundle.Bundle_UIPrefab, strName) as GameObject;
        if (uiPrefab == null)
        {
            Debug.LogError(strName + "is None");
            return;
        }
        if (uiPrefab.GetComponent<BaseUI>() == null)
            return;
        GameObject uiObj = GameObject.Instantiate(uiPrefab);

        uiObj.GetComponent<BaseUI>().UIType = uiType;
        Type t = uiObj.GetComponent<BaseUI>().GetType();
        switch (uiType)
        {
            case Enum_BaseUIType.Except:
                dict_ExceptUI.Add(uiObj.GetComponent<BaseUI>().GetType(), uiObj.GetComponent<BaseUI>());
                uiObj.transform.SetParent(GetLayerByLv(lv), false);
                break;
            case Enum_BaseUIType.Fixed:

                uiObj.transform.SetParent(GetLayerByLv(m_iFixRoot), false);
                break;
            case Enum_BaseUIType.Normal:
                uiObj.transform.SetParent(GetLayerByLv(m_iNormalRoot), false);
                break;
            case Enum_BaseUIType.PopUp:
                uiObj.transform.SetParent(GetLayerByLv(m_iPopUpRoot), false);
                break;
        }
        if (lv != -1)
            uiObj.transform.SetParent(GetLayerByLv(lv), false);
        dict_AllUICache.Add(uiObj.GetComponent<BaseUI>().GetType(), uiObj.GetComponent<BaseUI>());

        if (!Map_Type_Info.ContainsKey(t))
        {
            Map_Type_Info.Add(t, new UIInfo(strName, uiType, lv));
        }
    }

    void CreateTestGO(GameObject uiObj, Enum_BaseUIType uiType, int lv = -1)
    {
        uiObj.GetComponent<BaseUI>().UIType = uiType;
        Type t = uiObj.GetComponent<BaseUI>().GetType();
        switch (uiType)
        {
            case Enum_BaseUIType.Except:
                dict_ExceptUI.Add(uiObj.GetComponent<BaseUI>().GetType(), uiObj.GetComponent<BaseUI>());
                uiObj.transform.SetParent(GetLayerByLv(lv), false);
                break;
            case Enum_BaseUIType.Fixed:
                uiObj.transform.SetParent(GetLayerByLv(m_iFixRoot), false);
                break;
            case Enum_BaseUIType.Normal:
                uiObj.transform.SetParent(GetLayerByLv(m_iNormalRoot), false);
                break;
            case Enum_BaseUIType.PopUp:
                uiObj.transform.SetParent(GetLayerByLv(m_iPopUpRoot), false);
                break;
        }
        if (lv != -1)
            uiObj.transform.SetParent(GetLayerByLv(lv), false);
        dict_AllUICache.Add(uiObj.GetComponent<BaseUI>().GetType(), uiObj.GetComponent<BaseUI>());

        if (!Map_Type_Info.ContainsKey(t))
        {
            //Map_Type_Info.Add(t, new UIInfo(strName, uiType, lv));
        }
    }

    /// <summary>
    /// 根据预设删除一部分Prefab回收空间，只有加入组中的UI会受到影响，否则会一直存在
    /// </summary>
    /// <param name="preStatus">先前状态</param>
    /// <param name="curStatus">当前目标状态</param>
    void SwitchGroup(emUIStatus preStatus, emUIStatus curStatus)
    {
        List<Type> ls_destroyType = new List<Type>();
        List<Type> ls_createType = new List<Type>();
        if (Dict_UIGroup.ContainsKey(preStatus) && Dict_UIGroup.ContainsKey(curStatus))
        {
            ls_destroyType = Dict_UIGroup[preStatus].Except(Dict_UIGroup[curStatus]).ToList();
            ls_createType = Dict_UIGroup[curStatus].Except(Dict_UIGroup[preStatus]).ToList();
        }
        else
        {
            if (!Dict_UIGroup.ContainsKey(preStatus))
                ls_destroyType.Clear();
            if (!Dict_UIGroup.ContainsKey(curStatus))
                ls_createType.Clear();
        }

        foreach (var i in ls_destroyType)
        {
            DestroyUI(i);
        }
        foreach (var i in ls_createType)
        {
            if (Map_Type_Info.ContainsKey(i))
            {
                CreateUI(Map_Type_Info[i].path, Map_Type_Info[i].uiType, Map_Type_Info[i].lv);
            }
            else
            {
                Debug.LogError("Not Have This UI INFO :" + i.ToString());
            }
        }
    }

    struct SceneInfo
    {
        public byte isSaved;
        public List<Type> t_ls_AloneUI;
        public Type t_SwitchUI;
        public Stack<Type> t_sta_CoverUI;
    }

    SceneInfo SavedUIScene;
    void ClearSceneInfo()
    {
        SavedUIScene = new SceneInfo();
    }
    void SaveTheScene()
    {
        SavedUIScene = new SceneInfo();
        SavedUIScene.t_ls_AloneUI = new List<Type>();
        SavedUIScene.t_sta_CoverUI = new Stack<Type>();
        foreach (var i in Ls_AloneUI)
        {
            SavedUIScene.t_ls_AloneUI.Add(i.GetType());
        }
        if (m_SwitchUI != null)
        {
            SavedUIScene.t_SwitchUI = m_SwitchUI.GetType();
        }
        else
        {
            SavedUIScene.t_SwitchUI = null;
        }
        if (m_CoverUIStack != null && m_CoverUIStack.Count > 0)
            foreach (var i in m_CoverUIStack)
            {
                SavedUIScene.t_sta_CoverUI.Push(i.GetType());
            }

        SavedUIScene.isSaved = 1;
    }




    void DestroyUI(Type i)
    {
        if (dict_AllUICache.ContainsKey(i))
        {
            GameObject.Destroy(dict_AllUICache[i].gameObject);
        }
    }

    void ShowUIAndHideOther(Type i)
    {
        //if (dict_CurrentShowUI.ContainsKey(i))
        //    return;
        if (!dict_AllUICache.ContainsKey(i))
            return;
        list_UIToHide.Clear();
        foreach (var ui in dict_CurrentShowUI)
        {
            if (ui.Key != i)
            {
                ui.Value.Hide();
                list_UIToHide.Add(ui.Value);
            }
        }
        foreach (var ui in m_CoverUIStack)
        {
            ui.Hide();

            list_UIToHide.Add(ui);

            if (ui.GetType() == i)
            {
                Debug.LogError("The Same Window More Then Once Push In This Stack :" + i.ToString());
            }
        }
        if (!dict_CurrentShowUI.ContainsKey(i))
            dict_CurrentShowUI.Add(i, GetUI(i));
        if (!dict_CurrentShowUI[i].isActive)
            dict_AllUICache[i].Show(() => { EndShowHide(new List<BaseUI>(list_UIToHide)); });
        m_CoverUIStack.Push(dict_AllUICache[i]);
    }

    List<BaseUI> list_UIToHide = new List<BaseUI>();
    void EndShowHide(List<BaseUI> ls_HideUI)
    {
        foreach (var i in ls_HideUI)
        {
            i.Hide();
        }
    }

    void ExitUIAndShowOther(Type t)
    {
        if (!dict_CurrentShowUI.ContainsKey(t))
            return;
        dict_CurrentShowUI[t].Hide();

        bool isHaveSameUI = false;
        Type popT = null;
        if (m_CoverUIStack.Peek().GetType() == t)
            popT = m_CoverUIStack.Pop().GetType();

        //BaseUI[] staArray = m_CoverUIStack.ToArray();

        bool isNoHideOther = true;
        List<BaseUI> toReDisplayWindow = new List<BaseUI>();
        foreach (var i in m_CoverUIStack)
        {
            if (isNoHideOther)
                toReDisplayWindow.Add(i);
            else
                i.Hide();

            if (i.GetType() == popT)
            {
                isHaveSameUI = true;
            }

            if (i.ShowMode == Enum_BaseUIShowMode.HideOther && isNoHideOther)
            {
                isNoHideOther = false;
                //break;
            }
        }

        if (isNoHideOther)
            foreach (var ui in dict_CurrentShowUI)
            {
                if (ui.Key != t && ui.Value.ShowMode == Enum_BaseUIShowMode.Normal)
                    ui.Value.ReDisplay();
            }

        foreach (var i in toReDisplayWindow)
        {
            i.ReDisplay();
        }

        if (!isHaveSameUI)
            dict_CurrentShowUI.Remove(t);
    }

    public void ShowUI(Type t, Enum_BaseUIShowMode showMode = Enum_BaseUIShowMode.Normal, bool isClearStack = false, bool isSaveScene = false, bool isClearAll = false)
    {
        if (!dict_AllUICache.ContainsKey(t))
            return;

        if (isSaveScene)
        {
            SaveTheScene();
        }

        if (isClearStack)
        {
            ClearStack();
        }

        if (isClearAll)
        {
            ClearAll();
        }

        dict_AllUICache[t].ShowMode = showMode;
        switch (showMode)
        {
            case Enum_BaseUIShowMode.Normal:
                SetUIInCache(t, true);
                Ls_AloneUI.Add(dict_AllUICache[t]);
                break;
            case Enum_BaseUIShowMode.HideOther:
                ShowUIAndHideOther(t);
                m_SwitchUI = dict_AllUICache[t];
                break;
            case Enum_BaseUIShowMode.Cover:
                PushUI(t);
                break;
        }

        CastCurrentShowUI();
    }

    public void CloseOrReturnUIForms(Type t, Enum_BaseUIShowMode showMode)
    {
        if (!dict_AllUICache.ContainsKey(t))
            return;

        switch (showMode)
        {
            case Enum_BaseUIShowMode.Normal:
                SetUIInCache(t, false);
                break;
            case Enum_BaseUIShowMode.HideOther:
                ExitUIAndShowOther(t);
                m_SwitchUI = null;
                break;
            case Enum_BaseUIShowMode.Cover:
                PopUI();
                break;
        }

        CastCurrentShowUI();
    }

    void SetUIInCache(Type t, bool isShow)
    {
        if (!dict_AllUICache.ContainsKey(t))
            return;
        if (isShow)
        {
            if (!dict_CurrentShowUI.ContainsKey(t))
            {
                dict_CurrentShowUI.Add(t, dict_AllUICache[t]);
                if (!dict_CurrentShowUI[t].isActive)
                    dict_AllUICache[t].Show();
            }
        }
        else
        {
            if (dict_CurrentShowUI.ContainsKey(t))
            {
                if (dict_CurrentShowUI[t].isActive)
                    dict_CurrentShowUI[t].Hide();
                dict_CurrentShowUI.Remove(t);
            }
        }
    }

    public BaseUI GetUI(Type t)
    {
        if (!dict_AllUICache.ContainsKey(t))
            return null;
        return dict_AllUICache[t];
    }

    private void PushUI(Type t)
    {
        if (!dict_AllUICache.ContainsKey(t))
        {
            Debug.LogError("Dont Have This UI Type:" + t.Name);
            return;
        }

        if (m_CoverUIStack.Contains(GetUI(t)))
        {
            Debug.LogWarning("The Same Window More Then Once Push In This Stack :" + t.ToString());
        }

        m_CoverUIStack.Push(dict_AllUICache[t]);

        if (!dict_AllUICache[t].isActive)
            dict_AllUICache[t].Show();
    }

    private void PopUI()
    {
        if (m_CoverUIStack.Count == 0)
            return;
        BaseUI ui = null;
        if (m_CoverUIStack.Count >= 1)
        {
            ui = m_CoverUIStack.Pop();
            ui.Hide();
        }

        if (m_CoverUIStack.Count >= 1)
        {
            ui = m_CoverUIStack.Peek();
            ui.ReDisplay();
        }
    }

    private void CastCurrentShowUI()
    {
        if (m_CoverUIStack.Count == 0)
        {
            foreach (var i in Ls_AloneUI)
            {
                Enum_NeedLead_UI uiEnum = TypeToEnum(i.GetType());
                if (uiEnum != Enum_NeedLead_UI.Null)
                {
                    EventManager.Broadcast("UIManager_UIIsShow", new object[] { uiEnum });
                    EventManager.Broadcast("CallGuide", new object[] { dict_AllUICache[i.GetType()].gameObject, uiEnum });
                }
            }
        }
        else
        {
            Enum_NeedLead_UI uiEnum = TypeToEnum(m_CoverUIStack.Peek().GetType());
            if (uiEnum != Enum_NeedLead_UI.Null)
            {
                EventManager.Broadcast("UIManager_UIIsShow", new object[] { uiEnum });

                EventManager.Broadcast("CallGuide", new object[] { dict_AllUICache[m_CoverUIStack.Peek().GetType()].gameObject, uiEnum });
            }
            else
            {
                Debug.LogError(m_CoverUIStack.Peek().GetType().ToString() + " is Not in TypeCastFunction is there right Type Or Forget add it");
            }
        }
    }

    private Enum_NeedLead_UI TypeToEnum(Type t)
    {
        switch (t.ToString())
        {
            case "MainUI":
                return Enum_NeedLead_UI.MainUI;
            case "JoinFamily":
                return Enum_NeedLead_UI.JoinFamily;
            case "SquadsSelect":
                return Enum_NeedLead_UI.Military_SquadSelect;
            case "BattleSquadEdit":
                return Enum_NeedLead_UI.Military_SquadEdit;
            case "DrawCardUI":
                return Enum_NeedLead_UI.DrawCard_Main;
            case "StateWarSelectUI":
                return Enum_NeedLead_UI.StateWar_WarSelect;
            case "StateWarRoom":
                return Enum_NeedLead_UI.StateWar_Room_StateSelect;
            case "MatchingUI":
                return Enum_NeedLead_UI.MatchingUI;
            case "Battle":
                return Enum_NeedLead_UI.BattleUI;
            case "StoryFightUI":
                return Enum_NeedLead_UI.StoryFightUI;
            //InStateWar_CityDetail,
            case "FightingUI":
                return Enum_NeedLead_UI.FightingUI;
            case "FightPrepare":
                return Enum_NeedLead_UI.FightingPrepareUI;
            case "OptionalGenerals":
                return Enum_NeedLead_UI.OptionalGenerals;
            case "Women_CardRepository":
                return Enum_NeedLead_UI.OptionalGenerals_LoverSelect;
            case "Gift":
                return Enum_NeedLead_UI.OptionalGenerals_GiftSelect;
            case "SocialityUI":
                return Enum_NeedLead_UI.Sociality;

        }

        return Enum_NeedLead_UI.Null;
    }

    private void ClearStack()
    {
        if (m_CoverUIStack != null && m_CoverUIStack.Count > 0)
        {
            foreach (var i in m_CoverUIStack)
            {
                i.Hide();
                if (dict_CurrentShowUI.ContainsKey(i.GetType()))
                {
                    dict_CurrentShowUI.Remove(i.GetType());
                }
            }
            m_CoverUIStack.Clear();
        }
    }

    private void ClearAll()
    {
        ClearStack();
        foreach (var i in dict_CurrentShowUI)
        {
            i.Value.Hide();
        }
        dict_CurrentShowUI.Clear();
    }

    #endregion


    bool isBaseUICreate = false;
    public void CreateBaseUI()
    {
        if (isBaseUICreate)
            return;
        #region 原代码
        //CreateUI("Login", 5);
        //CreateUI("CreatePlayer", 5);
        //GameObject go = ResourceManager.Instance.GetTestPrefabByName("UITestPrefab", "NewNoviceGuideUI");
        //GameObject uiObj4 = GameObject.Instantiate(go);
        //uiObj4.transform.SetParent(GetLayerByLv(9), false);
        //listUI.Add(uiObj4.GetComponent<BaseUI>());
        //CreateUI("Main", 5);
        //CreateUI("StateWarUI", 5);
        //CreateUI("FightUI", 5);
        //CreateUI("TalkUI", 5);
        //CreateUI("Military", 5);

        //CreateUI("OptionalGenerals", 5);

        //CreateUI("StateWar", 5);

        //CreateUI("VirtualKeyboard", 6);

        //CreateUI("PlayerInfoDetail", 5);
        //CreateUI("FightingUI", 5);
        //CreateUI("FightResultUIPre", 5);
        //CreateUI("FightPrepare", 5);
        //CreateUI("DrawCardUI", 5);

        //CreateUI("WaitData", i_LayerNum - 1);

        //CreateUI("WarMsg", 5);
        //CreateUI("WorldMsg", 5);

        //CreateUI("Battle", 5);
        //CreateUI("Property", 5);

        //CreateUI("NoticeUI", 5);
        //CreateUI("SocialityUI", 5);

        //CreateUI("SystemSetting", 5);
        //CreateUI("NoviceGuideUI", 5);
        //CreateUI("JoinFamily", 5);
        //CreateUI("StoryFightUI", 5);
        //CreateUI("TaskUI", 5);


        //foreach (BaseUI ui in listUI)
        //{
        //    ui.Init();
        //}
        //isBaseUICreate = true;
        #endregion

        CreateUI("Login", Enum_BaseUIType.Fixed, 5);
        CreateUI("CreatePlayer", Enum_BaseUIType.Fixed, 5);
        //GameObject go = ResourceManager.Instance.GetTestPrefabByName("UITestPrefab", "NewNoviceGuideUI");
        //GameObject uiObj4 = GameObject.Instantiate(go);
        CreateUI("NewNoviceGuideUI", Enum_BaseUIType.PopUp, 9);
        CreateUI("Main", Enum_BaseUIType.Fixed, 5);

        CreateUI("StateWarUI", Enum_BaseUIType.Fixed, 5);
        CreateUI("FightUI", Enum_BaseUIType.Fixed, 5);


        CreateUI("TalkUI", Enum_BaseUIType.Fixed, 5);
        //GameObject go2 = ResourceManager.Instance.GetTestPrefabByName("UITestPrefab", "TalkUI");
        //GameObject uiObj2 = GameObject.Instantiate(go2);
        //CreateTestGO(uiObj2, Enum_BaseUIType.Fixed, 5);

        CreateUI("Military", Enum_BaseUIType.Fixed, 5);
        //GameObject go20= ResourceManager.Instance.GetTestPrefabByName("UITestPrefab", "Military");
        //GameObject uiObj20= GameObject.Instantiate(go20);
        //CreateTestGO(uiObj20, Enum_BaseUIType.Fixed, 5);

        CreateUI("OptionalGenerals", Enum_BaseUIType.Fixed, 5);
        //GameObject go9 = ResourceManager.Instance.GetTestPrefabByName("UITestPrefab", "OptionalGenerals");
        //GameObject uiObj9 = GameObject.Instantiate(go9);
        //CreateTestGO(uiObj9, Enum_BaseUIType.Fixed, 5);


        CreateUI("StateWar", Enum_BaseUIType.Fixed, 5);

        CreateUI("VirtualKeyboard", Enum_BaseUIType.Fixed, 6);

        CreateUI("PlayerInfoDetail", Enum_BaseUIType.Fixed, 5);

        CreateUI("FightingUI", Enum_BaseUIType.Fixed, 5);
        //GameObject go1 = ResourceManager.Instance.GetTestPrefabByName("UITestPrefab", "FightingUI");
        //GameObject uiObj1 = GameObject.Instantiate(go1);
        //CreateTestGO(uiObj1, Enum_BaseUIType.Fixed, 5);

        CreateUI("FightResultUIPre", Enum_BaseUIType.Fixed, 5);
        CreateUI("FightPrepare", Enum_BaseUIType.Fixed, 5);

        //CreateUI("DrawCardUI", Enum_BaseUIType.Fixed, 5);
        //GameObject go1 = ResourceManager.Instance.GetTestPrefabByName("UITestPrefab", "DrawCardUI");
        //GameObject uiObj1 = GameObject.Instantiate(go1);
        CreateUI("DrawCardUI", Enum_BaseUIType.Fixed, 5);


        CreateUI("WaitData", Enum_BaseUIType.Except, i_LayerNum - 1);

        CreateUI("WarMsg", Enum_BaseUIType.Fixed, 5);
        CreateUI("WorldMsg", Enum_BaseUIType.Fixed, 5);

        CreateUI("Battle", Enum_BaseUIType.Fixed, 5);
        //GameObject go3 = ResourceManager.Instance.GetTestPrefabByName("UITestPrefab", "Battle");
        //GameObject uiObj3 = GameObject.Instantiate(go3);
        //CreateTestGO(uiObj3, Enum_BaseUIType.Fixed, 5);

        CreateUI("Property", Enum_BaseUIType.Fixed, 5);

        CreateUI("NoticeUI", Enum_BaseUIType.Fixed, 5);
        //GameObject go7 = ResourceManager.Instance.GetTestPrefabByName("UITestPrefab", "NoticeUI");
        //GameObject uiObj7 = GameObject.Instantiate(go7);
        //CreateTestGO(uiObj7, Enum_BaseUIType.Fixed, 5);
        CreateUI("SocialityUI", Enum_BaseUIType.Fixed, 5);
        //GameObject go6 = ResourceManager.Instance.GetTestPrefabByName("UITestPrefab", "SocialityUI");
        //GameObject uiObj6 = GameObject.Instantiate(go6);
        //CreateTestGO(uiObj6, Enum_BaseUIType.Fixed, 5);

        CreateUI("SystemSetting", Enum_BaseUIType.Fixed, 5);
        CreateUI("NoviceGuideUI", Enum_BaseUIType.Fixed, 5);
        CreateUI("JoinFamily", Enum_BaseUIType.Fixed, 5);
        CreateUI("StoryFightUI", Enum_BaseUIType.Fixed, 5);
        CreateUI("TaskUI", Enum_BaseUIType.Fixed, 5);
        CreateUI("Achievement", Enum_BaseUIType.Fixed, 5);

        //GameObject go5 = ResourceManager.Instance.GetTestPrefabByName("UITestPrefab", "Achievement");
        //GameObject uiObj5 = GameObject.Instantiate(go5);
        //CreateTestGO(uiObj5, Enum_BaseUIType.Fixed, 5);
        CreateUI("Welfare", Enum_BaseUIType.Fixed, 5);
        //GameObject go8 = ResourceManager.Instance.GetTestPrefabByName("UITestPrefab", "Welfare");
        //GameObject uiObj8 = GameObject.Instantiate(go8);
        //CreateTestGO(uiObj8, Enum_BaseUIType.Fixed, 5);

        foreach (var kv in dict_AllUICache)
        {
            kv.Value.Init();
        }
        isBaseUICreate = true;
    }

    //原代码
    //void ChangeUI()
    //{
    //    switch (EmStatus)
    //    {
    //        case emUIStatus.emUIStatus_None:
    //            ShowUI(null);
    //            break;
    //        case emUIStatus.emUIStatus_Login:
    //            ShowUI(typeof(LoginUI));
    //            break;
    //        case emUIStatus.emUIstatus_CreatePlayer:
    //            ShowUI(typeof(CreatePlayerUI));
    //            break;
    //        case emUIStatus.emUIstatus_MainUI:
    //            ShowUI(typeof(MainUI));
    //            ShowUIDontHideOther(typeof(TalkUI));
    //            //ShowUIDontHideOther(typeof(PlayerInfo));
    //            break;
    //        //TODO之后改为主界面UI
    //        case emUIStatus.emUIstatus_FightUi:
    //            ShowUI(typeof(FightUI));
    //            ShowUIDontHideOther(typeof(FightingUI));
    //            break;
    //        case emUIStatus.emUIstatus_StateWarUI:
    //            ShowUI(typeof(StateWarUI));
    //            ShowUIDontHideOther(typeof(TalkUI));
    //            break;
    //    }
    //}
    void ChangeUI()
    {
        switch (EmStatus)
        {
            case emUIStatus.emUIStatus_None:
                ShowUI(null);
                break;
                //case emUIStatus.emUIStatus_Login:
                //    ShowUI(typeof(LoginUI), Enum_BaseUIShowMode.HideOther, true, isClearAll: true);
                //    ClearSceneInfo();
                //    break;
                //case emUIStatus.emUIstatus_CreatePlayer:
                //    ShowUI(typeof(CreatePlayerUI), Enum_BaseUIShowMode.HideOther, true, isClearAll: true);
                //    ClearSceneInfo();
                //    break;
                //case emUIStatus.emUIstatus_MainUI:
                //    ShowUI(typeof(MainUI), Enum_BaseUIShowMode.HideOther, true, isClearAll: true);
                //    ClearSceneInfo();
                //    //ShowUIDontHideOther(typeof(TalkUI));
                //    //ShowUI(typeof(TalkUI),Enum_BaseUIShowMode.);
                //    //ShowUIDontHideOther(typeof(PlayerInfo));
                //    break;
                ////TODO之后改为主界面UI
                //case emUIStatus.emUIstatus_FightUi:
                //    ShowUI(typeof(FightingUI), Enum_BaseUIShowMode.HideOther, true, true, isClearAll: true);
                //    //ShowUIDontHideOther(typeof(FightingUI));
                //    break;
                //case emUIStatus.emUIstatus_StateWarUI:
                //    ShowUI(typeof(StateWarUI), Enum_BaseUIShowMode.HideOther, true, isClearAll: true);
                //    ClearSceneInfo();
                //    //ShowUIDontHideOther(typeof(TalkUI));
                //    break;
        }
    }

    //基础层级的切换，清空stack
    /// <summary>
    /// 显示传入UI,关闭所有UI
    /// </summary>
    /// <param name="showUI">显示的UI</param>
    void ShowUI(params Type[] showUI)
    {
        List<BaseUI> toShowUI = new List<BaseUI>();
        foreach (BaseUI ui in listUI)
        {
            if (!examineExceptUI(showUI, ui))
            {
                //if (!IsEqualUI(typeof(LoadingUI), ui.GetType()))
                ui.Hide();
            }
            else
            {
                toShowUI.Add(ui);
            }
        }
        if (toShowUI != null)
            foreach (BaseUI ui in toShowUI)
            {
                ui.Show();
            }

        //foreach (BaseUI ui in listUI)
        //{

        //    if (examineExceptUI(showUI, ui))
        //    {
        //        ui.Show();
        //    }
        //    else
        //    {
        //        if (!IsEqualUI(typeof(LoadingUI), ui.GetType()))
        //            ui.Hide();
        //    }
        //}
    }

    /// <summary>
    /// 检查除外UI
    /// </summary>
    /// <param name="exceptUI">除外的UI</param>
    /// <param name="curUI">当前UI</param>
    /// <returns></returns>
    bool examineExceptUI(Type[] exceptUI, BaseUI curUI)
    {
        if (exceptUI == null)
            return false;
        if (curUI == null)
            return false;

        foreach (Type ui in exceptUI)
        {
            if (IsEqualUI(ui, curUI.GetType()))
            {
                return true;
            }
        }
        return false;
    }

    bool IsEqualUI(Type surUI, Type tagerUI)
    {
        return surUI == tagerUI;
    }

    public void SetUIStatus(emUIStatus status)
    {
        EmStatus = status;
    }

    public UIRender UIRender
    {
        get
        {
            return GameMain.Instance.UIRender.GetComponent<UIRender>();
        }
    }

    public float CanvasScaleX
    {
        get
        {
            return Screen.height / MainCanvas.rect.height;
        }
    }
    public float CanvasScaleY
    {
        get
        {
            return Screen.width / MainCanvas.rect.width;
        }
    }

    /// <summary>
    /// 显示UI，不关闭其他
    /// </summary>
    /// <param name="showUI">想要显示的UI</param>
    void ShowUIDontHideOther(params Type[] showUI)
    {
        foreach (BaseUI ui in listUI)
        {
            if (examineExceptUI(showUI, ui))
                ui.Show();
        }
    }

    #region 层级相关

    /// <summary>
    /// 根据传入的级别获取对应层级,，当lv大于所有层级时默认返回最前层，
    /// 当lv小于等于0默认返回最底层
    /// </summary>
    /// <param name="lv">层级级别(越大越在前面)</param>
    /// <returns>层级的Transform</returns>
    public Transform GetLayerByLv(int lv)
    {
        if (ls_LayerList == null)
        {
            return MainCanvas.transform;
        }
        if (lv >= ls_LayerList.Count)
        {
            return ls_LayerList[ls_LayerList.Count - 1].transform;
        }
        if (lv < 0)
        {
            return ls_LayerList[0].transform;
        }
        return ls_LayerList[lv].transform;
    }

    #endregion

    //public T GetUI<T>() where T : BaseUI
    //{
    //    if (listUI == null)
    //        return null;
    //    foreach (BaseUI ui in listUI)
    //    {
    //        if (typeof(T) == ui.GetType())
    //            return ui as T;
    //    }
    //    return null;
    //}

    public T GetUI<T>() where T : BaseUI
    {
        if (dict_AllUICache.ContainsKey(typeof(T)))
        {
            return dict_AllUICache[typeof(T)] as T;
        }
        return null;
    }

    //public void DestroyPreSceneUI()
    //{
    //    foreach(var ui in listUI)
    //    {
    //        if (typeof(LoadingUI) != ui.GetType())
    //        {
    //            (ui.gameObject);
    //        }
    //    }
    //}

    //public void onChangeSceneDone(emSceneStatus sceneStatus, emSceneStatus lastSceneStatus)
    //{
    //    switch (sceneStatus)
    //    {
    //        case emSceneStatus.emSceneStatus_Game:
    //            //GetUI<MainUI>().UpdateState();
    //            UIManager.Instance.EmStatus = emUIStatus.emUIstatus_MainUI;

    //            break;
    //        case emSceneStatus.emSceneStatus_Login:
    //            CreateBaseUI();
    //            UIManager.Instance.EmStatus = emUIStatus.emUIStatus_Login;
    //            UIManager.Instance.ReInit();
    //            break;
    //        case emSceneStatus.emSceneStatus_Fight:
    //            UIManager.Instance.EmStatus = emUIStatus.emUIstatus_FightUi;
    //            break;
    //        case emSceneStatus.emSceneStatus_StateWar:
    //            UIManager.Instance.EmStatus = emUIStatus.emUIstatus_StateWarUI;
    //            break;
    //        default:
    //            break;
    //    }
    //}

    //public void SearchNeedPreHeatSource(emSceneStatus sceneStatus)
    //{
    //    //SceneManager.Instance.
    //}

}