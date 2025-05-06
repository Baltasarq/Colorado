// Colorado (c) 2015-2018 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Gui {
    using Core;

	public partial class DlgImport : Gtk.Dialog {
		public DlgImport(Gtk.Window parent) {
			this.Build();
			
			this.Title = parent.Title + " import";
			this.Icon = parent.Icon;
			this.Parent = parent;
			this.TransientFor = parent;
			this.SetPosition( Gtk.WindowPosition.CenterOnParent );
            this.ShowAll();

			// Last file name opened
			lastFileName = ( (MainWindow) parent ).lastFileName;
            this.edFileName.Text = lastFileName;
            this.DetermineAcceptance();
            this.edFileName.Changed += (o, evt) => this.DetermineAcceptance();
			
			// Add delimiters to the delimiter combo
			foreach(string delimiterName in Core.Delimiter.PredefinedDelimiterNames) {
				this.cmbDelimiter.AppendText( delimiterName );
			}
			this.cmbDelimiter.Entry.Text = Core.Delimiter.TabDelimiterName;

            // Add types to the type combo
            foreach(Importer importer in Importer.GetAllImporters()) {
                if ( importer.GetType() != typeof( Core.Importers.CsvImporter ) ) {
                    this.cmbType.AppendText( importer.Id );                        
                }
            }
            this.cmbType.InsertText( 0, Core.Importers.CsvImporter.Name );
            this.cmbType.Active = 0;
            this.cmbType.Changed += (o, evt) => {
                this.frmOptions.Sensitive = ( this.cmbType.Active == 0 );
            };
		}

		void Build()
        {
            var hBoxType = new Gtk.HBox( false, 2 );
			var hBoxFileName = new Gtk.HBox( false, 2 );
			var hBoxDelimiter = new Gtk.HBox( false, 2 );
			var vBoxFileName = new Gtk.VBox( false, 2 );
			var vBoxOptions = new Gtk.VBox( false, 2 );

            // Type selector
            this.frmType = new Gtk.Frame( "<b>Format</b>" );
            ((Gtk.Label) this.frmType.LabelWidget ).UseMarkup = true;
            this.lblType = new Gtk.Label( "Type: " );
            this.cmbType = new Gtk.ComboBox( new string[]{} );
            hBoxType.PackStart( this.lblType, false, false, 5 );
            hBoxType.PackStart( this.cmbType, true, true, 5 );
            this.frmType.Add( hBoxType );

			// File name
			this.frmFileName = new Gtk.Frame( "<b>Name</b>" );
            ((Gtk.Label) this.frmFileName.LabelWidget ).UseMarkup = true;
			this.lblFileName = new Gtk.Label( "File name:" );
			this.edFileName = new Gtk.Entry();
			this.btOpen = new Gtk.Button( Gtk.Stock.Open );
			this.btOpen.Clicked += (sender, e) => this.OnOpen();
			hBoxFileName.PackStart( this.lblFileName, false, false, 5 );
			hBoxFileName.PackStart( this.edFileName, true, true, 5 );
			hBoxFileName.PackStart( this.btOpen, false, false, 5 );

			vBoxFileName.PackStart( hBoxFileName, true, true, 5 );
			this.frmFileName.Add( vBoxFileName );

			// Options
			this.frmOptions = new Gtk.Frame( "<b>Options</b>" );
            ((Gtk.Label) this.frmOptions.LabelWidget ).UseMarkup = true;
			this.lblDelimiter = new Gtk.Label( "Delimiter" );
			this.cmbDelimiter = new Gtk.ComboBoxEntry( new string[]{} );
			this.cbFirstRowForHeaders = new Gtk.CheckButton( "First row is for headers" );
			hBoxDelimiter.PackStart( this.lblDelimiter, false, false, 5 );
			hBoxDelimiter.PackStart( this.cmbDelimiter, true, true, 5 );
			vBoxOptions.PackStart( hBoxDelimiter, true, true, 5 );
			vBoxOptions.PackStart( this.cbFirstRowForHeaders, true, true, 5 );
			this.frmOptions.Add( vBoxOptions );

			// Layout
            this.VBox.PackStart( this.frmType, true, true, 5 );
			this.VBox.PackStart( this.frmFileName, true, true, 5 );
			this.VBox.PackStart( this.frmOptions, true, true, 5 );

			// Buttons
			this.AddButton( Gtk.Stock.Cancel, Gtk.ResponseType.Cancel );
            this.btOk = (Gtk.Button) this.AddButton( Gtk.Stock.Ok, Gtk.ResponseType.Ok );
			this.DefaultResponse = Gtk.ResponseType.Ok;
		}

        public void DetermineAcceptance()
        {
            this.btOk.Sensitive = ( this.edFileName.Text.Trim().Length > 0 );
        }

        Gtk.Button btOk;
        Gtk.Frame frmType;
        Gtk.Label lblType;
		Gtk.Frame frmFileName;
		Gtk.Frame frmOptions;
		Gtk.Label lblFileName;
		Gtk.Entry edFileName;
		Gtk.Button btOpen;
		Gtk.CheckButton cbFirstRowForHeaders;
		Gtk.Label lblDelimiter;
		Gtk.ComboBoxEntry cmbDelimiter;
        Gtk.ComboBox cmbType;
	}
}
