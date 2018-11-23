using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AssetBundleDefine;
using System.Threading;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// 资源管理类（资源加载卸载）
/// </summary>
public class ResourceManager : BaseManager
{
    private static string strBundleFilePath = Application.persistentDataPath + "/";

    private ResourceManager() { }
    private static ResourceManager instance = null;

    private Dictionary<emAssetBundle, BundleFileData> dictAssetBundleInfo = new Dictionary<emAssetBundle, BundleFileData>();
    private List<IGetProgress> listAsync = null;
    private bool loadingAsyncFlag = false;

    private Dictionary<string, AudioClip> AudioBundle;

    //用于存储Audio对应的资源包的字典，如资源包内字典的Key为Foot_Run_01,包字典的Key为Player;
    private Dictionary<string, Dictionary<string, AudioClip>> AudioBundleDict = new Dictionary<string, Dictionary<string, AudioClip>>();

    //角色相关资源对应字典
    private Dictionary<string, GameObject> ActorBundle = new Dictionary<string, GameObject>();

    //EntityUI预制
    private Dictionary<string, GameObject> EntityUIDict = new Dictionary<string, GameObject>();

    //图片字典
    //private Dictionary<string, UnityEngine.Object[]> IconDict = new Dictionary<string, UnityEngine.Object[]>();
    private Dictionary<string, Dictionary<string, Sprite>> IconDict = new Dictionary<string, Dictionary<string, Sprite>>();
    //特效字典
    private Dictionary<string, Dictionary<string, GameObject>> EffectDict = new Dictionary<string, Dictionary<string, GameObject>>();
    //prefab字典
    private Dictionary<string, Dictionary<string, GameObject>> PrefabDict = new Dictionary<string, Dictionary<string, GameObject>>();

    public static ResourceManager Instance
    {
        get
        {
            if (instance == null)
                instance = new ResourceManager();
            return instance;
        }
    }

    public bool LoadingAsyncFlag
    {
        get
        {
            return loadingAsyncFlag;
        }
    }

    private string AudioSourcePath = "Audio/";

    private string EntityPrefabSourcePath = "Prefabs/EntityPrefabs/";
    private string EntityUISourcePath = "Prefabs/UIPrefabs/InFightUI/HpBar/";

    private string IconSourcePath = "Sprite/";

    private string IconAssetSourcePath = "/sprites/";

    private string EffectBundleSourcePath = "/resources/prefabs/effectprefabs/";

    private string TwoDMapBundleSourcePath = "/sprites/";

    private string PrefabBundleSourcePath = "/resources/prefabs/";

    private string FolderForTest = "TestPrefabs/";

    public override void Init()
    {
        //LoadBundleFile(emAssetBundle.Bundle_Base,false);
        LoadBundleFile(emAssetBundle.Bundle_Config, false);
        LoadBundleFile(emAssetBundle.Bundle_Config_MonsterMap, false);
        LoadBundleFile(emAssetBundle.Bundle_Audio, false);
        //LoadBundleFile<Sprite>(emAssetBundle.Bundle_SmallSprite, false, false);
        //LoadBundleFile(emAssetBundle.Bundle_Fonts, false, false);

        //LoadBundleFile(emAssetBundle.Bundle_UIPrefab, false);
        //LoadBundleFile(emAssetBundle.Bundle_Audio, false);
    }

    #region tool

    public static String FormatFileSize(long fileSize)
    {
        if (fileSize < 0)
        {
            return "0 bytes";
        }
        else if (fileSize >= 1024 * 1024 * 1024)
        {
            return string.Format("{0:########0.00} GB", ((Double)fileSize) / (1024 * 1024 * 1024));
        }
        else if (fileSize >= 1024 * 1024)
        {
            return string.Format("{0:####0.00} MB", ((Double)fileSize) / (1024 * 1024));
        }
        else if (fileSize >= 1024)
        {
            return string.Format("{0:####0.00} KB", ((Double)fileSize) / 1024);
        }
        else
        {
            return string.Format("{0} bytes", fileSize);
        }
    }

    public static void ReleaseMemory()
    {
        //UnityEngine.Debug.Log(string.Format("=====================Start GC Memory:{0}=====================", FormatFileSize(GC.GetTotalMemory(false))));
        Debug.Log(string.Format("=====================Start GC Memory:{0}=====================", FormatFileSize(GC.GetTotalMemory(false))));
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        //UnityEngine.Debug.Log(string.Format("=====================End GC Memory:{0}=====================", FormatFileSize(GC.GetTotalMemory(false))));
        Debug.Log(string.Format("=====================End GC Memory:{0}=====================", FormatFileSize(GC.GetTotalMemory(false))));
    }

    public static Dictionary<string, UnityEngine.Object> InsertObjsToDict(UnityEngine.Object[] objs)
    {
        Dictionary<string, UnityEngine.Object> rtDict = new Dictionary<string, UnityEngine.Object>(objs.Length);
        foreach (var obj in objs)
        {
            if (!rtDict.ContainsKey(obj.name))
            {
                rtDict.Add(obj.name, obj);
            }
            else
            {
                Debug.Log("Obj Have The Same Name :" + obj.name);
            }
        }
        return rtDict;
    }
    #endregion

    public override void OnDestroy()
    {
        foreach (var bundleFileData in dictAssetBundleInfo)
        {
            bundleFileData.Value.Unload(true);
        }
        dictAssetBundleInfo.Clear();
        if (listAsync != null)
            listAsync.Clear();
        ReleaseMemory();
    }
    public override void Update()
    {
        if (!loadingAsyncFlag)
        {
            return;
        }

        for (int i = NextToLoadAssets.Count - 1; i >= 0; i--)
        {
            if (dictAssetBundleInfo.ContainsKey(NextToLoadAssets[i].assetType))
            {
                dictAssetBundleInfo[NextToLoadAssets[i].assetType].IsWaiting = false;
            }
            else
            {
                NextToLoadAssets[i].isEndLoad = true;
                Debug.LogWarning("Not in Loading AssetBundle");
            }
            NextToLoadAssets.RemoveAt(i);
        }

        if (dictAssetBundleInfo != null)
        {
            List<emAssetBundle> delList = new List<emAssetBundle>();
            foreach (var x in dictAssetBundleInfo)
            {
                try
                {
                    if (x.Value == null)
                    {
                        delList.Add(x.Key);
                        continue;
                    }
                    x.Value.OnUpdate();
                }
                catch (System.Exception ex)
                {
                    delList.Add(x.Key);
                    listAsync.Remove(x.Value);
                    Debug.LogError(string.Format("ResourceManager::LoadingBundle Failed! Key : {0} Error : {1}", x.Key, ex.Message));
                }
            }

            foreach (var key in delList)
            {
                dictAssetBundleInfo.Remove(key);
            }
        }

        bool flag = true;
        foreach (var x in dictAssetBundleInfo)
        {
            if (x.Value == null)
                continue;

            if (!x.Value.IsDone)
            {
                flag = false;
            }
            else
            {
                EndLoadEvent(x.Value.EmBundleType);
            }
        }

        foreach (var i in WaitingLoadAssets)
        {
            if (!i.Value.isEndLoad)
            {
                flag = false;
            }
        }

        if (flag)
        {
            Resources.UnloadUnusedAssets();
            loadingAsyncFlag = false;
            ReleaseMemory();
        }
    }

    public override void FixedUpdate()
    {
    }

