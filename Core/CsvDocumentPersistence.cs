// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core {
    using System;
    using System.IO;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;

	public class CsvDocumentPersistence {
        /// <summary>The available file extensions for CSV documents.</summary>
		public static ReadOnlyCollection<string> FileExtension = new ReadOnlyCollection<string>(new string[]{ "csv", "tsv" } );

        /// <summary>The file filters for the available extensions.</summary>
        public static ReadOnlyCollection<string> FileFilter = new ReadOnlyCollection<string>(
            new string[]{ "*." + FileExtension[ 0 ], "*." + FileExtension[ 1 ] } );

        /// <summary>Extension for temporary files.</summary>
		public const string TempExtension = "tmp";

        /// <summary>Spaces. Beware of including delimiters such tabs.</summary>
        public const string Spaces = " \n\r";

		public CsvDocumentPersistence()
        {
			this.Document = null;
		}

		public CsvDocumentPersistence(CsvDocument doc)
            :this()
        {
			this.Document = doc;
		}

		public static void PrepareFileName(ref string fileName)
		{
			fileName = fileName.Trim();
			string fileNameLower = fileName.ToLower();

			// Look in all extensions
			int i = 0;
			while ( i < FileExtension.Count ) {
				if ( fileNameLower.EndsWith( FileExtension[ i ].ToLower() ) ) {
					break;
				}

				++i;
			}

			// Okay, no valid extension: append one.
			if ( i >= FileExtension.Count ) {
				fileName += "." + FileExtension[ 0 ];
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
        public static string TrimSpaces(string line)
        {
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

        static void Read(TextReader reader, bool firstRowForHeaders, List<string> dynLines, ref string headers)
        {
            string line;

            while ( ( line = reader.ReadLine() ) != null )
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

            return;
        }

        public void Load(string fileName, char delimiter = '\0', bool firstRowForHeaders = true)
        {
            using (var reader = new StreamReader( fileName ) )
            {
                this.Load( reader, delimiter, firstRowForHeaders );
            }

            this.Document.FileName = fileName;
            return;
        }

        public void Load(TextReader reader, char delimiter = '\0', bool firstRowForHeaders = true)
		{
			var dynLines = new List<string>();
			string headers = "";

            this.Document = new CsvDocument( 0, 0 ) {
                FileName = "unknown.csv",
                SurroundText = false
            };

            Read( reader, firstRowForHeaders, dynLines, ref headers );

            if ( dynLines.Count > 0
              || headers.Length > 0 )
            {
                // Determine the delimiter
                if ( delimiter == '\0' ) {
                    string firstLine = "";

                    if ( firstRowForHeaders ) {
                        firstLine = headers;
                    } else {
                        if ( dynLines.Count > 0 ) {
                            firstLine = dynLines[ 0 ];
                        }
                    }

                    this.DetermineDelimiter( firstLine );
                } else {
                    this.Document.Delimiter = new Delimiter( delimiter );
                }

    			// Check for the existence of a single line, and proceed
                if ( dynLines.Count > 0 ) {
    				// Determine the number of rows and columns
                    this.Document.Data.SetInitialSize( dynLines.Count, SplitLine( dynLines[ 0 ] ).Length );

    				// Get the headers
    				if ( firstRowForHeaders ) {
                        this.Document.Data.CreateNamedHeaders( SplitLine( headers ) );
                    } else {
                        this.Document.Data.CreateDefaultHeaders();
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
                // No data in the document - invent a new one
                this.Document.Data.SetInitialSize( 1, 1 );

                if ( delimiter == '\0' ) {
                    this.Document.Delimiter = new Delimiter( Delimiter.TabDelimiter );
                } else {
                    this.Document.Delimiter = new Delimiter( delimiter );
                }

                this.Document.Data.CreateDefaultHeaders();
                this.Document.Data.FirstRowContainsHeaders = true;
            }

            this.Document.Changed = false;
            dynLines.Clear();
		}

		protected void DetermineDelimiter(string line)
		{
            // Tab separator as default, for documents of one column.
            Document.Delimiter = new Delimiter( Delimiter.TabDelimiter );

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
            for(int i = 0; i < Delimiter.PredefinedDelimiters.Count; ++i) {
                char delimiter = Delimiter.PredefinedDelimiters[ i ];

                if ( line.IndexOf( delimiter ) > -1 ) {
                    Document.Delimiter = new Delimiter( delimiter );
                    break;
                }
            }

            return;
		}

		string FormatLoadedCell(string cell)
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

		string[] SplitLine(string line)
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
    			      && line[ i ] == this.Document.DelimiterValue[ 0 ] )
    				{
    					row.Add( FormatLoadedCell( line.Substring( pos, i - pos ) ) );
    					pos = i + 1;
    				}
    				else
    				// Quote found
    				if ( line[ i ] == CsvDocument.Quote ) {
    					this.Document.SurroundText = true;
    					inQuoted = !inQuoted;
    				}
    			}

    			// Add last column
    			if ( pos < line.Length ) {
    				row.Add( this.FormatLoadedCell( line.Substring( pos, line.Length - pos ) ) );
    			}
    			else
    			if ( line[ line.Length -1 ] == this.Document.DelimiterValue[ 0 ] ) {
    				row.Add( "" );
    			}
            }

			return row.ToArray();
		}

        protected void LoadCsvData(IList<string> lines)
		{
            int numRows = lines.Count;
            int numCols = this.Document.Data.NumColumns;

            // Convert data to fixed structure
            string[][] rawData = new string[ numRows ][];

            for(int i = 0; i < numRows; ++i) {
                rawData[ i ] = SplitLine( lines[ i ] );
                numCols = Math.Max( numCols, rawData[ i ].Length );
            }

            // Resize document accordingly
            if ( this.Document.Data.NumRows != numRows
              || this.Document.Data.NumColumns != numCols )
            {
                string[] headers = this.Document.Data.Headers;

                this.Document.Data.SetInitialSize( numRows, numCols );
                this.Document.Data.CreateNamedHeaders( headers );
            }

			// Load all data, line by line
            for(int i = 0; i < numRows; ++i) {
                for(int j = 0; j < rawData[ i ].Length; ++j) {
                    Document.Data[ i, j ] = rawData[ i ][ j ];
				}
			}

            Document.Changed = false;
		}

		public void SaveCsvData()
		{
            this.SaveCsvData( new ExportOptions( Document.FileName, Document ) );
		}

        public void SaveCsvData(ExportOptions options)
		{
			StreamWriter file = null;
			string fileName = Path.GetTempFileName();
            Func<string, string, string> prepareCellForSaving = null;

			try {
				// Decide whether to use quotes or not
                if ( options.QuotedText ) {
                    prepareCellForSaving = QuoteCellForSaving;
                } else {
                    prepareCellForSaving = (cell, delimiter) => cell ?? "";
                }

				// Open file for saving
				file = new StreamWriter( fileName );

				// Write headers
				if ( options.IncludeRowNumbers ) {
					file.Write( "#" + Document.DelimiterValue );
				}

                for(int i = 0; i < options.ColumnsIncluded.Length; ++i) {
                    int colIndex = options.ColumnsIncluded[ i ];

					file.Write(
                        prepareCellForSaving(
                            Document.Data.ColumnInfo[ colIndex ].Header,
                            this.Document.DelimiterValue ) );

					if ( i < ( options.ColumnsIncluded.Length -1 ) ) {
						file.Write( options.Delimiter );
					}
				}

				file.WriteLine();

				// Write each row
                for(int row = 0; row < Document.Data.NumRows; ++row) {
					if ( options.IncludeRowNumbers ) {
                        file.Write( Convert.ToString( row +1 ) + Document.DelimiterValue );
					}

                    for(int i = 0; i < options.ColumnsIncluded.Length; ++i) {
                        int colIndex = options.ColumnsIncluded[ i ];

                        file.Write(
                            prepareCellForSaving(
                                this.Document.Data[ row, colIndex ],
                                this.Document.DelimiterValue ) );

						if ( i < ( options.ColumnsIncluded.Length - 1 ) ) {
							file.Write( options.Delimiter );
						}
					}

					file.WriteLine();
				}

                Document.Changed = false;
				file.Close();
				File.Copy( fileName, options.Path, true );
				File.Delete( fileName );
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

        /// <summary>
        /// Quotes the cell for saving, if needed.
        /// A cell needs quoting if it contains a delimiter or a space.
        /// </summary>
        /// <returns>The cell for saving, with the modifications.</returns>
        /// <param name="cell">The cell information to save, as a string</param>
        /// <param name="delimiter">The delimiter to use</param>
        static string QuoteCellForSaving(string cell, string delimiter)
        {
            string toret = cell ?? "";

            // Take into account delimiters...
            var delimitersAndSpace = new HashSet<char>( Delimiter.PredefinedDelimiters )
            {
                // ...and spaces...
                ' ',
                // ...and the current delimiter of the document (could be repeated)
                delimiter[ 0 ]
            };

            foreach (char ch in cell) {
                if ( delimitersAndSpace.Contains( ch ) ) {
                    // Quoting needed
                    toret = CsvDocument.Quote + toret + CsvDocument.Quote;
                    break;
                }
            }

            return toret;
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>The CsvDocument.</value>
        public CsvDocument Document {
            get; private set;
        }
    }
}
