using Gtk;

namespace Pinta
{
	public class CommandMapWindow : Window
	{
		public CommandMapWindow () : base ("Command Map")
		{
			WindowPosition = WindowPosition.Center;
			SetDefaultSize (200, 200);
			Decorated = false;
			Opacity = 0.9;

            Label label = new Label ("Hello");
            Add (label);

            KeyReleaseEvent += CommandMapWindow_KeyReleaseEvent;
            FocusOutEvent += CommandMapWindow_FocusOutEvent;
		}

        [GLib.ConnectBefore]
        void CommandMapWindow_FocusOutEvent (object o, FocusOutEventArgs e)
        {
            System.Console.WriteLine ("Lost focus.");
            HideAll ();
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
