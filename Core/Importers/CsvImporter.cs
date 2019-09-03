// Colorado (c) 2015/19 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core.Importers {
    /// <summary>Import from CSV using the persistence layer.</summary>
    public class CsvImporter: Importer {
        public const string Name = "CSV";
        const string Extension = "csv";

        /// <summary>Import data from CSV.</summary>
        public override CsvDocument Load()
        {
            string fn = this.Options.Path;
            bool useHeaders = this.Options.FirstRowIsForHeaders;
            char delim = this.Options.Delimiter.Raw;
            var loader = new CsvDocumentPersistence();

            if ( delim == '\0' ) {
                loader.Load( fn, firstRowForHeaders: useHeaders );
            } else {
                loader.Load( fn, delim, useHeaders );
            }

            return loader.Document;
        }

        /// <summary>Gets the name of the importer.</summary>
        public override string Id => Name;

        /// <summary>Gets the extension for this importer's output.</summary>
        public override string FileExtension => Extension;
    }
}
