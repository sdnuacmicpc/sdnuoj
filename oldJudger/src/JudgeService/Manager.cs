using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;

using JudgeClient.Definition;

namespace JudgeClient.JudgeService
{
    public class Manager
    {
        private bool paused;
        private object timer_lock;

        private object _task_count_lock;
        private int min_task_count_in_pool, current_task_count_in_pool;
        private Dictionary<string, IJudger> judgers;

        private object _fetch_data_lock_dic_lock;
        private Dictionary<string, bool> _fetch_data_sign_dic;
        private Dictionary<string, object> _fetch_data_lock_dic;
        private object get_lock(string key)
        {
            lock (_fetch_data_lock_dic_lock)
            {
                if (!_fetch_data_lock_dic.ContainsKey(key))
                    _fetch_data_lock_dic.Add(key, new object());
                return _fetch_data_lock_dic[key];
            }
        }
        private object _judge_lock_dic_lock;
        private Dictionary<string, int> _judge_lock_dic;
        private int get_judging(string key)
        {
            lock (_judge_lock_dic_lock)
            {
                if (!_judge_lock_dic.ContainsKey(key))
                    _judge_lock_dic.Add(key, 0);
                return _judge_lock_dic[key];
            }
        }
        private void increase_judging(string key)
        {
            lock (_judge_lock_dic_lock)
            {
                if (!_judge_lock_dic.ContainsKey(key))
                    _judge_lock_dic.Add(key, 1);
                else
                    ++_judge_lock_dic[key];
            }
        }
        private void decrease_judging(string key)
        {
            lock (_judge_lock_dic_lock)
            {
                --_judge_lock_dic[key];
            }
        }

        public void Pause()
        {
            paused = true;
        }
        public void Continue()
        {
            paused = false;
        }

        public static readonly Manager Singleton = new Manager();

        public void ConfigureAndRun()
        {
            paused = false;
            timer_lock = new object();
            min_task_count_in_pool = Configuration.Singleton.MinTaskCountInPool;
            current_task_count_in_pool = 0;
            _fetch_data_lock_dic_lock = new object();
            _fetch_data_sign_dic = new Dictionary<string, bool>();
            _fetch_data_lock_dic = new Dictionary<string, object>();
            _judge_lock_dic_lock = new object();
            _judge_lock_dic = new Dictionary<string, int>();

            judgers = new Dictionary<string, IJudger>();
            foreach (var p in Configuration.Singleton.Judgers)
                judgers.Add(string.Format("{0}[{1}]", p.Language, p.Special), Factory.CreateAndConfigure<IJudger>(p));

            var _fetcher_index = 0;
            foreach (var p in Configuration.Singleton.Fetchers)
            {
                var fetcher_index = _fetcher_index++;
                var fetcher = Factory.CreateAndConfigure<IFetcher>(p);
                fetcher.ConfigureSupportedLanguages(Configuration.Singleton.Judgers);
                var timer = new System.Timers.Timer(p.FetchInterval);
                DateTime last_elapse_time = DateTime.MinValue;
                var dele = new ElapsedEventHandler((object sender, ElapsedEventArgs e) =>
                {
                    if (paused)
                        return;
                    lock(timer_lock)
                    {
                        var cur_time = DateTime.Now;
                        var interval = (cur_time - last_elapse_time).TotalMilliseconds;
                        if (interval < p.FetchInterval)
                            return;
                        last_elapse_time = cur_time;
                    }
                    lock (_task_count_lock)
                    {
                        if (current_task_count_in_pool < min_task_count_in_pool)
                        {
                            ExceptionManager.LogEvent("BeforeFetch");
                            var tasks = fetcher.FetchTask();
                            current_task_count_in_pool += tasks.Count;
                            foreach (var _t in tasks)
                            {
                                var task = _t;
                                ThreadPool.QueueUserWorkItem((object context) =>
                                {
                                    var key = string.Format("{0}_{1}", fetcher_index, task.Problem.Id);
                                    Result result = null;
                                    if (judgers.ContainsKey(task.LanguageAndSpecial))
                                    {
                                        bool data_valid;
                                        lock (get_lock(key))
                                        {
                                            while (_fetch_data_sign_dic.ContainsKey(key))
                                                Thread.Sleep(300);
                                            increase_judging(key);
                                            data_valid = task.Fetcher.DataAccessor.CheckValid(task.Problem);
                                            if (!data_valid)
                                                _fetch_data_sign_dic.Add(key, true);
                                        }
                                        if (!data_valid)
                                        {
                                            while (get_judging(key) > 1)
                                                Thread.Sleep(300);
                                            var fetch_suc = task.Fetcher.FetchData(task.Problem.Id);
                                            _fetch_data_sign_dic.Remove(key);
                                            if (fetch_suc)
                                                result = judgers[task.LanguageAndSpecial].Judge(task);
                                            else
                                                result = new Result() { ResultCode = ResultCode.UnJudgable, Task = task, Detail = "Could not fetch data." };
                                        }
                                        else
                                            result = judgers[task.LanguageAndSpecial].Judge(task);
                                        decrease_judging(key);
                                    }
                                    else
                                        result = new Result() { ResultCode = ResultCode.UnJudgable, Task = task, Detail = "Unsupportted language." };
                                    result.Submit();
                                    lock (_task_count_lock)
                                    {
                                        --current_task_count_in_pool;
                                    }
                                });
                            }
                        }
                    }
                });
                timer.Elapsed += dele;
                timer.Start();
            }
        }

        private Manager()
        {
            _task_count_lock = new object();
        }
    }
}
