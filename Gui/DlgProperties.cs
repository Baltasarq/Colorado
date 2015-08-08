using System;

using GtkUtil;

using Colorado.Core;

namespace Colorado.Gui {
	public partial class DlgProperties : Gtk.Dialog {
		private CsvDocument document;
		
        /// <summary>
        /// Initializes a new instance of the <see cref="Colorado.Gui.DlgProperties"/> class.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="doc">Document.</param>
		public DlgProperties(Gtk.Window parent, CsvDocument doc) {
            var minSize = new Gdk.Geometry() {
                MinWidth = 400,
                MinHeight = 300
            };

            this.document = doc;

			// Prepare dialog
            this.Build();
			this.Title = parent.Title + " properties";
			this.Icon = parent.Icon;
			this.Parent = parent;
			this.TransientFor = parent;
			this.SetPosition( Gtk.WindowPosition.CenterOnParent );
            this.WindowPosition = Gtk.WindowPosition.CenterOnParent;
            this.SetDefaultSize( minSize.MinHeight, minSize.MinWidth );
            this.SetGeometryHints( this, minSize, Gdk.WindowHints.MinSize );
			
			// Sync info
			sbRows.Value = document.Data.NumRows;
			sbColumns.Value = document.Data.NumColumns;
            cbSurroundWithDoubleQuotes.Active = document.SurroundText;
            cbFirstRowForHeaders.Active = document.Data.FirstRowForHeaders;
            this.UpdateColumnsData();
            this.ShowAll();
		}
		
		protected void UpdateColumnsData()
		{
			var listStore = ( (Gtk.ListStore) listHeaders.Model );

			// Insert data
			listStore.Clear();
			foreach(ColumnInfo colInfo in document.Data.ColumnInfo) {
				listStore.AppendValues( colInfo.Header );
			}
		}
		
		protected void OnHeaderEdited(object sender, Gtk.EditedArgs args)
		{
			int row;
			var rowPath = new Gtk.TreePath( args.Path );
			Gtk.TreeIter rowPointer;
			
			// Update GUI
			listHeaders.Model.GetIter( out rowPointer, rowPath );
			row = rowPath.Indices[ 0 ];
			listHeaders.Model.SetValue( rowPointer, 0, args.NewText );
			
			// Update info in document
			document.Data.ColumnInfo[ row ].Header = args.NewText;
			document.Changed = true;
		}
		
        /// <summary>
        /// Applies the preferences.
        /// </summary>
		public void ApplyPreferences()
		{
			// Get delimiter
			string delimiter = cmbDelimiter.Entry.Text.Trim();
			
			if ( delimiter.Length > 0
			  && delimiter[ 0 ] != document.Delimiter.Raw )
			{
				document.Delimiter.Name = delimiter;
			}

			// Get decimal mark
			document.DecimalMark = (CsvDocument.DecimalSeparator) cmbDecimalMark.Active;
			
			// Get surround text
            document.SurroundText = this.cbSurroundWithDoubleQuotes.Active;
			
			// Check rows and headers size
			if ( document.Data.NumColumns > ( (int) sbColumns.Value ) ) {
				if ( !Util.Ask( this, AppInfo.Name, "The new column value is lower. This will imply data loss. Are you sure ?" ) ) {
					sbColumns.Value = document.Data.NumColumns;
					sbRows.Value = document.Data.NumRows;
					return;
				}
			}
			
			if ( document.Data.NumRows > ( (int) sbRows.Value ) ) {
				if ( !Util.Ask( this, AppInfo.Name, "The new row value is lower. This will imply data loss. Are you sure ?" ) ) {
					sbColumns.Value = document.Data.NumColumns;
					sbRows.Value = document.Data.NumRows;
					return;
				}
			}
			
			// Now yes, modify the size
			document.Data.NumColumns = (int) sbColumns.Value;
			document.Data.NumRows = (int) sbRows.Value;
			
			// Modify headers, if needed
			if ( document.Data.FirstRowForHeaders != cbFirstRowForHeaders.Active ) {
				document.Data.FirstRowForHeaders = cbFirstRowForHeaders.Active;
				sbRows.Value = Convert.ToDouble( document.Data.NumRows );
			}

            this.UpdateColumnsData();
		}

        private void Build() {
            var hBox = new Gtk.HBox( false, 2 );

            this.BuildHeadersFrame();
            this.BuildPropertiesFrame();

            // Apply button
            this.btApply = new Gtk.Button( Gtk.Stock.Apply );
            this.btApply.Clicked += (obj, args) => this.ApplyPreferences();
            this.ActionArea.PackStart( this.btApply, false, false, 5 );

            // Buttons
            this.AddButton( Gtk.Stock.Close, Gtk.ResponseType.Close );
            this.DefaultResponse = Gtk.ResponseType.Close;

            // Layout
            hBox.PackStart( this.frmHeaders, true, true, 5 );
            hBox.PackStart( this.frmProperties, false, false, 5 );
            this.VBox.PackStart( hBox, true, true, 5 );
        }

