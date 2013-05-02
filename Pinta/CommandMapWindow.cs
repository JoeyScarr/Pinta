using Gtk;
using Pinta.Core;
using Pinta.Gui.Widgets;

namespace Pinta
{
	public class CommandMapWindow : Window
	{
		private HBox tools1;
		private HBox tools2;

		public CommandMapWindow (Window parent) : base ("Command Map")
		{
			TransientFor = parent;
			TypeHint = Gdk.WindowTypeHint.Dialog;
			Decorated = false;
			Opacity = 0.9;

			VBox vbox = new VBox ();
			Add (vbox);

			tools1 = new HBox ();
			vbox.Add (tools1);
			tools2 = new HBox ();
			vbox.Add (tools2);

			KeyPressEvent += HandleKeyPress;
			KeyReleaseEvent += HandleKeyRelease;
			FocusOutEvent += HandleFocusOut;

			PintaCore.Tools.ToolAdded += HandleToolAdded;
			PintaCore.Tools.ToolRemoved += HandleToolRemoved;
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

		private void HandleKeyPress (object sender, KeyPressEventArgs e)
		{
			// TODO
		}
	}
}
