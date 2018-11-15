// Colorado (c) 2015-2018 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Gui {	
	public partial class DlgFill : Gtk.Dialog {
		public DlgFill(Gtk.Window parent)
		{
			this.Build();
			
			this.Parent = parent;
			this.TransientFor = parent;
            this.SetPosition( Gtk.WindowPosition.CenterOnParent );
			this.Title = parent.Title;
			this.Icon = parent.Icon;
            this.ShowAll();
		}
				
		private void Build() {
			var vBoxMutation = new Gtk.VBox( false, 2 );
			var hBoxValue = new Gtk.HBox( false, 2 );

			// Frame mutation
			this.frmMutation = new Gtk.Frame( "<b>Value mutation</b>" );
            ((Gtk.Label) this.frmMutation.LabelWidget ).UseMarkup = true;
			this.rbFixed = new Gtk.RadioButton( "_Fixed" );
            this.rbIncrement = new Gtk.RadioButton( this.rbFixed, "_Increment" );
            this.rbDecrement = new Gtk.RadioButton( this.rbFixed, "_Decrement" );
			vBoxMutation.PackStart( this.rbFixed, true, true, 2 );
			vBoxMutation.PackStart( this.rbIncrement, true, true, 2 );
			vBoxMutation.PackStart( this.rbDecrement, true, true, 2 );
			this.frmMutation.Add( vBoxMutation );

			// Value
			this.lblValue = new Gtk.Label( "Value" );
			this.edValue = new Gtk.Entry();
			hBoxValue.PackStart( this.lblValue, false, false, 5 );
			hBoxValue.PackStart( this.edValue, false, false, 5 );

			// Layout
			this.VBox.PackStart( hBoxValue, true, true, 5 );
			this.VBox.PackStart( this.frmMutation, true, true, 5 );

			// Buttons
			this.AddButton( Gtk.Stock.Cancel, Gtk.ResponseType.Cancel );
			this.AddButton( Gtk.Stock.Ok, Gtk.ResponseType.Ok );
			this.DefaultResponse = Gtk.ResponseType.Ok;
            this.Resizable = false;
		}

		private Gtk.Frame frmMutation;
		private Gtk.RadioButton rbFixed;
		private Gtk.RadioButton rbIncrement;
		private Gtk.RadioButton rbDecrement;
		private Gtk.Label lblValue;
		private Gtk.Entry edValue;
	}
}

