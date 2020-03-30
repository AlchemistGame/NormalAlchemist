using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace ConfigLoad
{
    class CodeGenerationMain
    {
        public readonly int xxxxx = 1;
        public string CodeGenerationResult { get; private set; } = "";
        public string EnumGenerationResult { get; private set; } = "";
        public string EnumGenerationResultProto { get; private set; } = "";
        public string StructGenerationResult { get; private set; } = "";
        public string StructGenerationResultProto { get; private set; } = "";
        Dictionary<string, List<SelfPropertyInfo>> dict_GeneralCodeData;
        Dictionary<string, string> dict_EnumCodeData;
        Dictionary<string, string> dict_StructCodeData;
        public bool isCoverDebug = true; // 是否编译时完全覆盖重写。否则会空出对应的字段作为废弃字段兼容老档
        public void GenerationAll()
        {
            //CodeGenerationResult = "namespace DataConfig\r\n{\r\n";
        }

        private const string UsingHead = "";

        private const string StartCode = "using DefineEnum;\nusing DefineStruct;\nnamespace DataConfig\r\n{\r\n";
        private const string StartCodeNew = "using DefineEnum;\nusing DefineStruct;\nnamespace ConfigLoad\r\n{\r\n";
        private const string EndCode = "}";
        private const string StartEnum = "namespace DefineEnum\r\n{\r\n";
        private const string StartEnumProto = "syntax = \"proto3\";\r\noption csharp_namespace = \"ConfigLoad\";\r\nmessage DefineEnum {\r\n";
        private const string StartStruct = "namespace DefineStruct\r\n{\r\n";
        private const string StartStructProto = "syntax = \"proto3\";\r\noption csharp_namespace = \"ConfigLoad\";\r\nmessage DefineStruct {\r\n";

        private string temp = "";

        private string CreateTop(string fileName)
        {
            return "    public class " + fileName + "\r\n" + "    {\r\n";
        }

        private string CreateBottom()
        {
            return "    }\r\n";
        }
        private string CreateStruct(string json, string structName)
        {
            string result = "    public struct " + structName + "\r\n    {\r\n";

            return result;
        }

        public struct Test
        {
            int x;
            int y;
            public Test(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        private int t_x;
        private int t_y;
        public Test T => new Test(this.t_x, this.t_y);

        HashSet<string> typeValidate = new HashSet<string>() { "uint", "int", "bool", "byte", "string", "float", "double", "enum", "struct" };
        HashSet<string> orignalTypeValidate = new HashSet<string>() { "uint", "int", "bool", "byte", "string", "float", "double" };
        //HashSet<string> subTypeValidate = new HashSet<string>() { "Vector3" };

        Dictionary<string, List<string>> subTypeValidate = new Dictionary<string, List<string>>() {
            { "Vector3",new List<string>(){ "float","float","float" }  },
        };

        HashSet<string> needChangeTypeValidate = new HashSet<string>() { "enum", "struct", "Vector3", "Vector4", "Vector2" };
        HashSet<string> structValidate = new HashSet<string>() { };

        private string TypeToProtoType(string cfgType)
        {
            switch (cfgType)
            {
                case "uint":
                    return "uint32";
                case "int":
                    return "int32";
                case "byte":
                    return "int32";
                default:
                    if (typeValidate.Contains(cfgType))
                    {
                        return cfgType;
                    }
                    else
                    {
                        return null;
                    }
            }
        }

        private string CreatePropertyProto(string type, string name, int idx, string defaultData = "")
        {
            //type = type.ToLower();
            string result = "";
            string enumTypeName = "";
            string structTypeName = "";
            bool isEnumTypeAndHad = false;
            if (!typeValidate.Contains(type) && !dict_EnumCodeData.ContainsKey(type) && !dict_StructCodeData.ContainsKey(structTypeName))
            {
                if (type.IndexOf("enum") != -1)
                {
                    enumTypeName = type.Split('|')[1];
                    type = "enum";
                }
                else if (type.IndexOf("struct") != -1)
                {
                    structTypeName = type.Split('|')[1];
                    type = "struct";
                    if (!dict_StructCodeData.ContainsKey(structTypeName))
                    {
                        Console.WriteLine("No this type struct:" + structTypeName);
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            defaultData = defaultData.Replace(" ", "");
            if (dict_EnumCodeData.ContainsKey(enumTypeName))
            {
                isEnumTypeAndHad = true;
                int outInt_enum;
                bool isInt = int.TryParse(defaultData, out outInt_enum);
                if (!isInt)
                {
                    if (defaultData != "" && dict_EnumCodeData[enumTypeName].IndexOf(defaultData.ToLower()) == -1)
                    {
                        defaultData = "";
                    }
                    else
                    {
                        defaultData = enumTypeName.ToUpper() + "." + defaultData.ToUpper();
                    }
                }
            }

            bool isSuccess = true;
            if (defaultData != "")
            {
                switch (type)
                {
                    case "uint":
                        uint outUInt;
                        isSuccess = uint.TryParse(defaultData, out outUInt);
                        break;
                    case "int":
                        int outInt;
                        isSuccess = int.TryParse(defaultData, out outInt);
                        break;
                    case "bool":
                        bool outBool;
                        isSuccess = bool.TryParse(defaultData, out outBool);
                        break;
                    case "byte":
                        byte outByte;
                        isSuccess = byte.TryParse(defaultData, out outByte);
                        break;
                    case "float":
                        float outF;
                        isSuccess = float.TryParse(defaultData, out outF);
                        break;
                    case "double":
                        double outD;
                        isSuccess = double.TryParse(defaultData, out outD);
                        break;
                    case "string":
                        defaultData = "\"" + defaultData + "\"";
                        break;
                    default:
                        if (isEnumTypeAndHad)
                        {
                            isSuccess = true;
                        }
                        break;
                }
                if (isSuccess == false)
                {
                    Console.WriteLine("Type :" + type + " But Get: " + defaultData);
                    return null;
                }
            }

            if (typeValidate.Contains(type) || isEnumTypeAndHad)
            {
                if (type != "enum" && type != "struct")
                {
                    string protoType = TypeToProtoType(type);
                    if (protoType != null)
                    {
                        result = "".PadLeft(8) + string.Format("public {0} {1} = {2};\r\n", protoType, name, idx);
                    }
                }
                else
                {
                    if (type == "enum")
                    {
                        if (enumTypeName != "")
                        {

                            result = "".PadLeft(8) + string.Format("public {0} {1} = {2};\r\n", enumTypeName.ToUpper(), name, idx);
                        }
                        else
                        {
                            Console.WriteLine("EnumTypeName is Empty");
                            return "";
                        }
                    }
                    if (type == "struct")
                    {
                        if (structTypeName != "")
                        {
                            result = "".PadLeft(8) + $"public {structTypeName} {name} = {idx};\r\n";
                        }
                        else
                        {
                            Console.WriteLine("structTypeName is Empty");
                            return "";
                        }
                    }
                }
                return result;
            }
            else
            {
                Console.WriteLine("Type :" + type + " Is Not Legal");
                //Debug.LogError("Type :" + type + " Is Not Legal");
                return "";
            }
        }

        private string CreateProperty(string type, string name, string defaultData = "")
        {
            //type = type.ToLower();
            string result = "";
            string enumTypeName = "";
            string structTypeName = "";
            bool isEnumTypeAndHad = false;
            if (!typeValidate.Contains(type) && !dict_EnumCodeData.ContainsKey(type) && !dict_StructCodeData.ContainsKey(structTypeName))
            {
                if (type.IndexOf("enum") != -1)
                {
                    enumTypeName = type.Split('|')[1];
                    type = "enum";
                }
                else if (type.IndexOf("struct") != -1)
                {
                    structTypeName = type.Split('|')[1];
                    type = "struct";
                    if (!dict_StructCodeData.ContainsKey(structTypeName))
                    {
                        Console.WriteLine("No this type struct:" + structTypeName);
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            defaultData = defaultData.Replace(" ", "");
            if (dict_EnumCodeData.ContainsKey(enumTypeName))
            {
                isEnumTypeAndHad = true;
                int outInt_enum;
                bool isInt = int.TryParse(defaultData, out outInt_enum);
                if (!isInt)
                {
                    if (defaultData != "" && dict_EnumCodeData[enumTypeName].IndexOf(defaultData.ToLower()) == -1)
                    {
                        defaultData = "";
                    }
                    else
                    {
                        defaultData = enumTypeName.ToUpper() + "." + defaultData.ToUpper();
                    }
                }
            }

            bool isSuccess = true;
            if (defaultData != "")
            {
                switch (type)
                {
                    case "uint":
                        uint outUInt;
                        isSuccess = uint.TryParse(defaultData, out outUInt);
                        break;
                    case "int":
                        int outInt;
                        isSuccess = int.TryParse(defaultData, out outInt);
                        break;
                    case "bool":
                        bool outBool;
                        isSuccess = bool.TryParse(defaultData, out outBool);
                        break;
                    case "byte":
                        byte outByte;
                        isSuccess = byte.TryParse(defaultData, out outByte);
                        break;
                    case "float":
                        float outF;
                        isSuccess = float.TryParse(defaultData, out outF);
                        break;
                    case "double":
                        double outD;
                        isSuccess = double.TryParse(defaultData, out outD);
                        break;
                    case "string":
                        defaultData = "\"" + defaultData + "\"";
                        break;
                    //case "enum":
                    //    int outInt_enum;
                    //    isSuccess = int.TryParse(defaultData, out outInt_enum);
                    //    if (!isSuccess )
                    //    {
                    //        defaultData = enumTypeName.ToUpper() + "." + defaultData.ToUpper();
                    //        isSuccess = true;
                    //    }
                    //    break;
                    default:
                        if (isEnumTypeAndHad)
                        {
                            isSuccess = true;
                        }
                        break;
                }
                if (isSuccess == false)
                {
                    Console.WriteLine("Type :" + type + " But Get: " + defaultData);
                    return null;
                }
            }

            if (typeValidate.Contains(type) || isEnumTypeAndHad)
            {
                if (type != "enum" && type != "struct")
                {
                    if (defaultData != "")
                    {
                        result = $"        public {type} {name} = {defaultData};\r\n";
                    }
                    else
                    {
                        result = $"        public {type} {name};\r\n";
                    }
                }
                else
                {
                    if (type == "enum")
                    {
                        if (enumTypeName != "")
                        {
                            if (defaultData != "")
                            {
                                result = "        public " + enumTypeName.ToUpper() + " " + name + " = " + defaultData + ";\r\n";
                            }
                            else
                            {
                                result = "        public " + enumTypeName.ToUpper() + " " + name + ";\r\n";
                            }
                        }
                        else
                        {
                            Console.WriteLine("EnumTypeName is Empty");
                            return "";
                        }
                    }
                    if (type == "struct")
                    {
                        if (structTypeName != "")
                        {
                            result = "        public " + structTypeName + "? " + name + ";\r\n";
                        }
                        else
                        {
                            Console.WriteLine("structTypeName is Empty");
                            return "";
                        }
                    }
                }
                return result;
            }
            else
            {
                Console.WriteLine("Type :" + type + " Is Not Legal");
                //Debug.LogError("Type :" + type + " Is Not Legal");
                return "";
            }
        }

        //key: Vector3  value :  float = X| float=Y| float=Z
        public void GeneralCodeStructFromDict(Dictionary<string, string> GeneralStructData)
        {
            dict_StructCodeData = GeneralStructData;
            StructGenerationResult += StartStruct;
            StructGenerationResultProto += StartStructProto;
            foreach (var kv in GeneralStructData)
            {
                StructGenerationResult += CreateStructKeyValue(kv.Key, kv.Value);
                StructGenerationResultProto += CreateStructKeyValueProto(kv.Key, kv.Value);
            }
            StructGenerationResult += EndCode;
            StructGenerationResultProto += EndCode;
        }
        private string CreateStructKeyValue(string structName, string structValue)
        {
            string structResult = "";
            string top = "    public struct " + structName + "\r\n" + "    {\r\n";
            string bottom = "    }\r\n";
            if (structValue.IndexOf("|") == -1)
            {
                return "";
            }
            string temp = structValue.Replace(" ", "");
            string[] keyValue = temp.Split('|');
            foreach (var i in keyValue)
            {
                string[] kv = i.Split('=');
                string result = CreateProperty(kv[0].ToLower(), kv[1]);
                if (result != null)
                {
                    structResult += result;
                }
            }

            return top + structResult + bottom;
        }
        private string CreateStructKeyValueProto(string structName, string structValue)
        {
            string structResult = "";
            string top = "".PadLeft(2) + "Struct " + structName + "\r\n" + "".PadLeft(2) + "{\r\n";
            string bottom = "".PadLeft(2) + "}\r\n";
            if (structValue.IndexOf("|") == -1)
            {
                return "";
            }
            string temp = structValue.Replace(" ", "");
            string[] keyValue = temp.Split('|');
            int id = 1;
            foreach (var i in keyValue)
            {
                string[] kv = i.Split('=');
                string result = CreatePropertyProto(kv[0].ToLower(), kv[1], id);
                if (result != null && result != "")
                {
                    id = id + 1;
                    structResult += result;
                }
            }

            return top + structResult + bottom;
        }

        //key: Color  value :  Red = 1| Black=2| Green=3
        public void GeneralCodeEnumFromDict(Dictionary<string, string> GeneralEnumData)
        {
            dict_EnumCodeData = GeneralEnumData;
            EnumGenerationResult += StartEnum;
            EnumGenerationResultProto += StartEnumProto;
            foreach (var kv in GeneralEnumData)
            {
                EnumGenerationResult += CreateEnumKeyValue(kv.Key, kv.Value);
                EnumGenerationResultProto += CreateEnumKeyValueProto(kv.Key, kv.Value);
            }
            EnumGenerationResult += EndCode;
            EnumGenerationResultProto += EndCode;
        }

        /// <summary>
        /// 生成Proto格式的Enum
        /// </summary>
        /// <param name="enumName"></param>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        private string CreateEnumKeyValueProto(string enumName, string enumValue)
        {
            string enumResult = "";
            string enumNameUpper = enumName.ToUpper();
            string blank = "".PadLeft(2);
            string enumDeclear = "{0}enum {1} {{\r\n";
            string top = string.Format(enumDeclear, blank, enumNameUpper);
            string bottom = "".PadLeft(2) + "}\r\n";
            if (enumValue.IndexOf("|") == -1)
            {
                return "";
            }
            string temp = enumValue.Replace(" ", "");
            string[] keyValue = temp.Split('|');
            uint outUInt;
            string firstKey = null;
            foreach (var i in keyValue)
            {
                string[] kv = i.Split('=');
                bool result = uint.TryParse(kv[1], out outUInt);
                //Proto格式的语法中Enum的第一个值必须为0
                if (outUInt == 0)
                {
                    firstKey = kv[0];
                    continue;
                }
                if (result)
                {
                    enumResult += "".PadLeft(4) + string.Format("{0} = {1};\r\n", kv[0].ToUpper(), kv[1]);
                }
            }
            //自动添加NULL，防止重名预留GNULL为关键字
            if (firstKey == null)
            {
                firstKey = "GNULL";
            }
            enumResult = "".PadLeft(4) + string.Format("{0} = {1};\r\n", firstKey.ToUpper(), 0) + enumResult;
            return top + enumResult + bottom;
        }

        private List<string> GetEnumStringList(string enumValue)
        {
            List<string> list = null;
            if (enumValue.IndexOf("|") == -1)
            {
                return list;
            }
            string temp = enumValue.Replace(" ", "");
            string[] keyValue = temp.Split('|');
            list = new List<string>();
            uint outUInt;
            foreach (var i in keyValue)
            {
                string[] kv = i.Split('=');
                bool result = uint.TryParse(kv[1], out outUInt);
                if (result)
                {
                    list.Add(kv[0].ToUpper());
                }
            }
            return list;
        }

        private string CreateEnumKeyValue(string enumName, string enumValue)
        {
            string enumResult = "";
            string top = "".PadLeft(4) + string.Format("public enum {0}\r\n", enumName.ToUpper()) + "".PadLeft(4) + "{\r\n";
            //string top = "    public enum " + enumName.ToUpper() + "\r\n" + "    {\r\n";
            string bottom = "    }\r\n";
            if (enumValue.IndexOf("|") == -1)
            {
                return "";
            }
            string temp = enumValue.Replace(" ", "");
            string[] keyValue = temp.Split('|');
            uint outUInt;
            foreach (var i in keyValue)
            {
                string[] kv = i.Split('=');
                bool result = uint.TryParse(kv[1], out outUInt);
                if (result)
                {
                    enumResult += "        " + kv[0].ToUpper() + " = " + kv[1] + ",\r\n";
                }
            }

            return top + enumResult + bottom;
        }

        public void GeneralCodeFromDictUseTemp(string className, string temp)
        {
            var GeneralCodeData = LoadExcel.Instance.GeneralCodeData;
            CodeGenerationResult += StartCodeNew;
            //string className = "";
            string propertyAndNickNameList = "";
            string classPropertySetAndDefault = "";

            if (LoadExcel.Instance.dict_ConfigIdNick.ContainsKey(className))
            {
                var list = LoadExcel.Instance.dict_ConfigIdNick[className];
                if (list.Count != 0)
                {
                    string IdNickName = "";
                    IdNickName += BuildIdNickNameToUintString(list);
                    propertyAndNickNameList += IdNickName;
                }
            }
            if (LoadExcel.Instance.GeneralCodeData.ContainsKey(className))
            {
                var list = LoadExcel.Instance.GeneralCodeData[className];

                var propertyList = GeneralPropertyList(list, out classPropertySetAndDefault);
                propertyAndNickNameList += propertyList;
            }

            CodeGenerationResult += string.Format(temp, className, propertyAndNickNameList, classPropertySetAndDefault);

            CodeGenerationResult += EndCode;
        }
        public enum Test2
        {
            A = 1,
            B = 2,
        }
        private string GeneralPropertyList(List<SelfPropertyInfo> list, out string setAndDefaultList)
        {
            string PropertyList = "";
            setAndDefaultList = "";
            // propertyName propertyDefault propertyType
            // 基本类型
            string norSetAndDefaultTemp = "cfg.{0} = (temp = ConfigTable.TryGetColDataFromPool(\"{0}\", idx)) == null ? {1} : ({2})temp;\r\n";
            //string enumTemp = "cfg.{0} = (temp = ConfigTable.TryGetColDataFromPool(\"{0}\", idx)) == null ? {1} : ({2})temp;\r\n";
            string structAndDefaultTemp = "cfg.{0} = new {2} (temp = ConfigTable.TryGetColDataFromPool(\"{0}\", idx)) == null ? {1} : ({2})temp;\r\n";

            string temp = "".PadLeft(4) + "public {0} {1};\r\n";
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].generalTarget.IndexOf("c") == -1)
                {
                    if (orignalTypeValidate.Contains(list[i].fieldType))
                    {
                        PropertyList += string.Format(temp, list[i].fieldType, list[i].propertyName);
                        setAndDefaultList += string.Format(norSetAndDefaultTemp, list[i].propertyName, list[i].defaultData, list[i].fieldType);
                    }
                    else if (needChangeTypeValidate.Contains(list[i].fieldType))
                    {
                        PropertyList += string.Format(temp, list[i].fieldType, list[i].propertyName);
                    }
                    else if (list[i].fieldType.IndexOf("struct") != -1)
                    {
                        string realType = GetRealType(list[i].fieldType);
                        var structFieldDict = LoadExcel.Instance.structFieldNameType[realType];
                        PropertyList += string.Format(temp, realType, list[i].propertyName);
                    }
                    else if (list[i].fieldType.IndexOf("enum") != -1)
                    {
                        string realType = GetRealType(list[i].fieldType);
                        if (!LoadExcel.Instance.EnumDict.ContainsKey(realType))
                        {
                            continue;
                        }
                        string enumValue = LoadExcel.Instance.EnumDict[realType];
                        var enumlist = GetEnumStringList(enumValue);
                        if (enumlist == null || enumlist.Count == 0)
                        {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(list[i].defaultData))
                        {
                            //默认值为 EnumName.defualtData
                            setAndDefaultList += string.Format(norSetAndDefaultTemp, list[i].propertyName, list[i].fieldType + "." + list[i].defaultData.ToUpper(), list[i].fieldType);
                        }
                        else
                        {
                            setAndDefaultList += string.Format(norSetAndDefaultTemp, list[i].propertyName, list[i].fieldType + "." + enumlist[0].ToUpper(), list[i].fieldType);
                        }
                        PropertyList += string.Format(temp, realType, list[i].propertyName);
                    }
                }
            }
            return PropertyList;
        }

        private string GetRealType(string buildType)
        {
            string[] split = buildType.Split('|');
            if (split.Length > 1)
            {
                return split[1];
            }
            else
            {
                return null;
            }
        }

        public void GeneralCodeFromDict(Dictionary<string, List<SelfPropertyInfo>> GeneralCodeData, Dictionary<string, List<string>> IdNickData)
        {
            dict_GeneralCodeData = GeneralCodeData;
            CodeGenerationResult += StartCode;

            if (IdNickData != null)
                foreach (var i in IdNickData)
                {
                    if (i.Value.Count == 0)
                        continue;
                    string _GeneralEnumCode = "";
                    _GeneralEnumCode += SerchIdNickName(i.Key, i.Value);
                    CodeGenerationResult += _GeneralEnumCode;
                }

            foreach (var i in GeneralCodeData)
            {
                if (i.Value.Count == 0)
                    continue;
                string _GeneralClassCode = "";
                _GeneralClassCode += CreateTop(i.Key);
                _GeneralClassCode += CreateProperty("int", "id");
                foreach (var j in i.Value)
                {
                    if (j.generalTarget.IndexOf("c") == -1)
                        continue;
                    _GeneralClassCode += CreateProperty(j.fieldType, j.propertyName, j.defaultData);
                }
                _GeneralClassCode += CreateBottom();
                CodeGenerationResult += _GeneralClassCode;
            }
            CodeGenerationResult += EndCode;
        }

        private string SerchIdNickName(string fileName, List<string> id_NickName)
        {
            string nickNameEnum = "";
            string top = "    public enum " + fileName + "Id\r\n" + "    {\r\n";
            string bottom = "    }\r\n";
            foreach (var i in id_NickName)
            {
                if (i.IndexOf("|") == -1)
                    continue;
                if (i.IndexOf("~") != -1)
                    continue;
                string temp = i.Replace(" ", "");
                string[] id_nick = temp.Split('|');
                if (id_nick.Length != 2)
                    continue;
                uint outUInt;
                bool result = uint.TryParse(id_nick[0], out outUInt);
                if (result)
                {
                    nickNameEnum += "        " + id_nick[1] + " = " + id_nick[0] + ",\r\n";
                }
            }

            return top + nickNameEnum + bottom;
        }

        private string BuildIdNickNameToUintString(List<string> id_NickName)
        {
            string nickNameList = "";
            string template = "    public uint {0} = {1};\r\n";

            foreach (var i in id_NickName)
            {
                if (i.IndexOf("|") == -1)
                    continue;
                if (i.IndexOf("~") != -1)
                    continue;
                string temp = i.Replace(" ", "");
                string[] id_nick = temp.Split('|');
                if (id_nick.Length != 2)
                    continue;
                uint outUInt;
                bool result = uint.TryParse(id_nick[0], out outUInt);
                if (result)
                {
                    nickNameList += string.Format(template, id_nick[1], id_nick[0]);
                }
            }

            return nickNameList;
        }

        public void WriteResultToCs(string filePath, string outPutString)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            FileStream fs = new FileStream(filePath, FileMode.Create);
            fs.Seek(0, SeekOrigin.Begin);
            byte[] data = System.Text.Encoding.Default.GetBytes(outPutString);
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
        }
        public void WriteResultToProtoc(string filePath, string outPutString)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            FileStream fs = new FileStream(filePath, FileMode.Create);
            fs.Seek(0, SeekOrigin.Begin);
            byte[] data = System.Text.Encoding.Default.GetBytes(outPutString);
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
        }

        public string LoadTemplate(string filePath)
        {
            if (File.Exists(filePath))
            {
                StreamReader strmopen = new StreamReader(filePath);
                string strOpen = strmopen.ReadToEnd();
                strmopen.Close();
                return strOpen;
            }
            else
            {
                return null;
            }

        }
    }
}
