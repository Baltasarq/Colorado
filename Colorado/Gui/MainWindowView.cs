// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>


namespace Colorado.Gui {
    public partial class MainWindow {
        void Build()
        {
            var vPanel = new Gtk.Box( Gtk.Orientation.Vertical, 2 );
            var hPanel = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );

            // Create components
            this.edFind.Activated += (sender, e) => this.OnEdFindActivated();
            this.edFind.FocusInEvent += (sender, e) => this.edFind.Text = "";
            this.edFind.FocusOutEvent += (sender, e) => this.edFind.Text = "Find...";

            // Create tree view
            var swScroll = new Gtk.ScrolledWindow();
            swScroll.Add( this.tvTable );

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
                var asm = System.Reflection.Assembly.GetExecutingAssembly();

                this.Icon = new Gdk.Pixbuf( asm,
                    "Colorado.assets.colorado.png", 32, 32 );

				this.iconAbout = new Gdk.Pixbuf( asm,
					"Colorado.assets.about.png", 32, 32 );

				this.iconAdd = new Gdk.Pixbuf( asm,
					"Colorado.assets.add.png", 32, 32 );

				this.iconClear = new Gdk.Pixbuf( asm,
					"Colorado.assets.clear.png", 32, 32 );

				this.iconClose = new Gdk.Pixbuf( asm,
					"Colorado.assets.close.png", 32, 32 );

				this.iconCopy = new Gdk.Pixbuf( asm,
					"Colorado.assets.copy.png", 32, 32 );

				this.iconExit = new Gdk.Pixbuf( asm,
					"Colorado.assets.exit.png", 32, 32 );

				this.iconExport = new Gdk.Pixbuf( asm,
					"Colorado.assets.export.png", 32, 32 );

				this.iconFind = new Gdk.Pixbuf( asm,
					"Colorado.assets.find.png", 32, 32 );

				this.iconFormula = new Gdk.Pixbuf( asm,
					"Colorado.assets.formula.png", 32, 32 );

				this.iconImport = new Gdk.Pixbuf( asm,
					"Colorado.assets.import.png", 32, 32 );

				this.iconNew = new Gdk.Pixbuf( asm,
					"Colorado.assets.new.png", 32, 32 );

				this.iconOpen = new Gdk.Pixbuf( asm,
					"Colorado.assets.open.png", 32, 32 );

				this.iconPaste = new Gdk.Pixbuf( asm,
					"Colorado.assets.paste.png", 32, 32 );

				this.iconProperties = new Gdk.Pixbuf( asm,
					"Colorado.assets.properties.png", 32, 32 );

				this.iconRemove = new Gdk.Pixbuf( asm,
					"Colorado.assets.remove.png", 32, 32 );

				this.iconRevert = new Gdk.Pixbuf( asm,
					"Colorado.assets.revert.png", 32, 32 );

				this.iconSave = new Gdk.Pixbuf( asm,
					"Colorado.assets.save.png", 32, 32 );

                this.iconSort = new Gdk.Pixbuf( asm,
					"Colorado.assets.sort.png", 32, 32 );

