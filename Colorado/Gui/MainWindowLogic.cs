// Colorado (c) 2015-2025 Baltasar MIT License <baltasarq@gmail.com>
/*
 * Colorado, a tsv/csv-based spreadsheet
 */


namespace Colorado.Gui;


using System;
using System.Collections.Generic;
using Core;


public partial class MainWindow: Gtk.ApplicationWindow {
    public const int NumFixedColumns = 1;
    public const int NumFixedRows = 1;

    const int MaxFileLengthForTitle = 40;
    const string LongFilePrefix = "...";
    const string NoNamedFile = "nonamed.csv";

    public MainWindow(Gtk.Application app, string fileName = "")
        : base( app )
    {
        this.Title = AppInfo.Name;
        this.lastFileName = "";
        this.txtToFind = "";
        this.edFind = new Gtk.Entry( "Find..." );
        this.tvTable = this.BuildTable();
        this.popup = new Gtk.Menu();
        this.menuBar = new Gtk.MenuBar();
        this.tbTools = new Gtk.Toolbar { ToolbarStyle = this.ToolbarMode };
        this.mRecent = new Gtk.Menu();
        this.lblCount = new Gtk.Label( "..." );
        this.lblType = new Gtk.Label( "..." );
        this.sbStatus = new Gtk.Statusbar();

        this.newAction = new ( "new", "_New", "new spreadsheet", this.iconNew );
        this.openAction = new ( "open", "_Open", "open spreadhseet", this.iconOpen );
        this.saveAction = new ( "save", "_Save", "save spreadhseet", this.iconSave );
        this.saveAsAction = new ( "save_as", "Save _as...", "save spreadhseet as...", this.iconSave );
        this.propertiesAction = new ( "properties", "_Properties", "properties", this.iconProperties );
        this.closeAction = new ( "close", "_Close", "close spreadhseet", this.iconClose );
        this.aboutAction = new ( "about", "_About", "about...", this.iconAbout );
        this.importAction = new ( "import", "_Import", "import data", this.iconImport );
        this.exportAction = new ( "export", "_Export", "export to...", this.iconExport );
        this.revertAction = new ( "revert", "_Revert", "revert to file", this.iconRevert );
        this.quitAction = new ( "quit", "_Quit", "quit", this.iconExit );
        this.findAction = new ( "find", "_Find", "find...", this.iconFind );
        this.findAgainAction = new ( "find_again", "_Find again", "find again", this.iconFind );
        this.insertFormulaAction = new ( "insert_formula", "_Insert formula", "insert formula", this.iconFormula );
        this.addRowsAction = new ( "add_rows", "_Add rows", "add rows", this.iconAdd );
        this.removeRowsAction = new ( "remove_rows", "_Remove rows", "remove rows", this.iconRemove );
        this.clearRowsAction = new ( "clear_rows", "_Clear rows", "clear rows", this.iconClear );
        this.copyRowAction = new ( "copy_row", "_Copy row", "copy row", this.iconCopy );
        this.sortRowsAction = new ( "sort_rows", "_Sort", "short rows", this.iconSort );
        this.fillRowAction = new ( "fill_row", "_Fill row", "fill row", this.iconPaste );
        this.addColumnsAction = new ( "add_columns", "_Add columns", "add columns", this.iconAdd );
        this.removeColumnsAction = new ( "remove_columns", "_Remove columns", "remove columns", this.iconRemove );
        this.clearColumnsAction = new ( "clear_Columns", "_Clear columns", "clear columns", this.iconRemove );
        this.copyColumnAction = new ( "copy_column", "_Copy column", "copy column", this.iconCopy );
        this.fillColumnAction = new ( "fill_column", "_Fill column", "fill column", this.iconPaste );

        this.Build();

        this.Document = null;
        this.cfg = Core.Cfg.Config.Load();
        this.LoadRecentFilesIntoMenu();
        this.ActivateIde( false );

        if ( !string.IsNullOrWhiteSpace( fileName ) ) {
            this.OpenDocument( fileName );
        }
    }

