// Colorado (c) 2015/19 Baltasar MIT License <baltasarq@gmail.com>

using System.ComponentModel;
using Colorado.Core.Importers;

namespace Colorado.Core {
    /// <summary>Import options.</summary>
    public class ImportOptions {
        public ImportOptions(string path)
        {
            this.Path = path;
        }

        /// <summary>Gets or sets the path to the file.</summary>
        /// <value>The path, as a string.</value>
        public string Path {
            get;
        }

        /// <summary>Gets or sets the id of the importer.</summary>
        /// <value>The path, as a string.</value>
        public required string ImportId {
            get; init;
        }

        /// <summary>Gets or sets the delimiter.</summary>
        /// <value>The delimiter, as a char.</value>
        public required Delimiter Delimiter {
            get; init;
        }

        /// <summary>Gets or sets the possiblity of headers.</summary>
        /// <value>A boolean.</value>
        public required bool FirstRowIsForHeaders {
            get; init;
        }

        public static ImportOptions Default => new( "." ) {
                                        Delimiter = new Delimiter(),
                                        FirstRowIsForHeaders = true,
                                        ImportId = CsvImporter.Name };
    }
}
