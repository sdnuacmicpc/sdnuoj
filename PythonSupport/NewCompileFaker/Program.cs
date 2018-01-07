using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NewCompileFaker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Args Error");
                return;
            }
            DeleteIllegalInfo(args[0]);
            File.Copy(args[0], args[1] + ".py");
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
    }
}
