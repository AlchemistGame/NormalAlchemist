using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.Reflection;
/// <summary>
/// 配表管理
/// </summary>
public class ConfigManager : BaseManager
{
    private ConfigManager() { }
    private static ConfigManager instance;
    //string path = Application.isEditor ? "Assets/Config/" : "Config/";
    //string path = Application.streamingAssetsPath + "/config.assetbundle";
    //string path = "jar:file://" + Application.dataPath + "!/assets/";
    //string path = AssetbundlePath.AssetBundlePath.strRootResourcePath+"cfg/";
    string path = "cfg/";
    string MonsterMapPath = "cfg/MonsterMap/";
    Dictionary<string, JsonData> jsData;
    Dictionary<string, Dictionary<object, object>> dic_Data;

    Dictionary<string, JsonData> MonsterMapJsData;
    Dictionary<string, Dictionary<object, object>> dic_MonsterMapData;

    public static ConfigManager Instance
    {
        get
        {
            if (instance == null)
                instance = new ConfigManager();
            return instance;
        }
    }


    public override void Init()
    {
        try
        {
            LoadConfigFile();
            LoadMonsterMapConfigFile();
            
            //LoadData<Constants>();
            
            jsData.Clear();
            MonsterMapJsData.Clear();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        { }
    }

    public override void Update()
    {
    }
    public override void FixedUpdate()
    {
    }
    public override void OnDestroy()
    {
        jsData.Clear();
        dic_Data.Clear();
    }

    /// <summary>
    /// 获得常量表
    /// </summary>
    /// <returns></returns>
    //public Constants GetConstants()
    //{
    //    return GetConfig<Constants>(1);
    //}

    /// <summary>
    /// 获得配置数据
    /// </summary>
    /// <typeparam name="T">配表数据类型</typeparam>
    /// <param name="id"></param>
    /// <returns>当前ID的数据</returns>
    public T GetConfig<T>(object id) where T : IConfig
    {
        Type type = typeof(T);
        if (!dic_Data.ContainsKey(type.Name))
        {
            Debug.LogWarning(string.Format("GetConfig Is Null! Name:{0:S} ID:{1:D}", type.Name, id));
            return default(T);
        }

        if (!dic_Data[type.Name].ContainsKey(id))
        {
             
            return default(T);
        }

        return (T)dic_Data[type.Name][id];
    }

    /// <summary>
    /// 获得配表数据
    /// </summary>
    /// <typeparam name="T">配表数据类型</typeparam>
    /// <returns>配表全部数据</returns>
    public Dictionary<object, object> GetConfig<T>() where T : IConfig
    {
        Type type = typeof(T);
        if (dic_Data == null)
            return null;
        if (dic_Data.Count==0)
            return null;
        if (!dic_Data.ContainsKey(type.Name))
            return null;

        return dic_Data[type.Name];
    }

    /// <summary>
    /// 获得配表Enumerator
    /// </summary>
    /// <typeparam name="T">配表数据类型</typeparam>
    /// <returns>Enumerator</returns>
    public Dictionary<object, object>.Enumerator GetConfigEnumerator<T>() where T : IConfig
    {
        Type type = typeof(T);
        if (!dic_Data.ContainsKey(type.Name))
            return new Dictionary<object, object>.Enumerator();

        return dic_Data[type.Name].GetEnumerator();
    }

    void LoadConfigFile()
    {
        Debug.Log("------------- LoadConfig Start -------------");
        dic_Data = new Dictionary<string, Dictionary<object, object>>();
        jsData = new Dictionary<string, JsonData>();
        try
        {

#if DEVELOPMENT_BUILD
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (Directory.Exists(path))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(path);
                    FileInfo[] fileInfo = dirInfo.GetFiles("*.json", SearchOption.AllDirectories);

                    foreach (FileInfo fileData in fileInfo)
                    {
                        StreamReader sr = fileData.OpenText();
                        try
                        {
                            jsData[fileData.Name.Split('.')[0]] = JsonMapper.ToObject(sr.ReadToEnd());
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.Message);
                        }
                        finally
                        {
                            sr.Close();
                        }
                    }
                }
            }
            else
            {
                var listIE = ResourceManager.Instance.GetAllResource(emAssetBundle.Bundle_Config);

                //var listMC = ResourceManager.Instance.GetAllResource(emAssetBundle.Bundle_Config_MonsterMap);
                //TODO ceshi
                //WWW www = new WWW(Wpath);
                //string Wpath = Application.streamingAssetsPath+"/"+path+".assetbundle";
                if (listIE == null)
                    throw new Exception("Load Config Bundle Failed!");

                foreach (var ie in listIE)
                {
                    while (ie.MoveNext())
                    {
                        var obj = (KeyValuePair<string, UnityEngine.Object>)ie.Current;
                        if (obj.Value)
                        {
                            try
                            {
                                jsData[obj.Key] = JsonMapper.ToObject(obj.Value.ToString());
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogError(ex.Message + " keyName:" + obj.Key);
                            }
                        }
                    }
                }

            }
#else
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (Directory.Exists(path))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(path);
                    FileInfo[] fileInfo = dirInfo.GetFiles("*.json", SearchOption.AllDirectories);

