using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            excel.LoadGeneralCodeDataFromFile(folderPath);
            codeGeneration.GeneralCodeFromDict(excel.GeneralCodeData);
            codeGeneration.WriteResultToCs(folderPath+"\\ConfigDefine.cs", codeGeneration.CodeGenerationResult);

        }
    }
}
