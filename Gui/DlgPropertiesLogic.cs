using GtkUtil;

using Colorado.Core;

namespace Colorado.Gui {
    public partial class DlgProperties {
        private void UpdateColumnsData()
        {
            var listStore = ( (Gtk.ListStore) listHeaders.Model );

            // Insert data
            listStore.Clear();
            foreach(ColumnInfo colInfo in document.Data.ColumnInfo) {
                listStore.AppendValues( colInfo.Header );
            }
        }

        private void OnHeaderEdited(object sender, Gtk.EditedArgs args)
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
                this.document.Delimiter.Name = delimiter;
            }

            // Get surround text
            this.document.SurroundText = this.cbSurroundWithDoubleQuotes.Active;

            // Check rows and headers size
            if ( this.document.Data.NumColumns > ( (int) sbColumns.Value ) ) {
                if ( !Util.Ask( this, AppInfo.Name, "The new column value is lower. This will imply data loss. Are you sure ?" ) ) {
                    sbColumns.Value = this.document.Data.NumColumns;
                    sbRows.Value = this.document.Data.NumRows;
                    return;
                }
            }

            if ( this.document.Data.NumRows > ( (int) sbRows.Value ) ) {
                if ( !Util.Ask( this, AppInfo.Name, "The new row value is lower. This will imply data loss. Are you sure ?" ) ) {
                    sbColumns.Value = this.document.Data.NumColumns;
                    sbRows.Value = this.document.Data.NumRows;
                    return;
                }
            }

            // Now yes, modify the size
            this.document.Data.NumColumns = (int) sbColumns.Value;
            this.document.Data.NumRows = (int) sbRows.Value;

            // Modify the decimal mark
            if ( this.DecimalMark != document.DecimalMark ) {
                char chOldDecimalMark = CsvDocument.DecimalSeparatorChar[ (int) this.document.DecimalMark ];
                char chNewDecimalMark = CsvDocument.DecimalSeparatorChar[ (int) this.DecimalMark ];

                for(int i = 0; i < this.document.Data.NumRows; ++i) {
                    for(int j = 0; j < this.document.Data.NumColumns; ++j) {
                        string cell = this.document.Data[ i, j ];

                        if ( CsvDocument.IsNumber( cell ) ) {
                            this.document.Data[ i, j ] = cell.Replace( chOldDecimalMark, chNewDecimalMark );
                        }
                    }
                }
            }

            // Modify headers, if needed
            if ( this.document.Data.FirstRowForHeaders != cbFirstRowForHeaders.Active ) {
                this.document.Data.FirstRowForHeaders = cbFirstRowForHeaders.Active;
                sbRows.Value = System.Convert.ToDouble( this.document.Data.NumRows );
            }

            this.UpdateColumnsData();
        }

        public CsvDocument.DecimalSeparator DecimalMark {
            get {
                return (CsvDocument.DecimalSeparator) this.cmbDecimalMark.Active;
            }
        }

        private CsvDocument document;       
    }
}

