using GtkUtil;

namespace Colorado.Gui {
    public partial class DlgImport {
        public string FileName {
            get { return this.edFileName.Text; }
        }

        public char Delimiter {
            get {
                string delimiter = cmbDelimiter.Entry.Text.Trim();

                if ( delimiter.Length > 0 )
                    return this.cmbDelimiter.Entry.Text[ 0 ];
                else    return Core.Delimiter.TabDelimiter;
            }
        }

        public bool FirstRowForHeaders {
            get { return this.cbFirstRowForHeaders.Active; }
        }

        private void OnOpen()
        {
            if ( Util.DlgOpen(
                Core.AppInfo.Name, "Open",
                (Gtk.Window) this.Parent,
                ref lastFileName,
                Core.CsvDocumentPersistence.FileFilter ) )
            {
                this.edFileName.Text = lastFileName;
            }
            else lastFileName = this.edFileName.Text;
        }

        public static string lastFileName;
    }
}

