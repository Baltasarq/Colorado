using System;

using Colorado.Core;

namespace Colorado.Gui {
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
				
        private void Build() {
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

        private void BuildChecksForFields()
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

        private void BuildFileFrame() {
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
            string[] formatOptions = new string[ ExportOptions.SelectionExtension.Count ];
            ExportOptions.SelectionExplanation.CopyTo( formatOptions, 0 );
            this.lblOutput = new Gtk.Label( "Output format:" );
            this.cmbOutput = new Gtk.ComboBox( formatOptions );
            this.cmbOutput.Changed += (obj, args) => this.OnOutputFormatChanged();
            this.cmbOutput.Active = 0;
            hBoxOutput.PackStart( this.lblOutput, false, false, 5 );
            hBoxOutput.PackStart( this.cmbOutput, true, true, 5 );

            // Layout
            vBoxFile.PackStart( hBoxFileName, false, false, 5 );
            vBoxFile.PackStart( hBoxOutput, false, false, 5 );
            this.frmFile = new Gtk.Frame( "<b>File</b>" );
            ((Gtk.Label) this.frmFile.LabelWidget ).UseMarkup = true;
            this.frmFile.Add( vBoxFile );
        }

        private void BuildDocOptions() {
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

        private void BuildCsvOptions() {
            var hBoxDelimiter = new Gtk.HBox( false, 2 );
            var vBoxOptions = new Gtk.VBox( false, 2 );

            // Delimiter
            var delimiters = new string[ Core.Delimiter.PredefinedDelimiters.Count ];
            Core.Delimiter.PredefinedDelimiterNames.CopyTo( delimiters, 0 );
            this.lblDelimiter = new Gtk.Label( "Delimiter:" );
            this.cmbDelimiter = new Gtk.ComboBoxEntry( delimiters );
            hBoxDelimiter.PackStart( this.lblDelimiter, false, false, 5 );
            hBoxDelimiter.PackStart( this.cmbDelimiter, true, true, 5 );

            // Chk quotes
            this.chkQuotes = new Gtk.CheckButton( "Enclose text with quotes" );

            // Set options honoring current document
            this.cmbDelimiter.Entry.Text = this.document.Delimiter.Name;
            chkQuotes.Active = this.document.SurroundText;

            // Layout
            vBoxOptions.PackStart( hBoxDelimiter );
            vBoxOptions.PackStart( this.chkQuotes );
            this.frmCsvOptions = new Gtk.Frame( "<b>Csv options</b>" );
            ((Gtk.Label) this.frmCsvOptions.LabelWidget ).UseMarkup = true;
            this.frmCsvOptions.Add( vBoxOptions );
        }

        private Gtk.Frame frmFields;
        private Gtk.Frame frmDocOptions;
        private Gtk.Frame frmCsvOptions;
        private Gtk.Frame frmFile;
        private Gtk.Label lblFile;
        private Gtk.Label lblOutput;
        private Gtk.Entry edFile;
        private Gtk.Button btSaveAs;
        private Gtk.ComboBox cmbOutput;
        private Gtk.ScrolledWindow swScroll;
        private Gtk.CheckButton cbRowNumbers;
        private Gtk.CheckButton cbTableBorders;
        private Gtk.Label lblDelimiter;
        private Gtk.ComboBoxEntry cmbDelimiter;
        private Gtk.CheckButton chkQuotes;
        private Gtk.CheckButton[] fieldChecks;
	}
}
