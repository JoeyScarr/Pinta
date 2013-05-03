using System;
using Gtk;
using Pinta.Core;

namespace Pinta.Gui.Widgets
{
	public class CommandMapButton : Button
	{
		public BaseTool Tool { get; private set; }

		public CommandMapButton (BaseTool tool)
		{
			Tool = tool;

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
			PintaCore.Tools.SetCurrentTool (Tool);
		}
	}
}

