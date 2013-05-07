using Gtk;
using Pinta.Core;
using Pinta.Gui.Widgets;

namespace Pinta
{
	public class CommandMapWindow : Window
	{
		private HBox tools1;
		private HBox tools2;

		public HBox AdjustmentsCommandMapBox { get; private set; }
		public VBox EffectsCommandMapBox { get; private set; }

		public CommandMapWindow () : base (WindowType.Popup)
		{
			TransientFor = PintaCore.Chrome.MainWindow;
			WindowPosition = WindowPosition.CenterOnParent;
			Opacity = 0.9;

			VBox vbox = new VBox ();
			Add (vbox);

			// Add the main toolbars.
			HBox main1 = new HBox ();
			PintaCore.Actions.File.CreateFileCommandMapBox (main1);
			PintaCore.Actions.Edit.CreateEditCommandMapBox (main1);
			vbox.Add (main1);

			HBox main2 = new HBox ();
			PintaCore.Actions.Edit.CreateSelectionCommandMapBox (main2);
			PintaCore.Actions.Image.CreateCropCommandMapBox (main2);
			vbox.Add (main2);

			HBox main3 = new HBox ();
			PintaCore.Actions.View.CreateZoomCommandMapBox (main3);
			PintaCore.Actions.Image.CreateTransformCommandMapBox (main3);
			vbox.Add (main3);

			HBox main4 = new HBox ();
			PintaCore.Actions.Layers.CreateLayerCommandMapBox (main4);
			PintaCore.Actions.Layers.CreateLayerTransformCommandMapBox (main4);
			vbox.Add (main4);

			// Add two rows for tools.
			var toolsFrame = new Frame ("Tools");
			var toolsBox = new VBox ();
			tools1 = new HBox ();
			toolsBox.Add (tools1);
			tools2 = new HBox ();
			toolsBox.Add (tools2);
			toolsFrame.Add (toolsBox);
			vbox.Add (toolsFrame);

			// Add color palette.
			var paletteBox = new HBox ();
			var palette = new ColorPaletteWidget (false);
			palette.Initialize ();
			PintaCore.Actions.Edit.CreatePaletteCommandMapBox (paletteBox, palette);
			vbox.Add (paletteBox);

			// Add adjustments.
			var adjustmentsFrame = new Frame ("Adjustments");
			AdjustmentsCommandMapBox = new HBox ();
			adjustmentsFrame.Add (AdjustmentsCommandMapBox);
			vbox.Add (adjustmentsFrame);

			// Add effects.
			var effectsFrame = new Frame ("Effects");
			EffectsCommandMapBox = new VBox ();
			effectsFrame.Add (EffectsCommandMapBox);
			vbox.Add (effectsFrame);

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
