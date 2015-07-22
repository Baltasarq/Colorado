using System;

using GtkUtil;

using Core = Colorado.Core;

namespace Colorado.Gui {
	public partial class DlgImport : Gtk.Dialog {
		public static string lastFileName;
		
		public DlgImport(Gtk.Window parent) {
			this.Build();
			this.ShowAll();
			
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
			this.edFileName.Text = lastFileName;
		}
	
		public string FileName {
			get { return this.edFileName.Text; }
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
			get { return this.cbFirstRowForHeaders.Active; }
		}

		protected virtual void OnOpen()
		{
			if ( Util.DlgOpen(
						Core.AppInfo.Name, "Open",
						(Gtk.Window) this.Parent,
						ref lastFileName,
						Core.CsvDocumentPersistence.FileFilter ) )
			{
				this.edFileName.Text = lastFileName;
			}
			else lastFileName = this.edFileName.Text;
		}

		private void Build() {
			var hBoxFileName = new Gtk.HBox( false, 2 );
			var hBoxDelimiter = new Gtk.HBox( false, 2 );
			var vBoxFileName = new Gtk.VBox( false, 2 );
			var vBoxOptions = new Gtk.VBox( false, 2 );

			// File name
			this.frmFileName = new Gtk.Frame( "Name" );
			this.lblFileName = new Gtk.Label( "File name:" );
			this.edFileName = new Gtk.Entry();
			this.btOpen = new Gtk.Button( Gtk.Stock.Open );
			this.btOpen.Clicked += (sender, e) => this.OnOpen();
			hBoxFileName.PackStart( this.lblFileName, false, false, 5 );
			hBoxFileName.PackStart( this.edFileName, true, true, 5 );
			hBoxFileName.PackStart( this.btOpen, false, false, 5 );

			vBoxFileName.PackStart( hBoxFileName, true, true, 5 );
			this.frmFileName.Add( vBoxFileName );

			// Options
			this.frmOptions = new Gtk.Frame( "Options" );
			this.lblDelimiter = new Gtk.Label( "Delimiter" );
			this.cmbDelimiter = new Gtk.ComboBoxEntry( new string[]{ ";", "," } );
			this.cbFirstRowForHeaders = new Gtk.CheckButton( "_First row is for headers" );
			hBoxDelimiter.PackStart( this.lblDelimiter, false, false, 5 );
			hBoxDelimiter.PackStart( this.cmbDelimiter, true, true, 5 );
			vBoxOptions.PackStart( hBoxDelimiter, true, true, 5 );
			vBoxOptions.PackStart( this.cbFirstRowForHeaders, true, true, 5 );
			this.frmOptions.Add( vBoxOptions );

			// Layout
			this.VBox.PackStart( this.frmFileName, true, true, 5 );
			this.VBox.PackStart( this.frmOptions, true, true, 5 );

			// Buttons
			this.AddButton( Gtk.Stock.Cancel, Gtk.ResponseType.Cancel );
			this.AddButton( Gtk.Stock.Ok, Gtk.ResponseType.Ok );
			this.DefaultResponse = Gtk.ResponseType.Ok;
		}

		private Gtk.Frame frmFileName;
		private Gtk.Frame frmOptions;
		private Gtk.Label lblFileName;
		private Gtk.Entry edFileName;
		private Gtk.Button btOpen;
		private Gtk.CheckButton cbFirstRowForHeaders;
		private Gtk.Label lblDelimiter;
		private Gtk.ComboBoxEntry cmbDelimiter;
	}
	
			
}
