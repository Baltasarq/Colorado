// Colorado (c) 2015/19 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core {
    /// <summary>Import options.</summary>
    public class ImportOptions {
        public ImportOptions(string path)
        {
            this.Path = path;
            this.FirstRowIsForHeaders = false;
            this.ImportId = Importers.CsvImporter.Name;
            this.Delimiter = new Delimiter();
        }

        /// <summary>Gets or sets the path to the file.</summary>
        /// <value>The path, as a string.</value>
        public string Path {
            get;
        }

        /// <summary>Gets or sets the id of the importer.</summary>
        /// <value>The path, as a string.</value>
        public string ImportId {
            get; set;
        }

        /// <summary>Gets or sets the delimiter.</summary>
        /// <value>The delimiter, as a char.</value>
        public Delimiter Delimiter {
            get; set;
        }

        /// <summary>Gets or sets the possiblity of headers.</summary>
        /// <value>A boolean.</value>
        public bool FirstRowIsForHeaders {
            get; set;
        }
    }
}
