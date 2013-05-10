using System;
using System.Collections.Generic;
using System.IO;
using Gtk;
using Gdk;
using Pinta.Core;
using Pinta.Gui.Widgets;
using MonoDevelop.Components.Docking;

namespace Pinta
{
	public class CommandMapWindow : Gtk.Window
	{
		private HBox tools1;
		private HBox tools2;

		private CommandMapMaskWindow mask_window;

		private List<HBox> command_map_boxes;
		private Dictionary<Gtk.Action, Button> command_map_buttons;
		private Dictionary<Gtk.Action, HBox> command_map_button_boxes;
		private Dictionary<BaseEffect, Button> adjustment_command_map_buttons;
		private List<Widget> important_widgets; // The widgets which should show through the grey mask.

		public HBox ToolToolbarBox { get; private set; }
		public HBox AdjustmentsCommandMapBox { get; private set; }
		public VBox EffectsCommandMapBox { get; private set; }

		private const string log_file = "cmd_map_log.txt";

		static CommandMapWindow ()
		{
			Log ("New Pinta session started.");
		}

		public CommandMapWindow () : base (Gtk.WindowType.Popup)
		{
			const int spacing = 5;

			TransientFor = PintaCore.Chrome.MainWindow;
			WindowPosition = WindowPosition.CenterOnParent;
			Opacity = 0.9;

			mask_window = new CommandMapMaskWindow ();

			var frame = new Frame ();
			HBox container = new HBox ();
			container.BorderWidth = spacing;
			container.Spacing = spacing;

			VBox vbox = new VBox ();
			vbox.Spacing = spacing;

			var dock = new DockFrame ();
			dock.CompactGuiLevel = 5;

			container.Add (vbox);
			container.Add (dock);
			frame.Add (container);
			Add (frame);

			// Add the dock pads.
			// Open Images pad
			var open_images_pad = new OpenImagesPad ();
			open_images_pad.Initialize (dock);

			// Layer pad
			var layers_pad = new LayersPad ();
			layers_pad.Initialize (dock);

			// History pad
			var history_pad = new HistoryPad ();
			history_pad.Initialize (dock);

			dock.CreateLayout ("Default", false);
			dock.CurrentLayout = "Default";

			// Add the main toolbars.
			HBox main1 = new HBox ();
			main1.Spacing = spacing;
			var file = new CategoryBox ("File");
			var edit = new CategoryBox ("Edit");
			CreateButtons (PintaCore.Actions.File.GetFileActions (), file.Body);
			CreateButtons (PintaCore.Actions.Edit.GetEditActions (), edit.Body);
			main1.Add (file);
			main1.Add (edit);
			vbox.Add (main1);

			HBox main2 = new HBox ();
			main2.Spacing = spacing;
			var select = new CategoryBox ("Select");
			var crop = new CategoryBox ("Crop");
			CreateButtons (PintaCore.Actions.Edit.GetSelectActions (), select.Body);
			CreateButtons (PintaCore.Actions.Image.GetCropActions (), crop.Body);
			main2.Add (select);
			main2.Add (crop);
			vbox.Add (main2);

			HBox main3 = new HBox ();
			main3.Spacing = spacing;
			var zoom = new CategoryBox ("Zoom");
			var transform = new CategoryBox ("Transform");
			CreateButtons (PintaCore.Actions.View.GetZoomActions (), zoom.Body);
			CreateButtons (PintaCore.Actions.Image.GetTransformActions (), transform.Body);
			main3.Add (zoom);
			main3.Add (transform);
			vbox.Add (main3);

			HBox main4 = new HBox ();
			main4.Spacing = spacing;
			var layers = new CategoryBox ("Layers");
			var layer_transform = new CategoryBox ("Layer Transform");
			CreateButtons (PintaCore.Actions.Layers.GetLayerActions (), layers.Body);
			CreateButtons (PintaCore.Actions.Layers.GetLayerTransformActions (), layer_transform.Body);
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
			paletteBox.Body.Add (palette);
			CreateButtons (PintaCore.Actions.Edit.GetPaletteActions (), paletteBox.Body);
			paletteRow.Add (paletteBox);

			// Add add-ins manager on same line as palette.
			var addins = new CategoryBox ("Add-ins");
			CreateButtons (PintaCore.Actions.Addins.GetAddinActions (), addins.Body);
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
			CreateButtons (PintaCore.Actions.File.GetQuitActions (), quit.Body);
			CreateButtons (PintaCore.Actions.Help.GetHelpActions (), help.Body);
			main5.Add (quit);
			main5.Add (help);
			vbox.Add (main5);

			PintaCore.Tools.ToolAdded += HandleToolAdded;
			PintaCore.Tools.ToolRemoved += HandleToolRemoved;

			command_map_boxes = new List<HBox> ();
			command_map_buttons = new Dictionary<Gtk.Action, Button> ();
			command_map_button_boxes = new Dictionary<Gtk.Action, HBox> ();
			adjustment_command_map_buttons = new Dictionary<BaseEffect, Button> ();
			important_widgets = new List<Widget> ();

			PintaCore.Effects.AddEffectEvent += AddEffect;
			PintaCore.Effects.RemoveEffectEvent += RemoveEffect;
			PintaCore.Effects.AddAdjustmentEvent += AddAdjustment;
		}

