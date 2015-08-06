
namespace Colorado.Gui {
    public partial class MainWindow {
        private void Build() {
            var vPanel = new Gtk.VBox( false, 2 );
            var hPanel = new Gtk.HBox( false, 2 );
            var swScroll = new Gtk.ScrolledWindow();

            // Create components
            this.edFind = new Gtk.Entry( "Find..." );
            this.edFind.Activated += (sender, e) => this.OnEdFindActivated();
            this.edFind.FocusInEvent += (sender, e) => this.edFind.Text = "";
            this.edFind.FocusOutEvent += (sender, e) => this.edFind.Text = "Find...";
            this.tvTable = new Gtk.TreeView();
            this.tvTable.EnableSearch = false;
            this.tvTable.ButtonReleaseEvent +=
                (object o, Gtk.ButtonReleaseEventArgs args) => this.OnTableClicked( args );
            this.tvTable.KeyPressEvent +=
                (object o, Gtk.KeyPressEventArgs args) => this.OnTableKeyPressed( args );
            swScroll.AddWithViewport( this.tvTable );

            // Build them all
            this.BuildActions();
            this.BuildStatusBar();
            this.BuildMenu();
            this.BuildToolbar();
            this.BuildPopup();

            // Create layout
            hPanel.PackStart( this.tbTools, true, true, 0 );
            hPanel.PackStart( this.edFind, false, false, 0 );

            vPanel.PackStart( this.menuBar, false, false, 0 );
            vPanel.PackStart( hPanel, false, false, 0 );
			vPanel.PackStart( swScroll, true, true, 0 );
            vPanel.PackStart( this.sbStatus, false, false, 0 );

            // Add to this
            this.Add( vPanel );

            // Polishing
            Gdk.Geometry minSize = new Gdk.Geometry();
            minSize.MinHeight = 480;
            minSize.MinWidth = 640;
            this.SetDefaultSize( minSize.MinHeight, minSize.MinWidth );
            this.SetGeometryHints( this, minSize, Gdk.WindowHints.MinSize );
            this.sbStatus.Push( 0, "Ready" );
            this.SetPosition( Gtk.WindowPosition.Center );
            this.DeleteEvent += (o, args) => { this.OnQuit(); args.RetVal = true; };
        }

