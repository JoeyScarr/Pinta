using Gtk;
using Pinta.Core;
using Pinta.Gui.Widgets;

namespace Pinta
{
	public class CommandMapWindow : Window
	{
		private HBox tools1;
		private HBox tools2;

		public CommandMapWindow () : base (WindowType.Popup)
		{
			TransientFor = PintaCore.Chrome.MainWindow;
			WindowPosition = WindowPosition.CenterOnParent;
			Opacity = 0.9;

			VBox vbox = new VBox ();
			Add (vbox);

			// Add the main toolbar.
			var toolbar = new Toolbar ();
			PintaCore.Actions.CreateToolBar (toolbar);
			vbox.Add (toolbar);

			// Add two rows for tools.
			tools1 = new HBox ();
			vbox.Add (tools1);
			tools2 = new HBox ();
			vbox.Add (tools2);

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
			foreach (CommandMapButton button in tools1) {
				if (button.Tool == e.Tool) {
					tools1.Remove(button);
					return;
				}
			}

			foreach (CommandMapButton button in tools2) {
				if (button.Tool == e.Tool) {
					tools2.Remove(button);
					return;
				}
			}
		}
	}
}
