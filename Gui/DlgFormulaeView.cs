// Nombre: DlgFormulae.cs
// Fecha:  2011-01-17
// Autor:  baltasarq@gmail.com

using System;

namespace Colorado.Gui {
	public partial class DlgFormulae : Gtk.Dialog {
		public DlgFormulae(Gtk.Window parent) {
			this.Build();
			this.Parent = parent;
            this.TransientFor = parent;
            this.SetPosition( Gtk.WindowPosition.CenterOnParent );
            this.ShowAll();
			
			// Load formulas
			var model = new Gtk.ListStore( typeof( string ) );
			foreach(var x in Core.Formula.GetAllFormulas() ) {
				model.AppendValues( Core.Formula.GetFormulaName( x ) );
			}
			this.cbFormula.Model = model;
			this.cbFormula.Active = 0;
		}

        private void Build() {
            var vBox = new Gtk.VBox( false, 2 );

            // Prepare apply frame
            this.frmApply = new Gtk.Frame( "<b>Apply to cells...</b>" );
            ((Gtk.Label) this.frmApply.LabelWidget ).UseMarkup = true;
            this.rbUp = new Gtk.RadioButton( "Up" );
            this.rbDown = new Gtk.RadioButton( this.rbUp, "Down" );
            this.rbRight = new Gtk.RadioButton( this.rbUp, "Right" );
            this.rbLeft = new Gtk.RadioButton( this.rbUp, "Left" );
            vBox.PackStart( this.rbUp, true, true, 2 );
            vBox.PackStart( this.rbDown, true, true, 2 );
            vBox.PackStart( this.rbLeft, true, true, 2 );
            vBox.PackStart( this.rbRight, true, true, 2 );
            this.frmApply.Add( vBox );

            // Prepare formula frame
            this.frmFormula = new Gtk.Frame( "<b>Formula</b>" );
            ((Gtk.Label) this.frmFormula.LabelWidget ).UseMarkup = true;
            this.cbFormula = new Gtk.ComboBox( new string[] { "Average", "Sum" } );
            this.frmFormula.Add( this.cbFormula );

            // Buttons
            this.AddButton( Gtk.Stock.Cancel, Gtk.ResponseType.Cancel );
            this.AddButton( Gtk.Stock.Ok, Gtk.ResponseType.Ok );
            this.DefaultResponse = Gtk.ResponseType.Ok;

            // Layout
            this.VBox.PackStart( this.frmFormula, true, true, 5 );
            this.VBox.PackStart( this.frmApply, true, true, 5 );
            this.Resizable = false;
        }
		
        private Gtk.Frame frmApply;
        private Gtk.Frame frmFormula;
        private Gtk.ComboBox cbFormula;
        private Gtk.RadioButton rbUp;
        private Gtk.RadioButton rbDown;
        private Gtk.RadioButton rbLeft;
        private Gtk.RadioButton rbRight;
	}

}