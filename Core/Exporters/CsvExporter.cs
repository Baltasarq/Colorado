// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core.Exporters {
    using Core;


    public class CsvExporter: Exporter {
        public const string Name = "CSV";
        const string Extension = "csv";

        public override void Save()
        {
            new CsvDocumentPersistence( this.Options.Document ).SaveCsvData( this.Options );
        }

        /// <summary>Gets the name of the exporter.</summary>
        public override string Id => Name;

        /// <summary>Gets the extension for this exporter's output.</summary>
        public override string FileExtension => Extension;
    }
}
