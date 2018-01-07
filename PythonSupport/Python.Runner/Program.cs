using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Python.Runner
{
    class Program
    {
        const int EOF = -1;
        static void Main(string[] args)
        {
            Process cur = Process.GetCurrentProcess();
            string runnerName = cur.ProcessName + ".exe";
            
            string pyPath = AppDomain.CurrentDomain.BaseDirectory + runnerName + ".py";

            DeleteIllegalInfo(pyPath);

            RunPython(pyPath);
        }

        static void DeleteIllegalInfo(string pyFilePath)
        {
            StreamReader sr = new StreamReader(pyFilePath, Encoding.Default);
            string allText = sr.ReadToEnd();
            sr.Close();
            int p = allText.LastIndexOf("/*");
            if (p == -1)
            {
                return;
            }
            allText = allText.Substring(0, p);

            FileStream fs = new FileStream(pyFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(allText);
            sw.Close();
            fs.Close();
        }
        static void RunPython(string pyFilePath)
        {
            Process p = new Process();
            p.StartInfo.FileName = @"C:\Users\Administrator\AppData\Local\Programs\Python\Python36\python.exe";
            p.StartInfo.Arguments = "\"" + pyFilePath+"\"";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;

            p.Start();

            p.StandardInput.AutoFlush = true;

            int input = Console.Read();
            while (input != EOF) 
            {
                p.StandardInput.Write((char)input);
                input = Console.Read();
            }
            p.WaitForExit();

            string res = p.StandardOutput.ReadToEnd();
            string error = p.StandardError.ReadToEnd();
            Console.Write(res);
            if(!String.IsNullOrEmpty(error))
            {
                Console.Error.WriteLine(error);
            }
            p.Close();
        }
    }
}
