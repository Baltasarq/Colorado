// Colorado (c) 2015-2018 Baltasar MIT License <baltasarq@gmail.com>


namespace Colorado.Gui {
    public partial class DlgImport {
        public Core.ImportOptions Options =>
                new Core.ImportOptions( this.FileName ) {
                    ImportId = this.cmbType.ActiveText,
                    FirstRowIsForHeaders = this.FirstRowForHeaders,
                    Delimiter = this.Delimiter
                };

        public string FileName => this.edFileName.Text;
        public Core.Delimiter Delimiter => new( this.cmbDelimiter.Entry.Text.Trim() );
        public bool FirstRowForHeaders => this.cbFirstRowForHeaders.Active;

        void OnOpen()
        {
            if ( string.IsNullOrWhiteSpace( lastFileName ) ) {
                lastFileName = ".";
            }

            var importer = Core.Importer.GetImporter( this.cmbType.ActiveText );

            if ( GtkUtil.Misc.DlgOpen(
                                Core.AppInfo.Name, "Open",
                                (Gtk.Window) this.Parent,
                                ref lastFileName,
                                "*." + importer.FileExtension ) )
            {
                this.edFileName.Text = lastFileName;
            }
            else lastFileName = this.edFileName.Text;
        }

        private string lastFileName;
    }
}
