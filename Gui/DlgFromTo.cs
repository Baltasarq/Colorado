
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
			this.ShowAll();
			
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

		private void Build() {
			var vBox = new Gtk.VBox( false, 2 );
			var hBoxFrom = new Gtk.HBox( false, 2 );
			var hBoxTo = new Gtk.HBox( false, 2 );

			// To
			this.sbTo = new Gtk.SpinButton( 1, 10, 1 );
			this.lblTo = new Gtk.Label( "To:" );
			hBoxTo.PackStart( this.lblTo, false, false, 5 );
			hBoxTo.PackStart( this.sbTo, true, true, 5 );

			// From
			this.sbFrom = new Gtk.SpinButton( 1, 10, 1 );
			this.lblFrom = new Gtk.Label( "From:" );
			hBoxFrom.PackStart( this.lblFrom, false, false, 5 );
			hBoxFrom.PackStart( this.sbFrom, true, true, 5 );

			// Layout
			this.frmData = new Gtk.Frame( "Data" );
			vBox.PackStart( hBoxFrom, true, true, 5 );
			vBox.PackStart( hBoxTo, true, true, 5 );
			this.frmData.Add( vBox );
			this.VBox.PackStart( this.frmData, true, true, 5 );

			// Buttons
			this.AddButton( Gtk.Stock.Cancel, Gtk.ResponseType.Cancel );
			this.AddButton( Gtk.Stock.Ok, Gtk.ResponseType.Ok );
			this.DefaultResponse = Gtk.ResponseType.Ok;
		}

		private Gtk.Frame frmData;
		private Gtk.SpinButton sbFrom; 
		private Gtk.SpinButton sbTo; 
		private Gtk.Label lblFrom;
		private Gtk.Label lblTo;
	}
}
