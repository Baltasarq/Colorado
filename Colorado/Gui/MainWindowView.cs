// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Gui;


public partial class MainWindow {
    void Build()
    {
        var vPanel = new Gtk.Box( Gtk.Orientation.Vertical, 2 );
        var hPanel = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );

        // Create components
        this._edFind.Activated += (sender, e) => this.OnEdFindActivated();
        this._edFind.FocusInEvent += (sender, e) => this._edFind.Text = "";
        this._edFind.FocusOutEvent += (sender, e) => this._edFind.Text = "Find...";

        // Buil'em all
        this.BuildIcons();
        this.BuildActions();
        this.BuildStatusBar();
        this.BuildMenu();
        this.BuildToolbar();
        this.BuildPopup();

        // Create layout
        hPanel.PackStart( this._tbTools, true, true, 0 );
        hPanel.PackStart( this._edFind, false, false, 0 );

        vPanel.PackStart( this._menuBar, false, false, 0 );
        vPanel.PackStart( hPanel, false, false, 0 );
        vPanel.PackStart( this._mainPanel, true, true, 0 );
        vPanel.PackStart( this._sbStatus, false, false, 0 );

        // Add to this
        this.Add( vPanel );

        // Polishing
        var minSize = new Gdk.Size( 640, 480 );
        this.SetSizeRequest( minSize.Width, minSize.Height );
        this.SetDefaultSize( minSize.Height, minSize.Width );
        this._sbStatus.Push( 0, "Ready" );
        this.SetPosition( Gtk.WindowPosition.Center );
        this.DeleteEvent += (o, args) => { args.RetVal = this.OnQuit(); };
    }

    void BuildIcons()
    {
        this.ToolbarMode = Gtk.ToolbarStyle.Icons;

        try {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();

            this.Icon = new Gdk.Pixbuf( asm,
                "Colorado.assets.colorado.png", 32, 32 );

            this._iconAbout = new Gdk.Pixbuf( asm,
                "Colorado.assets.about.png", 32, 32 );

            this._iconAdd = new Gdk.Pixbuf( asm,
                "Colorado.assets.add.png", 32, 32 );

            this._iconClear = new Gdk.Pixbuf( asm,
                "Colorado.assets.clear.png", 32, 32 );

            this._iconClose = new Gdk.Pixbuf( asm,
                "Colorado.assets.close.png", 32, 32 );

            this._iconCopy = new Gdk.Pixbuf( asm,
                "Colorado.assets.copy.png", 32, 32 );

            this._iconExit = new Gdk.Pixbuf( asm,
                "Colorado.assets.exit.png", 32, 32 );

            this._iconExport = new Gdk.Pixbuf( asm,
                "Colorado.assets.export.png", 32, 32 );

            this._iconFind = new Gdk.Pixbuf( asm,
                "Colorado.assets.find.png", 32, 32 );

            this._iconFormula = new Gdk.Pixbuf( asm,
                "Colorado.assets.formula.png", 32, 32 );

            this._iconImport = new Gdk.Pixbuf( asm,
                "Colorado.assets.import.png", 32, 32 );

            this._iconNew = new Gdk.Pixbuf( asm,
                "Colorado.assets.new.png", 32, 32 );

            this._iconOpen = new Gdk.Pixbuf( asm,
                "Colorado.assets.open.png", 32, 32 );

            this._iconPaste = new Gdk.Pixbuf( asm,
                "Colorado.assets.paste.png", 32, 32 );

            this._iconProperties = new Gdk.Pixbuf( asm,
                "Colorado.assets.properties.png", 32, 32 );

            this._iconRemove = new Gdk.Pixbuf( asm,
                "Colorado.assets.remove.png", 32, 32 );

            this._iconRevert = new Gdk.Pixbuf( asm,
                "Colorado.assets.revert.png", 32, 32 );

            this._iconSave = new Gdk.Pixbuf( asm,
                "Colorado.assets.save.png", 32, 32 );

            this._iconSort = new Gdk.Pixbuf( asm,
                "Colorado.assets.sort.png", 32, 32 );

            this._openAction.Icon = this._iconOpen;
            this._newAction.Icon = this._iconNew;
            this._saveAction.Icon = this._iconSave;
            this._propertiesAction.Icon = this._iconProperties;
            this._closeAction.Icon = this._iconClose;
            this._importAction.Icon = this._iconImport;
            this._exportAction.Icon = this._iconImport;
            this._revertAction.Icon = this._iconRevert;
            this._findAction.Icon = this._iconFind;
            this._insertFormulaAction.Icon = this._iconFormula;
            this._addRowsAction.Icon = this._iconAdd;
            this._removeRowsAction.Icon = this._iconRemove;
            this._clearRowsAction.Icon = this._iconClear;
            this._copyRowAction.Icon = this._iconCopy;
            this._sortRowsAction.Icon = this._iconSort;
            this._addColumnsAction.Icon = this._iconAdd;
            this._removeRowsAction.Icon = this._iconRemove;
            this._clearRowsAction.Icon = this._iconClear;
            this._copyColumnAction.Icon = this._iconCopy;
        } catch (Exception) {
            // No icons -- get over it
            this.ToolbarMode = Gtk.ToolbarStyle.Text;
        }
    }

    void BuildActions()
    {
        this._newAction.Activated += (sender, e) => this.OnNew();
        this._openAction.Activated += (sender, e) => this.OnOpen();
        this._saveAction.Activated += (sender, e) => this.OnSave();
        this._saveAsAction.Activated += (sender, e) => this.OnSaveAs();
        this._propertiesAction.Activated += (sender, e) => this.OnProperties();
        this._closeAction.Activated += (sender, e) => this.CloseSpreadSheet();
        this._aboutAction.Activated += (sender, e) => this.OnAbout();
        this._importAction.Activated += (sender, e) => this.OnImport();
        this._exportAction.Activated += (sender, e) => this.OnExport();
        this._revertAction.Activated += (sender, e) => this.OnRevert();
        this._quitAction.Activated += (sender, e) => this.OnQuit();
        this._findAction.Activated += (sender, e) => this.OnFind();
        this._findAgainAction.Activated += (sender, e) => this.OnFindAgain();
        this._insertFormulaAction.Activated += (sender, e) => this.OnInsertFormula();
        this._addRowsAction.Activated += (sender, e) => this.OnAddRows();
        this._removeRowsAction.Activated += (sender, e) => this.OnRemoveRows();
        this._clearRowsAction.Activated += (sender, e) => this.OnClearRows();
        this._copyRowAction.Activated += (sender, e) => this.OnCopyRow();
        this._fillRowAction.Activated += (sender, e) => this.OnFillRow();
        this._sortRowsAction.Activated += (sender, e) => this.OnSortRows();
        this._addColumnsAction.Activated += (sender, e) => this.OnAddColumns();
        this._removeColumnsAction.Activated += (sender, e) => this.OnRemoveColumns();
        this._clearColumnsAction.Activated += (sender, e) => this.OnClearColumns();
        this._copyColumnAction.Activated += (sender, e) => this.OnCopyColumn();
        this._fillColumnAction.Activated += (sender, e) => this.OnFillColumn();
    }

    void BuildStatusBar()
    {
        var hPanel = new Gtk.Box( Gtk.Orientation.Horizontal, 2 );

        hPanel.PackStart( this._lblType, true, false, 5 );
        hPanel.PackStart( this._lblCount, true, false, 5 );

        this._sbStatus.PackStart( hPanel, false, false, 5 );
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

        var opNew = this._newAction.CreateMenuItem();
        this._newAction.SetAccelerator( Gdk.Key.N, Gdk.ModifierType.ControlMask );

        var opOpen = this._openAction.CreateMenuItem();
        this._openAction.SetAccelerator( Gdk.Key.O, Gdk.ModifierType.ControlMask );

        var opRecent = new Gtk.MenuItem("_Recent") { Submenu = this._mRecent };

        var opSave = this._saveAction.CreateMenuItem();
        this._saveAction.SetAccelerator( Gdk.Key.S, Gdk.ModifierType.ControlMask );

        var opSaveAs = this._saveAsAction.CreateMenuItem();

        var opClose = this._closeAction.CreateMenuItem();

        var opProperties = this._propertiesAction.CreateMenuItem();
        this._propertiesAction.SetAccelerator( Gdk.Key.F2, Gdk.ModifierType.None );

        var opQuit = this._quitAction.CreateMenuItem();
        this._quitAction.SetAccelerator( Gdk.Key.Q, Gdk.ModifierType.ControlMask );

        var opAddRows = this._addRowsAction.CreateMenuItem();
        this._addRowsAction.SetAccelerator( Gdk.Key.Insert, Gdk.ModifierType.ControlMask );

        var opRemoveRows = this._removeRowsAction.CreateMenuItem();
        this._removeRowsAction.SetAccelerator( Gdk.Key.Delete, Gdk.ModifierType.ControlMask );

        var opFind = this._findAction.CreateMenuItem();
        this._findAction.SetAccelerator( Gdk.Key.F, Gdk.ModifierType.ControlMask );

        var opFindAgain = this._findAgainAction.CreateMenuItem();
        this._findAgainAction.SetAccelerator( Gdk.Key.F3, Gdk.ModifierType.None );

        mFile.Append( opNew );
        mFile.Append( opOpen );
        mFile.Append( opRecent );
        mFile.Append( opSave );
        mFile.Append( opSaveAs );
        mFile.Append( new Gtk.SeparatorMenuItem() );
        mFile.Append( opProperties );
        mFile.Append( opClose );
        mFile.Append( new Gtk.SeparatorMenuItem() );
        mFile.Append( this._importAction.CreateMenuItem() );
        mFile.Append( this._exportAction.CreateMenuItem() );
        mFile.Append( this._revertAction.CreateMenuItem() );
        mFile.Append( opQuit );

        mEdit.Append( opFind );
        mEdit.Append( opFindAgain );
        mEdit.Append( this._insertFormulaAction.CreateMenuItem() );
        mEdit.Append( new Gtk.SeparatorMenuItem() );
        mEdit.Append( miRows );
        mEdit.Append( miColumns );
        mRows.Append( opAddRows );
        mRows.Append( opRemoveRows );
        mRows.Append( this._clearRowsAction.CreateMenuItem() );
        mRows.Append( this._copyRowAction.CreateMenuItem() );
        mRows.Append( this._fillRowAction.CreateMenuItem() );
        mRows.Append( this._sortRowsAction.CreateMenuItem() );
        mColumns.Append( this._addColumnsAction.CreateMenuItem() );
        mColumns.Append( this._removeColumnsAction.CreateMenuItem() );
        mColumns.Append( this._clearColumnsAction.CreateMenuItem() );
        mColumns.Append( this._copyColumnAction.CreateMenuItem() );
        mColumns.Append( this._fillColumnAction.CreateMenuItem() );

        mHelp.Append( this._aboutAction.CreateMenuItem() );

        this._menuBar.Append( miFile );
        this._menuBar.Append( miEdit );
        this._menuBar.Append( miHelp );
        this.AddAccelGroup( GtkUtil.UIAction.AccelGroup );
    }

    void BuildToolbar()
    {
        this._tbTools.Insert( this._newAction.CreateToolButton(), 0 );
        this._tbTools.Insert( this._openAction.CreateToolButton(), 1 );
        this._tbTools.Insert( this._saveAction.CreateToolButton(), 2 );
        this._tbTools.Insert( this._propertiesAction.CreateToolButton(), 3 );
        this._tbTools.Insert( this._closeAction.CreateToolButton(), 4 );
        this._tbTools.Insert( new Gtk.SeparatorToolItem(), 5 );
        this._tbTools.Insert( this._addRowsAction.CreateToolButton(), 6 );
        this._tbTools.Insert( this._removeRowsAction.CreateToolButton(), 7 );
        this._tbTools.Insert( this._clearRowsAction.CreateToolButton(), 8 );
        this._tbTools.Insert( this._copyRowAction.CreateToolButton(), 9 );
        this._tbTools.Insert( this._fillRowAction.CreateToolButton(), 10 );
        this._tbTools.Insert( new Gtk.SeparatorToolItem(), 11 );
        this._tbTools.Insert( this._addColumnsAction.CreateToolButton(), 12 );
        this._tbTools.Insert( this._removeColumnsAction.CreateToolButton(), 13 );
        this._tbTools.Insert( this._clearColumnsAction.CreateToolButton(), 14 );
        this._tbTools.Insert( this._copyColumnAction.CreateToolButton(), 15 );
        this._tbTools.Insert( this._fillColumnAction.CreateToolButton(), 16 );
    }

    void BuildPopup()
    {
        // Rows
        this._popup.Append( this._addRowsAction.CreateMenuItem() );
        this._popup.Append( this._removeRowsAction.CreateMenuItem() );
        this._popup.Append( this._clearRowsAction.CreateMenuItem() );
        this._popup.Append( this._copyRowAction.CreateMenuItem() );
        this._popup.Append( this._fillRowAction.CreateMenuItem() );

        // Columns
        this._popup.Append( new Gtk.SeparatorMenuItem() );
        this._popup.Append( this._addColumnsAction.CreateMenuItem() );
        this._popup.Append( this._removeColumnsAction.CreateMenuItem() );
        this._popup.Append( this._clearColumnsAction.CreateMenuItem() );
        this._popup.Append( this._copyColumnAction.CreateMenuItem() );
        this._popup.Append( this._fillColumnAction.CreateMenuItem() );

        // General
        this._popup.Append( new Gtk.SeparatorMenuItem() );
        this._popup.Append( this._propertiesAction.CreateMenuItem() );
        this._popup.Append( this._closeAction.CreateMenuItem() );

        // Finish
        this._popup.ShowAll();
    }

    Gtk.ToolbarStyle ToolbarMode {
        get; set;
    }

    // Icons
    Gdk.Pixbuf? _iconAbout;
    Gdk.Pixbuf? _iconAdd;
    Gdk.Pixbuf? _iconClear;
    Gdk.Pixbuf? _iconClose;
    Gdk.Pixbuf? _iconCopy;
    Gdk.Pixbuf? _iconExit;
    Gdk.Pixbuf? _iconExport;
    Gdk.Pixbuf? _iconFind;
    Gdk.Pixbuf? _iconFormula;
    Gdk.Pixbuf? _iconImport;
    Gdk.Pixbuf? _iconNew;
    Gdk.Pixbuf? _iconOpen;
    Gdk.Pixbuf? _iconPaste;
    Gdk.Pixbuf? _iconProperties;
    Gdk.Pixbuf? _iconRemove;
    Gdk.Pixbuf? _iconRevert;
    Gdk.Pixbuf? _iconSave;
    Gdk.Pixbuf? _iconSort;

    // Widgets
    Gtk.Statusbar _sbStatus;
    Gtk.Toolbar _tbTools;
    Gtk.MenuBar _menuBar;
    Gtk.Menu _popup;
    Gtk.Menu _mRecent;
    Gtk.Entry _edFind;
    Gtk.Label _lblType;
    Gtk.Label _lblCount;
    Gtk.ScrolledWindow _mainPanel;

    // Actions
    readonly GtkUtil.UIAction _newAction;
    readonly GtkUtil.UIAction _openAction;
    readonly GtkUtil.UIAction _saveAction;
    readonly GtkUtil.UIAction _saveAsAction;
    readonly GtkUtil.UIAction _propertiesAction;
    readonly GtkUtil.UIAction _closeAction;
    readonly GtkUtil.UIAction _importAction;
    readonly GtkUtil.UIAction _exportAction;
    readonly GtkUtil.UIAction _revertAction;
    readonly GtkUtil.UIAction _quitAction;
    readonly GtkUtil.UIAction _findAction;
    readonly GtkUtil.UIAction _findAgainAction;
    readonly GtkUtil.UIAction _insertFormulaAction;
    readonly GtkUtil.UIAction _addRowsAction;
    readonly GtkUtil.UIAction _removeRowsAction;
    readonly GtkUtil.UIAction _clearRowsAction;
    readonly GtkUtil.UIAction _copyRowAction;
    readonly GtkUtil.UIAction _fillRowAction;
    readonly GtkUtil.UIAction _sortRowsAction;
    readonly GtkUtil.UIAction _addColumnsAction;
    readonly GtkUtil.UIAction _removeColumnsAction;
    readonly GtkUtil.UIAction _clearColumnsAction;
    readonly GtkUtil.UIAction _copyColumnAction;
    readonly GtkUtil.UIAction _fillColumnAction;
    readonly GtkUtil.UIAction _aboutAction;
}