    /// <summary>Loads all the recent files into menu.</summary>
    void LoadRecentFilesIntoMenu()
    {
        this.cfg.RecentFiles.ToList<string>().ForEach( this.AppendRecentFileToMenu );
    }

    /// <summary>Appends a given recent file to the menu.</summary>
    /// <param name="fileName">The file name, as a string.</param>
    void AppendRecentFileToMenu(string fileName)
    {
        string fileNameOnly = System.IO.Path.GetFileName( fileName );
        var miFile = new Gtk.MenuItem( fileNameOnly );

        this.cfg.RecentFiles = [ fileName ];
        this.mRecent.Append( miFile );

        miFile.Activated += (o, evt) => {
            if ( this.OnCloseDocument() ) {
                this.OpenDocument( fileName );
            }
        };

        miFile.Show();
        this.cfg.Save();
    }

    void PrepareForDocument(CsvDocument? doc)
    {
        this.Document = doc;

        if ( this.Document is not null ) {
            this.Document.ClientUpdater += this.UpdateFromData;
            this.LastFileName = this.Document.FileName;
            this.AppendRecentFileToMenu( this.Document.FileName );
            this.ShowDocument();
            this.ShowProjectInfo();
        }

        this.ActivateIde( doc != null );
    }

    void ActivateIde()
    {
        this.ActivateIde( true );
    }

    void ActivateIde(bool active)
    {
        this.SetStatus();
        this.SetTitle();

        this.tvTable.Visible               = active;
        this.edFind.Sensitive              = active;

        this.sbStatus.Visible              = true;

        this.openAction.IsEnabled            = true;
        this.newAction.IsEnabled           = true;
        this.importAction.IsEnabled        = true;
        this.quitAction.IsEnabled          = true;
        this.aboutAction.IsEnabled         = true;

        this.saveAction.IsEnabled          = active;
        this.saveAsAction.IsEnabled        = active;
        this.revertAction.IsEnabled        = active;
        this.exportAction.IsEnabled        = active;
        this.closeAction.IsEnabled         = active;
        this.propertiesAction.IsEnabled    = active;

        this.addRowsAction.IsEnabled       = active;
        this.addColumnsAction.IsEnabled    = active;
        this.removeRowsAction.IsEnabled    = active;
        this.removeColumnsAction.IsEnabled = active;
        this.clearRowsAction.IsEnabled     = active;
        this.clearColumnsAction.IsEnabled  = active;
        this.insertFormulaAction.IsEnabled = active;
        this.copyColumnAction.IsEnabled    = active;
        this.copyRowAction.IsEnabled       = active;
        this.fillRowAction.IsEnabled       = active;
        this.sortRowsAction.IsEnabled      = active;
        this.fillColumnAction.IsEnabled    = active;

        this.findAction.IsEnabled          = active;
        this.findAgainAction.IsEnabled     = active;

        this.ShowProjectInfo();
        this.SetCurrentCell( 0, 0, false );
        GtkUtil.Misc.UpdateUI();
    }

    protected void ShowDocument()
    {
        int row = 0;

        if ( this.Document != null ) {
            this.GetCurrentCell( out row, out int column );
        }

        this.ShowDocument( row );
    }

    protected void ShowDocument(int numRow)
    {
        if ( this.Document == null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
            return;
        }

        this.ShowProjectInfo();
        this.tvTable.Hide();

        try {
            // Create liststore
            var types = new Type[ this.Document.Data.NumColumns + 1 ];
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
            for(int colNum = 0; colNum < this.Document.Data.NumColumns; ++colNum)
            {
                column = new Gtk.TreeViewColumn { Expand = true };
                cell = new Gtk.CellRendererText();
                column.Title = this.Document.Data.ColumnInfo[ colNum ].Header;
                column.PackStart( cell, true );
                cell.Editable = true;
                column.AddAttribute( cell, "text", colNum + 1 );
                cell.Edited += (o, args) => OnTableCellEdited( args );

                tvTable.AppendColumn( column );
            }

            // Insert data
            var row = new List<string>();
            for (int i = 0; i < this.Document.Data.NumRows; ++i) {
                row.Clear();
                row.AddRange( this.Document.Data[ i ] );
                row.Insert( 0, Convert.ToString( i + 1 ) );

                listStore.AppendValues( row.ToArray() );
            }
        } catch(Exception e) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Error building view: '" + e.Message + '\'' );
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