        private void BuildActions() {
            this.newAction = new Gtk.Action( "new", "_New", "new spreadhseet", Gtk.Stock.New );
            this.newAction.Activated += (sender, e) => this.OnNew();

            this.openAction = new Gtk.Action( "open", "_Open", "open spreadhseet", Gtk.Stock.Open );
            this.openAction.Activated += (sender, e) => this.OnOpen();

            this.saveAction = new Gtk.Action( "save", "_Save", "save spreadhseet", Gtk.Stock.Save );
            this.saveAction.Activated += (sender, e) => this.OnSave();

            this.saveAsAction = new Gtk.Action( "save_as", "Save _as...", "save spreadhseet as...", Gtk.Stock.SaveAs );
            this.saveAsAction.Activated += (sender, e) => this.OnSaveAs();

            this.propertiesAction = new Gtk.Action( "properties", "_Properties", "properties", Gtk.Stock.Properties );
            this.propertiesAction.Activated += (sender, e) => this.OnProperties();

            this.closeAction = new Gtk.Action( "close", "_Close", "close spreadhseet", Gtk.Stock.Close );
            this.closeAction.Activated += (sender, e) => this.OnClose();

            this.aboutAction = new Gtk.Action( "about", "_About", "about...", Gtk.Stock.About );
            this.aboutAction.Activated += (sender, e) => this.OnAbout();

            this.importAction = new Gtk.Action( "import", "_Import", "import data", Gtk.Stock.Dnd );
            this.importAction.Activated += (sender, e) => this.OnImport();

            this.exportAction = new Gtk.Action( "export", "_Export", "export to...", Gtk.Stock.Convert );
            this.exportAction.Activated += (sender, e) => this.OnExport();

            this.revertAction = new Gtk.Action( "revert", "_Revert", "revert to file", Gtk.Stock.RevertToSaved );
            this.revertAction.Activated += (sender, e) => this.OnRevert();

            this.quitAction = new Gtk.Action( "quit", "_Quit", "quit", Gtk.Stock.Quit );
            this.quitAction.Activated += (sender, e) => this.OnQuit();

            this.findAction = new Gtk.Action( "find", "_Find", "find...", Gtk.Stock.Find );
            this.findAction.Activated += (sender, e) => this.OnFind();

            this.findAgainAction = new Gtk.Action( "find_again", "_Find again", "find again", Gtk.Stock.Find );
            this.findAgainAction.Activated += (sender, e) => this.OnFindAgain();

            this.insertFormulaAction = new Gtk.Action( "insert_formula", "_Insert formula", "insert formula", Gtk.Stock.Edit );
            this.insertFormulaAction.Activated += (sender, e) => this.OnInsertFormula();

            this.addRowsAction = new Gtk.Action( "add_rows", "_Add rows", "add rows", Gtk.Stock.Add );
            this.addRowsAction.Activated += (sender, e) => this.OnAddRows();

            this.removeRowsAction = new Gtk.Action( "remove_rows", "_Remove rows", "remove rows", Gtk.Stock.Remove );
            this.removeRowsAction.Activated += (sender, e) => this.OnRemoveRows();

            this.clearRowsAction = new Gtk.Action( "clear_rows", "_Clear rows", "clear rows", Gtk.Stock.Clear );
            this.clearRowsAction.Activated += (sender, e) => this.OnClearRows();

            this.copyRowAction = new Gtk.Action( "copy_row", "_Copy row", "copy row", Gtk.Stock.Copy );
            this.copyRowAction.Activated += (sender, e) => this.OnCopyRow();

            this.fillRowAction = new Gtk.Action( "fill_row", "_Fill row", "fill row", Gtk.Stock.Paste );
            this.fillRowAction.Activated += (sender, e) => this.OnFillRow();

            this.addColumnsAction = new Gtk.Action( "add_columns", "_Add columns", "add columns", Gtk.Stock.Add );
            this.addColumnsAction.Activated += (sender, e) => this.OnAddColumns();

            this.removeColumnsAction = new Gtk.Action( "remove_columns", "_Remove columns", "remove columns", Gtk.Stock.Remove );
            this.removeColumnsAction.Activated += (sender, e) => this.OnRemoveColumns();

            this.clearColumnsAction = new Gtk.Action( "clear_Columns", "_Clear columns", "clear columns", Gtk.Stock.Clear );
            this.clearColumnsAction.Activated += (sender, e) => this.OnClearColumns();

            this.copyColumnAction = new Gtk.Action( "copy_column", "_Copy column", "copy column", Gtk.Stock.Copy );
            this.copyColumnAction.Activated += (sender, e) => this.OnCopyColumn();

            this.fillColumnAction = new Gtk.Action( "fill_column", "_Fill column", "fill column", Gtk.Stock.Paste );
            this.fillColumnAction.Activated += (sender, e) => this.OnFillColumn();
        }

        private void BuildStatusBar() {
            var hPanel = new Gtk.HBox( false, 2 );
            this.lblCount = new Gtk.Label( "..." );
            this.lblType = new Gtk.Label( "..." );

            this.sbStatus = new Gtk.Statusbar();

            hPanel.PackStart( this.lblType, true, false, 5 );
            hPanel.PackStart( this.lblCount, true, false, 5 );

            this.sbStatus.PackStart( hPanel, false, false, 5 );
        }

