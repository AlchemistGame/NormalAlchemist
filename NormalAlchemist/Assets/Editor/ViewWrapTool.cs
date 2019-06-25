using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System;
public class ViewWrapTool : EditorWindow
{
    public static string prefabPath = "Assets/_Prefabs/UI/";
    public static string wrapCsPath = "Assets/_Scripts/Common/UI/UIWrap/";

    public static string startHead = "public class {0} : BaseView, IBindView\n{\n";
    [MenuItem("MyTool/CreateUI")]
    public static void CreateUI()
    {
        ViewWrapTool window = (ViewWrapTool)GetWindow(typeof(ViewWrapTool));  //定义一个窗口对象
        if (Selection.activeGameObject != null)
        {
            string selectName = Selection.activeGameObject.name;
            window.prefabName = selectName;
            window.prefabViewName = selectName + "View";
            window.prefabControlName = selectName + "Control";
            //window.prefabModelName = selectName + "ViewModel";
        }
    }

    [MenuItem("GameObject/小工具/WrapView", priority = 0)]
    static void WrapView()
    {
        if (Selection.activeGameObject != null)
        {
            GameObject go = Selection.activeGameObject;
            BaseView view = go.GetComponentInChildren<BaseView>();

            if (view != null)
            {
                string path = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(view));
                System.IO.FileStream fs = new System.IO.FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamReader sr = new StreamReader(fs);
                string con = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                fs.Close();
                fs.Dispose();
                bool isWraped = con.Contains("#region WrapStart");
                string pattern = @"(#region WrapStart)[\d\D]*?(#endregion WrapEnd)";
                UIBindCellBase[] lsBindCell = go.GetComponentsInChildren<UIBindCellBase>();
                Debug.LogError(view.GetType().Name);
                if (lsBindCell.Length > 0)
                {
                    string wrapStr = MakeWrapFieldDefault(lsBindCell);
                    string result = con;
                    Debug.LogError(wrapStr);
                    if (isWraped)
                    {
                        Match match = Regex.Match(con, pattern);
                        result = Regex.Replace(con, pattern, wrapStr);
                        Debug.LogError(match.Success);
                    }
                    else
                    {
                        string str = GetViewHead(view.GetType().Name);
                        Match match = Regex.Match(con, str);
                        Debug.LogError(match.Success);
                        result = Regex.Replace(con, str, str + "\n" + wrapStr);
                    }
                    StreamWriter fileWriter = new StreamWriter(path, false);
                    fileWriter.Write(result);
                    fileWriter.Flush();
                    fileWriter.Close();
                    fileWriter.Dispose();
                    AssetDatabase.Refresh();
                }
            }
        }
    }

    void OnInspectorUpdate()
    {
        this.Repaint();  //重新画窗口
    }
    private string prefabName;
    private string prefabViewName;
    private string prefabControlName;
    private string prefabModelName;
    void OnGUI()
    {
        GUILayout.BeginVertical();
        prefabName = EditorGUILayout.TextField("Prefab Name", prefabName);

        prefabViewName = EditorGUILayout.TextField("Prefab View Name", prefabViewName);
        prefabControlName = EditorGUILayout.TextField("Prefab Control Name", prefabControlName);
        prefabModelName = EditorGUILayout.TextField("Prefab Model Name", prefabModelName);

        if (GUI.Button(new Rect(0, position.height - 20, 120, 20), "生成脚本并创建Prefab"))
        {
            string controlPath, viewPath;
            CreateClass(prefabName, prefabControlName, prefabViewName , prefabModelName ,out controlPath, out viewPath);

            GameObject go = CreatePrefab(Selection.activeGameObject, prefabName);
            //CreatePrefab(go, prefabName);
            PrefabUtility.RevertObjectOverride(Selection.activeGameObject, InteractionMode.AutomatedAction);
        }
        if (GUI.Button(new Rect(120, position.height - 20, 100, 20), "添加脚本"))
        {
            GameObject go = CreatePrefab(Selection.activeGameObject, prefabName);
            go.AddComponent(Type.GetType(prefabControlName + ", Assembly-CSharp"));
            go.AddComponent(Type.GetType(prefabViewName + ", Assembly-CSharp"));
            PrefabUtility.RevertObjectOverride(Selection.activeGameObject, InteractionMode.AutomatedAction);
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// 此函数用来根据某物体创建指定名字的Prefab
    /// </summary>
    /// <param name="go">选定的某物体</param>
    /// <param name="name">物体名</param>
    /// <returns>void</returns>
    GameObject CreatePrefab(GameObject go, string name)
    {
        if (go == null)
        {
            go = new GameObject(name);
        }
        //先创建一个空的预制物体
        //预制物体保存在工程中路径，可以修改("Assets/" + name + ".prefab");
        GameObject tempPrefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath + name + ".prefab");
        return tempPrefab;
    }

    public static void CreateClass(string fileName, string ControlName, string ViewName,string ModelName, out string controlPath, out string viewPath)
    {

        string curDir = Application.dataPath + "/_Scripts/Common/UI/UIWrap/" + fileName;
        if (!System.IO.Directory.Exists(curDir))
        {
            System.IO.Directory.CreateDirectory(curDir);
        }
        controlPath = CreateControl(curDir, ControlName);
        viewPath = CreateView(curDir, ViewName);
        CreateModel(curDir, ModelName);
        AssetDatabase.Refresh();
        //CreateView(curDir, ViewName);
    }

    public static string CreateControl(string filePath, string controlName)
    {
        string FilePath = filePath + "/" + controlName + ".cs";
        Debug.LogError(FilePath);
        System.IO.FileStream file = new System.IO.FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamWriter fileWriter = new StreamWriter(file);
        fileWriter.Write("using System.Collections;\n");
        fileWriter.Write("using System.Collections.Generic;\n");
        fileWriter.Write("using UnityEngine;\n");
        fileWriter.Write("\n");
        fileWriter.Write("public class " + controlName);
        fileWriter.Write(" : BaseControll");
        fileWriter.Write("\n{\n");
        fileWriter.Write("\n}");
        fileWriter.Close();
        fileWriter.Dispose();
        return FilePath;
    }

    public static string CreateView(string filePath, string viewName)
    {
        string FilePath = filePath + "/" + viewName + ".cs";
        System.IO.FileStream file = new System.IO.FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamWriter fileWriter = new StreamWriter(file);
        fileWriter.Write("using System.Collections;\n");
        fileWriter.Write("using System.Collections.Generic;\n");
        fileWriter.Write("using UnityEngine;\n");
        fileWriter.Write("\n");
        string str = GetViewHead(viewName);
        fileWriter.Write("\n" + str);
        fileWriter.Write("\n}");
        fileWriter.Close();
        fileWriter.Dispose();
        return FilePath;
    }

    public static string CreateModel(string filePath, string modelName)
    {
        string FilePath = filePath + "/" + modelName + ".cs";
        Debug.LogError(FilePath);
        System.IO.FileStream file = new System.IO.FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamWriter fileWriter = new StreamWriter(file);
        fileWriter.Write("using System.Collections;\n");
        fileWriter.Write("using System.Collections.Generic;\n");
        fileWriter.Write("using UnityEngine;\n");
        fileWriter.Write("\n");
        fileWriter.Write("public class " + modelName);
        fileWriter.Write(" : BaseModel");
        fileWriter.Write("\n{\n");
        fileWriter.Write("\n}");
        fileWriter.Close();
        fileWriter.Dispose();
        return FilePath;
    }

    public static string MakeWrapFieldDefault(UIBindCellBase[] lsBindCell)
    {
        string rt = "#region WrapStart\n" +
            "    public new Dictionary<string, dynamic> ModelDefaultData = new Dictionary<string, dynamic>()\n    {";

        string kvs = "";
        foreach (var i in lsBindCell)
        {
            object value = i.GetDefaultObj();
            if (value != null)
            {
                kvs = kvs + "\n         { \"" + i.bindCellName + "\"," + TryParse(value) + " },";
            }
        }
        rt = rt + kvs + "\n    };\n    #endregion WrapEnd";
        return rt;
    }

    public static void MakeWrapModel(UIBindCellBase[] lsBindCell,string viewName,string path)
    {
        StreamWriter fileWriter = new StreamWriter(path, false);
        fileWriter.Write("public class " + viewName+ "Model");
        fileWriter.Write(" : BaseModel");
        fileWriter.Write("\n{\n");

        string kvs = "";
        foreach (var i in lsBindCell)
        {
            bool isHandle = i.GetHandleFunc()!=null;
            bool isFunc = i.GetFuncName() != null;
            if (isFunc || isHandle)
            {
                string kv = "\nprivate dynamic _"+i.bindCellName.ToLower();
                kv += "\npublic dynamic " + i.bindCellName;
                kv += "\nget";
                kv += "\n{";
                kv += "\nreturn _"+ i.bindCellName.ToLower()+";";
                kv += "\n}";
                kv += "\nset";
                kv += "\n{";
                kv += "\n  _" + i.bindCellName.ToLower() + " = value;";
                if (isHandle)
                {
                    kv += "\nTryCallHandle(\"" + i.bindCellName + "\", \"" + i.GetHandleFunc() + "\", value);";
                }
                if (isFunc)
                {
                    kv += "\nTryCall(\"" + i.bindCellName + "\", \"" + i.GetHandleFunc() + "\", value);";
                }

                kvs = kvs + kv;
            }
        }
        fileWriter.Write(kvs);
        fileWriter.Write("\n}");
        fileWriter.Flush();
        fileWriter.Close();
        fileWriter.Dispose();
    }

    static string GetViewHead(string name)
    {
        return "public class " + name + " : BaseView, IBindView\n{";
    }

    static string TryParse(object value)
    {
        string str = value.ToString();

        if (value.GetType() == typeof(string))
        {
            str = "\"" + str + "\"";
        }
        else
        {

        }

        return str;
    }
}