    //TODO 在此处加载场景所需资源并卸载上个场景的资源
    public List<IGetProgress> onChangeScene(emSceneStatus sceneStatus, emSceneStatus lastSceneStatus)
    {
        switch (sceneStatus)
        {
            case emSceneStatus.emSceneStatus_Login:
                //LoadBundleFile(emAssetBundle.Bundle_SmallSprite, false, false);
                //LoadBundleFile(emAssetBundle.Bundle_Fonts, false, false);
                //ReleaseResource(emAssetBundle.Bundle_SmallSprite, false);
                //LoadBundleFile(emAssetBundle.Bundle_TalkChannelIcon);
                //LoadBundleFile(emAssetBundle.Bundle_UIPrefab, false);

                //ReleaseResourceRapid(emAssetBundle.Bundle_Fonts);
                //ReleaseResourceRapid(emAssetBundle.Bundle_UIPrefab);
                LoadBundleFile(emAssetBundle.Bundle_Sprite_PlayerHeadIcon);
                BuildAndStartLoadList(new List<emAssetBundle>()
                    {
                        emAssetBundle.Bundle_UISp_LoginAndMian,
                        emAssetBundle.Bundle_UISp_common,
                        emAssetBundle.Bundle_UIPrefab,
                        emAssetBundle.Bundle_TalkChannelIcon,
                        emAssetBundle.Bundle_UISp_DrawCard,
                        emAssetBundle.Bundle_UISp_StateWar,
                        emAssetBundle.Bundle_UISp_Fighting,
                        emAssetBundle.Bundle_Fonts,
                    },
                    TempDependDict
                );
                LoadBundleFile(emAssetBundle.Bundle_GeneralCard, true, false, isNeedPreHeat: true);
                LoadBundleFile(emAssetBundle.Bundle_GeneralCard_1, true, false, isNeedPreHeat: true);
                LoadBundleFile(emAssetBundle.Bundle_GeneralCard_2, true, false, isNeedPreHeat: true);
                LoadBundleFile(emAssetBundle.Bundle_GeneralCard_3, true, false, isNeedPreHeat: true);
                LoadBundleFile(emAssetBundle.Bundle_GeneralCard_4, true, false, isNeedPreHeat: true);
                LoadBundleFile(emAssetBundle.Bundle_GeneralCard_5, true, false, isNeedPreHeat: true);
                LoadBundleFile(emAssetBundle.Bundle_GeneralCard_6, true, false, isNeedPreHeat: true);
                LoadBundleFile(emAssetBundle.Bundle_GeneralCard_7, true, false, isNeedPreHeat: true);
                LoadBundleFile(emAssetBundle.Bundle_GeneralCard_8, true, false, isNeedPreHeat: true);
                //ReleaseResource(emAssetBundle.Bundle_SmallSprite, false);

                break;
            case emSceneStatus.emSceneStatus_Game:
                LoadBundleFile(emAssetBundle.Bundle_TalkChannelIcon);
                //LoadBundleFile(emAssetBundle.Bundle_UISp_DrawCard, true, false);

                //LoadBundleFile(emAssetBundle.Bundle_GeneralCard, true, false, isNeedPreHeat: true);
                //LoadBundleFile(emAssetBundle.Bundle_GeneralCard_1, true, false, isNeedPreHeat: true);
                //LoadBundleFile(emAssetBundle.Bundle_GeneralCard_2, true, false, isNeedPreHeat: true);
                //LoadBundleFile(emAssetBundle.Bundle_GeneralCard_3, true, false, isNeedPreHeat: true);
                //LoadBundleFile(emAssetBundle.Bundle_GeneralCard_4, true, false, isNeedPreHeat: true);
                //LoadBundleFile(emAssetBundle.Bundle_GeneralCard_5, true, false, isNeedPreHeat: true);
                //LoadBundleFile(emAssetBundle.Bundle_GeneralCard_6, true, false, isNeedPreHeat: true);
                //LoadBundleFile(emAssetBundle.Bundle_GeneralCard_7, true, false, isNeedPreHeat: true);
                //LoadBundleFile(emAssetBundle.Bundle_GeneralCard_8, true, false, isNeedPreHeat: true);

                LoadBundleFile(emAssetBundle.Bundle_SoldierCard, true, false, isNeedPreHeat: true);
                LoadBundleFile(emAssetBundle.Bundle_Sprite_GeneralHeadIcon/*, true, false, isNeedPreHeat: true*/);

                LoadBundleFile(emAssetBundle.Bundle_StoryFightMap, true, false, isNeedPreHeat: true);

                LoadBundleFile(emAssetBundle.Bundle_Sprite_SquadFlag/*,true,false,isNeedPreHeat:true*/);
                LoadBundleFile(emAssetBundle.Bundle_Sprite_Achievement);
                LoadBundleFile(emAssetBundle.Bundle_DrawCard);
                LoadBundleFile(emAssetBundle.Bundle_ItemsIcon);
                LoadBundleFile(emAssetBundle.Bundle_StateFlag);
                LoadBundleFile(emAssetBundle.Bundle_Sprite_PlayerHeadIcon);
                LoadBundleFile(emAssetBundle.Bundle_TempResource);
                LoadBundleFile(emAssetBundle.Bundle_SkillIcon);
                LoadBundleFile(emAssetBundle.Bundle_Textures, false, false, isNeedPreHeat: true);
                //LoadBundleFile(emAssetBundle.Bundle_Textures, false, false);
                LoadBundleFile(emAssetBundle.Bundle_Models, false, false, isNeedPreHeat: true);
                LoadBundleFile(emAssetBundle.Bundle_Anim, false, false, isNeedPreHeat: true);
                LoadBundleFile(emAssetBundle.Bundle_SoldierPrefabs);

                break;
            case emSceneStatus.emSceneStatus_StateWar:
                //LoadBundleFile(emAssetBundle.Bundle_UISp_StateWar,true,false);
                LoadBundleFile(emAssetBundle.Bundle_Sprite_2DBuild, true, false, isNeedPreHeat: true);
                LoadBundleFile(emAssetBundle.Bundle_Sprite_GeneralHeadIcon, true, false, isNeedPreHeat: true);
                LoadBundleFile(emAssetBundle.Bundle_Pre_StateWarPrefab);
                LoadBundleFile(emAssetBundle.Bundle_Sprite_PlayerHeadIcon, true, false, isNeedPreHeat: true);
                
                break;
            case emSceneStatus.emSceneStatus_Fight:
                //LoadBundleFile(emAssetBundle.Bundle_UISp_Fighting,true, false);
                LoadBundleFile(emAssetBundle.Bundle_FightMapPrefabs);

                LoadBundleFile(emAssetBundle.Bundle_Textures, false, false, isNeedPreHeat: true);
                //LoadBundleFile(emAssetBundle.Bundle_Textures, false, false);
                LoadBundleFile(emAssetBundle.Bundle_Models, false, false, isNeedPreHeat: true);
                LoadBundleFile(emAssetBundle.Bundle_Anim, false, false, isNeedPreHeat: true);

                LoadBundleFile(emAssetBundle.Bundle_SkillIcon);
                LoadBundleFile(emAssetBundle.Bundle_SoldierPrefabs);
                LoadBundleFile(emAssetBundle.Bundle_SkillEffect);
                LoadBundleFile(emAssetBundle.Bundle_Sprite_GeneralHeadIcon);
                LoadBundleFile(emAssetBundle.Bundle_BuffIcon);
                LoadBundleFile(emAssetBundle.Bundle_GeneralCard, true, false, isNeedPreHeat: true);
                
                break;
        }

        listAsync = new List<IGetProgress>();
        foreach (var x in dictAssetBundleInfo)
        {
            if (x.Value == null)
                continue;
            if (x.Value.IsDone)
                continue;
            listAsync.Add(x.Value);
        }
        return listAsync;
    }

    public void onSceneDestory(emSceneStatus sceneStatus)
    {
        //return;
        switch (sceneStatus)
        {
            case emSceneStatus.emSceneStatus_Game:
                ReleaseResource(emAssetBundle.Bundle_GeneralCard, true, true);

                ReleaseResource(emAssetBundle.Bundle_SoldierCard, true);
                ReleaseResource(emAssetBundle.Bundle_DrawCard, true);
                ReleaseResource(emAssetBundle.Bundle_ItemsIcon, true);
                //ReleaseResource(emAssetBundle.Bundle_StateFlag, true);
                ReleaseResource(emAssetBundle.Bundle_Sprite_PlayerHeadIcon, true);
                ReleaseResource(emAssetBundle.Bundle_TempResource, true);
                break;
            case emSceneStatus.emSceneStatus_Fight:
                ReleaseResource(emAssetBundle.Bundle_Textures, true);
                ReleaseResource(emAssetBundle.Bundle_Models, true);
                ReleaseResource(emAssetBundle.Bundle_Anim, true);

                ReleaseResource(emAssetBundle.Bundle_GeneralCard, true, true);

                ReleaseResource(emAssetBundle.Bundle_SkillIcon, true);

                ReleaseResource(emAssetBundle.Bundle_SoldierPrefabs, true);

                ReleaseResource(emAssetBundle.Bundle_SkillEffect, true);
                ReleaseResource(emAssetBundle.Bundle_Sprite_GeneralHeadIcon, true);
                ReleaseResource(emAssetBundle.Bundle_BuffIcon, true);
                ReleaseResource(emAssetBundle.Bundle_FightMapPrefabs, true);
                break;
            case emSceneStatus.emSceneStatus_StateWar:
                ReleaseResource(emAssetBundle.Bundle_Sprite_2DBuild, true);
                ReleaseResource(emAssetBundle.Bundle_Pre_StateWarPrefab, true);
                ReleaseResource(emAssetBundle.Bundle_StateWar_Map_1_1, true, true);
                ReleaseResource(emAssetBundle.Bundle_Sprite_GeneralHeadIcon, true);
                //List<emAssetBundle> mapList = StateWarDataManager.Instance.GetStateWarReqMapResourceList();
                //if (mapList != null)
                //    foreach (emAssetBundle mapBundle in mapList)
                //    {
                //        LoadBundleFile<Sprite>(mapBundle, true, false, isNeedPreHeat: true);
                //    }
                break;
                //LoadBundleFileAsync(emAssetBundle.Bundle_SoldierCard);
        }
    }

    public UnityEngine.Object GetResource(emAssetBundle emBundle, string objName)
    {
        var listMappingBundle = AssetBundlePath.GetMappingName(emBundle);
        if (listMappingBundle != null)
        {
            UnityEngine.Object obj = null;
            foreach (var bundle in listMappingBundle)
            {
                if (dictAssetBundleInfo.ContainsKey(bundle))
                {
                    obj = dictAssetBundleInfo[bundle].GetResourceObj(objName);
                    if (obj != null)
                    {
                        return obj;
                    }
                }
            }
            Debug.LogWarning("the obj No in BundlePack and BundlePack Mapping! objName:" + objName + "BundleName;" + emBundle.ToString());
        }
        else
        {
            if (dictAssetBundleInfo.ContainsKey(emBundle))
            {
                var obj = dictAssetBundleInfo[emBundle].GetResourceObj(objName);
                if (obj == null)
                {
                    Debug.LogWarning("the obj No in BundlePack and BundlePack Mapping! objName:" + objName + "BundleName;" + emBundle.ToString());
                }
                return obj;
            }
            else
            {
                Debug.LogWarning("No This Loaded Asset Bundle " + emBundle.ToString());
            }
        }

        return null;
    }

