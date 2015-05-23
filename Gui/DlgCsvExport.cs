using Gtk;
using System;
using Core = Colorado.Core;

namespace Colorado.Gui
{
	public partial class DlgCsvExport : Gtk.Dialog {
		public DlgCsvExport(Gtk.Window parent, Core.CsvDocument doc)
		{
			this.Build();
			
			this.Title = parent.Title + " export as CSV";
			this.Icon = parent.Icon;
			this.Parent = parent;
			this.TransientFor = parent;
			
			// Add items to the combo
			foreach(string delimiter in Core.Delimiter.PredefinedDelimiterNames) {
				this.cmbDelimiter.AppendText( delimiter );
			}
			
			// Set options honoring current document
			this.cmbDelimiter.Entry.Text = doc.Delimiter.Name;
			chkQuotes.Active = doc.SurroundText;
		}
		
		public string Delimiter {
			get {
				string delimiter = cmbDelimiter.Entry.Text.Trim();
				
				if ( delimiter.Length > 0 )
						return this.cmbDelimiter.Entry.Text;
				else 	return Core.Delimiter.TabDelimiterName;
			}
		}
		
		public bool SurroundWithDoubleQuotes {
			get { return this.chkQuotes.Active; }
		}
	}
}
