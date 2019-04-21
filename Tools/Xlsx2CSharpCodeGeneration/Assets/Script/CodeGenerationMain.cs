using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excel;
public class CodeGenerationMain {
    //FileInfo[] fileInfo;
    public System.Collections.Generic.Dictionary<string, string> filePath = new System.Collections.Generic.Dictionary<string, string>();

    public void GenerationAll()
    {

    }

    private string CreateTop()
    {
        return "";
    }

    private string CreateBottom()
    {
        return "}\r\n}";
    }

    private string CreateStruct(string json)
    {
        string result = "";

        return result;
    }
    HashSet<string> typeValidate =new HashSet<string>(){ "uint", "int", "bool", "byte", "string", "float", "double" };

    //string[] typeValidate = { "uint","int","bool","byte","string","float","double" };
    private string CreateProperty(string type,string name)
    {
        type = type.ToLower();
        string result = "";
        if (typeValidate.Contains(type))
        {
            result = "public " + type + " " + name + ";\r\n";
            return result;
        }
        else
        {
            Debug.LogError("Type :"+type+ " Is Not Legal");
            return "";
        }
    }

    private void OpenAndReadExcel(string path)
    {
        filePath.Clear();
        //DirectoryInfo dirInfo = new DirectoryInfo(path);
        //fileInfo = dirInfo.GetFiles("*.xlsx", SearchOption.AllDirectories);
    }
}
