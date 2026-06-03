// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>


namespace Colorado.Gui;


using Core;


public class SpreadSheet: Gtk.Box {
    public const int NumFixedColumns = 1;
    public const int NumFixedRows = 1;

    public SpreadSheet(Gtk.Window owner, CsvDocument doc)
        :this(owner, new Gtk.TreeView(), doc)
    {
    }

    public SpreadSheet(Gtk.Window owner, Gtk.TreeView tv, CsvDocument doc)
        :base( Gtk.Orientation.Vertical, 5 )
    {
        this.Owner = owner;
        this.Document = doc;
        this.TreeView = tv;
        this.TreeView.EnableSearch = false;
        this.TreeView.Selection.Mode = Gtk.SelectionMode.Multiple;
        this.TreeView.KeyPressEvent +=
                (_, args) => this.OnTableKeyPressed( args );
        this.Document.ClientUpdater += this.UpdateFromData;
    }

    public void SetOnClick(Action<Gtk.ButtonReleaseEventArgs> handler)
    {
        this.TreeView.ButtonReleaseEvent += (_, args) => handler( args );
    }

    public string FileName {
        get => this.Document.FileName;
        set => this.Document.FileName = value;
    }

    public DecimalMark.DecimalSeparator DecimalSeparator {
        get => this.Document.DecimalSeparator;
        set => this.Document.DecimalSeparator = value;
    }

    public int NumRows {
        get => this.Document.Data.NumRows;
        set => this.Document.Data.NumRows = value;
    }

    public int NumColumns {
        get => this.Document.Data.NumColumns;
        set => this.Document.Data.NumColumns = value;
    }

    public bool SurroundText {
        get => this.Document.SurroundText;
        set => this.Document.SurroundText = value;
    }

    public string DelimiterValue => this.Document.DelimiterValue;
    public bool Changed => this.Document.Changed;
    public bool HasName => this.Document.HasName;

    public Delimiter Delimiter {
        get => this.Document.Delimiter;
        set => this.Document.Delimiter = value;
    }

    public bool FirstRowContainsHeaders
    {
        get => this.Document.Data.FirstRowContainsHeaders;
        set => this.Document.Data.FirstRowContainsHeaders = value;
    }

    public void CleanRows(int rowBeg, int rowEnd, int colStart = 0)
    {
        this.Document.Data.CleanRows( rowBeg, rowEnd );
    }

    public void CleanColumns(int colBeg, int colEnd, int rowStart = 0)
    {
        this.Document.Data.CleanColumns( colBeg, colEnd, rowStart );
    }

