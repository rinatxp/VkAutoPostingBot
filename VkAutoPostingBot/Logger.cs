using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkAutoPostingBot
{
    public static class Logger
    {
        private static string path;
        private static object lockObj = new object();

        public static void Init()
        {
            path = DateTime.Now.ToString("dd.MM.yy_HH.mm.ss") + "_all.txt";
            using (File.Create(path)) { }
        }

        public static void Log(string log)
        {
            lock (lockObj)
            {
                DateTime now = DateTime.Now;

                string hour = (now.Hour < 10 ? "0" : "") + now.Hour,
                        minute = (now.Minute < 10 ? "0" : "") + now.Minute,
                        second = (now.Second < 10 ? "0" : "") + now.Second;

                string logWithTime = $"[{hour}:{minute}:{second}] {log}";

                using (var stream = new StreamWriter(path, true))
                {
                    stream.WriteLine(logWithTime);
                }
            }
        }
    }
}
