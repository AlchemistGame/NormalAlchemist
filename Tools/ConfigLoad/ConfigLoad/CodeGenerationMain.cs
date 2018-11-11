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
        public string CodeGenerationResult { get; private set; } = "";
        public string EnumGenerationResult { get; private set; } = "";
        Dictionary<string, List<SelfPropertyInfo>> dict_GeneralCodeData;
        Dictionary<string, string> dict_EnumCodeData;
        public void GenerationAll()
        {
            //CodeGenerationResult = "namespace DataConfig\r\n{\r\n";
        }

        private const string UsingHead = "";

        private const string StartCode = "namespace DataConfig\r\n{\r\n";
        private const string EndCode = "}";
        private const string StartEnum = "namespace DefineEnum\r\n{\r\n";


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

        HashSet<string> typeValidate = new HashSet<string>() { "uint", "int", "bool", "byte", "string", "float", "double","enum" };
        private string CreateProperty(string type, string name, string defaultData = "")
        {
            type = type.ToLower();
            string result = "";
            string enumTypeName = "";
            bool isEnumTypeAndHad = false;
            if (!typeValidate.Contains(type) && !dict_EnumCodeData.ContainsKey(type))
            {
                if (type.IndexOf("enum") != -1)
                {
                    enumTypeName = type.Split('|')[1];
                    type = "enum";
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
                        defaultData= enumTypeName.ToUpper() + "." + defaultData.ToUpper();
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
                if (isEnumTypeAndHad)
                {
                    if (defaultData != "")
                    {
                        result = "        public " + enumTypeName.ToUpper() + " " + name + " = " + defaultData + ";\r\n";
                    }
                    else
                    {
                        result = "        public " + enumTypeName.ToUpper() + " " + name + ";\r\n";

                    }
                    return result;
                }
                if (type != "enum")
                {
                    if (defaultData != "")
                    {
                        result = "        public " + type + " " + name + " = " + defaultData + ";\r\n";
                    }
                    else
                    {
                        result = "        public " + type + " " + name + ";\r\n";
                    }
                }
                else
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
                return result;
            }
            else
            {
                Console.WriteLine("Type :" + type + " Is Not Legal");
                //Debug.LogError("Type :" + type + " Is Not Legal");
                return "";
            }
        }

        //key: Color  value :  Red = 1| Black=2| Green=3
        public void GeneralCodeEnumFromDict(Dictionary<string, string> GeneralEnumData)
        {
            dict_EnumCodeData = GeneralEnumData;
            EnumGenerationResult += StartEnum;
            foreach (var kv in GeneralEnumData)
            {
                EnumGenerationResult+= CreateEnumKeyValue(kv.Key, kv.Value);
            }
            EnumGenerationResult += EndCode;
        }
        private string CreateEnumKeyValue(string enumName, string enumValue)
        {
            string enumResult = "";
            string top = "public enum " + enumName.ToUpper() + "\r\n" + " {\r\n";
            string bottom = "}\r\n";
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
                    enumResult += kv[0].ToUpper() + " = " + kv[1] + ",\r\n";
                }
            }

            return top + enumResult + bottom;
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
            string top = "public enum " + fileName + "Id\r\n" + " {\r\n";
            string bottom = "}\r\n";
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
                    nickNameEnum += id_nick[1] + " = " +  id_nick[0] + ",\r\n";
                }
            }

            return top + nickNameEnum + bottom;
        }

        public void WriteResultToCs(string filePath, string outPutString)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            FileStream fs = new FileStream(filePath, FileMode.Create);
            byte[] data = System.Text.Encoding.Default.GetBytes(outPutString);
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
        }

    }
}
