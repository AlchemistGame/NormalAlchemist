using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace ConfigLoad
{
    class ProtoGeneration
    {


        public static string RunProtocEXE(string stringcommandLine, int packagesize)
        {
            Process process = new Process();
            process.StartInfo.Arguments = stringcommandLine;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "ping.exe";
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            return process.StandardOutput.ReadToEnd();//返回结果
        }

    }
}