    [GLib.ConnectBefore]
    void OnTableKeyPressed(Gtk.KeyPressEventArgs args)
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
                args.RetVal = true;         // Eat the TAB
            }
        }

        return;
    }

    public void Show(int numRow)
    {
        this.TreeView.Hide();

        // Create liststore
        var types = new Type[ this.Document.Data.NumColumns + 1 ];
        for(int typeNumber = 0; typeNumber < types.Length; ++typeNumber) {
            types[ typeNumber ] = typeof( string );
        }
        Gtk.ListStore listStore = new Gtk.ListStore( types );
        this.TreeView.Model = listStore;

        // Delete existing columns
        while( this.TreeView.Columns.Length > 0 ) {
            this.TreeView.RemoveColumn( this.TreeView.Columns[ 0 ] );
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
        this.TreeView.AppendColumn( column );

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

            this.TreeView.AppendColumn( column );
        }

        // Insert data
        var row = new List<string>();
        for (int i = 0; i < this.Document.Data.NumRows; ++i) {
            row.Clear();
            row.AddRange( this.Document.Data[ i ] );
            row.Insert( 0, Convert.ToString( i + 1 ) );

            listStore.AppendValues( row.ToArray() );
        }

        this.TreeView.EnableGridLines = Gtk.TreeViewGridLines.Both;
        this.TreeView.HeadersClickable = true;
        this.TreeView.SetCursor(
            new Gtk.TreePath( new int[]{ numRow } ),
            this.TreeView.Columns[ 1 ],
            false
        );

        this.TreeView.Show();
    }

    /// <summary>
    /// Sets the current cell to the position marked in rowIndex, colIndex
    /// </summary>
    /// <param name="rowIndex">The row index.</param>
    /// <param name="colIndex">The column index.</param>
    /// <param name="edit">Start editing the cell if set to <c>true</c>.</param>
    public void SetCurrentCell(int rowIndex, int colIndex, bool edit = false)
    {
        if ( this.Document is not null ) {
            var rowPath = new Gtk.TreePath( new int[]{ rowIndex } );
            Gtk.TreeViewColumn colPath = this.TreeView.Columns[ colIndex ];

            this.TreeView.ScrollToCell(
                rowPath,
                colPath,
                false,
                (float) 0.0,
                (float) 0.0
            );

            this.TreeView.SetCursor( rowPath, colPath, edit );
        }

        this.TreeView.GrabFocus();
        return;
    }

    public void GetCurrentCell(out int row, out int col)
    {
        // Convert path in row and rowPointer
        this.TreeView.GetCursor( out Gtk.TreePath rowPath,
                                out Gtk.TreeViewColumn colPath );

        if ( rowPath is not null
          && colPath is not null )
        {
            this.TreeView.Model.GetIter( out Gtk.TreeIter rowPointer, rowPath );
            row = rowPath.Indices[ 0 ];

            // Find out the column order
            for(col = 0; col < this.TreeView.Columns.Length; ++col) {
                if ( this.TreeView.Columns[ col ] == colPath ) {
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
        int rowIndex;
        int colIndex;

        try {
            // Get current position
            Gtk.TreePath rowPath = new Gtk.TreePath( args.Path );

            // Convert path in row and rowPointer
            this.TreeView.Model.GetIter( out Gtk.TreeIter rowPointer, rowPath );
            rowIndex = rowPath.Indices[ 0 ];

            // Find out the column order
            this.TreeView.GetCursor( out rowPath, out Gtk.TreeViewColumn colPath );
            for(colIndex = 0; colIndex < this.TreeView.Columns.Length; ++colIndex)
            {
                if ( this.TreeView.Columns[ colIndex ] == colPath ) {
                    break;
                }
            }

            // Store data
            try {
                this.Document.Data[ rowIndex, colIndex - NumFixedColumns ] = args.NewText;
                this.TreeView.Model.SetValue( rowPointer, colIndex, args.NewText );

                if ( args.NewText == "" ) {
                    this.Document.FormulaManager.RemoveFormula( rowIndex, colIndex - NumFixedColumns );
                }

                this.Document.Changed = true;
            } catch(Exception exc) {
                GtkUtil.Misc.MsgError( this.Owner, AppInfo.Name, "Passing coordinates to data:\n"
                    + "Rows: " + this.Document.Data.NumRows + "\n"
                    + "Columns: " + this.Document.Data.NumColumns + "\n"
                    + exc.Message
                );
            }
        } catch(Exception exc)
        {
            GtkUtil.Misc.MsgError( this.Owner, AppInfo.Name, exc.Message );
        }

        return;
    }

    public void UpdateHeaders()
    {
        for(int j = 0; j < this.Document.Data.ColumnInfo.Length; ++j) {
            this.TreeView.Columns[ j + NumFixedColumns ].Title =
                this.Document.Data.ColumnInfo[ j ].Header;
        }
    }

    public void FindText(int rowBegin, string txtToFind)
    {
        this.TreeView.GrabFocus();

        if ( this.Document is not null ) {
            for(int i = rowBegin; i < this.Document.Data.NumRows; ++i) {
                for(int j = 0; j < this.Document.Data.NumColumns; ++j) {
                    var cell = this.Document.Data[ i, j ].Trim().ToLower();

                    if ( cell.Contains( txtToFind ) ) {
                        int[] path = { i };
                        this.TreeView.SetCursor(
                                        new Gtk.TreePath( path ),
                                        this.TreeView.Columns[ j + 1 ], false );
                        goto End;
                    }
                }
            }
        }

        End:
        return;
    }

    public void RefreshRows(int begin, int end, int col = 0)
    {
        // Run over all rows
        for (int i = begin; i <= end; ++i) {
            // Get an iterator for this row
            Gtk.TreePath rowPath = new Gtk.TreePath( Convert.ToString( i ) );
            this.TreeView.Model.GetIter( out Gtk.TreeIter rowPointer, rowPath );

            // Refresh row
            for (int j = col; j < this.Document.Data.NumColumns; ++j) {
                // do It for each cell
                this.TreeView.Model.SetValue( rowPointer, j + 1,
                                Convert.ToString(this.Document.Data[i, j])
                );
            }
        }

        return;
    }

    /// <summary>
    /// Determines the selected rows.
    /// </summary>
    /// <param name="first">To be filled with the first row.</param>
    /// <param name="last">To be filled with the last row.</param>
    public void DetermineSelectedRows(out int first, out int last)
    {
        var orgList = (Gtk.ListStore) this.TreeView.Model;
        Gtk.TreeSelection selected = this.TreeView.Selection;
        Gtk.TreePath[] rowPaths = selected.GetSelectedRows();

        if ( rowPaths.Length > 0 ) {
            first = rowPaths[ 0 ].Indices[ 0 ] + NumFixedRows;
            last = rowPaths[ ^1 ].Indices[ 0 ] + NumFixedRows;
        } else {
            first = last = -1;
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
    public void UpdateFromData(int row, int col, string value)
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
    public void Set(int row, int col, string value)
    {
        if ( this.Document is not null ) {
            var table = (Gtk.ListStore) this.TreeView.Model;

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
    public string Get(int row, int col)
    {
        if ( this.Document is not null ) {
            var table = (Gtk.ListStore) this.TreeView.Model;

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

    /// <summary>
    /// Gets the document being edited.
    /// </summary>
    /// <value>The document.</value>
    public CsvDocument Document {
        get;
    }

    public Gtk.TreeView TreeView {
        get; private set;
    }

    public Gtk.Window Owner {
        get; private set;
    }
}