    void OnFind()
    {
        if ( this.Document == null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
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

            loader.Load( fn, delim, firstRowForHeaders: useHeaders );
            this.PrepareForDocument( loader.Document );
        } catch(Exception e) {
            GtkUtil.Misc.MsgError(
                        this, AppInfo.Name,
                        "Error while loading file: '" + e.Message + '\'' );
            this.Document = null;
            this.SetStatus();
        }

        return;
    }

    bool OnCloseDocument()
    {
        bool toret = true;

        if ( this.Document != null ) {
            if ( GtkUtil.Misc.Ask( this, AppInfo.Name,
                            "Close spreadsheet '" + this.Document.FileName + "' ?" ) )
            {
                this.CloseDocument();
            } else {
                toret = false;
            }
        }

        return toret;
    }

    void CloseDocument()
    {
        if ( this.Document is not null
          && this.Document.Changed )
        {
            // Save the document, if needed
            if ( GtkUtil.Misc.Ask( this, AppInfo.Name,
                                    "Save spreadsheet '"
                                    + this.Document.FileName + "' ?" ) )
            {
                this.OnSave();
            }
        }

        this.Document = null;
        this.ActivateIde( false );
    }

    /// <summary>
    /// Opens a new document.
    /// </summary>
    void OnOpen()
    {
        if ( this.OnCloseDocument() ) {
            if ( string.IsNullOrWhiteSpace( this.LastFileName ) ) {
                this.LastFileName = ".";
            }

            if ( GtkUtil.Misc.DlgOpen( AppInfo.Name, "Open spreadsheet",
                                this,
                                ref this.lastFileName,
                                CsvDocumentPersistence.FileFilter[ 0 ] ) )
            {
                this.OpenDocument( this.LastFileName, '\0', true );
            }
        }

        return;
    }

    public void SetStatus()
    {
        this.sbStatus.Pop( 1 );
        this.sbStatus.Push( 1, "Ready" );
        GtkUtil.Misc.UpdateUI();
    }

    public void SetStatus(string msg)
    {
        this.sbStatus.Pop( 1 );
        this.sbStatus.Push( 1, msg );
        GtkUtil.Misc.UpdateUI();
    }

    public void SetTitle()
    {
        string titleFile;

        // Prepare file title
        if ( this.Document != null ) {
            titleFile = this.Document.FileName;

            if ( titleFile.Length > ( MaxFileLengthForTitle + LongFilePrefix.Length ) )
            {
                titleFile = titleFile.Remove( 0,
                    titleFile.Length
                    - MaxFileLengthForTitle - LongFilePrefix.Length
                );
                titleFile = LongFilePrefix + titleFile;
            }

            this.Title = titleFile + " - " + AppInfo.Name;
        } else {
            this.Title = AppInfo.Name;
        }
    }

