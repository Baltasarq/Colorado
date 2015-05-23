
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Colorado.Core {	
	public class CsvDocument {
        public enum DecimalSeparator { Point, Comma };
		public const char Quote = '"';
		public const string NewFileName = "newdoc.csv";

        public static readonly ReadOnlyCollection<char> DecimalSeparatorChar =
            new ReadOnlyCollection<char>( new char[] { '.', ',' } );

		private CsvDocument()
		{
			this.data = new Data( this, 0, 0 );
			this.fileName = NewFileName;
			this.delimiter = new Delimiter( Delimiter.TabDelimiter );
			this.HasName = false;
			this.surroundText = true;
            this.DecimalMark = AppInfo.DecimalMark;
			this.ClientUpdater = null;
			this.formulaManager = new FormulaManager( this );
		}
		
        public CsvDocument(int numRows, int numCols)
			: this()
        {
			Data.SetInitialSize( numRows, numCols );
        }

        public char GetDecimalMark()
        {
            return DecimalSeparatorChar[ (int) DecimalMark ];
        }

		public bool Changed {
			get { return Data.Changed; }
			set { Data.Changed = value; this.formulaManager.UpdateFormulas(); }
		}

		public string FileName {
			get { return fileName; }
			set { CsvDocumentPersistence.PrepareFileName( ref value ); fileName = value; }
		}
		
		public bool HasName {
			get { return hasName; }
			set { hasName = value; }
		}
		
		public bool SurroundText {
			get { return surroundText; }
			set { surroundText = value; Changed = true; }
		}

		public Delimiter Delimiter {
			get { return delimiter; }
		}

		public DecimalSeparator DecimalMark {
			get {
				return this.decimalMark;
			}
			set {
				int val = (int) value;

				if ( val < 0
				  || val >= Enum.GetValues( typeof(DecimalSeparator) ).Length )
				{
					val = 0;
				}

				this.decimalMark = (DecimalSeparator) val;
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

		private DecimalSeparator decimalMark;
        private Delimiter delimiter;
        private bool surroundText;
		private Data data;
        private string fileName;
        private bool hasName;
		private FormulaManager formulaManager;
		public ClientUpdate ClientUpdater;
	}
}
