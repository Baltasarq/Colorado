
using System;
using System.Collections.ObjectModel;

namespace Colorado.Gui {
	public partial class DlgCopy : Gtk.Dialog {
		public enum DialogType { Rows, Columns };
		public static readonly ReadOnlyCollection<string> StrDialogType = new ReadOnlyCollection<string>(
			new string[]{ "Row", "Column" }
		);
		
		public DlgCopy(Gtk.Window parent, int org, int max, DialogType type)
		{
			int to = org + 1;
			this.Build();
			
			// Prepare limits
			if ( to > max ) {
				to = max;
			}
			
			// Prepare dialog
			this.Title = "Copy " + StrDialogType[ (int) type ];
			this.Icon = parent.Icon;
			this.Parent = parent;
			this.TransientFor = parent;
			this.SetPosition( Gtk.WindowPosition.CenterOnParent );
			
			// Prepare widgets
			sbFrom.SetRange( 1, max );
			sbTo.SetRange( 1, max );
			sbFrom.Value = org;
			sbTo.Value = to;
		}
		
		public int From {
			get { return sbFrom.ValueAsInt; }
		}
		
		public int To {
			get { return sbTo.ValueAsInt; }
		}
	}
}
