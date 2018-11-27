// Colorado (c) 2015/18 Baltasar MIT License <baltasarq@gmail.com>


namespace Colorado.Core.Exporters
{
    using System;
    using System.Text;
    using System.IO;

    /// <summary>Exports the document to UTF-8 text.</summary>
    public class TxtExporter: Exporter
    {
        /// <summary>The default delimiter, four spaces.</summary>
        public const string DefaultDelimiter = "    ";

        public TxtExporter(CsvDocument doc, ExportOptions opts)
            :base( doc, opts )
        {
            this.Delimiter = DefaultDelimiter;
        }

        public override void Save()
        {
            string fileName = this.Options.Name;

            using(var f = new StreamWriter(
                            new FileStream( fileName, FileMode.Create, FileAccess.Write ),
                            Encoding.UTF8 ))
            {
                string delimiter;

                // Write headers
                if ( this.Options.IncludeRowNumbers ) {
                    f.Write( '#' );
                    f.Write( this.Delimiter );
                }

                delimiter = "";
                foreach(int i in this.Options.ColumnsIncluded) {
                    string header = this.Document.Data.ColumnInfo[ i ].Header;

                    f.Write( delimiter );
                    f.Write( header );

                    delimiter = this.Delimiter;
                }

                f.WriteLine();

                // Write data
                for(int i = 0; i < this.Document.Data.NumRows; ++i) {
                    // Row index
                    if ( this.Options.IncludeRowNumbers ) {
                        f.Write( ( i + 1 ).ToString() );
                        f.Write( this.Delimiter );
                    }

                    // Data
                    delimiter = "";
                    foreach(int col in this.Options.ColumnsIncluded) {
                        string data = this.Document.Data[ i, col ];

                        f.Write( delimiter );
                        f.Write( data );
                        delimiter = this.Delimiter;
                    }

                    f.WriteLine();
                }
            }

            return;
        }

        /// <summary>
        /// Gets or sets the delimiter among data.
        /// </summary>
        /// <value>The delimiter.</value>
        public string Delimiter {
            get; set;
        }
    }
}
