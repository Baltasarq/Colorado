// Nombre: DlgFormulae.cs
// Fecha:  2011-01-17
// Autor:  baltasarq@gmail.com

using System;

using Colorado.Core;

namespace Colorado.Gui {
	
	public partial class DlgFormulae : Gtk.Dialog
	{
		public DlgFormulae(Gtk.Window parent)
		{
			this.Build();
			this.Parent = parent;
			
			// Load formulas
			var model = new Gtk.ListStore( typeof( string ) );
			foreach(var x in Core.Formula.GetAllFormulas() ) {
				model.AppendValues( Core.Formula.GetFormulaName( x ) );
			}
			this.cbFormula.Model = model;
			this.cbFormula.Active = 0;
		}
		
		public int Formula {
			get { return Math.Max( this.cbFormula.Active, 0 ); }
		}
		
		public new Position.Direction Direction {
			get {
				Position.Direction toret = Position.Direction.Left;
				
				if ( this.rbDown.Active ) {
					toret = Position.Direction.Down;
				}
				else
				if ( this.rbUp.Active ) {
					toret = Position.Direction.Up;
				}
				else
				if ( this.rbRight.Active ) {
					toret = Position.Direction.Right;
				}
				
				return toret;
			}
		}
	}

}