        private void BuildMenu() {
            var mFile = new Gtk.Menu();
            var mEdit = new Gtk.Menu();
            var mRows = new Gtk.Menu();
            var mColumns = new Gtk.Menu();
            var mHelp = new Gtk.Menu();

            var miFile = new Gtk.MenuItem( "_File" );
            var miEdit = new Gtk.MenuItem( "_Edit" );
            var miRows = new Gtk.MenuItem( "_Rows" );
            var miColumns = new Gtk.MenuItem( "_Columns" );
            var miHelp = new Gtk.MenuItem( "_Help" );

            var accelGroup = new Gtk.AccelGroup();
            miFile.Submenu = mFile;
            miEdit.Submenu = mEdit;
            miRows.Submenu = mRows;
            miColumns.Submenu = mColumns;
            miHelp.Submenu = mHelp;

            var opNew = this.newAction.CreateMenuItem();
            opNew.AddAccelerator( "activate", accelGroup, new Gtk.AccelKey(
                Gdk.Key.n, Gdk.ModifierType.ControlMask, Gtk.AccelFlags.Visible) );

            var opOpen = this.openAction.CreateMenuItem();
            opOpen.AddAccelerator( "activate", accelGroup, new Gtk.AccelKey(
                Gdk.Key.o, Gdk.ModifierType.ControlMask, Gtk.AccelFlags.Visible) );

            var opSave = this.saveAction.CreateMenuItem();
            opSave.AddAccelerator( "activate", accelGroup, new Gtk.AccelKey(
                Gdk.Key.s, Gdk.ModifierType.ControlMask, Gtk.AccelFlags.Visible) );

            var opSaveAs = this.saveAsAction.CreateMenuItem();

            var opClose = this.closeAction.CreateMenuItem();

            var opProperties = this.propertiesAction.CreateMenuItem();
            opProperties.AddAccelerator( "activate", accelGroup, new Gtk.AccelKey(
                Gdk.Key.F2, Gdk.ModifierType.None, Gtk.AccelFlags.Visible) );

            var opQuit = this.quitAction.CreateMenuItem();
            opQuit.AddAccelerator( "activate", accelGroup, new Gtk.AccelKey(
                Gdk.Key.q, Gdk.ModifierType.ControlMask, Gtk.AccelFlags.Visible) );

            var opAddRows = this.addRowsAction.CreateMenuItem();
            opAddRows.AddAccelerator( "activate", accelGroup, new Gtk.AccelKey(
                Gdk.Key.plus, Gdk.ModifierType.ControlMask, Gtk.AccelFlags.Visible) );

            var opFind = this.findAction.CreateMenuItem();
            opFind.AddAccelerator( "activate", accelGroup, new Gtk.AccelKey(
                Gdk.Key.F, Gdk.ModifierType.ControlMask, Gtk.AccelFlags.Visible) );
            
            var opFindAgain = this.findAgainAction.CreateMenuItem();
            opFindAgain.AddAccelerator( "activate", accelGroup, new Gtk.AccelKey(
                Gdk.Key.F3, Gdk.ModifierType.None, Gtk.AccelFlags.Visible) );

            mFile.Append( opNew );
            mFile.Append( opOpen );
            mFile.Append( opSave );
            mFile.Append( opSaveAs );
            mFile.Append( new Gtk.SeparatorMenuItem() );
            mFile.Append( opProperties );
            mFile.Append( opClose );
            mFile.Append( new Gtk.SeparatorMenuItem() );
            mFile.Append( this.importAction.CreateMenuItem() );
            mFile.Append( this.exportAction.CreateMenuItem() );
            mFile.Append( this.revertAction.CreateMenuItem() );
            mFile.Append( opQuit );

            mEdit.Append( opFind );
            mEdit.Append( opFindAgain );
            mEdit.Append( this.insertFormulaAction.CreateMenuItem() );
            mEdit.Append( new Gtk.SeparatorMenuItem() );
            mEdit.Append( miRows );
            mEdit.Append( miColumns );
            mRows.Append( opAddRows );
            mRows.Append( this.removeRowsAction.CreateMenuItem() );
            mRows.Append( this.clearRowsAction.CreateMenuItem() );
            mRows.Append( this.copyRowAction.CreateMenuItem() );
            mRows.Append( this.fillRowAction.CreateMenuItem() );
            mColumns.Append( this.addColumnsAction.CreateMenuItem() );
            mColumns.Append( this.removeColumnsAction.CreateMenuItem() );
            mColumns.Append( this.clearColumnsAction.CreateMenuItem() );
            mColumns.Append( this.copyColumnAction.CreateMenuItem() );
            mColumns.Append( this.fillColumnAction.CreateMenuItem() );

            mHelp.Append( this.aboutAction.CreateMenuItem() );

            this.menuBar = new Gtk.MenuBar();
            this.menuBar.Append( miFile );
            this.menuBar.Append( miEdit );
            this.menuBar.Append( miHelp );
            this.AddAccelGroup( accelGroup );
        }

