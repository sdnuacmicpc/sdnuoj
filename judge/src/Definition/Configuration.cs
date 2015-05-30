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
                AuthenticationURL = "http://www.acmicpc.sdnu.edu.cn/Judge.ashx?action=login",
                Username = "httpjudge",
                Password = "iloveeclan",
                TaskFetchURL = "http://www.acmicpc.sdnu.edu.cn/Judge.ashx?action=getpending",
                FetchInterval = 5000,
                FetchTimeout = 5000,
                DataFetchURL = "http://www.acmicpc.sdnu.edu.cn/Judge.ashx?action=getproblem",
                ResultSubmitURL = "http://www.acmicpc.sdnu.edu.cn/Judge.ashx?action=updatestatus",
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

            string gcc_prefix = "";

            #if DEBUG

            gcc_prefix = @"D:\lib\limited_mingw\build_for_no_fstream";

            #else

            gcc_prefix = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"compiler\gcc");

            #endif

            string java_prefix = string.Empty;
            try
            {
                java_prefix = Environment.GetEnvironmentVariable("JAVA_HOME").EnsureTailSplash() + "bin\\";
            }
            catch { }

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
                JudgeDirectory = ".gcc_judge"
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
                JudgeDirectory = ".gpp_judge"
            });
            if (!string.IsNullOrEmpty(java_prefix))
            {
                Judgers.Add(new JudgerProfile()
                {
                    Language = "java",
                    Type = "JudgeClient.Judger.DefaultJudger, JudgeClient.Judger",
                    SourceCodeFileName = "Main.java",
                    CompilerPath = java_prefix + "javac.exe",
                    CompileParameters = "\"%judge_path%Main.java\"",
                    RunnerFileName = java_prefix + "java.exe",
                    RunnerWorkingDirectory = "%judge_path%",
                    RunnerParameters = "Main",
                    TimeLimitScale = 1.5,
                    CompilerWaitTime = 20000,
                    OutputLimit = 20971520,
                    JudgeDirectory = ".java_judge"
                });
            }
            LogFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
            WriteLog = true;
        }
    }
}