		private static void LogButtonClick (CommandMapButton button)
		{
			Log ("User clicked button: " + button.Name);
		}

		private static void Log (string message)
		{
			using (var log = new StreamWriter (log_file, true))
			{
				log.WriteLine ("{0}: {1}", DateTime.Now, message);
			}
		}

		public void On ()
		{
			ShowAll ();
			//mask_window.ShowAll ();
		}

		public void Off ()
		{
			HideAll ();
			//mask_window.HideAll ();
		}

		public void RecreateMask ()
		{
			int width, height;
			GetSize (out width, out height);

			mask_window.Recreate (width, height, important_widgets);
		}

		private class CommandMapMaskWindow : Gtk.Window
		{
			public CommandMapMaskWindow () : base (Gtk.WindowType.Popup)
			{
				TransientFor = PintaCore.Chrome.MainWindow;
				WindowPosition = WindowPosition.CenterOnParent;
				Opacity = 0.6;
			}

			public void Recreate (int width, int height, List<Widget> widgets)
			{
				Resize (width, height);

				var image = new Cairo.ImageSurface (Cairo.Format.Argb32, width, height);
				using (var cr = new Cairo.Context (image))
				{
					cr.Operator = Cairo.Operator.Source;

					cr.Rectangle (0, 0, width, height);
					cr.SetSourceRGBA (0, 0, 0, 1);
					cr.Fill ();

					foreach (var widget in widgets)
					{
						int w, h, x, y;
						w = widget.Allocation.Width;
						h = widget.Allocation.Height;
						widget.TranslateCoordinates (widget.Toplevel, 0, 0, out x, out y);

						cr.Rectangle (x, y, w, h);
						cr.SetSourceRGBA (0, 0, 0, 0);
						cr.Fill ();
					}
				}

				var pixbuf = image.ToPixbuf ();
				Pixmap pm, mask;
				pixbuf.RenderPixmapAndMask (out pm, out mask, 1);
				ShapeCombineMask (mask, 0, 0);
				pm.Dispose ();
				mask.Dispose ();
			}
		}

		private void CreateButtons (Gtk.Action[] actions, Box box)
		{
			foreach (var action in actions)
			{
				box.Add (new CommandMapButton (action));
			}
		}

		private static Color BlendColors (Color fg, Color bg, double opacity)
		{
			var newRed   = (byte)fg.Red   * opacity + (byte)bg.Red   * (1 - opacity);
			var newGreen = (byte)fg.Green * opacity + (byte)bg.Green * (1 - opacity);
			var newBlue  = (byte)fg.Blue  * opacity + (byte)bg.Blue  * (1 - opacity);
			var newColor = new Color ((byte)newRed, (byte)newGreen, (byte)newBlue);
			Colormap.System.AllocColor (ref newColor, true, true);
			return newColor;
		}

		private class CommandMapButton : Button
		{
			private const int max_most_recently_used = 10;
			private static LinkedList<CommandMapButton> most_recently_used =
				new LinkedList<CommandMapButton> ();
			private static double[] most_recently_used_opacities;

			public virtual string Name { get { return Label; } }

			static CommandMapButton ()
			{
				most_recently_used_opacities = new double[max_most_recently_used];

				for (int i = 0; i < max_most_recently_used; i++)
				{
					most_recently_used_opacities[i] = (double)i / max_most_recently_used;
				}
			}

			private double HighlightOpacity { get; set; }

			public CommandMapButton ()
			{
				HighlightOpacity = 0.0;
				Relief = ReliefStyle.None;

				ButtonReleaseEvent += HandleButtonReleaseEvent;
			}

			public CommandMapButton (Gtk.Action action) : this ()
			{
				action.ConnectProxy (this);

				TooltipText = action.Label;
				Label = action.Label;
				Image = new Gtk.Image (action.StockId, IconSize.Button);
				ImagePosition = PositionType.Top;
				Image.Show ();
			}

			protected override bool OnExposeEvent (EventExpose evnt)
			{
				if (HighlightOpacity > 0.0)
				{
					using (var cr = Gdk.CairoHelper.Create (evnt.Window))
					{
						var bg = Style.Background (StateType.Normal);

						var fg = new Color ();
						Color.Parse ("yellow", ref fg);
						Colormap.AllocColor (ref fg, true, true);

						var color = BlendColors (fg, bg, HighlightOpacity);

						cr.FillRoundedRectangle (evnt.Area.ToCairoRectangle (), 5, color.ToCairoColor ());
					}
				}

				return base.OnExposeEvent (evnt);
			}

