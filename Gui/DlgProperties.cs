using Gtk;
using System;

using GtkUtil;

using Colorado.Core;

namespace Colorado.Gui {
	public partial class DlgProperties : Gtk.Dialog {
		private CsvDocument document;
		
		public DlgProperties(Gtk.Window parent, CsvDocument doc) {
			this.Build();
			
			this.document = doc;
			
			// Prepare dialog
			this.Title = parent.Title + " properties";
			this.Icon = parent.Icon;
			this.Parent = parent;
			this.TransientFor = parent;
			this.SetPosition( Gtk.WindowPosition.CenterOnParent );
            this.SetResponseSensitive( ResponseType.Close, true );
			
			// Set the widgets correctly
			Gtk.ListStore listStore = new Gtk.ListStore( typeof( string ) );
			listHeaders.Model = listStore;
			var column = new Gtk.TreeViewColumn();
			var cell = new Gtk.CellRendererText();
			column.Title = "Headers";
			column.PackStart( cell, true );
			cell.Editable = true;
			column.AddAttribute( cell, "text", 0 );
			cell.Edited += OnHeaderEdited;
			listHeaders.AppendColumn( column );
			listHeaders.EnableGridLines = Gtk.TreeViewGridLines.Both;
			
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

			// Set info
			UpdateColumnsData();
			sbRows.Value = document.Data.NumRows;
			sbColumns.Value = document.Data.NumColumns;
			cbSurroundText.Active = document.SurroundText;
			cbFirstRowHeaders.Active = document.Data.FirstRowForHeaders;
		}
		
		protected void UpdateColumnsData()
		{
			var listStore = ( (ListStore) listHeaders.Model );

			// Insert data
			listStore.Clear();
			foreach(ColumnInfo colInfo in document.Data.ColumnInfo) {
				listStore.AppendValues( colInfo.Header );
			}
		}
		
		protected void OnHeaderEdited(object sender, EditedArgs args)
		{
			int row;
			var rowPath = new Gtk.TreePath( args.Path );
			TreeIter rowPointer;
			
			// Update GUI
			listHeaders.Model.GetIter( out rowPointer, rowPath );
			row = rowPath.Indices[ 0 ];
			listHeaders.Model.SetValue( rowPointer, 0, args.NewText );
			
			// Update info in document
			document.Data.ColumnInfo[ row ].Header = args.NewText;
			document.Changed = true;
		}
		
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
			document.SurroundText = cbSurroundText.Active;
			
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
			if ( document.Data.FirstRowForHeaders != cbFirstRowHeaders.Active ) {
				document.Data.FirstRowForHeaders = cbFirstRowHeaders.Active;
				sbRows.Value = Convert.ToDouble( document.Data.NumRows );
			}
		}

		protected void OnApply(object sender, EventArgs e)
		{
			ApplyPreferences();
			UpdateColumnsData();
		}

	}
}
