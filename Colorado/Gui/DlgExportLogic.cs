// Colorado (c) 2015-2018 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Gui {
    using System.Collections.Generic;

    using GtkUtil;

    using Colorado.Core;


    public partial class DlgExport {
        void OnSaveAs()
        {
            // Prepare
            if ( this.cmbOutput.Active < 0 ) {
                this.cmbOutput.Active = 0;
            }

            string filter = "*." + Exporter.GetExporter( this.cmbOutput.ActiveText ).FileExtension;

            if ( Util.DlgSave( AppInfo.Name,
                              "Save as...",
                              (Gtk.Window) this.Parent,
                              ref lastFileName,
                              filter ) )
            {
                this.OnOutputFormatChanged();
            }
            else {
                lastFileName = this.edFile.Text;   
            }

            return;
        }

        void OnOutputFormatChanged()
        {
            // Prepare
            if ( this.cmbOutput.Active < 0 ) {
                this.cmbOutput.Active = 0;
            }

            if ( string.IsNullOrWhiteSpace( lastFileName ) ) {
                lastFileName = "output_file.csv";
            }

            // Options
            if ( this.cmbOutput.ActiveText == Core.Exporters.CsvExporter.Name )
            {
                this.frmCsvOptions.Visible = true;
                this.frmDocOptions.Visible = false;
            } else {
                this.frmCsvOptions.Visible = false;
                this.frmDocOptions.Visible = true;

            }

            // File name
            var exporter = Exporter.GetExporter( this.cmbOutput.ActiveText );
            lastFileName = System.IO.Path.ChangeExtension( lastFileName, "." + exporter.FileExtension );

            this.edFile.Text = lastFileName;
            this.edFile.Position = lastFileName.Length;
        }

        public string ExporterId {
            get { return cmbOutput.ActiveText; }
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

        CsvDocument document;
        public static string lastFileName;
    }
}

