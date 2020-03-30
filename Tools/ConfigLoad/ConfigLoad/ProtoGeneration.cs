using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
//using System.
using System.IO;

namespace ConfigLoad
{
    class ProtoGeneration
    {
        public static string RunProtocEXE(string stringcommandLine)
        {
            string strInput = stringcommandLine;
            Process p = RunCmd();
            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(strInput + "&exit");

            p.StandardInput.AutoFlush = true;

            //获取输出信息
            string strOuput = p.StandardOutput.ReadToEnd();
            //等待程序执行完退出进程
            p.WaitForExit();
            p.Close();
            MessageBox.Show(strOuput);
            Console.WriteLine(strOuput);
            return strOuput;
        }

        public static Process RunCmd()
        {
            Process p = new Process();
            //设置要启动的应用程序
            p.StartInfo.FileName = "cmd.exe";
            //是否使用操作系统shell启动
            p.StartInfo.UseShellExecute = false;
            // 接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardInput = true;
            //输出信息
            p.StartInfo.RedirectStandardOutput = true;
            // 输出错误
            p.StartInfo.RedirectStandardError = true;
            //不显示程序窗口
            //p.StartInfo.CreateNoWindow = true;
            //启动程序
            p.Start();

            return p;
        }

        /// <summary>
        /// 懒得写了，先放着了，先用cs。
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string RunCSCToCreateDll(Action action)
        {
            //C:\Windows\Microsoft.NET\Framework
            FileStream stream = new FileStream(@".\pathConfig.txt", FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            //this.textbox1.Text = reader.ReadLine(); //一次性读取一行
            string path = reader.ReadToEnd();  //一次性读取全部数据
            reader.Close();
            stream.Close();
            bool isExists = false;
            if (System.IO.File.Exists(path + @"\csc.exe"))
            {
                //存在文件
                isExists = true;
            }
            else
            {
                if (System.IO.File.Exists(@"C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe"))
                {
                    path = @"C:\Windows\Microsoft.NET\Framework\v3.5";
                    isExists = true;
                }
            }
            if (isExists)
            {
                Process p = RunCmd();
                //向cmd窗口发送输入信息
                p.StandardInput.WriteLine("cd "+path);
                p.StandardInput.WriteLine("csc " + path);
                p.StandardInput.AutoFlush = true;
                p.WaitForExit();
                action();
            }
            else
            {
                MessageBox.Show("找不到文件csc.exe,请在根目录下的pathConfig中写入csc的目录");
            }
            //Process p = RunCmd();
            //向cmd窗口发送输入信息
            //p.StandardInput.WriteLine(strInput + "&exit");

            //p.StandardInput.AutoFlush = true;

            ////获取输出信息
            //string strOuput = p.StandardOutput.ReadToEnd();
            ////等待程序执行完退出进程
            //p.WaitForExit();
            //p.Close();
            //MessageBox.Show(strOuput);
            //Console.WriteLine(strOuput);
            return "";
        }

    }
}
