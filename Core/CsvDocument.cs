// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core {
    /// <summary>
    /// Represents the CSV/TSV document itself, with rows and columns of data.
    /// <seealso cref="Data"/>
    /// </summary>
	public class CsvDocument {
		public const char Quote = '"';
		public const string NewFileName = "new-doc.csv";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Colorado.Core.CsvDocument"/> class.
        /// </summary>
		private CsvDocument()
		{
			this.Data = new Data( this, 0, 0 );
			this.fileName = NewFileName;
			this.delimiter = new Delimiter( Delimiter.TabDelimiter );
            this.decimalMark = new DecimalMark();
			this.surroundText = true;
			this.ClientUpdater = null;
			this.FormulaManager = new FormulaManager( this );
            this.HasName = false;
		}
		
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Colorado.Core.CsvDocument"/> class.
        /// </summary>
        /// <param name="numRows">Number of rows.</param>
        /// <param name="numCols">Number of cols.</param>
        public CsvDocument(int numRows, int numCols)
			: this()
        {
			Data.SetInitialSize( numRows, numCols );
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Colorado.Core.CsvDocument"/> has changed.
        /// </summary>
        /// <value><c>true</c> if changed; otherwise, <c>false</c>.</value>
		public bool Changed {
			get { return Data.Changed; }
			set { Data.Changed = value; this.FormulaManager.UpdateFormulas(); }
		}

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
		public string FileName {
			get { return this.fileName; }
			set {
                CsvDocumentPersistence.PrepareFileName( ref value );
                this.fileName = value;
                this.HasName = true;
            }
		}
		
        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Colorado.Core.CsvDocument"/>
        /// has been assigned a file name.
        /// </summary>
        /// <value><c>true</c> if has name; otherwise, <c>false</c>.</value>
		public bool HasName {
            get; private set;
		}
		
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Colorado.Core.CsvDocument"/> should surround text.
        /// </summary>
        /// <value><c>true</c> if text should be surrounded; otherwise, <c>false</c>.</value>
		public bool SurroundText {
			get { return surroundText; }
			set { surroundText = value; Changed = true; }
		}

        /// <summary>
        /// Gets or sets the delimiter value between data in the same row.
        /// </summary>
        /// <value>The delimiter value.</value>
        public string DelimiterValue {
            get {
                return this.delimiter.ToString();
            }
            set {
                this.delimiter.Name = value;
                this.Changed = true;
            }
        }

        /// <summary>
        /// Gets or sets the decimal separator for numeric, real data.
        /// </summary>
        /// <value>The decimal separator.</value>
        public DecimalMark.DecimalSeparator DecimalSeparator {
            get {
                return this.decimalMark.Value;
            }
            set {
                if ( value != this.decimalMark.Value )
                {
                    char chOldDecimalMark = DecimalMark.DecimalSeparatorChar[ (int) this.decimalMark.Value ];
                    char chNewDecimalMark = DecimalMark.DecimalSeparatorChar[ (int) value ];

                    for (int i = 0; i < this.Data.NumRows; ++i) {
                        for (int j = 0; j < this.Data.NumColumns; ++j) {
                            string cell = this.Data[ i, j ];

                            if ( DecimalMark.IsNumber( cell ) ) {
                                this.Data[ i, j ] = cell.Replace( chOldDecimalMark, chNewDecimalMark );
                            }
                        }
                    }

                    this.decimalMark.Value = value;
                    this.Changed = true;
                }

                return;
            }
        }

        /// <summary>
        /// Gets the formula manager.
        /// </summary>
        /// <value>The formula manager.</value>
		public FormulaManager FormulaManager {
            get; private set;
		}

		/// <summary>
		/// Gets the data stored.
		/// </summary>
		/// <value>The data, as a Data object.</value>
		public Data Data {
            get; private set;
		}

		public delegate void ClientUpdate(int row, int col, string value);

		public void UpdateClients(int rowIndex, int colIndex, string value)
		{
            this.ClientUpdater?.Invoke( rowIndex, colIndex, value );
		}

		public ClientUpdate ClientUpdater;
        private DecimalMark decimalMark;
        private Delimiter delimiter;
        private string fileName;
        private bool surroundText;
	}
}
