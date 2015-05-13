
using System;
using System.Collections.ObjectModel;

namespace Colorado.Gui {
	public partial class DlgIncDec : Gtk.Dialog {
		public enum DialogType { Add, Insert, Erase };
		public enum Target { Rows, Columns };
		
		public static readonly ReadOnlyCollection<string> StrDialogType = new ReadOnlyCollection<string>(
			new string[]{ "Add", "Insert", "Erase" }
		);
		
		public static readonly ReadOnlyCollection<string> StrTarget = new ReadOnlyCollection<string>(
			new string[]{ "Rows", "Columns" }
		);
		
		public DlgIncDec(Gtk.Window parent, DialogType type, Target target, int begin, int max)
		{
			string info = "<b>Number of ";
			this.Build ();
			
			// Prepare dialog
			this.Title = StrDialogType[ (int) type ] + " ";
			this.Title += StrTarget[ (int) target ];
			this.Icon = parent.Icon;
			this.Parent = parent;
			this.TransientFor = parent;
			this.SetPosition( Gtk.WindowPosition.CenterOnParent );
			
			// Set widgets
			sbFrom.SetRange( 1, max );
			sbNumber.SetRange( 1, int.MaxValue );
			sbFrom.Value = begin;
			sbNumber.Value = 1;
			info += StrTarget[ (int) target ] + "</b>: ";
			info += Convert.ToString( max );
			lblValues.Markup = info;
			
			// When we are adding...
			if ( type == DlgIncDec.DialogType.Add ) {
				sbFrom.Sensitive = false;
				lblValues.Markup = "<b>Max. </b>" + info;
			}
		}
		
		public int From {
			get { return sbFrom.ValueAsInt; }
		}
		
		public int Number {
			get { return sbNumber.ValueAsInt; }
		}
	}
}
