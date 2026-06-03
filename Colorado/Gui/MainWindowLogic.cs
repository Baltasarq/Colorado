// Colorado (c) 2015-2025 Baltasar MIT License <baltasarq@gmail.com>
/*
 * Colorado, a tsv/csv-based spreadsheet
 */


namespace Colorado.Gui;


using System;
using Core;


public sealed partial class MainWindow: Gtk.ApplicationWindow {
    const int MaxFileLengthForTitle = 40;
    const string LongFilePrefix = "...";
    const string NoNamedFile = "nonamed.csv";

    public MainWindow(Gtk.Application app, string fileName = "")
        : base( app )
    {
        this.Title = AppInfo.Name;
        this._lastFileName = "";
        this._txtToFind = "";
        this._edFind = new Gtk.Entry( "Find..." );
        this._popup = new Gtk.Menu();
        this._menuBar = new Gtk.MenuBar();
        this._tbTools = new Gtk.Toolbar { ToolbarStyle = this.ToolbarMode };
        this._mRecent = new Gtk.Menu();
        this._lblCount = new Gtk.Label( "..." );
        this._lblType = new Gtk.Label( "..." );
        this._sbStatus = new Gtk.Statusbar();
        this._mainPanel = new Gtk.ScrolledWindow();

        this._newAction = new ( "new", "_New", "new spreadsheet", this._iconNew );
        this._openAction = new ( "open", "_Open", "open spreadhseet", this._iconOpen );
        this._saveAction = new ( "save", "_Save", "save spreadhseet", this._iconSave );
        this._saveAsAction = new ( "save_as", "Save _as...", "save spreadhseet as...", this._iconSave );
        this._propertiesAction = new ( "properties", "_Properties", "properties", this._iconProperties );
        this._closeAction = new ( "close", "_Close", "close spreadhseet", this._iconClose );
        this._aboutAction = new ( "about", "_About", "about...", this._iconAbout );
        this._importAction = new ( "import", "_Import", "import data", this._iconImport );
        this._exportAction = new ( "export", "_Export", "export to...", this._iconExport );
        this._revertAction = new ( "revert", "_Revert", "revert to file", this._iconRevert );
        this._quitAction = new ( "quit", "_Quit", "quit", this._iconExit );
        this._findAction = new ( "find", "_Find", "find...", this._iconFind );
        this._findAgainAction = new ( "find_again", "_Find again", "find again", this._iconFind );
        this._insertFormulaAction = new ( "insert_formula", "_Insert formula", "insert formula", this._iconFormula );
        this._addRowsAction = new ( "add_rows", "_Add rows", "add rows", this._iconAdd );
        this._removeRowsAction = new ( "remove_rows", "_Remove rows", "remove rows", this._iconRemove );
        this._clearRowsAction = new ( "clear_rows", "_Clear rows", "clear rows", this._iconClear );
        this._copyRowAction = new ( "copy_row", "_Copy row", "copy row", this._iconCopy );
        this._sortRowsAction = new ( "sort_rows", "_Sort", "short rows", this._iconSort );
        this._fillRowAction = new ( "fill_row", "_Fill row", "fill row", this._iconPaste );
        this._addColumnsAction = new ( "add_columns", "_Add columns", "add columns", this._iconAdd );
        this._removeColumnsAction = new ( "remove_columns", "_Remove columns", "remove columns", this._iconRemove );
        this._clearColumnsAction = new ( "clear_Columns", "_Clear columns", "clear columns", this._iconRemove );
        this._copyColumnAction = new ( "copy_column", "_Copy column", "copy column", this._iconCopy );
        this._fillColumnAction = new ( "fill_column", "_Fill column", "fill column", this._iconPaste );

        this.Build();
        this.PrepareForNoSpreadSheet();
        this._cfg = Core.Cfg.Config.Load();
        this.LoadRecentFilesIntoMenu();
        this.ActivateIde( false );

        if ( !string.IsNullOrWhiteSpace( fileName ) ) {
            this.OpenSpreadSheet( fileName );
        }
    }

