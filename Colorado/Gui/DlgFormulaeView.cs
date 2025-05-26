// Colorado (c) 2015-2018-2025 Baltasar MIT License <baltasarq@gmail.com>


namespace Colorado.Gui;


public partial class DlgFormulae : Gtk.Dialog {
    public DlgFormulae(Gtk.Window parent)
    {
        this.Parent = parent;
        this.TransientFor = parent;
        this.frmFormula = new Gtk.Frame( "<b>Formula</b>" );
        this.cbFormula = new Gtk.ComboBoxText();
        this.frmApply = new Gtk.Frame( "<b>Apply to cells...</b>" );
        this.rbUp = new Gtk.RadioButton( "Up" );
        this.rbDown = new Gtk.RadioButton( this.rbUp, "Down" );
        this.rbRight = new Gtk.RadioButton( this.rbUp, "Right" );
        this.rbLeft = new Gtk.RadioButton( this.rbUp, "Left" );

        this.Build();
        this.SetPosition( Gtk.WindowPosition.CenterOnParent );
        this.ShowAll();

        // Load formulas
        foreach(Type formula in Core.Formula.GetAllFormulas() ) {
            this.cbFormula.AppendText( Core.Formula.GetFormulaName( formula ) );
        }

        this.cbFormula.Active = 0;
    }

    private void Build() {
        var vBox = new Gtk.Box( Gtk.Orientation.Vertical, 2 );

        // Prepare apply frame
        ((Gtk.Label) this.frmApply.LabelWidget ).UseMarkup = true;
        vBox.PackStart( this.rbUp, true, true, 2 );
        vBox.PackStart( this.rbDown, true, true, 2 );
        vBox.PackStart( this.rbLeft, true, true, 2 );
        vBox.PackStart( this.rbRight, true, true, 2 );
        this.frmApply.Add( vBox );

        // Prepare formula frame
        ((Gtk.Label) this.frmFormula.LabelWidget ).UseMarkup = true;
        this.frmFormula.Add( this.cbFormula );

        // Buttons
        this.AddButton( Gtk.Stock.Cancel, Gtk.ResponseType.Cancel );
        this.AddButton( Gtk.Stock.Ok, Gtk.ResponseType.Ok );
        this.DefaultResponse = Gtk.ResponseType.Ok;

        // Layout
        this.ContentArea.PackStart( this.frmFormula, true, true, 5 );
        this.ContentArea.PackStart( this.frmApply, true, true, 5 );
        this.Resizable = false;
    }

    private readonly Gtk.Frame frmApply;
    private readonly Gtk.Frame frmFormula;
    private readonly Gtk.ComboBoxText cbFormula;
    private readonly Gtk.RadioButton rbUp;
    private readonly Gtk.RadioButton rbDown;
    private readonly Gtk.RadioButton rbLeft;
    private readonly Gtk.RadioButton rbRight;
}
