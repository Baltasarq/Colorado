using System;
using System.Collections.Generic;

namespace Colorado.Core {
	/// <summary>
	/// Represents the whole column info, i.e., type and header.
	/// </summary>
	public class ColumnInfo {
		public const string ColEtq = "Col";
		public enum ColumnType { Text, Number };

		public ColumnInfo() {
			Type = ColumnType.Text;
			Header = ColEtq;
		}

		public ColumnType Type {
			get; set;
		}

		public string Header {
			get; set;
		}
	}

	/// <summary>
	/// Represents the data in the document
	/// </summary>
	public class Data {
		public Data(CsvDocument doc, int numRows, int numCols)
		{
			this.owner = doc;
			this.data = new List<List<string>>( numRows );
			this.columnInfo = new List<ColumnInfo>();

			this.SetInitialSize( numRows, numCols );
		}

		/// <summary>
		/// Gets or sets the cell with the specified numRow numColumn.
		/// </summary>
		/// <param name="numRow">Number of row.</param>
		/// <param name="numColumn">Number of column.</param>
		public string this[int numRow, int numColumn] {
			get {
                ChkValue( numRow, 0, NumRows, "row number" );
                ChkValue( numColumn, 0, NumColumns, "column number" );

                string toret = this.data[ numRow ][ numColumn ];

                if ( toret == null ) {
                    toret = this.data[ numRow ][ numColumn ] = "";
                }

				return toret;
			}
			set {
				ChkValue( numRow, 0, NumRows, "row number" );
                ChkValue( numColumn, 0, NumColumns, "column number" );

                this.data[ numRow ][ numColumn ] = value ?? "";
			}
		}

        /// <summary>
		/// Gets the complete row with the specified numRow, as a string[].
		/// </summary>
		/// <param name="numRow">The row number.</param>
		public string[] this[int numRow] {
			get {
				ChkValue( numRow, 0, this.numRows, "row number" );
				return this.data[ numRow ].ToArray();
			}
		}

		/// <summary>
		/// Gets or sets the number rows.
		/// </summary>
		/// <value>The number rows.</value>
		public int NumRows {
			get { return numRows; }
			set { this.SetNumRows( value ); }
		}