			[GLib.ConnectBefore]
			protected void HandleButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
			{
				LogButtonClick (this);

				if (max_most_recently_used == 0)
					return;

				// If we haven't hit the max number of recently used buttons
				// yet, we have to start at some offset into the opacities
				// array.
				int i = max_most_recently_used - most_recently_used.Count;

				// If a button occurs in the list more than once, it's opacity
				// will be set more than once but it will be overriden last by
				// the highest opacity (it's most recenty use).
				foreach (var button in most_recently_used)
				{
					button.HighlightOpacity = most_recently_used_opacities[i];
					button.QueueDraw ();
					i++;
				}

				if (most_recently_used.Count == max_most_recently_used)
				{
					most_recently_used.RemoveFirst ();
				}

				most_recently_used.AddLast (this);
				HighlightOpacity = 1.0;
				QueueDraw ();
			}
		}

		private class CommandMapToolButton : CommandMapButton
		{
			public BaseTool Tool { get; private set; }
			public override string Name { get { return Tool.Name; } }

			public CommandMapToolButton (BaseTool tool) : base ()
			{
				Tool = tool;

				TooltipText = tool.ToolTip;

				var icon = new Gtk.Image (PintaCore.Resources.GetIcon (tool.Icon));

				VBox box = new VBox ();
				box.Add (icon);
				box.Add (new Label (tool.Name));
				Add (box);

				ButtonReleaseEvent += Tool_HandleButtonReleaseEvent;
			}

			[GLib.ConnectBefore]
			void Tool_HandleButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
			{
				PintaCore.Tools.SetCurrentTool (Tool);
			}
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
			byte newRed   = (byte)((byte)bg.Red   + color_change);
			byte newGreen = (byte)((byte)bg.Green + color_change);
			byte newBlue  = (byte)((byte)bg.Blue  + color_change);
			var newBg = new Color (newRed, newGreen, newBlue);
			Colormap.System.AllocColor (ref newBg, true, true);
			widget.ModifyBg (StateType.Normal, newBg);
		}

		private void AddAdjustment (BaseEffect adjustment, Gtk.Action action)
		{
			var button = new CommandMapButton (action);
			AdjustmentsCommandMapBox.Add (button);
			adjustment_command_map_buttons.Add (adjustment, button);
		}

		private void RemoveAdjustment (BaseEffect adjustment)
		{
			var button = adjustment_command_map_buttons[adjustment];
			AdjustmentsCommandMapBox.Remove (button);
		}

		private void AddEffect (string category, Gtk.Action action)
		{
			HBox command_map_box;

			if (command_map_boxes.Count > 0)
			{
				var last_box = command_map_boxes[command_map_boxes.Count - 1];

				const int widgets_per_box = 10;
				if (last_box.Children.Length == widgets_per_box)
				{
					last_box = new HBox ();
					command_map_boxes.Add (last_box);
					EffectsCommandMapBox.Add (last_box);
				}

				command_map_box = last_box;
			}
			else
			{
				command_map_box = new HBox ();
				command_map_boxes.Add (command_map_box);
				EffectsCommandMapBox.Add (command_map_box);
			}

			var button = new CommandMapButton (action);

			command_map_button_boxes[action] = command_map_box;
			command_map_box.Add (button);
			command_map_buttons.Add (action, button);
		}

		private void RemoveEffect (string category, Gtk.Action action)
		{
			if (!command_map_buttons.ContainsKey (action))
				return;

			var box = command_map_button_boxes[action];
			box.Remove (command_map_buttons[action]);
			command_map_buttons.Remove (action);
			command_map_button_boxes.Remove (action);

			if (box.Children.Length == 0)
			{
				command_map_boxes.Remove (box);
				EffectsCommandMapBox.Remove (box);
			}
		}

		private void HandleToolAdded (object sender, ToolEventArgs e)
		{
			var button = new CommandMapToolButton (e.Tool);

			if(tools1.Children.Length <= tools2.Children.Length) {
				tools1.PackStart (button);
			} else {
				tools2.PackStart (button);
			}
		}

		private void HandleToolRemoved (object sender, ToolEventArgs e)
		{
			foreach (CommandMapToolButton button in tools1) {
				if (button.Tool == e.Tool) {
					tools1.Remove(button);
					return;
				}
			}

			foreach (CommandMapToolButton button in tools2) {
				if (button.Tool == e.Tool) {
					tools2.Remove(button);
					return;
				}
			}
		}
	}
}
