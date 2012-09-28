using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BBEEGInteger.Log
{
    public static class LogError
    {
        /// <summary>
        /// Init the Log Singleton
        /// </summary>
        /// <param name="logPath">local path of the log file</param>
        public static void StartLog(string logPath)
        {
            MyLogListener myLog = MyLogListener.Instance(logPath);
            myLog.MaxLogSize = 500000; // 1Mo max
            myLog.WriteDateInfo = true;
            System.Diagnostics.Trace.Listeners.Add(myLog);
            System.Diagnostics.Trace.AutoFlush = true;
        }

        /// <summary>
        /// Log the error
        /// </summary>
        /// <param name="ex">the Exception</param>
        public static void Write(Exception ex)
        {
            LogError.StartLog(System.Configuration.ConfigurationSettings.AppSettings["LogFile"]);
            MyLogListener myLog = MyLogListener.instance;

            myLog.WriteLine(string.Format("{0} {1} {2}", ex.InnerException, ex.Message, ex.StackTrace));
        }

        /// <summary>
        /// Log the error
        /// </summary>
        /// <param name="ex">the Exception</param>
        public static void Write(string ex)
        {
            LogError.StartLog(System.Configuration.ConfigurationSettings.AppSettings["LogFile"]);
            MyLogListener myLog = MyLogListener.instance;

            myLog.WriteLine(ex);
        }
    }
}
