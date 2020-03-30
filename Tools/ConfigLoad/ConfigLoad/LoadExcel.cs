using System;
using System.IO;
using Excel;
using System.Data;
using LitJson;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
public struct SelfPropertyInfo
{
    public string defaultData;
    public string fieldType;
    public string propertyName;
    public string generalTarget;
}
namespace ConfigLoad
{
    public class LoadExcel
    {
        public static LoadExcel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LoadExcel();
                }
                return _instance;
            }
        }
        static LoadExcel _instance;
        FileInfo[] fileInfo;
        public JsonData jsRoot;
        public Dictionary<string, string> filePath = new Dictionary<string, string>();

        public Dictionary<string, List<SelfPropertyInfo>> GeneralCodeData = new Dictionary<string, List<SelfPropertyInfo>>();

        public Dictionary<string, List<string>> dict_ConfigIdNick = new Dictionary<string, List<string>>();

        public Dictionary<string, string> EnumDict = new Dictionary<string, string>();
        public Dictionary<string, string> StructDict = new Dictionary<string, string>();

        // key:ConfigName value ConfigData 配表数据汇总 
        public Dictionary<string, Dictionary<string, List<object>>> cfgDatas = new Dictionary<string, Dictionary<string, List<object>>>();
        // 表字段名顺序
        public Dictionary<string, string[]> cfgDataNameIdxMap = new Dictionary<string, string[]>();

        // struct的转换 
        public Dictionary<string, Dictionary<string, TypeCode>> structFieldNameType = new Dictionary<string, Dictionary<string, TypeCode>>();
        public Dictionary<string, string[]> structFieldIdx = new Dictionary<string, string[]>();
        public Dictionary<string, TypeCode> baseCanConvertType = new Dictionary<string, TypeCode>()
        {
            { "int",TypeCode.Int32},
            { "uint",TypeCode.UInt32},
            { "bool",TypeCode.Boolean},
            { "float",TypeCode.Single},
            { "byte",TypeCode.Byte },
            { "long",TypeCode.Int64},
            { "string",TypeCode.String}
        };

        private int dataNameRow = 0;
        private int dataTypeRow = 1;
        private int dataGeneralTargetRow = 2;
        private int dataDefaultRow = 3;

        public Dictionary<string, TypeCode> ConvertStructStringToStructTypeDict(string structValue, out string[] fieldIdx)
        {
            if (structValue.IndexOf("|") == -1)
            {
                fieldIdx = null;
                return null;
            }
            Dictionary<string, TypeCode> dict = new Dictionary<string, TypeCode>();
            string temp = structValue.Replace(" ", "");
            string[] keyValue = temp.Split('|');
            fieldIdx = new string[keyValue.Length];
            for (int i = 0; i < keyValue.Length; i++)
            {
                string[] kv = keyValue[i].Split('=');
                string typeName = kv[0].ToLower();
                if (!baseCanConvertType.ContainsKey(typeName))
                {
                    throw new Exception($"type {typeName} is not support");
                }
                dict.Add(kv[1], baseCanConvertType[typeName]);
                fieldIdx[i] = kv[1];
            }

            return dict;
        }

        public void LoadGeneralCodeDataFromFile(string path)
        {
            GeneralCodeData.Clear();
            dict_ConfigIdNick.Clear();
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] fileInfo = dirInfo.GetFiles("*.xlsx", SearchOption.AllDirectories);
            const int keyRow = 0;

            foreach (FileInfo fileData in fileInfo)
            {
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileData.OpenRead());
                DataSet excData = excelReader.AsDataSet();

                if (excData == null)
                    continue;
                if (excData.Tables.Count == 0)
                    continue;
                int idStartRow = 0;

                string cfgName = fileData.Name.Replace(".xlsx", "").Replace(".xls", "") + "Config";

                Dictionary<int, string> dict_ColumnsIndex_Key = new Dictionary<int, string>();
                for (int tabs = 0; tabs < excData.Tables.Count; ++tabs)
                {
                    for (int j = 0; j < excData.Tables[tabs].Rows.Count; ++j)
                    {
                        var key = excData.Tables[tabs].Rows[j][keyRow];
                        if (key.ToString() == "id")
                        {
                            idStartRow = j;
                            break;
                        }
                    }
                }
                //TODO合并同表但列数不同的tab
                string[] dataNameIdx = null;
                for (int tabs = 0; tabs < excData.Tables.Count; ++tabs)
                {
                    if (dataNameIdx == null)
                    {
                        dataNameIdx = new string[excData.Tables[tabs].Columns.Count];
                    }
                    for (int j = 0; j < excData.Tables[tabs].Columns.Count; ++j)
                    {
                        var key = excData.Tables[tabs].Rows[idStartRow][j];
                        dataNameIdx[j] = key.ToString();
                    }
                }
                cfgDataNameIdxMap.Add(cfgName, dataNameIdx);

                // 别名
                List<string> ls_idNickName = new List<string>();
                for (int tabs = 0; tabs < excData.Tables.Count; ++tabs)
                {
                    // 正式数据开始的行
                    for (int j = idStartRow + 1; j < excData.Tables[tabs].Rows.Count; ++j)
                    {
                        var key = excData.Tables[tabs].Rows[j][keyRow];
                        if (key is string)
                        {
                            if ((key as string).IndexOf('|') != -1)
                            {
                                ls_idNickName.Add(key as string);
                            }
                        }

                    }
                }

                List<SelfPropertyInfo> ls_OneClassPropertyInfo = new List<SelfPropertyInfo>();
                for (int tabs = 0; tabs < excData.Tables.Count; ++tabs)
                {
                    for (int i = 1; i < excData.Tables[tabs].Columns.Count; ++i)
                    {
                        if (!(excData.Tables[tabs].Rows[0][i] is string))
                            continue;
                        SelfPropertyInfo spi = new SelfPropertyInfo();
                        Type t = spi.GetType();
                        object obj = Activator.CreateInstance(t);
                        for (int j = 0; j < idStartRow; ++j)
                        {
                            var key = excData.Tables[tabs].Rows[j][keyRow];
                            if (key is string)
                            {
                                string k = key.ToString();
                                string value = excData.Tables[tabs].Rows[j][i].ToString();
                                if (k == "fieldType")
                                {
                                    if (value.ToLower().IndexOf("enum") != -1)
                                    {
                                        string temp = value.ToLower();
                                        //enum Color:{ Red = 1| Black=2| Green=3 }
                                        temp = temp.Replace("enum", "").Replace(" ", "").Replace("{", "").Replace("}", "");
                                        string[] enumValue = temp.Split(':');
                                        if (!EnumDict.ContainsKey(enumValue[0]))
                                        {
                                            EnumDict.Add(enumValue[0], enumValue[1]);
                                        }
                                        else
                                        {
                                            Console.Write("enum Name:" + enumValue[0] + " is Already Exist!!");
                                        }
                                        value = "enum|" + enumValue[0];
                                    }
                                    if (value.ToLower().IndexOf("struct") != -1)
                                    {
                                        string temp = value;
                                        //enum Color:{ Red = 1| Black=2| Green=3 }
                                        temp = temp.Replace("struct", "").Replace(" ", "").Replace("{", "").Replace("}", "");
                                        string[] structValue = temp.Split(':');
                                        if (!StructDict.ContainsKey(structValue[0]))
                                        {
                                            StructDict.Add(structValue[0], structValue[1]);
                                            string[] fieldIdx;
                                            var dict = ConvertStructStringToStructTypeDict(structValue[1], out fieldIdx);
                                            structFieldNameType.Add(structValue[0], dict);
                                            structFieldIdx.Add(structValue[0], fieldIdx);
                                        }
                                        else
                                        {
                                            Console.Write("struct Name:" + structValue[0] + " is Already Exist!!");
                                        }
                                        value = "struct|" + structValue[0];
                                    }

                                }

                                FieldInfo fi = t.GetField(k);
                                if (fi != null)
                                {
                                    fi.SetValue(obj, value);
                                }
                            }
                            else
                            {
                                Console.WriteLine(key + " is not string");
                            }
                        }
                        ls_OneClassPropertyInfo.Add((SelfPropertyInfo)obj);
                    }
                }

                Dictionary<string, List<object>> cfgDataList = new Dictionary<string, List<object>>();
                //Columns 列的集合 Rows 行的集合
                //开始转换数据
                for (int tabs = 0; tabs < excData.Tables.Count; ++tabs)
                {
                    //Dictionary<string, List<object>> keyValuePairs = new Dictionary<string, List<object>>();
                    for (int i = 1; i < excData.Tables[tabs].Columns.Count; ++i)
                    {
                        List<object> list = new List<object>();
                        string dataName = (string)excData.Tables[tabs].Rows[dataNameRow][i];
                        var rowData = excData.Tables[tabs].Rows[dataTypeRow][i];
                        var defaultData = excData.Tables[tabs].Rows[dataDefaultRow][i];
                        if (rowData.GetType() != typeof(string))
                        {
                            continue;
                        }
                        var dataType = (string)rowData;
                        //这是一个自定义的struct
                        if (!baseCanConvertType.ContainsKey(dataType))
                        {
                            string typeName = GetSelfTypeName(dataType);

                            if (typeName == null)
                            {
                                continue;
                            }

                            if (!structFieldNameType.ContainsKey(typeName))
                            {
                                continue;
                            }

                            string[] fieldIdx = structFieldIdx[typeName];
                            var fieldTypeDict = structFieldNameType[typeName];
                            TypeCode[] typeCodes = new TypeCode[fieldIdx.Length];
                            for (int j = 0; j < fieldIdx.Length; j++)
                            {
                                string fieldName = fieldIdx[j];
                                TypeCode fieldType = fieldTypeDict[fieldName];
                                typeCodes[j] = fieldType;
                            }
                            for (int j = idStartRow + 1; j < excData.Tables[tabs].Rows.Count; ++j)
                            {
                                string orignalData = excData.Tables[tabs].Rows[j][i].ToString();
                                string[] subDatas = orignalData.Replace(" ", "").Split(';');
                                if (subDatas.Length < typeCodes.Length)
                                {
                                    subDatas = defaultData.ToString().Replace(" ", "").Split(';');
                                }
                                for (int k = 0; k < typeCodes.Length; k++)
                                {
                                    string subName = $"{dataName}.{fieldIdx[k]}";
                                    if (!cfgDataList.ContainsKey(subName))
                                    {
                                        cfgDataList.Add(subName, new List<object>());
                                    }
                                    string subData = typeCodes[k] == TypeCode.String ? "" : "0";
                                    if (k < subDatas.Length)
                                    {
                                        subData = subDatas[k];
                                    }

                                    object cSubData = Convert.ChangeType(subData, typeCodes[k]);
                                    cfgDataList[subName].Add(cSubData);
                                }
                            }

                        }
                        else
                        {
                            if (!cfgDataList.ContainsKey(dataName))
                            {
                                cfgDataList.Add(dataName, new List<object>());
                            }
                            for (int j = idStartRow + 1; j < excData.Tables[tabs].Rows.Count; ++j)
                            {
                                cfgDataList[dataName].Add(excData.Tables[tabs].Rows[j][i]);
                            }
                        }
                    }
                }
                cfgDatas.Add(cfgName, cfgDataList);
                dict_ConfigIdNick.Add(fileData.Name.Replace(".xlsx", "").Replace(".xls", ""), ls_idNickName);
                GeneralCodeData.Add(fileData.Name.Replace(".xlsx", "").Replace(".xls", ""), ls_OneClassPropertyInfo);
            }
        }

        private string GetSelfTypeName(string value)
        {
            if (value.ToLower().IndexOf("enum") != -1)
            {
                string temp = value.ToLower();
                //enum Color:{ Red = 1| Black=2| Green=3 }
                temp = temp.Replace("enum", "").Replace(" ", "").Replace("{", "").Replace("}", "");
                string[] enumValue = temp.Split(':');
                return enumValue[0];
            }
            if (value.ToLower().IndexOf("struct") != -1)
            {
                string temp = value;
                //enum Color:{ Red = 1| Black=2| Green=3 }
                temp = temp.Replace("struct", "").Replace(" ", "").Replace("{", "").Replace("}", "");
                string[] structValue = temp.Split(':');
                return structValue[0];
            }
            return null;
        }

        public void OpenFile(string path)
        {
            filePath.Clear();
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            fileInfo = dirInfo.GetFiles("*.xlsx", SearchOption.AllDirectories);

            const int keyRow = 0;

            jsRoot = new JsonData();
            jsRoot.SetJsonType(JsonType.Object);
            JsonData js;

            foreach (FileInfo fileData in fileInfo)
            {
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileData.OpenRead());
                DataSet excData = excelReader.AsDataSet();
                if (excData == null)
                    continue;
                if (excData.Tables.Count == 0)
                    continue;

                string fileName = fileData.Name;

                int id = 0;
                JsonData jsData = new JsonData();
                jsData.SetJsonType(JsonType.Object);
                for (int tabs = 0; tabs < excData.Tables.Count; ++tabs)
                {
                    for (int i = 1; i < excData.Tables[tabs].Rows.Count; ++i)
                    {
                        js = new JsonData();
                        js.SetJsonType(JsonType.Object);
                        bool flag = false;
                        for (int j = 0; j < excData.Tables[tabs].Columns.Count; ++j)
                        {
                            string key = excData.Tables[tabs].Rows[keyRow][j].ToString();
                            object data = excData.Tables[tabs].Rows[i][j];

                            if (data is double)
                            {
                                if (data.ToString().Contains("."))
                                {
                                    js[key] = new JsonData((double)data);
                                }
                                else
                                {
                                    js[key] = new JsonData(Convert.ToInt32(data));
                                }
                                flag = true;
                            }
                            else if (data is string)
                            {
                                try
                                {
                                    if ((string)data == " ")
                                        js[key] = new JsonData((string)data);
                                    else
                                    {
                                        JsonReader jsr = new JsonReader((string)data);
                                        JsonData jd = JsonMapper.ToObject(jsr);
                                        js[key] = jd;
                                    }

                                }
                                catch (System.Exception ex)
                                {
                                    js[key] = new JsonData((string)data);
                                }
                                flag = true;
                            }
                        }

                        if (flag)
                            if (js.IsValidKey("id"))
                            {
                                string strid = js["id"].ToString();
                                try
                                {
                                    id = int.Parse(strid);
                                    js["id"] = id;
                                    if (jsData.IsValidKey(strid))
                                        throw new Exception("此ID已存在! ID: " + id.ToString() + " Row: " + i.ToString());
                                    else
                                        jsData[strid] = js;
                                }
                                catch (System.Exception ex)
                                {
                                    MessageBox.Show(ex.Message.ToString() + "/nID: " + strid, fileName);
                                }
                            }
                            else
                            {
                                try
                                {
                                    js["id"] = ++id;
                                    if (jsData.IsValidKey(id.ToString()))
                                        throw new Exception("此ID已存在! ID: " + id.ToString() + " Row: " + i.ToString());
                                    else
                                        jsData[id.ToString()] = js;
                                }
                                catch (System.Exception ex)
                                {
                                    MessageBox.Show(ex.Message.ToString(), fileName);
                                }
                            }
                    }
                }

                string cfgName = fileData.Name.Split('.')[0];
                string cfgPath = "\\" + fileData.Directory.Name + "\\";

                jsRoot[cfgName] = jsData;

                if (!filePath.ContainsKey(cfgName))
                {
                    filePath.Add(cfgName, cfgPath);
                }
            }

            //Console.WriteLine(jsRoot.ToJson());
        }
    }
}
