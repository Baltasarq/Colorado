namespace Colorado.Core.Exporters {
    using System.IO;
    using System.Text;
    using System.Json;

    public class JsonExporter: Exporter {
        public const string Name = "JSON";
        const string Extension = "json";

        /// <summary>Export this data as JSON./// </summary>
        public override void Save()
        {
            CsvDocument doc = this.Options.Document;
            int[] columnsIncluded = this.Options.ColumnsIncluded;
            string[] headers = doc.Data.Headers;
            string fileName = this.Options.Path;
            var root = new JsonArray();

            for(int i = 0; i < doc.Data.NumRows; ++i) {
                var obj = new JsonObject();

                root.Add( obj );

                if ( this.Options.IncludeRowNumbers ) {
                    obj.Add( EtqId, i );
                }

                foreach(int j in columnsIncluded) {
                    obj.Add( headers[ j ], doc.Data[ i, j ] );
                }
            }

            using(var f = new StreamWriter(
                new FileStream( fileName, FileMode.Create, FileAccess.Write ),
                Encoding.UTF8 ))
            {
                f.WriteLine( root.ToString() );
            }

            return;
        }

        /// <summary>Gets the name of the exporter.</summary>
        public override string Id => Name;

        /// <summary>Gets the extension for this exporter's output.</summary>
        public override string FileExtension => Extension;
    }
}
