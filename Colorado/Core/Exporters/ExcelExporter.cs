// Colorado (c) 2015/18 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core.Exporters
{
    using System;
    using System.Xml;
    using System.Text;


    /// <summary>Exports CSV data to Excel 2003 XML format.</summary>
    public class ExcelExporter: Exporter {
        public const string Name = "Excel";
        const string Extension = "xls";
        enum DataType { String, Number };

        public override void Save()
        {
            CsvDocument doc = this.Options.Document;

            using(var textWriter = new XmlTextWriter( this.Options.Path, Encoding.UTF8 )) {
                textWriter.WriteStartDocument();

                WriteStartWorkBook( textWriter );                        // WkBk

                textWriter.WriteStartElement( "Table" );                 // Table

                // Headers
                textWriter.WriteStartElement( "Row" );                   // Row

                if ( this.Options.IncludeRowNumbers ) {
                    WriteCell( textWriter, "#" );
                }

                foreach(int col in this.Options.ColumnsIncluded) {                            
                    WriteCell( textWriter,
                              doc.Data.ColumnInfo[ col ].Header );
                }

                textWriter.WriteEndElement();    

                // Data
                for(int row = 0; row < doc.Data.NumRows; ++row) {
                    textWriter.WriteStartElement( "Row" );               // Row

                    if ( this.Options.IncludeRowNumbers ) {
                        WriteCell( textWriter, row + 1 );
                    }

                    foreach(int col in this.Options.ColumnsIncluded) {
                        string data = doc.Data[ row, col ];
                        DataType type = DataType.String;

                        if ( Double.TryParse( data, out double d ) ) {
                            type = DataType.Number;
                        }

                        WriteCell( textWriter, data, type );
                    }

                    textWriter.WriteEndElement();                        // /Row
                }

                textWriter.WriteEndElement();                            // /Table
                textWriter.WriteEndElement();                            // /Worksheet

                textWriter.WriteEndElement();                            // /WkBk
                textWriter.WriteEndDocument();
            }

            return;
        }

        static void WriteStartWorkBook(XmlWriter textWriter)
        {
            textWriter.WriteStartElement( "Workbook" );              // Workbook

            textWriter.WriteStartAttribute( "xmlns" );
            textWriter.WriteString( "urn:schemas-microsoft-com:office:spreadsheet" );
            textWriter.WriteEndAttribute();

            textWriter.WriteStartAttribute( "xmlns:o" );
            textWriter.WriteString( "urn:schemas-microsoft-com:office:office" );
            textWriter.WriteEndAttribute();

            textWriter.WriteStartAttribute( "xmlns:x" );
            textWriter.WriteString( "urn:schemas-microsoft-com:office:excel" );
            textWriter.WriteEndAttribute();

            textWriter.WriteStartAttribute( "xmlns:ss" );
            textWriter.WriteString( "urn:schemas-microsoft-com:office:spreadsheet" );
            textWriter.WriteEndAttribute();

            textWriter.WriteStartAttribute( "xmlns:html" );
            textWriter.WriteString( "http://www.w3.org/TR/REC-html40" );
            textWriter.WriteEndAttribute();

            textWriter.WriteStartElement( "Worksheet" );             // Worksheet
            textWriter.WriteStartAttribute( "ss:Name" );
            textWriter.WriteString( AppInfo.Name + " XSL export" );
            textWriter.WriteEndAttribute();
        }

        static void WriteCell(XmlWriter textWriter, int val)
        {
            WriteCell( textWriter, val.ToString(), DataType.Number );
        }

        static void WriteCell(XmlWriter textWriter, string data, DataType type = DataType.String)
        {
            textWriter.WriteStartElement( "Cell" );                      // Cell
            textWriter.WriteStartElement( "Data" );                      // Data

            textWriter.WriteStartAttribute( "ss:Type" );
            textWriter.WriteString( type.ToString() );
            textWriter.WriteEndAttribute();

            textWriter.WriteString( data );

            textWriter.WriteEndElement();                                // /Data
            textWriter.WriteEndElement();                                // /Cell
        }

        /// <summary>Gets the name of the exporter.</summary>
        public override string Id => Name;

        /// <summary>Gets the extension for this exporter's output.</summary>
        public override string FileExtension => Extension;
    }
}
