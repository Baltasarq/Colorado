
// This file has been generated by the GUI designer. Do not modify.
namespace Colorado.Gui
{
	public partial class DlgFromTo
	{
		private global::Gtk.VBox vbox4;
		
		private global::Gtk.HBox hbox6;
		
		private global::Gtk.Label label5;
		
		private global::Gtk.SpinButton sbFrom;
		
		private global::Gtk.HBox hbox7;
		
		private global::Gtk.Label label6;
		
		private global::Gtk.SpinButton sbTo;
		
		private global::Gtk.Button buttonCancel;
		
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Colorado.Gui.DlgFromTo
			this.Name = "Colorado.Gui.DlgFromTo";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child Colorado.Gui.DlgFromTo.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.vbox4 = new global::Gtk.VBox ();
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 6;
			// Container child vbox4.Gtk.Box+BoxChild
			this.hbox6 = new global::Gtk.HBox ();
			this.hbox6.Name = "hbox6";
			this.hbox6.Spacing = 6;
			// Container child hbox6.Gtk.Box+BoxChild
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.LabelProp = "From";
			this.hbox6.Add (this.label5);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.label5]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child hbox6.Gtk.Box+BoxChild
			this.sbFrom = new global::Gtk.SpinButton (0, 100, 1);
			this.sbFrom.CanFocus = true;
			this.sbFrom.Name = "sbFrom";
			this.sbFrom.Adjustment.PageIncrement = 10;
			this.sbFrom.ClimbRate = 1;
			this.sbFrom.Numeric = true;
			this.hbox6.Add (this.sbFrom);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.sbFrom]));
			w3.Position = 1;
			this.vbox4.Add (this.hbox6);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.hbox6]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.hbox7 = new global::Gtk.HBox ();
			this.hbox7.Name = "hbox7";
			this.hbox7.Spacing = 6;
			// Container child hbox7.Gtk.Box+BoxChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.LabelProp = "To";
			this.hbox7.Add (this.label6);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox7 [this.label6]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			// Container child hbox7.Gtk.Box+BoxChild
			this.sbTo = new global::Gtk.SpinButton (0, 100, 1);
			this.sbTo.CanFocus = true;
			this.sbTo.Name = "sbTo";
			this.sbTo.Adjustment.PageIncrement = 10;
			this.sbTo.ClimbRate = 1;
			this.sbTo.Numeric = true;
			this.hbox7.Add (this.sbTo);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox7 [this.sbTo]));
			w6.Position = 1;
			this.vbox4.Add (this.hbox7);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.hbox7]));
			w7.Position = 1;
			w7.Expand = false;
			w7.Fill = false;
			w1.Add (this.vbox4);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(w1 [this.vbox4]));
			w8.Position = 0;
			w8.Expand = false;
			w8.Fill = false;
			// Internal child Colorado.Gui.DlgFromTo.ActionArea
			global::Gtk.HButtonBox w9 = this.ActionArea;
			w9.Name = "dialog1_ActionArea";
			w9.Spacing = 10;
			w9.BorderWidth = ((uint)(5));
			w9.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w10 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w9 [this.buttonCancel]));
			w10.Expand = false;
			w10.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			this.AddActionWidget (this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w11 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w9 [this.buttonOk]));
			w11.Position = 1;
			w11.Expand = false;
			w11.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 400;
			this.DefaultHeight = 140;
			this.Show ();
		}
	}
}
