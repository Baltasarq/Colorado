// Colorado (c) 2015-2018-2025 Baltasar MIT License <baltasarq@gmail.com>


namespace Colorado.Gui;


using Core;


public partial class DlgProperties : Gtk.Dialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Colorado.Gui.DlgProperties"/> class.
    /// </summary>
    /// <param name="parent">Parent.</param>
    /// <param name="doc">Document.</param>
    public DlgProperties(Gtk.Window parent, CsvDocument doc)
    {
        var minSize = new Gdk.Geometry {
            MinWidth = 400,
            MinHeight = 300
        };

        this.document = doc;
        this.listHeaders = new Gtk.TreeView();
        this.frmHeaders = new Gtk.Frame( "<b>Headers</b>" );
        this.sbColumns = new Gtk.SpinButton( 1, int.MaxValue, 1 );
        this.sbRows = new Gtk.SpinButton( 0, int.MaxValue, 1 );
        this.cmbDelimiter = Gtk.ComboBoxText.NewWithEntry();
        this.cmbDecimalMark = new Gtk.ComboBoxText();
        this.cbSurroundWithDoubleQuotes = new Gtk.CheckButton( "Surround text with double quotes" );
        this.cbFirstRowForHeaders = new Gtk.CheckButton( "First row contains the headers" );
        this.frmProperties = new Gtk.Frame( "<b>Properties</b>" );
        this.lblRows = new Gtk.Label( "Rows" );
        this.lblColumns = new Gtk.Label( "Columns" );
        this.lblDelimiter = new Gtk.Label( "Delimiter" );
        this.lblDecimalMark = new Gtk.Label( "Decimal mark" );

        // Combobox activation
        this.cmbDelimiter.Changed += (o, e) => this.OnDelimiterChosen();

        // Prepare dialog
        this.Build();
        this.Title = parent.Title + " properties";
        this.Icon = parent.Icon;
        this.Parent = parent;
        this.TransientFor = parent;
        this.SetPosition(Gtk.WindowPosition.CenterOnParent);
        this.WindowPosition = Gtk.WindowPosition.CenterOnParent;
        this.SetDefaultSize(minSize.MinHeight, minSize.MinWidth);
        this.SetGeometryHints(this, minSize, Gdk.WindowHints.MinSize);

        // Sync info
        this.sbRows.Value = document.Data.NumRows;
        this.sbColumns.Value = document.Data.NumColumns;
        this.cbSurroundWithDoubleQuotes.Active = document.SurroundText;
        this.cbFirstRowForHeaders.Active = document.Data.FirstRowContainsHeaders;
        this.UpdateColumnsData();
        this.ShowAll();
    }

    private void Build()
    {
        var hBox = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );

        this.BuildHeadersFrame();
        this.BuildPropertiesFrame();

        // Buttons
        this.AddButton( Gtk.Stock.Apply, Gtk.ResponseType.Apply );
        this.AddButton( Gtk.Stock.Close, Gtk.ResponseType.Close );
        this.DefaultResponse = Gtk.ResponseType.Close;

        // Layout
        hBox.PackStart (this.frmHeaders, true, true, 5 );
        hBox.PackStart (this.frmProperties, false, false, 5 );
        this.ContentArea.PackStart( hBox, true, true, 5 );
    }

    private void BuildHeadersFrame()
    {
        var swScroll = new Gtk.ScrolledWindow();

        ((Gtk.Label)this.frmHeaders.LabelWidget).UseMarkup = true;
        this.listHeaders.EnableSearch = false;
        swScroll.Add( this.listHeaders );
        this.frmHeaders.Add( swScroll );

        // Set the treeview
        var column = new Gtk.TreeViewColumn();
        var cell = new Gtk.CellRendererText();
        Gtk.ListStore listStore = new Gtk.ListStore( typeof( string ) );

        column.Title = "";
        column.PackStart( cell, true );
        cell.Editable = true;
        column.AddAttribute( cell, "text", 0 );
        cell.Edited += OnHeaderEdited;

        this.listHeaders.AppendColumn( column );
        this.listHeaders.Model = listStore;
        this.listHeaders.EnableGridLines = Gtk.TreeViewGridLines.Horizontal;
    }

    private void BuildPropertiesFrame()
    {
        var vBox = new Gtk.Box( Gtk.Orientation.Vertical, 2 );
        var hBoxRows = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );
        var hBoxColumns = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );
        var hBoxDelimiter = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );
        var hBoxDecimalMark = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );

        // Rows
        hBoxRows.PackStart( this.lblRows, false, false, 5 );
        hBoxRows.PackStart( this.sbRows, true, true, 5 );

        // Columns
        hBoxColumns.PackStart( this.lblColumns, false, false, 5 );
        hBoxColumns.PackStart( this.sbColumns, true, true, 5 );

        // Delimiter
        hBoxDelimiter.PackStart( this.lblDelimiter, false, false, 5 );
        hBoxDelimiter.PackStart( this.cmbDelimiter, true, true, 5 );

        // Decimal mark
        hBoxDecimalMark.PackStart( this.lblDecimalMark, false, false, 5 );
        hBoxDecimalMark.PackStart( this.cmbDecimalMark, true, true, 5 );

        // Layout
        vBox.PackStart( hBoxRows, true, true, 5 );
        vBox.PackStart( hBoxColumns, true, true, 5 );
        vBox.PackStart( hBoxDelimiter, true, false, 5 );
        vBox.PackStart( hBoxDecimalMark, true, false, 5 );
        vBox.PackStart( this.cbFirstRowForHeaders, true, true, 5 );
        vBox.PackStart( this.cbSurroundWithDoubleQuotes, true, true, 5 );
        ( (Gtk.Label)this.frmProperties.LabelWidget ).UseMarkup = true;
        this.frmProperties.Add( vBox );

        // Add delimiters to its combo
        foreach (string delimiter in Delimiter.PredefinedDelimiterNames) {
            cmbDelimiter.AppendText( delimiter );
        }

        this.cmbDelimiter.Entry.Text = Delimiter.GetName( this.document.DelimiterValue );

        // Add decimal separators to its combo
        foreach (char separator in DecimalMark.DecimalSeparatorChar) {
            cmbDecimalMark.AppendText( separator.ToString() );
        }

        this.cmbDecimalMark.Active = (int) this.document.DecimalSeparator;
    }


    private readonly Gtk.TreeView listHeaders;
    private readonly Gtk.Label lblRows;
    private readonly Gtk.Label lblColumns;
    private readonly Gtk.Label lblDelimiter;
    private readonly Gtk.Label lblDecimalMark;
    private readonly Gtk.CheckButton cbSurroundWithDoubleQuotes;
    private readonly Gtk.CheckButton cbFirstRowForHeaders;
    private readonly Gtk.SpinButton sbRows;
    private readonly Gtk.SpinButton sbColumns;
    private readonly Gtk.Frame frmHeaders;
    private readonly Gtk.Frame frmProperties;
    private readonly Gtk.ComboBoxText cmbDelimiter;
    private readonly Gtk.ComboBoxText cmbDecimalMark;
}
