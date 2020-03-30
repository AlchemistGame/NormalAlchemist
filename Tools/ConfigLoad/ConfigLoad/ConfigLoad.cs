using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Windows.Forms;
namespace ConfigLoad
{
    public partial class ConfigLoad : Form
    {
        string path = Application.StartupPath + "\\Config\\";
        public ConfigLoad()
        {
            InitializeComponent();

        }
        static string folderPath;
        LoadExcel excel = null;
        CodeGenerationMain codeGeneration = new CodeGenerationMain();
        private void SelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = path;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                folderPath = dlg.SelectedPath;
                message.Text = "当前文件夹:" + folderPath;
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (folderPath == null)
            {
                MessageBox.Show("请选择文件夹!");
                return;
            }

            excel.OpenFile(folderPath);

            if (excel.jsRoot == null)
                return;
            if (excel.jsRoot.Count <= 0)
            {
                MessageBox.Show("未找到配置文件!请重新选择!");
                return;
            }

            try
            {
                SaveJson.Save(excel.jsRoot, excel.filePath);
                MessageBox.Show("成功!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void ConfigLoad_Load(object sender, EventArgs e)
        {
            excel = new LoadExcel();
            excel.GetType().GetProperties();
        }

        private void OpenFiles_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", SaveJson.path);
        }

        private void CodeCreate_Click(object sender, EventArgs e)
        {
            if(excel == null)
            {
                excel = new LoadExcel();
            }
            LoadExcel.Instance.LoadGeneralCodeDataFromFile(folderPath);
            string str = codeGeneration.LoadTemplate(folderPath + "\\ConfigDataTemplate.txt");
            // cfg.{propertyName[0]} =  (temp = ConfigTable.TryGetColDataFromPool("{propertyName[0]}", idx)) == null ? {propertyDefault[0]} : (propertyType[0])temp;
            // public {propertyType[0]} {propertyName[0]};
            string configName = "test";
            string buildStr = string.Format(str, configName);
            Console.Write(buildStr);
            codeGeneration.GeneralCodeEnumFromDict(excel.EnumDict);
            codeGeneration.GeneralCodeStructFromDict(excel.StructDict);
            codeGeneration.GeneralCodeFromDict(excel.GeneralCodeData,excel.dict_ConfigIdNick);

            codeGeneration.WriteResultToCs(folderPath + "\\StructDefine.cs", codeGeneration.StructGenerationResult);
            codeGeneration.WriteResultToCs(folderPath+"\\ConfigDefine.cs", codeGeneration.CodeGenerationResult);
            codeGeneration.WriteResultToCs(folderPath + "\\EnumDefine.cs", codeGeneration.EnumGenerationResult);
            codeGeneration.WriteResultToProtoc(folderPath + "\\EnumDefine.proto", codeGeneration.EnumGenerationResultProto);
            //codeGeneration.WriteResultToProtoc(folderPath + "\\ConfigDefine.proto", codeGeneration.EnumGenerationResultProto);
            codeGeneration.WriteResultToProtoc(folderPath + "\\StructDefine.proto", codeGeneration.StructGenerationResultProto);
            string protocCmd = ".\\protoc --proto_path={0} --csharp_out=.\\ {1}.proto";
            string buildedCmd = string.Format(protocCmd, folderPath+ "\\TestOutput", "EnumDefine");
            string buildedCmd2 = string.Format(protocCmd, folderPath + "\\TestOutput", "StructDefine");
            string retrunInfo =  ProtoGeneration.RunProtocEXE(buildedCmd);
            string retrunInfo2 = ProtoGeneration.RunProtocEXE(buildedCmd2);
            string[] codeList = new string[] { codeGeneration.EnumGenerationResult, codeGeneration.StructGenerationResult,codeGeneration.CodeGenerationResult };
            CompilerResults info = DebugRun(codeList, folderPath+ "\\ConfigLoad.dll");//+"\\EnumDefine.dll"
            System.Reflection.Assembly assembly = info.CompiledAssembly;




            Console.WriteLine(info.Output);
        }

        /// <summary>
        /// 动态编译并执行代码
        /// </summary>
        /// <param name="codelist">代码</param>
        /// <returns>返回输出内容</returns>
        public CompilerResults DebugRun(string[] codelist, string newPath)
        {
            CSharpCodeProvider complier = new CSharpCodeProvider();
            //设置编译参数
            CompilerParameters paras = new CompilerParameters();
            //引入第三方dll
            paras.ReferencedAssemblies.Add(@"System.dll");
            paras.ReferencedAssemblies.Add(@"System.configuration.dll");
            paras.ReferencedAssemblies.Add(@"System.Data.dll");
            paras.ReferencedAssemblies.Add(@"System.Management.dll");
            //引入自定义dll
            //paras.ReferencedAssemblies.Add(@"D:\自定义方法\自定义方法\bin\LogHelper.dll");
            //是否内存中生成输出
            paras.GenerateInMemory = false;
            //是否生成可执行文件
            paras.GenerateExecutable = false;
            paras.OutputAssembly = newPath;

            //编译代码
            CompilerResults result = complier.CompileAssemblyFromSource(paras, codelist);

            return result;
        }


    }
}
