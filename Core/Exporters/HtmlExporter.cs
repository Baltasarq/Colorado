// Colorado (c) 2015/18 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core.Exporters
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>Exports CSV data to Html.</summary>
    public class HtmlExporter: Exporter
    {
        public HtmlExporter(CsvDocument doc, ExportOptions options)
            :base( doc, options )
        {
        }

        public override void Save()
        {
            string fileName = this.Options.Name;
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

                for(int col = 0; col < Document.Data.NumColumns; ++col) {
                    if ( this.Options.IsColumnIncluded( col ) ) {
                        file.Write( "<td style=\"color: white; background-color: black;\"><b>"
                                   + Document.Data.ColumnInfo[ col ].Header + "</b></td>"
                                  );
                    }
                }
                file.WriteLine();

                // Write each row
                for(int row = 0; row < Document.Data.NumRows; ++row) {
                    file.WriteLine( "<tr>\n" );

                    if ( this.Options.IncludeRowNumbers ) {
                        file.Write( "<td style=\"color: black; background-color: rgb(204,204,204);\"><b>" + Convert.ToString( row +1 ) + "</b></td>" );
                    }

                    for(int col = 0; col < Document.Data.NumColumns; ++col) {       
                        if ( this.Options.IsColumnIncluded( col ) ) {

                            file.Write( "<td" );

                            if ( ( row % 2 ) == 0 ) {
                                file.Write( " style=\"color: black; background-color: rgb(204,204,204);\"" );
                            }

                            file.Write( ">" + Document.Data[ row, col ] + "</td>" );
                        }
                    }
                    file.WriteLine();
                }

                // End
                file.WriteLine( "</table></body></html>\n" );
            }

            return;
        }
    }
}
