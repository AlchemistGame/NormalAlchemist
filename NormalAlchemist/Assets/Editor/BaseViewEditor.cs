using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaseView))]
public class BaseViewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BaseView myScript = (BaseView)target;
        if (GUILayout.Button("创建对象"))
        {
            UnityEngine.Object[] arr = Selection.GetFiltered(myScript.GetType(), SelectionMode.TopLevel);

            string path = AssetDatabase.GetAssetPath(arr[0]);
            Debug.LogError(path);
        }
    }

}
