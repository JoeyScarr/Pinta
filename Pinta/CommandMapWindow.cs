using Gtk;

namespace Pinta
{
	public class CommandMapWindow : Window
	{
		public CommandMapWindow (Window parent) : base ("Command Map")
		{
			TransientFor = parent;
			Decorated = false;
			Opacity = 0.9;
			Modal = true;
			SkipPagerHint = true;
			SkipTaskbarHint = true;

			VBox box = new VBox ();
			Add (box);

			Frame frame = new Frame ("Tools");
			box.PackEnd (frame);

			Button button = new Button ("Hello");
			frame.Add (button);

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
