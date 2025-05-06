// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>
/*
 * Colorado, a csv-based spreadsheet
 */

namespace Colorado.Gui {
    using System.Diagnostics;

	class MainClass {
        [Conditional("DEBUG")]
        private static void CreateConsoleTracing()
        {
            Trace.Listeners.Add( new ConsoleTraceListener( true ) );
        }

		static void Main(string[] args)
		{
			MainWindow win = null;
			
			try {
                CreateConsoleTracing();

				Gtk.Application.Init();

				if ( args.Length > 0 )
						win = new MainWindow( args[ 0 ] );
				else	win = new MainWindow();
					
				win.ShowAll();

				Gtk.Application.Run();
			}
			catch(System.Exception e) {
                Trace.WriteLine( "CRITICAL error: " + e.Message + e.StackTrace);
				GtkUtil.Util.MsgError( win, Core.AppInfo.Name, e.Message );
			}
		}
	}
}
