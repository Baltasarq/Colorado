// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Gui {
    using Core.Cfg;

    public partial class MainWindow {
        public MainWindow()
            : base( Gtk.WindowType.Toplevel )
        {
            this.Title = Core.AppInfo.Name;
            this.Build();

            this.lastFileName = "";
            this.document = null;
            this.txtToFind = "";
            this.ActivateIde( false );
            this.cfg = Config.Load();
            this.LoadRecentFilesIntoMenu();
        }

        public MainWindow(string fileName)
            : this()
        {
            this.OpenDocument( fileName );
        }

        void Build()
        {
            var vPanel = new Gtk.VBox( false, 2 );
            var hPanel = new Gtk.HBox( false, 2 );

            // Create components
            this.edFind = new Gtk.Entry( "Find..." );
            this.edFind.Activated += (sender, e) => this.OnEdFindActivated();
            this.edFind.FocusInEvent += (sender, e) => this.edFind.Text = "";
            this.edFind.FocusOutEvent += (sender, e) => this.edFind.Text = "Find...";

            // Create tree view
            this.tvTable = this.BuildTable();

            var swScroll = new Gtk.ScrolledWindow();
            swScroll.AddWithViewport( this.tvTable );

            // Build'em all
			this.BuildIcons();
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
            Gdk.Geometry minSize = new Gdk.Geometry {
                MinHeight = 480,
                MinWidth = 640
            };

            this.SetDefaultSize( minSize.MinHeight, minSize.MinWidth );
            this.SetGeometryHints( this, minSize, Gdk.WindowHints.MinSize );
            this.sbStatus.Push( 0, "Ready" );
            this.SetPosition( Gtk.WindowPosition.Center );
			this.DeleteEvent += (o, args) => { args.RetVal = this.OnQuit(); };
        }

        Gtk.TreeView BuildTable()
        {
            Gtk.TreeView toret = new Gtk.TreeView { EnableSearch = false };

            toret.Selection.Mode = Gtk.SelectionMode.Multiple;

            toret.ButtonReleaseEvent +=
                    (object o, Gtk.ButtonReleaseEventArgs args) => this.OnTableClicked( args );
            toret.KeyPressEvent +=
                    (object o, Gtk.KeyPressEventArgs args) => this.OnTableKeyPressed( args );

            return toret;
        }

		void BuildIcons()
        {
			this.ToolbarMode = Gtk.ToolbarStyle.Icons;

			try {
				this.Icon = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.colorado.png", 32, 32 );

				this.iconAbout = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.about.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-about", 32, this.iconAbout );

				this.iconAdd = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.add.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-add", 32, this.iconAdd );

				this.iconClear = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.clear.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-clear", 32, this.iconClear );

				this.iconClose = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.close.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-close", 32, this.iconClose );

				this.iconCopy = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.copy.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-copy", 32, this.iconCopy );

				this.iconExit = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.exit.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-exit", 32, this.iconExit );

				this.iconExport = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.export.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-export", 32, this.iconExport );

				this.iconFind = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.find.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-find", 32, this.iconFind );

				this.iconFormula = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.formula.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-formula", 32, this.iconFormula );

				this.iconImport = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.import.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-import", 32, this.iconImport );

				this.iconNew = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.new.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-new", 32, this.iconNew );

				this.iconOpen = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.open.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-open", 32, this.iconOpen );

				this.iconPaste = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.paste.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-paste", 32, this.iconPaste );

				this.iconProperties = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.properties.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-properties", 32, this.iconProperties );

				this.iconRemove = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.remove.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-remove", 32, this.iconRemove );

				this.iconRevert = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.revert.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-revert", 32, this.iconRevert );

				this.iconSave = new Gdk.Pixbuf(
					System.Reflection.Assembly.GetEntryAssembly(),
					"Colorado.Res.save.png", 32, 32 );
				Gtk.IconTheme.AddBuiltinIcon( "clrd-save", 32, this.iconSave );
			} catch(System.Exception) {
				this.ToolbarMode = Gtk.ToolbarStyle.Text;
			}
		}

        void BuildActions()
        {
			this.newAction = new Gtk.Action( "new", "_New", "new spreadhseet", "new" ) {IconName = "clrd-new" };
            this.newAction.Activated += (sender, e) => this.OnNew();

			this.openAction = new Gtk.Action( "open", "_Open", "open spreadhseet", "open") { IconName = "clrd-open" };
            this.openAction.Activated += (sender, e) => this.OnOpen();

			this.saveAction = new Gtk.Action( "save", "_Save", "save spreadhseet", "save" ) { IconName = "clrd-save" };
            this.saveAction.Activated += (sender, e) => this.OnSave();

			this.saveAsAction = new Gtk.Action( "save_as", "Save _as...", "save spreadhseet as...", "save" ) { IconName = "clrd-save" };
            this.saveAsAction.Activated += (sender, e) => this.OnSaveAs();

			this.propertiesAction = new Gtk.Action( "properties", "_Properties", "properties", "properties" ) { IconName = "clrd-properties" };
            this.propertiesAction.Activated += (sender, e) => this.OnProperties();

			this.closeAction = new Gtk.Action( "close", "_Close", "close spreadhseet", "close" ) { IconName = "clrd-close" };
            this.closeAction.Activated += (sender, e) => this.CloseDocument();

			this.aboutAction = new Gtk.Action( "about", "_About", "about...", "about" ) { IconName = "clrd-about" };
            this.aboutAction.Activated += (sender, e) => this.OnAbout();

			this.importAction = new Gtk.Action( "import", "_Import", "import data", "import" ) { IconName = "clrd-import" };
            this.importAction.Activated += (sender, e) => this.OnImport();

			this.exportAction = new Gtk.Action( "export", "_Export", "export to...", "export" ) { IconName = "clrd-export" };
            this.exportAction.Activated += (sender, e) => this.OnExport();

			this.revertAction = new Gtk.Action( "revert", "_Revert", "revert to file", "revert" ) { IconName = "clrd-revert" };
            this.revertAction.Activated += (sender, e) => this.OnRevert();

			this.quitAction = new Gtk.Action( "quit", "_Quit", "quit", "exit" ) { IconName = "clrd-exit" };
            this.quitAction.Activated += (sender, e) => this.OnQuit();

			this.findAction = new Gtk.Action( "find", "_Find", "find...", "find" ) { IconName = "clrd-find" };
            this.findAction.Activated += (sender, e) => this.OnFind();

			this.findAgainAction = new Gtk.Action( "find_again", "_Find again", "find again", "find" ) { IconName = "clrd-find" };
            this.findAgainAction.Activated += (sender, e) => this.OnFindAgain();

			this.insertFormulaAction = new Gtk.Action( "insert_formula", "_Insert formula", "insert formula", "formula" ) { IconName = "clrd-formula" };
            this.insertFormulaAction.Activated += (sender, e) => this.OnInsertFormula();

			this.addRowsAction = new Gtk.Action( "add_rows", "_Add rows", "add rows", "add" ) { IconName = "clrd-add" };
            this.addRowsAction.Activated += (sender, e) => this.OnAddRows();

			this.removeRowsAction = new Gtk.Action( "remove_rows", "_Remove rows", "remove rows", "remove" ) { IconName = "clrd-remove" };
            this.removeRowsAction.Activated += (sender, e) => this.OnRemoveRows();

			this.clearRowsAction = new Gtk.Action( "clear_rows", "_Clear rows", "clear rows", "clear" ) { IconName = "clrd-clear" };
            this.clearRowsAction.Activated += (sender, e) => this.OnClearRows();

			this.copyRowAction = new Gtk.Action( "copy_row", "_Copy row", "copy row", "copy" )  { IconName = "clrd-copy" };
            this.copyRowAction.Activated += (sender, e) => this.OnCopyRow();

			this.fillRowAction = new Gtk.Action( "fill_row", "_Fill row", "fill row", "paste" ) { IconName = "clrd-paste" };
            this.fillRowAction.Activated += (sender, e) => this.OnFillRow();

			this.addColumnsAction = new Gtk.Action( "add_columns", "_Add columns", "add columns", "add" ) { IconName = "clrd-add" };
            this.addColumnsAction.Activated += (sender, e) => this.OnAddColumns();

			this.removeColumnsAction = new Gtk.Action( "remove_columns", "_Remove columns", "remove columns", "remove" ) { IconName = "clrd-remove" };
            this.removeColumnsAction.Activated += (sender, e) => this.OnRemoveColumns();

			this.clearColumnsAction = new Gtk.Action( "clear_Columns", "_Clear columns", "clear columns", "clear" ) { IconName = "clrd-clear" };
            this.clearColumnsAction.Activated += (sender, e) => this.OnClearColumns();

			this.copyColumnAction = new Gtk.Action( "copy_column", "_Copy column", "copy column", "copy" )  { IconName = "clrd-copy" };
            this.copyColumnAction.Activated += (sender, e) => this.OnCopyColumn();

			this.fillColumnAction = new Gtk.Action( "fill_column", "_Fill column", "fill column", "paste" )  { IconName = "clrd-paste" };
            this.fillColumnAction.Activated += (sender, e) => this.OnFillColumn();
        }

        void BuildStatusBar()
        {
            var hPanel = new Gtk.HBox( false, 2 );
            this.lblCount = new Gtk.Label( "..." );
            this.lblType = new Gtk.Label( "..." );

            this.sbStatus = new Gtk.Statusbar();

            hPanel.PackStart( this.lblType, true, false, 5 );
            hPanel.PackStart( this.lblCount, true, false, 5 );

            this.sbStatus.PackStart( hPanel, false, false, 5 );
        }

        void BuildMenu()
        {
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

            this.mRecent = new Gtk.Menu();

			var opNew = this.newAction.CreateMenuItem();
            opNew.AddAccelerator( "activate", accelGroup, new Gtk.AccelKey(
                Gdk.Key.n, Gdk.ModifierType.ControlMask, Gtk.AccelFlags.Visible) );

            var opOpen = this.openAction.CreateMenuItem();
            opOpen.AddAccelerator( "activate", accelGroup, new Gtk.AccelKey(
                Gdk.Key.o, Gdk.ModifierType.ControlMask, Gtk.AccelFlags.Visible) );

            var opRecent = new Gtk.MenuItem("_Recent") { Submenu = this.mRecent };

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

            var opRemoveRows = this.removeRowsAction.CreateMenuItem();
            opRemoveRows.AddAccelerator( "activate", accelGroup, new Gtk.AccelKey(
                Gdk.Key.Delete, Gdk.ModifierType.None, Gtk.AccelFlags.Visible) );

            mFile.Append( opNew );
            mFile.Append( opOpen );
            mFile.Append( opRecent );
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
            mRows.Append( opRemoveRows );
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

        void BuildToolbar()
        {
            this.tbTools = new Gtk.Toolbar { ToolbarStyle = this.ToolbarMode };
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

        void BuildPopup()
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

		Gtk.ToolbarStyle ToolbarMode {
			get; set;
		}

        // Widgets
        Gtk.TreeView tvTable;
        Gtk.Statusbar sbStatus;
        Gtk.Toolbar tbTools;
        Gtk.MenuBar menuBar;
        Gtk.Menu popup;
        Gtk.Menu mRecent;
        Gtk.Entry edFind;
        Gtk.Label lblType;
        Gtk.Label lblCount;

        // Actions
        Gtk.Action newAction;
        Gtk.Action openAction;
        Gtk.Action saveAction;
        Gtk.Action saveAsAction;
        Gtk.Action propertiesAction;
        Gtk.Action closeAction;
        Gtk.Action importAction;
        Gtk.Action exportAction;
        Gtk.Action revertAction;
        Gtk.Action quitAction;
        Gtk.Action findAction;
        Gtk.Action findAgainAction;
        Gtk.Action insertFormulaAction;
        Gtk.Action addRowsAction;
        Gtk.Action removeRowsAction;
        Gtk.Action clearRowsAction;
        Gtk.Action copyRowAction;
        Gtk.Action fillRowAction;
        Gtk.Action addColumnsAction;
        Gtk.Action removeColumnsAction;
        Gtk.Action clearColumnsAction;
        Gtk.Action copyColumnAction;
        Gtk.Action fillColumnAction;
        Gtk.Action aboutAction;

		// Icons
		Gdk.Pixbuf iconAbout;
		Gdk.Pixbuf iconAdd;
		Gdk.Pixbuf iconClear;
		Gdk.Pixbuf iconClose;
		Gdk.Pixbuf iconCopy;
		Gdk.Pixbuf iconExit;
		Gdk.Pixbuf iconExport;
		Gdk.Pixbuf iconFind;
		Gdk.Pixbuf iconFormula;
		Gdk.Pixbuf iconImport;
		Gdk.Pixbuf iconNew;
		Gdk.Pixbuf iconOpen;
		Gdk.Pixbuf iconPaste;
		Gdk.Pixbuf iconProperties;
		Gdk.Pixbuf iconRemove;
		Gdk.Pixbuf iconRevert;
		Gdk.Pixbuf iconSave;
    }
}

