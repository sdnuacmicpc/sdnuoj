using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Burst;
using Burst.Json;

namespace JudgeClient.Definition
{
    [LocalConfiguration("config.json", true)]
    public class Configuration : LocalConfiguration<Configuration>
    {
        public List<FetcherProfile> Fetchers { get; set; }

        public int TaskCountPerFetch { get; set; }

        public List<JudgerProfile> Judgers { get; set; }

        public int MinTaskCountInPool { get; set; }

        public string LogFilePath { get; set; }

        public bool WriteLog { get; set; }

        private JsonSerializer json_serializer = new JsonSerializer() {
            FormatOption = JsonFormatOption.Readable
        };
        protected override JsonSerializer JsonSerializer
        {
            get
            {
                return json_serializer;
            }
        }

        protected override void InitializeDefault()
        {
            Fetchers = new List<FetcherProfile>();
            ///*
            Fetchers.Add(new FetcherProfile()
            {
                Type = "JudgeClient.SDNU.SDNUFetcher, JudgeClient.Fetcher",
                AuthenticationURL = "http://127.0.0.1/judge/login",
                Username = "httpjudge",
                Password = "12345678",
                TaskFetchURL = "http://127.0.0.1/judge/getpending",
                ResultSubmitURL = "http://127.0.0.1/judge/updatestatus",
                DataFetchURL = "http://127.0.0.1/judge/getproblem",
                FetchInterval = 3000,
                FetchTimeout = 5000,

                DataAccessorProfile = new DataAccessorProfile()
                {
                    Type = "JudgeClient.SDNU.SDNUDataAccessor, JudgeClient.Fetcher",
                    TestDataSavePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testdata")
                }
            });
            /*
            Fetchers.Add(new FetcherProfile() {
                Type = "JudgeClient.Fetcher.TestFetcher, JudgeClient.Fetcher",
                FetchInterval = 2000
            });
             */

            string gcc_prefix = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Compilers\gcc");

            string java_prefix = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Compilers\jdk\bin\");
            try
            {
                string jdkPath = Environment.GetEnvironmentVariable("JAVA_HOME").EnsureTailSplash();
                System.Diagnostics.Debug.WriteLine(jdkPath);
                if (!String.IsNullOrEmpty(jdkPath) && jdkPath != "\\") 
                {
                    java_prefix = jdkPath + "bin\\";
                }
            }
            catch { }

            string python_prefix = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Compilers\python");

            MinTaskCountInPool = 5;
            TaskCountPerFetch = 5;
            Judgers = new List<JudgerProfile>();
            Judgers.Add(new JudgerProfile()
            {
                Language = "gcc",
                Type = "JudgeClient.Judger.DefaultJudger, JudgeClient.Judger",
                SourceCodeFileName = "src.c",
                CompilerPath = gcc_prefix + @"\bin\gcc.exe",
                CompileParameters = "\"%judge_path%src.c\" -o \"%judge_path%test.exe\"",
                CompilerWorkingDirectory = gcc_prefix + @"\bin",
                CompilerWaitTime = 20000,
                RunnerFileName = "\"%judge_path%test.exe\"",
                RunnerWorkingDirectory = gcc_prefix + @"\bin",
                TimeLimitScale = 1,
                OutputLimit = 20971520,
                JudgeDirectory = ".gcc_judge",
                SeeNoCompilerAsCompileError = true,
                NeedCompile = true
            });
            Judgers.Add(new JudgerProfile()
            {
                Language = "g++",
                Type = "JudgeClient.Judger.DefaultJudger, JudgeClient.Judger",
                SourceCodeFileName = "src.cpp",
                CompilerPath = gcc_prefix + @"\bin\g++.exe",
                CompileParameters = "\"%judge_path%src.cpp\" -o \"%judge_path%test.exe\"",
                CompilerWorkingDirectory = gcc_prefix + @"\bin",
                CompilerWaitTime = 20000,
                RunnerFileName = "\"%judge_path%test.exe\"",
                RunnerWorkingDirectory = gcc_prefix + @"\bin",
                TimeLimitScale = 1.5,
                OutputLimit = 20971520,
                JudgeDirectory = ".gpp_judge",
                SeeNoCompilerAsCompileError = true,
                NeedCompile = true
            });
            Judgers.Add(new JudgerProfile()
            {
                Language = "java",
                Type = "JudgeClient.Judger.DefaultJudger, JudgeClient.Judger",
                SourceCodeFileName = "Main.java",
                CompilerPath = java_prefix + "javac.exe",
                CompileParameters = "-encoding utf-8 \"%judge_path%Main.java\"",
                RunnerFileName = java_prefix + "java.exe",
                RunnerWorkingDirectory = "%judge_path%",
                RunnerParameters = "Main",
                TimeLimitScale = 1.5,
                CompilerWaitTime = 20000,
                OutputLimit = 20971520,
                JudgeDirectory = ".java_judge",
                SeeNoCompilerAsCompileError = true,
                NeedCompile = true
            });
            Judgers.Add(new JudgerProfile()
            {
                Language = "python",
                Type = "JudgeClient.Judger.DefaultJudger, JudgeClient.Judger",
                SourceCodeFileName = "src.py",
                RunnerFileName = python_prefix + "\\python.exe",
                RunnerWorkingDirectory = "%judge_path%",
                RunnerParameters = "\"%judge_path%src.py\"",
                TimeLimitScale = 1.5,
                OutputLimit = 20971520,
                JudgeDirectory = ".py_judge",
                SeeNoCompilerAsCompileError = true,
                NeedCompile = false
            });
            LogFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
            WriteLog = true;
        }
    }
}
