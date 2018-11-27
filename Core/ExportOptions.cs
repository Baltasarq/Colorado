// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;


	public class ExportOptions {
		
		public enum SelectionType { Rtf, Html, Csv, Excel, Txt };
		public static readonly ReadOnlyCollection<string> SelectionExtension =
			new ReadOnlyCollection<string>(
				new string[]{ ".rtf", ".html", ".csv", ".xls", ".txt" }
			);
		public static readonly ReadOnlyCollection<string> SelectionExplanation =
			new ReadOnlyCollection<string>(
				new string[]{ "RTF (Rich Text Format)", "HTML (web page)",
			                  "CSV (Comma Separated Values)", "XLS (MS Excel)",
                              "TXT (UTF-8 Text)" }
			);

		/// <summary>
		/// Export options. By default, no row numbers are included,
		/// nor the table border. All fields are included.
		/// </summary>
		/// <param name="name">
		/// A <see cref="String"/> holding the name of the file.
		/// </param>
		/// <param name="doc">
		/// A <see cref="CsvDocument"/> reference to the document to export.
		/// </param>
		public ExportOptions(String name, CsvDocument doc) {
			this.name = name;
			this.includeRowNumbers = false;
			this.includeTableBorder = false;
            this.delimiter = new Delimiter( doc.DelimiterValue );
			this.QuotedText = doc.SurroundText;
            this.Format = SelectionType.Html;

			this.columnsIncluded = new List<int>( doc.Data.NumColumns );
			this.FillDefaultColumnsIncluded();
		}

        /// <summary>
        /// Gets or sets the format of the export.
        /// </summary>
        /// <value>The format, as a SelectionType enum.</value>
        public SelectionType Format {
            get; set;
        }
		
		/// <summary>
		/// Gets or sets whether the text is quoted or not.
		/// This is only used when the export format is CSV
		/// By default, has the same value as on the document.
		/// </summary>
		public bool QuotedText {
			get { return this.quotedText;  }
			set { this.quotedText = value; }
		}
		
		/// <summary>
		/// Gets or sets the delimiter to use while exporting.
		/// This is only used when the export format is CSV
		/// By default, has the same value as on the document.
		/// </summary>
        public Delimiter Delimiter {
            get { return delimiter; }
        }
		
		/// <summary>
		/// Gets the whole set of selected fields.
		/// </summary>
		public int[] ColumnsIncluded {
			get { return columnsIncluded.ToArray(); }
			set {
				columnsIncluded.Clear();
				foreach( var i in value ) {
					columnsIncluded.Add( i );
				}
			}
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
			return this.columnsIncluded.Contains( fieldNum );
		}
		
		/// <summary>
		/// Name of the file for exportation
		/// </summary>
		public string Name {
			get { return this.name; }
			set { name = value; }
		}

		private bool includeRowNumbers;
		private bool includeTableBorder;
		
		/// <summary>
		/// Include row numbers
		/// </summary>
		public bool IncludeRowNumbers {
			get { return this.includeRowNumbers; }
			set { this.includeRowNumbers = value; }
		}
		
		/// <summary>
		/// Include or not a margin line for the border of the table.
		/// </summary>
		public bool IncludeTableBorder {
			get { return this.includeTableBorder; }
			set { this.includeTableBorder = value; }
		}
		
		private void FillDefaultColumnsIncluded()
		{
			columnsIncluded.Clear();
			
			for(int i = 0; i < columnsIncluded.Capacity; ++i) {
				columnsIncluded.Add( i );
			}
		}
	
		private string name;
		private List<int> columnsIncluded;
		private Delimiter delimiter;
		private bool quotedText;
	}

}

