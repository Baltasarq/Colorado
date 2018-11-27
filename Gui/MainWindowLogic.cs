// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>
/*
 * Colorado, a csv-based spreadsheet
 */

namespace Colorado.Gui {
	using System;
	using Colorado.Core;
	using System.Collections.Generic;
	
	using GtkUtil;
    
    public partial class MainWindow: Gtk.Window {
        private const int MaxFileLengthForTitle = 40;
        private const string LongFilePrefix = "...";
        public const int NumFixedColumns = 1;
        public const int NumFixedRows = 1;

        /// <summary>Loads all the recent files into menu.</summary>
        private void LoadRecentFilesIntoMenu()
        {
            foreach(string fileName in this.cfg.RecentFiles) {
                this.AppendRecentFileToMenu( fileName );
            }

            return;
        }

        /// <summary>Appends a given recent file to the menu.</summary>
        /// <param name="fileName">The file name, as a string.</param>
        private void AppendRecentFileToMenu(string fileName)
        {
            string fileNameOnly = System.IO.Path.GetFileName( fileName );
            var miFile = new Gtk.MenuItem( fileNameOnly );

            this.cfg.RecentFiles = new string[]{ fileName };
            this.mRecent.Append( miFile );

            miFile.Activated += (o, evt) => {
                if ( this.OnCloseDocument() ) {
                    this.OpenDocument( fileName );
                }
            };

            miFile.Show();
            this.cfg.Save();
        }


        public CsvDocument Document {
            get { return this.document; }
        }

        protected void PrepareDocument(CsvDocument doc)
        {
            this.document = doc;
            this.Document.ClientUpdater += this.UpdateFromData;
        }   

        private void ActivateIde()
        {
            this.ActivateIde( true );
        }

        private void ActivateIde(bool active)
        {
            this.SetStatus();
            this.SetTitle();

            this.tvTable.Visible               = active;
            this.edFind.Sensitive              = active;

            this.sbStatus.Visible              = true;

            this.openAction.Sensitive          = true;
            this.newAction.Sensitive           = true;
            this.importAction.Sensitive        = true;
            this.quitAction.Sensitive          = true;
            this.aboutAction.Sensitive         = true;

            this.saveAction.Sensitive          = active;
            this.saveAsAction.Sensitive        = active;
            this.revertAction.Sensitive = active;
            this.exportAction.Sensitive       = active;
            this.closeAction.Sensitive         = active;
            this.propertiesAction.Sensitive    = active;

            this.addRowsAction.Sensitive       = active;
            this.addColumnsAction.Sensitive    = active;
            this.removeRowsAction.Sensitive    = active;
            this.removeColumnsAction.Sensitive = active;
            this.clearRowsAction.Sensitive      = active;
            this.clearColumnsAction.Sensitive   = active;
            this.insertFormulaAction.Sensitive = active;
            this.copyColumnAction.Sensitive    = active;
            this.copyRowAction.Sensitive       = active;
            this.fillRowAction.Sensitive       = active;
            this.fillColumnAction.Sensitive    = active;

            this.findAction.Sensitive          = active;
            this.findAgainAction.Sensitive     = active;

            this.ShowProjectInfo();
			this.SetCurrentCell( 0, 0, false );
            Util.UpdateUI();
        }

        protected void ShowDocument()
        {
            int row = 0;

            if ( this.document != null ) {
                this.GetCurrentCell( out row, out int column );
            }

            this.ShowDocument( row );
        }

        protected void ShowDocument(int numRow)
        {
            if ( this.document == null ) {
                Util.MsgError( this, AppInfo.Name, "Document does not exist" );
                return;
            }

            this.ShowProjectInfo();
            this.tvTable.Hide();

            try {
                // Create liststore
                var types = new System.Type[ document.Data.NumColumns + 1 ];
                for(int typeNumber = 0; typeNumber < types.Length; ++typeNumber) {
                    types[ typeNumber ] = typeof( string );
                }
                Gtk.ListStore listStore = new Gtk.ListStore( types );
                tvTable.Model = listStore;

                // Delete existing columns
                while(tvTable.Columns.Length > 0) {
                    tvTable.RemoveColumn( tvTable.Columns[ 0 ] );
                }

                // Create index column
                var column = new Gtk.TreeViewColumn();
                var cell = new Gtk.CellRendererText();
                column.Title = "#";
                column.PackStart( cell, true );
                cell.Editable = false;
                cell.Foreground = "black";
                cell.Background = "light gray";
                column.AddAttribute( cell, "text", 0 );
                tvTable.AppendColumn( column );

                // Create columns belonging to the document
                for(int colNum = 0; colNum < document.Data.NumColumns; ++colNum) {
                    column = new Gtk.TreeViewColumn { Expand = true };
                    cell = new Gtk.CellRendererText();
                    column.Title = document.Data.ColumnInfo[ colNum ].Header;
                    column.PackStart( cell, true );
                    cell.Editable = true;
                    column.AddAttribute( cell, "text", colNum + 1 );
                    cell.Edited += (o, args) => OnTableCellEdited( args );

                    tvTable.AppendColumn( column );
                }

                // Insert data
                var row = new List<string>();
                for (int i = 0; i < document.Data.NumRows; ++i) {
                    row.Clear();
                    row.AddRange( document.Data[ i ] );
                    row.Insert( 0, Convert.ToString( i + 1 ) );

                    listStore.AppendValues( row.ToArray() );
                }
            } catch(Exception e) {
                Util.MsgError( this, AppInfo.Name, "Error building view: '" + e.Message + '\'' );
            }

            this.tvTable.EnableGridLines = Gtk.TreeViewGridLines.Both;
            this.tvTable.HeadersClickable = true;
            this.tvTable.SetCursor(
                new Gtk.TreePath( new int[]{ numRow } ),
                tvTable.Columns[ 1 ],
                false
            );
            this.tvTable.Show();
        }

