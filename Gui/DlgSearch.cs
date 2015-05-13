
using System;

namespace Colorado.Gui {
	public partial class DlgSearch : Gtk.Dialog {
		public DlgSearch(Gtk.Window parent, string title, string text) {
			this.Build();
			
			this.Title = title;
			this.Icon = parent.Icon;
			this.lblExplanation.Markup = "<i>" + text + "</i>";
			this.Parent = parent;
			this.TransientFor = parent;
			this.SetPosition( Gtk.WindowPosition.CenterOnParent );
		}
		
		public string Text {
		 get { return this.edTextSearch.Text; }
		}
	
		public static string ask(Gtk.Window parent, string title, string text)
		{
			string toret = "";
			var dlg = new DlgSearch( parent, title, text );
			var result = (Gtk.ResponseType) dlg.Run();
			
			if ( result == Gtk.ResponseType.Ok ) {
				toret = dlg.Text;
			}
			
			dlg.Destroy();
			
			return toret;
		}

		protected virtual void onEditKeyReleased(object o, Gtk.KeyReleaseEventArgs args)
		{
			if ( args.Event.Key == Gdk.Key.Return ) {
				this.Respond(Gtk.ResponseType.Ok );
			}
		}
	}
}
