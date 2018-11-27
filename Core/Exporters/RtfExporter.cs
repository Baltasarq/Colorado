// Colorado (c) 2015/18 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core.Exporters
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>Exports CSV data to RTF.</summary>
    public class RtfExporter: Exporter
    {
        public RtfExporter(CsvDocument doc, ExportOptions options)
            :base( doc, options )
        {
        }

        public override void Save()
        {
            int colNumber = 0;
            string fileName = this.Options.Name;
            const string TableBorder = @"\clbrdrt\brdrs\clbrdrl\brdrs\clbrdrb\brdrs\clbrdrr\brdrs";

            using(var file = new StreamWriter(
                                new FileStream( fileName, FileMode.Create, FileAccess.Write ),
                                Encoding.ASCII ))
            {
                // write rtf header
                file.WriteLine( @"{\rtf2\ansi\deff0 {\fonttbl {\f0 Times;}{\f1 Courier;}}\f0 Times;}{\f1 Courier;}}" );

                // write rtf table codes
                file.WriteLine( @"\trowd\trautofit1\trgaph144" );

                if ( this.Options.IncludeRowNumbers ) {
                    if ( this.Options.IncludeTableBorder ) {
                        file.Write( TableBorder );
                    }

                    file.Write( @"\cellx100" );
                    colNumber = 1;
                }

                for(; colNumber < Document.Data.ColumnInfo.Length; ++colNumber)
                {
                    if ( this.Options.IsColumnIncluded( colNumber - ( this.Options.IncludeRowNumbers ? 1 : 0 ) ) )
                    {
                        if ( this.Options.IncludeTableBorder ) {
                            file.Write( TableBorder );
                        }

                        file.WriteLine( @"\cellx" + Convert.ToString( ( colNumber + 1 ) * 2000 ) );
                    }
                }
                file.WriteLine( @"\intbl" );

                // write headers
                if ( this.Options.IncludeRowNumbers ) {
                    file.WriteLine( @"{\b #}\cell" );
                }

                for(int col = 0; col < Document.Data.NumColumns; ++col) {
                    if ( this.Options.IsColumnIncluded( col ) ) {
                        file.WriteLine( "{\\b " + Document.Data.ColumnInfo[ col ].Header + "}\\cell" );
                    }
                }
                file.WriteLine( @"\row\trow" );

                // write each row
                for(int row = 0; row < Document.Data.NumRows; ++row) {
                    if ( this.Options.IncludeRowNumbers ) {
                        file.WriteLine( Convert.ToString( row +1 ) + @"\cell" );
                    }

                    for(int col = 0; col < Document.Data.NumColumns; ++col) {
                        if ( this.Options.IsColumnIncluded( col ) ) {
                            file.WriteLine( Document.Data[ row, col ] + @"\cell" );
                        }
                    }
                    file.WriteLine( @"\row\trow" );
                }

                // End
                file.WriteLine( @"\pard}" );
            }

            return;
        }
    }
}