                    foreach (FileInfo fileData in fileInfo)
                    {
                        StreamReader sr = fileData.OpenText();
                        try
                        {
                            jsData[fileData.Name.Split('.')[0]] = JsonMapper.ToObject(sr.ReadToEnd());
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.Message);
                        }
                        finally
                        {
                            sr.Close();
                        }
                    }
                }
            }
            else
            {
                var listIE = ResourceManager.Instance.GetAllResource(emAssetBundle.Bundle_Config);
                //TODO ceshi
                //WWW www = new WWW(Wpath);
                //string Wpath = Application.streamingAssetsPath+"/"+path+".assetbundle";
                if (listIE == null)
                    throw new Exception("Load Config Bundle Failed!");

                foreach (var ie in listIE)
                {
                    while (ie.MoveNext())
                    {
                        var obj = (KeyValuePair<string, UnityEngine.Object>)ie.Current;
                        if (obj.Value)
                        {
                            try
                            {
                                jsData[obj.Key] = JsonMapper.ToObject(obj.Value.ToString());
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogError(ex.Message + " keyName:" + obj.Key);
                            }
                        }
                    }
                }

            }
#endif
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE


#endif
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        Debug.Log("------------- LoadConfig Over -------------");
    }

    void LoadMonsterMapConfigFile()
    {
        Debug.Log("------------- LoadMonsterMap Start -------------");
        dic_MonsterMapData = new Dictionary<string, Dictionary<object, object>>();
        MonsterMapJsData = new Dictionary<string, JsonData>();
        try
        {

#if DEVELOPMENT_BUILD
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (Directory.Exists(MonsterMapPath))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(MonsterMapPath);
                    FileInfo[] fileInfo = dirInfo.GetFiles("*.json", SearchOption.AllDirectories);

                    foreach (FileInfo fileData in fileInfo)
                    {
                        StreamReader sr = fileData.OpenText();
                        try
                        {
                            MonsterMapJsData["MonsterMap_" + fileData.Name.Split('.')[0]] = JsonMapper.ToObject(sr.ReadToEnd());
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.Message);
                        }
                        finally
                        {
                            sr.Close();
                        }
                    }
                }
            }
            else
            {
                var listIE = ResourceManager.Instance.GetAllResource(emAssetBundle.Bundle_Config_MonsterMap);

                //var listMC = ResourceManager.Instance.GetAllResource(emAssetBundle.Bundle_Config_MonsterMap);
                //TODO ceshi
                //WWW www = new WWW(Wpath);
                //string Wpath = Application.streamingAssetsPath+"/"+path+".assetbundle";
                if (listIE == null)
                    throw new Exception("Load Config_MonsterMap Bundle Failed!");

                foreach (var ie in listIE)
                {
                    while (ie.MoveNext())
                    {
                        var obj = (KeyValuePair<string, UnityEngine.Object>)ie.Current;
                        if (obj.Value)
                        {
                            try
                            {
                                MonsterMapJsData["MonsterMap_" + obj.Key] = JsonMapper.ToObject(obj.Value.ToString());
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogError(ex.Message + " keyName:" + obj.Key);
                            }
                        }
                    }
                }

            }
