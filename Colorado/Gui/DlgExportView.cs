// Colorado (c) 2015-2018-2025 Baltasar MIT License <baltasarq@gmail.com>


namespace Colorado.Gui;
using Colorado.Core;

using System.Collections.ObjectModel;
using System.Linq;


public partial class DlgExport : Gtk.Dialog
{
    public DlgExport(Gtk.Window parent, CsvDocument doc)
    {
        document = doc;
        lastFileName = ( (MainWindow) parent ).LastFileName;
        formatOptions = Exporter.GetAllExporters().ToList<Exporter>()
                                                .Select( e => e.Id ).ToList();

        this.Title = parent.Title + " export";
        this.Icon = parent.Icon;
        this.Parent = parent;
        this.TransientFor = parent;
        this.SetPosition( Gtk.WindowPosition.CenterOnParent );

        this.fieldChecks = new Gtk.CheckButton[ document.Data.ColumnInfo.Length ];
        this.frmFields = new Gtk.Frame( "<b>Fields</b>" );
        this.swScroll = new Gtk.ScrolledWindow();
        this.edFile = new Gtk.Entry();
        this.btSaveAs = new Gtk.Button( Gtk.Stock.SaveAs );
        this.btSaveAs.Clicked += (obj, args) => this.OnSaveAs();
        this.lblFile = new Gtk.Label( "File:" );
        this.lblOutput = new Gtk.Label( "Output format:" );
        this.frmFile = new Gtk.Frame( "<b>File</b>" );
        this.frmDocOptions = new Gtk.Frame( "<b>Document options</b>" );
        this.cbRowNumbers = new Gtk.CheckButton( "Include row numbers" );
        this.cbTableBorders = new Gtk.CheckButton( "Include table borders" );
        this.lblDelimiter = new Gtk.Label( "Delimiter:" );
        this.chkQuotes = new Gtk.CheckButton( "Enclose text with quotes" );
        this.frmCsvOptions = new Gtk.Frame( "<b>Csv options</b>" );

        // Delimiter
        this.cmbDelimiter = Gtk.ComboBoxText.NewWithEntry();
        this.cmbDelimiter.Entry.IsEditable = true;

        foreach (string delimiter in Delimiter.PredefinedDelimiterNames) {
            this.cmbDelimiter.AppendText( delimiter );
        }

        this.cmbDelimiter.Active = 0;

        // Output
        this.cmbOutput = new Gtk.ComboBoxText();

        foreach (string option in formatOptions) {
            this.cmbOutput.AppendText( option );
        }

        this.cmbOutput.Active = 0;
        this.cmbOutput.Changed += (obj, args) => this.OnOutputFormatChanged();

        // Build
        this.Build();
        this.ShowAll();

        // Sync dialog
        this.OnOutputFormatChanged();
    }

    void Build()
    {
        var hBoxOptions = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );

        this.BuildDocOptions();
        this.BuildCsvOptions();
        this.BuildFileFrame();

        // Options
        hBoxOptions.PackStart( this.frmDocOptions, true, true, 5 );
        hBoxOptions.PackStart( this.frmCsvOptions, true, true, 5 );

        // Fields
        this.BuildChecksForFields();

        // Buttons
        this.AddButton( Gtk.Stock.Cancel, Gtk.ResponseType.Cancel) ;
        this.AddButton( Gtk.Stock.Ok, Gtk.ResponseType.Ok );
        this.DefaultResponse = Gtk.ResponseType.Ok;

        // Layout
        this.ContentArea.PackStart( this.frmFile, false, false, 5 );
        this.ContentArea.PackStart( hBoxOptions, false, false, 5 );
        this.ContentArea.PackStart( this.frmFields, true, true, 5 );
    }

    void BuildChecksForFields()
    {
        // Add fields
        var vbox = new Gtk.Box( Gtk.Orientation.Vertical, 2 );

        for (int i = 0; i < document.Data.ColumnInfo.Length; ++i) {
            fieldChecks[i] = new Gtk.CheckButton(
                                    document.Data.ColumnInfo[i].Header )
            {
                Visible = true,
                Active = true
            };
            vbox.Add( fieldChecks[ i ] );
        }

        // Layout
        ( (Gtk.Label) this.frmFields.LabelWidget ).UseMarkup = true;
        this.swScroll.Add( vbox );
        this.frmFields.Add( this.swScroll );
    }

    void BuildFileFrame()
    {
        var vBoxFile = new Gtk.Box( Gtk.Orientation.Vertical, 2 );
        var hBoxFileName = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );
        var hBoxOutput = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );

        // File name
        hBoxFileName.PackStart( this.lblFile, false, false, 5 );
        hBoxFileName.PackStart( this.edFile, true, true, 5 );
        hBoxFileName.PackStart( this.btSaveAs, false, false, 5 );

        // Output format
        hBoxOutput.PackStart( this.lblOutput, false, false, 5 );
        hBoxOutput.PackStart( this.cmbOutput, true, true, 5 );

        // Layout
        vBoxFile.PackStart( hBoxFileName, false, false, 5);
        vBoxFile.PackStart( hBoxOutput, false, false, 5);
        ( (Gtk.Label) this.frmFile.LabelWidget ).UseMarkup = true;
        this.frmFile.Add( vBoxFile );
    }

    void BuildDocOptions()
    {
        var vBoxDocOptions = new Gtk.Box( Gtk.Orientation.Vertical, 2 );

        // Doc options
        ( (Gtk.Label) this.frmDocOptions.LabelWidget).UseMarkup = true;
        vBoxDocOptions.PackStart (this.cbRowNumbers, false, false, 5 );
        vBoxDocOptions.PackStart( this.cbTableBorders, false, false, 5 );

        this.frmDocOptions.Add( vBoxDocOptions );
    }

    void BuildCsvOptions()
    {
        var hBoxDelimiter = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );
        var vBoxOptions = new Gtk.Box( Gtk.Orientation.Vertical, 2 );

        // Delimiter
        hBoxDelimiter.PackStart( this.lblDelimiter, false, false, 5 );
        hBoxDelimiter.PackStart( this.cmbDelimiter, true, true, 5 );

        // Set options honoring current document
        this.cmbDelimiter.Entry.Text = Delimiter.GetName( this.document.DelimiterValue );
        chkQuotes.Active = this.document.SurroundText;

        // Layout
        vBoxOptions.PackStart( hBoxDelimiter, expand: false, fill: true, 2 );
        vBoxOptions.PackStart( this.chkQuotes, expand: false, fill: true, 2 );
        ( (Gtk.Label) this.frmCsvOptions.LabelWidget) .UseMarkup = true;
        this.frmCsvOptions.Add( vBoxOptions );
    }

    readonly List<string> formatOptions;

    readonly Gtk.Frame frmFields;
    readonly Gtk.Frame frmDocOptions;
    readonly Gtk.Frame frmCsvOptions;
    readonly Gtk.Frame frmFile;
    readonly Gtk.Label lblFile;
    readonly Gtk.Label lblOutput;
    readonly Gtk.Entry edFile;
    readonly Gtk.Button btSaveAs;
    readonly Gtk.ScrolledWindow swScroll;
    readonly Gtk.CheckButton cbRowNumbers;
    readonly Gtk.CheckButton cbTableBorders;
    readonly Gtk.Label lblDelimiter;
    readonly Gtk.CheckButton chkQuotes;
    readonly Gtk.CheckButton[] fieldChecks;
    readonly Gtk.ComboBoxText cmbOutput;
    readonly Gtk.ComboBoxText cmbDelimiter;
}
