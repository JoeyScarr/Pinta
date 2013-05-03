using System;
using Gtk;
using Pinta.Core;

namespace Pinta.Gui.Widgets
{
	public class CommandMapButton : Button
	{
		private BaseTool tool;

		public BaseTool Tool { get { return tool; } }

		public CommandMapButton (BaseTool tool)
		{
			this.tool = tool;

			Relief = ReliefStyle.None;
			TooltipText = tool.ToolTip;

			var icon = new Image (PintaCore.Resources.GetIcon (tool.Icon));

			VBox box = new VBox ();
			box.Add (icon);
			box.Add (new Label (tool.Name));
			Add (box);

			Clicked += HandleClicked;
		}

		private void HandleClicked (object sender, EventArgs e)
		{
			PintaCore.Tools.SetCurrentTool (tool);
		}
	}
}