#else
             if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (Directory.Exists(MonsterMapPath))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(MonsterMapPath);
                    FileInfo[] fileInfo = dirInfo.GetFiles("*.json", SearchOption.AllDirectories);

                    foreach (FileInfo fileData in fileInfo)
                    {
                        StreamReader sr = fileData.OpenText();
                        try
                        {
                            MonsterMapJsData[ "MonsterMap_"+fileData.Name.Split('.')[0]] = JsonMapper.ToObject(sr.ReadToEnd());
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.Message);
                        }
                        finally
                        {
                            sr.Close();
                        }
                    }
                }
            }
            else
            {
                var listIE = ResourceManager.Instance.GetAllResource(emAssetBundle.Bundle_Config_MonsterMap);

                //var listMC = ResourceManager.Instance.GetAllResource(emAssetBundle.Bundle_Config_MonsterMap);
                //TODO ceshi
                //WWW www = new WWW(Wpath);
                //string Wpath = Application.streamingAssetsPath+"/"+path+".assetbundle";
                if (listIE == null)
                    throw new Exception("Load Config_MonsterMap Bundle Failed!");

                foreach (var ie in listIE)
                {
                    while (ie.MoveNext())
                    {
                        var obj = (KeyValuePair<string, UnityEngine.Object>)ie.Current;
                        if (obj.Value)
                        {
                            try
                            {
                                var jsobj = JsonMapper.ToObject(obj.Value.ToString()); 
                                MonsterMapJsData["MonsterMap_"+obj.Key] = JsonMapper.ToObject(obj.Value.ToString());
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogError(ex.Message + " keyName:" + obj.Key);
                            }
                        }
                    }
                }

            }
#endif
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE


#endif
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        Debug.Log("------------- LoadMonsterMap Over -------------");
    }

    public Dictionary<object,object> GetMonsterMap(UInt16 id) {
        string key = "MonsterMap_" + id;
        if (dic_MonsterMapData.ContainsKey(key))
        {
            return dic_MonsterMapData[key];
        }
        else
        {
            return null;
        }
    }
    
    
    void LoadData<T>() where T : IConfig
    {
        Type objT = typeof(T);
        string key = objT.Name;
        //Debug.Log("------------- " + key + " LoadConfig Start -------------");
        try
        {
            if (jsData.ContainsKey(key))
            {
                JsonData js = JsonMapper.ToObject(jsData[key].ToJson());

                if (!dic_Data.ContainsKey(key))
                {
                    dic_Data[key] = new Dictionary<object, object>(js.Count);
                }

                var it = js.Keys.GetEnumerator();
                while (it.MoveNext())
                {
#region 弃用
                    //object obj = Activator.CreateInstance(objT);

                    //var itt = js[it.Current].Keys.GetEnumerator();

                    //while (itt.MoveNext())
                    //{
                    //    FieldInfo fi = objT.GetField(itt.Current);
                    //    if (fi == null)
                    //    {
                    //        Debug.LogError("ConfigManager.cs Not Found Field:" + itt.Current);
                    //        continue;
                    //    }
                    //    object value;
                    //    string data = js[it.Current][itt.Current].ToString();

                    //    if (fi.FieldType.BaseType == typeof(Array))
                    //    {
                    //        JsonData jsarray = JsonMapper.ToObject(data);
                    //        if (data != "null")
                    //        {
                    //            if(jsarray.IsArray)
                    //            {
                    //                string s= jsarray[0].ToJson();
                    //            }
                    //        }
                    //        value = new object[3];
                    //    }
                    //    else if (fi.FieldType.BaseType == typeof(System.Object)&& fi.FieldType != typeof(string))
                    //    {
                    //        FieldInfo[] field = fi.FieldType.GetFields();
                    //        value = new object();
                    //    }
                    //    else
                    //    {
                    //        value = Convert.ChangeType(js[it.Current][itt.Current].ToString(), fi.FieldType);
                    //    }


                    //    fi.SetValue(obj, value);
#endregion
                    try
                    {
                        T t = JsonMapper.ToObject<T>(js[it.Current].ToJson());
                        AddData(key, t);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError(ex.Message + "   " + key +" " + it.Current.ToString());
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message + " keyName:" + key);
        }
        finally
        {
            Debug.Log("------------- " + key + " LoadConfig Over -------------");
        }
    }

    void AddData(string tabsName, object obj)
    {
        var id = obj.GetType().GetField("id").GetValue(obj);

        if (dic_Data.ContainsKey(tabsName))
        {
            dic_Data[tabsName][id] = obj;
        }
        else
        {
            dic_Data[tabsName] = new Dictionary<object, object>();
            dic_Data[tabsName][id] = obj;
        }
        //Debug.Log("ID:" + id);
    }

}