    public T GetResource<T>(emAssetBundle emBundle, string objName) where T : UnityEngine.Object
    {
        var listMappingBundle = AssetBundlePath.GetMappingName(emBundle);
        if (listMappingBundle != null)
        {
            T obj = null;
            foreach (var bundle in listMappingBundle)
            {
                if (dictAssetBundleInfo.ContainsKey(bundle))
                {
                    obj = dictAssetBundleInfo[bundle].GetResourceObj<T>(objName);
                    if (obj != null)
                    {
                        return obj;
                    }
                }
            }
            Debug.LogWarning("the obj No in BundlePack and BundlePack Mapping! objName:" + objName + " BundleName;" + emBundle.ToString());
        }
        else
        {
            if (dictAssetBundleInfo.ContainsKey(emBundle))
            {
                T obj = dictAssetBundleInfo[emBundle].GetResourceObj<T>(objName);
                if (obj == null)
                {
                    Debug.LogWarning("the obj No in BundlePack and BundlePack Mapping! objName:" + objName + " BundleName;" + emBundle.ToString());
                }
                return obj;
            }
            else
            {
                Debug.LogWarning("No This Loaded Asset Bundle " + emBundle.ToString());
            }
        }

        return null;
    }

    public List<IEnumerator> GetAllResource(emAssetBundle emBundle)
    {
        List<IEnumerator> listIE = new List<IEnumerator>();
        var listMappingBundle = AssetBundlePath.GetMappingName(emBundle);
        if (listMappingBundle != null)
        {
            foreach (var bundleName in listMappingBundle)
            {
                if (dictAssetBundleInfo.ContainsKey(bundleName))
                {
                    var ie = dictAssetBundleInfo[bundleName].GetAllResource();
                    if (ie != null)
                    {
                        listIE.Add(ie);
                    }
                }
            }
        }
        else
        {
            if (dictAssetBundleInfo.ContainsKey(emBundle))
            {
                var ie = dictAssetBundleInfo[emBundle].GetAllResource();
                if (ie != null)
                {
                    listIE.Add(ie);
                }
            }
        }
        if (listIE.Count > 0)
        {
            return listIE;
        }
        return null;
    }

    public int GetResourceLength(emAssetBundle emBundle)
    {
        int count = 0;
        var listMappingBundle = AssetBundlePath.GetMappingName(emBundle);

        if (listMappingBundle != null)
        {

            foreach (var bundleName in listMappingBundle)
            {
                if (dictAssetBundleInfo.ContainsKey(bundleName))
                {
                    count += dictAssetBundleInfo[bundleName].GetResourceLength();
                }
            }

        }
        else
        {
            if (dictAssetBundleInfo.ContainsKey(emBundle))
            {
                return dictAssetBundleInfo[emBundle].GetResourceLength();
            }
        }
        return count;
    }

    /// <summary>
    /// 快速释放资源(需不释放文件数据)
    /// </summary>
    /// <param name="emBundleType"></param>
    /// <returns></returns>
    public bool ReleaseResourceRapid(emAssetBundle emBundleType)
    {
        return ReleaseResource(emBundleType, false);
    }

    /// <summary>
    /// 快速加载资源(需不释放文件数据)
    /// </summary>
    /// <param name="emBundleType"></param>
    /// <returns></returns>
    public bool LoadBundleFileRapid(emAssetBundle emBundleType)
    {
        if (dictAssetBundleInfo.ContainsKey(emBundleType))
        {
            var bundle = dictAssetBundleInfo[emBundleType];
            if (bundle != null)
            {
                if (bundle.IsCanReload)
                {
                    bool tmp = bundle.Reload();
                    if (tmp)
                    {
                        loadingAsyncFlag = bundle.IsAsync;
                    }
                    return tmp;
                }
                if (bundle.IsDone)
                {
                    Debug.Log("The Bundle Existing! bundleName:" + emBundleType.ToString());
                    return true;
                }
            }
        }
        return false;
    }

    public bool PreHeatAssetSingle(emAssetBundle emBundleType, string objName)
    {
        if (dictAssetBundleInfo.ContainsKey(emBundleType))
        {
            var bundle = dictAssetBundleInfo[emBundleType];
            if (bundle != null)
            {
                if (bundle.IsCanReload)
                {
                    return bundle.PreHeatLoad(objName);
                }
            }
        }
        return false;
    }

    public struct PreHeatInfo
    {
        public emAssetBundle assetBundleType;
        public List<string> ls_PreHeatResourceName;
        /// <summary>
        /// 是否预热之后再也不新增
        /// </summary>
        public bool isNerverUse;
        public PreHeatInfo(emAssetBundle assetBundleType, List<string> preHeatResourceList, bool isNerverUse = false)
        {
            ls_PreHeatResourceName = new List<string>();
            if (preHeatResourceList != null)
                foreach (var res in preHeatResourceList)
                {
                    ls_PreHeatResourceName.Add(res);
                }
            this.assetBundleType = assetBundleType;
            this.isNerverUse = isNerverUse;
        }
    }

    private Dictionary<emAssetBundle, PreHeatInfo> dict_PreHeat = new Dictionary<emAssetBundle, PreHeatInfo>();
    public IEnumerator PreHeatAsset(List<PreHeatInfo> ls_PreHeatInfo)
    {
        dict_PreHeat.Clear();
        int count = 0;
        foreach (var preHeat in ls_PreHeatInfo)
        {
            if (dict_PreHeat.ContainsKey(preHeat.assetBundleType))
            {
                PreHeatInfo preInDict = dict_PreHeat[preHeat.assetBundleType];
                foreach (var name in preHeat.ls_PreHeatResourceName)
                {
                    preInDict.ls_PreHeatResourceName.Add(name);
                }

                preInDict.isNerverUse = preInDict.isNerverUse & preHeat.isNerverUse;

                dict_PreHeat[preHeat.assetBundleType] = preInDict;
            }
            else
            {
                PreHeatInfo insetToDictInfo = new PreHeatInfo(preHeat.assetBundleType, preHeat.ls_PreHeatResourceName, preHeat.isNerverUse);
                dict_PreHeat.Add(preHeat.assetBundleType, insetToDictInfo);
            }

            if (preHeat.ls_PreHeatResourceName != null)
            {
                count += preHeat.ls_PreHeatResourceName.Count;
            }

        }

        int completeCount = 0;
        foreach (var record in dict_PreHeat)
        {
            if (dictAssetBundleInfo.ContainsKey(record.Key))
            {
                foreach (var preHeatName in record.Value.ls_PreHeatResourceName)
                {
                    dictAssetBundleInfo[record.Key].PreHeatLoad(preHeatName);
                    completeCount++;
                }
                dictAssetBundleInfo[record.Key].Unload(record.Value.isNerverUse);
            }
            else
            {
                Debug.LogWarning("Resource " + record.Key.ToString() + " is Need PreHeat But Dont Exist");
            }
            PreHeatProgress = (float)completeCount / count;
            yield return 0;
        }

        PreHeatProgress = 1f;
    }

    private float PreHeatProgress = 0;
    public float GetPreHeatProgress()
    {
        return PreHeatProgress;
    }

    /// <summary>
    /// 在初始化之外加载Bundle包
    /// 强制异步 强制不卸载文件数据
    /// 用于实时动态加载
    /// </summary>
    /// <param name="emBundleType">包类型</param>
    /// <param name="entLoadSuccess">加载完成事件(基本上用在实时动态加载回调)</param>
    /// <returns></returns>
    public bool LoadBundleFileDynamic(emAssetBundle emBundleType, EventBundleLoadSuccess entLoadSuccess = null)
    {
        return LoadBundleFile(emBundleType, true, false, entLoadSuccess);
    }

    public bool HaveBundleFile(emAssetBundle emAssetBundleType)
    {
        return dictAssetBundleInfo.ContainsKey(emAssetBundleType);
    }
    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="emBundleType">包类型</param>
    /// <param name="isDelFileData">是否释放文件数据</param>
    /// <returns></returns>
    private bool ReleaseResource(emAssetBundle emBundleType, bool isDelFileData, bool isUnloadAllChild = false)
    {
        List<emAssetBundle> ls_bundle = null;
        if (isUnloadAllChild)
            ls_bundle = AssetBundlePath.GetMappingName(emAssetBundle.Bundle_StateWar_Map_1_1);
        if (ls_bundle != null)
        {
            foreach (var embundle in ls_bundle)
            {
                if (dictAssetBundleInfo.ContainsKey(embundle))
                {
                    var bundle = dictAssetBundleInfo[embundle];

                    if (isDelFileData)
                    {
                        if (bundle.Unload(isDelFileData))
                        {
                            dictAssetBundleInfo.Remove(embundle);
                        }
                    }
                    else
                    {
                        bundle.Unload(isDelFileData);
                    }

                }
            }
            return true;
        }
        else
        {
            if (dictAssetBundleInfo.ContainsKey(emBundleType))
            {
                var bundle = dictAssetBundleInfo[emBundleType];

                if (isDelFileData)
                {
                    if (bundle.Unload(isDelFileData))
                    {
                        dictAssetBundleInfo.Remove(emBundleType);
                    }
                }
                else
                {
                    bundle.Unload(isDelFileData);
                }
                return true;
            }
        }
        return false;
    }

