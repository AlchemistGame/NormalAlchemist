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
    public List<string> id_nick;
}
namespace ConfigLoad
{
    public class LoadExcel
    {
        FileInfo[] fileInfo;
        public JsonData jsRoot;
        public Dictionary<string, string> filePath = new Dictionary<string, string>();
        
      
        public Dictionary<string, List<SelfPropertyInfo>> GeneralCodeData = new Dictionary<string, List<SelfPropertyInfo>>();

        public Dictionary<string, List<string>> dict_ConfigIdNick = new Dictionary<string, List<string>>();

        public Dictionary<string, string> EnumDict = new Dictionary<string, string>();
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
                int idStartColumns = 0;
                for (int tabs = 0; tabs < excData.Tables.Count; ++tabs)
                {
                    for (int j = 0; j < excData.Tables[tabs].Columns.Count; ++j)
                    {
                        var key = excData.Tables[tabs].Rows[j][keyRow];
                        if (!(key is string))
                        {
                            idStartColumns = j;
                            break;
                        }
                    }
                }
                List<string> ls_idNickName = new List<string>();
                for(int tabs = 0; tabs < excData.Tables.Count; ++tabs)
                {
                    for (int j = idStartColumns+1; j < excData.Tables[tabs].Columns.Count; ++j)
                    {
                        var key = excData.Tables[tabs].Rows[j][keyRow];

                        if(key is string)
                        {
                            if((key as string).IndexOf('|') != -1)
                            {
                                ls_idNickName.Add(key as string);
                            }
                        }

                    }
                }

                List<SelfPropertyInfo> ls_OneClassPropertyInfo = new List<SelfPropertyInfo>();
                for (int tabs = 0; tabs < excData.Tables.Count; ++tabs)
                {
                    for (int i = 1; i < excData.Tables[tabs].Rows.Count; ++i)
                    {
                        if (!(excData.Tables[tabs].Rows[0][i] is string))
                            continue;
                        SelfPropertyInfo spi = new SelfPropertyInfo();
                        Type t = spi.GetType();
                        object obj = Activator.CreateInstance(t);
                        for (int j = 0; j < idStartColumns; ++j)
                        {
                            var key = excData.Tables[tabs].Rows[j][keyRow];
                            if(key is string)
                            {
                                string k = key.ToString();
                                string value = excData.Tables[tabs].Rows[j][i].ToString();
                                if (k == "fieldType" && value.ToLower().IndexOf("enum")!=-1)
                                {
                                    string temp = value.ToLower();
                                    //enum Color:{ Red = 1| Black=2| Green=3 }
                                    temp = temp.Replace("enum", "").Replace(" ", "").Replace("{", "").Replace("}","");
                                    string[] enumValue = temp.Split(':');
                                    if (!EnumDict.ContainsKey(enumValue[0]))
                                    {
                                        EnumDict.Add(enumValue[0], enumValue[1]);
                                    }
                                    else
                                    {
                                        Console.Write("enum Name:"+enumValue[0]+" is Already Exist!!");
                                    }
                                    value = "enum|" + enumValue[0];
                                }
                                
                                FieldInfo fi = t.GetField(k);
                                if(fi!= null)
                                {
                                    fi.SetValue(obj, value);
                                }
                            }
                            else
                            {
                                Console.WriteLine(key+" is not string");
                            }
                        }
                        SelfPropertyInfo result = (SelfPropertyInfo)obj;
                        //result.id_nick = ls_idNickName;
                        ls_OneClassPropertyInfo.Add(result);
                    }
                }
                dict_ConfigIdNick.Add(fileData.Name.Replace(".xlsx", "").Replace(".xls", ""), ls_idNickName);
                GeneralCodeData.Add(fileData.Name.Replace(".xlsx", "").Replace(".xls", ""), ls_OneClassPropertyInfo);
            }
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
