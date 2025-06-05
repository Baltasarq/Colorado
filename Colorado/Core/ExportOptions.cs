// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core {
    using System;
    using System.Linq;


	public class ExportOptions {
		/// <summary>
		/// Export options. By default, no row numbers are included,
		/// nor the table border. All fields are included.
		/// </summary>
		/// <param name="path">
		/// A <see cref="String"/> holding the name of the file.
		/// </param>
		/// <param name="doc">
		/// A <see cref="CsvDocument"/> reference to the document to export.
		/// </param>
        public ExportOptions(String path, CsvDocument doc)
        {
            this.ExporterId = Exporters.CsvExporter.Name;
			this.Path = path;
            this.Document = doc;
            this.IncludeRowNumbers = false;
			this.IncludeTableBorder = false;
            this.Delimiter = doc.Delimiter;
			this.QuotedText = doc.SurroundText;

			this.ColumnsIncluded = Enumerable.Range( 0, this.Document.Data.NumColumns ).ToArray();
		}

        /// <summary>
        /// Returns whether or not a given column index is included or not.
        /// </summary>
        /// <param name="fieldNum">
        /// A <see cref="System.Int32"/> holding the column index to ask for.
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean"/> holding true if the column index is included; false otherwise.
        /// </returns>
        public bool IsColumnIncluded(int fieldNum)
        {
            return ( Array.IndexOf( this.ColumnsIncluded, fieldNum ) >= 0 );
        }

  		/// <summary>
		/// Gets or sets whether the text is quoted or not.
		/// This is only used when the export format is CSV
		/// By default, has the same value as on the document.
		/// </summary>
		public bool QuotedText {
            get; set;
		}

		/// <summary>
		/// Gets or sets the delimiter to use while exporting.
		/// This is only used when the export format is CSV
		/// By default, has the same value as in the document.
		/// </summary>
        public Delimiter Delimiter {
            get; private set;
        }

		/// <summary>
		/// Gets the whole set of selected fields.
		/// </summary>
		public int[] ColumnsIncluded {
            get; set;
		}

		/// <summary>
		/// Name of the file for exportation
		/// </summary>
		public string Path {
            get; set;
		}

		/// <summary>
		/// Include row numbers
		/// </summary>
		public bool IncludeRowNumbers {
            get; set;
		}

		/// <summary>
		/// Include or not a margin line for the border of the table.
		/// </summary>
		public bool IncludeTableBorder {
            get; set;
		}

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>The <see cref="CsvDocument"/>.</value>
        public CsvDocument Document {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the exporter identifier.
        /// </summary>
        /// <value>The exporter identifier.</value>
        public string ExporterId {
            get; set;
        }

        /// <summary>
        /// Gets the exporter. This is just a shortcut.
        /// </summary>
        /// <value>The <see cref="Exporter"/>.</value>
        /// <seealso cref="Exporter.GetExporter"/>
        public Exporter Exporter => Exporter.GetExporter( this.ExporterId );
	}
}