        private void BuildHeadersFrame() {
            var swScroll = new Gtk.ScrolledWindow();

            this.frmHeaders = new Gtk.Frame( "<b>Headers</b>" );
            ((Gtk.Label) this.frmHeaders.LabelWidget ).UseMarkup = true;
            this.listHeaders = new Gtk.TreeView();
            swScroll.AddWithViewport( this.listHeaders );
            this.frmHeaders.Add( swScroll );

            // Set the treeview
            var column = new Gtk.TreeViewColumn();
            var cell = new Gtk.CellRendererText();
            Gtk.ListStore listStore = new Gtk.ListStore( typeof( string ) );

            column.Title = "";
            column.PackStart( cell, true );
            cell.Editable = true;
            column.AddAttribute( cell, "text", 0 );
            cell.Edited += OnHeaderEdited;

            this.listHeaders.AppendColumn( column );
            this.listHeaders.Model = listStore;
            this.listHeaders.EnableGridLines = Gtk.TreeViewGridLines.Horizontal;
        }

        private void BuildPropertiesFrame() {
            var vBox = new Gtk.VBox( false, 2 );
            var hBoxRows = new Gtk.HBox( false, 2 );
            var hBoxColumns = new Gtk.HBox( false, 2 );
            var hBoxDelimiter = new Gtk.HBox( false, 2 );
            var hBoxDecimalMark = new Gtk.HBox( false, 2 );

            // Rows
            this.sbRows = new Gtk.SpinButton( 0, 100, 1 );
            this.lblRows = new Gtk.Label( "Rows" );
            hBoxRows.PackStart( this.lblRows, false, false, 5 );
            hBoxRows.PackStart( this.sbRows, true, true, 5 );

            // Columns
            this.sbColumns = new Gtk.SpinButton( 0, 100, 1 );
            this.lblColumns = new Gtk.Label( "Columns" );
            hBoxColumns.PackStart( this.lblColumns, false, false, 5 );
            hBoxColumns.PackStart( this.sbColumns, true, true, 5 );

            // Delimiter
            this.cmbDelimiter = new Gtk.ComboBoxEntry( new string[] {} );
            this.lblDelimiter = new Gtk.Label( "Delimiter" );
            hBoxDelimiter.PackStart( this.lblDelimiter, false, false, 5 );
            hBoxDelimiter.PackStart( this.cmbDelimiter, true, true, 5 );

            // Decimal mark
            this.cmbDecimalMark = new Gtk.ComboBoxEntry( new string[] {} );
            this.lblDecimalMark= new Gtk.Label( "Decimal mark" );
            hBoxDecimalMark.PackStart( this.lblDecimalMark, false, false, 5 );
            hBoxDecimalMark.PackStart( this.cmbDecimalMark, true, true, 5 );

            // Double quotes
            this.cbSurroundWithDoubleQuotes = new Gtk.CheckButton( "Surround with double quotes" );
            this.cbFirstRowForHeaders = new Gtk.CheckButton( "First row for headers" );

            // Layout
            vBox.PackStart( hBoxRows, true, true, 5 );
            vBox.PackStart( hBoxColumns, true, true, 5 );
            vBox.PackStart( hBoxDelimiter, true, false, 5 );
            vBox.PackStart( hBoxDecimalMark, true, false, 5 );
            vBox.PackStart( this.cbFirstRowForHeaders, true, true, 5 );
            vBox.PackStart( this.cbSurroundWithDoubleQuotes, true, true, 5 );
            this.frmProperties = new Gtk.Frame( "<b>Properties</b>" );
            ((Gtk.Label) this.frmProperties.LabelWidget ).UseMarkup = true;
            this.frmProperties.Add( vBox );

            // Add delimiters to its combo
            foreach(string delimiter in Delimiter.PredefinedDelimiterNames) {
                cmbDelimiter.AppendText( delimiter );
            }
            cmbDelimiter.Entry.Text = document.Delimiter.Name;

            // Add decimal separators to its combo
            foreach (char separator in CsvDocument.DecimalSeparatorChar) {
                cmbDecimalMark.AppendText( separator.ToString() );
            }
            cmbDecimalMark.Active = (int) document.DecimalMark;
        }

        private Gtk.Frame frmHeaders;
        private Gtk.Frame frmProperties;
        private Gtk.Button btApply;
        private Gtk.TreeView listHeaders;
        private Gtk.SpinButton sbRows;
        private Gtk.SpinButton sbColumns;
        private Gtk.ComboBoxEntry cmbDelimiter;
        private Gtk.ComboBoxEntry cmbDecimalMark;
        private Gtk.CheckButton cbSurroundWithDoubleQuotes;
        private Gtk.CheckButton cbFirstRowForHeaders;
        private Gtk.Label lblRows;
        private Gtk.Label lblColumns;
        private Gtk.Label lblDelimiter;
        private Gtk.Label lblDecimalMark;
	}
}
