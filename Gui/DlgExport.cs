
using System;
using System.Collections;
using System.Collections.Generic;
using GtkUtil;

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
				
		public ExportOptions.SelectionType Selection {
			get { return ( (ExportOptions.SelectionType) cmbOutput.Active ); }
		}
		
		public string FileName {
			get { return edFile.Text.Trim(); }
		}
		
		public bool IncludeRowNumbers {
			get { return this.cbRowNumbers.Active; }
		}
		
		public bool IncludeTableBorder {
			get { return this.cbTableBorders.Active; }
		}

        /// <summary>
        /// Gets a value indicating whether cells should be surrounde with double quotes.
        /// </summary>
        /// <value><c>true</c> if cells are to be surrounded with double quotes; otherwise, <c>false</c>.</value>
        public bool SurroundWithDoubleQuotes {
            get { return this.chkQuotes.Active; }
        }

        /// <summary>
        /// Gets the delimiter.
        /// </summary>
        /// <value>The delimiter, as a string of one position.</value>
        public string Delimiter {
            get {
                string delimiter = cmbDelimiter.Entry.Text.Trim();

                if ( delimiter.Length > 0 )
                    return this.cmbDelimiter.Entry.Text;
                else    return Core.Delimiter.TabDelimiterName;
            }
        }
		
		/// <summary>
		/// Return the sequence of column indexes to include
		/// </summary>
		public int[] ColumnsIncluded {
			get {
				var colsIncluded = new List<int>();
				for(int i = 0; i < this.fieldChecks.Length; ++i) {
					if ( this.fieldChecks[ i ].Active ) {
						colsIncluded.Add( i );
					}
				}
				
				return colsIncluded.ToArray();
			}
		}
		
		private void OnSaveAs()
		{
			string filter = "*" + ExportOptions.SelectionExtension[ this.cmbOutput.Active ];
			
			if ( Util.DlgSave( AppInfo.Name,
			                  "Save as...",
			                  (Gtk.Window) this.Parent,
			                  ref lastFileName,
			                  filter ) )
			{
				this.OnOutputFormatChanged();
			}
			else lastFileName = this.edFile.Text;
		}
		
		private void OnOutputFormatChanged()
		{
			int active = this.cmbOutput.Active;
			
            // Prepare
			if ( active < 0 ) {
				this.cmbOutput.Active = active = 0;
			}

			if ( string.IsNullOrWhiteSpace( lastFileName ) ) {
				lastFileName = "output_file.csv";
			}

            // Options
            if ( ( (ExportOptions.SelectionType) active ) == ExportOptions.SelectionType.Csv )
            {
                this.frmCsvOptions.Visible = true;
                this.frmDocOptions.Visible = false;
            } else {
                this.frmCsvOptions.Visible = false;
                this.frmDocOptions.Visible = true;

            }

			// File name
			lastFileName = System.IO.Path.ChangeExtension( lastFileName, ExportOptions.SelectionExtension[ active ] );
			this.edFile.Text = lastFileName;
			this.edFile.Position = lastFileName.Length;
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

		private CsvDocument document;
		public static string lastFileName;
	}
}