    public bool ReleaseResourceAsset(emAssetBundle emBundleType, bool isUnloadAsset)
    {
        if (dictAssetBundleInfo.ContainsKey(emBundleType))
        {
            var bundle = dictAssetBundleInfo[emBundleType];

            bundle.UnloadAssetBundle(isUnloadAsset);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 加载Bundle文件
    /// </summary>
    /// <param name="emBundleType">包类型</param>
    /// <param name="isAsync">是否异步</param>
    /// <param name="isUnloadImmediate">是否在加载完成后直接删除文件数据(在实时动态加载的时候选择false)</param>
    /// <param name="entLoadSuccess">加载完成事件(基本上用在实时动态加载回调)</param>
    /// <returns></returns>
    private bool LoadBundleFile(emAssetBundle emBundleType, bool isAsync = true, bool isUnloadImmediate = true, EventBundleLoadSuccess entLoadSuccess = null, bool isNeedPreHeat = false)
    {
        if (!AssetBundlePath.s_dictAssetBundle.ContainsKey(emBundleType))
        {
            Debug.LogError("The Bundle Path Invalid! BundleName:" + emBundleType.ToString());
            return false;
        }

        if (LoadBundleFileRapid(emBundleType))
        {
            return true;
        }

        try
        {
            BundleFileData bundleFileData = new BundleFileData(emBundleType, isUnloadImmediate, entLoadSuccess, isNeedPreHeat: isNeedPreHeat);
            if (bundleFileData.Load(isAsync))
            {
                dictAssetBundleInfo[emBundleType] = bundleFileData;
                loadingAsyncFlag = isAsync;
                return true;
            }
            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 加载Bundle文件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="emBundleType">包类型</param>
    /// <param name="isAsync">是否异步</param>
    /// <param name="isUnloadImmediate">是否在加载完成后直接删除文件数据(在实时动态加载的时候选择false)</param>
    /// <param name="entLoadSuccess">加载完成接口(基本上用在实时动态加载回调)</param>
    /// <returns></returns>
    private bool LoadBundleFile<T>(emAssetBundle emBundleType, bool isAsync = true, bool isUnloadImmediate = true, EventBundleLoadSuccess entLoadSuccess = null, bool isNeedPreHeat = false) where T : UnityEngine.Object
    {
        if (!AssetBundlePath.s_dictAssetBundle.ContainsKey(emBundleType))
        {
            Debug.LogError("The Bundle Path Invalid! BundleName:" + emBundleType.ToString());
            return false;
        }
        if (dictAssetBundleInfo.ContainsKey(emBundleType))
        {
            var bundle = dictAssetBundleInfo[emBundleType];
            if (bundle != null)
            {
                if (bundle.IsCanReload)
                {
                    bool tmp = false;
                    if (!isNeedPreHeat)
                        bundle.Reload();
                    if (tmp)
                    {
                        loadingAsyncFlag = isAsync;
                    }
                    return tmp;
                }
                if (bundle.IsDone)
                {
                    Debug.Log("The Bundle Existing! bundleName:" + emBundleType.ToString());
                    return true;
                }
            }
        }

        try
        {
            BundleFileData bundleFileData = new BundleFileData(emBundleType, isUnloadImmediate, entLoadSuccess, typeof(T), isNeedPreHeat);
            if (bundleFileData.Load(isAsync))
            {
                dictAssetBundleInfo[emBundleType] = bundleFileData;
                loadingAsyncFlag = isAsync;
                return true;
            }

            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
    }

    //private bool LoadBundleFileAsync(emAssetBundle emBundle)
    //{
    //    if (!AssetBundlePath.s_dictAssetBundl.ContainsKey(emBundle))
    //    {
    //        Debug.LogError("The Bundle Path Invalid! BundleName:" + emBundle.ToString());
    //        return false;
    //    }
    //    if (dictAssetResourcesObj.ContainsKey(emBundle))
    //    {
    //        Debug.Log("The Bundle Existing! bundleName:" + emBundle.ToString());
    //        return true;
    //    }

    //    if (!dictAssetBundleInfo.ContainsKey(emBundle))
    //    {
    //        try
    //        {
    //            var bundleAsync = GetAsyncBundleData(Application.persistentDataPath + "/" + emBundle.ToString().ToLower() + AssetBundlePath.s_fileSuffix);
    //            if (bundleAsync != null)
    //            {
    //                dictAssetBundleInfo[emBundle] = new BundleFileData(null, bundleAsync);
    //                loadingAsyncFlag = true;
    //                return true;
    //            }
    //        }
    //        catch (System.Exception ex)
    //        {
    //            Debug.LogError(ex.Message);
    //            return false;
    //        }
    //    }
    //    return false;
    //}

    //private bool LoadBundleFileAsync<T>(emAssetBundle emBundle) where T : UnityEngine.Object
    //{
    //    if (!AssetBundlePath.s_dictAssetBundl.ContainsKey(emBundle))
    //    {
    //        Debug.LogError("The Bundle Path Invalid! BundleName:" + emBundle.ToString());
    //        return false;
    //    }
    //    if (dictAssetResourcesObj.ContainsKey(emBundle))
    //    {
    //        Debug.Log("The Bundle Existing! bundleName:" + emBundle.ToString());
    //        return true;
    //    }

    //    if (!dictAssetBundleInfo.ContainsKey(emBundle))
    //    {
    //        try
    //        {
    //            var bundleAsync = GetAsyncBundleData(Application.persistentDataPath + "/" + emBundle.ToString().ToLower() + AssetBundlePath.s_fileSuffix);
    //            if (bundleAsync != null)
    //            {
    //                dictAssetBundleInfo[emBundle] = new BundleFileData(typeof(T), bundleAsync);
    //                loadingAsyncFlag = true;
    //                return true;
    //            }
    //        }
    //        catch (System.Exception ex)
    //        {
    //            Debug.LogError(ex.Message);
    //            return false;
    //        }
    //    }
    //    return false;
    //}

    //private AssetBundleCreateRequest GetAsyncBundleData(string path)
    //{
    //    AssetBundleCreateRequest bundleAsync = null;
    //    FileStream filedata = null;
    //    byte[] deData = null;
    //    //byte[] filedata = null;
    //    //MemoryStream deData = null;
    //    try
    //    {
    //        filedata = MyFile.ReadFileStream(path);
    //        deData = MyCrypto.BlowfishDecode(filedata);

    //        bundleAsync = AssetBundle.LoadFromMemoryAsync(deData);

    //        //filedata = MyFile.ReadFile(path);
    //        //deData = MyCrypto.BlowfishDecode(filedata);
    //        //bundleAsync = AssetBundle.LoadFromMemoryAsync(deData.GetBuffer());
    //        return bundleAsync;
    //    }
    //    catch (System.Exception ex)
    //    {
    //        Debug.LogError(ex.Message);
    //        return null;
    //    }
    //    finally
    //    {
    //        deData = null;
    //        filedata = null;
    //        ReleaseMemory();
    //    }
    //}

    #region Audio资源相关管理

    /// <summary>
    /// 根据传入的资源去读取单个资源，并以原名存储在对应包的包内资源字典里，（若包在字典中不存在则创建，以包名作为键将包存入包字典）
    /// </summary>
    /// <param name="AudioName">资源名(带包名如Player_Foot_walk_01)</param>
    private void LoadAudioToDictByName(string AudioName)
    {
        string[] AudioPath = AudioName.Split('_');
        string ClipName = AudioName.Replace(AudioPath[0] + "_", "");

        AudioClip newClip = (AudioClip)LoadAssetByPath(AudioSourcePath + AudioPath[0] + '/' + AudioName.Replace(AudioPath[0] + "_", ""));
        if (newClip != null)
        {
            if (AudioBundleDict.ContainsKey(AudioPath[0]))
            {
                AudioBundle = AudioBundleDict[AudioPath[0]];
                AudioBundle.Add(ClipName, newClip);
            }
            else
            {
                AudioBundle = new Dictionary<string, AudioClip>();
                AudioBundle.Add(ClipName, newClip);
                AudioBundleDict.Add(AudioPath[0], AudioBundle);
            }
            Debug.Log(AudioName + " Was Loaded");
        }
        else
        {
            Debug.Log("Named:" + AudioName + " AudioSource Not Find");
        }
    }

    /// <summary>
    /// 通过资源名字（前缀加上所属的包名）（如(Player_)Foot_walk_01）从字典中获取对应资源，如果不存在则加载
    /// </summary>
    /// <param name="AudioName">资源名字，格式为 所属包名_所属音源节点_声音动作名_编号（Player_Foot_walk_01）</param>
    /// <returns></returns>
    public AudioClip GetAudioClipByName(string AudioName)
    {
        string[] strAudioName = AudioName.Split('_');
        string ClipName = AudioName.Replace(strAudioName[0] + "_", "");
        if (AudioBundleDict.ContainsKey(strAudioName[0]))
        {
            AudioBundle = AudioBundleDict[strAudioName[0]];
        }
        else
        {
            AudioBundle = new Dictionary<string, AudioClip>();
            AudioBundleDict.Add(strAudioName[0], AudioBundle);
        }

        if (AudioBundle == null)
        {
            Debug.Log("No This Bundle");
            return null;
        }

        if (AudioBundle.ContainsKey(ClipName))
        {
            return AudioBundle[ClipName];
        }
        else
        {
            LoadAudioToDictByName(AudioName);
            if (AudioBundle.ContainsKey(ClipName))
            {
                return AudioBundle[ClipName];
            }
            else
            {
                Debug.Log("Dont Have This Source Under Bundle");
                return null;
            }
        }
    }
    /// <summary>
    /// 根据传入的包名获取对应包的资源字典
    /// </summary>
    /// <param name="AudioBundleName">包名</param>
    /// <returns>包内资源字典</returns>
    public Dictionary<string, AudioClip> GetAudioBundleByName(string AudioBundleName)
    {
        if (!AudioBundleDict.ContainsKey(AudioBundleName))
        {
            UnityEngine.Object[] audioClips = LoadAllAssetUnderPathForAssetBundle(AudioSourcePath + AudioBundleName);
            Dictionary<string, AudioClip> newAudioBuddle = new Dictionary<string, AudioClip>();

            if (audioClips == null)
            {
                Debug.Log("No This Buddle Found");
                return null;
            }

            foreach (var audioClip in audioClips)
            {
                newAudioBuddle.Add(audioClip.name, (AudioClip)audioClip);
            }
            AudioBundleDict.Add(AudioBundleName, newAudioBuddle);
        }

        return AudioBundleDict[AudioBundleName];
    }

    #endregion

    #region 角色相关prefab资源管理

    //TODO临时使用待修改
    private void LoadPrefabToDictByName(string actorName)
    {
        GameObject newPrefab = ResourceManager.Instance.GetResource(emAssetBundle.Bundle_FightMapPrefabs, actorName) as GameObject;
        ActorBundle.Add(actorName, newPrefab);
    }

    //TODO临时使用待修改
    public GameObject GetActorPrefabByName(string actorName)
    {
        if (!ActorBundle.ContainsKey(actorName))
        {
            LoadPrefabToDictByName(actorName);
        }
        return ActorBundle[actorName];
    }

    #endregion
    private UnityEngine.Object LoadAssetByPath(string AssetPath)
    {
        UnityEngine.Object ReturnAsset = Resources.Load(AssetPath);
        if (ReturnAsset == null)
        {
            Debug.Log("No Asset Find This Path: " + AssetPath);
        }
        return ReturnAsset;
    }

    /// <summary>
    /// 根据传入的地址取该文件夹下的所有资源
    /// </summary>
    /// <param name="AssetPath">所需资源所在文件夹</param>
    /// <param name="type">资源类型</param>
    /// <returns>返回的所有资源</returns>
    private UnityEngine.Object[] LoadAllAssetUnderPath(string AssetPath)
    {
        UnityEngine.Object[] ReturnAsset = Resources.LoadAll(AssetPath);
        if (ReturnAsset == null)
        {
            Debug.Log("No Asset Uder " + AssetPath);
        }
        return ReturnAsset;
    }

    /// <summary>
    /// 根据传入的地址取该包
    /// </summary>
    /// <param name="AssetPath">所需资源的包名，去掉.bundle即所在文件夹位置和名字，如打包前为Player文件夹，则取Player文件对应Resource的位置</param>
    /// <returns>返回对象</returns>
    private UnityEngine.Object[] LoadAllAssetUnderPathForAssetBundle(string AssetPath)
    {
        UnityEngine.Object[] ReturnObjs;
        AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/assets" + AssetPath.ToLower() + ".assetbundle");
        //AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.dataPath + "/StreamingAssets/assets" + AssetPath.ToLower() + ".assetbundle");
        if (assetBundle != null)
        {
            ReturnObjs = assetBundle.LoadAllAssets();
            assetBundle.Unload(false);
        }
        else
        {
            //Debug.Log("No this Path :" + Application.streamingAssetsPath + AssetPath.ToLower() + ".assetbundle");
            Debug.LogWarning("No this Path :" + Application.dataPath + "!/assets" + AssetPath.ToLower() + ".assetbundle");
            return null;
        }

        if (ReturnObjs != null)
        {
            return ReturnObjs;
        }
        else
        {
            Debug.LogWarning("No Objs In Bundle");
            return null;
        }
    }

    ///// <summary>
    ///// 根据传入的地址取该文件夹（在Asset/下）的所有资源
    ///// </summary>
    ///// <returns></returns>
    //private UnityEngine.Object[] LoadAllAssetUnderAssetPath(string AssetPath)
    //{
    //    UnityEngine.Object[] ReturnAsset = AssetDatabase.LoadAssetAtPath
    //    if (ReturnAsset == null)
    //    {
    //        Debug.Log("No Asset Uder " + AssetPath);
    //    }
    //    return ReturnAsset;
    //}

    private IEnumerator AsyncLoadCoroutine(string path, System.Type type)
    {
        WWW bundle = new WWW(path);
        yield return null;
    }


    /// <summary>
    /// 传入prefab在对应的对象下生成
    /// </summary>
    /// <param name="prefab">传入的prefab</param>
    /// <param name="parent">生成实例的父节点</param>
    /// <returns>返回生成的实例</returns>
    public GameObject CreateGoUnderParent(GameObject prefab, Transform parent)
    {
        if (prefab == null)
            return null;
        GameObject newGo = GameObject.Instantiate(prefab);
        newGo.transform.SetParent(parent, false);
        return newGo;
    }

    #region Icon相关资源
    /// <summary>
    /// 根据传入的Folder名获取Icon集合
    /// </summary>
    /// <param name="name">Folder名</param>
    /// <returns></returns>
    public Dictionary<string, Sprite> GetIconByName(string name)
    {
        if (IconDict.ContainsKey(name))
        {
            return IconDict[name];
        }
        else
        {
            LoadIconToDictByName(name);
            if (IconDict.ContainsKey(name))
                return IconDict[name];
        }
        return null;
    }

    /// <summary>
    /// 根据传入的参数取sprite
    /// </summary>
    /// <param name="FolderName">文件夹名字</param>
    /// <param name="name">文件名</param>
    /// <returns></returns>
    public Sprite GetIconByName(string FolderName, string name)
    {
        if (IconDict.ContainsKey(FolderName))
        {
            if (IconDict[FolderName].ContainsKey(name))
                return IconDict[FolderName][name];
        }
        else
        {
            LoadIconToDictByName(FolderName);
            if (IconDict[FolderName].ContainsKey(name))
                return IconDict[FolderName][name];
        }
        return null;
    }

    private void LoadIconToDictByName(string IconTypeName)
    {
        //UnityEngine.Object[] Icons = LoadAllAssetUnderPath(IconSourcePath + IconTypeName);
        UnityEngine.Object[] Icons = LoadAllAssetUnderPathForAssetBundle(IconAssetSourcePath + IconTypeName);
        Dictionary<string, Sprite> newDict = new Dictionary<string, Sprite>();
        foreach (UnityEngine.Object Icon in Icons)
        {
            Sprite pic = Icon as Sprite;
            if (newDict.ContainsKey(Icon.name))
            {
                newDict[Icon.name] = pic;
                continue;
            }
            newDict.Add(Icon.name, pic);
        }
        IconDict.Add(IconTypeName, newDict);
    }

    //TODO 准备废弃
    private void LoadIconToDictByNameOld(string IconTypeName)
    {
        UnityEngine.Object[] Icons = LoadAllAssetUnderPath(IconSourcePath + IconTypeName);
        //UnityEngine.Object[] Icons = LoadAllAssetUnderPathForAssetBundle(IconAssetSourcePath + IconTypeName);
        Dictionary<string, Sprite> newDict = new Dictionary<string, Sprite>();
        foreach (UnityEngine.Object Icon in Icons)
        {
            Sprite pic = Icon as Sprite;
            if (newDict.ContainsKey(Icon.name))
            {
                newDict[Icon.name] = pic;
                continue;
            }
            newDict.Add(Icon.name, pic);
        }
        IconDict.Add(IconTypeName, newDict);
    }

    public Dictionary<string, Sprite> GetSpriteDict(string FolderName)
    {
        if (IconDict.ContainsKey(FolderName))
        {
            return IconDict[FolderName];
        }
        else
        {
            return null;
        }
    }
    #endregion

    #region 特效相关资源
    private void LoadEffectToDictByName(string EffectTypeName)
    {
        //UnityEngine.Object[] Icons = LoadAllAssetUnderPath(IconSourcePath + IconTypeName);
        UnityEngine.Object[] Effects = LoadAllAssetUnderPathForAssetBundle(EffectBundleSourcePath + EffectTypeName);
        Dictionary<string, GameObject> newDict = new Dictionary<string, GameObject>();
        foreach (UnityEngine.Object Effect in Effects)
        {
            GameObject effectPre = Effect as GameObject;
            if (newDict.ContainsKey(effectPre.name))
            {
                newDict[effectPre.name] = effectPre;
                continue;
            }
            newDict.Add(effectPre.name, effectPre);
        }
        EffectDict.Add(EffectTypeName, newDict);
    }


    public GameObject GetEffectByName(string FolderName, string Name)
    {
        return GetResource<GameObject>(emAssetBundle.Bundle_SkillEffect, Name);

        //if (EffectDict.ContainsKey(FolderName))
        //{
        //    if (EffectDict[FolderName].ContainsKey(Name))
        //        return EffectDict[FolderName][Name];
        //}
        //else
        //{
        //    LoadEffectToDictByName(FolderName);
        //    if (EffectDict[FolderName].ContainsKey(Name))
        //        return EffectDict[FolderName][Name];
        //}
        //return null;
    }

    #endregion

    #region 2D地图相关资源

    private Dictionary<string, List<List<Sprite>>> MapDict = new Dictionary<string, List<List<Sprite>>>();
    /// <summary>
    /// 读取地图片，地图名称为xxx_y_x
    /// </summary>
    /// <param name="PieceFolderName"></param>
    private void LoadMapToDictByName(string PieceFolderName)
    {
        UnityEngine.Object[] Maps = LoadAllAssetUnderPathForAssetBundle(TwoDMapBundleSourcePath + PieceFolderName);
        List<List<Sprite>> PieceList = new List<List<Sprite>>();
        foreach (UnityEngine.Object Map in Maps)
        {
            if (Map.GetType() == typeof(Sprite))
            {
                Sprite map = Map as Sprite;
                string s_y = map.name.Split('_')[1];
                int y = 0;
                int.TryParse(s_y, out y);
                string s_x = map.name.Split('_')[2];
                int x = 0;
                int.TryParse(s_x, out x);
                if (x > 0)
                {
                    while (PieceList.Count < x)
                    {
                        if (PieceList.Count > 200)
                        {
                            Debug.LogError("----To Big List------");
                            break;
                        }
                        PieceList.Add(new List<Sprite>());
                    }

                    while (PieceList[x - 1].Count < y)
                    {
                        if (PieceList[x - 1].Count > 200)
                        {
                            Debug.LogError("----To Big List------");
                            break;
                        }
                        PieceList[x - 1].Add(null);
                    }
                    PieceList[x - 1][y - 1] = map;
                }
            }
        }
        MapDict.Add(PieceFolderName, PieceList);
    }

    public Sprite GetMapPieceByPos(string PieceFolderName, int x, int y)
    {

        Sprite rtSp = GetResource<Sprite>(emAssetBundle.Bundle_StateWar_Map_1_1, global_ConstDataDefine.IconPath.StateWarMapPrefix + y + "_" + x);
        //Sprite rtSp = null;

        return rtSp;
        //if (MapDict.ContainsKey(PieceFolderName))
        //{
        //    if (MapDict[PieceFolderName].Count > x - 1)
        //        if (MapDict[PieceFolderName][x - 1].Count > y - 1)
        //            return MapDict[PieceFolderName][x - 1][y - 1];
        //}
        //else
        //{
        //    LoadMapToDictByName(PieceFolderName);
        //    if (MapDict.ContainsKey(PieceFolderName))
        //        if (MapDict[PieceFolderName].Count > x - 1)
        //            if (MapDict[PieceFolderName][x - 1].Count > y - 1)
        //                return MapDict[PieceFolderName][x - 1][y - 1];
        //}
        //return null;
    }

    #endregion

    #region 其余Prefab
    private void LoadTestPrefabToDictByName(string PrefabFolderName)
    {
        UnityEngine.Object[] Prefabs = LoadAllAssetUnderPath(FolderForTest + PrefabFolderName);
        //UnityEngine.Object[] Prefabs = LoadAllAssetUnderPathForAssetBundle(PrefabBundleSourcePath + PrefabFolderName);
        Dictionary<string, GameObject> newDict = new Dictionary<string, GameObject>();
        foreach (UnityEngine.Object Prefab in Prefabs)
        {
            if (Prefab.GetType() != typeof(GameObject))
                continue;
            GameObject pre = Prefab as GameObject;
            if (newDict.ContainsKey(pre.name))
            {
                newDict[pre.name] = pre;
                continue;
            }
            newDict.Add(pre.name, pre);
        }
        PrefabDict.Add(PrefabFolderName, newDict);
    }

    public GameObject GetTestPrefabByName(string FolderName, string Name)
    {
        if (PrefabDict.ContainsKey(FolderName))
        {
            if (PrefabDict[FolderName].ContainsKey(Name))
                return PrefabDict[FolderName][Name];
        }
        else
        {
            LoadTestPrefabToDictByName(FolderName);
            if (PrefabDict[FolderName].ContainsKey(Name))
                return PrefabDict[FolderName][Name];
        }
        return null;
    }


    #endregion

    #region 依赖加载测试代码
    public class ToLoadingAssetBundelTreeNode
    {
        public List<ToLoadingAssetBundelTreeNode> DependThisToLoadingAssetBundelTreeNode;
        public emAssetBundle assetType;
        public List<ToLoadingAssetBundelTreeNode> ChildToLoadingAssetBundelTreeNode;
        public bool isEndLoad = false;
        public bool isAllChildEndLoad
        {
            get
            {
                if (ChildToLoadingAssetBundelTreeNode == null)
                    return true;
                bool isEnd = true;
                foreach (var i in ChildToLoadingAssetBundelTreeNode)
                {
                    if (i.assetType == assetType)
                        continue;
                    if (!i.isEndLoad)
                    {
                        isEnd = false;
                    }
                }
                return isEnd;
            }
        }
        public ToLoadingAssetBundelTreeNode(emAssetBundle bundle)
        {
            assetType = bundle;
            DependThisToLoadingAssetBundelTreeNode = new List<ToLoadingAssetBundelTreeNode>();
            ChildToLoadingAssetBundelTreeNode = new List<ToLoadingAssetBundelTreeNode>();
            isEndLoad = false;
        }
    }

    //public List<ToLoadingAssetBundelTreeNode> endLeaf = new List<ToLoadingAssetBundelTreeNode>();

    private Dictionary<emAssetBundle, ToLoadingAssetBundelTreeNode> WaitingLoadAssets = new Dictionary<emAssetBundle, ToLoadingAssetBundelTreeNode>();

    List<ToLoadingAssetBundelTreeNode> NextToLoadAssets = new List<ToLoadingAssetBundelTreeNode>();

    public Dictionary<emAssetBundle, Dependence> TempDependDict = new Dictionary<emAssetBundle, Dependence>()
    {
        {emAssetBundle.Bundle_UIPrefab,new Dependence(emAssetBundle.Bundle_UIPrefab,new List<emAssetBundle>(){emAssetBundle.Bundle_Fonts, emAssetBundle.Bundle_UISp_common,emAssetBundle.Bundle_UISp_DrawCard,emAssetBundle.Bundle_UISp_Fighting,emAssetBundle.Bundle_UISp_StateWar,emAssetBundle.Bundle_UISp_LoginAndMian, emAssetBundle.Bundle_Fonts,emAssetBundle.Bundle_TalkChannelIcon }) },
        {emAssetBundle.Bundle_SoldierPrefabs,new Dependence(emAssetBundle.Bundle_SoldierPrefabs,new List<emAssetBundle>(){emAssetBundle.Bundle_Models }) }
    };

    public struct Dependence
    {
        public emAssetBundle Name;
        public List<emAssetBundle> DependenceList;
        public Dependence(emAssetBundle name, List<emAssetBundle> ls_AssetBundle)
        {
            Name = name;
            DependenceList = ls_AssetBundle;
        }
    }

    /// <summary>
    /// 等待加载Bundle文件
    /// </summary>
    /// <param name="emBundleType">包类型</param>
    /// <param name="isAsync">是否异步</param>
    /// <param name="isUnloadImmediate">是否在加载完成后直接删除文件数据(在实时动态加载的时候选择false)</param>
    /// <param name="entLoadSuccess">加载完成事件(基本上用在实时动态加载回调)</param>
    /// <returns></returns>

    private BundleFileData LoadBundleFileByDependence<T>(emAssetBundle emBundleType, bool isAsync = true, bool isUnloadImmediate = true, EventBundleLoadSuccess entLoadSuccess = null) where T : UnityEngine.Object
    {
        if (!AssetBundlePath.s_dictAssetBundle.ContainsKey(emBundleType))
        {
            Debug.LogError("The Bundle Path Invalid! BundleName:" + emBundleType.ToString());
            return null;
        }
        if (dictAssetBundleInfo.ContainsKey(emBundleType))
        {
            var bundle = dictAssetBundleInfo[emBundleType];
            if (bundle != null)
            {
                if (bundle.IsCanReload)
                {
                    bool tmp = bundle.Reload();
                    if (tmp)
                    {
                        loadingAsyncFlag = isAsync;
                    }
                    return bundle;
                }
                if (bundle.IsDone)
                {
                    Debug.Log("The Bundle Existing! bundleName:" + emBundleType.ToString());
                    return bundle;
                }
            }
        }

        try
        {
            BundleFileData bundleFileData = new BundleFileData(emBundleType, isUnloadImmediate, entLoadSuccess, typeof(T));
            if (bundleFileData.Load(isAsync))
            {
                dictAssetBundleInfo[emBundleType] = bundleFileData;
                loadingAsyncFlag = isAsync;
                return bundleFileData;
            }

            return null;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    public void BuildAndStartLoadList(List<emAssetBundle> ToLoadAssets, Dictionary<emAssetBundle, Dependence> dependenceList)
    {
        WaitingLoadAssets.Clear();
        NextToLoadAssets.Clear();

        foreach (var kp in ToLoadAssets)
        {
            //kp.Value.endLoadEvent += EndLoadEvent;
            if (!WaitingLoadAssets.ContainsKey(kp))
                WaitingLoadAssets.Add(kp, new ToLoadingAssetBundelTreeNode(kp));

            if (dependenceList.ContainsKey(kp))
            {
                if (dependenceList[kp].DependenceList != null)
                    if (dependenceList[kp].DependenceList.Count > 0)
                    {
                        foreach (var i in dependenceList[kp].DependenceList)
                        {
                            if (!ToLoadAssets.Contains(i))
                            {
                                //可以选择部分的依赖进行加载
                                continue;
                                //Debug.LogWarning("Dependence " + i.ToString() + " Not In To Load List");
                            }
                            if (!WaitingLoadAssets.ContainsKey(i))
                            {
                                WaitingLoadAssets.Add(i, new ToLoadingAssetBundelTreeNode(i));
                            }

                            WaitingLoadAssets[kp].ChildToLoadingAssetBundelTreeNode.Add(WaitingLoadAssets[i]);
                            WaitingLoadAssets[i].DependThisToLoadingAssetBundelTreeNode.Add(WaitingLoadAssets[kp]);
                        }
                    }
            }
        }

        foreach (var i in WaitingLoadAssets)
        {
            if (i.Value.isAllChildEndLoad)
            {
                NextToLoadAssets.Add(i.Value);
            }
            if (i.Value.DependThisToLoadingAssetBundelTreeNode != null && i.Value.DependThisToLoadingAssetBundelTreeNode.Count > 0)
            {
                LoadBundleFile(i.Value.assetType, true, false, EndLoadEvent);
            }
            else
            {
                LoadBundleFile(i.Value.assetType, true, true, EndLoadEvent);

            }

            if (dictAssetBundleInfo.ContainsKey(i.Value.assetType))
                dictAssetBundleInfo[i.Value.assetType].IsWaiting = true;
        }

    }


    public void EndLoadEvent(emAssetBundle asset)
    {

        if (WaitingLoadAssets.ContainsKey(asset))
        {
            ToLoadingAssetBundelTreeNode node = WaitingLoadAssets[asset];
            if (node.isEndLoad == true)
                return;
            node.isEndLoad = true;
            if (NextToLoadAssets.Contains(node))
                NextToLoadAssets.Remove(node);
            if (node.DependThisToLoadingAssetBundelTreeNode != null)
            {
                if (node.DependThisToLoadingAssetBundelTreeNode.Count > 0)
                {
                    foreach (var i in node.DependThisToLoadingAssetBundelTreeNode)
                    {
                        //i.ChildToLoadingAssetBundelTreeNode.Remove(node);
                        if (i.isAllChildEndLoad)
                        {
                            if (!NextToLoadAssets.Contains(i) && i.isEndLoad == false)
                            {
                                NextToLoadAssets.Add(i);
                            }
                        }

                    }
                }
            }
        }
    }


    #endregion

    #region BundleFileData(Class)
    public delegate void EventBundleLoadSuccess(emAssetBundle emBundleType);

    private class BundleFileData : IGetProgress
    {
        private System.DateTime lastTime;
        private emAssetBundle emBundleType;

        public emAssetBundle EmBundleType
        {
            get
            {
                return emBundleType;
            }
        }

        private byte[] fileDedata = null;
        private bool isDone = false;
        private System.Type loadType = null;
        private AssetBundleCreateRequest createRequest = null;
        private AssetBundleRequest request = null;
        private AssetBundle assetBundle = null;
        private Dictionary<string, UnityEngine.Object> dictObjs = null;
        private bool isUnloadImmediate = false;
        private bool isAsync = false;
        private bool isNeedPreHeat = false;

        private bool isWaitForDependence = false;

        public event EventBundleLoadSuccess entLoadSuccess;

        public bool IsDone
        {
            get
            {
                return isDone;
            }
        }
        public bool IsCanReload
        {
            get
            {
                return isDone && dictObjs == null && IsSaveFileData;
            }
        }
        public bool IsSaveFileData
        {
            get
            {
                return createRequest != null && createRequest.assetBundle != null || assetBundle != null;
            }
        }

        public bool IsAsync
        {
            get
            {
                return isAsync;
            }
        }

        public bool IsWaiting
        {
            get
            {
                return isWaitForDependence;
            }
            set
            {
                isWaitForDependence = value;
            }
        }

        /// <summary>
        /// Bundle文件读取类
        /// </summary>
        /// <param name="emBundleType">类型</param>
        /// <param name="entLoadSuccess">加载完成接口(基本上用在实时动态加载回调)</param>
        /// <param name="isUnloadAssetImmediate">是否在加载完成后直接删除文件数据(在实时动态加载的时候选择false)</param>
        /// <param name="loadType">只读取包内此类型文件</param>
        public BundleFileData(emAssetBundle emBundleType, bool isUnloadAssetImmediate = true, EventBundleLoadSuccess entLoadSuccess = null, System.Type loadType = null, bool isNeedPreHeat = false)
        {
            this.emBundleType = emBundleType;
            this.loadType = loadType;
            this.isUnloadImmediate = isUnloadAssetImmediate;
            this.isNeedPreHeat = isNeedPreHeat;
            if (entLoadSuccess != null)
            {
                this.entLoadSuccess = entLoadSuccess;
            }
        }

        ~BundleFileData()
        {
            if (createRequest != null)
            {
                throw new System.Exception("This package not Unload will in destroyed! BundleName:" + emBundleType.ToString());
            }
        }

        public void OnUpdate()
        {
            if (isDone || dictObjs != null)
                return;

            if (IsWaiting)
                return;

            if (request != null)
            {
                try
                {
                    if (request.isDone)
                    {
                        if (!isNeedPreHeat)
                        {
                            dictObjs = InsertObjsToDict(request.allAssets);
                        }
                        else
                        {
                            dictObjs = new Dictionary<string, UnityEngine.Object>();
                        }

                        request = null;
                        if (isUnloadImmediate)
                        {
                            createRequest.assetBundle.Unload(false);
                        }

                        isDone = true;
                        Debug.Log("==============AssetBundle Async Load Success! UseTime: " + (System.DateTime.Now - lastTime) + " BundleName:" + emBundleType.ToString() + "==============");
                        //ReleaseMemory();
                        if (entLoadSuccess != null)
                        {
                            entLoadSuccess(emBundleType);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                    if (createRequest != null && createRequest.assetBundle != null)
                    {
                        createRequest.assetBundle.Unload(true);
                    }
                    throw ex;
                }
            }
            else
            {
                try
                {
                    if (createRequest != null && createRequest.isDone)
                    {
                        if (loadType == null)
                        {
                            request = createRequest.assetBundle.LoadAllAssetsAsync();
                        }
                        else
                        {
                            request = createRequest.assetBundle.LoadAllAssetsAsync(loadType);
                        }
                    }
                    else if (createRequest == null && fileDedata != null)
                    {
                        createRequest = AssetBundle.LoadFromMemoryAsync(fileDedata);
                        fileDedata = null;
                        ReleaseMemory();
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                    if (createRequest != null && createRequest.assetBundle != null)
                    {
                        createRequest.assetBundle.Unload(true);
                    }
                    throw ex;
                }
                finally
                {
                    if (fileDedata != null)
                    {
                        fileDedata = null;
                        ReleaseMemory();
                    }
                }
            }

        }

        public UnityEngine.Object GetResourceObj(string objName)
        {
            if (objName == null)
            {
                Debug.LogWarning("objName is Null!");
                return null;
            }

            if (!isDone)
            {
                Debug.LogWarning("Load the unfinished! BundleName: " + emBundleType.ToString());
                return null;
            }

            if (dictObjs != null)
            {
                if (isNeedPreHeat)
                {
                    PreHeatLoad(objName);
                }
                if (dictObjs.ContainsKey(objName))
                {
                    return dictObjs[objName];
                }
            }
            else
                Debug.LogError("dictObjs is null! BundleName: " + emBundleType.ToString());

            return null;
        }

        public T GetResourceObj<T>(string objName) where T : UnityEngine.Object
        {
            if (objName == null)
            {
                Debug.LogWarning("objName is Null!");
                return null;
            }
            if (!isDone)
            {
                Debug.LogWarning("Load the unfinished! BundleName: " + emBundleType.ToString());
                return null;
            }

            if (dictObjs != null)
            {
                if (isNeedPreHeat)
                {
                    PreHeatLoad<T>(objName);
                }
                if (dictObjs.ContainsKey(objName))
                {
                    return dictObjs[objName] as T;
                }
            }
            else
                Debug.LogError("dictObjs is null! BundleName: " + emBundleType.ToString());

            return null;
        }

        public IEnumerator GetAllResource()
        {
            if (!isDone)
            {
                Debug.LogWarning("Load the unfinished! BundleName: " + emBundleType.ToString());
                return null;
            }
            if (dictObjs == null)
            {
                Debug.LogError("dictObjs is null! BundleName: " + emBundleType.ToString());
                return null;
            }

            return dictObjs.GetEnumerator();
        }

        public int GetResourceLength()
        {

            if (dictObjs != null)
            {
                return dictObjs.Count;
            }
            return 0;
        }
        public bool Load(bool isAsync)
        {
            if (dictObjs != null || createRequest != null || assetBundle != null)
            {
                return false;
            }


            lastTime = System.DateTime.Now;
            if (isAsync)
            {
                isDone = false;
                ThreadPool.QueueUserWorkItem(OnThreadDecode, emBundleType);
            }
            else
            {
                try
                {
                    var filedata = GetFileDedata(strBundleFilePath + emBundleType.ToString().ToLower() + AssetBundlePath.s_fileSuffix);
                    assetBundle = AssetBundle.LoadFromMemory(filedata);

                    filedata = null;

                    if (!isNeedPreHeat)
                    {
                        dictObjs = InsertObjsToDict(assetBundle.LoadAllAssets());
                    }
                    else
                    {
                        dictObjs = new Dictionary<string, UnityEngine.Object>();
                        Resources.UnloadUnusedAssets();
                    }

                    if (isUnloadImmediate)
                    {
                        assetBundle.Unload(false);
                    }

                    isDone = true;
                    Debug.Log("==============AssetBundle Load Success! UseTime: " + (System.DateTime.Now - lastTime) + " BundleName:" + emBundleType.ToString() + "==============");
                    if (entLoadSuccess != null)
                    {
                        entLoadSuccess(emBundleType);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                    if (assetBundle != null)
                    {
                        assetBundle.Unload(true);
                    }
                    return false;
                }
                finally
                {
                    ReleaseMemory();
                }
            }
            this.isAsync = isAsync;
            return true;

        }

        public bool Load(byte[] filedata, bool isAsync)
        {
            if (dictObjs != null || createRequest != null || assetBundle != null || filedata == null)
            {
                return false;
            }

            try
            {
                lastTime = System.DateTime.Now;
                if (isAsync)
                {
                    isDone = false;
                    createRequest = AssetBundle.LoadFromMemoryAsync(filedata);
                }
                else
                {
                    assetBundle = AssetBundle.LoadFromMemory(filedata);

                    if (!isNeedPreHeat)
                    {
                        dictObjs = InsertObjsToDict(assetBundle.LoadAllAssets());
                    }
                    else
                    {
                        dictObjs = new Dictionary<string, UnityEngine.Object>();
                        Resources.UnloadUnusedAssets();
                    }

                    if (isUnloadImmediate)
                    {
                        assetBundle.Unload(false);
                    }
                    isDone = true;
                    Debug.Log("==============AssetBundle Load Success! UseTime: " + (System.DateTime.Now - lastTime) + " BundleName:" + emBundleType.ToString() + "==============");
                    if (entLoadSuccess != null)
                    {
                        entLoadSuccess(emBundleType);
                    }
                }
                this.isAsync = isAsync;
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 快速预热 是否异步为第一次Load时的选择
        /// </summary>
        /// <returns></returns>
        public bool Reload()
        {
            if (!IsCanReload)
            {
                return false;
            }

            if (createRequest != null && createRequest.assetBundle != null)
            {
                lastTime = System.DateTime.Now;
                isDone = false;
                return true;
            }

            if (assetBundle != null && !IsAsync)
            {
                lastTime = System.DateTime.Now;
                dictObjs = InsertObjsToDict(assetBundle.LoadAllAssets());
                isDone = true;
                Debug.Log("==============AssetBundle Load Success! UseTime: " + (System.DateTime.Now - lastTime) + " BundleName:" + emBundleType.ToString() + "==============");
                if (entLoadSuccess != null)
                {
                    entLoadSuccess(emBundleType);
                }
                return true;
            }
            else if (request != null && IsAsync)
            {
                lastTime = System.DateTime.Now;
                dictObjs = InsertObjsToDict(request.allAssets);
                isDone = true;
                Debug.Log("==============AssetBundle Load Success! UseTime: " + (System.DateTime.Now - lastTime) + " BundleName:" + emBundleType.ToString() + "==============");
                if (entLoadSuccess != null)
                {
                    entLoadSuccess(emBundleType);
                }
                return true;
            }
            return false;
        }

        /// 文件内存保存与否
        /// asset的内存实例保存与否
        /// 文件是否通过引用预热（当预热则必定有内存实例，有内存实例则直接存入已预热字典
        /// 有文件内存无实例-> 读取 预热到dict->asset.load,asset.loadAll dict.add
        /// 有文件内存有实例-> 读取 从dict中读取->dict[]
        /// 释放文件内存保留实例 -> 加快读取,占用略高->dict[]
        /// 释放文件内存生成gameobject后释放实例 -> 只用于prefab ->占用最少,无法再次读入->dict[]
        /// 有文件内存无实例-> 释放 asset.unload(false/true)
        /// 有文件内存有实例-> 释放所有 dict.clear() asset.unload(true) 释放文件保存实例 asset.unload(false)

        public bool PreHeatLoad(string name)
        {
            if (dictObjs.ContainsKey(name))
            {
                return true;
            }
            else
            {
                UnityEngine.Object loadObj = null;
                if (isAsync)
                {
                    if (createRequest != null && createRequest.assetBundle != null)
                    {
                        loadObj = createRequest.assetBundle.LoadAsset(name);
                    }
                }
                else
                {
                    if (assetBundle != null)
                    {
                        loadObj = assetBundle.LoadAsset(name);
                    }
                }

                if (loadObj != null)
                {
                    dictObjs.Add(name, loadObj);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool PreHeatLoad<T>(string name) where T : UnityEngine.Object
        {
            if (dictObjs.ContainsKey(name))
            {
                return true;
            }
            else
            {
                UnityEngine.Object loadObj = null;
                if (isAsync)
                {
                    if (createRequest != null && createRequest.assetBundle != null)
                    {
                        loadObj = createRequest.assetBundle.LoadAsset<T>(name);
                    }
                }
                else
                {
                    if (assetBundle != null)
                    {
                        loadObj = assetBundle.LoadAsset<T>(name);
                    }
                }

                if (loadObj != null)
                {
                    dictObjs.Add(name, loadObj);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 释放单实例内存
        /// </summary>
        /// <param name="name">单文件的文件名</param>
        public void UnloadAssetObj(string name)
        {
            if (dictObjs.ContainsKey(name))
            {
                Resources.UnloadAsset(dictObjs[name]);
                dictObjs[name] = null;
            }
        }

        /// <summary>
        /// 释放全部实例内存资源
        /// </summary>
        /// <param name="isDelFileData">是否释放文件数据</param>
        /// <returns>数据是否全部销毁</returns>
        public bool Unload(bool isDelFileData)
        {
            if (createRequest != null && isDelFileData)
            {
                if (createRequest.assetBundle != null)
                {
                    createRequest.assetBundle.Unload(true);
                    createRequest = null;
                }
            }

            if (assetBundle != null && isDelFileData)
            {
                assetBundle.Unload(true);
                assetBundle = null;
            }

            request = null;

            if (dictObjs != null)
            {
                foreach (var obj in dictObjs.Values)
                {
                    if (obj != null)
                    {
                        if (obj.GetType() != typeof(GameObject))
                        {
                            Resources.UnloadAsset(obj);
                        }
                        else
                        {
                            GameObject.DestroyImmediate(obj, true);
                        }
                    }

                }
                dictObjs.Clear();
                dictObjs = null;
            }

            isDone = !isDelFileData;
            Resources.UnloadUnusedAssets();
            ReleaseMemory();
            return isUnloadImmediate || isDelFileData;
        }

        /// <summary>
        /// 释放包
        /// </summary>
        /// <param name="isUnloadObj">是否销毁obj</param>
        public void UnloadAssetBundle(bool isUnloadObj)
        {
            if (createRequest != null)
            {
                if (createRequest.assetBundle != null)
                {
                    createRequest.assetBundle.Unload(isUnloadObj);
                    createRequest = null;
                }
            }

            if (assetBundle != null)
            {
                assetBundle.Unload(isUnloadObj);
                assetBundle = null;
            }

            if (isUnloadObj)
            {
                dictObjs.Clear();
            }

            isDone = false;
            Resources.UnloadUnusedAssets();
            ReleaseMemory();
        }


        float IGetProgress.GetProgress()
        {
            if (isDone)
            {
                return 1f;
            }
            float val = 0f;
            if (createRequest != null)
            {
                val += createRequest.progress * 0.5f;
                if (request != null)
                {
                    val += request.progress * 0.5f;
                }
            }

            return val;
        }
        private byte[] GetFileDedata(string path, bool isStream = false)
        {
            if (isStream)
            {
                FileStream filedata = null;
                byte[] deData = null;
                try
                {
                    filedata = MyFile.GetFileStream(path);
                    deData = MyCrypto.BlowfishDecode(filedata);

                    return deData;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                    return null;
                }
                finally
                {
                    if (filedata != null)
                    {
                        filedata.Close();
                        filedata.Dispose();
                        filedata = null;
                    }
                    ReleaseMemory();
                }
            }
            else
            {
                //byte[] filedata = null;
                MemoryStream deData = null;
                try
                {
                    byte[] buffer = new byte[global_ConstDataDefine.DecodeEncodeBlockSize];
                    FileStream fs = null;

                    System.DateTime lastTime = System.DateTime.Now;
                    try
                    {
                        fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);

                        Debug.Log("decode : ___" + fs.Name + "____________");
                        byte[] rtBuff = new byte[fs.Length];
                        deData = new System.IO.MemoryStream(rtBuff);
                        //deData.Flush();
                        long leftLength = fs.Length;
                        for (int i = 0; i <= fs.Length / global_ConstDataDefine.DecodeEncodeBlockSize; i++)
                        {

                            if (i != fs.Length / global_ConstDataDefine.DecodeEncodeBlockSize)
                            {
                                fs.Position = i * global_ConstDataDefine.DecodeEncodeBlockSize;
                                fs.Read(buffer, 0, buffer.Length);
                                MyCrypto.BlowfishDecode_alpha(deData, buffer, false);
                            }
                            else
                            {
                                if (fs.Length > i * global_ConstDataDefine.DecodeEncodeBlockSize)
                                {
                                    fs.Position = i * global_ConstDataDefine.DecodeEncodeBlockSize;
                                    buffer = new byte[fs.Length - i * global_ConstDataDefine.DecodeEncodeBlockSize];
                                    fs.Read(buffer, 0, buffer.Length);
                                    MyCrypto.BlowfishDecode_alpha(deData, buffer, true);
                                }
                            }
                        }

                        Debug.Log("Read File Byte[] Sucess! UseTime:" + (System.DateTime.Now - lastTime) + " path:" + path);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("decode error " + fs.Name);
                        Debug.LogError(ex.Message);
                        return null;
                    }
                    finally
                    {
                        buffer = null;
                        if (fs != null)
                        {
                            fs.Close();
                        }
                    }
                    ///

                    ///原代码
                    //filedata = MyFile.ReadFile(path);
                    //deData = MyCrypto.BlowfishDecode(filedata);

                    return deData.ToArray();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                    return null;
                }
                finally
                {
                    if (deData != null)
                    {
                        deData.Close();
                        deData.Dispose();
                        deData = null;
                    }
                    //buffer = null;
                    ReleaseMemory();
                }
            }
        }

        public void OnThreadDecode(object emBundleType)
        {
            lock (this)
            {
                try
                {
                    fileDedata = GetFileDedata(strBundleFilePath + ((emAssetBundle)emBundleType).ToString().ToLower() + AssetBundlePath.s_fileSuffix);
                    if (fileDedata == null)
                    {
                        throw new System.Exception("Load BundleFile Failed! Type: " + emBundleType.ToString());
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                    instance.listAsync.Remove(this);
                    instance.dictAssetBundleInfo.Remove((emAssetBundle)emBundleType);
                }
            }
        }
    }
    #endregion
}