// Colorado (c) 2015-2018 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Gui {
    using System.Collections.ObjectModel;

    public partial class DlgIncDec {
        public enum DialogType { Insert, Erase };
        public enum WherePosition { After, Before };
        public enum Target { Rows, Columns };

        public static readonly ReadOnlyCollection<string> StrDialogType = new ReadOnlyCollection<string>(
            new string[]{ "Insert", "Erase" }
        );

        public static readonly ReadOnlyCollection<string> StrTarget = new ReadOnlyCollection<string>(
            new string[]{ "Rows", "Columns" }
        );

        /// <summary>
        /// Where to insert, after or before the "from" row or column number.
        /// </summary>
        /// <value>The row or column index.</value>
        public WherePosition Where {
            get {
                var toret = WherePosition.After;

                if ( this.rbBefore.Active ) {
                    toret = WherePosition.Before;
                }

                return toret;
            }
        }

        /// <summary>
        /// The row or column to insert or delete from
        /// </summary>
        /// <value>The row or column index, as a number.</value>
        public int From {
            get { return sbFrom.ValueAsInt; }
        }

        /// <summary>
        /// The row or column number to insert or delete.
        /// </summary>
        /// <value>The number of items to insert or delete.</value>
        public int Number {
            get { return sbNumber.ValueAsInt; }
        }
    }
}