        protected void OnAbout()
        {
            var about = new Gtk.AboutDialog();
            String[] authors = { AppInfo.Author };

            about.ProgramName = AppInfo.Name;
            about.Version     = AppInfo.Version;
            about.Authors     = authors;
            about.Comments    = AppInfo.Comments;
            about.License     = AppInfo.License;
            about.Copyright   = "(c) " + authors[ 0 ];
            about.Website     = AppInfo.Website;

            about.Logo = this.Icon;

            about.Parent = this;
            about.TransientFor = this;
            about.SetPosition( Gtk.WindowPosition.CenterOnParent );
            about.Run();
            about.Destroy();
        }

        private void OnFind()
        {
            if ( this.document == null ) {
                Util.MsgError( this, AppInfo.Name, "Document does not exist" );
                return;
            }

            // Get text to search
            this.edFind.Text = "";
            this.edFind.GrabFocus();
        }

        /// <summary>
        /// Opens a document in which the delimiter is inferred, and
        /// the first row is used for headers
        /// </summary>
        /// <param name="fileName">
        /// A <see cref="System.String"/>
        /// </param>
        protected void OpenDocument(string fileName)
        {
            this.OpenDocument( fileName, '\0', true );
        }

        /// <summary>
        /// Opens a document, using its fileName, delimiter and a bool to
        /// decide whether the first row is for headers or not
        /// </summary>
        /// <param name="fn">
        /// A <see cref="System.String"/> holding the file name
        /// </param>
        /// <param name="delim">
        /// A <see cref="System.Char"/> whihc is going to be the delimiter. A '\0'
        /// says that the delimiter must be inferred
        /// </param>
        /// <param name="useHeaders">
        /// A <see cref="System.Boolean"/> saying whether the first row is for headers or not.
        /// </param>
        protected void OpenDocument(string fn, char delim, bool useHeaders)
        {   
            this.ActivateIde( false );
            this.SetStatus( "Loading..." );

            // Load document
            try {
                var loader = new CsvDocumentPersistence();

                if ( delim == '\0' ) {
                    loader.Load( fn, firstRowForHeaders: useHeaders );
                } else {
                    loader.Load( fn, delim, useHeaders );
                }

                this.PrepareDocument( loader.Document );

                // Show it
                this.lastFileName = fn;
                this.ShowDocument();
                this.ShowProjectInfo();
                this.ActivateIde();
                this.AppendRecentFileToMenu( fn );
            } catch(Exception e) {
                Util.MsgError(
                            this, AppInfo.Name,
                            "Error while loading file: '" + e.Message + '\'' );
                this.document = null;
                this.SetStatus();
            }

            return;
        }

        private bool OnCloseDocument()
        {
			bool toret = true;

			if ( this.document != null ) {
				if ( Util.Ask( this, AppInfo.Name,
                               "Close spreadsheet '" + document.FileName + "' ?" ) )
                {
					this.CloseDocument();
				} else {
					toret = false;
				}
            }

			return toret;
        }

		private void CloseDocument() {
			if ( this.document.Changed ) {
				// Save the document, if needed
				if ( Util.Ask( this, AppInfo.Name, "Save spreadsheet '" + document.FileName + "' ?" ) ) {
					this.OnSave();
				}
			}

			this.document = null;
			this.ActivateIde( false );
		}

        private void OnOpen()
        {
			if ( this.OnCloseDocument() ) {
	            if ( Util.DlgOpen(
                    AppInfo.Name,
					"Open spreadsheet",
	                this,
	                ref this.lastFileName,
	                CsvDocumentPersistence.FileFilter[ 0 ] ) )
	            {
	                this.OpenDocument( this.lastFileName, '\0', true );
	            }
			}

            return;
        }

