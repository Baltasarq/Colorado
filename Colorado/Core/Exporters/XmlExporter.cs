namespace Colorado.Core.Exporters {
    using System.IO;
    using System.Xml;

    public class XmlExporter: Exporter {
        public const string Name = "XML";
        const string Extension = "xml";
        public const string EtqData = "data";
        public const string EtqRow = "row";

        /// <summary>Export this data as XML.</summary>
        public override void Save()
        {
            CsvDocument doc = this.Options.Document;
            int[] columnsIncluded = this.Options.ColumnsIncluded;
            string[] headers = doc.Data.Headers;
            string fileName = this.Options.Path;

            using var f = XmlWriter.Create(
                new FileStream( fileName, FileMode.Create, FileAccess.Write ));

            f.WriteStartDocument( true );
            f.WriteStartElement( EtqData );

            for(int i = 0; i < doc.Data.NumRows; ++i) {
                f.WriteStartElement( EtqRow );

                if ( this.Options.IncludeRowNumbers ) {
                    f.WriteStartAttribute( EtqId );
                    f.WriteValue( i );
                    f.WriteEndAttribute();
                }

                foreach(int j in columnsIncluded) {
                    f.WriteStartElement( headers[j] );
                    f.WriteString( doc.Data[ i, j ] );
                    f.WriteEndElement();
                }

                f.WriteEndElement();
            }

            f.WriteEndElement();

            return;
        }

        /// <summary>Gets the name of the exporter.</summary>
        public override string Id => Name;

        /// <summary>Gets the extension for this exporter's output.</summary>
        public override string FileExtension => Extension;
    }
}
