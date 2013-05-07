// 
// EffectsActions.cs
//  
// Author:
//       Jonathan Pobst <monkey@jpobst.com>
// 
// Copyright (c) 2010 Jonathan Pobst
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using Gtk;
using System.Collections.Generic;

namespace Pinta.Core
{
	public class EffectsActions
	{
		private Menu effects_menu;
		private Dictionary<Gtk.Action, MenuItem> menu_items;
		private List<HBox> command_map_boxes;
		private Dictionary<Gtk.Action, Button> command_map_buttons;
		private Dictionary<Gtk.Action, HBox> command_map_button_boxes;

		public Dictionary<string, Gtk.Menu> Menus { get; private set; }
		public List<Gtk.Action> Actions { get; private set; }

		public EffectsActions ()
		{
			Actions = new List<Gtk.Action> ();
			Menus = new Dictionary<string,Menu> ();
			menu_items = new Dictionary<Gtk.Action, MenuItem> ();
			command_map_boxes = new List<HBox> ();
			command_map_buttons = new Dictionary<Gtk.Action, Button> ();
			command_map_button_boxes = new Dictionary<Gtk.Action, HBox> ();
		}
		
		#region Initialization
		public void CreateMainMenu (Gtk.Menu menu)
		{
			effects_menu = menu;
		}

		public void AddEffect (string category, Gtk.Action action)
		{
			if (!Menus.ContainsKey (category)) {
				Gtk.Action menu_action = new Gtk.Action (category, Mono.Unix.Catalog.GetString (category), null, null);
				Menu category_menu = (Menu)effects_menu.AppendMenuItemSorted ((MenuItem)(menu_action.CreateSubMenuItem ())).Submenu;
				
				Menus.Add (category, category_menu);
			}

			HBox command_map_box;

			if (command_map_boxes.Count > 0)
			{
				var last_box = command_map_boxes[command_map_boxes.Count - 1];

				const int widgets_per_box = 10;
				if (last_box.Children.Length == widgets_per_box)
				{
					last_box = new HBox ();
					command_map_boxes.Add (last_box);
					PintaCore.Chrome.EffectsCommandMapBox.Add (last_box);
				}

				command_map_box = last_box;
			}
			else
			{
				command_map_box = new HBox ();
				command_map_boxes.Add (command_map_box);
				PintaCore.Chrome.EffectsCommandMapBox.Add (command_map_box);
			}
			
			Actions.Add (action);
			var menu_item = (MenuItem)action.CreateMenuItem ();
			var button = action.CreateButton ();

			Menu m = Menus[category];
			m.AppendMenuItemSorted (menu_item);

			command_map_button_boxes[action] = command_map_box;
			command_map_box.Add (button);

			menu_items.Add (action, menu_item);
			command_map_buttons.Add (action, button);
		}

		// TODO: Remove category if empty
		internal void RemoveEffect (string category, Gtk.Action action)
		{
			if (!Menus.ContainsKey (category))
				return;
			if (!menu_items.ContainsKey (action))
				return;
			if (!command_map_buttons.ContainsKey (action))
				return;

			var menu = Menus[category];
			menu.Remove (menu_items[action]);

			var box = command_map_button_boxes[action];
			box.Remove (command_map_buttons[action]);
			command_map_buttons.Remove (action);
			command_map_button_boxes.Remove (action);

			if (box.Children.Length == 0)
			{
				command_map_boxes.Remove (box);
				PintaCore.Chrome.EffectsCommandMapBox.Remove (box);
			}
		}
		#endregion

		#region Public Methods
		public void ToggleActionsSensitive (bool sensitive)
		{
			foreach (Gtk.Action a in Actions)
				a.Sensitive = sensitive;
		}
		#endregion
	}
}