        public void SetStatus()
        {
            this.sbStatus.Pop( 1 );
            this.sbStatus.Push( 1, "Ready" );
            Util.UpdateUI();
        }

        public void SetStatus(string msg)
        {
            this.sbStatus.Pop( 1 );
            this.sbStatus.Push( 1, msg );
            Util.UpdateUI();
        }

        public void SetTitle()
        {
            string titleFile;

            // Prepare file title
            if ( this.document != null ) {
                titleFile = this.document.FileName;

                if ( titleFile.Length > ( MaxFileLengthForTitle + LongFilePrefix.Length ) )
                {
                    titleFile = titleFile.Remove( 0,
                        titleFile.Length
                        - MaxFileLengthForTitle - LongFilePrefix.Length
                    );
                    titleFile = LongFilePrefix + titleFile;
                }

                this.Title = titleFile + " - " + AppInfo.Name;
            }
            else {
                this.Title = AppInfo.Name;
            }
        }

        protected void ShowProjectInfo()
        {
            if ( this.document != null ) {
                string delimiter = Delimiter.GetName( this.Document.DelimiterValue );
                string text = "field";
                string number = "4";

                if ( this.document.SurroundText ) {
                    text = "\"field\"";
                }

                number += DecimalMark.AsChar( this.Document.DecimalSeparator ) + "5";

                this.lblType.Text = '(' + text + delimiter + number + delimiter + "...)";
                this.lblCount.Text = "["
                    + this.Document.Data.NumRows
                    + " x "
                    + this.Document.Data.NumColumns
                    + "]";
            } else {
                this.lblType.Text = this.lblCount.Text = "...";
            }

            return;
        }

		private bool OnQuit()
        {
			bool toret = true;

			if ( this.OnCloseDocument() ) {
				this.Visible = false;
				Gtk.Application.Quit();
				toret = false;
			}

			return toret;
        }

        private void OnImport()
        {
            var dlg = new DlgImport( this );

            if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
                this.OpenDocument( dlg.FileName, dlg.Delimiter, dlg.FirstRowForHeaders );
            }

            this.lastFileName = dlg.FileName;
            dlg.Destroy();
        }

        /// <summary>
        /// Sets the current cell to the position marked in rowIndex, colIndex
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="edit">Start editing the cell if set to <c>true</c>.</param>
        public void SetCurrentCell(int rowIndex, int colIndex, bool edit = false)
        {
			if ( this.document != null ) {
				var rowPath = new Gtk.TreePath( new int[]{ rowIndex } );
				Gtk.TreeViewColumn colPath = this.tvTable.Columns[ colIndex ];

				this.tvTable.ScrollToCell(
					rowPath,
					colPath,
					false,
					(float) 0.0,
					(float) 0.0
				);

				this.tvTable.SetCursor( rowPath, colPath, edit );
			}

			this.tvTable.GrabFocus();
			return;
        }

        public void GetCurrentCell(out int row, out int col)
        {
            // Convert path in row and rowPointer
            this.tvTable.GetCursor( out Gtk.TreePath rowPath,
                                    out Gtk.TreeViewColumn colPath );

            if ( rowPath != null
              && colPath != null )
            {
                this.tvTable.Model.GetIter( out Gtk.TreeIter rowPointer, rowPath );
                row = rowPath.Indices[ 0 ];

                // Find out the column order
                for(col = 0; col < this.tvTable.Columns.Length; ++col) {
                    if ( tvTable.Columns[ col ] == colPath ) {
                        break;
                    }
                }

                // Adapt column from UI
                --col;
                if ( col < 0 ) {
                    col = 0;
                }
            } else {
                row = 0;
                col = 1;
            }

            return;
        }

        private void OnTableCellEdited(Gtk.EditedArgs args)
        {
            int rowIndex;
            int colIndex;

            try {
                // Get current position
                Gtk.TreePath rowPath = new Gtk.TreePath( args.Path );

                // Convert path in row and rowPointer
                tvTable.Model.GetIter( out Gtk.TreeIter rowPointer, rowPath );
                rowIndex = rowPath.Indices[ 0 ];

                // Find out the column order
                tvTable.GetCursor( out rowPath, out Gtk.TreeViewColumn colPath );
                for(colIndex = 0; colIndex < tvTable.Columns.Length; ++colIndex)
                {
                    if ( tvTable.Columns[ colIndex ] == colPath ) {
                        break;
                    }
                }

                // Store data
                try {
                    this.document.Data[ rowIndex, colIndex - NumFixedColumns ] = args.NewText;
                    this.tvTable.Model.SetValue( rowPointer, colIndex, args.NewText );

                    if ( args.NewText == "" ) {
                        this.Document.FormulaManager.RemoveFormula( rowIndex, colIndex - NumFixedColumns );
                    }

                    this.Document.Changed = true;
                } catch(Exception exc) {
                    Util.MsgError( this, AppInfo.Name, "Passing coordinates to data:\n"
                        + "Rows: " + document.Data.NumRows + "\n"
                        + "Columns: " + document.Data.NumColumns + "\n"
                        + exc.Message
                    );
                }
            } catch(Exception exc)
            {
                Util.MsgError( this, AppInfo.Name, exc.Message );
            }
        }

