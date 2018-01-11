using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.IO;
using JudgeClient.Definition;

namespace JudgeClient.Judger
{
    public class DefaultJudger : IJudger
    {
        private int MonitorInterval;
        private int BufferSize;

        private string wrap_judge_path(string ori, string JudgeTempPath)
        {
            if (string.IsNullOrEmpty(ori))
                return ori;
            return ori.Replace("%judge_path%", JudgeTempPath);
        }

        private string remove(string input, params string[] patterns)
        {
            foreach (string pattern in patterns)
            {
                StringBuilder res = new StringBuilder();
                int index = 0, length = pattern.Length;
                while (true)
                {
                    int cur_index = input.IndexOf(pattern, index, StringComparison.CurrentCultureIgnoreCase);
                    if (cur_index == -1)
                    {
                        res.Append(input.Substring(index, input.Length - index));
                        break;
                    }
                    res.Append(input.Substring(index, cur_index - index));
                    index = cur_index + length;
                }
                input = res.ToString();
            }
            return input;
        }

        private string grep_directory(string output, string JudgeTempPath)
        {
            if (string.IsNullOrEmpty(_profile.CompilerWorkingDirectory))
                return remove(output, JudgeTempPath);
            else
                return remove(output, JudgeTempPath, _profile.CompilerWorkingDirectory);
        }

