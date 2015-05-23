
// This file has been generated by the GUI designer. Do not modify.
namespace Colorado.Gui
{
	public partial class DlgIncDec
	{
		private global::Gtk.VBox vbox3;
		
		private global::Gtk.HBox hbox1;
		
		private global::Gtk.Label lblValues;
		
		private global::Gtk.Alignment alignment1;
		
		private global::Gtk.Alignment alignment2;
		
		private global::Gtk.HBox hbox4;
		
		private global::Gtk.Label label3;
		
		private global::Gtk.SpinButton sbFrom;
		
		private global::Gtk.HBox hbox5;
		
		private global::Gtk.Label label4;
		
		private global::Gtk.SpinButton sbNumber;
		
		private global::Gtk.Button buttonCancel;
		
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Colorado.Gui.DlgIncDec
			this.Name = "Colorado.Gui.DlgIncDec";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child Colorado.Gui.DlgIncDec.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			this.vbox3.BorderWidth = ((uint)(5));
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.lblValues = new global::Gtk.Label ();
			this.lblValues.Name = "lblValues";
			this.lblValues.LabelProp = "<b>Accepted values:</b>";
			this.lblValues.UseMarkup = true;
			this.lblValues.SingleLineMode = true;
			this.hbox1.Add (this.lblValues);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.lblValues]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.alignment1 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.alignment1.Name = "alignment1";
			this.hbox1.Add (this.alignment1);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.alignment1]));
			w3.Position = 1;
			// Container child hbox1.Gtk.Box+BoxChild
			this.alignment2 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.alignment2.Name = "alignment2";
			this.hbox1.Add (this.alignment2);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.alignment2]));
			w4.Position = 2;
			this.vbox3.Add (this.hbox1);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox1]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox4 = new global::Gtk.HBox ();
			this.hbox4.Name = "hbox4";
			this.hbox4.Spacing = 6;
			// Container child hbox4.Gtk.Box+BoxChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.LabelProp = "From";
			this.hbox4.Add (this.label3);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.label3]));
			w6.Position = 0;
			w6.Expand = false;
			w6.Fill = false;
			// Container child hbox4.Gtk.Box+BoxChild
			this.sbFrom = new global::Gtk.SpinButton (0D, 100D, 1D);
			this.sbFrom.CanFocus = true;
			this.sbFrom.Name = "sbFrom";
			this.sbFrom.Adjustment.PageIncrement = 10D;
			this.sbFrom.ClimbRate = 1D;
			this.sbFrom.Numeric = true;
			this.hbox4.Add (this.sbFrom);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.sbFrom]));
			w7.Position = 1;
			this.vbox3.Add (this.hbox4);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox4]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox5 = new global::Gtk.HBox ();
			this.hbox5.Name = "hbox5";
			this.hbox5.Spacing = 6;
			// Container child hbox5.Gtk.Box+BoxChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.LabelProp = "Number";
			this.hbox5.Add (this.label4);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.label4]));
			w9.Position = 0;
			w9.Expand = false;
			w9.Fill = false;
			// Container child hbox5.Gtk.Box+BoxChild
			this.sbNumber = new global::Gtk.SpinButton (0D, 100D, 1D);
			this.sbNumber.CanFocus = true;
			this.sbNumber.Name = "sbNumber";
			this.sbNumber.Adjustment.PageIncrement = 10D;
			this.sbNumber.ClimbRate = 1D;
			this.sbNumber.Numeric = true;
			this.hbox5.Add (this.sbNumber);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.sbNumber]));
			w10.Position = 1;
			this.vbox3.Add (this.hbox5);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox5]));
			w11.Position = 2;
			w11.Expand = false;
			w11.Fill = false;
			w1.Add (this.vbox3);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(w1 [this.vbox3]));
			w12.Position = 0;
			w12.Expand = false;
			w12.Fill = false;
			w12.Padding = ((uint)(5));
			// Internal child Colorado.Gui.DlgIncDec.ActionArea
			global::Gtk.HButtonBox w13 = this.ActionArea;
			w13.Name = "dialog1_ActionArea";
			w13.Spacing = 10;
			w13.BorderWidth = ((uint)(5));
			w13.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w14 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w13 [this.buttonCancel]));
			w14.Expand = false;
			w14.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			this.AddActionWidget (this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w15 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w13 [this.buttonOk]));
			w15.Position = 1;
			w15.Expand = false;
			w15.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 393;
			this.DefaultHeight = 156;
			this.Show ();
		}
	}
}
