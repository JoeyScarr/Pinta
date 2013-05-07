using Gtk;
using Pinta.Core;
using Pinta.Gui.Widgets;

namespace Pinta
{
	public class CommandMapWindow : Window
	{
		private HBox tools1;
		private HBox tools2;

		public HBox ToolToolbarBox { get; private set; }
		public HBox AdjustmentsCommandMapBox { get; private set; }
		public VBox EffectsCommandMapBox { get; private set; }

		public CommandMapWindow () : base (WindowType.Popup)
		{
			TransientFor = PintaCore.Chrome.MainWindow;
			WindowPosition = WindowPosition.CenterOnParent;
			Opacity = 0.9;

			var frame = new Frame ();
			VBox vbox = new VBox ();
			vbox.BorderWidth = 5;
			frame.Add (vbox);
			Add (frame);

			// Add the main toolbars.
			HBox main1 = new HBox ();
			main1.Spacing = 5;
			PintaCore.Actions.File.CreateFileCommandMapBox (main1);
			PintaCore.Actions.Edit.CreateEditCommandMapBox (main1);
			vbox.Add (main1);

			HBox main2 = new HBox ();
			main2.Spacing = 5;
			PintaCore.Actions.Edit.CreateSelectionCommandMapBox (main2);
			PintaCore.Actions.Image.CreateCropCommandMapBox (main2);
			vbox.Add (main2);

			HBox main3 = new HBox ();
			main3.Spacing = 5;
			PintaCore.Actions.View.CreateZoomCommandMapBox (main3);
			PintaCore.Actions.Image.CreateTransformCommandMapBox (main3);
			vbox.Add (main3);

			HBox main4 = new HBox ();
			main4.Spacing = 5;
			PintaCore.Actions.Layers.CreateLayerCommandMapBox (main4);
			PintaCore.Actions.Layers.CreateLayerTransformCommandMapBox (main4);
			vbox.Add (main4);

			// Add rows for tools and box for tool toolbar.
			var toolsFrame = new Frame ("Tools");
			var toolsBox = new VBox ();
			ToolToolbarBox = new HBox ();
			toolsBox.Add (ToolToolbarBox);
			tools1 = new HBox ();
			toolsBox.Add (tools1);
			tools2 = new HBox ();
			toolsBox.Add (tools2);
			toolsFrame.Add (toolsBox);
			vbox.Add (toolsFrame);

			// Add color palette.
			var paletteBox = new HBox ();
			paletteBox.Spacing = 5;
			var palette = new ColorPaletteWidget (false);
			palette.Initialize ();
			PintaCore.Actions.Edit.CreatePaletteCommandMapBox (paletteBox, palette);
			vbox.Add (paletteBox);

			// Add add-ins manager on same line as palette.
			PintaCore.Actions.Addins.CreateAddinsCommandMapBox (paletteBox);

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
