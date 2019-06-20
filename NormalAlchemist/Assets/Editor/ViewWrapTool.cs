using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class ViewWrapTool:EditorWindow
{
    public static string prefabPath = "Assets/_Prefabs/UI/"; 
    public static string wrapCsPath = "Assets/_Scripts/Common/UI/UIWrap/";
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
            window.prefabModelName = selectName + "Model";
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

        if (GUI.Button(new Rect(0, position.height - 20, 100, 20), "生成脚本并创建Prefab"))
        {
            CreatePrefab(Selection.activeGameObject, prefabName);

            CreateClass(prefabName, prefabControlName, prefabViewName);
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// 此函数用来根据某物体创建指定名字的Prefab
    /// </summary>
    /// <param name="go">选定的某物体</param>
    /// <param name="name">物体名</param>
    /// <returns>void</returns>
    void CreatePrefab(GameObject go, string name)
    {
        if (go == null)
        {
            go = new GameObject(name);
        }
        //先创建一个空的预制物体
        //预制物体保存在工程中路径，可以修改("Assets/" + name + ".prefab");
        GameObject tempPrefab = PrefabUtility.SaveAsPrefabAsset(go,prefabPath + name + ".prefab");
    }

    public static void CreateClass( string fileName,string ControlName,string ViewName)
    {

        try
        {
            string curDir = Application.dataPath + "/_Scripts/Common/UI/UIWrap/"+fileName;
            if (!System.IO.Directory.Exists(curDir))
            {
                System.IO.Directory.CreateDirectory(curDir);
            }
            CreateControl(curDir, ControlName);
            CreateView(curDir, ViewName);

        }
        catch (System.Exception e)
        {
            throw e;
        }
    }

    public static void CreateControl(string filePath,string controlName)
    {
        string FilePath = filePath+"/" + controlName + ".cs";

        System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, false);
        file.Write("using System.Collections;\n");
        file.Write("using System.Collections.Generic;\n");
        file.Write("using UnityEngine;\n");
        file.Write("\n");
        file.Write("public class " + controlName);
        file.Write(" : BaseControll");
        file.Write("\n{\n");
        file.Write("\n}");
        file.Close();
        file.Dispose();
    }

    public static void CreateView(string filePath, string viewName)
    {
        string FilePath = filePath + "/"+ viewName + ".cs";
        System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, false);
        file.Write("using System.Collections;\n");
        file.Write("using System.Collections.Generic;\n");
        file.Write("using UnityEngine;\n");
        file.Write("\n");
        file.Write("public class " + viewName);
        file.Write(" : BaseView, IBindView");
        file.Write("\n{\n");
        file.Write("\n}");
        file.Close();
        file.Dispose();
    }
}