    protected void ShowProjectInfo()
    {
        if ( this.Document != null ) {
            string delimiter = Delimiter.GetName( this.Document.DelimiterValue );
            string text = "field";
            string number = "4";

            if ( this.Document.SurroundText ) {
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

    bool OnQuit()
    {
        bool toret = true;

        if ( this.OnCloseDocument() ) {
            this.Visible = false;
            Gtk.Application.Quit();
            toret = false;
        }

        return toret;
    }

    void OnImport()
    {
        var dlg = new DlgImport( this );

        if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
            try {
                if ( this.OnCloseDocument() ) {
                    var options = dlg.Options;
                    var importer = Importer.GetImporter( options.ImportId );

                    importer.Options = options;
                    this.PrepareForDocument( importer.Load() );
                }
            } catch(Exception exc) {
                GtkUtil.Misc.MsgError( this, AppInfo.Name,
                                        "unable to import: " + exc.Message );
                this.PrepareForDocument( null );
            }
        }

        this.LastFileName = dlg.FileName;
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
        if ( this.Document != null ) {
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

    void OnTableCellEdited(Gtk.EditedArgs args)
    {
        if ( this.Document is null ) {
            goto Exit;
        }

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
                this.Document.Data[ rowIndex, colIndex - NumFixedColumns ] = args.NewText;
                this.tvTable.Model.SetValue( rowPointer, colIndex, args.NewText );

                if ( args.NewText == "" ) {
                    this.Document.FormulaManager.RemoveFormula( rowIndex, colIndex - NumFixedColumns );
                }

                this.Document.Changed = true;
            } catch(Exception exc) {
                GtkUtil.Misc.MsgError( this, AppInfo.Name, "Passing coordinates to data:\n"
                    + "Rows: " + this.Document.Data.NumRows + "\n"
                    + "Columns: " + this.Document.Data.NumColumns + "\n"
                    + exc.Message
                );
            }
        } catch(Exception exc)
        {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
        }

        Exit:
        return;
    }

    void OnSave()
    {
        try {
            if ( this.Document != null ) {
                if ( this.Document.Changed ) {
                    if ( this.Document.HasName ) {
                        this.SetStatus( "Saving..." );
                        new CsvDocumentPersistence( this.Document ).SaveCsvData();
                        this.SetStatus();
                    } else {
                        this.OnSaveAs();
                    }

                    this.SetTitle();
                }
            } else GtkUtil.Misc.MsgError( this, AppInfo.Name, "No document loaded" );
        } catch(Exception exc) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
            this.SetStatus();
        }

        return;
    }

    void OnSaveAs()
    {
        try {
            // Prepare file name
            if ( string.IsNullOrWhiteSpace( this.lastFileName ) ) {
                string docsPath = Environment.GetFolderPath(
                                    Environment.SpecialFolder.MyDocuments );

                this.lastFileName = System.IO.Path.Combine( docsPath, NoNamedFile );
            }

            // Ask for name
            if ( this.Document is not null ) {
                if ( GtkUtil.Misc.DlgSave(
                            AppInfo.Name, "Save spreadsheet as...",
                            this,
                            ref this.lastFileName,
                            CsvDocumentPersistence.FileFilter[ 0 ] ) )
                {
                    this.SetStatus( "Saving..." );
                    this.Document.FileName = this.lastFileName;
                    this.lastFileName = this.Document.FileName;  // CSVDoc fixed filename
                    new CsvDocumentPersistence( this.Document ).SaveCsvData();
                    this.SetTitle();
                    this.SetStatus();
                }
            } else {
                GtkUtil.Misc.MsgError( this, AppInfo.Name, "No document loaded" );
                this.SetStatus();
            }
        } catch(Exception exc) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
        }
    }

