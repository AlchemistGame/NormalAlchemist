using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigLoad
{
    class CodeGenerationMain
    {

        public System.Collections.Generic.Dictionary<string, string> filePath = new System.Collections.Generic.Dictionary<string, string>();
        string CodeGenerationResult;
        public void GenerationAll()
        {
            CodeGenerationResult = "namespace DataConfig\r\n{\r\n";

        }

        private string CreateTop(string fileName)
        {
            return "public class "+fileName+ "{\r\n";
        }

        private string CreateBottom()
        {
            return "}\r\n";
        }
        private string CreateStruct(string json,string structName)
        {
            string result = " public struct "+ structName+" {\r\n";

            return result;
        }

        HashSet<string> typeValidate = new HashSet<string>() { "uint", "int", "bool", "byte", "string", "float", "double" };
        private string CreateProperty(string type, string name,string defaultData)
        {
            type = type.ToLower();
            string result = "";
            if (typeValidate.Contains(type))
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
                    result = "  public " + type + " " + name + " = "+defaultData +";\r\n";
                }
                else
                {
                    result = "  public " + type + " " + name + ";\r\n";
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

        private void OpenAndReadExcel(string path)
        {
            filePath.Clear();
            //DirectoryInfo dirInfo = new DirectoryInfo(path);
            //fileInfo = dirInfo.GetFiles("*.xlsx", SearchOption.AllDirectories);
        }
    }
}
