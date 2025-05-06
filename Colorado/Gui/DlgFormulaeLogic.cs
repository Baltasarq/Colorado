// Colorado (c) 2015-2018 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Gui {
    using System;

    using Colorado.Core;

    public partial class DlgFormulae {
        public int Formula {
            get { return Math.Max( this.cbFormula.Active, 0 ); }
        }

        public new Position.Direction Direction {
            get {
                Position.Direction toret = Position.Direction.Left;

                if ( this.rbDown.Active ) {
                    toret = Position.Direction.Down;
                }
                else
                    if ( this.rbUp.Active ) {
                        toret = Position.Direction.Up;
                    }
                    else
                        if ( this.rbRight.Active ) {
                            toret = Position.Direction.Right;
                        }

                return toret;
            }
        }
    }
}

