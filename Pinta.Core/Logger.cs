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

        public static void AddMenuLogging(MenuBar menu)
        {
            AddMenuLogging(menu.Children, null);
        }

        public static void AddMenuLogging(Widget[] menu_items, string path)
        {
            foreach (Widget child in menu_items)
            {
                if (child is SeparatorMenuItem)
                    continue;

                MenuItem m = (MenuItem)child;
                string name = m.GetText();
                string full_name;

                if (path == null)
                    full_name = name;
                else
                    full_name = path + " | " + name;

                m.ButtonPressEvent += delegate(object o, ButtonPressEventArgs e)
                {
                    Log("Menu item \"" + full_name + "\" clicked");
                };

                Menu submenu = m.Submenu as Menu;
                if (submenu != null)
                {
                    submenu.Shown += delegate(object o, EventArgs e)
                    {
                        Log("Menu \"" + full_name + "\" opened");
                    };

                    submenu.Hidden += delegate(object o, EventArgs e)
                    {
                        Log("Menu \"" + full_name + "\" closed");
                    };

                    AddMenuLogging(submenu.Children, full_name);
                }
            }
        }

        public static void AddToolLogging(ToolButton item)
        {
            item.Clicked += delegate(object o, EventArgs e)
            {
                Log("Tool button \"" + item.Label + "\" clicked");
            };
        }
    }
}
