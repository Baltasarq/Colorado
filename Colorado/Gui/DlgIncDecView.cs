// Colorado (c) 2015-2018 Baltasar MIT License <baltasarq@gmail.com>


namespace Colorado.Gui;


using System;


public partial class DlgIncDec : Gtk.Dialog {
	public DlgIncDec(Gtk.Window parent, DialogType type, Target target, int begin, int end, int max)
	{
		this.frmWhere = new Gtk.Frame( "<b>Position</b>" );
		this.frmValues = new Gtk.Frame( "<b>Values</b>" );
		this.rbBefore = new Gtk.RadioButton( "_Before" );
		this.rbAfter = new Gtk.RadioButton( this.rbBefore, "_After" );
		this.sbFrom = new Gtk.SpinButton( 1, 10, 1 );
		this.lblFrom = new Gtk.Label( "From:" );
		this.sbNumber = new Gtk.SpinButton( 1, 10, 1 );
		this.lblTo = new Gtk.Label( "Number:" );
		this.lblInfo = new Gtk.Label( "<b>Max.: </b>" );
		string info = "<b>Number of ";

		this.Build();

		// Prepare dialog
		this.Title = StrDialogType[ (int) type ] + " ";
		this.Title += StrTarget[ (int) target ];
		this.Icon = parent.Icon;
		this.Parent = parent;
		this.TransientFor = parent;
		this.SetPosition( Gtk.WindowPosition.CenterOnParent );
		this.ShowAll();

		// Set widgets
		this.frmWhere.Visible = ( type == DialogType.Insert );
		sbFrom.SetRange( 1, max );
		sbNumber.SetRange( 1, max - 1 );
		sbFrom.Value = begin;
		sbNumber.Value = ( end - begin ) + 1;

		// Info
		info += StrTarget[ (int) target ] + "</b>: ";
		info += Convert.ToString( max );

		if ( type == DialogType.Insert ) {
			info = "<b>Max. </b>" + info;
		}

		this.lblInfo.Markup = info;
	}

	void Build()
	{
		var hBoxFrames = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );
		var hBoxFrom = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );
		var hBoxTo = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );
		var hBoxInfo = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );
		var vBoxPosition = new Gtk.Box( Gtk.Orientation.Vertical, 2 );
		var vBoxValues = new Gtk.Box( Gtk.Orientation.Vertical, 2 );

		// Frames
		((Gtk.Label)this.frmWhere.LabelWidget).UseMarkup = true;
		((Gtk.Label) this.frmValues.LabelWidget ).UseMarkup = true;

		hBoxFrames.PackStart( this.frmWhere, false, false, 5 );
		hBoxFrames.PackStart( this.frmValues, true, true, 5 );

		// Position
		this.rbBefore.Active = true;
		vBoxPosition.PackStart( this.rbBefore, true, true, 5 );
		vBoxPosition.PackStart( this.rbAfter, true, true, 5 );
		this.frmWhere.Add( vBoxPosition );

		// From
		hBoxFrom.PackStart( this.lblFrom, false, false, 5 );
		hBoxFrom.PackStart( this.sbFrom, true, true, 5 );

		// To
		hBoxTo.PackStart( this.lblTo, false, false, 5 );
		hBoxTo.PackStart( this.sbNumber, true, true, 5 );

		// Values
		vBoxValues.PackStart( hBoxFrom, true, true, 5 );
		vBoxValues.PackStart( hBoxTo, true, true, 5 );
		this.frmValues.Add( vBoxValues );

		// Info
		this.lblInfo.UseMarkup = true;
		this.lblInfo.Justify = Gtk.Justification.Left;
		hBoxInfo.PackStart( this.lblInfo, false, false, 5 );

		this.AddButton( Gtk.Stock.Cancel, Gtk.ResponseType.Cancel );
		this.AddButton( Gtk.Stock.Ok, Gtk.ResponseType.Ok );
		this.DefaultResponse = Gtk.ResponseType.Ok;

		// Finish layout
		this.ContentArea.PackStart( hBoxInfo, false, false, 5 );
		this.ContentArea.PackStart( hBoxFrames, true, true, 5 );
		this.Resizable = false;
	}

	private readonly Gtk.Frame frmWhere;
	private readonly Gtk.Frame frmValues;
	private readonly Gtk.RadioButton rbBefore;
	private readonly Gtk.RadioButton rbAfter;
	private readonly Gtk.SpinButton sbFrom;
	private readonly Gtk.SpinButton sbNumber;
	private readonly Gtk.Label lblFrom;
	private readonly Gtk.Label lblTo;
	private readonly Gtk.Label lblInfo;
}