    /// <summary>Loads all the recent files into menu.</summary>
    void LoadRecentFilesIntoMenu()
    {
        this._cfg.RecentFiles.ToList<string>().ForEach( this.AppendRecentFileToMenu );
    }

    /// <summary>Appends a given recent file to the menu.</summary>
    /// <param name="fileName">The file name, as a string.</param>
    void AppendRecentFileToMenu(string fileName)
    {
        string fileNameOnly = System.IO.Path.GetFileName( fileName );
        var miFile = new Gtk.MenuItem( fileNameOnly );

        this._cfg.RecentFiles = [ fileName ];
        this._mRecent.Append( miFile );

        miFile.Activated += (o, evt) => {
            if ( this.OnCloseSpreadSheet() ) {
                this.OpenSpreadSheet( fileName );
            }
        };

        miFile.Show();
        this._cfg.Save();
    }

    void PrepareForNoSpreadSheet()
    {
        this.PrepareForSpreadSheet( spreadSheet: null );
    }

    void PrepareForSpreadSheet(CsvDocument csv)
    {
        this.PrepareForSpreadSheet( new SpreadSheet( this, csv ) );
    }

    void PrepareForSpreadSheet(SpreadSheet? spreadSheet)
    {
        this.SpreadSheet = spreadSheet;

        if ( this._mainPanel.Child is not null ) {
            this._mainPanel.Remove( this._mainPanel.Child );
        }

        if ( this.SpreadSheet is not null ) {
            this.SpreadSheet.SetOnClick( this.OnTableClicked );
            this.LastFileName = this.SpreadSheet.FileName;
            this.AppendRecentFileToMenu( this.SpreadSheet.FileName );
            this._mainPanel.Add( this.SpreadSheet.TreeView );
            this.ShowSpreadSheet();
            this.ShowSpreadSheetInfo();
        }

        this.ActivateIde( spreadSheet is not null );
    }

    public void Update(int oldRows, int oldColumns)
    {
        this.SetStatus( "Reconfiguring..." );

        if ( this.SpreadSheet is not null ) {
            if ( this.SpreadSheet.NumRows != oldRows
              || this.SpreadSheet.NumColumns != oldColumns )
            {
                this.ShowSpreadSheet();
            } else {
                this.SpreadSheet.UpdateHeaders();
            }

            this.ShowSpreadSheetInfo();
        }

        this.SetStatus();
    }

    void ActivateIde()
    {
        this.ActivateIde( true );
    }

    void ActivateIde(bool active)
    {
        this.SetStatus();
        this.SetTitle();

        this.SpreadSheet?.Visible          = active;
        this._edFind.Sensitive              = active;

        this._sbStatus.Visible              = true;

        this._openAction.IsEnabled          = true;
        this._newAction.IsEnabled           = true;
        this._importAction.IsEnabled        = true;
        this._quitAction.IsEnabled          = true;
        this._aboutAction.IsEnabled         = true;

        this._saveAction.IsEnabled          = active;
        this._saveAsAction.IsEnabled        = active;
        this._revertAction.IsEnabled        = active;
        this._exportAction.IsEnabled        = active;
        this._closeAction.IsEnabled         = active;
        this._propertiesAction.IsEnabled    = active;

        this._addRowsAction.IsEnabled       = active;
        this._addColumnsAction.IsEnabled    = active;
        this._removeRowsAction.IsEnabled    = active;
        this._removeColumnsAction.IsEnabled = active;
        this._clearRowsAction.IsEnabled     = active;
        this._clearColumnsAction.IsEnabled  = active;
        this._insertFormulaAction.IsEnabled = active;
        this._copyColumnAction.IsEnabled    = active;
        this._copyRowAction.IsEnabled       = active;
        this._fillRowAction.IsEnabled       = active;
        this._sortRowsAction.IsEnabled      = active;
        this._fillColumnAction.IsEnabled    = active;

        this._findAction.IsEnabled          = active;
        this._findAgainAction.IsEnabled     = active;

        this.ShowSpreadSheetInfo();
        GtkUtil.Misc.UpdateUI();
    }

