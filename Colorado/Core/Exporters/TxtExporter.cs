// Colorado (c) 2015/18 Baltasar MIT License <baltasarq@gmail.com>


namespace Colorado.Core.Exporters {
    using System.Text;
    using System.IO;

    /// <summary>Exports the document to UTF-8 text.</summary>
    public class TxtExporter: Exporter {
        public const string Name = "Text";
        const string Extension = "txt";

        /// <summary>The default delimiter, four spaces.</summary>
        public const string DefaultDelimiter = "    ";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Colorado.Core.Exporters.TxtExporter"/> class.
        /// </summary>
        public TxtExporter()
        {
            this.Delimiter = DefaultDelimiter;
        }

        public override void Save()
        {
            string fileName = this.Options.Path;

            using(var f = new StreamWriter(
                            new FileStream( fileName, FileMode.Create, FileAccess.Write ),
                            Encoding.UTF8 ))
            {
                CsvDocument doc = this.Options.Document;
                string delimiter;

                // Write headers
                if ( this.Options.IncludeRowNumbers ) {
                    f.Write( '#' );
                    f.Write( this.Delimiter );
                }

                delimiter = "";
                foreach(int i in this.Options.ColumnsIncluded) {
                    string header = doc.Data.ColumnInfo[ i ].Header;

                    f.Write( delimiter );
                    f.Write( header );

                    delimiter = this.Delimiter;
                }

                f.WriteLine();

                // Write data
                for(int i = 0; i < doc.Data.NumRows; ++i) {
                    // Row index
                    if ( this.Options.IncludeRowNumbers ) {
                        f.Write( ( i + 1 ).ToString() );
                        f.Write( this.Delimiter );
                    }

                    // Data
                    delimiter = "";
                    foreach(int col in this.Options.ColumnsIncluded) {
                        string data = doc.Data[ i, col ];

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

        /// <summary>Gets the name of the exporter.</summary>
        public override string Id => Name;

        /// <summary>Gets the extension for this exporter's output.</summary>
        public override string FileExtension => Extension;
    }
}
