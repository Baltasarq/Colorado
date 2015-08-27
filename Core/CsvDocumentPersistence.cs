using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Colorado.Core {
	public class CsvDocumentPersistence {
		public const string FileExtension = "csv";
		public const string TempExtension = "tmp";
        public const string Spaces = " \n\r";
		public const string FileFilter = "*." + FileExtension;

		public CsvDocumentPersistence() {
			this.document = null;
		}

		public CsvDocumentPersistence(CsvDocument doc) {
			this.document = doc;
		}

		/// <summary>
		/// Gets the document.
		/// </summary>
		/// <value>The CsvDocument.</value>
		public CsvDocument Document {
			get {
				return this.document;
			}
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
			
        /// <summary>
        /// Strips spaces from a text line, trimming it.
        /// Note this is needed because the TAB delimiter is part of the spaces
        /// in the standard library.
        /// </summary>
        /// <returns>The line passed, without spaces to the right or left</returns>
        /// <param name="line">The string to trim</param>
        private string TrimSpaces(string line) {
            int lIndex = 0;
            int rIndex = line.Length - 1;
            string toret = "";

            // Provided the line is not completely empty...
            if ( rIndex >= 0 ) {
                // Trim left
                while( lIndex < rIndex
                    && Spaces.IndexOf( line[ lIndex ] ) != -1 )
                {
                    ++lIndex;
                }
                    
                // Trim right
                while( rIndex >= lIndex
                    && Spaces.IndexOf( line[ rIndex ] ) != -1  ) 
                {
                    --rIndex;
                }
                    
                toret = line.Substring( lIndex, rIndex - lIndex + 1 );
            }

            return toret;
        }

        private void Read(string fileName, bool firstRowForHeaders, List<string> dynLines, ref string headers)
        {
            string line;

            using (var file = new StreamReader( fileName ))
            {
                while ( ( line = file.ReadLine() ) != null )
                {
                    line = TrimSpaces( line );

                    if ( line.Length == 0 )
                    {
                        continue;
                    }

                    if ( headers.Length == 0
                      && firstRowForHeaders )
                    {
                        headers = line;
                    } else {
                        dynLines.Add( line );
                    }
                }
            }

            return;
        }

		public void Load(string fileName, char delimiter = '\0', bool firstRowForHeaders = true)
		{
			var dynLines = new List<string>();
			string headers = "";

			this.document = new CsvDocument( 0, 0 );
			this.Document.FileName = fileName;
			this.Document.SurroundText = false;
			this.Read( fileName, firstRowForHeaders, dynLines, ref headers );

            if ( dynLines.Count > 0
              || headers.Length > 0 )
            {
                // Determine the delimiter
                if ( delimiter == '\0' ) {
                    this.DetermineDelimiter( headers );
                } else {
                    this.Document.DelimiterValue = delimiter.ToString();
                }

    			// Check for the existence of a single line, and proceed
                if ( dynLines.Count > 0 ) {
    				// Determine the number of rows and columns
                    this.Document.Data.SetInitialSize( dynLines.Count, SplitLine( dynLines[ 0 ] ).Length );

    				// Get the headers
    				if ( firstRowForHeaders ) {
                        this.Document.Data.CreateNamedHeaders( SplitLine( headers ) );
                    } else {
                        dynLines.Insert( 0, headers );
                    }

                    // Fill all data in
                    this.LoadCsvData( dynLines );
                } else {
                    // Prepare
                    string[] heads = this.SplitLine( headers );

                    // Get the headers
                    if ( firstRowForHeaders ) {
                        this.Document.Data.SetInitialSize( 0, heads.Length );
                        this.Document.Data.CreateNamedHeaders( heads );
                    } else {
                        this.Document.Data.SetInitialSize( 1, heads.Length );
                        dynLines.Add( headers );
                        this.LoadCsvData( dynLines );
                    }
                }
            } else {
                throw new ApplicationException( "No data in CSV document" );
            }

            this.Document.Changed = false;
            dynLines.Clear();
		}

		protected void DetermineDelimiter(string line)
		{
			// Eliminate all double-quoted text
			int qpos2 = 0;
			int qpos = line.IndexOf( CsvDocument.Quote );
			while( qpos > -1 ) {
                qpos2 = line.IndexOf( CsvDocument.Quote, qpos + 1 );

                if ( qpos2 > -1 ) {
					line = line.Remove( qpos, qpos2 - qpos + 1 );
                } else	{
                    line = line.Remove( qpos );
                }

                qpos = line.IndexOf( CsvDocument.Quote );
			}

			// Now yes, determine delimiter
            int delimiterIndex = -1;
            for(int i = 0; i < Delimiter.PredefinedDelimiters.Count; ++i) {
                if ( line.IndexOf( Delimiter.PredefinedDelimiters[ i ] ) > -1 ) {
                    delimiterIndex = i;
                }
            }

            if ( delimiterIndex < 0 ) {
                throw new ApplicationException( "Unable to determine delimiter in file." );
            } else {
                Document.DelimiterValue = Delimiter.PredefinedDelimiterNames[(int) delimiterIndex];
            }

            return;
		}

		private string FormatLoadedCell(string cell)
		{
			string toret = cell.Trim();

			if ( toret.Length > 0 ) {
				// Strip contents
				if ( toret[ 0 ] == '"' ) {
					// Remove double quotes
					toret = toret.Substring( 1 );

					if ( toret[ toret.Length -1 ] == '"' ) {
						toret = toret.Substring( 0, toret.Length -1 );
					}
				} else {
					toret = cell;
				}

				// Deduce decimal separator
				if ( DecimalMark.IsRealNumber( toret ) ) {
					this.Document.DecimalSeparator = DecimalMark.WhichDecimalMark( toret );
				}
			}

			return toret;
		}

		private string[] SplitLine(string line)
		{
			var row = new List<string>();
			var pos = 0;
			int i = 0;
			bool inQuoted = false;

            if ( line.Length == 0 ) {
                row = new List<string>( this.Document.Data.NumColumns );

                for(int n = 0; n < row.Count; ++n) {
                    row[ n ] = "";
                }
            } else {
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
    			      && line[ i ] == Document.DelimiterValue[ 0 ] )
    				{
    					row.Add( FormatLoadedCell( line.Substring( pos, i - pos ) ) );
    					pos = i + 1;
    				}
    				else
    				// Quote found
    				if ( line[ i ] == CsvDocument.Quote ) {
    					Document.SurroundText = true;
    					inQuoted = !inQuoted;
    				}
    			}

    			// Add last column
    			if ( pos < line.Length ) {
    				row.Add( FormatLoadedCell( line.Substring( pos, line.Length - pos ) ) );
    			}
    			else
    			if ( line[ line.Length -1 ] == Document.DelimiterValue[ 0 ] ) {
    				row.Add( "" );
    			}
            }

			return row.ToArray();
		}

        protected void LoadCsvData(IList<string> lines)
		{
			// Load all data, line by line
            for(int rowIndex = 0; rowIndex < Document.Data.NumRows; ++rowIndex) {
				// Get line info
				string[] cols = SplitLine( lines[ rowIndex ] );
				int colsLength = cols.Length;

                if ( colsLength > Document.Data.NumColumns ) {
					throw new ApplicationException( "Bad CSV format -- too variable number of columns" );
				}

				// Set data
                for(int colIndex = 0; colIndex < colsLength; ++colIndex) {
                    Document.Data[ rowIndex, colIndex ] = cols[ colIndex ];
				}
			}

            Document.Changed = false;
		}

		public void SaveCsvData()
		{
            SaveCsvData( new ExportOptions( Document.FileName, Document ) );
		}

		/// <summary>
		/// Quotes the cell for saving, if needed.
		/// A cell needs quoting if it contains a delimiter or a space.
		/// </summary>
		/// <returns>The cell for saving, with the modifications.</returns>
		/// <param name="cell">The cell information to save, as a string</param>
		protected string QuoteValueForSaving(string cell)
		{
			string toret = cell;

			if ( toret == null ) {
				toret = "";
			} else {
				// Take into account delimiters...
				var delimitersAndSpace = new HashSet<char>( Delimiter.PredefinedDelimiters );

				// ...and spaces...
				delimitersAndSpace.Add( ' ' );

                // ...and the current delimiter of the document (could be repeated)
                delimitersAndSpace.Add( Document.DelimiterValue[ 0 ] );

				foreach(char ch in cell) {
					if ( delimitersAndSpace.Contains( ch ) ) {
						// Quoting needed
						toret = CsvDocument.Quote + toret + CsvDocument.Quote;
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
			string fileName = options.Name + '.' + CsvDocumentPersistence.TempExtension;

			try {
				// Decide whether to use quotes or not
                if ( options.QuotedText ) {
                    prepareCellForSaving = new ModifyCell( QuoteValueForSaving );
                } else {
                    prepareCellForSaving = new ModifyCell( DontQuoteCellForSaving );
                }

				// Open file for saving
				file = new StreamWriter( fileName );

				// Write headers
				if ( options.IncludeRowNumbers ) {
					file.Write( "#" + Document.DelimiterValue );
				}

				for(int col = 0; col < options.ColumnsIncluded.Length; ++col) {
					file.Write( prepareCellForSaving(
                        Document.Data.ColumnInfo[ options.ColumnsIncluded[ col ] ].Header ) );

					if ( col < ( options.ColumnsIncluded.Length -1 ) ) {
						file.Write( options.Delimiter );
					}
				}
				file.WriteLine();

				// Write each row
                for(int row = 0; row < Document.Data.NumRows; ++row) {
					if ( options.IncludeRowNumbers ) {
                        file.Write( Convert.ToString( row +1 ) + Document.DelimiterValue );
					}

					for(int col = 0; col < options.ColumnsIncluded.Length; ++col) {
                        file.Write( prepareCellForSaving( Document.Data[ row, options.ColumnsIncluded[ col ] ] ) );

						if ( col < ( options.ColumnsIncluded.Length -1 ) ) {
							file.Write( options.Delimiter );
						}
					}
					file.WriteLine();
				}

                Document.Changed = false;
				file.Close();
				File.Delete( options.Name );
				File.Copy( fileName, options.Name, true );
			} catch(Exception) {
				throw;
			}
			finally {
				if ( file != null ) {
					file.Close();
					File.Delete( fileName );
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
			var file = new StreamWriter( fileName );

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

            for(int col = 0; col < Document.Data.NumColumns; ++col) {
				if ( options.IsColumnIncluded( col ) ) {
					file.Write( "<td style=\"color: white; background-color: black;\"><b>"
                        + Document.Data.ColumnInfo[ col ].Header + "</b></td>"
					);
				}
			}
			file.WriteLine();

			// Write each row
            for(int row = 0; row < Document.Data.NumRows; ++row) {
				file.WriteLine( "<tr>\n" );

				if ( options.IncludeRowNumbers ) {
					file.Write( "<td style=\"color: black; background-color: rgb(204,204,204);\"><b>" + Convert.ToString( row +1 ) + "</b></td>" );
				}

                for(int col = 0; col < Document.Data.NumColumns; ++col) {		
					if ( options.IsColumnIncluded( col ) ) {

						file.Write( "<td" );

						if ( ( row % 2 ) == 0 ) {
							file.Write( " style=\"color: black; background-color: rgb(204,204,204);\"" );
						}

                        file.Write( ">" + Document.Data[ row, col ] + "</td>" );
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
			var file = new StreamWriter( fileName );

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

            for(; colNumber < Document.Data.ColumnInfo.Length; ++colNumber)
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

			for(int col = 0; col < Document.Data.NumColumns; ++col) {
				if ( options.IsColumnIncluded( col ) ) {
                    file.WriteLine( "{\\b " + Document.Data.ColumnInfo[ col ].Header + "}\\cell" );
				}
			}
			file.WriteLine( "\\row\\trow\n" );

			// write each row
            for(int row = 0; row < Document.Data.NumRows; ++row) {
				if ( options.IncludeRowNumbers ) {
					file.WriteLine( Convert.ToString( row +1 ) + "\\cell" );
				}

                for(int col = 0; col < Document.Data.NumColumns; ++col) {
					if ( options.IsColumnIncluded( col ) ) {
                        file.WriteLine( Document.Data[ row, col ] + "\\cell" );
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

			for(int row = 0; row < Document.Data.NumRows; ++row) {
				textWriter.WriteStartElement( "Row" );               // Row

				foreach(var column in options.ColumnsIncluded) {							
					textWriter.WriteStartElement( "Cell" );          // Cell
					textWriter.WriteStartElement( "Data" );          // Data
					var data = Document.Data[ row, column ];
					double d;

					textWriter.WriteStartAttribute( "ss:Type" );
					if ( Double.TryParse( data, out d ) ) {
						textWriter.WriteString( "Number" );
					} else {
						textWriter.WriteString( "String" );
					}
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

        public void Save(ExportOptions options)
        {
            switch( options.Format ) {
                case ExportOptions.SelectionType.Rtf:
                    SaveCsvDataAsRtf( options );
                    break;
                case ExportOptions.SelectionType.Html:
                    SaveCsvDataAsHtml( options );
                    break;
                case ExportOptions.SelectionType.Excel:
                    SaveCsvDataAsExcel( options );
                    break;
                case ExportOptions.SelectionType.Csv:
                    SaveCsvData( options );
                    break;
                default:
                    throw new ApplicationException( "Internal: conversion not understood" );
            }

            return;
        }

		private CsvDocument document;
	}
}
