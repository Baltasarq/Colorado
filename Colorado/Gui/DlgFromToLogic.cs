// Colorado (c) 2015-2018 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Gui {
    using System.Collections.ObjectModel;

    public partial class DlgFromTo {
        public enum ItemType { Rows, Columns };
        public enum ActionType { Clean, Copy };

        public static readonly ReadOnlyCollection<string> StrItemType = new ReadOnlyCollection<string>(
            new string[]{ "Row", "Column" }
        );
        public static readonly ReadOnlyCollection<string> StrActionType = new ReadOnlyCollection<string>(
            new string[]{ "Clean", "Copy" }
        );

        public int From {
            get { return sbFrom.ValueAsInt; }
        }

        public int To {
            get { return sbTo.ValueAsInt; }
        }
    }
}

