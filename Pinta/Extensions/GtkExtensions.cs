// 
// GtkExtensions.cs
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
using MonoDevelop.Components.Docking;

namespace Pinta
{
	public static class GtkExtensions
	{
		public static DockToolButton CreateDockToolBarItem (this Gtk.Action action)
		{
			DockToolButton item = new DockToolButton (action.StockId);
			action.ConnectProxy (item);
			
			item.Show ();
			item.TooltipText = action.Label;
			item.Label = string.Empty;
			item.Image.Show ();
			
			return item;
		}

		public static Gtk.Button CreateButton (this Gtk.Action action)
		{
			var button = new Button ();
			action.ConnectProxy (button);

			button.Relief = ReliefStyle.None;
			button.TooltipText = action.Label;
			button.Label = action.Label;
			button.Image = new Image (action.StockId, IconSize.Button);
			button.ImagePosition = PositionType.Top;
			button.Image.Show ();

			return button;
		}

		public static Gtk.ToolItem CreateToolBarItem (this Gtk.Action action)
		{
			Gtk.ToolItem item = (Gtk.ToolItem)action.CreateToolItem ();
			item.TooltipText = action.Label;
			return item;
		}

		public static void AppendItem (this Toolbar tb, ToolItem item)
		{
			item.Show ();
			tb.Insert (item, tb.NItems);
		}
	}
}
