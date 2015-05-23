// Colorado, a csv editor

using System;
using Gtk;
using System.Collections;
using Colorado.Core;
using System.Collections.Generic;

using GtkUtil;

namespace Colorado.Gui {
public partial class MainWindow : Gtk.Window {
	public const int MaxFileLengthForTitle = 40;
	public const string LongFilePrefix = "...";
	public const int NumFixedColumns = 1;
	public const int NumFixedRows = 1;

	public MainWindow()
		: base( Gtk.WindowType.Toplevel )
    {
		lastFileName = "";
		popup = null;
		document = null;
		txtToFind = "";
		
		Build();

		Gdk.Geometry minSize = new Gdk.Geometry();
		minSize.MinHeight = 480;
		minSize.MinWidth = 640;

		this.Title = AppInfo.Name;
		this.SetDefaultSize( minSize.MinHeight, minSize.MinWidth );
		this.SetGeometryHints( this, minSize, Gdk.WindowHints.MinSize );
		this.CreatePopup();
		this.ActivateIde( false );
	}
	
	public MainWindow(string fileName)
		: this()
	{
		OpenDocument( fileName );
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
		
		this.tvTable.Visible			   = active;
		this.sbStatus.Visible 			   = true;
		
		this.EditAction.Sensitive          = active;
		this.SaveAction.Sensitive          = active;
		this.SaveAsAction.Sensitive        = active;
		this.revertToSavedAction.Sensitive = active;
		this.convertAction.Sensitive       = active;
		this.closeAction.Sensitive         = active;
		this.propertiesAction.Sensitive    = active;
		
		this.btAdd.Sensitive			   = active;
		this.btInsert.Sensitive		 	   = active;
		this.btSave.Sensitive			   = active;
		this.btFind.Sensitive			   = active;
		this.btProperties.Sensitive		   = active;
		this.btRemove.Sensitive			   = active;
		this.btClearColumn.Sensitive	   = active;
		this.btClearRow.Sensitive		   = active;
		this.btCopyRow.Sensitive           = active;
		this.btInsertColumns.Sensitive	   = active;
		this.btRemoveColumn.Sensitive      = active;
		this.btAddColumns.Sensitive        = active;
		this.btCopyColumn.Sensitive        = active;
		this.btFillRow.Sensitive           = active;
		this.btFillColumn.Sensitive        = active;
		
		Util.UpdateUI();
	}
	
	protected void ShowDocument()
	{
		int row = 0;
		int column;
		
		if ( this.document != null ) {
			this.GetCurrentCell( out row, out column );
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
				column = new Gtk.TreeViewColumn();
				column.Expand = true;
				cell = new Gtk.CellRendererText();
				column.Title = document.Data.ColumnInfo[ colNum ].Header;
				column.PackStart( cell, true );
				cell.Editable = true;
				column.AddAttribute( cell, "text", colNum + 1 );
				cell.Edited += OnTreeViewCellEdited;
				
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
		
		tvTable.EnableGridLines = TreeViewGridLines.Both;
		tvTable.SetCursor(
				new TreePath( new int[]{ numRow } ),
				tvTable.Columns[ 1 ],
				false
		);
		tvTable.Show();
	}
	
	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		OnExit( sender, a );
		a.RetVal = true;
	}
	
	protected void OnAbout(object sender, System.EventArgs args)
	{
		var about = new Gtk.AboutDialog();
		String[] authors = { AppInfo.Author };
		
		about.ProgramName = AppInfo.Name;
		about.Version = AppInfo.Version;
		about.Authors = authors;
		about.Comments = AppInfo.Comments;
		about.License = AppInfo.License;
		about.Copyright = "(c) " + authors[ 0 ];
		about.Website = AppInfo.Website;
		
		about.Logo = this.Icon;
		
		about.Parent = this;
		about.TransientFor = this;
		about.SetPosition( WindowPosition.CenterOnParent );
	    about.Run();
	    about.Destroy();
	}