                this.openAction.Icon = this.iconOpen;
                this.newAction.Icon = this.iconNew;
                this.saveAction.Icon = this.iconSave;
                this.propertiesAction.Icon = this.iconProperties;
                this.closeAction.Icon = this.iconClose;
                this.importAction.Icon = this.iconImport;
                this.exportAction.Icon = this.iconImport;
                this.revertAction.Icon = this.iconRevert;
                this.findAction.Icon = this.iconFind;
                this.insertFormulaAction.Icon = this.iconFormula;
                this.addRowsAction.Icon = this.iconAdd;
                this.removeRowsAction.Icon = this.iconRemove;
                this.clearRowsAction.Icon = this.iconClear;
                this.copyRowAction.Icon = this.iconCopy;
                this.sortRowsAction.Icon = this.iconSort;
                this.addColumnsAction.Icon = this.iconAdd;
                this.removeRowsAction.Icon = this.iconRemove;
                this.clearRowsAction.Icon = this.iconClear;
                this.copyColumnAction.Icon = this.iconCopy;
            } catch (Exception) {
                // No icons -- get over it
                this.ToolbarMode = Gtk.ToolbarStyle.Text;
            }
		}

        void BuildActions()
        {
            this.newAction.Activated += (sender, e) => this.OnNew();
            this.openAction.Activated += (sender, e) => this.OnOpen();
            this.saveAction.Activated += (sender, e) => this.OnSave();
            this.saveAsAction.Activated += (sender, e) => this.OnSaveAs();
            this.propertiesAction.Activated += (sender, e) => this.OnProperties();
            this.closeAction.Activated += (sender, e) => this.CloseDocument();
            this.aboutAction.Activated += (sender, e) => this.OnAbout();
            this.importAction.Activated += (sender, e) => this.OnImport();
            this.exportAction.Activated += (sender, e) => this.OnExport();
            this.revertAction.Activated += (sender, e) => this.OnRevert();
            this.quitAction.Activated += (sender, e) => this.OnQuit();
            this.findAction.Activated += (sender, e) => this.OnFind();
            this.findAgainAction.Activated += (sender, e) => this.OnFindAgain();
            this.insertFormulaAction.Activated += (sender, e) => this.OnInsertFormula();
            this.addRowsAction.Activated += (sender, e) => this.OnAddRows();
            this.removeRowsAction.Activated += (sender, e) => this.OnRemoveRows();
            this.clearRowsAction.Activated += (sender, e) => this.OnClearRows();
            this.copyRowAction.Activated += (sender, e) => this.OnCopyRow();
            this.fillRowAction.Activated += (sender, e) => this.OnFillRow();
            this.sortRowsAction.Activated += (sender, e) => this.OnSortRows();
            this.addColumnsAction.Activated += (sender, e) => this.OnAddColumns();
            this.removeColumnsAction.Activated += (sender, e) => this.OnRemoveColumns();
            this.clearColumnsAction.Activated += (sender, e) => this.OnClearColumns();
            this.copyColumnAction.Activated += (sender, e) => this.OnCopyColumn();
            this.fillColumnAction.Activated += (sender, e) => this.OnFillColumn();
        }

        void BuildStatusBar()
        {
            var hPanel = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );

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

            //var accelGroup = UIAction.AccelGroup;
            miFile.Submenu = mFile;
            miEdit.Submenu = mEdit;
            miRows.Submenu = mRows;
            miColumns.Submenu = mColumns;
            miHelp.Submenu = mHelp;

			var opNew = this.newAction.CreateMenuItem();
            this.newAction.SetAccelerator( Gdk.Key.N, Gdk.ModifierType.ControlMask );

            var opOpen = this.openAction.CreateMenuItem();
            this.openAction.SetAccelerator( Gdk.Key.O, Gdk.ModifierType.ControlMask );

            var opRecent = new Gtk.MenuItem("_Recent") { Submenu = this.mRecent };

            var opSave = this.saveAction.CreateMenuItem();
            this.saveAction.SetAccelerator( Gdk.Key.S, Gdk.ModifierType.ControlMask );

            var opSaveAs = this.saveAsAction.CreateMenuItem();

            var opClose = this.closeAction.CreateMenuItem();

            var opProperties = this.propertiesAction.CreateMenuItem();
            this.propertiesAction.SetAccelerator( Gdk.Key.F2, Gdk.ModifierType.None );

            var opQuit = this.quitAction.CreateMenuItem();
            this.quitAction.SetAccelerator( Gdk.Key.Q, Gdk.ModifierType.ControlMask );

            var opAddRows = this.addRowsAction.CreateMenuItem();
            this.addRowsAction.SetAccelerator( Gdk.Key.Insert, Gdk.ModifierType.ControlMask );

            var opRemoveRows = this.removeRowsAction.CreateMenuItem();
            this.removeRowsAction.SetAccelerator( Gdk.Key.Delete, Gdk.ModifierType.ControlMask );

            var opFind = this.findAction.CreateMenuItem();
            this.findAction.SetAccelerator( Gdk.Key.F, Gdk.ModifierType.ControlMask );

            var opFindAgain = this.findAgainAction.CreateMenuItem();
            this.findAgainAction.SetAccelerator( Gdk.Key.F3, Gdk.ModifierType.None );

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
            mRows.Append( this.sortRowsAction.CreateMenuItem() );
            mColumns.Append( this.addColumnsAction.CreateMenuItem() );
            mColumns.Append( this.removeColumnsAction.CreateMenuItem() );
            mColumns.Append( this.clearColumnsAction.CreateMenuItem() );
            mColumns.Append( this.copyColumnAction.CreateMenuItem() );
            mColumns.Append( this.fillColumnAction.CreateMenuItem() );

            mHelp.Append( this.aboutAction.CreateMenuItem() );

            this.menuBar.Append( miFile );
            this.menuBar.Append( miEdit );
            this.menuBar.Append( miHelp );
            this.AddAccelGroup( GtkUtil.UIAction.AccelGroup );
        }

        void BuildToolbar()
        {
            this.tbTools.Insert( this.newAction.CreateToolButton(), 0 );
            this.tbTools.Insert( this.openAction.CreateToolButton(), 1 );
            this.tbTools.Insert( this.saveAction.CreateToolButton(), 2 );
            this.tbTools.Insert( this.propertiesAction.CreateToolButton(), 3 );
            this.tbTools.Insert( this.closeAction.CreateToolButton(), 4 );
            this.tbTools.Insert( new Gtk.SeparatorToolItem(), 5 );
            this.tbTools.Insert( this.addRowsAction.CreateToolButton(), 6 );
            this.tbTools.Insert( this.removeRowsAction.CreateToolButton(), 7 );
            this.tbTools.Insert( this.clearRowsAction.CreateToolButton(), 8 );
            this.tbTools.Insert( this.copyRowAction.CreateToolButton(), 9 );
            this.tbTools.Insert( this.fillRowAction.CreateToolButton(), 10 );
            this.tbTools.Insert( new Gtk.SeparatorToolItem(), 11 );
            this.tbTools.Insert( this.addColumnsAction.CreateToolButton(), 12 );
            this.tbTools.Insert( this.removeColumnsAction.CreateToolButton(), 13 );
            this.tbTools.Insert( this.clearColumnsAction.CreateToolButton(), 14 );
            this.tbTools.Insert( this.copyColumnAction.CreateToolButton(), 15 );
            this.tbTools.Insert( this.fillColumnAction.CreateToolButton(), 16 );
        }

        void BuildPopup()
        {
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

        // Icons
        Gdk.Pixbuf? iconAbout;
		Gdk.Pixbuf? iconAdd;
		Gdk.Pixbuf? iconClear;
		Gdk.Pixbuf? iconClose;
		Gdk.Pixbuf? iconCopy;
		Gdk.Pixbuf? iconExit;
		Gdk.Pixbuf? iconExport;
		Gdk.Pixbuf? iconFind;
		Gdk.Pixbuf? iconFormula;
		Gdk.Pixbuf? iconImport;
		Gdk.Pixbuf? iconNew;
		Gdk.Pixbuf? iconOpen;
		Gdk.Pixbuf? iconPaste;
		Gdk.Pixbuf? iconProperties;
		Gdk.Pixbuf? iconRemove;
		Gdk.Pixbuf? iconRevert;
		Gdk.Pixbuf? iconSave;
        Gdk.Pixbuf? iconSort;

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
        readonly GtkUtil.UIAction newAction;
        readonly GtkUtil.UIAction openAction;
        readonly GtkUtil.UIAction saveAction;
        readonly GtkUtil.UIAction saveAsAction;
        readonly GtkUtil.UIAction propertiesAction;
        readonly GtkUtil.UIAction closeAction;
        readonly GtkUtil.UIAction importAction;
        readonly GtkUtil.UIAction exportAction;
        readonly GtkUtil.UIAction revertAction;
        readonly GtkUtil.UIAction quitAction;
        readonly GtkUtil.UIAction findAction;
        readonly GtkUtil.UIAction findAgainAction;
        readonly GtkUtil.UIAction insertFormulaAction;
        readonly GtkUtil.UIAction addRowsAction;
        readonly GtkUtil.UIAction removeRowsAction;
        readonly GtkUtil.UIAction clearRowsAction;
        readonly GtkUtil.UIAction copyRowAction;
        readonly GtkUtil.UIAction fillRowAction;
        readonly GtkUtil.UIAction sortRowsAction;
        readonly GtkUtil.UIAction addColumnsAction;
        readonly GtkUtil.UIAction removeColumnsAction;
        readonly GtkUtil.UIAction clearColumnsAction;
        readonly GtkUtil.UIAction copyColumnAction;
        readonly GtkUtil.UIAction fillColumnAction;
        readonly GtkUtil.UIAction aboutAction;
    }
}

