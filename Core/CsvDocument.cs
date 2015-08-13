
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Colorado.Core {	
	public class CsvDocument {
        public enum DecimalSeparator { Point, Comma };
		public const char Quote = '"';
		public const string NewFileName = "new-doc.csv";

        public static readonly ReadOnlyCollection<char> DecimalSeparatorChar =
            new ReadOnlyCollection<char>( new char[] { '.', ',' } );

		private CsvDocument()
		{
			this.data = new Data( this, 0, 0 );
			this.fileName = NewFileName;
			this.delimiter = new Delimiter( Delimiter.TabDelimiter );
			this.surroundText = true;
            this.DecimalMark = AppInfo.DecimalMark;
			this.ClientUpdater = null;
			this.formulaManager = new FormulaManager( this );
            this.HasName = false;
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

        /// <summary>
        /// Determines if the parameter is a decimal mark.
        /// </summary>
        /// <returns><c>true</c> if the parameter is a decimal mark; otherwise, <c>false</c>.</returns>
        /// <param name="ch">A char possibly containing . or ,</param>
        public static bool IsDecimalMark(char ch) {
            return ( ch == ',' || ch == '.' );
        }

        /// <summary>
        /// Determines if parameter is a number.
        /// </summary>
        /// <returns><c>true</c> if parameter is a number; otherwise, <c>false</c>.</returns>
        /// <param name="s">A string possibly containing a number.</param>
        public static bool IsNumber(string s) {
            bool toret = true;
            int pos = 0;
            int numMarks = 1;
            int numEs = 1;

            // Maybe there is a sign before the number
            if ( s[ pos ] == '+'
              || s[ pos ] == '-' )
            {
                ++pos;
            }

            // Maybe the decimal mark is at the beginning
            if ( IsDecimalMark( s[ pos ] ) ) {
                ++pos;
                --numMarks;
            }

            // Check the remaining positions
            while( pos < s.Length ) {
                if ( IsDecimalMark( s[ pos ] ) ) {
                    --numMarks;
                }
                else
                if ( char.ToUpper( s[ pos ] ) == 'E' ) {
                    --numEs;
                }
                else
                if ( !char.IsDigit( s[ pos ] ) ) {
                    toret = false;
                    break;
                }

                ++pos;
            }

            // More than one separator oe 'e'?
            if ( numMarks < 0
              || numEs < 0 )
            {
                toret = false;
            }

            return toret;
        }

		private DecimalSeparator decimalMark;
        private Delimiter delimiter;
        private bool surroundText;
		private Data data;
        private string fileName;
		private FormulaManager formulaManager;
		public ClientUpdate ClientUpdater;
	}
}
