// Colorado (c) 2015/18 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core.Exporters
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>Exports CSV data to Html.</summary>
    public class HtmlExporter: Exporter {
        public const string Name = "HTML";
        const string Extension = "html";

        public override void Save()
        {
            CsvDocument doc = this.Options.Document;
            string fileName = this.Options.Path;
            int tableBorder = 0;

            // Determine border
            if ( this.Options.IncludeTableBorder ) {
                ++tableBorder;
            }

            // Open file for saving
            using (var file = new StreamWriter(
                                new FileStream( fileName, FileMode.Create, FileAccess.Write ),
                                Encoding.UTF8 ))
            {
                // Write html header
                file.WriteLine( "<html><header>" );
                file.WriteLine( "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">" );
                file.WriteLine( "<title>"
                               + Path.GetFileNameWithoutExtension( fileName )
                               + "</title>" );

                file.WriteLine( "</header><body><table border={0}>\n", tableBorder );

                // Write headers
                file.WriteLine( "<tr>\n" );

                if ( this.Options.IncludeRowNumbers ) {
                    file.Write( "<td style=\"color: white; background-color: black;\"><b>#</b></td>" );
                }

                for(int col = 0; col < doc.Data.NumColumns; ++col) {
                    if ( this.Options.IsColumnIncluded( col ) ) {
                        file.Write( "<td style=\"color: white; background-color: black;\"><b>"
                                   + doc.Data.ColumnInfo[ col ].Header + "</b></td>"
                                  );
                    }
                }
                file.WriteLine();

                // Write each row
                for(int row = 0; row < doc.Data.NumRows; ++row) {
                    file.WriteLine( "<tr>\n" );

                    if ( this.Options.IncludeRowNumbers ) {
                        file.Write( "<td style=\"color: black; background-color: rgb(204,204,204);\"><b>" + Convert.ToString( row +1 ) + "</b></td>" );
                    }

                    for(int col = 0; col < doc.Data.NumColumns; ++col) {       
                        if ( this.Options.IsColumnIncluded( col ) ) {

                            file.Write( "<td" );

                            if ( ( row % 2 ) == 0 ) {
                                file.Write( " style=\"color: black; background-color: rgb(204,204,204);\"" );
                            }

                            file.Write( ">" + doc.Data[ row, col ] + "</td>" );
                        }
                    }
                    file.WriteLine();
                }

                // End
                file.WriteLine( "</table></body></html>\n" );
            }

            return;
        }

        /// <summary>Gets the name of the exporter.</summary>
        public override string Id => Name;

        /// <summary>Gets the extension for this exporter's output.</summary>
        public override string FileExtension => Extension;
    }
}
