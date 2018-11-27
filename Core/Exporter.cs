// Colorado (c) 2015/18 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core
{
    /// <summary>Base class for all exporters.</summary>
    public abstract class Exporter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Colorado.Core.Exporter"/> class.
        /// </summary>
        /// <param name="doc">The <see cref="CsvDocument"/> to export.</param>
        /// <param name="options">The <see cref="ExportOptions"/> object.</param>
        protected Exporter(CsvDocument doc, ExportOptions options)
        {
            this.Document = doc;
            this.Options = options;
        }

        /// <summary>Save this instance.</summary>
        public abstract void Save();

        /// <summary>
        /// Gets the document to export.
        /// </summary>
        /// <value>The <see cref="CsvDocument"/>.</value>
        public CsvDocument Document {
            get; private set;
        }

        /// <summary>
        /// Gets the export options.
        /// </summary>
        /// <value>The <see cref="ExportOptions"/>.</value>
        public ExportOptions Options {
            get; private set;
        }
    }
}
