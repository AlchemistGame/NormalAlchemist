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

        Dictionary<string, List<SelfPropertyInfo>> dict_GeneralCodeData;
        public void GenerationAll()
        {
            //CodeGenerationResult = "namespace DataConfig\r\n{\r\n";
        }

        private const string StartCode = "namespace DataConfig\r\n{\r\n";
        private const string EndCode = "}";

        private string CreateTop(string fileName)
        {
            return "    public class "+fileName+ "\r\n"+"    {\r\n";
        }

        private string CreateBottom()
        {
            return "    }\r\n";
        }
        private string CreateStruct(string json,string structName)
        {
            string result = "    public struct "+ structName+ "\r\n    {\r\n";

            return result;
        }

        HashSet<string> typeValidate = new HashSet<string>() { "uint", "int", "bool", "byte", "string", "float", "double" };
        private string CreateProperty(string type, string name,string defaultData="")
        {
            type = type.ToLower();
            string result = "";
            if (!typeValidate.Contains(type))
            {
                return null;
            }
            defaultData = defaultData.Replace(" ", "");
            bool isSuccess = true;
            if (defaultData != "") {
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
                }
                if (isSuccess == false)
                {
                    Console.WriteLine("Type :" + type + " But Get: " + defaultData);
                    return null;
                }
            }
            if (typeValidate.Contains(type))
            {
                if (defaultData != "")
                {
                    result = "        public " + type + " " + name + " = "+defaultData +";\r\n";
                }
                else
                {
                    result = "        public " + type + " " + name + ";\r\n";
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

        public void GeneralCodeFromDict(Dictionary<string, List<SelfPropertyInfo>> GeneralCodeData)
        {
            dict_GeneralCodeData = GeneralCodeData;
            CodeGenerationResult += StartCode;
            foreach(var i in GeneralCodeData)
            {
                if (i.Value.Count == 0)
                    continue;
                string _GeneralClassCode = "";
                _GeneralClassCode += CreateTop(i.Key);
                _GeneralClassCode += CreateProperty("int", "id");
                foreach (var j in i.Value)
                {
                    if (j.generalTarget.IndexOf("c")==-1)
                        continue;
                    _GeneralClassCode += CreateProperty(j.fieldType,j.propertyName,j.defaultData);
                }
                _GeneralClassCode += CreateBottom();
                CodeGenerationResult += _GeneralClassCode;
            }
            CodeGenerationResult += EndCode;
        }
        
        public void WriteResultToCs(string filePath,string outPutString)
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
