using System;
using System.IO;
using System.Diagnostics;
using Gtk;

namespace Pinta.Core
{
    public static class Logger
    {
        private const string log_dir = "Logs";
        private static string log_file;
        private static Stopwatch stopwatch;

        public static void Initialize(int sid, int block)
        {
            Directory.CreateDirectory(log_dir);
            log_file = Path.Combine(log_dir, sid + " - Std " + block + " - " + DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss") + ".txt");

            stopwatch = new Stopwatch();
            Log("New standard UI Pinta session started. Subject ID: " + sid + ". Block Number: " + block);
            stopwatch.Start();
        }

        public static void Log(string format, params object[] args)
        {
            string output = stopwatch.ElapsedMilliseconds + ": " + string.Format(format, args);
            Console.WriteLine(output);

            using (var log = new StreamWriter(log_file, true))
            {
                log.WriteLine(output);
            }
        }

        public static void AddMenuLogging(MenuItem menu_item)
        {
            menu_item.Submenu.Shown += delegate(object o, EventArgs e) {
                Logger.Log("\"" + menu_item.GetText() + "\" menu opened.");
            };

            menu_item.Submenu.Hidden += delegate(object o, EventArgs e)
            {
                Logger.Log("\"" + menu_item.GetText() + "\" menu closed.");
            };
        }

        public static void AddMenuItemLogging(string category, params Gtk.Action[] actions)
        {
            foreach (var action in actions) {
                string name = action.Label;

                action.Activated += delegate(object o, EventArgs e) {
                    Logger.Log("Selected \"" + name + "\" in \"" + category + "\"");
                };
            }
        }
    }
}
