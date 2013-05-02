using Gtk;
using Pinta.Core;
using Pinta.Gui.Widgets;

namespace Pinta
{
	public class CommandMapWindow : Window
	{
		private HBox tools1;
		private HBox tools2;

		public CommandMapWindow () : base ("Command Map")
		{
			FitScreen ();
			SetPosition (WindowPosition.Center);

			Decorated = false;
			Opacity = 0.9;
			Modal = true;
			SkipPagerHint = true;
			SkipTaskbarHint = true;

			VBox vbox = new VBox ();
			Add (vbox);

			tools1 = new HBox ();
			vbox.Add (tools1);
			tools2 = new HBox ();
			vbox.Add (tools2);

			KeyReleaseEvent += HandleKeyRelease;
			FocusOutEvent += HandleFocusOut;

			PintaCore.Tools.ToolAdded += HandleToolAdded;
			PintaCore.Tools.ToolRemoved += HandleToolRemoved;
		}

		private void FitScreen ()
		{
			//SetDefaultSize (Screen.Width, Screen.Height);
		}

		private void HandleToolAdded (object sender, ToolEventArgs e)
		{
			var button = new CommandMapButton (e.Tool);

			if(tools1.Children.Length <= tools2.Children.Length) {
				tools1.PackStart (button);
			} else {
				tools2.PackStart (button);
			}
		}

		private void HandleToolRemoved (object sender, ToolEventArgs e)
		{
			//tools.Remove (e.Tool.ToolItem);
		}

		private void HandleFocusOut (object sender, FocusOutEventArgs e)
		{
			HideAll ();
		}

		private void HandleKeyRelease (object sender, KeyReleaseEventArgs e)
		{
			if (e.Event.Key == Gdk.Key.Control_L || e.Event.Key == Gdk.Key.Control_R) {
				HideAll ();
			}
		}
	}
}
