using Core = Colorado.Core;

namespace Colorado.Gui
{
	public partial class DlgCsvExport : Gtk.Dialog {
		public DlgCsvExport(Gtk.Window parent, Core.CsvDocument doc)
		{
			this.Build();
            this.ShowAll();
			
			this.Title = parent.Title + " export as CSV";
			this.Icon = parent.Icon;
			this.Parent = parent;
			this.TransientFor = parent;
			
			// Add items to the combo
			foreach(string delimiter in Core.Delimiter.PredefinedDelimiterNames) {
				this.cmbDelimiter.AppendText( delimiter );
			}
			
			// Set options honoring current document
			this.cmbDelimiter.Entry.Text = doc.Delimiter.Name;
			chkQuotes.Active = doc.SurroundText;
		}
		
		public string Delimiter {
			get {
				string delimiter = cmbDelimiter.Entry.Text.Trim();
				
				if ( delimiter.Length > 0 )
						return this.cmbDelimiter.Entry.Text;
				else 	return Core.Delimiter.TabDelimiterName;
			}
		}
		
		public bool SurroundWithDoubleQuotes {
			get { return this.chkQuotes.Active; }
		}

        private void Build() {
            var hBoxDelimiter = new Gtk.HBox( false, 2 );

            // Delimiter
            this.lblDelimiter = new Gtk.Label( "Delimiter:" );
            this.cmbDelimiter = new Gtk.ComboBoxEntry( new string[] { ",", ";" } );
            hBoxDelimiter.PackStart( this.lblDelimiter, false, false, 5 );
            hBoxDelimiter.PackStart( this.cmbDelimiter, true, true, 5 );

            // Chk quotes
            this.chkQuotes = new Gtk.CheckButton( "_Enclose text with quotes" );

            // Layout
            this.VBox.PackStart( hBoxDelimiter );
            this.VBox.PackStart( this.chkQuotes );

            // Buttons
            this.AddButton( Gtk.Stock.Cancel, Gtk.ResponseType.Cancel );
            this.AddButton( Gtk.Stock.Ok, Gtk.ResponseType.Ok );
            this.DefaultResponse = Gtk.ResponseType.Ok;
        }

        private Gtk.Label lblDelimiter;
        private Gtk.ComboBoxEntry cmbDelimiter;
        private Gtk.CheckButton chkQuotes;
	}
}
