
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Text;

namespace Colorado.Core {	
	public class CsvDocument {
		public const char Quote = '"';
		public const string NewFileName = "newdoc.csv";
		public const string FileExtension = "csv";
		public const string TempExtension = "tmp";
		public const string FileFilter = "*." + FileExtension;

		private CsvDocument()
		{
			this.headers = new string[ 0 ];
			this.data = new string[ 0 ][];
			this.changed = true;
			this.rows = this.rowsCapacity = 0;
			this.columns = this.columnsCapacity = 0;
			this.fileName = NewFileName;
			this.delimiter = new Delimiter( Delimiter.TabDelimiter );
			this.HasName = false;
			this.firstRowForHeaders = true;
			this.surroundText = true;

			PrepareFormulas();
		}
		
        public CsvDocument(int rows, int cols)
			: this()
        {
            this.rows = this.rowsCapacity = rows;
            this.columns = this.columnsCapacity = cols;

            this.CreateDefaultHeaders();

            // Create data table
            data = new string[ Rows ][];
            for(int i = 0; i < Rows; ++i) {
                // Create & init each row
                data[ i ] = new string[ Columns ];

                for(int j = 0; j < Columns; ++j) {
                    data[ i ][j ] = "";
                }
            }

            return;
        }

        public CsvDocument(string fileName, bool firstRowForHeaders)
			: this()
        {
            Init( fileName, '\0', firstRowForHeaders );
        }

        public CsvDocument(string fileName, char delimiter, bool firstRowForHeaders)
			: this()
        {
            Init( fileName, delimiter, firstRowForHeaders );
        }
		
		public delegate void ClientUpdate(int row, int col, string value);
		public ClientUpdate clientUpdater = null;
		
		public bool FirstRowForHeaders {
			get { return firstRowForHeaders; }
			set {
				if ( value != this.firstRowForHeaders ) {
					if ( value )
							this.MoveFirstRowToHeaders();
					else 	this.MoveHeadersToFirstRow();
					
					this.firstRowForHeaders = value;
				}
			}
		}

		public bool AllowFormulaUpdating {
			get; set;
		}

		public int Rows {
			get { return rows; }
			set { SetRows( value ); }
		}
		
		public string[] Headers {
			get {
				return headers;
			}
		}
		
		public string[][] Data {
			get { return data; }
		}

		public int Columns {
			get { return columns; }
			set { SetColumns( value ); }
		}

		public bool Changed {
			get { return changed; }
			set { this.changed = value; this.UpdateFormulas(); }
		}

		public string FileName {
			get { return fileName; }
			set { PrepareFileName( ref value ); fileName = value; }
		}
		
		public int RowsCapacity {
			get { return rowsCapacity; }
		}
		
