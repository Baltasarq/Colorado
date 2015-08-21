using System;

namespace Colorado.Core {	
	public class CsvDocument {
		public const char Quote = '"';
		public const string NewFileName = "new-doc.csv";

		private CsvDocument()
		{
			this.data = new Data( this, 0, 0 );
			this.fileName = NewFileName;
			this.delimiter = new Delimiter( Delimiter.TabDelimiter );
            this.decimalMark = new DecimalMark();
			this.surroundText = true;
			this.ClientUpdater = null;
			this.formulaManager = new FormulaManager( this );
            this.HasName = false;
		}
		
        public CsvDocument(int numRows, int numCols)
			: this()
        {
			Data.SetInitialSize( numRows, numCols );
        }

		public bool Changed {
			get { return Data.Changed; }
			set { Data.Changed = value; this.formulaManager.UpdateFormulas(); }
		}

		public string FileName {
			get { return this.fileName; }
			set {
                CsvDocumentPersistence.PrepareFileName( ref value );
                this.fileName = value;
                this.HasName = true;
            }
		}
		
		public bool HasName {
            get; private set;
		}
		
		public bool SurroundText {
			get { return surroundText; }
			set { surroundText = value; Changed = true; }
		}

		public Delimiter Delimiter {
			get { return delimiter; }
		}

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

		public FormulaManager FormulaManager {
			get {
				return this.formulaManager;
			}
		}

		/// <summary>
		/// Gets the data stored.
		/// </summary>
		/// <value>The data, as a Data object.</value>
		public Data Data {
			get {
				return this.data;
			}
		}

		public delegate void ClientUpdate(int row, int col, string value);

		public void UpdateClients(int rowIndex, int colIndex, string value)
		{
			if ( this.ClientUpdater != null ) {
				this.ClientUpdater( rowIndex, colIndex, value );
			}

			return;
		}

        private Delimiter delimiter;
        private DecimalMark decimalMark;
        private bool surroundText;
		private Data data;
        private string fileName;
		private FormulaManager formulaManager;
		public ClientUpdate ClientUpdater;
	}
}
