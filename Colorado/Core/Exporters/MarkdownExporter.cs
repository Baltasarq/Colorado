// Colorado (c) 2015/18 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core.Exporters
{
    using System.IO;
    using System.Text;

    /// <summary>Exports CSV data to Markdown.</summary>
    public class MarkdownExporter: Exporter {
        public const string Name = "MarkDown";
        const string Extension = "md";

        public override void Save()
        {
            string fileName = this.Options.Path;

            using(var f = new StreamWriter(
                                new FileStream( fileName, FileMode.Create, FileAccess.Write ),
                                Encoding.UTF8 ))
            {
                CsvDocument doc = this.Options.Document;
                string headerUnderline = "";

                f.Write( '|' );
                headerUnderline = "|";

                // Write table header
                if ( this.Options.IncludeRowNumbers ) {
                    f.Write( "#|" );
                    headerUnderline += "-|";
                }

                foreach(int col in this.Options.ColumnsIncluded) {
                    string header = doc.Data.ColumnInfo[ col ].Header;

                    f.Write( header + "|" );
                    headerUnderline += new string( '-', header.Length ) + "|";
                }

                f.WriteLine();
                f.WriteLine( headerUnderline );

                // Write each row
                for(int row = 0; row < doc.Data.NumRows; ++row) {
                    f.Write( '|' );

                    if ( this.Options.IncludeRowNumbers ) {
                        f.Write( ( row + 1 ) + "|" );
                    }

                    foreach(int col in this.Options.ColumnsIncluded) {
                        f.Write( doc.Data[ row, col ] + "|" );
                    }

                    f.WriteLine();
                }
            }

            return;
        }

        /// <summary>Gets the name of the exporter.</summary>
        public override string Id => Name;

        /// <summary>Gets the extension for this exporter's output.</summary>
        public override string FileExtension => Extension;
    }
}