        private void BuildToolbar() {
            this.tbTools = new Gtk.Toolbar();
            this.tbTools.ToolbarStyle = Gtk.ToolbarStyle.Icons;

            this.tbTools.Insert( (Gtk.ToolItem) this.newAction.CreateToolItem(), 0 );
            this.tbTools.Insert( (Gtk.ToolItem) this.openAction.CreateToolItem(), 1 );
            this.tbTools.Insert( (Gtk.ToolItem) this.saveAction.CreateToolItem(), 2 );
            this.tbTools.Insert( (Gtk.ToolItem) this.propertiesAction.CreateToolItem(), 3 );
            this.tbTools.Insert( (Gtk.ToolItem) this.closeAction.CreateToolItem(), 4 );
            this.tbTools.Insert( new Gtk.SeparatorToolItem(), 5 );
            this.tbTools.Insert( (Gtk.ToolItem) this.addRowsAction.CreateToolItem(), 6 );
            this.tbTools.Insert( (Gtk.ToolItem) this.removeRowsAction.CreateToolItem(), 7 );
            this.tbTools.Insert( (Gtk.ToolItem) this.clearRowsAction.CreateToolItem(), 8 );
            this.tbTools.Insert( (Gtk.ToolItem) this.copyRowAction.CreateToolItem(), 9 );
            this.tbTools.Insert( (Gtk.ToolItem) this.fillRowAction.CreateToolItem(), 10 );
            this.tbTools.Insert( new Gtk.SeparatorToolItem(), 11 );
            this.tbTools.Insert( (Gtk.ToolItem) this.addColumnsAction.CreateToolItem(), 12 );
            this.tbTools.Insert( (Gtk.ToolItem) this.removeColumnsAction.CreateToolItem(), 13 );
            this.tbTools.Insert( (Gtk.ToolItem) this.clearColumnsAction.CreateToolItem(), 14 );
            this.tbTools.Insert( (Gtk.ToolItem) this.copyColumnAction.CreateToolItem(), 15 );
            this.tbTools.Insert( (Gtk.ToolItem) this.fillColumnAction.CreateToolItem(), 16 );
        }

        private void BuildPopup()
        {
            // Menus
            this.popup = new Gtk.Menu();

            // Rows
            this.popup.Append( this.addRowsAction.CreateMenuItem() );
            this.popup.Append( this.removeRowsAction.CreateMenuItem() );
            this.popup.Append( this.clearRowsAction.CreateMenuItem() );
            this.popup.Append( this.copyRowAction.CreateMenuItem() );
            this.popup.Append( this.fillRowAction.CreateMenuItem() );

            // Columns
            this.popup.Append( new Gtk.SeparatorMenuItem() );
            this.popup.Append( this.addColumnsAction.CreateMenuItem() );
            this.popup.Append( this.removeColumnsAction.CreateMenuItem() );
            this.popup.Append( this.clearColumnsAction.CreateMenuItem() );
            this.popup.Append( this.copyColumnAction.CreateMenuItem() );
            this.popup.Append( this.fillColumnAction.CreateMenuItem() );

            // General
            this.popup.Append( new Gtk.SeparatorMenuItem() );
            this.popup.Append( this.propertiesAction.CreateMenuItem() );
            this.popup.Append( this.closeAction.CreateMenuItem() );

            // Finish
            this.popup.ShowAll();
        }
            
        // Widgets
        private Gtk.TreeView tvTable;
        private Gtk.Statusbar sbStatus;
        private Gtk.Toolbar tbTools;
        private Gtk.MenuBar menuBar;
        private Gtk.Menu popup;
        private Gtk.Entry edFind;
        private Gtk.Label lblType;
        private Gtk.Label lblCount;

        // Actions
        private Gtk.Action newAction;
        private Gtk.Action openAction;
        private Gtk.Action saveAction;
        private Gtk.Action saveAsAction;
        private Gtk.Action propertiesAction;
        private Gtk.Action closeAction;
        private Gtk.Action importAction;
        private Gtk.Action exportAction;
        private Gtk.Action revertAction;
        private Gtk.Action quitAction;
        private Gtk.Action findAction;
        private Gtk.Action findAgainAction;
        private Gtk.Action insertFormulaAction;
        private Gtk.Action addRowsAction;
        private Gtk.Action removeRowsAction;
        private Gtk.Action clearRowsAction;
        private Gtk.Action copyRowAction;
        private Gtk.Action fillRowAction;
        private Gtk.Action addColumnsAction;
        private Gtk.Action removeColumnsAction;
        private Gtk.Action clearColumnsAction;
        private Gtk.Action copyColumnAction;
        private Gtk.Action fillColumnAction;
        private Gtk.Action aboutAction;
    }
}

