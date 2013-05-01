using Gtk;

namespace Pinta
{
	public class CommandMapWindow : Window
	{
		public CommandMapWindow () : base ("Command Map")
		{
			SetDefaultSize (200, 200);
			//Decorated = false;

			Button button = new Button ("Click");
			Add (button);
		}
	}
}
