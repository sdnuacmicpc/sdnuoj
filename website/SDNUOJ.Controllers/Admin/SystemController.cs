using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Mvc;

using SDNUOJ.Caching;
using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Core;

namespace SDNUOJ.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdministrator")]
    public class SystemController : AdminBaseController
    {
        /// <summary>
        /// 后台欢迎页面
        /// </summary>
        /// <param name="id">测试内容</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Info(String id)
        {
            ViewData["SystemVersion"] = ConfigurationManager.Version;
            ViewData["DataBaseType"] = DatabaseManager.DataBaseType;
            ViewData["ServerName"] = Server.MachineName.ToString() + "/" + Request.ServerVariables["LOCAL_ADDR"] + ":" + Request.ServerVariables["SERVER_PORT"];
            ViewData["DataBaseSize"] = DatabaseManager.AccessDBSize;

            ViewData["ServerOSVersion"] = Environment.OSVersion.ToString();
            ViewData["DotNetVersion"] = ".NET CLR " + Environment.Version.ToString();
            ViewData["ServerSoftware"] = Request.ServerVariables["SERVER_SOFTWARE"];
            ViewData["RealPath"] = HttpRuntime.AppDomainAppPath;
            ViewData["ServerLanguage"] = CultureInfo.InstalledUICulture.EnglishName;
            ViewData["TimeZoneName"] = TimeZoneInfo.Local.DisplayName;
            ViewData["ServerCPUCount"] = Environment.ProcessorCount.ToString(); 
            ViewData["ServerCPUType"] = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");

            ViewData["ServerRunTime"] = this.GetSystemRunTime();
            ViewData["ScriptTimeout"] = Server.ScriptTimeout.ToString();
            ViewData["SystemStartTime"] = ConfigurationManager.SystemStartTime.ToString("yyyy-MM-dd HH:mm:ss");
            ViewData["ServerNowTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ViewData["DotNetCpuTime"] = (Process.GetCurrentProcess().TotalProcessorTime).TotalSeconds;
            ViewData["DotNetMemoryUse"] = (Process.GetCurrentProcess().WorkingSet64 / 1024.0 / 1024.0);

            ViewData["FileTestTime"] = 0.0;
            ViewData["ComputePlusTime"] = 0.0;
            ViewData["ComputeSqrtTime"] = 0.0;

            if (String.Equals(id, "filetest"))
            {
                ViewData["FileTestTime"] = this.GetFileTestTime();
            }
            else if (String.Equals(id, "mathtest"))
            {
                ViewData["ComputePlusTime"] = this.GetMathPlusTime();
                ViewData["ComputeSqrtTime"] = this.GetMathSqrtTime();
            }

            return View();
        }

        /// <summary>
        /// 缓存列表页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult CacheList()
        {
            ViewBag.CacheCount = CacheManager.GetCount().ToString();

            return View(CacheManager.GetAll());
        }

        /// <summary>
        /// 缓存查看页面
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <returns>操作后的结果</returns>
        public ActionResult CacheView(String key)
        {
            ViewBag.CacheKey = key;

            return View(CacheManager.GetDetail(key));
        }

        /// <summary>
        /// 缓存删除
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CacheDelete(FormCollection form)
        {
            if (!String.IsNullOrEmpty(form["key"]))
            {
                CacheManager.Remove(form["key"].Split(','));
            }

            return RedirectToAction("CacheList", "System");
        }

        /// <summary>
        /// 系统配置页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        public ActionResult Config()
        {
            return View(ConfigurationFileManager.GetConfigPairList());
        }

        /// <summary>
        /// 系统配置保存
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ConfigSave(FormCollection form)
        {
            return ResultToMessagePage(ConfigurationFileManager.SaveToConfig, form, "Your have modified configuration successfully!");
        }

        private String GetSystemRunTime()
        {
            TimeSpan ts = TimeSpan.FromMilliseconds(Environment.TickCount);
            return String.Format("{0} 天 {1} 小时 {2} 分钟 {3}.{4} 秒", ts.Days.ToString(), ts.Hours.ToString(), ts.Minutes.ToString(), ts.Seconds.ToString(), ts.Milliseconds.ToString());
        }

        private Double GetFileTestTime()
        {
            try
            {
                String fileName = Path.Combine(ConfigurationManager.UploadDirectoryPath, "OJTEST.TMP");
                Stopwatch watch = new Stopwatch();
                watch.Start();

                for (Int32 m = 1; m <= 50; m++)
                {
                    if (System.IO.File.Exists(fileName) == true) System.IO.File.Delete(fileName);

                    System.IO.File.WriteAllText(fileName, "Hello World!");//文件创建
                    System.IO.File.ReadAllBytes(fileName);//文件读取
                    System.IO.File.Delete(fileName);//文件删除
                }


                return (Double)watch.ElapsedTicks * 1000 / Stopwatch.Frequency;
            }
            catch
            {
                return -1;
            }
        }

        private Double GetMathPlusTime()
        {
            Stopwatch watch = new Stopwatch();
            Int64 sum = 0;

            watch.Start();
            for (Int64 i = 1; i <= 5000000; i++)
            {
                sum += i;
            }
            watch.Stop();

            return (Double)watch.ElapsedTicks * 1000 / Stopwatch.Frequency;
        }

        private Double GetMathSqrtTime()
        {
            Stopwatch watch = new Stopwatch();
            Double num = 10E300;

            watch.Start();
            for (Int64 i = 1; i < 2000000; i++)
            {
                num = Math.Sqrt(num);
            }
            watch.Stop();

            return (Double)watch.ElapsedTicks * 1000 / Stopwatch.Frequency;
        }
    }
}