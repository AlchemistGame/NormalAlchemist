using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
public class GameMain : MonoBehaviour
{
    public static GameMain Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(UIRender);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        StartCoroutine(Splash());
    }

    private void Start()
    {
        InitScene();
    }

    private void InitScene()
    {
        ACTOR_INFO enemy_info = new ACTOR_INFO();
        enemy_info.name = "Enemy";
        enemy_info.position = new Vector3(0.5f, 0, 8.5f);
        enemy_info.rotation = new Vector3(0, 180, 0);
        ActorManager.Instance.CreateActor("Model/UnityChan", "Enemy", enemy_info);

        ACTOR_INFO player_info = new ACTOR_INFO();
        player_info.name = "Player";
        player_info.position = new Vector3(0.5f, 0, 0.5f);
        player_info.rotation = new Vector3(0, 0, 0);
        ActorManager.Instance.CreateActor("Model/UnityChan", "Friend", player_info);
    }
    
    private List<BaseManager> listManager = null;

    public GameObject UIRender = null;
    
    public GameObject splash;

    // TODO 在此处添加全局主管理器
    public IEnumerator init()
    {


        if (listManager != null)
            yield break;
        //KBEngine.Event.fireOut("ChangeLoadingProgress", 0, "客户端引擎初始化...");

        //System.Threading.ThreadPool.SetMaxThreads(SystemInfo.processorCount, SystemInfo.processorCount);
        //System.Threading.ThreadPool.SetMaxThreads(1, 1);
        //errorListener = new ErrorListener();

        listManager = new List<BaseManager>();
        listManager.Add(UIManager.Instance);
        



        int initCount = 0;
        foreach (BaseManager bm in listManager)
        {
            yield return null;
            try
            {
                bm.Init();
                Debug.Log(bm.GetType().Name + ": Init Done!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError(bm.GetType().Name + ": Init Fail! Content:" + ex.Message);
            }
            finally
            {
                initCount++;
                //KBEngine.Event.fireOut("ChangeLoadingProgress", ((float)initCount / listManager.Count) * 0.5f, null);
            }
        }
    }
    

    IEnumerator Splash()
    {
        splash.gameObject.SetActive(true);
        splash.GetComponent<CanvasGroup>().alpha = 0;
        while (splash.GetComponent<CanvasGroup>().alpha != 1)
        {
            yield return new WaitForFixedUpdate();
            splash.GetComponent<CanvasGroup>().alpha += 0.01f;
        }

        yield return new WaitForSeconds(2);

        while (splash.GetComponent<CanvasGroup>().alpha != 0)
        {
            yield return new WaitForFixedUpdate();
            splash.GetComponent<CanvasGroup>().alpha -= 0.01f;
        }

        Destroy(splash);
        splash = null;
        //LoadingUI.ShowLoadingUI();
        //yield return new WaitUntil(() => LoadingUI.Instance.UIShowStatus == Enum_UIShowStatus.Show);
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (listManager == null)
            return;
        foreach (BaseManager bm in listManager)
        {
            bm.Update();
        }
        
    }

    public void FixedUpdate()
    {
        //MessageCenter.Instance.FixedUpdate();
        if (listManager == null)
            return;
        foreach (BaseManager bm in listManager)
        {
            bm.FixedUpdate();
        }
    }

    public void OnDestroy()
    {
        if (listManager == null)
            return;

        foreach (BaseManager bm in listManager)
        {
            bm.OnDestroy();
        }
        listManager.Clear();
    }
}
