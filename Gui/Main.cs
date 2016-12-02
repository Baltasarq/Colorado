using System;
using Gtk;

using GtkUtil;

namespace Colorado.Gui {
	class MainClass {
		static void Main(string[] args)
		{
			MainWindow win = null;
			
			try {
				Application.Init();

				if ( args.Length > 0 )
						win = new MainWindow( args[ 0 ] );
				else	win = new MainWindow();
					
				win.ShowAll();

				Application.Run();
			}
			catch(Exception e) {
                System.Console.WriteLine( "CRITICAL error: " + e.Message );
				Util.MsgError( win, Core.AppInfo.Name, e.Message );
			}
		}
	}
}