	protected virtual void OnFind(object sender, System.EventArgs e)
	{
		if ( this.document == null ) {
			Util.MsgError( this, AppInfo.Name, "Document does not exist" );
			return;
		}

		// Get text to search
		string txt = DlgSearch.ask( this, this.Title, "Text will be searched for in the whole document." );
		txt = txt.Trim().ToLower();
		txtToFind = txt;
		
		// Do it
		this.FindText( 0 );
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
	/// <param name="fileName">
	/// A <see cref="System.String"/> holding the file name
	/// </param>
	/// <param name="delimiter">
	/// A <see cref="System.Char"/> whihc is going to be the delimiter. A '\0'
	/// says that the delimiter must be inferred
	/// </param>
	/// <param name="firstRowForHeaders">
	/// A <see cref="System.Boolean"/> saying whether the first row is for headers or not.
	/// </param>
	protected void OpenDocument(string fileName, char delimiter, bool firstRowForHeaders)
	{	
        this.ActivateIde( false );
		this.SetStatus( "Loading..." );
		
		// Load document
		try {
			var loader = new CsvDocumentPersistence();

			if ( delimiter == '\0' ) {
				loader.Load( fileName, firstRowForHeaders: firstRowForHeaders );
			} else {
				loader.Load( fileName, delimiter, firstRowForHeaders );
			}

			this.PrepareDocument( loader.Document );

			// Show it
			this.lastFileName = fileName;
			this.ShowDocument();
			this.ShowProjectInfo();
			this.ActivateIde();
		} catch(Exception e) {
			Util.MsgError( this, AppInfo.Name, "Error while loading csv file: '" + e.Message + '\'' );
			this.document = null;
		}

		return;
	}
	
	protected void CloseDocument()
	{
		if ( this.document != null ) {
			if ( this.document.Changed ) {
				// Save the document, if needed
				if ( Util.Ask( this, AppInfo.Name, "Save CSV document '" + document.FileName + "' ?" ) ) {
					this.OnSave( null, null );
				}
			}
			
			this.document = null;
		}
		
		this.ActivateIde( false );
	}

	protected virtual void OnOpen(object sender, System.EventArgs evt)
	{
		this.CloseDocument();
		
		if ( Util.DlgOpen( AppInfo.Name,
							"Open CSV",
							this,
							ref lastFileName,
							CsvDocumentPersistence.FileFilter ) )
		{
			this.OpenDocument( lastFileName, '\0', true );
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
		string delimiter = this.Document.Delimiter.Name;
		string text = "field";
        string number = "4";
		
		if ( this.document.SurroundText ) {
			text = "\"field\"";
		}

        number += Document.GetDecimalMark() + "5";
		
        lblType.Text = '(' + text + delimiter + number + delimiter + "...)";
		return;
	}

	protected virtual void OnExit(object sender, System.EventArgs e)
	{
		this.CloseDocument();
		
		this.Visible = false;
		Application.Quit();
	}

	protected virtual void OnImport(object sender, System.EventArgs e)
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
        var rowPath = new TreePath( new int[]{ rowIndex } );
        TreeViewColumn colPath = this.tvTable.Columns[ colIndex ];

        this.tvTable.ScrollToCell(
            rowPath,
            colPath,
            false,
            (float) 0.0,
            (float) 0.0
        );

        this.tvTable.SetCursor( rowPath, colPath, edit );
    }
	
	public void GetCurrentCell(out int row, out int col)
	{
		TreePath rowPath;
		TreeIter rowPointer;
		TreeViewColumn colPath;

		// Convert path in row and rowPointer
		tvTable.GetCursor( out rowPath, out colPath );
		
		if ( rowPath != null
		  && colPath != null )
		{
			tvTable.Model.GetIter( out rowPointer, rowPath );
			row = rowPath.Indices[ 0 ];
			
			// Find out the column order
			for(col = 0; col < tvTable.Columns.Length; ++col) {
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
	
	protected virtual void OnTreeViewCellEdited(object sender, EditedArgs args)
	{
		int row;
		int col;
		
		try {
			// Get current position
			TreePath rowPath;
			TreeIter rowPointer;
			TreeViewColumn colPath;
	
			// Convert path in row and rowPointer
			rowPath = new Gtk.TreePath( args.Path );
			tvTable.Model.GetIter( out rowPointer, rowPath );
			row = rowPath.Indices[ 0 ];
			
			// Find out the column order
			tvTable.GetCursor( out rowPath, out colPath );
			for(col = 0; col < tvTable.Columns.Length; ++col) {
				if ( tvTable.Columns[ col ] == colPath ) {
					break;
				}
			}
		
			// Store data
			try {
				this.document.Data[ row, col - NumFixedColumns ] = args.NewText;
				this.tvTable.Model.SetValue( rowPointer, col, args.NewText );
					
				if ( args.NewText == "" ) {
					this.Document.FormulaManager.RemoveFormula( row, col - NumFixedColumns );
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

	protected virtual void OnSave(object sender, System.EventArgs e)
	{
		try {
			if ( this.document != null ) {
				if ( this.document.Changed ) {
					if ( this.document.HasName ) {
							new CsvDocumentPersistence( Document ).SaveCsvData();
						} else {
							this.OnSaveAs( sender, e );
						}
					
					this.SetTitle();
				}
			} else Util.MsgError( this, AppInfo.Name, "No document loaded" );
		} catch(Exception exc) {
			Util.MsgError( this, AppInfo.Name, exc.Message );
		}

		return;
	}
	
	protected virtual void OnSaveAs(object sender, System.EventArgs e)
	{
		try {
			if ( this.document != null ) {
				if ( Util.DlgOpen(
						AppInfo.Name, "Save CSV as...",
						this,
						ref lastFileName,
						CsvDocumentPersistence.FileFilter ) )
				{
					this.document.FileName = this.lastFileName;
					this.lastFileName = this.document.FileName;
					new CsvDocumentPersistence( Document ).SaveCsvData(
							new ExportOptions( this.lastFileName, this.document ) );
					this.SetTitle();
				}
			} else {
				Util.MsgError( this, AppInfo.Name, "No document loaded" );
			}
		} catch(Exception exc) {
			Util.MsgError( this, AppInfo.Name, exc.Message );
		}		
	}

	protected virtual void OnExport(object sender, System.EventArgs e)
		{
			ExportOptions options = null;
		
			if ( this.document != null ) {
				DlgExport dlg = new DlgExport( this, this.document );
		
			if ( ( (Gtk.ResponseType) dlg.Run() ) == Gtk.ResponseType.Ok ) {
				try {
					string fn = ( (DlgExport) dlg ).FileName;
					
					if ( fn.Trim().Length > 0 ) {
						this.lastFileName = fn;
						options = new ExportOptions( lastFileName, document );
                        options.Format = dlg.Selection;
						options.IncludeRowNumbers = dlg.IncludeRowNumbers;
						options.IncludeTableBorder = dlg.IncludeTableBorder;
						options.ColumnsIncluded = dlg.ColumnsIncluded;

                        // If CSV export, get more data
                        if ( dlg.Selection == ExportOptions.SelectionType.Csv )
                        {
                                var dlgCsv = new DlgCsvExport( this, document );
                                if ( ( (Gtk.ResponseType) dlgCsv.Run() ) == Gtk.ResponseType.Ok ) {
                                    options.Delimiter.Name = dlgCsv.Delimiter;
                                    options.QuotedText = dlgCsv.SurroundWithDoubleQuotes;
                                }

                                dlgCsv.Destroy();
                        }
                        
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
	
	protected virtual void OnClose(object sender, System.EventArgs e)
	{
		this.CloseDocument();
	}
	
	protected void UpdateDocumentView(int oldRows, int oldColumns)
	{
		SetStatus( "Reconfiguring..." );
			
		if ( this.document.Changed ) {
			if ( this.document.Data.NumRows != oldRows
			  || this.document.Data.NumColumns != oldColumns )
			{
				this.ShowDocument();
			} else {
				// Update headers
				for(int j = 0; j < this.document.Data.ColumnInfo.Length; ++j) {
					this.tvTable.Columns[ j + NumFixedColumns ].Title =
							this.document.Data.ColumnInfo[ j ].Header;
				}
			}
			
			this.ShowProjectInfo();
		}
		
		this.SetStatus();
	}
	
	protected virtual void OnProperties(object sender, System.EventArgs e)
	{
		if ( this.document != null ) {
			var dlg = new DlgProperties( this, this.document );
			var oldRows = this.document.Data.NumRows;
			var oldColumns = this.document.Data.NumColumns;
			var answer = Gtk.ResponseType.Apply;
			
			do {
				answer = (Gtk.ResponseType) dlg.Run();
				
				if ( answer == Gtk.ResponseType.Apply ) {
					dlg.ApplyPreferences();
					this.UpdateDocumentView( oldRows, oldColumns );
					oldRows = this.document.Data.NumRows;
					oldColumns = this.document.Data.NumColumns;
				}
			} while( answer != Gtk.ResponseType.Close );
			
			dlg.ApplyPreferences();
			dlg.Destroy();
			this.UpdateDocumentView( oldRows, oldColumns );
			
		} else Util.MsgError( this, AppInfo.Name, "No document loaded" );
	}
	
	protected virtual void OnNew(object sender, System.EventArgs e)
	{
		this.CloseDocument();
	
		// Create new document
		this.PrepareDocument( new CsvDocument( 10, 10 ) );
		
		// Trigger the properties dialog
		this.ShowDocument();
		this.SetTitle();
		this.OnProperties( this, null);
		
		// Show everything
		this.ActivateIde();
		this.ShowDocument();
		this.SetStatus();
		this.ShowProjectInfo();
	}
	
	public void FindText(int rowBegin)
	{
		this.FindText( rowBegin, this.txtToFind );
	}
	
	public void FindText(int rowBegin, string txtToFind)
	{
		for(int i = rowBegin; i < this.document.Data.NumRows; ++i) {
			for(int j = 0; j < this.document.Data.NumColumns; ++j) {
				var cell = this.document.Data[ i, j ].Trim().ToLower();
				
				if ( cell.Contains( txtToFind ) ) {
					int[] path = new int[] { i };
					this.tvTable.SetCursor( new TreePath( path ), tvTable.Columns[ j + 1 ], false );
					goto End;
				}
			}
		}
		
		End:
		return;
	}
	
	protected virtual void OnFindAgain(object sender, System.EventArgs e)
	{
		int row;
		int col;
		
		if ( this.document != null ) {
            this.GetCurrentCell( out row, out col );
            this.FindText( row +1 );
        } else {
			Util.MsgError( this, AppInfo.Name, "Document does not exist" );
		}
		
        return;
	}
	
	public void RefreshRows(int col, int begin, int end)
	{
		TreePath rowPath;
		TreeIter rowPointer;
		
		// Run over rows
		for(int i = begin; i <= end; ++i) {
			// Get an iterator for this row
			rowPath = new TreePath( Convert.ToString( i ) );
			this.tvTable.Model.GetIter( out rowPointer, rowPath );
			
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
		
	protected virtual void OnClearRow(object sender, System.EventArgs e)
	{
		int rowBegin;
		int rowEnd;
		int col;
		
		// Get position
		if ( document == null ) {
			Util.MsgError( this, AppInfo.Name, "Document does not exist" );
			return;
		}
		
		// Get current position and adapt it to UI
		GetCurrentCell( out rowBegin, out col );
		++rowBegin;
		
		var dlg = new DlgClean( this, DlgClean.Items.Row, rowBegin, document.Data.NumRows );
		
		if ( ( (ResponseType) dlg.Run() ) == ResponseType.Ok ) {
			// Adapt from UI to document (headers)
			rowBegin = dlg.Begin - NumFixedRows;
			rowEnd = dlg.End - NumFixedRows;
			
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
	
	protected virtual void OnClearColumn(object sender, System.EventArgs e)
	{
		int colBegin;
		int colEnd;
		int row;
		
		// Chk
		if ( this.document == null ) {
			Util.MsgError( this, AppInfo.Name, "Document does not exist" );
			return;
		}
		
		// Get current position and adapt it to UI
		GetCurrentCell( out row, out colBegin );
		++colBegin;
		
		var dlg = new DlgClean( this, DlgClean.Items.Column, colBegin, document.Data.NumColumns );
		
		if ( ( (ResponseType) dlg.Run() ) == ResponseType.Ok ) {
			// Adapt from UI to document
			colBegin = dlg.Begin - 1;
			colEnd = dlg.End - 1;
			
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
	
	protected virtual void OnInsertRows(object sender, System.EventArgs e)
	{
		int row;
		int col;
		
		// Chk
		if ( this.document == null ) {
			Util.MsgError( this, AppInfo.Name, "Document does not exist" );
			return;
		}
		
		// Get current position
		this.GetCurrentCell( out row, out col );
		
		var dlg = new DlgIncDec( this,
		                         DlgIncDec.DialogType.Insert,
		                         DlgIncDec.Target.Rows,
		                         row + 1, this.document.Data.NumRows
		);
		
		if ( ( (ResponseType) dlg.Run() ) == ResponseType.Ok ) {
			try {
				// do it
				this.document.Data.InsertRows( dlg.From - NumFixedRows, dlg.Number );
				this.ShowDocument();
			} catch(Exception exc) {
				Util.MsgError( this, AppInfo.Name, exc.Message );
			}
		}
		
		dlg.Destroy();
	}
	
	protected virtual void OnAddRows(object sender, System.EventArgs e)
	{
		int row;
		int col;
		
		// Chk
		if ( this.document == null ) {
			Util.MsgError( this, AppInfo.Name, "Document does not exist" );
			return;
		}
		
		// Get current position
		this.GetCurrentCell( out row, out col );
		
		var dlg = new DlgIncDec( this,
		                         DlgIncDec.DialogType.Add,
		                         DlgIncDec.Target.Rows,
		                         this.document.Data.NumRows,
								 int.MaxValue
		);
		
		if ( ( (ResponseType) dlg.Run() ) == ResponseType.Ok ) {
			try {
				// do it
				this.document.Data.NumRows += dlg.Number;
				this.ShowDocument();
			} catch(Exception exc) {
				Util.MsgError( this, AppInfo.Name, exc.Message );
			}
		}
		
		dlg.Destroy();
	}
	
	protected virtual void OnInsertColumns(object sender, System.EventArgs e)
	{
		int row;
		int col;
		
		// Chk
		if ( this.document == null ) {
			Util.MsgError( this, AppInfo.Name, "Document does not exist" );
			return;
		}
		
		// Get current position
		this.GetCurrentCell( out row, out col );
		
		var dlg = new DlgIncDec( this,
		                         DlgIncDec.DialogType.Insert,
		                         DlgIncDec.Target.Columns,
		                         col + 1, this.document.Data.NumColumns
		);
		
		if ( ( (ResponseType) dlg.Run() ) == ResponseType.Ok ) {
			try {
				// do it
				this.document.Data.InsertColumns( dlg.From - NumFixedColumns, dlg.Number );
				this.ShowDocument();
			} catch(Exception exc) {
				Util.MsgError( this, AppInfo.Name, exc.Message );
			}
		}
		
		dlg.Destroy();
	}
	
	protected virtual void OnAddColumns(object sender, System.EventArgs e)
	{
		int row;
		int col;
		
		// Chk
		if ( this.document == null ) {
			Util.MsgError( this, AppInfo.Name, "Document does not exist" );
			return;
		}
		
		// Get current position
		this.GetCurrentCell( out row, out col );
		
		var dlg = new DlgIncDec( this,
		                         DlgIncDec.DialogType.Add,
		                         DlgIncDec.Target.Columns,
		                         this.document.Data.NumColumns, int.MaxValue
		);
		
		if ( ( (ResponseType) dlg.Run() ) == ResponseType.Ok ) {
			try {
				// do it
				this.document.Data.NumColumns += dlg.Number;
				this.ShowDocument();
			} catch(Exception exc) {
				Util.MsgError( this, AppInfo.Name, exc.Message );
			}
		}
		
		dlg.Destroy();
	}
	
	protected virtual void OnRevert(object sender, System.EventArgs e)
		{
			var oldDocument = this.document;
		
			// Chk
			if ( this.document != null ) {
				if ( Util.Ask( this, AppInfo.Name, "Revert to the file on disk. Are you sure?" ) )
			{
				// Store the parameters and reload	
				var fileName = document.FileName;
				var firstRowForHeaders = document.Data.FirstRowForHeaders;
				char delimiter = document.Delimiter.Raw;
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
	
	protected virtual void OnRemoveRows(object sender, System.EventArgs e)
	{
		int row;
		int col;
		
		// Chk
		if ( this.document == null ) {
			Util.MsgError( this, AppInfo.Name, "Document does not exist" );
			return;
		}
		
		// Get current position
		this.GetCurrentCell( out row, out col );
		
		var dlg = new DlgIncDec( this,
		                         DlgIncDec.DialogType.Erase,
		                         DlgIncDec.Target.Rows,
		                         row +1,
								 this.document.Data.NumRows
		);
		
		if ( ( (ResponseType) dlg.Run() ) == ResponseType.Ok ) {
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
	
	protected virtual void OnRemoveColumns(object sender, System.EventArgs e)
	{
		int row;
		int col;
		
		// Chk
		if ( this.document == null ) {
			Util.MsgError( this, AppInfo.Name, "Document does not exist" );
			return;
		}
		
		// Get current position
		this.GetCurrentCell( out row, out col );
		
		var dlg = new DlgIncDec( this,
		                         DlgIncDec.DialogType.Erase,
		                         DlgIncDec.Target.Columns,
		                         col + 1,
								 this.document.Data.NumColumns
		);
		
		if ( ( (ResponseType) dlg.Run() ) == ResponseType.Ok ) {
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
	
	protected virtual void OnCopyRow(object sender, System.EventArgs e)
	{
		int row;
		int col;
		
		// Chk
		if ( this.document != null ) {
			
			// Get current position
			this.GetCurrentCell( out row, out col );
			
			var dlg = new DlgCopy( this, row +1, document.Data.NumRows, DlgCopy.DialogType.Rows );
			
			if ( ( (ResponseType) dlg.Run() ) == ResponseType.Ok ) {
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
	
	protected virtual void OnCopyColumn(object sender, System.EventArgs e)
	{
		int row;
		int col;
		
		// Chk
		if ( this.document == null ) {
			Util.MsgError( this, AppInfo.Name, "Document does not exist" );
			return;
		}
		
		// Get current position
		this.GetCurrentCell( out row, out col );
		
		var dlg = new DlgCopy( this, col + 1, this.document.Data.NumColumns, DlgCopy.DialogType.Columns );
		
		if ( ( (ResponseType) dlg.Run() ) == ResponseType.Ok ) {
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
	
	protected virtual void CreatePopup()
	{
		// Menus
		this.popup = new Menu();
		
		// Rows
		// Add row
		var menuItemAddRow = new ImageMenuItem( "_Add rows" );
		menuItemAddRow.Image = new Gtk.Image( Stock.Add, IconSize.Menu );
		menuItemAddRow.Activated += delegate { this.OnAddRows( null, null ); };
		this.popup.Append( menuItemAddRow );
		
		// Insert row
		var menuItemInsertRow = new ImageMenuItem( "_Insert rows" );
		menuItemInsertRow.Image = new Gtk.Image( Stock.Indent, IconSize.Menu );
		menuItemInsertRow.Activated += delegate { this.OnInsertRows( null, null ); };
		this.popup.Append( menuItemInsertRow );
		
		// Remove row
		var menuItemRemoveRow = new ImageMenuItem( "_Remove rows" );
		menuItemRemoveRow.Image = new Gtk.Image( Stock.Remove, IconSize.Menu );
		menuItemRemoveRow.Activated += delegate { this.OnRemoveRows( null, null ); };
		this.popup.Append( menuItemRemoveRow );
		
		// Clean row
		var menuItemCleanRow = new ImageMenuItem( "C_lean rows" );
		menuItemCleanRow.Image = new Gtk.Image( Stock.Clear, IconSize.Menu );
		menuItemCleanRow.Activated += delegate { this.OnClearRow( null, null ); };
		this.popup.Append( menuItemCleanRow );
		
		// Copy row
		var menuItemCopyRow = new ImageMenuItem( "_Copy rows" );
		menuItemCopyRow.Image = new Gtk.Image( Stock.GotoBottom, IconSize.Menu );
		menuItemCopyRow.Activated += delegate { this.OnCopyRow( null, null ); };
		this.popup.Append( menuItemCopyRow );
		
		// Fill row
		var menuItemFillRow = new ImageMenuItem( "Fill _row" );
		menuItemFillRow.Image = new Gtk.Image( Stock.ColorPicker, IconSize.Menu );
		menuItemFillRow.Activated += delegate { this.OnFillRow( null, null ); };
		this.popup.Append( menuItemFillRow );
		
		// Columns
		this.popup.Append( new Gtk.SeparatorMenuItem() );
		
		// Add column
		var menuItemAddColumn = new ImageMenuItem( "_Add columns" );
		menuItemAddColumn.Image = new Gtk.Image( Stock.Add, IconSize.Menu );
		menuItemAddColumn.Activated += delegate { this.OnAddColumns( null, null ); };
		this.popup.Append( menuItemAddColumn );
		
		// Insert column
		var menuItemInsertColumn = new ImageMenuItem( "_Insert columns" );
		menuItemInsertColumn.Image = new Gtk.Image( Stock.Indent, IconSize.Menu );
		menuItemInsertColumn.Activated += delegate { this.OnInsertColumns( null, null ); };
		this.popup.Append( menuItemInsertColumn );
		
		// Remove column
		var menuItemRemoveColumn = new ImageMenuItem( "_Remove columns" );
		menuItemRemoveColumn.Image = new Gtk.Image( Stock.Remove, IconSize.Menu );
		menuItemRemoveColumn.Activated += delegate { this.OnRemoveColumns( null, null ); };
		popup.Append( menuItemRemoveColumn );
		
		// Clean column
		var menuItemCleanColumn = new ImageMenuItem( "C_lean columns" );
		menuItemCleanColumn.Image = new Gtk.Image( Stock.Clear, IconSize.Menu );
		menuItemCleanColumn.Activated += delegate { this.OnClearColumn( null, null ); };
		this.popup.Append( menuItemCleanColumn );
		
		// Copy column
		var menuItemCopyColumn = new ImageMenuItem( "_Copy columns" );
		menuItemCopyColumn.Image = new Gtk.Image( Stock.GotoLast, IconSize.Menu );
		menuItemCopyColumn.Activated += delegate { this.OnCopyColumn( null, null ); };
		this.popup.Append( menuItemCopyColumn );
		
		// Fill column
		var menuItemFillColumn = new ImageMenuItem( "Fill colu_mn" );
		menuItemFillColumn.Image = new Gtk.Image( Stock.ColorPicker, IconSize.Menu );
		menuItemFillColumn.Activated += delegate { this.OnFillRow( null, null ); };
		this.popup.Append( menuItemFillColumn );
		
		// Find
		popup.Append( new Gtk.SeparatorMenuItem() );
		var menuItemFind = new ImageMenuItem( "_Find" );
		menuItemFind.Image = new Gtk.Image( Stock.Find, IconSize.Menu );
		menuItemFind.Activated += delegate { this.OnFind( null, null ); };
		this.popup.Append( menuItemFind );
		
		// Properties
		var menuItemProperties = new ImageMenuItem( "_Properties" );
		menuItemProperties.Image = new Gtk.Image( Stock.Properties, IconSize.Menu );
		menuItemProperties.Activated += delegate { this.OnProperties( null, null ); };
		this.popup.Append( menuItemProperties );
		
		// Close
		var menuItemClose = new ImageMenuItem( "_Close" );
		menuItemClose.Image = new Gtk.Image( Stock.Close, IconSize.Menu );
		menuItemClose.Activated += delegate { this.OnClose( null, null ); };
		this.popup.Append( menuItemClose );
		
		// Finish
		this.popup.ShowAll();
	}
	
	protected virtual void OnViewToolbarActivated(object sender, System.EventArgs e)
	{
		this.tbToolBar.Visible = !tbToolBar.Visible;
	}
	
	protected virtual void OnTableClicked(object o, Gtk.ButtonReleaseEventArgs args)
	{
		if ( args.Event.Button == 3 ) {
			this.popup.Popup();
		}
	}
	
	protected virtual void OnFillRow(object sender, System.EventArgs e)
	{
		int row;
		int column;
		
		// Retrieve position
		this.GetCurrentCell( out row, out column );

		// Ask for filling
		var dlg = new DlgFill( this );
		dlg.Modal = true;
		
		if ( (Gtk.ResponseType) dlg.Run() == Gtk.ResponseType.Ok ) {
			string fillValue = dlg.FillValue;
            Filler.FillType type = dlg.KindOfFill;
                var filler = Filler.CreateFiller(
                                 this.Document,
                                 new Position( this.Document, row, column ),
                                 -1,
                                 fillValue,
                                 type,
                                 Filler.FillDirection.Row );

            while ( filler.NumCells > 0 ) {
                filler.DoIt();
                Util.UpdateUI();
            }
			
			this.ShowDocument( row );
		}

		dlg.Destroy();
	}
	
	protected virtual void OnFillColumn(object sender, System.EventArgs e)
	{
		int row;
		int column;
		
		// Retrieve position
		this.GetCurrentCell( out row, out column );
		
		// Ask for filling
		var dlg = new DlgFill( this );
		dlg.Modal = true;
		
		if ( (Gtk.ResponseType) dlg.Run() == Gtk.ResponseType.Ok ) {
            string fillValue = dlg.FillValue;
            Filler.FillType type = dlg.KindOfFill;
            var filler = Filler.CreateFiller(
                this.Document,
                new Position( this.Document, row, column ),
                -1,
                fillValue,
                type,
                Filler.FillDirection.Column );

                System.Console.WriteLine( "NumCells: {0}", filler.NumCells );
            while ( filler.NumCells > 0 ) {
                filler.DoIt();
                Util.UpdateUI();
            }
			
			this.ShowDocument( row );
		}

		dlg.Destroy();
	}

	[GLib.ConnectBefore]
	protected void OnTableKeyPressed(object o, Gtk.KeyPressEventArgs args)
	{
        int rowIndex;
        int colIndex;

        // Do not "eat" the key, by default
        args.RetVal = false;

        // Get the current position, needed in both cases.
        this.GetCurrentCell( out rowIndex, out colIndex );

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
            } else {
                this.SetCurrentCell( rowIndex, colIndex, true );
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
			throw new ArgumentException( "invalid row to set: " + row.ToString(), "row" );
		}
		
		if( col < 0
		 || col >= ( this.Document.Data.NumColumns + NumFixedColumns ) )
		{
			throw new ArgumentException( "invalid column to set: " + col.ToString(), "col" );
		}
		
		// Find place
        Gtk.TreeIter itRow;
		table.GetIter( out itRow, new TreePath( new int[] { row } ) );
		
		// Set
		table.SetValue( itRow, col, value );
	}
	
	protected virtual void OnInsertFormula(object sender, System.EventArgs e)
	{
		int row;
		int col;
		var dlg = new DlgFormulae( this );
		
		// Get current position
		this.GetCurrentCell( out row, out col );
		
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
	private Menu popup;
}
	
}
