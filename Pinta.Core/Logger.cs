using System;
using System.IO;

namespace Pinta.Core
{
    public static class Logger
    {
        private const string log_file = "cmd_map_log.txt";

        public static void Log (string format, params object[] args)
        {
            Console.WriteLine ("{0}: {1}", DateTime.Now, string.Format (format, args));

            using (var log = new StreamWriter (log_file, true))
            {
                log.WriteLine ("{0}: {1}", DateTime.Now, string.Format (format, args));
            }
        }
    }
}