		public int ColumnsCapacity {
			get { return columnsCapacity; }
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
		
		protected void CreateDefaultHeaders()
		{
			headers = new string[ this.Columns ];
			this.CreateDefaultHeaders( 0 );
		}
		
		protected void CreateDefaultHeaders(int i)
		{
			for(int j = i; j < Columns; ++j) {
				this.headers[ j ] = "Col" + Convert.ToString( j + 1 );
			}
		}
		
        private void PrepareFormulas()
        {
            this.formulas = new List<Formula>();
            this.AllowFormulaUpdating = true;
        }

		public void Init(string fileName, char delimiter, bool firstRowForHeaders)
		{
			var dynLines = new List<string>();
			string[] lines = null;
			string line;
			string headers = "";

			this.fileName = fileName;
			this.formulas = new List<Formula>();
			this.AllowFormulaUpdating = true;
            this.surroundText = true;

			// Read the file
			using ( var file = new System.IO.StreamReader( fileName ) ) {
				while( ( line = file.ReadLine() ) != null )
				{
					if ( line.Trim().Length == 0 ) {
						continue;
					}
					
					if ( headers.Length == 0
					  && firstRowForHeaders )
					{
							headers = line;
					} else	dynLines.Add( line );
				}
			}

			// Get all the lines in the file
			lines = dynLines.ToArray();
			
			// Determine rows number
			this.rowsCapacity = this.rows = lines.Length;
			
			// Check for the existence of a single line, and proceed
			if ( lines.Length > 0 ) {
				// Determine the delimiter
				if ( delimiter == '\0' )
						this.DetermineDelimiter( lines[ 0 ] );
				else	this.Delimiter.Name = delimiter.ToString();
				
				// Determine the number of columns
				SurroundText = false;
				this.columns = this.columnsCapacity = SplitLine( lines[ 0 ] ).Length;
				
				// Fill all structure info
				LoadCsvData( lines );
				
				// Get the headers
				if ( firstRowForHeaders ) {
					PrepareHeaders( SplitLine( headers ) );
					this.firstRowForHeaders = true;
				} else {
					// Assign labels to columns
					this.firstRowForHeaders = false;
					this.headers = new string[ Columns ];
					for(int i = 0; i < Columns; ++i) {
						Headers[ i ] = "Col" + Convert.ToString( i );
					}
				}
				
				this.Changed = false;
			} else throw new ApplicationException( "No data in CSV document" );
		}
		
		/// <summary>
		/// Adds a formula.
		/// </summary>
		/// <param name="f">
		/// A <see cref="Formula"/> that will be added to the document.
		/// </param>
		public void AddFormula(Formula f)
		{
			int oldPos = -1;
			int newPos = -1;
			
			// Check the place for the new formula
			for(int i = 0; i < this.formulas.Count; ++i) {
				if ( this.formulas[ i ].Position == f.Position ) {
					oldPos = i;
					break;
				}
				else
				if ( f.Position < this.formulas[ i ].Position ) {
					newPos = i;
					break;
				}
			}
			
			// Add formula
			if ( oldPos < 0 ) {
				if ( newPos < 0 ) {
					this.formulas.Add( f );
				} else {
					this.formulas.Insert( newPos, f );
				}
			} else {
				this.formulas[ oldPos ] = f;
			}
			
			this.Changed = true;
		}
		
		/// <summary>
		/// Recalculates the results of all existing formulas.
		/// </summary>
		public void UpdateFormulas()
		{
			if ( this.AllowFormulaUpdating ) {
				foreach(var f in this.formulas) {
					// Apply formula & store result
					string result = f.DoIt().ToString();
					this.Data[ f.Position.Row ][ f.Position.Column ] = result;
					
					// Update clients
					if ( this.clientUpdater != null ) {
						this.clientUpdater( f.Position.Row, f.Position.Column, result );
					}
				}
			}
		}

		/// <summary>
		/// Moves the formula f to the new position
		/// </summary>
		/// <param name="f">
		/// A <see cref="Formula"/> holding the formula to move.
		/// </param>
		/// <param name="newPos">
		/// A <see cref="Position"/> holding the position to move the formula to.
		/// </param>
		public void MoveFormula(Formula f, Position newPos)
		{
			//this.Data[ f.Position.Row ][ f.Position.Column ] = "";
			f.Position = newPos;
		}

		/// <summary>
		/// Eliminates/fixes all formulas deleted with columns
		/// </summary>
		/// <param name="numCol">
		/// A <see cref="System.Int32"/> holding the column deleted.
		/// </param>
		/// <param name="num">
		/// A <see cref="System.Int32"/> holding the number of columns deleted.
		/// </param>
		public void FixFormulasColumnsRemoved(int numCol, int num)
		{
			for(int i = 0; i < this.formulas.Count; ++i) {
				if ( this.formulas[ i ].Position.Column >= numCol
				  && this.formulas[ i ].Position.Column < ( numCol + num ) )
				{
					this.formulas.RemoveAt( i );
					--i;
				}
				else
				if ( this.formulas[ i ].Position.Column > numCol ) {
					MoveFormula( this.formulas[ i ],
							     new Position( this,
											   this.formulas[ i ].Position.Row,
							  				   this.formulas[ i ].Position.Column - num )
					);
				}
			}

			this.UpdateFormulas();
		}

		/// <summary>
		/// Fixes all formulas when columns inserted.
		/// </summary>
		/// <param name="numCol">
		/// A <see cref="System.Int32"/> holding the position in which the column inserted.
		/// </param>
		/// <param name="num">
		/// A <see cref="System.Int32"/> holding the number of columns inserted.
		/// </param>
		public void FixFormulasColumnsInserted(int numCol, int num)
		{
			for(int i = 0; i < this.formulas.Count; ++i) {
				if ( this.formulas[ i ].Position.Column >= numCol ) {
					MoveFormula( this.formulas[ i ],
							     new Position( this,
											   this.formulas[ i ].Position.Row,
							  				   this.formulas[ i ].Position.Column + num )
					);
				}
			}

			this.UpdateFormulas();
		}

		/// <summary>
		/// Eliminates/fixes all formulas deleted with rows.
		/// </summary>
		/// <param name="numRow">
		/// A <see cref="System.Int32"/> holding the row deleted.
		/// </param>
		/// <param name="num">
		/// A <see cref="System.Int32"/> holding the number of rows deleted.
		/// </param>
		public void FixFormulasRowsRemoved(int numRow, int num)
		{
			for(int i = 0; i < this.formulas.Count; ++i) {
				if ( this.formulas[ i ].Position.Row >= numRow
				  && this.formulas[ i ].Position.Row < ( numRow + num ) )
				{
					this.formulas.RemoveAt( i );
					--i;
				}
				else
				if ( this.formulas[ i ].Position.Row > numRow ) {
					MoveFormula( this.formulas[ i ],
							     new Position( this,
						 					   this.formulas[ i ].Position.Row - num,
							  				   this.formulas[ i ].Position.Column )
					);
				}
			}

			this.UpdateFormulas();
		}

		/// <summary>
		/// Fixes all formulas when rows inserted.
		/// </summary>
		/// <param name="numRow">
		/// A <see cref="System.Int32"/> holding the position in which the rows inserted.
		/// </param>
		/// <param name="num">
		/// A <see cref="System.Int32"/> holding the number of rows inserted.
		/// </param>
		public void FixFormulasRowsInserted(int numRow, int num)
		{
			for(int i = 0; i < this.formulas.Count; ++i) {
				if ( this.formulas[ i ].Position.Row >= numRow ) {
					MoveFormula( this.formulas[ i ],
							     new Position( this,
							  				   this.formulas[ i ].Position.Row + num,
							  				   this.formulas[ i ].Position.Column )
					);
				}
			}

			this.UpdateFormulas();
		}
		
		/// <summary>
		/// Eliminates a formula in a given position.
		/// </summary>
		/// <param name="row">
		/// A <see cref="System.Int32"/> holding the row in which the formula lives.
		/// </param>
		/// <param name="col">
		/// A <see cref="System.Int32"/> holding the column in which the formula lives.
		/// </param>
		public void RemoveFormula(int row, int col)
		{
			int i = 0;
			var pos = new Position( this, row, col );
			
			for(i = 0; i < this.formulas.Count; ++i) {
				if ( this.formulas[ i ].Position == pos ) {
					this.formulas.RemoveAt( i );
					break;
				}
			}
			
			return;
		}
		
		/// <summary>
		/// Assigns the headers contents.
		/// </summary>
		/// <param name="headers">
		/// A <see cref="System.String[]"/> vector of headers. This vector can be or not the final
		/// vector of headers to be assigned to this.vector.
		/// </param>
		/// <returns>
		/// A <see cref="System.String[]"/> vector of headers. The this.Headers
		/// </returns>
		protected string[] PrepareHeaders(string[] headers)
		{
			this.headers = headers;
			
			if ( this.headers.Length < this.Columns ) {
				int i = 0;
				this.headers = new string[ this.Columns ];
				
				for(; i < headers.Length; ++i) {
					this.headers[ i ] = headers[ i ];
				}
				
				this.CreateDefaultHeaders( i );
			}
			else
			if ( headers.Length > this.Columns ) {
				this.Columns = headers.Length;
			}
			
			return this.Headers;
		}
		
		protected void DetermineDelimiter(string line)
		{
			// Eliminate all double-quoted text
			int qpos2 = 0;
			int qpos = line.IndexOf( Quote );
			while( qpos > -1 ) {
				qpos2 = line.IndexOf( Quote, qpos + 1 );
				
				if ( qpos2 > -1 )
						line = line.Remove( qpos, qpos2 - qpos + 1 );
				else	line = line.Remove( qpos );

				qpos = line.IndexOf( Quote );
			}
			
			// Now yes, determine delimiter
			if ( line.IndexOf( Delimiter.TabDelimiter ) > -1 ) {
				Delimiter.Name = Delimiter.TabDelimiterName;
			}
			else
			if ( line.IndexOf( Delimiter.SemicolonDelimiter ) > -1 ) {
				Delimiter.Name = Delimiter.SemicolonDelimiter.ToString();
			}
			else
			if ( line.IndexOf( Delimiter.CommaDelimiter ) > -1 ) {
				Delimiter.Name = Delimiter.CommaDelimiter.ToString();
			}
			else throw new ApplicationException( "Unable to determine delimiter in file." );
		}
		
		protected string[] SplitLine(string line)
		{
			var row = new List<string>();
			var pos = 0;
			int i = 0;
			bool inQuoted = false;

			// Look for cells
			for(; i < line.Length; ++i) {
				// Ignore extra CR&LF, due to file port between platforms
				if ( !inQuoted
				    && ( line[ i ] == '\n'
				      || line[ i ] == '\r' ) )
				{
					continue;
				}
				    
				
				// Delimiter found, add cell
				if ( !inQuoted
				  && line[ i ] == Delimiter.Raw )
				{
					row.Add( PrepareCell( line.Substring( pos, i - pos ) ) );
					pos = i + 1;
				}
				else
				// Quote found
				if ( line[ i ] == Quote ) {
					SurroundText = true;
					inQuoted = !inQuoted;
				}
			}
	
			// Add last column
			if ( pos < line.Length ) {
				row.Add( PrepareCell( line.Substring( pos, line.Length - pos ) ) );
			}
			else
			if ( line[ line.Length -1 ] == Delimiter.Raw ) {
				row.Add( "" );
			}
			
			return row.ToArray();
		}
		
		public static string PrepareCell(string cell)
		{
			// Prepare
			string toret = cell.Trim();
			
			if ( toret.Length > 0 ) {
				if ( toret[ 0 ] == '"' ) {
					// Remove double quotes
					toret = toret.Substring( 1 );
					
					if ( toret[ toret.Length -1 ] == '"' ) {
						toret = toret.Substring( 0, toret.Length -1 );
					}
				} else {
					toret = cell;
				}
			}
			
			return toret;
		}
		
		protected void LoadCsvData(string[] lines)
		{
			int colsLength;
			string[] cols = null;
			
			// Reserve memory
			data = new string[rows][];
			
			// Load all data, line by line
			for(int row = 0; row < Rows; ++row) {
				// Get line info
				cols = SplitLine( lines[ row ] );
				colsLength = cols.Length;
				
				if ( colsLength > Columns ) {
					throw new ApplicationException( "Bad CSV format -- too variable number of columns" );
				}
				
				// Create data
				data[ row ] = new string[ Columns ];
				int i = 0;
				for(; i < colsLength; ++i) {
					data[ row ][ i ] = cols[ i ];
				}
				
				for (; i < Columns; ++i) {
					data[ row ][ i ] = "";
				}
			}
			
			Changed = false;
		}
		
		public void SaveCsvData()
		{
			SaveCsvData( new ExportOptions( FileName, this ) );
		}

		/// <summary>
		/// Quotes the cell for saving, if needed.
		/// A cell needs quoting if it contains a delimiter, or contains a space.
		/// </summary>
		/// <returns>The cell for saving, with the modifications.</returns>
		/// <param name="cell">The cell information to save, as a string</param>
		protected string QuoteCellForSaving(string cell)
		{
			string toret = cell;
			
			if ( toret == null ) {
				toret = "";
			} else {
				// Take into account delimiters...
				var delimitersAndSpace = new HashSet<char>( Delimiter.PredefinedDelimiters );

				// ...and spaces...
				delimitersAndSpace.Add( ' ' );

				// ...and the current delimiter of the document
				delimitersAndSpace.Add( this.Delimiter.Raw );		// Could be repeated

				foreach(char ch in cell) {
					if ( delimitersAndSpace.Contains( ch ) ) {
						// Quoting needed
						toret = Quote + toret + Quote;
						break;
					}
				}
			}
			    
			return toret;
		}
		
		protected string DontQuoteCellForSaving(string cell)
		{
			string toret = cell;
			
			if ( toret == null ) {
				toret = "";
			}
			
			return toret;
		}
		
		protected delegate string ModifyCell(string cell);
			
		public void SaveCsvData(ExportOptions options)
		{
			System.IO.StreamWriter file = null;
			ModifyCell prepareCellForSaving;
			string fileName = options.Name + '.' + TempExtension;
			
			try {
				// Decide whether to use quotes or not
				if ( options.QuotedText )
						prepareCellForSaving = new ModifyCell( QuoteCellForSaving );
				else 	prepareCellForSaving = new ModifyCell( DontQuoteCellForSaving );
	
				// Open file for saving
				file = new System.IO.StreamWriter( fileName );
				
				// Write headers
				if ( options.IncludeRowNumbers ) {
					file.Write( "#" + Delimiter );
				}
	
				for(int col = 0; col < options.ColumnsIncluded.Length; ++col) {
					file.Write( prepareCellForSaving( Headers[ options.ColumnsIncluded[ col ] ] ) );
					
					if ( col < ( options.ColumnsIncluded.Length -1 ) ) {
						file.Write( options.Delimiter );
					}
				}
				file.WriteLine();
				
				// Write each row
				for(int row = 0; row < Rows; ++row) {
					
					if ( options.IncludeRowNumbers ) {
						file.Write( Convert.ToString( row +1 ) + Delimiter );
					}
					
					for(int col = 0; col < options.ColumnsIncluded.Length; ++col) {
						file.Write( prepareCellForSaving( Data[ row ][ options.ColumnsIncluded[ col ] ] ) );
						
						if ( col < ( options.ColumnsIncluded.Length -1 ) ) {
							file.Write( options.Delimiter );
						}
					}
					file.WriteLine();
				}
				
				Changed = false;
				file.Close();
				File.Delete( options.Name );
				File.Copy( fileName, options.Name, true );
			} catch(Exception) {
				throw;
			}
			finally {
				if ( file != null ) {
					file.Close();
					System.IO.File.Delete( fileName );
				}
			}
			
			return;
		}
		
		public void SaveCsvDataAsHtml(ExportOptions options)
		{
			string fileName = options.Name;
			int tableBorder = 0;
			
			// Determine border
			if ( options.IncludeTableBorder ) {
				++tableBorder;
			}
			
			// Open file for saving
			var file = new System.IO.StreamWriter( fileName );
			
			// Write html header
			file.WriteLine( "<html><header>" );
			file.WriteLine( "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">" );
			file.WriteLine( "<title>"
			               + System.IO.Path.GetFileNameWithoutExtension( fileName )
			               + "</title>"
			);

			file.WriteLine( "</header><body><table border={0}>\n", tableBorder );
			
			// Write headers
			file.WriteLine( "<tr>\n" );
			
			if ( options.IncludeRowNumbers ) {
				file.Write( "<td style=\"color: white; background-color: black;\"><b>#</b></td>" );
			}
			
			for(int col = 0; col < Columns; ++col) {
				if ( options.IsColumnIncluded( col ) ) {
					file.Write( "<td style=\"color: white; background-color: black;\"><b>"
					            + Headers[ col ] + "</b></td>"
					);
				}
			}
			file.WriteLine();
			
			// Write each row
			for(int row = 0; row < Rows; ++row) {
				file.WriteLine( "<tr>\n" );
				
				if ( options.IncludeRowNumbers ) {
					file.Write( "<td style=\"color: black; background-color: rgb(204,204,204);\"><b>" + Convert.ToString( row +1 ) + "</b></td>" );
				}
				
				for(int col = 0; col < Columns; ++col) {		
					if ( options.IsColumnIncluded( col ) ) {
					
						file.Write( "<td" );
						
						if ( ( row % 2 ) == 0 ) {
							file.Write( " style=\"color: black; background-color: rgb(204,204,204);\"" );
						}
						
						file.Write( ">" + Data[ row ][ col ] + "</td>" );
					}
				}
				file.WriteLine();
			}
			
			// End
			file.WriteLine( "</table></body></html>\n" );
			file.Close();
			return;
		}
		
		public void SaveCsvDataAsRtf(ExportOptions options)
		{
			int colNumber = 0;
			string fileName = options.Name;
			const string TableBorder = "\\clbrdrt\\brdrs\\clbrdrl\\brdrs\\clbrdrb\\brdrs\\clbrdrr\\brdrs";
			
			// Open file for saving
			var file = new System.IO.StreamWriter( fileName );
			
			// write rtf header
			file.WriteLine( "{\\rtf2\\ansi\\deff0\n{\\fonttbl {\\f0 Times;}{\\f1 Courier;}}" );
			
			// write rtf table codes
			file.WriteLine( "\\trowd\\trautofit1\\trgaph144" );

			if ( options.IncludeRowNumbers ) {
				if ( options.IncludeTableBorder ) {
					file.Write( TableBorder );
				}
				
				file.Write( "\\cellx1000" );
				colNumber = 1;
			}
			
			for(; colNumber < Headers.Length; ++colNumber)
			{
				if ( options.IsColumnIncluded( colNumber - ( options.IncludeRowNumbers ? 1 : 0 ) ) )
				{
					if ( options.IncludeTableBorder ) {
						file.Write( TableBorder );
					}
					
					file.WriteLine( "\\cellx" + Convert.ToString( ( colNumber + 1 ) * 2000 ) );
				}
			}
			file.WriteLine( "\\intbl\n" );
			
			// write headers
			if ( options.IncludeRowNumbers ) {
				file.WriteLine( "{\\b #}\\cell" );
			}
			
			for(int col = 0; col < Columns; ++col) {
				if ( options.IsColumnIncluded( col ) ) {
					file.WriteLine( "{\\b " + Headers[ col ] + "}\\cell" );
				}
			}
			file.WriteLine( "\\row\\trow\n" );
			
			// write each row
			for(int row = 0; row < Rows; ++row) {
				if ( options.IncludeRowNumbers ) {
					file.WriteLine( Convert.ToString( row +1 ) + "\\cell" );
				}
				
				for(int col = 0; col < Columns; ++col) {
					if ( options.IsColumnIncluded( col ) ) {
						file.WriteLine( Data[ row ][ col ] + "\\cell" );
					}
				}
				file.WriteLine( "\\row\\trow\n" );
			}
			
			// End
			file.WriteLine( "\n\\pard\n}" );
			file.Close();
			return;
		}
		
		public void SaveCsvDataAsExcel(ExportOptions options)
		{
			XmlTextWriter textWriter = new XmlTextWriter( options.Name, Encoding.UTF8 );
			textWriter.WriteStartDocument();
			
			textWriter.WriteStartElement( "Workbook" );              // Workbook
			
			textWriter.WriteStartAttribute( "xmlns" );
			textWriter.WriteString( "urn:schemas-microsoft-com:office:spreadsheet" );
			textWriter.WriteEndAttribute();
			
			textWriter.WriteStartAttribute( "xmlns:o" );
			textWriter.WriteString( "urn:schemas-microsoft-com:office:office" );
			textWriter.WriteEndAttribute();
			
			textWriter.WriteStartAttribute( "xmlns:x" );
			textWriter.WriteString( "urn:schemas-microsoft-com:office:excel" );
			textWriter.WriteEndAttribute();
			
			textWriter.WriteStartAttribute( "xmlns:ss" );
			textWriter.WriteString( "urn:schemas-microsoft-com:office:spreadsheet" );
			textWriter.WriteEndAttribute();
			
			textWriter.WriteStartAttribute( "xmlns:html" );
			textWriter.WriteString( "http://www.w3.org/TR/REC-html40" );
			textWriter.WriteEndAttribute();
			
			textWriter.WriteStartElement( "Worksheet" );             // Worksheet
			textWriter.WriteStartAttribute( "ss:Name" );
			textWriter.WriteString( AppInfo.Name + " XSL export" );
			textWriter.WriteEndAttribute();
			
			textWriter.WriteStartElement( "Table" );                 // Table
			
			for(int row = 0; row < Rows; ++row) {
				textWriter.WriteStartElement( "Row" );               // Row
				
				foreach(var column in options.ColumnsIncluded) {							
					textWriter.WriteStartElement( "Cell" );          // Cell
					textWriter.WriteStartElement( "Data" );          // Data
					var data = Data[ row ][ column ];
					double d;
										
					textWriter.WriteStartAttribute( "ss:Type" );
					if ( Double.TryParse( data, out d ) )
						    textWriter.WriteString( "Number" );
					else    textWriter.WriteString( "String" );
					textWriter.WriteEndAttribute();
					
					textWriter.WriteString( data );
					textWriter.WriteEndElement();                    // /Data
					textWriter.WriteEndElement();                    // /Cell
				}
				textWriter.WriteEndElement();                        // /Row
			}
			
			textWriter.WriteEndElement();                            // /Table
			textWriter.WriteEndElement();                            // /Worksheet
			
			
			
			textWriter.WriteEndElement();                            // /WorkBook
			textWriter.WriteEndDocument();
			textWriter.Close();
			return;
		}
		
		public void SetRows(int rows)
		{
			if ( rows != Rows
			  && rows >= 0 )
			{
				Changed = true;
				
				this.rows = rows;
				
				if ( Rows > RowsCapacity ) {
					// Set the new capacity
					do {
						this.rowsCapacity <<= 1;
					} while( Rows >= RowsCapacity );
					
					// Reserve new memory
					var newRows = new string[ RowsCapacity ][];

					// Copy old values & replace
					int i = 0;
					for(; i < Data.Length; ++i) {
						newRows[ i ] = Data[ i ];
					}
					for(; i < RowsCapacity; ++i) {
						newRows[ i ] = new string[ ColumnsCapacity ];
						
						for(int j = 0; j < ColumnsCapacity; ++j) {
							newRows[ i ][ j ] = "";
						}
					}
					
					this.data = newRows;
				}
			}
		}
		
		public void SetColumns(int cols)
		{
			if ( cols != this.Columns
			  && cols > 0 )
			{
				this.Changed = true;
				this.columns = cols;
				
				if ( this.ColumnsCapacity < cols ) {
					// Set the new capacity
					do {
						this.columnsCapacity <<= 1;
					} while( cols >= this.ColumnsCapacity );
					
					// Copy all the data & add new empty data
					for(int i = 0; i < this.RowsCapacity; ++i) {
						var newRow = new string[ this.ColumnsCapacity ];
						
						// Copy all data in the row
						int j = 0;
						for(; j < this.Data[ i ].Length; ++j) {
							newRow[ j ] = Data[ i ][ j ];
						}
						
						// Complete the row
						for(; j < this.ColumnsCapacity; ++j) {
							newRow[ j ] = "";
						}
						
						this.data[ i ] = newRow;
					}
				}

				// Fix headers
				if ( cols != this.Headers.Length ) {
					var newHeadersSize = Headers.Length;
					var newHeaders = new string[ cols ];
					
					if ( newHeadersSize > Columns ) {
						newHeadersSize = Columns;
					}
					
					// Copy
					int col = 0;
					for(; col < newHeadersSize; ++col) {
						newHeaders[ col ] = Headers[ col ];
					}
					for(; col < Columns; ++col) {
						newHeaders[ col ] = "Col" + Convert.ToString( col );
					}
					
					this.headers = newHeaders;
				}
			}
		}
		
		public void CopyRow(int posOrg, int posDest)
		{
			// Chk
			if ( posOrg == posDest ) {
				return;
			}
			ChkValue( posOrg, 0, Rows, "beginning row for copying" );
			ChkValue( posDest, 0, Rows, "beginning row for copying" );
			Changed = true;
			
			// do it
			for(int j = 0; j < ColumnsCapacity; ++j) {
				Data[ posDest ][ j ] = Data[ posOrg ][ j ];
			}
		}
		
		public void CopyColumn(int posOrg, int posDest)
		{
			// Chk
			if ( posOrg == posDest ) {
				return;
			}
			
			ChkValue( posOrg, 0, Columns, "beginning row for copying" );
			ChkValue( posDest, 0, Columns, "beginning row for copying" );
			Changed = true;
			
			// do it
			for(int i = 0; i < Rows; ++i) {
				Data[ i ][ posDest ] = Data[ i ][ posOrg ];
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
		public void ChkValue(int val, int min, int max, string explanation)
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
		/// An adapter for <see cref="cleanRows"/>
		/// </summary>
		public void CleanRows(int posBeg, int posEnd)
		{
			CleanRows( 0, posBeg, posEnd );
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
			ChkValue( posBeg, 0, Rows, "beginning row" );
			ChkValue( posEnd, 0, Rows, "ending row" );
			ChkValue( colStart, 0, Columns, "the beginning column" );
			
			if ( posBeg > posEnd ) {
				throw new ApplicationException( "invalid parameters (beg>end)" );
			}
			
			// Do it
			for(int i = posBeg; i <= posEnd; ++i) {
				for(int j = colStart; j < ColumnsCapacity; ++j) {
					Data[ i ][ j ] = "";
				}
			}
			
			Changed = true;
		}
		
		/// <summary>
		/// An adapter for <see cref="cleanColumns"/>
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
			ChkValue( posBeg, 0, Columns, "the beginning column" );
			ChkValue( posEnd, 0, Columns, "the ending column" );
			ChkValue( rowStart, 0, Rows, "the beginning row" );
			
			if ( posBeg > posEnd ) {
				throw new ApplicationException( "invalid parameters (beg>end)" );
			}
			
			// Do it
			for(int i = rowStart; i < RowsCapacity; ++i) {
				for(int j = posBeg; j <= posEnd; ++j) {
					Data[ i ][ j ] = "";
				}
			}
			
			Changed = true;
		}
		
		public void InsertRows(int pos, int numRows)
		{
			this.AllowFormulaUpdating = false;

			// Chk
			ChkValue( pos, 0, Rows, "row number for insertion" );
			ChkValue( numRows, 0, Rows, "number of rows to insert" );
			Changed = true;
			
			// Make room for the new rows
			int oldNumRows = Rows;
			Rows += numRows;
			
			// Copy
			for(int i = oldNumRows -1; i >= pos; --i) {
				CopyRow( i, i + numRows );
			}
			
			CleanRows( pos, pos + numRows -1 );

			// Fix formulas
			FixFormulasRowsInserted( pos, numRows );
		}
		
		public void RemoveRows(int pos, int numRows)
		{
			// Chk
			this.AllowFormulaUpdating = false;
			ChkValue( pos, 0, Rows, "row number for deletion" );
			ChkValue( numRows, 0, Rows, "number of rows to remove" );
			Changed = true;
			
			// Copy
			int j = 0;
			for(int i = pos + numRows; i < Rows; ++i, ++j) {
				CopyRow( i, pos + j );
			}
			
			Rows -= numRows;

			// Fix formulas
			FixFormulasRowsRemoved( pos, numRows );
            this.AllowFormulaUpdating = true;
		}
		
		public void InsertColumns(int pos, int numCols)
		{
			// Chk
			this.AllowFormulaUpdating = false;
			ChkValue( pos, 0, Columns, "column number for insertion" );
			ChkValue( numCols, 0, int.MaxValue, "number of rows to insert" );
			Changed = true;
			
			// Make room for the new columns
			int oldNumCols = Columns;
			Columns += numCols;
			
			// Copy each row
			for(int i = 0; i < RowsCapacity; ++i) {
				// Copy each column
				for(int j = oldNumCols -1; j >= pos; --j) {
					Data[ i ][ j + numCols ] = Data[ i ][ j ];
				}
				
				// Clean now empty space in the middle
				for(int j = pos; j < ( pos + numCols ); ++j) {
					Data[ i ][ j ] = "";
				}
			}
			
			// Move headers
			for(int j = oldNumCols -1; j >= pos; --j) {
				this.headers[ j + numCols ] = this.headers[ j ];
			}
			
			// Prepare empty headers
			for(int j = pos; j < pos + numCols; ++j) {
				this.headers[ j ] = "NewCol" + Convert.ToString( j + 1 );
			}

			// Fix formulas
			FixFormulasColumnsInserted( pos, numCols );
            this.AllowFormulaUpdating = true;
		}
		
		public void RemoveColumns(int pos, int numCols)
		{
			// Chk
			this.AllowFormulaUpdating = false;
			ChkValue( pos, 0, Columns, "column number for deletion" );
			ChkValue( numCols, 0, Columns, "number of columns to remove" );
			Changed = true;
			
			// Copy
			int j = 0;
			for(int i = pos + numCols; i < Columns; ++i, ++j) {
				CopyColumn( i, pos + j );
			}
			
			// Fix headers
			j = 0;
			for(int i = pos + numCols; i < Columns; ++i, ++j) {
				this.headers[ pos + j ] = this.headers[ i ];
			}
			
			Columns -= numCols;

			// Fix formulas
			FixFormulasColumnsRemoved( pos, numCols );
            this.AllowFormulaUpdating = true;
		}
		
		public static void PrepareFileName(ref string fileName)
		{
			fileName = fileName.Trim();
			string fileNameLower = fileName.ToLower();
			
			if ( !fileNameLower.EndsWith( FileExtension.ToLower() ) ) {
				fileName += "." + FileExtension;
			}
			
			return;
		}
		
		protected void MoveFirstRowToHeaders()
		{
			int lastRow =  this.Rows -1;
			
			// Copy the data into headers
			for(int i = 0; i < this.Headers.Length; ++i) {
				this.Headers[ i ] = this.Data[ 0 ][ i ];
			}
			
			// Shift rows up
			for(int i = 0; i < lastRow; ++i) {
				this.Data[ i ] = this.Data[ i +1 ];
			}
			
			// Prepare last row
			this.Data[ lastRow ] = new string[ this.ColumnsCapacity ];
			for(int j = 0; j < this.ColumnsCapacity; ++j) {
				this.Data[ lastRow ][ j ] = "";
			}
			this.rows -= 1;
			
			this.Changed = true;
			return;
		}
		
		protected void MoveHeadersToFirstRow()
		{
			// Increment row space memory
			this.Rows += 1;
			
			// Shift rows down
			for(int i = this.Rows -1; i > 0; --i) {
				this.Data[ i ] = this.Data[ i -1 ];
			}
			
			// Prepare fist row
			int j = 0;
			this.Data[ 0 ] = new string[ this.ColumnsCapacity ];
			for(; j < this.ColumnsCapacity; ++j) {
				this.Data[ 0 ][ j ] = "";
			}
			
			// Copy headers to first row
			for(j = 0; j < this.Columns; ++j) {
				this.Data[ 0 ][ j ] = this.Headers[ j ];
			}
			for(; j < this.ColumnsCapacity; ++j) {
				this.Data[ 0 ][ j ] = "";
			}
			
			this.CreateDefaultHeaders();
			this.Changed = true;
			return;
		}

        private string[] headers;
        private int columns;
        private int rows;
        private int columnsCapacity;
        private int rowsCapacity;
        private Delimiter delimiter;
        private bool surroundText;
        private string[][] data;
        private bool changed;
        private string fileName;
        private bool hasName;
        private bool firstRowForHeaders;
        private List<Formula> formulas;
	}
}
