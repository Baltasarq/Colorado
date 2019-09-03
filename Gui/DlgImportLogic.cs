// Colorado (c) 2015-2018 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Gui {
    using GtkUtil;

    public partial class DlgImport {
        public Core.ImportOptions Options {
            get {
                return new Core.ImportOptions( this.FileName ) {
                    ImportId = this.cmbType.ActiveText,
                    FirstRowIsForHeaders = this.FirstRowForHeaders,
                    Delimiter = this.Delimiter
                };
            }
        }

        public string FileName {
            get { return this.edFileName.Text; }
        }

        public Core.Delimiter Delimiter {
            get {
                return new Core.Delimiter( this.cmbDelimiter.Entry.Text.Trim() );
            }
        }

        public bool FirstRowForHeaders {
            get { return this.cbFirstRowForHeaders.Active; }
        }

        void OnOpen()
        {
            if ( string.IsNullOrWhiteSpace( lastFileName) ) {
                lastFileName = ".";
            }

            var importer = Core.Importer.GetImporter( this.cmbType.ActiveText );

            if ( Util.DlgOpen(
                Core.AppInfo.Name, "Open",
                (Gtk.Window) this.Parent,
                ref lastFileName,
                "*." + importer.FileExtension ) )
            {
                this.edFileName.Text = lastFileName;
            }
            else lastFileName = this.edFileName.Text;
        }

        public static string lastFileName;
    }
}

