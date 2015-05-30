using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JudgeClient.Definition
{
    public class ExceptionManager
    {
        private static object _log_lock = new object();

        private static object _last_exception_lock = new object();
        private static Exception last_exception;
        private static DateTime first_occur_time, last_occur_time;
        private static int repeat_count = 0;

        private static void write_file(Exception ex)
        {
            StringBuilder message = new StringBuilder();

            first_occur_time = last_occur_time = DateTime.Now;
            message.AppendFormat("--------------------------------------------------------------------------------\r\n");
            message.AppendFormat("First occurs at {0}\r\n\r\n", first_occur_time);
            while (ex != null)
            {
                message.AppendFormat("[{0}]\r\n", ex.GetType().FullName);
                message.AppendFormat("Message: {0}\r\n", ex.Message);
                if (!string.IsNullOrEmpty(ex.StackTrace))
                    message.AppendFormat("Stack Trace:\r\n{0}\r\n", ex.StackTrace);
                ex = ex.InnerException;
                if (ex != null)
                    message.Append("---> Caused by: ");
            }
            message.Append("\r\n");

            lock (_log_lock)
            {
                File.AppendAllText(Configuration.Singleton.LogFilePath, message.ToString());
            }
        }

        private static void write_repeat()
        {
            lock (_log_lock)
            {
                File.AppendAllText(Configuration.Singleton.LogFilePath, string.Format("Repeat at {0}\r\n", DateTime.Now));
            }
        }

        private static bool deal_same_with_last(Exception ex)
        {
            lock (_last_exception_lock)
            {
                var cur = ex;
                var last = last_exception;
                while (cur != null && last != null)
                {
                    if (cur.GetType() != last.GetType() || cur.Message != last.Message || cur.StackTrace != last.StackTrace)
                        break;
                    cur = cur.InnerException;
                    last = last.InnerException;
                }
                if (cur == null && last == null)
                {
                    ++repeat_count;
                    last_occur_time = DateTime.Now;
                    return true;
                }
                else
                {
                    last_exception = ex;
                    return false;
                }
            }
        }

        public static void Flush()
        {
            lock (_log_lock)
            {
                if (repeat_count > 0)
                {
                    File.AppendAllText(Configuration.Singleton.LogFilePath,
                        string.Format("---- Repeat {0} times from {1} to {2}\r\n\r\n", repeat_count + 1, first_occur_time, last_occur_time));
                    repeat_count = 0;
                    first_occur_time = last_occur_time = DateTime.Now;
                }
            }
        }

        public static void FlushIfTime()
        {
            if ((DateTime.Now - last_occur_time).TotalDays >= 1)
                Flush();
        }

        public static void Log(Exception ex)
        {
            if (Configuration.Singleton.WriteLog)
            {
                if (!deal_same_with_last(ex))
                {
                    Flush();
                    write_file(ex);
                }
                else
                {
                    write_repeat();
                }
            }
        }

        public static void LogEvent(string ev)
        {
            lock (_log_lock)
            {
                // File.AppendAllText(Configuration.Singleton.LogFilePath,
                //     string.Format(ev + "\r\n"));
            }
        }

        public static void Throw(Exception ex)
        {
            #if DEBUG

            Log(ex);
            //throw ex;
            
            #else
            
            Log(ex);
            
            #endif
        }
    }
}
