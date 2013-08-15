using System;
using System.IO;
using System.Diagnostics;

namespace Pinta.Core
{
    public static class Logger
    {
        private const string log_dir = "Logs";
        private static string log_file;
        private static Stopwatch stopwatch;

        public static void Initialize()
        {
            Directory.CreateDirectory(log_dir);
            log_file = Path.Combine(log_dir, "Command Map - " + DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss") + ".txt");

            stopwatch = new Stopwatch();
            Log("New command map Pinta session started");
            stopwatch.Start();
        }

        public static void Log (string format, params object[] args)
        {
            string output = stopwatch.ElapsedMilliseconds + ": " + string.Format(format, args);
            Console.WriteLine (output);

            using (var log = new StreamWriter (log_file, true))
            {
                log.WriteLine (output);
            }
        }
    }
}