        private bool Compile(Task task, Result res, string JudgeTempPath)
        {
            if (CodeChecker.CheckUnsafeCode(task) == false)
            {
                res.ResultCode = ResultCode.CompileError;
                res.Detail = "Include unsafe code.你的代码中含有恶意代码, 因此服务器拒绝判定你的提交, 如有误报请联系QQ:961523404";
                return false;
            }

            Process p = new Process();
            p.StartInfo.FileName = string.Format("\"{0}\"", wrap_judge_path(_profile.CompilerPath, JudgeTempPath));
            p.StartInfo.Arguments = wrap_judge_path(_profile.CompileParameters, JudgeTempPath);
            p.StartInfo.WorkingDirectory = wrap_judge_path(_profile.CompilerWorkingDirectory, JudgeTempPath);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;

            string error_output = null;
            bool error_output_readed = false;
            string standard_output = null;
            bool standard_output_readed = false;

            ThreadPool.QueueUserWorkItem((object context) =>
            {
                while (true)
                {
                    try
                    {
                        error_output = EncodingUtils.DetectAndReadToEndAndDispose(p.StandardError.BaseStream);
                        error_output_readed = true;
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(100);
                    }
                }
            });

            ThreadPool.QueueUserWorkItem((object context) =>
            {
                while (true)
                {
                    try
                    {
                        standard_output = EncodingUtils.DetectAndReadToEndAndDispose(p.StandardOutput.BaseStream);
                        standard_output_readed = true;
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(100);
                    }
                }
            });

            p.Start();
            using (var usage = AffinityManager.GetUsage(p.ProcessorAffinity))
            {
                try
                {
                    p.ProcessorAffinity = usage.Affinity;
                }
                catch { }
                if (!p.WaitForExit(_profile.CompilerWaitTime))
                {
                    res.ResultCode = ResultCode.UnJudgable;
                    res.Detail = "Compile time too long.";
                    return false;
                }
                else if (p.ExitCode != 0)
                {
                    while (!error_output_readed || !standard_output_readed)
                        Thread.Sleep(100);
                    res.ResultCode = ResultCode.CompileError;
                    res.Detail = grep_directory(error_output, JudgeTempPath);
                    return false;
                }
                return true;
            }
        }
        private bool Run(Task task, TestData data, Result res, string JudgeTempPath, char[] Buffer)
        {
            Process run = new Process();
            run.StartInfo.FileName = wrap_judge_path(_profile.RunnerFileName, JudgeTempPath);
            run.StartInfo.Arguments = wrap_judge_path(_profile.RunnerParameters, JudgeTempPath);
            run.StartInfo.WorkingDirectory = wrap_judge_path(_profile.RunnerWorkingDirectory, JudgeTempPath);
            run.StartInfo.UseShellExecute = false;
            run.StartInfo.RedirectStandardInput = true;
            run.StartInfo.RedirectStandardOutput = true;
            run.StartInfo.RedirectStandardError = true;
            run.StartInfo.CreateNoWindow = true;

            //Monitor to measure memory and time usage and kill when memory or time limit exceed.
            int MemoryCost = 0, TimeCost = 0;
            var monitor = new System.Timers.Timer(MonitorInterval);
            monitor.Elapsed += (object sender, System.Timers.ElapsedEventArgs args) =>
            {
                try
                {
                    if (run.HasExited)
                    {
                        monitor.Stop();
                        monitor.Dispose();
                    }
                    else
                    {
                        try
                        {
                            var c_memory = (int)(run.PeakPagedMemorySize64 / 1024);
                            if (c_memory > MemoryCost)
                            {
                                MemoryCost = c_memory;
                                if (MemoryCost > task.MemoryLimit)
                                {
                                    res.ResultCode = ResultCode.MemoryLimitExceeded;
                                    res.MemoryCost = MemoryCost;
                                    try
                                    {
                                        run.Kill();
                                    }
                                    catch { }
                                }
                            }
                            TimeCost = (int)((DateTime.Now - run.StartTime).TotalMilliseconds / _profile.TimeLimitScale);
                            //TimeCost = (int)(run.TotalProcessorTime.TotalMilliseconds / _profile.TimeLimitScale);
                            if (TimeCost > task.TimeLimit)
                            {
                                res.ResultCode = ResultCode.TimeLimitExceeded;
                                res.TimeCost = task.TimeLimit;
                                try
                                {
                                    run.Kill();
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }
                catch (InvalidOperationException)
                { }
            };
            //monitor.Start();

            string error_output = null;
            bool error_output_readed = false;
            string standard_output = null;
            bool standard_output_readed = false;
            //int error_pro_time = 0;
            //int output_pro_time = 0;

            ThreadPool.QueueUserWorkItem((object context) =>
            {
                while (true)
                {
                    try
                    {
                        //var start_time = run.TotalProcessorTime.TotalMilliseconds;
                        //Stopwatch sw = Stopwatch.StartNew();
                        error_output = EncodingUtils.DetectAndReadToEndAndDispose(run.StandardError.BaseStream);
                        //sw.Stop();
                        //error_pro_time = (int)(sw.ElapsedMilliseconds - run.TotalProcessorTime.TotalMilliseconds + start_time);
                        error_output_readed = true;
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(100);
                    }
                }
            });

            ThreadPool.QueueUserWorkItem((object context) =>
            {
                while (true)
                {
                    try
                    {
                        //var start_time = run.TotalProcessorTime.TotalMilliseconds;
                        //Stopwatch sw = Stopwatch.StartNew();
                        standard_output = EncodingUtils.DetectAndReadToEndAndDispose(run.StandardOutput.BaseStream);
                        //sw.Stop();
                        //output_pro_time = (int)(sw.ElapsedMilliseconds - run.TotalProcessorTime.TotalMilliseconds + start_time);
                        standard_output_readed = true;
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(100);
                    }
                }
            });
            
            run.Start();
            monitor.Start();
            using (var usage = AffinityManager.GetUsage(run.ProcessorAffinity))
            {
                try
                {
                    try
                    {
                        run.ProcessorAffinity = usage.Affinity;
                        run.PriorityClass = ProcessPriorityClass.RealTime;
                        MemoryCost = (int)(run.PeakPagedMemorySize64 / 1024);
                    }
                    catch { }
                    run.StandardInput.WriteLine(data.Input);
                    run.StandardInput.Close();
                    /*run.StandardInput.AutoFlush = true;
                    int part_len = BufferSize, start_index, end_index;
                    for (start_index = 0, end_index = part_len; end_index < data.Input.Length; start_index += part_len, end_index += part_len)
                        run.StandardInput.Write(data.Input.Substring(start_index, part_len));
                    run.StandardInput.WriteLine(data.Input.Substring(start_index, data.Input.Length - start_index));*/
                }
                catch (Exception ex)
                {
                    if (res.ResultCode != ResultCode.MemoryLimitExceeded && res.ResultCode != ResultCode.TimeLimitExceeded)
                        throw ex;
                }

                if (!run.WaitForExit((int)(task.TimeLimit * _profile.TimeLimitScale)))
                {
                    if (res.ResultCode == ResultCode.MemoryLimitExceeded || res.ResultCode == ResultCode.TimeLimitExceeded)
                        return false;
                    try
                    {
                        if (run.ExitCode != 0)
                        {
                            res.ResultCode = ResultCode.RuntimeError;
                            res.Detail = grep_directory(
                                EncodingUtils.DetectAndReadToEndAndDispose(run.StandardError.BaseStream),
                                JudgeTempPath
                            );
                            return false;
                        }
                    }
                    catch { }
                    try
                    {
                        run.Kill();
                    }
                    catch { }
                    res.ResultCode = ResultCode.TimeLimitExceeded;
                    res.TimeCost = task.TimeLimit;
                    return false;
                }

                if (res.ResultCode == ResultCode.MemoryLimitExceeded || res.ResultCode == ResultCode.TimeLimitExceeded)
                    return false;

                try
                {
                    MemoryCost = (int)(run.PeakPagedMemorySize64 / 1024);
                }
                catch { }
                res.MemoryCost = MemoryCost;
                if (res.MemoryCost > task.MemoryLimit)
                {
                    res.ResultCode = ResultCode.MemoryLimitExceeded;
                    return false;
                }

                try
                {
                    TimeCost = (int)((run.ExitTime - run.StartTime).TotalMilliseconds / _profile.TimeLimitScale);
                    //TimeCost = (int)(run.TotalProcessorTime.TotalMilliseconds / _profile.TimeLimitScale);
                }
                catch { }
                if (TimeCost > res.TimeCost)
                {
                    res.TimeCost = TimeCost;
                    if (TimeCost > task.TimeLimit)
                    {
                        res.ResultCode = ResultCode.TimeLimitExceeded;
                        res.TimeCost = task.TimeLimit;
                        return false;
                    }
                }

                while (!error_output_readed || !standard_output_readed)
                    Thread.Sleep(100);

                if (run.ExitCode != 0)
                {
                    res.ResultCode = ResultCode.RuntimeError;
                    res.Detail = grep_directory(error_output, JudgeTempPath);
                    return false;
                }

                if (standard_output.Length > _profile.OutputLimit)
                {
                    res.ResultCode = ResultCode.OutputLimitExcceeded;
                    return false;
                }
                var output = standard_output.Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd('\n');
                var e_output = data.Output.Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd('\n');
                if (output == e_output)
                    return true;
                else
                {
                    var output_l = output.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    var e_output_l = e_output.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (output_l.Length == e_output_l.Length)
                    {
                        int i;
                        for (i = 0; i < output_l.Length; ++i)
                            if (output_l[i].TrimEnd() != e_output_l[i].TrimEnd())
                                break;
                        if (i == output_l.Length)
                            res.ResultCode = ResultCode.PresentationError;
                        else
                            res.ResultCode = ResultCode.WrongAnswer;
                    }
                    else
                        res.ResultCode = ResultCode.WrongAnswer;
                    return false;
                }
            }
        }

        private void DeleteTempDirectory(string JudgeTempPath)
        {
            var _del_timer = new System.Timers.Timer(200);
            _del_timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                try
                {
                    Directory.Delete(JudgeTempPath, true);
                    _del_timer.Stop();
                    _del_timer.Dispose();
                }
                catch { }
            };
            _del_timer.Start();
        }

        private List<Guid> list = new List<Guid>();

        public Result Judge(Task task)
        {
            var JudgeTempPath = string.Format("{0}{1}\\", _judge_path, Guid.NewGuid());
            Directory.CreateDirectory(JudgeTempPath);
            File.WriteAllText(JudgeTempPath + _profile.SourceCodeFileName, string.Format("{2}\r\n/* Task Id: {0}\r\n   Problem Id: {1} */\r\n", task.Id, task.Problem.Id, task.SourceCode));

            MonitorInterval = 30;
            BufferSize = 10000;
            var Buffer = new char[BufferSize];

            var res = new Result();
            res.ResultCode = ResultCode.Accepted;
            res.TimeCost = 0;
            res.Task = task;

            try
            {
                if (Compile(task, res, JudgeTempPath))
                {
                    int pass_count = 0, total_count = task.Fetcher.DataAccessor.GetDataCount(task.Problem);
                    var e = task.Fetcher.DataAccessor.GetDataEnumerator(task.Problem);
                    while (e.MoveNext())
                    {
                        if (Run(task, e.Current, res, JudgeTempPath, Buffer))
                            ++pass_count;
                        else
                            break;
                    }
                    if (total_count != 0)
                        res.PassRate = (double)pass_count / total_count;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Throw(new JudgerException("Judger error.", ex));
                if (File.Exists(string.Format("\"{0}\"", wrap_judge_path(_profile.CompilerPath, JudgeTempPath))))
                {
                    res.ResultCode = ResultCode.UnJudgable;
                    res.Detail = "Judger error.";
                }
                else//编译器路径不存在
                {
                    res.ResultCode = ResultCode.CompileError;
                    res.Detail = "Compiler not found";
                }
            }
            DeleteTempDirectory(JudgeTempPath);
            return res;
        }

        private ProcessorAffinityManager AffinityManager;

        protected JudgerProfile _profile;
        protected string _judge_path;

        void IModule.Configure(IProfile Profile)
        {
            _profile = Profile as JudgerProfile;
            if (_profile == null)
                ExceptionManager.Throw(new CreateAndConfigureModuleException("Configure DefaultJudger failed: Profile is null.", null));
            try
            {
                _judge_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _profile.JudgeDirectory);
                if (!_judge_path.EndsWith("\\") && !_judge_path.EndsWith("/"))
                    _judge_path += "\\";
                try
                {
                    Directory.Delete(_judge_path, true);
                }
                catch { }
                Directory.CreateDirectory(_judge_path);

                AffinityManager = new ProcessorAffinityManager();
            }
            catch (Exception ex)
            {
                ExceptionManager.Throw(new CreateAndConfigureModuleException("Configure DefaultJudger failed.", ex));
            }
        }
    }
}