        private void OnSave()
        {
            try {
                if ( this.document != null ) {
                    if ( this.document.Changed ) {
                        if ( this.document.HasName ) {
                            this.SetStatus( "Saving..." );
                            new CsvDocumentPersistence( Document ).SaveCsvData();
                            this.SetStatus();
                        } else {
                            this.OnSaveAs();
                        }

                        this.SetTitle();
                    }
                } else Util.MsgError( this, AppInfo.Name, "No document loaded" );
            } catch(Exception exc) {
                Util.MsgError( this, AppInfo.Name, exc.Message );
                this.SetStatus();
            }

            return;
        }

        private void OnSaveAs()
        {
            try {
                if ( this.document != null ) {
                    if ( Util.DlgOpen(
                        AppInfo.Name, "Save spreadsheet as...",
                        this,
                        ref this.lastFileName,
                        CsvDocumentPersistence.FileFilter[ 0 ] ) )
                    {
                        this.SetStatus( "Saving..." );
                        this.document.FileName = this.lastFileName;
                        this.lastFileName = this.document.FileName;  // CSVDoc fixed filename
                        new CsvDocumentPersistence( Document ).Save(
                            new ExportOptions( this.lastFileName, this.document ) );
                        this.SetTitle();
                        this.SetStatus();
                    }
                } else {
                    Util.MsgError( this, AppInfo.Name, "No document loaded" );
                    this.SetStatus();
                }
            } catch(Exception exc) {
                Util.MsgError( this, AppInfo.Name, exc.Message );
            }       
        }

        private void OnExport()
        {
            ExportOptions options = null;

            if ( this.document != null ) {
                DlgExport dlg = new DlgExport( this, this.document );

                if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
                    try {
                        string fn = ( (DlgExport) dlg ).FileName;

                        if ( fn.Trim().Length > 0 ) {
                            this.lastFileName = fn;
                            
                            options = new ExportOptions( fn, this.document )
                            {
                                Format = dlg.Selection,
                                IncludeRowNumbers = dlg.IncludeRowNumbers,
                                IncludeTableBorder = dlg.IncludeTableBorder,
                                ColumnsIncluded = dlg.ColumnsIncluded,
                                QuotedText = dlg.SurroundWithDoubleQuotes
                            };
                            
                            options.Delimiter.Name = dlg.DelimiterValue;

                            new CsvDocumentPersistence( Document ).Save( options );
                            Util.MsgInfo( this, AppInfo.Name, options.Format.ToString() + " file generated" );
                        }
                    } catch(Exception exc) {
                        Util.MsgError( this, AppInfo.Name, exc.Message );
                    }
                }

                dlg.Destroy();

            } else {
                Util.MsgError( this, AppInfo.Name, "No document loaded" );
            }

