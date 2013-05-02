using Gtk;
using Pinta.Core;
using Pinta.Gui.Widgets;

namespace Pinta
{
	public class CommandMapWindow : Window
	{
		private HBox tools;

		public CommandMapWindow (Window parent) : base ("Command Map")
		{
			TransientFor = parent;
			Decorated = false;
			Opacity = 0.9;
			Modal = true;
			SkipPagerHint = true;
			SkipTaskbarHint = true;

			tools = new HBox ();
			Add (tools);

			KeyReleaseEvent += HandleKeyRelease;
			FocusOutEvent += HandleFocusOut;

			PintaCore.Tools.ToolAdded += HandleToolAdded;
			PintaCore.Tools.ToolRemoved += HandleToolRemoved;
		}

		private void HandleToolAdded (object sender, ToolEventArgs e)
		{
			var button = new CommandMapButton (e.Tool);
			tools.PackStart (button);
		}

		private void HandleToolRemoved (object sender, ToolEventArgs e)
		{
			//tools.Remove (e.Tool.ToolItem);
		}

		void HandleFocusOut (object sender, FocusOutEventArgs e)
		{
			HideAll ();
		}

		void HandleKeyRelease (object sender, KeyReleaseEventArgs e)
		{
			if (e.Event.Key == Gdk.Key.Control_L || e.Event.Key == Gdk.Key.Control_R) {
				HideAll ();
			}
		}
	}
}
