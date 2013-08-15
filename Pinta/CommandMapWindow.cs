using System;
using System.Collections.Generic;
using System.IO;
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

		private List<HBox> command_map_boxes;
		private Dictionary<Gtk.Action, Button> command_map_buttons;
		private Dictionary<Gtk.Action, HBox> command_map_button_boxes;
		private Dictionary<BaseEffect, Button> adjustment_command_map_buttons;

		public HBox AdjustmentsCommandMapBox { get; private set; }
		public VBox EffectsCommandMapBox { get; private set; }

        private bool stay_open_when_clicked;

		public CommandMapWindow () : base (Gtk.WindowType.Popup)
		{
			const int spacing = 5;

			TransientFor = PintaCore.Chrome.MainWindow;
			WindowPosition = WindowPosition.CenterOnParent;
			Opacity = 0.9;

            var eventBox = new CommandMapEventBox(this);
			var frame = new Frame ();
			VBox vbox = new VBox ();
			vbox.Spacing = spacing;
            vbox.BorderWidth = spacing;
			frame.Add (vbox);
            eventBox.Add(frame);
			Add (eventBox);

			// Add the main toolbars.
			HBox main1 = new HBox ();
			main1.Spacing = spacing;
			var file = new CategoryBox ("File");
			var edit = new CategoryBox ("Edit");
			CreateButtons ("File", PintaCore.Actions.File.GetFileActions (), file.Body);
			CreateButtons ("Edit", PintaCore.Actions.Edit.GetEditActions (), edit.Body);
			main1.Add (file);
			main1.Add (edit);
			vbox.Add (main1);

			HBox main2 = new HBox ();
			main2.Spacing = spacing;
			var select = new CategoryBox ("Select");
			var crop = new CategoryBox ("Crop");
			CreateButtons ("Select", PintaCore.Actions.Edit.GetSelectActions (), select.Body);
			CreateButtons ("Crop", PintaCore.Actions.Image.GetCropActions (), crop.Body);
			main2.Add (select);
			main2.Add (crop);
			vbox.Add (main2);

			HBox main3 = new HBox ();
			main3.Spacing = spacing;
			var zoom = new CategoryBox ("Zoom");
			var transform = new CategoryBox ("Transform");
			CreateButtons ("Zoom", PintaCore.Actions.View.GetZoomActions (), zoom.Body);
            CreateButtons("Transform", PintaCore.Actions.Image.GetTransformActions(), transform.Body);
            main3.Add(transform);
			main3.Add (zoom);
			vbox.Add (main3);

			HBox main4 = new HBox ();
			main4.Spacing = spacing;
			var layers = new CategoryBox ("Layers");
			var layer_transform = new CategoryBox ("Layer Transform");
			CreateButtons ("Layers", PintaCore.Actions.Layers.GetLayerActions (), layers.Body);
			CreateButtons ("Layer Transform", PintaCore.Actions.Layers.GetLayerTransformActions (), layer_transform.Body);
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
			tools.Body.Add (toolsBox);
			vbox.Add (tools);

			// Add adjustments and add-ins manager.
            var adjustmentsRow = new HBox();

            var adjustments = new CategoryBox("Adjustments");
			AdjustmentsCommandMapBox = new HBox ();
			adjustments.Body.Add (AdjustmentsCommandMapBox);
            adjustmentsRow.Add(adjustments);

            var addins = new CategoryBox("Add-ins");
            CreateButtons("Add-ins", PintaCore.Actions.Addins.GetAddinActions(), addins.Body);
            adjustmentsRow.Add(addins);

			vbox.Add (adjustmentsRow);

			// Add effects.
			var effects = new CategoryBox ("Effects");
			EffectsCommandMapBox = new VBox ();
			effects.Body.Add (EffectsCommandMapBox);
			vbox.Add (effects);

			// Add quit, help, and palette frames.
			HBox main5 = new HBox ();
			main5.Spacing = spacing;
			var quit = new CategoryBox ("Quit");
            var help = new CategoryBox ("Help");
            var palette = new CategoryBox ("Palette");
			CreateButtons ("Quit", PintaCore.Actions.File.GetQuitActions (), quit.Body);
            CreateButtons ("Help", PintaCore.Actions.Help.GetHelpActions (), help.Body);
            CreateButtons ("Palette", PintaCore.Actions.Edit.GetPaletteActions (), palette.Body);
			main5.Add (quit);
			main5.Add (help);
            main5.Add (palette);
			vbox.Add (main5);

			PintaCore.Tools.ToolAdded += HandleToolAdded;
			PintaCore.Tools.ToolRemoved += HandleToolRemoved;

			command_map_boxes = new List<HBox> ();
			command_map_buttons = new Dictionary<Gtk.Action, Button> ();
			command_map_button_boxes = new Dictionary<Gtk.Action, HBox> ();
			adjustment_command_map_buttons = new Dictionary<BaseEffect, Button> ();

			PintaCore.Effects.AddEffectEvent += AddEffect;
			PintaCore.Effects.RemoveEffectEvent += RemoveEffect;
			PintaCore.Effects.AddAdjustmentEvent += AddAdjustment;
        }

        private static void Log (string message)
        {
            Logger.Log (message);
        }

        public void On(bool stay_open_when_clicked)
		{
            this.stay_open_when_clicked = stay_open_when_clicked;

			if (IsMapped)
				return;

			Log ("Command map window shown");
			ShowAll ();
		}

		public void Off (bool force_close)
		{
            if (!IsMapped || (stay_open_when_clicked && !force_close))
				return;

			Log ("Command map window hidden");
			HideAll ();
		}

        private class CommandMapEventBox : EventBox
        {
            private CommandMapWindow CMWindow;

            public CommandMapEventBox(CommandMapWindow cm_window)
            {
                CMWindow = cm_window;
            }

            protected override bool OnButtonReleaseEvent(EventButton evnt)
            {
                base.OnButtonReleaseEvent(evnt);
                CMWindow.Off(false);
                return true;
            }
        }

		private void CreateButtons (string category_name, Gtk.Action[] actions, Box box)
		{
			foreach (var action in actions)
			{
				box.Add (new CommandMapButton (category_name, action));
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
			public virtual string Name { get { return Label; } }
            public string CategoryName { get; private set; }

			public CommandMapButton (string category_name)
			{
                CategoryName = category_name;
				Relief = ReliefStyle.None;
			}

			public CommandMapButton (string category_name, Gtk.Action action) : this (category_name)
			{
				action.ConnectProxy (this);

				TooltipText = action.Label;
				Label = action.Label;
				Image = new Gtk.Image (action.StockId, IconSize.Button);
				ImagePosition = PositionType.Top;
				Image.Show ();
			}

            protected override bool OnButtonReleaseEvent(EventButton evnt)
            {
                Log("Button clicked: " + CategoryName + " | " + Name);
                base.OnButtonReleaseEvent(evnt);
                return false;
            }
		}

		private class CommandMapToolButton : CommandMapButton
		{
			public BaseTool Tool { get; private set; }
			public override string Name { get { return Tool.Name; } }

			public CommandMapToolButton (BaseTool tool) : base ("Tools")
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
			var button = new CommandMapButton ("Adjustments", action);
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

			var button = new CommandMapButton ("Effects", action);

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
