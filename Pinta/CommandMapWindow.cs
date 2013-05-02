using Gtk;
using Pinta.Core;

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

			KeyReleaseEvent += CommandMapWindow_KeyReleaseEvent;
			FocusOutEvent += CommandMapWindow_FocusOutEvent;

			PintaCore.Tools.ToolAdded += HandleToolAdded;
			PintaCore.Tools.ToolRemoved += HandleToolRemoved;
		}

		private void HandleToolAdded (object sender, ToolEventArgs e)
		{
			Gtk.Image icon = new Gtk.Image (PintaCore.Resources.GetIcon (e.Tool.Icon));
			Button tool = new Button (icon);
			tools.PackStart (tool);
		}

		private void HandleToolRemoved (object sender, ToolEventArgs e)
		{
			//tools.Remove (e.Tool.ToolItem);
		}

		[GLib.ConnectBefore]
		void CommandMapWindow_FocusOutEvent (object sender, FocusOutEventArgs e)
		{
			HideAll ();
		}

		[GLib.ConnectBefore]
		void CommandMapWindow_KeyReleaseEvent (object sender, KeyReleaseEventArgs e)
		{
			if (e.Event.Key == Gdk.Key.Control_L || e.Event.Key == Gdk.Key.Control_R) {
				HideAll ();
			}
		}
	}
}
