using System;
namespace Colorado.Gui {
	
	public partial class DlgFill : Gtk.Dialog {
	
		public DlgFill(Gtk.Window parent)
		{
			this.Build();
			
			this.Parent = parent;
			this.TransientFor = parent;
			this.Title = parent.Title;
			this.Icon = parent.Icon;
		}
		
		public string FillValue {
			get { return this.edValue.Text; }
		}
		
        public Core.Filler.FillType KindOfFill {
			get {
                var toret = Core.Filler.FillType.Fixed;
				
				if ( rbIncrement.Active ) {
                    toret = Core.Filler.FillType.Increment;
				}
				else
				if ( rbDecrement.Active ) {
                        toret = Core.Filler.FillType.Decrement;
				}
				
				return toret;
			}
		}
	}
}

