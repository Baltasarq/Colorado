
// This file has been generated by the GUI designer. Do not modify.
namespace Colorado.Gui
{
	public partial class DlgFormulae
	{
		private global::Gtk.VBox vbox2;
		
		private global::Gtk.Frame frame1;
		
		private global::Gtk.Alignment GtkAlignment2;
		
		private global::Gtk.ComboBox cbFormula;
		
		private global::Gtk.Label GtkLabel2;
		
		private global::Gtk.Frame frame2;
		
		private global::Gtk.Alignment GtkAlignment3;
		
		private global::Gtk.VBox vbox3;
		
		private global::Gtk.RadioButton rbLeft;
		
		private global::Gtk.RadioButton rbUp;
		
		private global::Gtk.RadioButton rbRight;
		
		private global::Gtk.RadioButton rbDown;
		
		private global::Gtk.Label GtkLabel3;
		
		private global::Gtk.Button buttonCancel;
		
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Colorado.Gui.DlgFormulae
			this.Name = "Colorado.Gui.DlgFormulae";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child Colorado.Gui.DlgFormulae.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			this.vbox2.BorderWidth = ((uint)(5));
			// Container child vbox2.Gtk.Box+BoxChild
			this.frame1 = new global::Gtk.Frame ();
			this.frame1.Name = "frame1";
			this.frame1.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment2 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment2.Name = "GtkAlignment2";
			this.GtkAlignment2.LeftPadding = ((uint)(12));
			// Container child GtkAlignment2.Gtk.Container+ContainerChild
			this.cbFormula = global::Gtk.ComboBox.NewText ();
			this.cbFormula.Name = "cbFormula";
			this.GtkAlignment2.Add (this.cbFormula);
			this.frame1.Add (this.GtkAlignment2);
			this.GtkLabel2 = new global::Gtk.Label ();
			this.GtkLabel2.Name = "GtkLabel2";
			this.GtkLabel2.LabelProp = "<b>Formula</b>";
			this.GtkLabel2.UseMarkup = true;
			this.frame1.LabelWidget = this.GtkLabel2;
			this.vbox2.Add (this.frame1);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.frame1]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.frame2 = new global::Gtk.Frame ();
			this.frame2.Name = "frame2";
			this.frame2.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame2.Gtk.Container+ContainerChild
			this.GtkAlignment3 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment3.Name = "GtkAlignment3";
			this.GtkAlignment3.LeftPadding = ((uint)(12));
			// Container child GtkAlignment3.Gtk.Container+ContainerChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.rbLeft = new global::Gtk.RadioButton ("Left");
			this.rbLeft.CanFocus = true;
			this.rbLeft.Name = "rbLeft";
			this.rbLeft.DrawIndicator = true;
			this.rbLeft.UseUnderline = true;
			this.rbLeft.Group = new global::GLib.SList (global::System.IntPtr.Zero);
			this.vbox3.Add (this.rbLeft);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.rbLeft]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.rbUp = new global::Gtk.RadioButton ("Up");
			this.rbUp.CanFocus = true;
			this.rbUp.Name = "rbUp";
			this.rbUp.DrawIndicator = true;
			this.rbUp.UseUnderline = true;
			this.rbUp.Group = this.rbLeft.Group;
			this.vbox3.Add (this.rbUp);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.rbUp]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.rbRight = new global::Gtk.RadioButton ("Right");
			this.rbRight.CanFocus = true;
			this.rbRight.Name = "rbRight";
			this.rbRight.DrawIndicator = true;
			this.rbRight.UseUnderline = true;
			this.rbRight.Group = this.rbLeft.Group;
			this.vbox3.Add (this.rbRight);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.rbRight]));
			w7.Position = 2;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.rbDown = new global::Gtk.RadioButton ("Down");
			this.rbDown.CanFocus = true;
			this.rbDown.Name = "rbDown";
			this.rbDown.DrawIndicator = true;
			this.rbDown.UseUnderline = true;
			this.rbDown.Group = this.rbLeft.Group;
			this.vbox3.Add (this.rbDown);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.rbDown]));
			w8.Position = 3;
			w8.Expand = false;
			w8.Fill = false;
			this.GtkAlignment3.Add (this.vbox3);
			this.frame2.Add (this.GtkAlignment3);
			this.GtkLabel3 = new global::Gtk.Label ();
			this.GtkLabel3.Name = "GtkLabel3";
			this.GtkLabel3.LabelProp = "<b>Apply to cells to the:</b>";
			this.GtkLabel3.UseMarkup = true;
			this.frame2.LabelWidget = this.GtkLabel3;
			this.vbox2.Add (this.frame2);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.frame2]));
			w11.Position = 1;
			w11.Expand = false;
			w11.Fill = false;
			w1.Add (this.vbox2);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(w1 [this.vbox2]));
			w12.Position = 0;
			w12.Expand = false;
			w12.Fill = false;
			w12.Padding = ((uint)(5));
			// Internal child Colorado.Gui.DlgFormulae.ActionArea
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
			this.DefaultWidth = 400;
			this.DefaultHeight = 254;
			this.Show ();
		}
	}
}
