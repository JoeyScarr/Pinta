using System;
using System.IO;

namespace Pinta.Core
{
    public static class Logger
    {
        private const string log_file = "cmd_map_log.txt";

        public static void Log (string format, params object[] args)
        {
            string output = DateTime.Now + ": " + string.Format(format, args);
            Console.WriteLine (output);

            using (var log = new StreamWriter (log_file, true))
            {
                log.WriteLine (output);
            }
        }
    }
}
