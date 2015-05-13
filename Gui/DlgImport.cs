using System;

using GtkUtil;

using Core = Colorado.Core;

namespace Colorado.Gui {
	public partial class DlgImport : Gtk.Dialog {
		public static string lastFileName;
		
		public DlgImport(Gtk.Window parent) {
			this.Build();
			
			this.Title = parent.Title + " import";
			this.Icon = parent.Icon;
			this.Parent = parent;
			this.TransientFor = parent;
			this.SetPosition( Gtk.WindowPosition.CenterOnParent );

			// Last file name opened
			lastFileName = ( (MainWindow) parent ).lastFileName;
			
			// Add delimiters to the combo
			foreach(string delimiterName in Core.Delimiter.PredefinedDelimiterNames) {
				this.cmbDelimiter.AppendText( delimiterName );
			}
			this.cmbDelimiter.Entry.Text = Core.Delimiter.TabDelimiterName;
			this.edFile.Text = lastFileName;
		}
	
		public string FileName {
			get { return this.edFile.Text; }
		}
		
		public char Delimiter {
			get {
				string delimiter = cmbDelimiter.Entry.Text.Trim();
				
				if ( delimiter.Length > 0 )
						return this.cmbDelimiter.Entry.Text[ 0 ];
				else 	return Core.Delimiter.TabDelimiter;
			}
		}
		
		public bool FirstRowForHeaders {
			get { return this.chkHeaders.Active; }
		}

		protected virtual void onOpen(object sender, System.EventArgs e)
		{
			if ( Util.DlgOpen(
						Core.AppInfo.Name, "Open",
						(Gtk.Window) this.Parent,
						ref lastFileName,
						Core.CsvDocument.FileFilter ) )
			{
				this.edFile.Text = lastFileName;
			}
			else lastFileName = this.edFile.Text;
		}
	}
	
			
}
