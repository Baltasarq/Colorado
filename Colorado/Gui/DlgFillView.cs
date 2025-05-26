// Colorado (c) 2015-2018 Baltasar MIT License <baltasarq@gmail.com>


namespace Colorado.Gui;


public partial class DlgFill : Gtk.Dialog {
	public DlgFill(Gtk.Window parent)
	{
		this.lblValue = new Gtk.Label( "Value" );
		this.edValue = new Gtk.Entry();
		this.rbFixed = new Gtk.RadioButton( "_Fixed" );
		this.frmMutation = new Gtk.Frame( "<b>Value mutation</b>" );
		this.rbIncrement = new Gtk.RadioButton( this.rbFixed, "_Increment" );
		this.rbDecrement = new Gtk.RadioButton( this.rbFixed, "_Decrement" );
		this.Build();

		this.Parent = parent;
		this.TransientFor = parent;
		this.SetPosition( Gtk.WindowPosition.CenterOnParent );
		this.Title = parent.Title;
		this.Icon = parent.Icon;
		this.ShowAll();
	}

	private void Build()
	{
		var vBoxMutation = new Gtk.Box( Gtk.Orientation.Vertical, 2 );
		var hBoxValue = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );

		// Frame mutation
		((Gtk.Label) this.frmMutation.LabelWidget ).UseMarkup = true;
		vBoxMutation.PackStart( this.rbFixed, true, true, 2 );
		vBoxMutation.PackStart( this.rbIncrement, true, true, 2 );
		vBoxMutation.PackStart( this.rbDecrement, true, true, 2 );
		this.frmMutation.Add( vBoxMutation );

		// Value
		hBoxValue.PackStart( this.lblValue, false, false, 5 );
		hBoxValue.PackStart( this.edValue, false, false, 5 );

		// Layout
		this.ContentArea.PackStart( hBoxValue, true, true, 5 );
		this.ContentArea.PackStart( this.frmMutation, true, true, 5 );

		// Buttons
		this.AddButton( Gtk.Stock.Cancel, Gtk.ResponseType.Cancel );
		this.AddButton( Gtk.Stock.Ok, Gtk.ResponseType.Ok );
		this.DefaultResponse = Gtk.ResponseType.Ok;
		this.Resizable = false;
	}

	private readonly Gtk.Frame frmMutation;
	private readonly Gtk.RadioButton rbFixed;
	private readonly Gtk.RadioButton rbIncrement;
	private readonly Gtk.RadioButton rbDecrement;
	private readonly Gtk.Label lblValue;
	private readonly Gtk.Entry edValue;
}
