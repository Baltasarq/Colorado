
// This file has been generated by the GUI designer. Do not modify.
namespace Colorado.Gui
{
	public partial class DlgProperties
	{
		private global::Gtk.HBox hbox1;
		
		private global::Gtk.VBox vbox3;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		
		private global::Gtk.TreeView listHeaders;
		
		private global::Gtk.VBox vbox2;
		
		private global::Gtk.HBox hbox7;
		
		private global::Gtk.Label label3;
		
		private global::Gtk.SpinButton sbRows;
		
		private global::Gtk.HBox hbox8;
		
		private global::Gtk.Label label4;
		
		private global::Gtk.SpinButton sbColumns;
		
		private global::Gtk.HBox hbox3;
		
		private global::Gtk.Label label1;
		
		private global::Gtk.ComboBoxEntry cmbDelimiter;
		
		private global::Gtk.CheckButton cbSurroundText;
		
		private global::Gtk.CheckButton cbFirstRowHeaders;
		
		private global::Gtk.Button btApply;
		
		private global::Gtk.Button btClose;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Colorado.Gui.DlgProperties
			this.Name = "Colorado.Gui.DlgProperties";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child Colorado.Gui.DlgProperties.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.listHeaders = new global::Gtk.TreeView ();
			this.listHeaders.CanFocus = true;
			this.listHeaders.Name = "listHeaders";
			this.GtkScrolledWindow.Add (this.listHeaders);
			this.vbox3.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.GtkScrolledWindow]));
			w3.Position = 0;
			this.hbox1.Add (this.vbox3);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vbox3]));
			w4.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox7 = new global::Gtk.HBox ();
			this.hbox7.Name = "hbox7";
			this.hbox7.Spacing = 6;
			// Container child hbox7.Gtk.Box+BoxChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.LabelProp = "Rows";
			this.hbox7.Add (this.label3);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox7 [this.label3]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			// Container child hbox7.Gtk.Box+BoxChild
			this.sbRows = new global::Gtk.SpinButton (0D, 1000000D, 1D);
			this.sbRows.CanFocus = true;
			this.sbRows.Name = "sbRows";
			this.sbRows.Adjustment.PageIncrement = 10D;
			this.sbRows.ClimbRate = 1D;
			this.sbRows.Numeric = true;
			this.hbox7.Add (this.sbRows);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox7 [this.sbRows]));
			w6.Position = 1;
			this.vbox2.Add (this.hbox7);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox7]));
			w7.Position = 0;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox8 = new global::Gtk.HBox ();
			this.hbox8.Name = "hbox8";
			this.hbox8.Spacing = 6;
			// Container child hbox8.Gtk.Box+BoxChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.LabelProp = "Columns";
			this.hbox8.Add (this.label4);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox8 [this.label4]));
			w8.Position = 0;
			w8.Expand = false;
			w8.Fill = false;
			// Container child hbox8.Gtk.Box+BoxChild
			this.sbColumns = new global::Gtk.SpinButton (0D, 1000000D, 1D);
			this.sbColumns.CanFocus = true;
			this.sbColumns.Name = "sbColumns";
			this.sbColumns.Adjustment.PageIncrement = 10D;
			this.sbColumns.ClimbRate = 1D;
			this.sbColumns.Numeric = true;
			this.hbox8.Add (this.sbColumns);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox8 [this.sbColumns]));
			w9.Position = 1;
			this.vbox2.Add (this.hbox8);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox8]));
			w10.Position = 1;
			w10.Expand = false;
			w10.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = "Delimiter";
			this.hbox3.Add (this.label1);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.label1]));
			w11.Position = 0;
			w11.Expand = false;
			w11.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.cmbDelimiter = global::Gtk.ComboBoxEntry.NewText ();
			this.cmbDelimiter.Name = "cmbDelimiter";
			this.hbox3.Add (this.cmbDelimiter);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.cmbDelimiter]));
			w12.Position = 1;
			w12.Expand = false;
			w12.Fill = false;
			this.vbox2.Add (this.hbox3);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox3]));
			w13.Position = 2;
			w13.Expand = false;
			w13.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.cbSurroundText = new global::Gtk.CheckButton ();
			this.cbSurroundText.CanFocus = true;
			this.cbSurroundText.Name = "cbSurroundText";
			this.cbSurroundText.Label = "Surround text with double quotes";
			this.cbSurroundText.Active = true;
			this.cbSurroundText.DrawIndicator = true;
			this.cbSurroundText.UseUnderline = true;
			this.vbox2.Add (this.cbSurroundText);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.cbSurroundText]));
			w14.Position = 3;
			w14.Expand = false;
			w14.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.cbFirstRowHeaders = new global::Gtk.CheckButton ();
			this.cbFirstRowHeaders.CanFocus = true;
			this.cbFirstRowHeaders.Name = "cbFirstRowHeaders";
			this.cbFirstRowHeaders.Label = "First row is for headers";
			this.cbFirstRowHeaders.Active = true;
			this.cbFirstRowHeaders.DrawIndicator = true;
			this.cbFirstRowHeaders.UseUnderline = true;
			this.vbox2.Add (this.cbFirstRowHeaders);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.cbFirstRowHeaders]));
			w15.Position = 4;
			w15.Expand = false;
			w15.Fill = false;
			this.hbox1.Add (this.vbox2);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vbox2]));
			w16.Position = 1;
			w16.Expand = false;
			w16.Fill = false;
			w1.Add (this.hbox1);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(w1 [this.hbox1]));
			w17.Position = 0;
			// Internal child Colorado.Gui.DlgProperties.ActionArea
			global::Gtk.HButtonBox w18 = this.ActionArea;
			w18.Name = "dialog1_ActionArea";
			w18.Spacing = 10;
			w18.BorderWidth = ((uint)(5));
			w18.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.btApply = new global::Gtk.Button ();
			this.btApply.CanFocus = true;
			this.btApply.Name = "btApply";
			this.btApply.UseStock = true;
			this.btApply.UseUnderline = true;
			this.btApply.Label = "gtk-apply";
			this.AddActionWidget (this.btApply, -10);
			global::Gtk.ButtonBox.ButtonBoxChild w19 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w18 [this.btApply]));
			w19.Expand = false;
			w19.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.btClose = new global::Gtk.Button ();
			this.btClose.CanDefault = true;
			this.btClose.CanFocus = true;
			this.btClose.Name = "btClose";
			this.btClose.UseStock = true;
			this.btClose.UseUnderline = true;
			this.btClose.Label = "gtk-close";
			this.AddActionWidget (this.btClose, -7);
			global::Gtk.ButtonBox.ButtonBoxChild w20 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w18 [this.btClose]));
			w20.Position = 1;
			w20.Expand = false;
			w20.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 555;
			this.DefaultHeight = 281;
			this.Show ();
			this.btApply.Clicked += new global::System.EventHandler (this.OnApply);
		}
	}
}
