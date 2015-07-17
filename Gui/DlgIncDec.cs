
using System;
using System.Collections.ObjectModel;

namespace Colorado.Gui {
	public partial class DlgIncDec : Gtk.Dialog {
		public enum DialogType { Insert, Erase };
		public enum WherePosition { After, Before };
		public enum Target { Rows, Columns };
		
		public static readonly ReadOnlyCollection<string> StrDialogType = new ReadOnlyCollection<string>(
			new string[]{ "Insert", "Erase" }
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
			lblValues.Markup = "<b>Max. </b>" + info;
		}

		/// <summary>
		/// Where to insert, after or before the "from" row or column number.
		/// </summary>
		/// <value>The row or column index.</value>
		public WherePosition Where {
			get {
				var toret = WherePosition.After;

				if ( this.rbBefore.Active ) {
					toret = WherePosition.Before;
				}

				return toret;
			}
		}

		/// <summary>
		/// The row or column to insert or delete from
		/// </summary>
		/// <value>The row or column index, as a number.</value>
		public int From {
			get { return sbFrom.ValueAsInt; }
		}

		/// <summary>
		/// The row or column number to insert or delete.
		/// </summary>
		/// <value>The number of items to insert or delete.</value>
		public int Number {
			get { return sbNumber.ValueAsInt; }
		}
	}
}
