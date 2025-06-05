// Colorado (c) 2015-2018 Baltasar MIT License <baltasarq@gmail.com>


namespace Colorado.Gui;


using Colorado.Core;


public partial class DlgProperties {
    public void UpdateColumnsData()
    {
        if ( this.listHeaders is not null ) {
            var listStore =  (Gtk.ListStore) listHeaders.Model;

            // Insert data
            listStore.Clear();
            foreach(ColumnInfo colInfo in document.Data.ColumnInfo) {
                listStore.AppendValues( colInfo.Header );
            }
        }
    }

    private void OnHeaderEdited(object sender, Gtk.EditedArgs args)
    {
        if ( this.listHeaders is not null ) {
            int row;
            var rowPath = new Gtk.TreePath( args.Path );

            // Update GUI
            listHeaders.Model.GetIter( out Gtk.TreeIter rowPointer, rowPath );
            row = rowPath.Indices[ 0 ];
            listHeaders.Model.SetValue( rowPointer, 0, args.NewText );

            // Update info in document
            document.Data.ColumnInfo[ row ].Header = args.NewText;
            document.Changed = true;
        }
    }

    private void OnDelimiterChosen()
    {
        this.cmbDelimiter.Entry.Text = this.cmbDelimiter.ActiveText;
    }

    public Delimiter Delimiter => new Delimiter( this.cmbDelimiter.Entry.Text.Trim() );

    public bool SurroundText => this.cbSurroundWithDoubleQuotes.Active;

    public bool FirstRowForHeaders => this.cbFirstRowForHeaders.Active;

    public int NumColumns {
        get {
            return (int) this.sbColumns.Value;
        }
        set {
            this.sbColumns.Value = value;
        }
    }

    public int NumRows {
        get {
            return (int) this.sbRows.Value;
        }
        set {
            this.sbRows.Value = value;
        }
    }

    public DecimalMark.DecimalSeparator DecimalMarkValue
                    => (DecimalMark.DecimalSeparator) this.cmbDecimalMark.Active;

    private readonly CsvDocument document;
}
