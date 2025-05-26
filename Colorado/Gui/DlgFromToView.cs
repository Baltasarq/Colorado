// Colorado (c) 2015-2018 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Gui {
	public partial class DlgFromTo : Gtk.Dialog {
		public DlgFromTo(Gtk.Window parent, int org, int end, int max, ActionType action, ItemType item)
		{
			this.sbFrom = new Gtk.SpinButton( 1, 10, 1 );
			this.lblFrom = new Gtk.Label( "From:" );
			this.sbTo = new Gtk.SpinButton( 1, 10, 1 );
			this.lblTo = new Gtk.Label( "To:" );
			this.frmData = new Gtk.Frame( "<b>Data</b>" );

			// Prepare limits
			if ( end > max ) {
				end = max;
			}

			// Prepare dialog
            this.Build();
            this.Title = StrActionType[ (int) action ] + ' ' + StrItemType[ (int) item ];
			this.Icon = parent.Icon;
			this.Parent = parent;
			this.TransientFor = parent;
			this.SetPosition( Gtk.WindowPosition.CenterOnParent );
            this.ShowAll();

			// Prepare widgets
			sbFrom.SetRange( 1, max );
			sbTo.SetRange( 1, max );
			sbFrom.Value = org;
			sbTo.Value = end;
		}

		private void Build() {
			var vBox = new Gtk.Box( Gtk.Orientation.Vertical, 2 );
			var hBoxFrom = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );
			var hBoxTo = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );

			// To
			hBoxTo.PackStart( this.lblTo, false, false, 5 );
			hBoxTo.PackStart( this.sbTo, true, true, 5 );

			// From
			hBoxFrom.PackStart( this.lblFrom, false, false, 5 );
			hBoxFrom.PackStart( this.sbFrom, true, true, 5 );

			// Layout
            ((Gtk.Label) this.frmData.LabelWidget ).UseMarkup = true;
			vBox.PackStart( hBoxFrom, true, true, 5 );
			vBox.PackStart( hBoxTo, true, true, 5 );
			this.frmData.Add( vBox );
			this.ContentArea.PackStart( this.frmData, true, true, 5 );

			// Buttons
			this.AddButton( Gtk.Stock.Cancel, Gtk.ResponseType.Cancel );
			this.AddButton( Gtk.Stock.Ok, Gtk.ResponseType.Ok );
			this.DefaultResponse = Gtk.ResponseType.Ok;
            this.Resizable = false;
		}

		private readonly Gtk.Frame frmData;
		private readonly Gtk.SpinButton sbFrom;
		private readonly Gtk.SpinButton sbTo;
		private readonly Gtk.Label lblFrom;
		private readonly Gtk.Label lblTo;
	}
}