            return;
        }

        private void OnEdFindActivated()
        {
            this.txtToFind = this.edFind.Text;

            // Reset the state of the entry
            this.edFind.Text = "Find...";
            this.tvTable.GrabFocus();

            // Do it
            this.FindText( 0 );
        }

        protected void UpdateDocumentView(int oldRows, int oldColumns)
        {
            this.SetStatus( "Reconfiguring..." );

            if ( this.document.Changed ) {
                if ( this.document.Data.NumRows != oldRows
                  || this.document.Data.NumColumns != oldColumns )
                {
                    this.ShowDocument();
                } else {
                    // Update headers
                    for(int j = 0; j < this.document.Data.ColumnInfo.Length; ++j)
                    {
                        this.tvTable.Columns[ j + NumFixedColumns ].Title =
                            this.document.Data.ColumnInfo[ j ].Header;
                    }
                }

                this.ShowProjectInfo();
            }

            this.SetStatus();
        }

        /// <summary>
        /// Applies the preferences of the properties dialog.
        /// </summary>
        /// <param name="dlg">The properties dialog</param>
        private void ApplyChangedProperties(DlgProperties dlg)
        {
            if ( dlg.DecimalMarkValue != this.document.DecimalSeparator ) {
				this.document.DecimalSeparator = dlg.DecimalMarkValue;
                this.ShowDocument();
            }

            this.document.DelimiterValue = dlg.DelimiterValue;
            this.document.SurroundText = dlg.SurroundText;

            // Check rows and headers size
            if ( this.document.Data.NumColumns > dlg.NumColumns ) {
                if ( !Util.Ask( this, AppInfo.Name, "The new column value is lower. This will imply data loss. Are you sure?" ) ) {
                    dlg.NumColumns = this.document.Data.NumColumns;
                    dlg.NumRows = this.document.Data.NumRows;
                    goto Exit;
                }
            }

            if ( this.document.Data.NumRows > dlg.NumRows ) {
                if ( !Util.Ask( this, AppInfo.Name, "The new row value is lower. This will imply data loss. Are you sure?" ) ) {
                    dlg.NumColumns = this.document.Data.NumColumns;
                    dlg.NumRows = this.document.Data.NumRows;
                    goto Exit;
                }
            }

            // Now yes, modify the size
            this.document.Data.NumColumns = dlg.NumColumns;
            this.document.Data.NumRows = dlg.NumRows;

            // Modify headers, if needed
            if ( this.document.Data.FirstRowContainsHeaders != dlg.FirstRowForHeaders )
            {
                this.document.Data.FirstRowContainsHeaders = dlg.FirstRowForHeaders;
                dlg.NumRows = this.document.Data.NumRows;
            }

            Exit:
            dlg.UpdateColumnsData();
        }

        private void OnProperties()
        {
            if ( this.document != null ) {
                Gtk.ResponseType answer;
                var dlg = new DlgProperties( this, this.document );
                var oldRows = this.document.Data.NumRows;
                var oldColumns = this.document.Data.NumColumns;

                do {
                    answer = (Gtk.ResponseType) dlg.Run();

                    if ( answer == Gtk.ResponseType.Apply ) {
                        this.ApplyChangedProperties( dlg );
                        this.UpdateDocumentView( oldRows, oldColumns );
                        Util.UpdateUI();
                        oldRows = this.document.Data.NumRows;
                        oldColumns = this.document.Data.NumColumns;
                    }
				} while( answer != Gtk.ResponseType.Close
					  && answer != Gtk.ResponseType.DeleteEvent );

                // Apply changes
                this.ApplyChangedProperties( dlg );
                this.UpdateDocumentView( oldRows, oldColumns );
                dlg.Destroy();
            } else {
                Util.MsgError(this, AppInfo.Name, "No document loaded");
            }

            return;
        }

        private void OnNew()
        {
			if ( this.OnCloseDocument() ) {
				// Create new document
				this.PrepareDocument( new CsvDocument( 10, 10 ) );

				// Trigger the properties dialog
				this.ShowDocument();
				this.SetTitle();
				this.OnProperties();

				// Show everything
				this.ShowDocument();
				this.ShowProjectInfo();
				this.ActivateIde();
			}

			return;
        }

        public void FindText(int rowBegin)
        {
            this.FindText( rowBegin, this.txtToFind );
        }

        public void FindText(int rowBegin, string txtToFind)
        {
            this.tvTable.GrabFocus();

            for(int i = rowBegin; i < this.document.Data.NumRows; ++i) {
                for(int j = 0; j < this.document.Data.NumColumns; ++j) {
                    var cell = this.document.Data[ i, j ].Trim().ToLower();

                    if ( cell.Contains( txtToFind ) ) {
                        int[] path = { i };
                        this.tvTable.SetCursor( new Gtk.TreePath( path ), tvTable.Columns[ j + 1 ], false );
                        goto End;
                    }
                }
            }

            End:
            return;
        }

        private void OnFindAgain()
        {
            if ( this.document != null ) {
                this.GetCurrentCell( out int row, out int col );
                this.FindText( row + 1 );
            } else {
                Util.MsgError( this, AppInfo.Name, "Document does not exist" );
            }

            return;
        }

        public void RefreshRows(int col, int begin, int end)
        {
            // Run over rows
            for(int i = begin; i <= end; ++i) {
                // Get an iterator for this row
                Gtk.TreePath rowPath = new Gtk.TreePath( Convert.ToString( i ) );
                this.tvTable.Model.GetIter( out Gtk.TreeIter rowPointer, rowPath );

                // Refresh row
                for(int j = col; j < document.Data.NumColumns; ++j) {
                    // do It for each cell
                    this.tvTable.Model.SetValue( rowPointer, j + 1,
                        Convert.ToString( this.document.Data[ i, j ] )
                    );
                }
            }

            return;
        }

        private void OnClearRows()
        {
            // Get position
            if ( document == null ) {
                Util.MsgError( this, AppInfo.Name, "Document does not exist" );
                return;
            }

            // Get current position and adapt it to UI
            GetCurrentCell( out int rowBegin, out int col );
            ++rowBegin;

            var dlg = new DlgFromTo( this,
                rowBegin, document.Data.NumRows,
                DlgFromTo.ActionType.Clean, DlgFromTo.ItemType.Rows );

            if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
                // Adapt from UI to document (headers)
                rowBegin = dlg.From - NumFixedRows;
                int rowEnd = dlg.To - NumFixedRows;

                try {
                    // do it
                    this.document.Data.CleanRows( col, rowBegin, rowEnd );
                    this.RefreshRows( col, rowBegin, rowEnd );
                } catch(Exception exc) {
                    Util.MsgError( this, AppInfo.Name, exc.Message );
                }
            }

            dlg.Destroy();
        }

        private void OnClearColumns()
        {
            // Chk
            if ( this.document == null ) {
                Util.MsgError( this, AppInfo.Name, "Document does not exist" );
                return;
            }

            // Get current position and adapt it to UI
            GetCurrentCell( out int row, out int colBegin );
            ++colBegin;

            var dlg = new DlgFromTo( this,
                colBegin, document.Data.NumColumns,
                DlgFromTo.ActionType.Clean, DlgFromTo.ItemType.Columns );

            if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
                // Adapt from UI to document
                colBegin = dlg.From - 1;
                int colEnd = dlg.To - 1;

                try {
                    // do it
                    this.document.Data.CleanColumns( row, colBegin, colEnd );
                    ShowDocument();
                } catch (System.Exception exc) {
                    Util.MsgError( this, AppInfo.Name, exc.Message );
                }
            }

            dlg.Destroy();
        }

        private void OnAddRows()
        {
            // Chk
            if ( this.document == null ) {
                Util.MsgError( this, AppInfo.Name, "Document does not exist" );
                return;
            }

            // Get current position
            this.GetCurrentCell( out int row, out int col );

            row += NumFixedRows;
            col += NumFixedColumns;

            var dlg = new DlgIncDec( this,
                DlgIncDec.DialogType.Insert,
                DlgIncDec.Target.Rows,
                row,
                int.MaxValue
            );

            if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
                try {
                    if ( dlg.From == this.Document.Data.NumRows
                        && dlg.Where == DlgIncDec.WherePosition.After )
                    {
                        // Add mode
                        this.document.Data.NumRows += dlg.Number;
                    } else {
                        int modifier = 0;

                        if ( dlg.From > 0
                          && dlg.Where == DlgIncDec.WherePosition.After )
                        {
                            modifier = -1;
                        }

                        this.document.Data.InsertRows( dlg.From - NumFixedRows - modifier, dlg.Number );
                    }

                    this.ShowDocument();
                } catch(Exception exc) {
                    Util.MsgError( this, AppInfo.Name, exc.Message );
                }
            }

            dlg.Destroy();
        }

        private void OnAddColumns()
        {
            // Chk
            if ( this.document == null ) {
                Util.MsgError( this, AppInfo.Name, "Document does not exist" );
                return;
            }

            // Get current position
            this.GetCurrentCell( out int row, out int col );

            var dlg = new DlgIncDec( this,
                DlgIncDec.DialogType.Insert,
                DlgIncDec.Target.Columns,
				col + 1, int.MaxValue
            );

            if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
                try {
                    // do it
                    if ( dlg.From == this.Document.Data.NumColumns
                        && dlg.Where == DlgIncDec.WherePosition.After )
                    {
                        this.document.Data.NumColumns += dlg.Number;
                    } else {
                        int modifier = 0;

                        if ( dlg.From > 0
                          && dlg.Where == DlgIncDec.WherePosition.After )
                        {
                            modifier = -1;
                        }

                        this.document.Data.InsertColumns( dlg.From - NumFixedColumns - modifier, dlg.Number );
                    }

                    this.ShowDocument();
                } catch(Exception exc) {
                    Util.MsgError( this, AppInfo.Name, exc.Message );
                }
            }

            dlg.Destroy();
        }

        private void OnRevert()
        {
            var oldDocument = this.document;

            // Chk
            if ( this.document != null ) {
                if ( Util.Ask( this, AppInfo.Name, "Revert to the file on disk. Are you sure?" ) )
                {
                    // Store the parameters and reload  
                    var fileName = document.FileName;
                    var firstRowForHeaders = document.Data.FirstRowContainsHeaders;
                    char delimiter = document.DelimiterValue[ 0 ];
                    this.document = null;

                    try {
                        OpenDocument( fileName, delimiter, firstRowForHeaders );

                        // Check result
                        if ( this.document == null ) {
                            this.document = oldDocument;
                        }
                    } catch(Exception exc)
                    {
                        this.document = oldDocument;
                        Util.MsgError( this, AppInfo.Name, exc.Message );
                    }
                }
            } else {
                Util.MsgError( this, AppInfo.Name, "Document does not exist" );
            }

            return;
        }

        private void OnRemoveRows()
        {
            // Chk
            if ( this.document == null ) {
                Util.MsgError( this, AppInfo.Name, "Document does not exist" );
                return;
            }

            // Get current position
            this.GetCurrentCell( out int row, out int col );

            var dlg = new DlgIncDec( this,
                DlgIncDec.DialogType.Erase,
                DlgIncDec.Target.Rows,
                row + 1,
                this.document.Data.NumRows
            );

            if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
                try {
                    // do it
                    this.SetStatus( "Removing columns" );
                    this.document.Data.RemoveRows( dlg.From - NumFixedRows, dlg.Number );
                    this.ShowDocument();
                    this.SetStatus();
                } catch(Exception exc) {
                    Util.MsgError( this, AppInfo.Name, exc.Message );
                }
            }

            dlg.Destroy();
        }

        private void OnRemoveColumns()
        {
            // Chk
            if ( this.document == null ) {
                Util.MsgError( this, AppInfo.Name, "Document does not exist" );
                return;
            }

            // Get current position
            this.GetCurrentCell( out int row, out int col );

            var dlg = new DlgIncDec( this,
                DlgIncDec.DialogType.Erase,
                DlgIncDec.Target.Columns,
                col + 1,
                this.document.Data.NumColumns
            );

            if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
                try {
                    // do it
                    this.SetStatus( "Removing columns" );
                    this.document.Data.RemoveColumns( dlg.From - NumFixedColumns, dlg.Number );
                    this.ShowDocument();
                    this.SetStatus();
                } catch(Exception exc) {
                    Util.MsgError( this, AppInfo.Name, exc.Message );
                }
            }

            dlg.Destroy();
        }

        private void OnCopyRow()
        {
            // Chk
            if ( this.document != null ) {

                // Get current position
                this.GetCurrentCell( out int row, out int col );

                var dlg = new DlgFromTo( this,
                    row +1, document.Data.NumRows,
                    DlgFromTo.ActionType.Copy, DlgFromTo.ItemType.Rows );

                if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
                    try {
                        // do it
                        this.SetStatus( "Copying row" );
                        this.document.Data.CopyRow( dlg.From - NumFixedRows, dlg.To - NumFixedRows );
                        this.ShowDocument();
                        this.SetStatus();
                    } catch(Exception exc) {
                        Util.MsgError( this, AppInfo.Name, exc.Message );
                    }
                }

                dlg.Destroy();

            } else {
                Util.MsgError( this, AppInfo.Name, "Document does not exist" );
            }

            return;
        }

        private void OnCopyColumn()
        {
            // Chk
            if ( this.document == null ) {
                Util.MsgError( this, AppInfo.Name, "Document does not exist" );
                return;
            }

            // Get current position
            this.GetCurrentCell( out int row, out int col );

            var dlg = new DlgFromTo( this,
                col + 1,
                this.document.Data.NumColumns,
                DlgFromTo.ActionType.Copy,
                DlgFromTo.ItemType.Columns );

            if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
                try {
                    // do it
                    this.SetStatus( "Copying column" );
                    this.document.Data.CopyColumn( dlg.From - NumFixedColumns, dlg.To - NumFixedColumns );
                    this.ShowDocument();
                    this.SetStatus();
                } catch(Exception exc) {
                    Util.MsgError( this, AppInfo.Name, exc.Message );
                }
            }

            dlg.Destroy();
        }

        private void OnFillRow()
        {
            // Retrieve position
            this.GetCurrentCell( out int row, out int column );

            // Ask for filling
            var dlg = new DlgFill( this ) { Modal = true };

            if ( (Gtk.ResponseType) dlg.Run() == Gtk.ResponseType.Ok ) {
                var filler = Filler.CreateFiller(
                    this.Document,
                    new Position( this.Document, row, column ),
                    -1,
                    dlg.FillValue,
                    dlg.KindOfFill,
                    Filler.FillDirection.Row );

                while ( filler.NumCells > 0 ) {
                    filler.DoIt();
                    Util.UpdateUI();
                }

                this.ShowDocument( row );
            }

            dlg.Destroy();
        }

        private void OnFillColumn()
        {
            // Retrieve position
            this.GetCurrentCell( out int row, out int column );

            // Ask for filling
            var dlg = new DlgFill( this );

            if ( (Gtk.ResponseType) dlg.Run() == Gtk.ResponseType.Ok ) {
                var filler = Filler.CreateFiller(
                    this.Document,
                    new Position( this.Document, row, column ),
                    -1,
                    dlg.FillValue,
                    dlg.KindOfFill,
                    Filler.FillDirection.Column );
                
                while ( filler.NumCells > 0 ) {
                    filler.DoIt();
                    Util.UpdateUI();
                }

                this.ShowDocument( row );
            }

            dlg.Destroy();
        }

        private void OnTableClicked(Gtk.ButtonReleaseEventArgs args)
        {
            if ( args.Event.Button == 3 ) {
                this.popup.Popup();
            }

            return;
        }

        [GLib.ConnectBefore]
        private void OnTableKeyPressed(Gtk.KeyPressEventArgs args)
        {
            // Do not "eat" the key, by default
            args.RetVal = false;

            // Get the current position, needed in both cases.
            this.GetCurrentCell( out int rowIndex, out int colIndex );

			// Adapt the column
            colIndex += NumFixedColumns;

            if ( args.Event.Key != Gdk.Key.ISO_Enter ) {
                if ( args.Event.Key == Gdk.Key.Tab
                  || args.Event.Key == Gdk.Key.ISO_Left_Tab )
                {
                   if( args.Event.State == Gdk.ModifierType.ShiftMask ) {
                        // Back
                        colIndex -= 1;
                        if ( colIndex < 1 ) {
                            colIndex = document.Data.NumColumns;
                            --rowIndex;
                        }

                        rowIndex = Math.Max( 0, rowIndex );
                    } else {
                        // Advance
                        colIndex += 1;
                        if ( colIndex > document.Data.NumColumns ) {
                            colIndex = 1;
                            ++rowIndex;
                        }

                        rowIndex = Math.Min( rowIndex, document.Data.NumRows );
                    }

                    this.SetCurrentCell( rowIndex, colIndex );
                    args.RetVal = true;                              // Eat the TAB
                }
            }

            return;
        }

        /// <summary>
        /// Updates the view when the document's changed,
        /// by formulas or other means, not the user.
        /// </summary>
        /// <param name='row'>
        /// The row in which to set the value.
        /// </param>
        /// <param name='col'>
        /// The column in which to set the value.
        /// </param>
        /// <param name='value'>
        /// The value to put in the view.
        /// </param>
        protected void UpdateFromData(int row, int col, string value)
        {
            this.Set( row, col + NumFixedColumns, value );
        }

        /// <summary>
        /// Set the contents of the tvTable
        /// </summary>
        /// <param name="row">
        /// A <see cref="System.Int32"/> with the row number of the cell to set
        /// </param>
        /// <param name="col"> with the column number of the cell to set
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="value">
        /// A <see cref="System.String"/> with the value of the cell to set
        /// </param>
        protected void Set(int row, int col, string value)
        {
            var table = (Gtk.ListStore) this.tvTable.Model;

            // Chk
            if( row < 0
             || row >= this.Document.Data.NumRows )
            {
                throw new ArgumentException(
                                    "invalid row to set: " + row.ToString(),
                                    nameof( row ) );
            }

            if( col < 0
             || col >= ( this.Document.Data.NumColumns + NumFixedColumns ) )
            {
                throw new ArgumentException(
                                    "invalid column to set: " + col.ToString(),
                                    nameof( col ) );
            }

            // Find place
            table.GetIter( out Gtk.TreeIter itRow,
                           new Gtk.TreePath( new []{ row } ) );

            // Set
            table.SetValue( itRow, col, value );
        }

        /// <summary>
        /// Get the contents of the tvTable
        /// </summary>
        /// <param name="row">
        /// A <see cref="System.Int32"/> with the row number of the cell to set
        /// </param>
        /// <param name="col"> with the column number of the cell to set
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <return>
        /// A <see cref="System.String"/> with the value of the cell
        /// </return>
        protected string Get(int row, int col)
        {
            var table = (Gtk.ListStore) this.tvTable.Model;

            // Chk
            if( row < 0
             || row >= this.Document.Data.NumRows )
            {
                throw new ArgumentException(
                                    "invalid row to set: " + row.ToString(),
                                    nameof( row ) );
            }

            if( col < 0
             || col >= ( this.Document.Data.NumColumns + NumFixedColumns ) )
            {
                throw new ArgumentException(
                                    "invalid column to set: " + col.ToString(),
                                    nameof( col ) );
            }

            // Find place
            table.GetIter( out Gtk.TreeIter itRow,
                           new Gtk.TreePath( new int[] { row } ) );

            // Get
            return ((Gtk.CellRendererText) table.GetValue( itRow, col ) ).Text;
        }

        private void OnInsertFormula()
        {
            var dlg = new DlgFormulae( this );

            // Get current position
            this.GetCurrentCell( out int row, out int col );

            // Fire dialog
            if ( (Gtk.ResponseType) dlg.Run() == Gtk.ResponseType.Ok ) {
                // Get data from dialog
                Formula f = Formula.GetFormula( dlg.Formula );
                f.Document = this.Document;
                f.Direction = dlg.Direction;
                f.Position = new Position( this.Document, row, col );

                // Add formula to document
                this.Document.FormulaManager.AddFormula( f );
            }

            dlg.Destroy();
        }

        private CsvDocument document;
        private string txtToFind;
        public string lastFileName;
        private Core.Cfg.Config cfg;
    }

}
