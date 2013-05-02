using Gtk;

namespace Pinta
{
	public class CommandMapWindow : Window
	{
		public CommandMapWindow () : base ("Command Map")
		{
			SetDefaultSize (200, 200);
			//Decorated = false;

            Label label = new Label ("Hello");
            Add (label);

            KeyReleaseEvent += CommandMapWindow_KeyReleaseEvent;
		}

        [GLib.ConnectBefore]
        void CommandMapWindow_KeyReleaseEvent (object o, KeyReleaseEventArgs e)
        {
            if (e.Event.Key == Gdk.Key.Control_L || e.Event.Key == Gdk.Key.Control_R) {
                System.Console.WriteLine ("Ctrl released! (on cmd_map)");
                HideAll ();
            }
        }
    }
}
