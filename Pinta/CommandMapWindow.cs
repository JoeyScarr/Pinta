using Gtk;
using Gdk;
using Pinta.Core;
using Pinta.Gui.Widgets;

namespace Pinta
{
	public class CommandMapWindow : Gtk.Window
	{
		private HBox tools1;
		private HBox tools2;

		public HBox ToolToolbarBox { get; private set; }
		public HBox AdjustmentsCommandMapBox { get; private set; }
		public VBox EffectsCommandMapBox { get; private set; }

		public CommandMapWindow () : base (Gtk.WindowType.Popup)
		{
			const int spacing = 5;

			TransientFor = PintaCore.Chrome.MainWindow;
			WindowPosition = WindowPosition.CenterOnParent;
			Opacity = 0.9;

			var frame = new Frame ();
			VBox vbox = new VBox ();
			vbox.BorderWidth = spacing;
			vbox.Spacing = spacing;
			frame.Add (vbox);
			Add (frame);

			// Add the main toolbars.
			HBox main1 = new HBox ();
			main1.Spacing = spacing;
			var file = new CategoryBox ("File");
			var edit = new CategoryBox ("Edit");
			PintaCore.Actions.File.CreateFileCommandMapBox (file.Body);
			PintaCore.Actions.Edit.CreateEditCommandMapBox (edit.Body);
			main1.Add (file);
			main1.Add (edit);
			vbox.Add (main1);

			HBox main2 = new HBox ();
			main2.Spacing = spacing;
			var select = new CategoryBox ("Select");
			var crop = new CategoryBox ("Crop");
			PintaCore.Actions.Edit.CreateSelectionCommandMapBox (select.Body);
			PintaCore.Actions.Image.CreateCropCommandMapBox (crop.Body);
			main2.Add (select);
			main2.Add (crop);
			vbox.Add (main2);

			HBox main3 = new HBox ();
			main3.Spacing = spacing;
			var zoom = new CategoryBox ("Zoom");
			var transform = new CategoryBox ("Transform");
			PintaCore.Actions.View.CreateZoomCommandMapBox (zoom.Body);
			PintaCore.Actions.Image.CreateTransformCommandMapBox (transform.Body);
			main3.Add (zoom);
			main3.Add (transform);
			vbox.Add (main3);

			HBox main4 = new HBox ();
			main4.Spacing = spacing;
			var layers = new CategoryBox ("Layers");
			var layer_transform = new CategoryBox ("Layer Transform");
			PintaCore.Actions.Layers.CreateLayerCommandMapBox (layers.Body);
			PintaCore.Actions.Layers.CreateLayerTransformCommandMapBox (layer_transform.Body);
			main4.Add (layers);
			main4.Add (layer_transform);
			vbox.Add (main4);

			// Add rows for tools and box for tool toolbar.
			var tools = new CategoryBox ("Tools");
			var toolsBox = new VBox ();
			tools1 = new HBox ();
			toolsBox.Add (tools1);
			tools2 = new HBox ();
			toolsBox.Add (tools2);
			ToolToolbarBox = new HBox ();
			toolsBox.Add (ToolToolbarBox);
			tools.Body.Add (toolsBox);
			vbox.Add (tools);

			// Add color palette.
			var paletteRow = new HBox ();
			paletteRow.Spacing = spacing;

			var paletteBox = new CategoryBox ("Palette");
			var palette = new ColorPaletteWidget (false);
			palette.Initialize ();
			DarkenBackground (palette);
			PintaCore.Actions.Edit.CreatePaletteCommandMapBox (paletteBox.Body, palette);
			paletteRow.Add (paletteBox);

			// Add add-ins manager on same line as palette.
			var addins = new CategoryBox ("Add-ins");
			PintaCore.Actions.Addins.CreateAddinsCommandMapBox (addins.Body);
			paletteRow.Add (addins);

			vbox.Add (paletteRow);

			// Add adjustments.
			var adjustments = new CategoryBox ("Adjustments");
			AdjustmentsCommandMapBox = new HBox ();
			adjustments.Body.Add (AdjustmentsCommandMapBox);
			vbox.Add (adjustments);

			// Add effects.
			var effects = new CategoryBox ("Effects");
			EffectsCommandMapBox = new VBox ();
			effects.Body.Add (EffectsCommandMapBox);
			vbox.Add (effects);

			// Add quit and help frames.
			HBox main5 = new HBox ();
			main5.Spacing = spacing;
			var quit = new CategoryBox ("Quit");
			var help = new CategoryBox ("Help");
			PintaCore.Actions.File.CreateQuitCommandMapBox (quit.Body);
			PintaCore.Actions.Help.CreateHelpCommandMapBox (help.Body);
			main5.Add (quit);
			main5.Add (help);
			vbox.Add (main5);

			PintaCore.Tools.ToolAdded += HandleToolAdded;
			PintaCore.Tools.ToolRemoved += HandleToolRemoved;
		}

		private class CategoryBox : EventBox
		{
			public Box Body { get; private set; }

			public CategoryBox (string labelText) : base ()
			{
				var frame = new Frame ();
				var vbox = new VBox ();

				var label = new Label (labelText);
				label.ModifyFont (new Pango.FontDescription () { Size = 24, Weight = Pango.Weight.Bold });

				Body = new HBox ();

				vbox.Add (label);
				vbox.Add (Body);
				frame.Add (vbox);
				Add (frame);

				DarkenBackground (this);
			}
		}

		private static void DarkenBackground (Widget widget)
		{
			const byte color_change = 10;

			var bg = widget.Style.Background (StateType.Normal);
			byte newRed = (byte)((byte)bg.Red + color_change);
			byte newGreen = (byte)((byte)bg.Green + color_change);
			byte newBlue = (byte)((byte)bg.Blue + color_change);
			var newBg = new Color (newRed, newGreen, newBlue);
			Colormap.System.AllocColor (ref newBg, true, true);
			widget.ModifyBg (StateType.Normal, newBg);
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
