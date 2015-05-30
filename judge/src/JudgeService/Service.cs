using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using JudgeClient.Definition;

namespace JudgeClient.JudgeService
{
    public partial class Service : ServiceBase
    {
        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
            {
                #if DEBUG
                #else

                ExceptionManager.Log(new UnhandledException("UnhandledException occured.", e.ExceptionObject as Exception));
                
                #endif
            };
            Configuration.Reload();
            System.Threading.ThreadPool.QueueUserWorkItem((object context) =>
            {
                Manager.Singleton.ConfigureAndRun();
            });
            while (running)
            {
                Thread.Sleep(500);
                ExceptionManager.FlushIfTime();
            }
        }

        #if DEBUG

        internal void Start()
        {
            OnStart(new string[] { });
        }

        #endif

        protected bool running = true;

        protected override void OnStop()
        {
            ExceptionManager.Flush();
            running = false;
            base.OnStop();
        }

        protected override void OnShutdown()
        {
            ExceptionManager.Flush();
            running = false;
            base.OnShutdown();
        }

        protected override void OnPause()
        {
            Manager.Singleton.Pause();
            base.OnPause();
        }

        protected override void OnContinue()
        {
            Manager.Singleton.Continue();
            base.OnContinue();
        }
    }
}