		/// <summary>
		/// Gets or sets the number columns.
		/// </summary>
		/// <value>The number columns.</value>
		public int NumColumns {
			get { return numColumns; }
			set { this.SetNumColumns( value ); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Data"/> is changed.
		/// </summary>
		/// <value><c>true</c> if changed; otherwise, <c>false</c>.</value>
		public bool Changed {
			get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the first row is for headers.
		/// When set, moves the first line to headers or viceversa, accordingly.
		/// </summary>
		/// <value><c>true</c> if first row for headers; otherwise, <c>false</c>.</value>
		public bool FirstRowContainsHeaders {
			get { return firstRowContainsHeaders; }
			set {
                if ( value != this.firstRowContainsHeaders ) {
					if ( value ) {
						this.MoveFirstRowToHeaders();
					} else {
						this.MoveHeadersToFirstRow();
					}

					this.firstRowContainsHeaders = value;
				}
			}
		}

		/// <summary>
		/// Gets the columns info.
		/// </summary>
		/// <value>The columns info, as a ColumnInfo[].</value>
		public ColumnInfo[] ColumnInfo {
			get {
				return this.columnInfo.ToArray();
			}
		}

		/// <summary>
		/// Gets the owner of this data
		/// </summary>
		/// <value>The owner.</value>
		public CsvDocument Owner {
			get {
				return this.owner;
			}
		}

		/// <summary>
		/// Sets the size of the Data, numRows x numCols
		/// </summary>
		/// <param name="numberOfRows">Number of rows.</param>
		/// <param name="numCols">Number of columns.</param>
        internal void SetInitialSize(int numberOfRows, int numCols)
		{
			this.data.Clear();
			this.data.Capacity = numberOfRows;

			for (int i = 0; i < numberOfRows; ++i) {
				this.data.Add( ( new List<string>( new string[ numCols ] ) ) );
			}

			this.numRows = numberOfRows;
			this.numColumns = numCols;
			this.CreateDefaultHeaders();
			this.Changed = true;
		}

		/// <summary>
		/// Sets the number rows.
		/// Expands used memory, if needed.
		/// </summary>
		/// <param name="numRows">Number rows.</param>
		public void SetNumRows(int numRows)
		{
			if ( this.numRows != numRows
			  && numRows >= 0 )
			{
				Changed = true;

				if ( numRows > this.numRows ) {
					this.data.Capacity = numRows;

					for (int i = 0; i < ( numRows - this.numRows ); ++i) {
						this.data.Add( new List<string>( new string[ this.numColumns ] ) );
					}
				} else {
					this.data.RemoveRange( numRows, this.NumRows - numRows );
				}

				this.numRows = numRows;
			}
		}

		/// <summary>
		/// Sets the number columns.
		/// Expands used memory, if needed.
		/// </summary>
		/// <param name="numCols">Number cols.</param>
		public void SetNumColumns(int numCols)
		{
			int delta = numCols - this.numColumns;

			if ( numCols != this.numColumns
			  && numCols >= 0 )
			{
				this.Changed = true;

				for (int i = 0; i < this.NumRows; ++i) {
					var column = this.data[ i ];

					if ( delta > 0 ) {
						for (int j = 0; j < delta; ++j) {
							column.AddRange( new List<string>( new string[ delta ] ) );
						}
					} else {
						column.RemoveRange( numCols, NumColumns - numCols );
					}
				}

				this.numColumns = numCols;
			}

			// Fix headers
			CreateNewColumnsInfo( NumColumns );
		}

		public void CopyRow(int posOrg, int posDest)
		{
			// Chk
			if ( posOrg == posDest ) {
				return;
			}

			ChkValue( posOrg, 0, NumRows, "beginning row for copying" );
			ChkValue( posDest, 0, NumRows, "ending row for copying" );
			Changed = true;

			// do it
			for(int j = 0; j < NumColumns; ++j) {
				this.data[ posDest ][ j ] = this.data[ posOrg ][ j ];
			}
		}

		public void CopyColumn(int posOrg, int posDest)
		{
			// Chk
			if ( posOrg == posDest ) {
				return;
			}

			ChkValue( posOrg, 0, NumColumns, "beginning row for copying" );
			ChkValue( posDest, 0, NumColumns, "beginning row for copying" );
			Changed = true;

			// do it
			for(int i = 0; i < NumRows; ++i) {
				this.data[ i ][ posDest ] = this.data[ i ][ posOrg ];
			}
		}

		/// <summary>
		/// Checks any given value for a range [min, max),
		/// and throws an exception if violated
		/// </summary>
		/// <param name="val">
		/// The <see cref="System.Int32"/> to check
		/// </param>
		/// <param name="min">
		/// The <see cref="System.Int32"/> for the minimum possible value
		/// </param>
		/// <param name="max">
		/// The <see cref="System.Int32"/> for the maximum possible value
		/// </param>
		/// <param name="explanation">
		/// A <see cref="System.String"/> containing info about the value being checked
		/// </param>
		public static void ChkValue(int val, int min, int max, string explanation)
		{
			if ( val < min
  			  || val >= max )
			{
				throw new ApplicationException(
					"Value " + Convert.ToString( val )
					+ " ("
					+ explanation
					+ ", outside range ["
					+ Convert.ToString( min )
					+ ", "
					+ Convert.ToString( max )
					+ ")"
				);
			}
		}

		/// <summary>
		/// An adapter for cleaning rows, always starting from the first col.
		/// </summary>
		public void CleanRows(int posBeg, int posEnd)
		{
			this.CleanRows( 0, posBeg, posEnd );
		}

		/// <summary>
		/// Cleans the contents of some rows. Can throw ApplicationException
		/// when detecting invalid parameters (negative, etc.)
		/// </summary>
		/// <param name="posBeg">
		/// A <see cref="System.Int32"/> representing the first row
		/// </param>
		/// <param name="posEnd">
		/// A <see cref="System.Int32"/> representing the last row (included)
		/// </param>
		/// <param name="colStart">
		/// A <see cref="System.Int32"/> representing the starting column
		/// </param>
		public void CleanRows(int colStart, int posBeg, int posEnd)
		{
			// Check
			ChkValue( posBeg, 0, NumRows, "beginning row" );
			ChkValue( posEnd, 0, NumRows, "ending row" );
			ChkValue( colStart, 0, NumColumns, "the beginning column" );

			if ( posBeg > posEnd ) {
				throw new ApplicationException( "invalid parameters (beg>end)" );
			}

			// Do it
			for(int i = posBeg; i <= posEnd; ++i) {
				for(int j = colStart; j < NumColumns; ++j) {
					this.data[ i ][ j ] = "";
				}
			}

			Changed = true;
		}

		/// <summary>
		/// An adapter for cleanColumns, always starting from first row.
		/// </summary>
		public void CleanColumns(int posBeg, int posEnd)
		{
			CleanColumns( 0, posBeg, posEnd );
		}

		/// <summary>
		/// Cleans the contents of some columns. Can throw ApplicationException
		/// when detecting invalid parameters (negative, etc.)
		/// </summary>
		/// <param name="rowStart">
		/// A <see cref="System.Int32"/> representing the starting row
		/// </param>
		/// <param name="posBeg">
		/// A <see cref="System.Int32"/> representing the first column
		/// </param>
		/// <param name="posEnd">
		/// A <see cref="System.Int32"/> representing the last column (included)
		/// </param>
		public void CleanColumns(int rowStart, int posBeg, int posEnd)
		{
			// Check
			ChkValue( posBeg, 0, NumColumns, "the beginning column" );
			ChkValue( posEnd, 0, NumColumns, "the ending column" );
			ChkValue( rowStart, 0, NumRows, "the beginning row" );

			if ( posBeg > posEnd ) {
				throw new ApplicationException( "invalid parameters (beg>end)" );
			}

			// Do it
			for(int i = rowStart; i < NumRows; ++i) {
				for(int j = posBeg; j <= posEnd; ++j) {
					this.data[ i ][ j ] = "";
				}
			}

			Changed = true;
		}

		/// <summary>
		/// Inserts some new rows at the given pos.
		/// </summary>
		/// <param name="pos">The position to insert in.</param>
		/// <param name="numRows">Number of blank rows to insert.</param>
		public void InsertRows(int pos, int numRows)
		{
			Owner.FormulaManager.AllowFormulaUpdating = false;

			// Chk
			ChkValue( pos, 0, NumRows, "row number for insertion" );
			ChkValue( numRows, 0, int.MaxValue, "number of rows to insert" );
			Changed = true;

			// Create list to insert
			var newRows = new List<List<string>>( numRows );
			for (int i = 0; i < numRows; ++i) {
				newRows.Add( new List<string>( new string[ this.NumColumns ] ) );
			}

			// Now insert
			this.data.InsertRange( pos, newRows );

			// Fix
			this.numRows += numRows;
			Owner.FormulaManager.FixFormulasRowsInserted( pos, numRows );
			Owner.FormulaManager.AllowFormulaUpdating = true;
		}

		public void RemoveRows(int pos, int numRows)
		{
			// Chk
			Owner.FormulaManager.AllowFormulaUpdating = false;
			ChkValue( pos, 0, NumRows, "row number for deletion" );
			ChkValue( numRows, 0, NumRows, "number of rows to remove" );
			Changed = true;

			// Value to really delete
			int removeCount =  Math.Min( numRows, this.NumRows - pos - 1 );

			if ( pos < ( this.NumRows - 1 ) ) {
				// Do it and fix
	            this.data.RemoveRange( pos, removeCount );
	            this.numRows -= removeCount;
			} else {
				this.numRows -=1;
			}
				
            Owner.FormulaManager.FixFormulasRowsRemoved( pos, removeCount );
			Owner.FormulaManager.AllowFormulaUpdating = true;
		}

		public void InsertColumns(int pos, int numCols)
		{
			// Chk
			Owner.FormulaManager.AllowFormulaUpdating = false;
			ChkValue( pos, 0, NumColumns, "column number for insertion" );
			ChkValue( numCols, 0, int.MaxValue, "number of rows to insert" );
			Changed = true;

			// For each row...
			for (int i = 0; i < NumRows; ++i) {
				this.data[ i ].InsertRange( pos, new string[ numCols ] );
			}

			// Prepare empty headers
			for(int j = pos; j < pos + numCols; ++j) {
				this.columnInfo.Insert( j, new ColumnInfo() {
                    Header = Core.ColumnInfo.ColEtq + Convert.ToString( j + 1 ) } );
			}

			// Fix
			this.numColumns += numCols;
			Owner.FormulaManager.FixFormulasColumnsInserted( pos, numCols );
			Owner.FormulaManager.AllowFormulaUpdating = true;
		}

		public void RemoveColumns(int pos, int numCols)
		{
			// Chk
			Owner.FormulaManager.AllowFormulaUpdating = false;
			ChkValue( pos, 0, NumColumns, "column number for deletion" );
			ChkValue( numCols, 0, NumColumns, "number of columns to remove" );
			Changed = true;

            // Real count to delete
			int removeCount = 1;

			if ( pos < ( this.NumColumns - 1 ) ) {
            	removeCount = Math.Min( numCols, this.NumColumns - pos - 1 );
			}

			// For each row...
			for (int i = 0; i < NumRows; ++i) {
                this.data[ i ].RemoveRange( pos, removeCount );
			}

			// Fix
            this.numColumns -= removeCount;
            this.columnInfo.RemoveRange( pos, removeCount );
            Owner.FormulaManager.FixFormulasColumnsRemoved( pos, removeCount );
			Owner.FormulaManager.AllowFormulaUpdating = true;
		}

		/// <summary>
		/// Creates the new columns info vector, saving the info from the previous one.
		/// </summary>
		/// <param name="newColNum">New number of columns.</param>
		private void CreateNewColumnsInfo(int newColNum)
		{
			int oldCount = this.columnInfo.Count;

			if ( newColNum != oldCount ) {
				if ( newColNum > oldCount ) {
					this.columnInfo.AddRange( new ColumnInfo[ newColNum - oldCount ] );
				} else {
					this.columnInfo.RemoveRange( newColNum, oldCount - newColNum );
				}

				// Assign values to new headers: col1, col2...
				for (int i = oldCount; i < newColNum; ++i) {
                    this.columnInfo[i] = new ColumnInfo {
                        Header = Core.ColumnInfo.ColEtq + (i + 1).ToString()
                    };
                }
			}

			return;
		}

		/// <summary>
		/// Creates the default headers, named col1, col2...
		/// </summary>
		internal void CreateDefaultHeaders()
		{
			CreateNewColumnsInfo( NumColumns );
			CreateDefaultHeaders( 0 );
            this.firstRowContainsHeaders = true;
		}

		/// <summary>
		/// Creates the default headers, starting in column "i"
		/// </summary>
		/// <param name="i">The index.</param>
		internal void CreateDefaultHeaders(int i)
		{
			for(int j = i; j < this.NumColumns; ++j) {
				this.columnInfo[ j ].Header = "Col" + Convert.ToString( j + 1 );
			}

			return;
		}

		/// <summary>
		/// Assigns the headers contents.
		/// </summary>
		/// <param name="headers">
		/// A <see cref="System.String[]"/> vector of headers.
		/// </param>
		internal void CreateNamedHeaders(string[] headers)
		{
			int i = 0;
			int lim = Math.Min( headers.Length, this.NumColumns );

			for(; i < lim; ++i) {
				this.columnInfo[ i ].Header = headers[ i ];
			}

			for (; i < this.NumColumns; ++i) {
				this.columnInfo[ i ].Header = "";
			}

			this.firstRowContainsHeaders = true;
		}

		/// <summary>
		/// Moves the first row to headers.
		/// </summary>
		private void MoveFirstRowToHeaders()
		{
            if ( this.NumRows > 0 ) {
    			// Copy the data into headers
    			for(int i = 0; i < ColumnInfo.Length; ++i) {
    				ColumnInfo[ i ].Header = this.data[ 0 ][ i ];
    			}

    			// Shift rows up
    			RemoveRows( 0, 1 );
            }

            return;
		}

		private void MoveHeadersToFirstRow()
		{
            if ( this.NumRows > 0 ) {
        			// Shift rows down
        			InsertRows( 0, 1 );

        			// Copy headers to first row
        			for(int j = 0; j < NumColumns; ++j) {
        				this.data[ 0 ][ j ] = this.ColumnInfo[ j ].Header;
        			}

        			CreateDefaultHeaders( 0 );
            }

            return;
		}

		private CsvDocument owner;
		private List<List<string>> data;
		private List<ColumnInfo> columnInfo;
		private bool firstRowContainsHeaders;
		private int numColumns;
		private int numRows;
	}
}

