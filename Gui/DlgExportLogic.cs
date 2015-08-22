using System.Collections.Generic;

using GtkUtil;

using Colorado.Core;

namespace Colorado.Gui {
    public partial class DlgExport {
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
        public string DelimiterValue {
            get {
                return Delimiter.GetName( cmbDelimiter.Entry.Text.Trim() );
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

        private CsvDocument document;
        public static string lastFileName;
    }
}