    void OnExport()
    {
        ExportOptions? options = null;

        if ( this.Document != null ) {
            DlgExport dlg = new DlgExport( this, this.Document );

            if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
                try {
                    string fn = ( (DlgExport) dlg ).FileName;

                    if ( fn.Trim().Length > 0 ) {
                        this.LastFileName = fn;

                        options = new ExportOptions( fn, this.Document ) {
                            ExporterId = dlg.ExporterId,
                            IncludeRowNumbers = dlg.IncludeRowNumbers,
                            IncludeTableBorder = dlg.IncludeTableBorder,
                            ColumnsIncluded = dlg.ColumnsIncluded,
                            QuotedText = dlg.SurroundWithDoubleQuotes
                        };

                        options.Delimiter.Name = dlg.DelimiterValue;

                        Exporter.Save( options );
                        GtkUtil.Misc.MsgInfo( this, AppInfo.Name,
                                        options.Exporter.FileExtension
                                        + " file generated" );
                    }
                } catch(Exception exc) {
                    GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
                }
            }

            dlg.Destroy();

        } else {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "No document loaded" );
        }

        return;
    }

    void OnEdFindActivated()
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

        if ( this.Document is not null
          && this.Document.Changed )
        {
            if ( this.Document.Data.NumRows != oldRows
              || this.Document.Data.NumColumns != oldColumns )
            {
                this.ShowDocument();
            } else {
                // Update headers
                for(int j = 0; j < this.Document.Data.ColumnInfo.Length; ++j)
                {
                    this.tvTable.Columns[ j + NumFixedColumns ].Title =
                        this.Document.Data.ColumnInfo[ j ].Header;
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
    void ApplyChangedProperties(DlgProperties dlg)
    {
        const string MsgDataLoss = "This will imply data loss. Are you sure?";
        const string ColDataLoss = "The new column value is lower. " + MsgDataLoss;
        const string RowDataLoss = "The new row value is lower. " + MsgDataLoss;

        if ( this.Document is null ) {
            goto Exit;
        }

        if ( dlg.DecimalMarkValue != this.Document.DecimalSeparator )
        {
            this.Document.DecimalSeparator = dlg.DecimalMarkValue;
            this.ShowDocument();
        }

        this.Document.Delimiter = dlg.Delimiter;
        this.Document.SurroundText = dlg.SurroundText;

        // Check rows and headers size
        if ( this.Document.Data.NumColumns > dlg.NumColumns ) {
            if ( !GtkUtil.Misc.Ask( this, AppInfo.Name, ColDataLoss ) ) {
                dlg.NumColumns = this.Document.Data.NumColumns;
                dlg.NumRows = this.Document.Data.NumRows;
                goto Exit;
            }
        }

        if ( this.Document.Data.NumRows > dlg.NumRows ) {
            if ( !GtkUtil.Misc.Ask( this, AppInfo.Name, RowDataLoss ) ) {
                dlg.NumColumns = this.Document.Data.NumColumns;
                dlg.NumRows = this.Document.Data.NumRows;
                goto Exit;
            }
        }

        // Now yes, modify the size
        this.Document.Data.NumColumns = dlg.NumColumns;
        this.Document.Data.NumRows = dlg.NumRows;

        // Modify headers, if needed
        if ( this.Document.Data.FirstRowContainsHeaders != dlg.FirstRowForHeaders )
        {
            this.Document.Data.FirstRowContainsHeaders = dlg.FirstRowForHeaders;
            dlg.NumRows = this.Document.Data.NumRows;
        }

        Exit:
        dlg.UpdateColumnsData();
    }

    void OnProperties()
    {
        if ( this.Document != null ) {
            Gtk.ResponseType answer;
            var dlg = new DlgProperties( this, this.Document );
            var oldRows = this.Document.Data.NumRows;
            var oldColumns = this.Document.Data.NumColumns;

            do {
                answer = (Gtk.ResponseType) dlg.Run();

                if ( answer == Gtk.ResponseType.Apply ) {
                    this.ApplyChangedProperties( dlg );
                    this.UpdateDocumentView( oldRows, oldColumns );
                    GtkUtil.Misc.UpdateUI();
                    oldRows = this.Document.Data.NumRows;
                    oldColumns = this.Document.Data.NumColumns;
                }
            } while( answer != Gtk.ResponseType.Close
                    && answer != Gtk.ResponseType.DeleteEvent );

            // Apply changes
            this.ApplyChangedProperties( dlg );
            this.UpdateDocumentView( oldRows, oldColumns );
            dlg.Destroy();
        } else {
            GtkUtil.Misc.MsgError(this, AppInfo.Name, "No document loaded");
        }

        return;
    }

    void OnNew()
    {
        if ( this.OnCloseDocument() ) {
            // Create new document
            this.PrepareForDocument( new CsvDocument( 10, 10 ) );

            // Trigger the properties dialog
            this.OnProperties();
            this.PrepareForDocument( this.Document );
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

        if ( this.Document is not null ) {
            for(int i = rowBegin; i < this.Document.Data.NumRows; ++i) {
                for(int j = 0; j < this.Document.Data.NumColumns; ++j) {
                    var cell = this.Document.Data[ i, j ].Trim().ToLower();

                    if ( cell.Contains( txtToFind ) ) {
                        int[] path = { i };
                        this.tvTable.SetCursor( new Gtk.TreePath( path ), tvTable.Columns[ j + 1 ], false );
                        goto End;
                    }
                }
            }
        }

        End:
        return;
    }

    void OnFindAgain()
    {
        if ( this.Document != null ) {
            this.GetCurrentCell( out int row, out int col );
            this.FindText( row + 1 );
        } else {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
        }

        return;
    }

    public void RefreshRows(int col, int begin, int end)
    {
        if ( this.Document is not null ) {
            // Run over rows
            for (int i = begin; i <= end; ++i) {
                // Get an iterator for this row
                Gtk.TreePath rowPath = new Gtk.TreePath( Convert.ToString( i ) );
                this.tvTable.Model.GetIter( out Gtk.TreeIter rowPointer, rowPath );

                // Refresh row
                for (int j = col; j < this.Document.Data.NumColumns; ++j) {
                    // do It for each cell
                    this.tvTable.Model.SetValue( rowPointer, j + 1,
                                    Convert.ToString(this.Document.Data[i, j])
                    );
                }
            }
        }

        return;
    }

    /// <summary>
    /// Determines the selected rows.
    /// </summary>
    /// <param name="first">To be filled with the first row.</param>
    /// <param name="last">To be filled with the last row.</param>
    void DetermineSelectedRows(out int first, out int last)
    {
        var orgList = (Gtk.ListStore) this.tvTable.Model;
        Gtk.TreeSelection selected = this.tvTable.Selection;
        Gtk.TreePath[] rowPaths = selected.GetSelectedRows();

        if ( rowPaths.Length > 0 ) {
            first = rowPaths[ 0 ].Indices[ 0 ] + NumFixedRows;
            last = rowPaths[ ^1 ].Indices[ 0 ] + NumFixedRows;
        } else {
            first = last = -1;
        }

        return;
    }

    void OnClearRows()
    {
        // Get position
        if ( this.Document == null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
            return;
        }

        // Get current selection
        this.DetermineSelectedRows( out int rowBegin, out int rowEnd );

        var dlg = new DlgFromTo( this,
            rowBegin, rowEnd, this.Document.Data.NumRows,
            DlgFromTo.ActionType.Clean, DlgFromTo.ItemType.Rows );

        if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
            // Adapt from UI to document (headers)
            rowBegin = dlg.From - NumFixedRows;
            rowEnd = dlg.To - NumFixedRows;

            try {
                // do it
                this.Document.Data.CleanRows( 0, rowBegin, rowEnd );
                this.RefreshRows( 0, rowBegin, rowEnd );
            } catch(Exception exc) {
                GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
            }
        }

        dlg.Destroy();
    }

    void OnClearColumns()
    {
        // Chk
        if ( this.Document == null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
            return;
        }

        // Get current position and adapt it to UI
        GetCurrentCell( out int row, out int colBegin );
        ++colBegin;

        var dlg = new DlgFromTo( this,
            colBegin, colBegin + 1, this.Document.Data.NumColumns,
            DlgFromTo.ActionType.Clean, DlgFromTo.ItemType.Columns );

        if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
            // Adapt from UI to document
            colBegin = dlg.From - 1;
            int colEnd = dlg.To - 1;

            try {
                // do it
                this.Document.Data.CleanColumns( row, colBegin, colEnd );
                ShowDocument();
            } catch (System.Exception exc) {
                GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
            }
        }

        dlg.Destroy();
    }

    void OnAddRows()
    {
        // Chk
        if ( this.Document == null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
            return;
        }

        // Get current selection
        this.GetCurrentCell( out int row, out int col );

        var dlg = new DlgIncDec( this,
            DlgIncDec.DialogType.Insert,
            DlgIncDec.Target.Rows,
            row + 1,
            1,
            int.MaxValue
        );

        if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
            try {
                if ( dlg.From == this.Document.Data.NumRows
                    && dlg.Where == DlgIncDec.WherePosition.After )
                {
                    // Add mode
                    this.Document.Data.NumRows += dlg.Number;
                } else {
                    int modifier = 0;

                    if ( dlg.From > 0
                        && dlg.Where == DlgIncDec.WherePosition.After )
                    {
                        modifier = -1;
                    }

                    this.Document.Data.InsertRows( dlg.From - NumFixedRows - modifier, dlg.Number );
                }

                this.ShowDocument();
            } catch(Exception exc) {
                GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
            }
        }

        dlg.Destroy();
    }

    void OnAddColumns()
    {
        // Chk
        if ( this.Document == null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
            return;
        }

        // Get current position
        this.GetCurrentCell( out int row, out int col );

        var dlg = new DlgIncDec( this,
            DlgIncDec.DialogType.Insert,
            DlgIncDec.Target.Columns,
            col + 1, col + 2, int.MaxValue
        );

        if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
            try {
                // do it
                if ( dlg.From == this.Document.Data.NumColumns
                    && dlg.Where == DlgIncDec.WherePosition.After )
                {
                    this.Document.Data.NumColumns += dlg.Number;
                } else {
                    int modifier = 0;

                    if ( dlg.From > 0
                        && dlg.Where == DlgIncDec.WherePosition.After )
                    {
                        modifier = -1;
                    }

                    this.Document.Data.InsertColumns( dlg.From - NumFixedColumns - modifier, dlg.Number );
                }

                this.ShowDocument();
            } catch(Exception exc) {
                GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
            }
        }

        dlg.Destroy();
    }

    void OnRevert()
    {
        var oldDocument = this.Document;

        // Chk
        if ( this.Document != null ) {
            if ( GtkUtil.Misc.Ask( this, AppInfo.Name, "Revert to the file on disk. Are you sure?" ) )
            {
                // Store the parameters and reload
                var fileName = this.Document.FileName;
                var firstRowForHeaders = this.Document.Data.FirstRowContainsHeaders;
                char delimiter = this.Document.DelimiterValue[ 0 ];
                this.Document = null;

                try {
                    OpenDocument( fileName, delimiter, firstRowForHeaders );

                    // Check result
                    if ( this.Document == null ) {
                        this.Document = oldDocument;
                    }
                } catch(Exception exc)
                {
                    this.Document = oldDocument;
                    GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
                }
            }
        } else {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
        }

        return;
    }

    void OnRemoveRows()
    {
        // Chk
        if ( this.Document == null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
            return;
        }

        // Get current selection
        this.DetermineSelectedRows( out int rowBegin, out int rowEnd );

        var dlg = new DlgIncDec( this,
            DlgIncDec.DialogType.Erase,
            DlgIncDec.Target.Rows,
            rowBegin, rowEnd,
            this.Document.Data.NumRows
        );

        if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
            try {
                // do it
                this.SetStatus( "Removing rows" );
                this.Document.Data.RemoveRows( dlg.From - NumFixedRows, dlg.Number );
                this.ShowDocument();
                this.SetStatus();
            } catch(Exception exc) {
                GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
            }
        }

        dlg.Destroy();
    }

    void OnRemoveColumns()
    {
        // Chk
        if ( this.Document == null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
            return;
        }

        // Get current position
        this.GetCurrentCell( out int row, out int col );

        var dlg = new DlgIncDec( this,
            DlgIncDec.DialogType.Erase,
            DlgIncDec.Target.Columns,
            col + 1, col + 2,
            this.Document.Data.NumColumns
        );

        if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
            try {
                // do it
                this.SetStatus( "Removing columns" );
                this.Document.Data.RemoveColumns( dlg.From - NumFixedColumns, dlg.Number );
                this.ShowDocument();
                this.SetStatus();
            } catch(Exception exc) {
                GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
            }
        }

        dlg.Destroy();
    }

    void OnCopyRow()
    {
        // Chk
        if ( this.Document != null ) {

            // Get current position
            this.DetermineSelectedRows( out int rowBegin, out int rowEnd );

            var dlg = new DlgFromTo( this,
                rowBegin, rowBegin + 1, this.Document.Data.NumRows,
                DlgFromTo.ActionType.Copy, DlgFromTo.ItemType.Rows );

            if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
                try {
                    // do it
                    this.SetStatus( "Copying row" );
                    this.Document.Data.CopyRow( dlg.From - NumFixedRows, dlg.To - NumFixedRows );
                    this.ShowDocument();
                    this.SetStatus();
                } catch(Exception exc) {
                    GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
                }
            }

            dlg.Destroy();

        } else {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
        }

        return;
    }

    void OnCopyColumn()
    {
        // Chk
        if ( this.Document == null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
            return;
        }

        // Get current position
        this.GetCurrentCell( out int row, out int col );

        var dlg = new DlgFromTo( this,
            col + 1, col + 2,
            this.Document.Data.NumColumns,
            DlgFromTo.ActionType.Copy,
            DlgFromTo.ItemType.Columns );

        if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
            try {
                // do it
                this.SetStatus( "Copying column" );
                this.Document.Data.CopyColumn( dlg.From - NumFixedColumns, dlg.To - NumFixedColumns );
                this.ShowDocument();
                this.SetStatus();
            } catch(Exception exc) {
                GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
            }
        }

        dlg.Destroy();
    }

    void OnFillRow()
    {
        if ( this.Document is not null ) {
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
                    GtkUtil.Misc.UpdateUI();
                }

                this.ShowDocument( row );
            }

            dlg.Destroy();
        }
    }

    void OnFillColumn()
    {
        if ( this.Document is not null ) {
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
                    GtkUtil.Misc.UpdateUI();
                }

                this.ShowDocument( row );
            }

            dlg.Destroy();
        }
    }

    void OnSortRows()
    {
    }

    void OnTableClicked(Gtk.ButtonReleaseEventArgs args)
    {
        if ( args.Event.Button == 3 ) {
            this.popup.Popup();
        }

        return;
    }

    [GLib.ConnectBefore]
    void OnTableKeyPressed(Gtk.KeyPressEventArgs args)
    {
        if ( this.Document is null ) {
            goto Exit;
        }

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
                        colIndex = this.Document.Data.NumColumns;
                        --rowIndex;
                    }

                    rowIndex = Math.Max( 0, rowIndex );
                } else {
                    // Advance
                    colIndex += 1;
                    if ( colIndex > this.Document.Data.NumColumns ) {
                        colIndex = 1;
                        ++rowIndex;
                    }

                    rowIndex = Math.Min( rowIndex, this.Document.Data.NumRows );
                }

                this.SetCurrentCell( rowIndex, colIndex );
                args.RetVal = true;                              // Eat the TAB
            }
        }

        Exit:
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
        if ( this.Document is not null ) {
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
        if ( this.Document is not null ) {
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

        return "";
    }

    void OnInsertFormula()
    {
        if ( this.Document is not null ) {
            var dlg = new DlgFormulae( this );

            // Get current position
            this.GetCurrentCell( out int row, out int col );

            // Fire dialog
            if ( (Gtk.ResponseType) dlg.Run() == Gtk.ResponseType.Ok ) {
                // Get data from dialog
                Formula? f = Formula.GetFormula( dlg.Formula );

                if ( f is not null ) {
                    f.Document = this.Document;
                    f.Direction = dlg.Direction;
                    f.Position = new Position( this.Document, row, col );

                    // Add formula to document
                    this.Document.FormulaManager.AddFormula( f );
                }
            }

            dlg.Destroy();
        }
    }

    /// <summary>
    /// Gets the document being edited.
    /// Can be null if no document is being edited.
    /// </summary>
    /// <value>The document.</value>
    public CsvDocument? Document {
        get; private set;
    }

    public string LastFileName {
        get => this.lastFileName;
        private set {
            this.lastFileName = value;
        }
    }

    private string lastFileName;
    private string txtToFind;
    private readonly Core.Cfg.Config cfg;
}
