
using System;
using System.Collections.ObjectModel;

namespace Colorado.Gui {
	public partial class DlgFromTo : Gtk.Dialog {
		public enum ItemType { Rows, Columns };
        public enum ActionType { Clean, Copy };
		public static readonly ReadOnlyCollection<string> StrItemType = new ReadOnlyCollection<string>(
			new string[]{ "Row", "Column" }
		);
        public static readonly ReadOnlyCollection<string> StrActionType = new ReadOnlyCollection<string>(
            new string[]{ "Clean", "Copy" }
        );
		
		public DlgFromTo(Gtk.Window parent, int org, int max, ActionType action, ItemType item)
		{
			int to = org + 1;
			this.Build();
			
			// Prepare limits
			if ( to > max ) {
				to = max;
			}
			
			// Prepare dialog
            this.Title = StrActionType[ (int) action ] + ' ' + StrItemType[ (int) item ];
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
