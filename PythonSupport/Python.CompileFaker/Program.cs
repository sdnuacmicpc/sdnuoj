using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Python.CompileFaker
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

            File.Copy("Runner.exe", args[1]);
            File.Copy(args[0], args[1] + ".py");
        }
    }
}
