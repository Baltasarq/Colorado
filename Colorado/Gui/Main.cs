// Colorado (c) 2015... 2023 Baltasar MIT License <baltasarq@gmail.com>
/*
 * Colorado, a csv-based spreadsheet
 */


namespace Colorado.Gui {
    using System.Diagnostics;
    using Colorado.Core;

    class MainClass {
        [Conditional("DEBUG")]
        private static void CreateConsoleTracing()
        {
            Trace.Listeners.Add( new ConsoleTraceListener( true ) );
        }

		[STAThread]
		static void Main(string[] args)
		{
			Gtk.ApplicationWindow? win = null;
			string arg = "";

			if ( args.Length > 0 ) {
				arg = args[ 0 ];
			}

			try {
				CreateConsoleTracing();
				var app = new Gtk.Application( AppInfo.RefName,
												GLib.ApplicationFlags.HandlesCommandLine
												| GLib.ApplicationFlags.HandlesOpen );

				Gtk.Application.Init();

				win = new MainWindow( app, arg );
				win.ShowAll();

				Gtk.Application.Run();
			} catch (System.Exception e) {
				Trace.WriteLine( "CRITICAL error: " + e.Message + e.StackTrace );
				GtkUtil.Misc.MsgError( win, Core.AppInfo.Name, e.Message );
			}
		}
	}
}
