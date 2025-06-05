// Colorado (c) 2015-2018 Baltasar MIT License <baltasarq@gmail.com>


namespace Colorado.Gui {
	using Core;

	public partial class DlgImport : Gtk.Dialog {
		public DlgImport(Gtk.Window parent)
		{
			this.edFileName = new Gtk.Entry();
			this.cmbDelimiter = Gtk.ComboBoxText.NewWithEntry();
			this.cmbDelimiter.Entry.IsEditable = true;
			this.cmbType = new Gtk.ComboBoxText();
			this.frmOptions = new Gtk.Frame( "<b>Options</b>" );
			this.frmType = new Gtk.Frame( "<b>Format</b>" );
			this.lblType = new Gtk.Label( "Type: " );
			this.frmFileName = new Gtk.Frame( "<b>Name</b>" );
			this.lblFileName = new Gtk.Label( "File name:" );
			this.btOpen = new Gtk.Button( Gtk.Stock.Open );
			this.lblDelimiter = new Gtk.Label( "Delimiter" );
			this.cbFirstRowForHeaders = new Gtk.CheckButton( "First row is for headers" );
			this.btOk = (Gtk.Button) this.AddButton( Gtk.Stock.Ok, Gtk.ResponseType.Ok );
			this.Build();

			this.Title = parent.Title + " import";
			this.Icon = parent.Icon;
			this.Parent = parent;
			this.TransientFor = parent;
			this.SetPosition( Gtk.WindowPosition.CenterOnParent );
			this.ShowAll();

			// Last file name opened
			lastFileName = ( (MainWindow) parent).LastFileName;
			this.edFileName.Text = lastFileName;
			this.DetermineAcceptance();
			this.edFileName.Changed += (o, evt) => this.DetermineAcceptance();

			// Add delimiters to the delimiter combo
			foreach (string delimiterName in Core.Delimiter.PredefinedDelimiterNames)
			{
				this.cmbDelimiter.AppendText( delimiterName );
			}
			this.cmbDelimiter.Entry.Text = Core.Delimiter.TabDelimiterName;

			// Add types to the type combo
			foreach (Importer importer in Importer.GetAllImporters()) {
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
			var hBoxType = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );
			var hBoxFileName = new Gtk.Box( Gtk.Orientation.Horizontal, 2  );
			var hBoxDelimiter = new Gtk.Box( Gtk.Orientation.Horizontal, 2);
			var vBoxFileName = new Gtk.Box( Gtk.Orientation.Vertical, 2 );
			var vBoxOptions = new Gtk.Box( Gtk.Orientation.Vertical, 2 );

			// Type selector
			( (Gtk.Label) this.frmType.LabelWidget ).UseMarkup = true;
			hBoxType.PackStart( this.lblType, false, false, 5 );
			hBoxType.PackStart( this.cmbType, true, true, 5 );
			this.frmType.Add( hBoxType );

			// File name
			( (Gtk.Label) this.frmFileName.LabelWidget ).UseMarkup = true;
			this.btOpen.Clicked += (sender, e) => this.OnOpen();
			hBoxFileName.PackStart( this.lblFileName, false, false, 5 );
			hBoxFileName.PackStart( this.edFileName, true, true, 5 );
			hBoxFileName.PackStart( this.btOpen, false, false, 5 );

			vBoxFileName.PackStart( hBoxFileName, true, true, 5 );
			this.frmFileName.Add( vBoxFileName );

			// Options
			( (Gtk.Label) this.frmOptions.LabelWidget ).UseMarkup = true;
			hBoxDelimiter.PackStart( this.lblDelimiter, false, false, 5 );
			hBoxDelimiter.PackStart( this.cmbDelimiter, true, true, 5 );
			vBoxOptions.PackStart( hBoxDelimiter, true, true, 5 );
			vBoxOptions.PackStart( this.cbFirstRowForHeaders, true, true, 5 );
			this.frmOptions.Add( vBoxOptions );

			// Layout
			this.ContentArea.PackStart( this.frmType, true, true, 5 );
			this.ContentArea.PackStart( this.frmFileName, true, true, 5 );
			this.ContentArea.PackStart( this.frmOptions, true, true, 5 );

			// Buttons
			this.AddButton( Gtk.Stock.Cancel, Gtk.ResponseType.Cancel);
			this.DefaultResponse = Gtk.ResponseType.Ok;
		}

		public void DetermineAcceptance()
		{
			if ( this.btOk is not null ) {
				this.btOk.Sensitive = this.edFileName.Text.Trim().Length > 0;
			}
		}

		readonly Gtk.Label lblType;
		readonly Gtk.Frame frmFileName;
		readonly Gtk.Label lblFileName;
		readonly Gtk.Button btOpen;
		readonly Gtk.Label lblDelimiter;
		readonly Gtk.CheckButton cbFirstRowForHeaders;
		readonly Gtk.Button btOk;
		readonly Gtk.Frame frmType;
		readonly Gtk.Entry edFileName;
		readonly Gtk.Frame frmOptions;
		readonly Gtk.ComboBoxText cmbDelimiter;
		readonly Gtk.ComboBoxText cmbType;
	}
}
