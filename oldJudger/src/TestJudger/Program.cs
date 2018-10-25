using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Timers;

namespace TestJudger
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Process p = new Process();
            p.StartInfo.FileName = @"d:\lib\MinGW\bin\g++.exe";
            p.StartInfo.Arguments = @" d:\testjudger\src.cpp -o d:\testjudger\src.exe";
            p.StartInfo.WorkingDirectory = @"d:\lib\MinGW\bin\";
            //p.StartInfo.UseShellExecute = false;
            //p.StartInfo.RedirectStandardInput = true;
            //p.StartInfo.RedirectStandardOutput = true;
            //p.StartInfo.RedirectStandardError = true;
            //p.StartInfo.CreateNoWindow = true;

            p.Start();
            p.WaitForExit();

            p = new Process();
            p.StartInfo.FileName = @"d:\testjudger\src.exe";
            p.StartInfo.WorkingDirectory = @"d:\lib\MinGW\bin\";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            p.StandardInput.WriteLine("1 2");
            p.WaitForExit();
            */
            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());

            foreach (var s in new string[] { "" })
            {
                var a = new System.Timers.Timer(2000);
                a.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
                {
                    var cur_time = DateTime.Now;
                    System.Diagnostics.Trace.WriteLine(cur_time.Second);
                    System.Diagnostics.Trace.WriteLine(cur_time.Millisecond);
                    System.Diagnostics.Trace.WriteLine(s);
                };
                a.Start();
            }
            while (true)
                System.Threading.Thread.Sleep(50000);
        }
    }
}
