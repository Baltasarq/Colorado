namespace Colorado.Gui {
    public partial class DlgFill {
        public string FillValue {
            get { return this.edValue.Text; }
        }

        public Core.Filler.FillType KindOfFill {
            get {
                var toret = Core.Filler.FillType.Fixed;

                if ( this.rbIncrement.Active ) {
                    toret = Core.Filler.FillType.Increment;
                }
                else
                if ( this.rbDecrement.Active ) {
                    toret = Core.Filler.FillType.Decrement;
                }

                return toret;
            }
        }

    }
}