    void ShowSpreadSheet()
    {
        int row = 0;

        if ( this.SpreadSheet is not null ) {
            this.SpreadSheet.GetCurrentCell( out row, out int column );
            this.ShowSpreadSheet( row );
        }
    }

    void ShowSpreadSheet(int numRow)
    {
        if ( this.SpreadSheet == null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Spreadsheet does not exist" );
            return;
        }

        this.ShowSpreadSheetInfo();
        try {
            this.SpreadSheet.Show( numRow );
        } catch(Exception e) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Error building view: '" + e.Message + '\'' );
        }
    }

    void OnAbout()
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
        if ( this.SpreadSheet is null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Spreadsheet does not exist" );
            return;
        }

        // Search
        this._txtToFind = this._edFind.Text;
        this.SpreadSheet.FindText( 0, this._txtToFind );

        // Clean searcb text
        this._edFind.Text = "";
        this._edFind.GrabFocus();
    }

    void OnFindAgain()
    {
        if ( this.SpreadSheet is null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Spreadsheet does not exist" );
            return;
        }

        this.SpreadSheet.GetCurrentCell( out int row, out int col );
        this.SpreadSheet.FindText( row + 1, this._txtToFind  );
    }

    /// <summary>
    /// Opens a document in which the delimiter is inferred, and
    /// the first row is used for headers
    /// </summary>
    /// <param name="fileName">
    /// A <see cref="System.String"/>
    /// </param>
    void OpenSpreadSheet(string fileName)
    {
        this.OpenSpreadSheet( fileName, '\0', true );
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
    void OpenSpreadSheet(string fn, char delim, bool useHeaders)
    {
        this.ActivateIde( false );
        this.SetStatus( "Loading..." );

        // Load document
        try {
            var loader = new CsvDocumentPersistence();

            loader.Load( fn, delim, firstRowForHeaders: useHeaders );
            this.PrepareForSpreadSheet( loader.Document );
        } catch(Exception e) {
            GtkUtil.Misc.MsgError(
                        this, AppInfo.Name,
                        "Error while loading file: '" + e.Message + '\'' );
            this.SpreadSheet = null;
            this.SetStatus();
        }

        return;
    }

    bool OnCloseSpreadSheet()
    {
        bool toret = true;

        if ( this.SpreadSheet is not null ) {
            if ( GtkUtil.Misc.Ask( this, AppInfo.Name,
                            "Close spreadsheet '" + this.SpreadSheet.FileName + "' ?" ) )
            {
                this.CloseSpreadSheet();
            } else {
                toret = false;
            }
        }

        return toret;
    }

    void CloseSpreadSheet()
    {
        if ( this.SpreadSheet is not null
          && this.SpreadSheet.Changed )
        {
            // Save the document, if needed
            if ( GtkUtil.Misc.Ask( this, AppInfo.Name,
                                    "Save spreadsheet '"
                                    + this.SpreadSheet.FileName + "' ?" ) )
            {
                this.OnSave();
            }
        }

        this.PrepareForNoSpreadSheet();
        this.ActivateIde( false );
    }

    /// <summary>Opens a new document.</summary>
    void OnOpen()
    {
        if ( this.OnCloseSpreadSheet() ) {
            if ( string.IsNullOrWhiteSpace( this.LastFileName ) ) {
                this.LastFileName = ".";
            }

            if ( GtkUtil.Misc.DlgOpen( AppInfo.Name, "Open spreadsheet",
                                this,
                                ref this._lastFileName,
                                CsvDocumentPersistence.FileFilter[ 0 ] ) )
            {
                this.OpenSpreadSheet( this.LastFileName, '\0', true );
            }
        }

        return;
    }

    public void SetStatus()
    {
        this._sbStatus.Pop( 1 );
        this._sbStatus.Push( 1, "Ready" );
        GtkUtil.Misc.UpdateUI();
    }

    public void SetStatus(string msg)
    {
        this._sbStatus.Pop( 1 );
        this._sbStatus.Push( 1, msg );
        GtkUtil.Misc.UpdateUI();
    }

    public void SetTitle()
    {
        string title = AppInfo.Name;

        // Prepare file title
        if ( this.SpreadSheet is not null ) {
            string fileName = this.SpreadSheet.FileName;

            if ( fileName.Length > ( MaxFileLengthForTitle + LongFilePrefix.Length ) )
            {
                fileName = fileName.Remove( 0,
                    fileName.Length
                    - MaxFileLengthForTitle - LongFilePrefix.Length
                );
                fileName = LongFilePrefix + title;
            }

            title = fileName + " - " + title;
        }

        this.Title = title;
    }

    void ShowSpreadSheetInfo()
    {
        if ( this.SpreadSheet is not null ) {
            string delimiter = Delimiter.GetName( this.SpreadSheet.DelimiterValue );
            string text = "field";
            string number = "4";

            if ( this.SpreadSheet.SurroundText ) {
                text = "\"field\"";
            }

            number += DecimalMark.AsChar( this.SpreadSheet.DecimalSeparator ) + "5";

            this._lblType.Text = '(' + text + delimiter + number + delimiter + "...)";
            this._lblCount.Text = "["
                + this.SpreadSheet.NumRows
                + " x "
                + this.SpreadSheet.NumColumns
                + "]";
        } else {
            this._lblType.Text = this._lblCount.Text = "...";
        }

        return;
    }

    bool OnQuit()
    {
        var app = GLib.Application.Default;
        bool toret = true;

        if ( this.OnCloseSpreadSheet() ) {
            this.Visible = false;
            toret = false;

            if ( app is not null ) {
                app.Quit();
            } else {
                Environment.Exit( 0 );
            }
        }

        return toret;
    }

    void OnImport()
    {
        var dlg = new DlgImport( this );

        if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
            try {
                if ( this.OnCloseSpreadSheet() ) {
                    var options = dlg.Options;
                    var importer = Importer.GetImporter( options.ImportId );

                    importer.Options = options;
                    this.PrepareForSpreadSheet( importer.Load() );
                }
            } catch(Exception exc) {
                GtkUtil.Misc.MsgError( this, AppInfo.Name,
                                        "unable to import: " + exc.Message );
                this.PrepareForNoSpreadSheet();
            }
        }

        this.LastFileName = dlg.FileName;
        dlg.Destroy();
    }

    void OnSave()
    {
        if ( this.SpreadSheet is null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "No SpreadSheet loaded" );
            return;
        }

        if ( !( this.SpreadSheet.Changed ) ) {
            return;
        }

        if ( !( this.SpreadSheet.HasName ) ) {
            this.OnSaveAs();
            return;
        }

        try {
            this.SetStatus( "Saving..." );
            new CsvDocumentPersistence( this.SpreadSheet.Document ).SaveCsvData();
            this.SetStatus();
            this.SetTitle();
        } catch(Exception exc) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
            this.SetStatus();
        }

        return;
    }

    void OnSaveAs()
    {
        if ( this.SpreadSheet is null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "No spreadhsheet loaded" );
            this.SetStatus();
            return;
        }

        try {
            // Prepare file name
            if ( string.IsNullOrWhiteSpace( this._lastFileName ) ) {
                string docsPath = Environment.GetFolderPath(
                                    Environment.SpecialFolder.MyDocuments );

                this._lastFileName = System.IO.Path.Combine( docsPath, NoNamedFile );
            }

            // Ask for name
            if ( GtkUtil.Misc.DlgSave(
                        AppInfo.Name, "Save spreadsheet as...",
                        this,
                        ref this._lastFileName,
                        CsvDocumentPersistence.FileFilter[ 0 ] ) )
            {
                this.SetStatus( "Saving..." );
                this.SpreadSheet.FileName = this._lastFileName;
                this._lastFileName = this.SpreadSheet.FileName;  // CSVDoc fixed filename
                new CsvDocumentPersistence( this.SpreadSheet.Document ).SaveCsvData();
                this.SetTitle();
                this.SetStatus();
            }
        } catch(Exception exc) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
        }
    }

    void OnExport()
    {
        if ( this.SpreadSheet is null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "No spreadhsheet loaded" );
            this.SetStatus();
            return;
        }

        // Run the dialog
        var dlg = new DlgExport( this, this.SpreadSheet.Document );

        if ( ( (Gtk.ResponseType) dlg.Run() ) != Gtk.ResponseType.Ok ) {
            goto Exit;
        }

        // Export
        try {
            string fn = dlg.FileName.Trim();

            if ( fn.Length > 0 ) {
                this.LastFileName = fn;

                var options = new ExportOptions( fn, this.SpreadSheet.Document )
                {
                    ExporterId = dlg.ExporterId,
                    IncludeRowNumbers = dlg.IncludeRowNumbers,
                    IncludeTableBorder = dlg.IncludeTableBorder,
                    ColumnsIncluded = dlg.ColumnsIncluded,
                    QuotedText = dlg.SurroundWithDoubleQuotes,
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

        Exit:
        dlg.Destroy();
        return;
    }

    void OnEdFindActivated()
    {
        this._txtToFind = this._edFind.Text;

        // Reset the state of the entry
        this._edFind.Text = "Find...";
        this.SpreadSheet?.GrabFocus();

        // Do it
        this.SpreadSheet?.FindText( 0, this._txtToFind );
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

        if ( this.SpreadSheet is null ) {
            goto Exit;
        }

        if ( dlg.DecimalMarkValue != this.SpreadSheet.DecimalSeparator )
        {
            this.SpreadSheet.DecimalSeparator = dlg.DecimalMarkValue;
            this.ShowSpreadSheet();
        }

        this.SpreadSheet.Delimiter = dlg.Delimiter;
        this.SpreadSheet.SurroundText = dlg.SurroundText;

        // Check rows and headers size
        if ( this.SpreadSheet.NumColumns > dlg.NumColumns ) {
            if ( !GtkUtil.Misc.Ask( this, AppInfo.Name, ColDataLoss ) ) {
                dlg.NumColumns = this.SpreadSheet.NumColumns;
                dlg.NumRows = this.SpreadSheet.NumRows;
                goto Exit;
            }
        }

        if ( this.SpreadSheet.NumRows > dlg.NumRows ) {
            if ( !GtkUtil.Misc.Ask( this, AppInfo.Name, RowDataLoss ) ) {
                dlg.NumColumns = this.SpreadSheet.NumColumns;
                dlg.NumRows = this.SpreadSheet.NumRows;
                goto Exit;
            }
        }

        // Now yes, modify the size
        this.SpreadSheet.NumColumns = dlg.NumColumns;
        this.SpreadSheet.NumRows = dlg.NumRows;

        // Modify headers, if needed
        if ( this.SpreadSheet.FirstRowContainsHeaders != dlg.FirstRowForHeaders )
        {
            this.SpreadSheet.FirstRowContainsHeaders = dlg.FirstRowForHeaders;
            dlg.NumRows = this.SpreadSheet.NumRows;
        }

        Exit:
        dlg.UpdateColumnsData();
    }

    void OnProperties()
    {
        if ( this.SpreadSheet is null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "No spreadsheet loaded" );
            return;
        }

        var dlg = new DlgProperties( this, this.SpreadSheet.Document );
        var oldRows = this.SpreadSheet.NumRows;
        var oldColumns = this.SpreadSheet.NumColumns;
        Gtk.ResponseType answer;

        do {
            answer = (Gtk.ResponseType) dlg.Run();

            if ( answer == Gtk.ResponseType.Apply ) {
                this.ApplyChangedProperties( dlg );
                this.Update( oldRows, oldColumns );
                GtkUtil.Misc.UpdateUI();
                oldRows = this.SpreadSheet.NumRows;
                oldColumns = this.SpreadSheet.NumColumns;
            }
        } while( answer != Gtk.ResponseType.Close
              && answer != Gtk.ResponseType.DeleteEvent );

        // Apply changes
        this.ApplyChangedProperties( dlg );
        this.Update( oldRows, oldColumns );
        dlg.Destroy();
    }

    void OnNew()
    {
        if ( this.OnCloseSpreadSheet() ) {
            // Create new document
            this.PrepareForSpreadSheet( new CsvDocument( 10, 10 ) );

            // Trigger the properties dialog
            this.OnProperties();
            this.PrepareForSpreadSheet( this.SpreadSheet );
        }

        return;
    }

    void OnClearRows()
    {
        // Get position
        if ( this.SpreadSheet is null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
            return;
        }

        // Get current selection
        this.SpreadSheet.DetermineSelectedRows( out int rowBegin, out int rowEnd );

        var dlg = new DlgFromTo( this,
            rowBegin, rowEnd, this.SpreadSheet.NumRows,
            DlgFromTo.ActionType.Clean, DlgFromTo.ItemType.Rows );

        if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
            // Adapt from UI to document (headers)
            rowBegin = dlg.From - SpreadSheet.NumFixedRows;
            rowEnd = dlg.To - SpreadSheet.NumFixedRows;

            try {
                // do it
                this.SpreadSheet.CleanRows( rowBegin, rowEnd );
                this.SpreadSheet.RefreshRows( rowBegin, rowEnd );
            } catch(Exception exc) {
                GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
            }
        }

        dlg.Destroy();
    }

    void OnClearColumns()
    {
        // Chk
        if ( this.SpreadSheet is null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Spreadsheet does not exist" );
            return;
        }

        // Get current position and adapt it to UI
        this.SpreadSheet.GetCurrentCell( out int row, out int colBegin );
        ++colBegin;

        var dlg = new DlgFromTo( this,
            colBegin, colBegin + 1, this.SpreadSheet.NumColumns,
            DlgFromTo.ActionType.Clean, DlgFromTo.ItemType.Columns );

        if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
            // Adapt from UI to document
            colBegin = dlg.From - 1;
            int colEnd = dlg.To - 1;

            try {
                // do it
                this.SpreadSheet.CleanColumns( colBegin, colEnd, row );
                ShowSpreadSheet();
            } catch (Exception exc) {
                GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
            }
        }

        dlg.Destroy();
    }

    void OnAddRows()
    {
        // Chk
        if ( this.SpreadSheet is null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Spreadsheet does not exist" );
            return;
        }

        // Get current selection
        this.SpreadSheet.GetCurrentCell( out int row, out int col );

        var dlg = new DlgIncDec( this,
            DlgIncDec.DialogType.Insert,
            DlgIncDec.Target.Rows,
            row + 1,
            1,
            int.MaxValue
        );

        if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
            try {
                if ( dlg.From == this.SpreadSheet.NumRows
                  && dlg.Where == DlgIncDec.WherePosition.After )
                {
                    // Add mode
                    this.SpreadSheet.NumRows += dlg.Number;
                } else {
                    int modifier = 0;

                    if ( dlg.From > 0
                      && dlg.Where == DlgIncDec.WherePosition.After )
                    {
                        modifier = -1;
                    }

                    this.SpreadSheet.Document.Data.InsertRows( dlg.From - SpreadSheet.NumFixedRows - modifier, dlg.Number );
                }

                this.ShowSpreadSheet();
            } catch(Exception exc) {
                GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
            }
        }

        dlg.Destroy();
    }

    void OnAddColumns()
    {
        // Chk
        if ( this.SpreadSheet is null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
            return;
        }

        // Get current position
        this.SpreadSheet.GetCurrentCell( out int row, out int col );

        var dlg = new DlgIncDec( this,
            DlgIncDec.DialogType.Insert,
            DlgIncDec.Target.Columns,
            col + 1, col + 2, int.MaxValue
        );

        if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
            try {
                // do it
                if ( dlg.From == this.SpreadSheet.NumColumns
                    && dlg.Where == DlgIncDec.WherePosition.After )
                {
                    this.SpreadSheet.NumColumns += dlg.Number;
                } else {
                    int modifier = 0;

                    if ( dlg.From > 0
                        && dlg.Where == DlgIncDec.WherePosition.After )
                    {
                        modifier = -1;
                    }

                    this.SpreadSheet.Document.Data.InsertColumns(
                                                        dlg.From - SpreadSheet.NumFixedColumns - modifier,
                                                        dlg.Number );
                }

                this.ShowSpreadSheet();
            } catch(Exception exc) {
                GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
            }
        }

        dlg.Destroy();
    }

    void OnRevert()
    {
        var oldSpreadSheet = this.SpreadSheet;

        // Chk
        if ( this.SpreadSheet is not null ) {
            if ( GtkUtil.Misc.Ask( this, AppInfo.Name, "Revert to the file on disk. Are you sure?" ) )
            {
                // Store the parameters and reload
                var fileName = this.SpreadSheet.FileName;
                var firstRowForHeaders = this.SpreadSheet.FirstRowContainsHeaders;
                char delimiter = this.SpreadSheet.DelimiterValue[ 0 ];
                this.SpreadSheet = null;

                try {
                    this.OpenSpreadSheet( fileName, delimiter, firstRowForHeaders );

                    // Check result
                    if ( this.SpreadSheet is null ) {
                        this.SpreadSheet = oldSpreadSheet;
                    }
                } catch(Exception exc)
                {
                    this.SpreadSheet = oldSpreadSheet;
                    GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
                }
            }
        } else {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "SpreadSheet does not exist" );
        }

        return;
    }

    void OnRemoveRows()
    {
        // Chk
        if ( this.SpreadSheet is null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
            return;
        }

        // Get current selection
        this.SpreadSheet.DetermineSelectedRows( out int rowBegin, out int rowEnd );

        var dlg = new DlgIncDec( this,
            DlgIncDec.DialogType.Erase,
            DlgIncDec.Target.Rows,
            rowBegin, rowEnd,
            this.SpreadSheet.NumRows
        );

        if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
            try {
                // do it
                this.SetStatus( "Removing rows" );
                this.SpreadSheet.Document.Data.RemoveRows(
                                                    dlg.From - SpreadSheet.NumFixedRows,
                                                    dlg.Number );
                this.ShowSpreadSheet();
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
        if ( this.SpreadSheet is null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
            return;
        }

        // Get current position
        this.SpreadSheet.GetCurrentCell( out int row, out int col );

        var dlg = new DlgIncDec( this,
            DlgIncDec.DialogType.Erase,
            DlgIncDec.Target.Columns,
            col + 1, col + 2,
            this.SpreadSheet.NumColumns
        );

        if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
            try {
                // do it
                this.SetStatus( "Removing columns" );
                this.SpreadSheet.Document.Data.RemoveColumns(
                                                dlg.From - SpreadSheet.NumFixedColumns,
                                                dlg.Number );
                this.ShowSpreadSheet();
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
        if ( this.SpreadSheet is not null ) {

            // Get current position
            this.SpreadSheet.DetermineSelectedRows( out int rowBegin, out int rowEnd );

            var dlg = new DlgFromTo( this,
                rowBegin, rowBegin + 1, this.SpreadSheet.NumRows,
                DlgFromTo.ActionType.Copy, DlgFromTo.ItemType.Rows );

            if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
                try {
                    // do it
                    this.SetStatus( "Copying row" );
                    this.SpreadSheet.Document.Data.CopyRow(
                                                        dlg.From - SpreadSheet.NumFixedRows,
                                                        dlg.To - SpreadSheet.NumFixedRows );
                    this.ShowSpreadSheet();
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
        if ( this.SpreadSheet is null ) {
            GtkUtil.Misc.MsgError( this, AppInfo.Name, "Document does not exist" );
            return;
        }

        // Get current position
        this.SpreadSheet.GetCurrentCell( out int row, out int col );

        var dlg = new DlgFromTo( this,
            col + 1, col + 2,
            this.SpreadSheet.NumColumns,
            DlgFromTo.ActionType.Copy,
            DlgFromTo.ItemType.Columns );

        if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
            try {
                // do it
                this.SetStatus( "Copying column" );
                this.SpreadSheet.Document.Data.CopyColumn(
                                                    dlg.From - SpreadSheet.NumFixedColumns,
                                                    dlg.To - SpreadSheet.NumFixedColumns );
                this.ShowSpreadSheet();
                this.SetStatus();
            } catch(Exception exc) {
                GtkUtil.Misc.MsgError( this, AppInfo.Name, exc.Message );
            }
        }

        dlg.Destroy();
    }

    void OnFillRow()
    {
        if ( this.SpreadSheet is not null ) {
            // Retrieve position
            this.SpreadSheet.GetCurrentCell( out int row, out int column );

            // Ask for filling
            var dlg = new DlgFill( this ) { Modal = true };

            if ( (Gtk.ResponseType) dlg.Run() == Gtk.ResponseType.Ok ) {
                var filler = Filler.CreateFiller(
                                this.SpreadSheet.Document,
                                new Position( this.SpreadSheet.Document, row, column ),
                                -1,
                                dlg.FillValue,
                                dlg.KindOfFill,
                                Filler.FillDirection.Row );

                while ( filler.NumCells > 0 ) {
                    filler.DoIt();
                    GtkUtil.Misc.UpdateUI();
                }

                this.SpreadSheet.Show( row );
            }

            dlg.Destroy();
        }
    }

    void OnFillColumn()
    {
        if ( this.SpreadSheet is not null ) {
            // Retrieve position
            this.SpreadSheet.GetCurrentCell( out int row, out int column );

            // Ask for filling
            var dlg = new DlgFill( this );

            if ( (Gtk.ResponseType) dlg.Run() == Gtk.ResponseType.Ok ) {
                var filler = Filler.CreateFiller(
                    this.SpreadSheet.Document,
                    new Position( this.SpreadSheet.Document, row, column ),
                    -1,
                    dlg.FillValue,
                    dlg.KindOfFill,
                    Filler.FillDirection.Column );

                while ( filler.NumCells > 0 ) {
                    filler.DoIt();
                    GtkUtil.Misc.UpdateUI();
                }

                this.SpreadSheet.Show( row );
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
            this._popup.Popup();
        }

        return;
    }

    void OnInsertFormula()
    {
        if ( this.SpreadSheet is not null ) {
            var dlg = new DlgFormulae( this );

            // Get current position
            this.SpreadSheet.GetCurrentCell( out int row, out int col );

            // Fire dialog
            if ( (Gtk.ResponseType) dlg.Run() == Gtk.ResponseType.Ok ) {
                // Get data from dialog
                Formula? f = Formula.GetFormula(
                                        dlg.Formula,
                                        this.SpreadSheet.Document,
                                        new Position( this.SpreadSheet.Document, row, col ),
                                        dlg.Direction );

                if ( f is not null ) {
                    this.SpreadSheet.Document.FormulaManager.AddFormula( f );
                } else {
                    GtkUtil.Misc.MsgError(
                                    this,
                                    AppInfo.Name,
                                    "Internal: formula not found." );
                }
            }

            dlg.Destroy();
        }
    }

    /// <summary>The spreadsheet to edit.
    /// Can be null if there is no spreadsheet being edited.
    /// </summary>
    public SpreadSheet? SpreadSheet { get; private set; }

    public string LastFileName {
        get => this._lastFileName;
        private set {
            this._lastFileName = value;
        }
    }

    private string _lastFileName;
    private string _txtToFind;
    private readonly Core.Cfg.Config _cfg;
}
