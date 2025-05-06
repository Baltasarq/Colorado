// Colorado (c) 2015-2018 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Gui {
    using Colorado.Core;

	public partial class DlgExport : Gtk.Dialog {
		public DlgExport(Gtk.Window parent, CsvDocument doc)
		{
			document = doc;
			this.Build();
			
			this.Title = parent.Title + " export";
			this.Icon = parent.Icon;
			this.Parent = parent;
			this.TransientFor = parent;
			this.SetPosition( Gtk.WindowPosition.CenterOnParent );
            this.ShowAll();
			
			// Last file name opened
			lastFileName = ( (MainWindow) parent ).lastFileName;

            // Sync dialog
			this.OnOutputFormatChanged();
		}
				
        void Build()
        {
            var hBoxOptions = new Gtk.HBox( false, 2 );

            this.BuildDocOptions();
            this.BuildCsvOptions();
            this.BuildFileFrame();

            // Options
            hBoxOptions.PackStart( this.frmDocOptions, true, true, 5 );
            hBoxOptions.PackStart( this.frmCsvOptions, true, true, 5 );

            // Fields
            this.BuildChecksForFields();

            // Buttons
            this.AddButton( Gtk.Stock.Cancel, Gtk.ResponseType.Cancel );
            this.AddButton( Gtk.Stock.Ok, Gtk.ResponseType.Ok );
            this.DefaultResponse = Gtk.ResponseType.Ok;

            // Layout
            this.VBox.PackStart( this.frmFile, false, false, 5 );
            this.VBox.PackStart( hBoxOptions, false, false, 5 );
            this.VBox.PackStart( this.frmFields, true, true, 5 );
        }

        void BuildChecksForFields()
        {
            // Add fields
            this.fieldChecks = new Gtk.CheckButton[ document.Data.ColumnInfo.Length ];
            var vbox = new Gtk.VBox();

            for(int i = 0; i < document.Data.ColumnInfo.Length; ++i ) {
                fieldChecks[ i ] = new Gtk.CheckButton( document.Data.ColumnInfo[ i ].Header );
                fieldChecks[ i ].Visible = true;
                fieldChecks[ i ].Active = true;
                vbox.Add( fieldChecks[ i ] );
            }

            // Layout
            this.frmFields = new Gtk.Frame( "<b>Fields</b>" );
            ((Gtk.Label) this.frmFields.LabelWidget ).UseMarkup = true;
            this.swScroll = new Gtk.ScrolledWindow();
            this.swScroll.AddWithViewport( vbox );
            this.frmFields.Add( this.swScroll );
        }

        void BuildFileFrame()
        {
            var vBoxFile = new Gtk.VBox( false, 2 );
            var hBoxFileName = new Gtk.HBox( false, 2 );
            var hBoxOutput = new Gtk.HBox( false, 2 );

            // File name
            this.edFile = new Gtk.Entry();
            this.btSaveAs = new Gtk.Button( Gtk.Stock.SaveAs );
            this.btSaveAs.Clicked += (obj, args) => this.OnSaveAs();
            this.lblFile = new Gtk.Label( "File:" );

            hBoxFileName.PackStart( this.lblFile, false, false, 5 );
            hBoxFileName.PackStart( this.edFile, true, true, 5 );
            hBoxFileName.PackStart( this.btSaveAs, false, false, 5 );

            // Output format
            BuildAllFormatOptions();
            this.lblOutput = new Gtk.Label( "Output format:" );
            this.cmbOutput = new Gtk.ComboBox( formatOptions ) { Active = 0 };
            this.cmbOutput.Changed += (obj, args) => this.OnOutputFormatChanged();

            hBoxOutput.PackStart( this.lblOutput, false, false, 5 );
            hBoxOutput.PackStart( this.cmbOutput, true, true, 5 );

            // Layout
            vBoxFile.PackStart( hBoxFileName, false, false, 5 );
            vBoxFile.PackStart( hBoxOutput, false, false, 5 );
            this.frmFile = new Gtk.Frame( "<b>File</b>" );
            ((Gtk.Label) this.frmFile.LabelWidget ).UseMarkup = true;
            this.frmFile.Add( vBoxFile );
        }

        void BuildDocOptions()
        {
            var vBoxDocOptions = new Gtk.VBox( false, 2 );

            // Doc options
            this.frmDocOptions = new Gtk.Frame( "<b>Document options</b>" );
            ((Gtk.Label) this.frmDocOptions.LabelWidget ).UseMarkup = true;
            this.cbRowNumbers = new Gtk.CheckButton( "Include row numbers" );
            this.cbTableBorders = new Gtk.CheckButton( "Include table borders" );
            vBoxDocOptions.PackStart( this.cbRowNumbers, false, false, 5 );
            vBoxDocOptions.PackStart( this.cbTableBorders, false, false, 5 );

            this.frmDocOptions.Add( vBoxDocOptions );
        }

        void BuildCsvOptions()
        {
            var hBoxDelimiter = new Gtk.HBox( false, 2 );
            var vBoxOptions = new Gtk.VBox( false, 2 );

            // Delimiter
            var delimiters = new string[ Delimiter.PredefinedDelimiters.Count ];
            Delimiter.PredefinedDelimiterNames.CopyTo( delimiters, 0 );
            this.lblDelimiter = new Gtk.Label( "Delimiter:" );
            this.cmbDelimiter = new Gtk.ComboBoxEntry( delimiters );
            hBoxDelimiter.PackStart( this.lblDelimiter, false, false, 5 );
            hBoxDelimiter.PackStart( this.cmbDelimiter, true, true, 5 );

            // Chk quotes
            this.chkQuotes = new Gtk.CheckButton( "Enclose text with quotes" );

            // Set options honoring current document
            this.cmbDelimiter.Entry.Text = Delimiter.GetName( this.document.DelimiterValue );
            chkQuotes.Active = this.document.SurroundText;

            // Layout
            vBoxOptions.PackStart( hBoxDelimiter );
            vBoxOptions.PackStart( this.chkQuotes );
            this.frmCsvOptions = new Gtk.Frame( "<b>Csv options</b>" );
            ((Gtk.Label) this.frmCsvOptions.LabelWidget ).UseMarkup = true;
            this.frmCsvOptions.Add( vBoxOptions );
        }

        static void BuildAllFormatOptions()
        {
            if ( formatOptions == null ) {
                Exporter[] exporters = Exporter.GetAllExporters();

                formatOptions = new string[ exporters.Length ];
                for(int i = 0; i < exporters.Length; ++i) {
                    formatOptions[ i ] = exporters[ i ].Id;
                }
            }

            return;
        }

        static string[] formatOptions;

        Gtk.Frame frmFields;
        Gtk.Frame frmDocOptions;
        Gtk.Frame frmCsvOptions;
        Gtk.Frame frmFile;
        Gtk.Label lblFile;
        Gtk.Label lblOutput;
        Gtk.Entry edFile;
        Gtk.Button btSaveAs;
        Gtk.ComboBox cmbOutput;
        Gtk.ScrolledWindow swScroll;
        Gtk.CheckButton cbRowNumbers;
        Gtk.CheckButton cbTableBorders;
        Gtk.Label lblDelimiter;
        Gtk.ComboBoxEntry cmbDelimiter;
        Gtk.CheckButton chkQuotes;
        Gtk.CheckButton[] fieldChecks;
	}
}
