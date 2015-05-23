
using System;
using System.Collections;
using System.Collections.Generic;
using GtkUtil;

using Colorado.Core;

namespace Colorado.Gui {
	public partial class DlgExport : Gtk.Dialog {
		public DlgExport(Gtk.Window parent, CsvDocument doc)
		{
			document = doc;
			this.Build();
			
			this.Title = parent.Title + " export";
			this.Icon = parent.Icon;
			this.Parent = parent;
			this.TransientFor = parent;
			this.SetPosition( Gtk.WindowPosition.CenterOnParent );
			
			// Last file name opened
			lastFileName = ( (MainWindow) parent ).lastFileName;
			this.cmbOutput.Active = 0;
			this.OnChanged( this, null );
			
			// Load combobox items
			var cbModel = new Gtk.ListStore( typeof( string ) );
			foreach(var explanation in ExportOptions.SelectionExplanation) {
				cbModel.AppendValues( new string[]{ explanation } );
			}
			this.cmbOutput.Model = cbModel;
			this.cmbOutput.Active = 0;
			
			this.AddChecksForFields();
		}
		
		/// <summary>
		/// Adds a check box for each field inside of the scroll window of the frField frame.
		/// </summary>
		protected void AddChecksForFields()
		{
			fieldChecks = new Gtk.CheckButton[ document.Data.ColumnInfo.Length ];
			int fieldNum = 0;
			var vbox = new Gtk.VBox();
			vbox.Visible = true;
			this.swScroll.AddWithViewport( vbox );
			
			foreach(ColumnInfo colInfo in document.Data.ColumnInfo) {
				fieldChecks[ fieldNum ] = new Gtk.CheckButton( colInfo.Header );
				fieldChecks[ fieldNum ].Visible = true;
				fieldChecks[ fieldNum ].Active = true;
				vbox.Add( fieldChecks[ fieldNum ] );
				                                              
				++fieldNum;
			}
		}
		
		public ExportOptions.SelectionType Selection {
			get { return ( (ExportOptions.SelectionType) cmbOutput.Active ); }
		}
		
		public string FileName {
			get { return edFileName.Text.Trim(); }
		}
		
		public bool IncludeRowNumbers {
			get { return chkIncludeRowNumbers.Active; }
		}
		
		public bool IncludeTableBorder {
			get { return chkIncludeTableBorder.Active; }
		}
		
		/// <summary>
		/// Return the sequence of column indexes to include
		/// </summary>
		public int[] ColumnsIncluded {
			get {
				var colsIncluded = new List<int>();
				for(int i = 0; i < this.fieldChecks.Length; ++i) {
					if ( this.fieldChecks[ i ].Active ) {
						colsIncluded.Add( i );
					}
				}
				
				return colsIncluded.ToArray();
			}
		}
		
		protected virtual void OnSaveAs(object sender, System.EventArgs e)
		{
			string filter = "*" + ExportOptions.SelectionExtension[ this.cmbOutput.Active ];
			
			if ( Util.DlgSave( AppInfo.Name,
			                  "Save as...",
			                  (Gtk.Window) this.Parent,
			                  ref lastFileName,
			                  filter ) )
			{
				this.OnChanged( sender, e );
			}
			else lastFileName = this.edFileName.Text;
		}
		
		protected virtual void OnChanged(object sender, System.EventArgs e)
		{
			int active = this.cmbOutput.Active;
			
			if ( active < 0 ) {
				this.cmbOutput.Active = active = 0;
			}

			if ( string.IsNullOrWhiteSpace( lastFileName ) ) {
				lastFileName = "output_file.csv";
			}
			
			lastFileName = System.IO.Path.ChangeExtension( lastFileName, ExportOptions.SelectionExtension[ active ] );
			this.edFileName.Text = lastFileName;
			this.edFileName.Position = lastFileName.Length;
		}

		protected CsvDocument document;
		protected Gtk.CheckButton[] fieldChecks;

		public static string lastFileName;
	}
}
