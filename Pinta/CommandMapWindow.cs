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
			PintaCore.Actions.File.CreateFileCommandMapBox (main1, CreateFrame ("File"));
			PintaCore.Actions.Edit.CreateEditCommandMapBox (main1, CreateFrame ("Edit"));
			vbox.Add (main1);

			HBox main2 = new HBox ();
			main2.Spacing = 5;
			PintaCore.Actions.Edit.CreateSelectionCommandMapBox (main2, CreateFrame ("Select"));
			PintaCore.Actions.Image.CreateCropCommandMapBox (main2, CreateFrame ("Crop"));
			vbox.Add (main2);

			HBox main3 = new HBox ();
			main3.Spacing = 5;
			PintaCore.Actions.View.CreateZoomCommandMapBox (main3, CreateFrame ("Zoom"));
			PintaCore.Actions.Image.CreateTransformCommandMapBox (main3, CreateFrame ("Image Transform"));
			vbox.Add (main3);

			HBox main4 = new HBox ();
			main4.Spacing = 5;
			PintaCore.Actions.Layers.CreateLayerCommandMapBox (main4, CreateFrame ("Layers"));
			PintaCore.Actions.Layers.CreateLayerTransformCommandMapBox (main4, CreateFrame ("Layer Transform"));
			vbox.Add (main4);

			// Add rows for tools and box for tool toolbar.
			var toolsFrame = CreateFrame ("Tools");
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
			PintaCore.Actions.Edit.CreatePaletteCommandMapBox (paletteBox, CreateFrame ("Palette"), palette);
			vbox.Add (paletteBox);

			// Add add-ins manager on same line as palette.
			PintaCore.Actions.Addins.CreateAddinsCommandMapBox (paletteBox, CreateFrame ("Add-ins"));

			// Add adjustments.
			var adjustmentsFrame = CreateFrame ("Adjustments");
			AdjustmentsCommandMapBox = new HBox ();
			adjustmentsFrame.Add (AdjustmentsCommandMapBox);
			vbox.Add (adjustmentsFrame);

			// Add effects.
			var effectsFrame = CreateFrame ("Effects");
			EffectsCommandMapBox = new VBox ();
			effectsFrame.Add (EffectsCommandMapBox);
			vbox.Add (effectsFrame);

			// Add quit and help frames.
			HBox main5 = new HBox ();
			main5.Spacing = 5;
			PintaCore.Actions.File.CreateQuitCommandMapBox (main5, CreateFrame ("Quit"));
			PintaCore.Actions.Help.CreateHelpCommandMapBox (main5, CreateFrame ("Help"));
			vbox.Add (main5);

			PintaCore.Tools.ToolAdded += HandleToolAdded;
			PintaCore.Tools.ToolRemoved += HandleToolRemoved;
		}

		private static Frame CreateFrame (string label)
		{
			var frame = new Frame (label);
			frame.LabelWidget.ModifyFont (new Pango.FontDescription () { Size = 24, Weight = Pango.Weight.Bold });
			return frame;
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
