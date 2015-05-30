using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Configuration.Install;
using System.Reflection;
using System.Threading;
using JudgeClient.Definition;

namespace JudgeClient.JudgeService
{
    static class Program
    {
        static bool Try(Action action)
        {
            bool success = false;
            int count = 0;
            while (!success && ++count < 50)
            {
                try
                {
                    action();
                    Thread.Sleep(200);
                }
                catch { success = true; }
            }
            return success;
        }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {
            #if DEBUG

            new Service().Start();

            #else

            if (System.Environment.UserInteractive)
            {
                string parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        try
                        {
                            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.Log(ex);
                            Console.WriteLine("服务安装失败。");
                        }
                        break;
                    case "--uninstall":
                        try
                        {
                            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.Log(ex);
                            Console.WriteLine("服务卸载失败。");
                        }
                        break;
                    case "--tryuninstall":
                        try
                        {
                            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        }
                        catch (Exception ex) { }
                        break;
                    case "--restart":
                        try
                        {
                            bool res = Try(() => {
                                new ServiceController("SDNUACM JudgeService").Stop();
                            });
                            if(!res)
                                throw new Exception("重启服务失败：停止服务超时。");
                            res = Try(() => {
                                new ServiceController("SDNUACM JudgeService").Start();
                            });
                            if(!res)
                                throw new Exception("重启服务失败：启动服务超时。");
                            Console.WriteLine("重启服务成功。");
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.Log(ex);
                            Console.WriteLine("服务重启失败。");
                        }
                        break;
                    case "--start":
                        try
                        {
                            bool res = Try(() =>
                            {
                                new ServiceController("SDNUACM JudgeService").Start();
                            });
                            if (!res)
                                throw new Exception("启动服务失败：启动服务超时。");
                            Console.WriteLine("服务启动成功。");
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.Log(ex);
                            Console.WriteLine("服务启动失败。");
                        }
                        break;
                    case "--stop":
                        try
                        {
                            bool res = Try(() =>
                            {
                                new ServiceController("SDNUACM JudgeService").Stop();
                            });
                            if (!res)
                                throw new Exception("停止服务失败：服务停止超时。");
                            Console.WriteLine("服务停止成功。");
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.Log(ex);
                            Console.WriteLine("服务停止失败。");
                        }
                        break;
                    default:
                        Console.WriteLine("使用参数\r\n\t--install : 安装服务\r\n\t--uninstall : 卸载服务\r\n\t--start : 启动服务\r\n\t--stop : 停止服务\r\n\t--restart : 重启服务\r\n");
                        break;
                }
            }
            else
            {
                ServiceBase.Run(new Service());
            }

            #endif
        }
    }
}
