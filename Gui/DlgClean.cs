
using System;
using System.Collections.ObjectModel;

namespace Colorado.Gui {
	public partial class DlgClean : Gtk.Dialog {
		public enum Items { Row, Column };
		public static readonly ReadOnlyCollection<string> StrItems = new ReadOnlyCollection<string>(
			new string[] { "rows", "columns" }
		);
		
		public DlgClean(Gtk.Window parent, Items item, int begin, int max)
		{
			this.Build ();
			
			// Prepare dialog
			this.Title = "Clean " + StrItems[ (int) item ];
			this.Icon = parent.Icon;
			this.Parent = parent;
			this.TransientFor = parent;
			this.SetPosition( Gtk.WindowPosition.CenterOnParent );
			
			// Set widgets
			sbStart.Value = begin;
			sbEnd.Value = begin;
			sbStart.SetRange( 1, max );
			sbEnd.SetRange( 1, max );
		}
		
		public int Begin {
			get { return sbStart.ValueAsInt; }
		}
		
		public int End {
			get { return sbEnd.ValueAsInt; }
		}
	}
}
