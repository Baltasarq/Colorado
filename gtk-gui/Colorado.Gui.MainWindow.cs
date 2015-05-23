
// This file has been generated by the GUI designer. Do not modify.
namespace Colorado.Gui
{
	public partial class MainWindow
	{
		private global::Gtk.UIManager UIManager;
		
		private global::Gtk.Action FileAction;
		
		private global::Gtk.Action HelpAction;
		
		private global::Gtk.Action ToolsAction;
		
		private global::Gtk.Action dfdAction;
		
		private global::Gtk.Action OpenAction;
		
		private global::Gtk.Action ExitAction;
		
		private global::Gtk.Action FindAction;
		
		private global::Gtk.Action AboutAction;
		
		private global::Gtk.Action ImportAction;
		
		private global::Gtk.Action SaveAction;
		
		private global::Gtk.Action SaveAsAction;
		
		private global::Gtk.Action convertAction;
		
		private global::Gtk.Action closeAction;
		
		private global::Gtk.Action propertiesAction;
		
		private global::Gtk.Action EditAction;
		
		private global::Gtk.Action RowsAction;
		
		private global::Gtk.Action ColumnsAction;
		
		private global::Gtk.Action addRows;
		
		private global::Gtk.Action removeAction;
		
		private global::Gtk.Action addColumns;
		
		private global::Gtk.Action removeColumnAction;
		
		private global::Gtk.Action CleanRowAction;
		
		private global::Gtk.Action CopyRowAction;
		
		private global::Gtk.Action newAction;
		
		private global::Gtk.Action RowAction;
		
		private global::Gtk.Action clearAction;
		
		private global::Gtk.Action gotoBottomAction;
		
		private global::Gtk.Action clearColumnAction;
		
		private global::Gtk.Action copyColumnAction;
		
		private global::Gtk.Action FindAgainAction;
		
		private global::Gtk.Action revertToSavedAction;
		
		private global::Gtk.Action btNew;
		
		private global::Gtk.Action btOpen;
		
		private global::Gtk.Action btSave;
		
		private global::Gtk.Action btProperties;
		
		private global::Gtk.Action btFind;
		
		private global::Gtk.Action btAdd;
		
		private global::Gtk.Action btRemove;
		
		private global::Gtk.Action ViewAction;
		
		private global::Gtk.Action ToolbarAction;
		
		private global::Gtk.ToggleAction viewToolbarAction;
		
		private global::Gtk.Action btClearRow;
		
		private global::Gtk.Action btCopyRow;
		
		private global::Gtk.Action btAddColumns;
		
		private global::Gtk.Action btRemoveColumn;
		
		private global::Gtk.Action btClearColumn;
		
		private global::Gtk.Action btCopyColumn;
		
		private global::Gtk.Action kAction;
		
		private global::Gtk.Action btInsert;
		
		private global::Gtk.Action insertRows;
		
		private global::Gtk.Action insertColumns;
		
		private global::Gtk.Action btInsertColumns;
		
		private global::Gtk.Action fillRow;
		
		private global::Gtk.Action fillColumn;
		
		private global::Gtk.Action btFillRow;
		
		private global::Gtk.Action btFillColumn;
		
		private global::Gtk.Action insertFormulaAction;
		
		private global::Gtk.VBox vbox1;
		
		private global::Gtk.MenuBar menubar2;
		
		private global::Gtk.Toolbar tbToolBar;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		
		private global::Gtk.TreeView tvTable;
		
		private global::Gtk.Statusbar sbStatus;
		
		private global::Gtk.Label lblType;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Colorado.Gui.MainWindow
			this.UIManager = new global::Gtk.UIManager ();
			global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
			this.FileAction = new global::Gtk.Action ("FileAction", "_File", null, null);
			this.FileAction.ShortLabel = "_File";
			w1.Add (this.FileAction, null);
			this.HelpAction = new global::Gtk.Action ("HelpAction", "_Help", null, null);
			this.HelpAction.ShortLabel = "_Help";
			w1.Add (this.HelpAction, null);
			this.ToolsAction = new global::Gtk.Action ("ToolsAction", "_Tools", null, null);
			this.ToolsAction.ShortLabel = "_Edit";
			w1.Add (this.ToolsAction, null);
			this.dfdAction = new global::Gtk.Action ("dfdAction", "dfd", null, null);
			this.dfdAction.ShortLabel = "dfd";
			w1.Add (this.dfdAction, null);
			this.OpenAction = new global::Gtk.Action ("OpenAction", "_Open", null, "gtk-open");
			this.OpenAction.ShortLabel = "_Open";
			w1.Add (this.OpenAction, null);
			this.ExitAction = new global::Gtk.Action ("ExitAction", "_Exit", null, "gtk-quit");
			this.ExitAction.ShortLabel = "_Exit";
			w1.Add (this.ExitAction, null);
			this.FindAction = new global::Gtk.Action ("FindAction", "_Find", null, "gtk-find");
			this.FindAction.ShortLabel = "_Find";
			w1.Add (this.FindAction, null);
			this.AboutAction = new global::Gtk.Action ("AboutAction", "_About", null, "gtk-about");
			this.AboutAction.ShortLabel = "_About";
			w1.Add (this.AboutAction, null);
			this.ImportAction = new global::Gtk.Action ("ImportAction", "_Import", null, "gtk-convert");
			this.ImportAction.ShortLabel = "_Import";
			w1.Add (this.ImportAction, null);
			this.SaveAction = new global::Gtk.Action ("SaveAction", "Save", null, "gtk-save");
			this.SaveAction.ShortLabel = "Save";
			w1.Add (this.SaveAction, null);
			this.SaveAsAction = new global::Gtk.Action ("SaveAsAction", "Save as...", null, "gtk-save-as");
			this.SaveAsAction.ShortLabel = "Save as...";
			w1.Add (this.SaveAsAction, null);
			this.convertAction = new global::Gtk.Action ("convertAction", "Export", null, "gtk-convert");
			this.convertAction.ShortLabel = "Export";
			w1.Add (this.convertAction, null);
			this.closeAction = new global::Gtk.Action ("closeAction", "_Close", null, "gtk-close");
			this.closeAction.ShortLabel = "_Close";
			w1.Add (this.closeAction, null);
			this.propertiesAction = new global::Gtk.Action ("propertiesAction", "_Properties", null, "gtk-properties");
			this.propertiesAction.ShortLabel = "_Properties";
			w1.Add (this.propertiesAction, "F2");
			this.EditAction = new global::Gtk.Action ("EditAction", "_Edit", null, null);
			this.EditAction.ShortLabel = "_Edit";
			w1.Add (this.EditAction, null);
			this.RowsAction = new global::Gtk.Action ("RowsAction", "_Rows", null, null);
			this.RowsAction.ShortLabel = "Rows";
			w1.Add (this.RowsAction, null);
			this.ColumnsAction = new global::Gtk.Action ("ColumnsAction", "_Columns", null, null);
			this.ColumnsAction.ShortLabel = "Columns";
			w1.Add (this.ColumnsAction, null);
			this.addRows = new global::Gtk.Action ("addRows", "_Add rows", null, "gtk-add");
			this.addRows.ShortLabel = "_Insert rows";
			w1.Add (this.addRows, "<Control>plus");
			this.removeAction = new global::Gtk.Action ("removeAction", "_Remove rows", null, "gtk-remove");
			this.removeAction.ShortLabel = "_Remove rows";
			w1.Add (this.removeAction, "<Control>minus");
			this.addColumns = new global::Gtk.Action ("addColumns", "_Add columns", null, "gtk-add");
			this.addColumns.ShortLabel = "_Insert columns";
			w1.Add (this.addColumns, null);
			this.removeColumnAction = new global::Gtk.Action ("removeColumnAction", "_Remove columns", null, "gtk-remove");
			this.removeColumnAction.ShortLabel = "_Remove columns";
			w1.Add (this.removeColumnAction, null);
			this.CleanRowAction = new global::Gtk.Action ("CleanRowAction", "_Clean row", null, null);
			this.CleanRowAction.ShortLabel = "_Clean row";
			w1.Add (this.CleanRowAction, null);
			this.CopyRowAction = new global::Gtk.Action ("CopyRowAction", "Cop_y row", null, null);
			this.CopyRowAction.ShortLabel = "Cop_y row";
			w1.Add (this.CopyRowAction, null);
			this.newAction = new global::Gtk.Action ("newAction", "_New", null, "gtk-new");
			this.newAction.ShortLabel = "_New";
			w1.Add (this.newAction, null);
			this.RowAction = new global::Gtk.Action ("RowAction", "_Row", null, null);
			this.RowAction.ShortLabel = "_Row";
			w1.Add (this.RowAction, null);
			this.clearAction = new global::Gtk.Action ("clearAction", "_Clean row", null, "gtk-clear");
			this.clearAction.ShortLabel = "_Clean row";
			w1.Add (this.clearAction, "<Control>Delete");
			this.gotoBottomAction = new global::Gtk.Action ("gotoBottomAction", "Cop_y row", null, "gtk-goto-bottom");
			this.gotoBottomAction.ShortLabel = "Cop_y row";
			w1.Add (this.gotoBottomAction, "<Alt>c");
			this.clearColumnAction = new global::Gtk.Action ("clearColumnAction", "_Clear column", null, "gtk-clear");
			this.clearColumnAction.ShortLabel = "_Clear column";
			w1.Add (this.clearColumnAction, null);
			this.copyColumnAction = new global::Gtk.Action ("copyColumnAction", "_Copy column", null, "gtk-goto-last");
			this.copyColumnAction.ShortLabel = "_Copy column";
			w1.Add (this.copyColumnAction, null);
			this.FindAgainAction = new global::Gtk.Action ("FindAgainAction", "Find _again", null, "gtk-jump-to");
			this.FindAgainAction.ShortLabel = "Find _again";
			w1.Add (this.FindAgainAction, "F3");
			this.revertToSavedAction = new global::Gtk.Action ("revertToSavedAction", "_Revert", null, "gtk-revert-to-saved");
			this.revertToSavedAction.ShortLabel = "_Revert";
			w1.Add (this.revertToSavedAction, null);
			this.btNew = new global::Gtk.Action ("btNew", null, "New", "gtk-new");
			w1.Add (this.btNew, null);
			this.btOpen = new global::Gtk.Action ("btOpen", null, "Open", "gtk-open");
			w1.Add (this.btOpen, null);
			this.btSave = new global::Gtk.Action ("btSave", null, "Save", "gtk-save");
			w1.Add (this.btSave, null);
			this.btProperties = new global::Gtk.Action ("btProperties", null, "Properties", "gtk-properties");
			w1.Add (this.btProperties, null);
			this.btFind = new global::Gtk.Action ("btFind", null, "Find", "gtk-find");
			w1.Add (this.btFind, null);
			this.btAdd = new global::Gtk.Action ("btAdd", null, "Add rows", "gtk-add");
			w1.Add (this.btAdd, null);
			this.btRemove = new global::Gtk.Action ("btRemove", null, "Remove rows", "gtk-remove");
			w1.Add (this.btRemove, null);
			this.ViewAction = new global::Gtk.Action ("ViewAction", "_View", null, null);
			this.ViewAction.ShortLabel = "_View";
			w1.Add (this.ViewAction, null);
			this.ToolbarAction = new global::Gtk.Action ("ToolbarAction", "_Toolbar", null, null);
			this.ToolbarAction.ShortLabel = "_Toolbar";
			w1.Add (this.ToolbarAction, null);
			this.viewToolbarAction = new global::Gtk.ToggleAction ("viewToolbarAction", "_Toolbar", null, null);
			this.viewToolbarAction.Active = true;
			this.viewToolbarAction.ShortLabel = "_Toolbar";
			w1.Add (this.viewToolbarAction, null);
			this.btClearRow = new global::Gtk.Action ("btClearRow", null, "Clear row values", "gtk-clear");
			w1.Add (this.btClearRow, null);
			this.btCopyRow = new global::Gtk.Action ("btCopyRow", null, "Copy row", "gtk-goto-bottom");
			w1.Add (this.btCopyRow, null);
			this.btAddColumns = new global::Gtk.Action ("btAddColumns", null, "Add columns", "gtk-add");
			w1.Add (this.btAddColumns, null);
			this.btRemoveColumn = new global::Gtk.Action ("btRemoveColumn", null, "Remove columns", "gtk-remove");
			w1.Add (this.btRemoveColumn, null);
			this.btClearColumn = new global::Gtk.Action ("btClearColumn", null, "Clear column values", "gtk-clear");
			w1.Add (this.btClearColumn, null);
			this.btCopyColumn = new global::Gtk.Action ("btCopyColumn", null, "Copy column", "gtk-goto-last");
			w1.Add (this.btCopyColumn, null);
			this.kAction = new global::Gtk.Action ("kAction", "k", null, null);
			this.kAction.ShortLabel = "k";
			w1.Add (this.kAction, null);
			this.btInsert = new global::Gtk.Action ("btInsert", null, "Insert rows", "gtk-indent");
			w1.Add (this.btInsert, null);
			this.insertRows = new global::Gtk.Action ("insertRows", "_Insert rows", null, "gtk-indent");
			this.insertRows.ShortLabel = "_Insert rows";
			w1.Add (this.insertRows, "<Alt>plus");
			this.insertColumns = new global::Gtk.Action ("insertColumns", "_Insert columns", null, "gtk-indent");
			this.insertColumns.ShortLabel = "_Insert columns";
			w1.Add (this.insertColumns, null);
			this.btInsertColumns = new global::Gtk.Action ("btInsertColumns", null, "Insert columns", "gtk-indent");
			w1.Add (this.btInsertColumns, null);
			this.fillRow = new global::Gtk.Action ("fillRow", "_Fill row", null, "gtk-color-picker");
			this.fillRow.ShortLabel = "_Fill row";
			w1.Add (this.fillRow, null);
			this.fillColumn = new global::Gtk.Action ("fillColumn", "_Fill column", null, "gtk-color-picker");
			this.fillColumn.ShortLabel = "_Fill column";
			w1.Add (this.fillColumn, null);
			this.btFillRow = new global::Gtk.Action ("btFillRow", null, null, "gtk-color-picker");
			w1.Add (this.btFillRow, null);
			this.btFillColumn = new global::Gtk.Action ("btFillColumn", null, null, "gtk-color-picker");
			w1.Add (this.btFillColumn, null);
			this.insertFormulaAction = new global::Gtk.Action ("insertFormulaAction", "_Insert formula", null, "gtk-edit");
			this.insertFormulaAction.ShortLabel = "_Insert formula";
			w1.Add (this.insertFormulaAction, null);
			this.UIManager.InsertActionGroup (w1, 0);
			this.AddAccelGroup (this.UIManager.AccelGroup);
			this.Name = "Colorado.Gui.MainWindow";
			this.Title = "Colorado";
			this.Icon = global::Gdk.Pixbuf.LoadFromResource ("Colorado.Res.colorado.ico");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child Colorado.Gui.MainWindow.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString ("<ui><menubar name=\'menubar2\'><menu name=\'FileAction\' action=\'FileAction\'><menuite" +
			"m name=\'newAction\' action=\'newAction\'/><menuitem name=\'OpenAction\' action=\'OpenA" +
			"ction\'/><menuitem name=\'SaveAction\' action=\'SaveAction\'/><menuitem name=\'SaveAsA" +
			"ction\' action=\'SaveAsAction\'/><separator/><menuitem name=\'propertiesAction\' acti" +
			"on=\'propertiesAction\'/><menuitem name=\'closeAction\' action=\'closeAction\'/><separ" +
			"ator/><menuitem name=\'revertToSavedAction\' action=\'revertToSavedAction\'/><menuit" +
			"em name=\'ImportAction\' action=\'ImportAction\'/><menuitem name=\'convertAction\' act" +
			"ion=\'convertAction\'/><menuitem name=\'ExitAction\' action=\'ExitAction\'/></menu><me" +
			"nu name=\'EditAction\' action=\'EditAction\'><menuitem name=\'FindAction\' action=\'Fin" +
			"dAction\'/><menuitem name=\'FindAgainAction\' action=\'FindAgainAction\'/><menuitem n" +
			"ame=\'insertFormulaAction\' action=\'insertFormulaAction\'/><separator/><menu name=\'" +
			"RowsAction\' action=\'RowsAction\'><menuitem name=\'addRows\' action=\'addRows\'/><menu" +
			"item name=\'insertRows\' action=\'insertRows\'/><menuitem name=\'removeAction\' action" +
			"=\'removeAction\'/><menuitem name=\'clearAction\' action=\'clearAction\'/><menuitem na" +
			"me=\'gotoBottomAction\' action=\'gotoBottomAction\'/><menuitem name=\'fillRow\' action" +
			"=\'fillRow\'/></menu><menu name=\'ColumnsAction\' action=\'ColumnsAction\'><menuitem n" +
			"ame=\'addColumns\' action=\'addColumns\'/><menuitem name=\'insertColumns\' action=\'ins" +
			"ertColumns\'/><menuitem name=\'removeColumnAction\' action=\'removeColumnAction\'/><m" +
			"enuitem name=\'clearColumnAction\' action=\'clearColumnAction\'/><menuitem name=\'cop" +
			"yColumnAction\' action=\'copyColumnAction\'/><menuitem name=\'fillColumn\' action=\'fi" +
			"llColumn\'/></menu></menu><menu name=\'ViewAction\' action=\'ViewAction\'><menuitem n" +
			"ame=\'viewToolbarAction\' action=\'viewToolbarAction\'/></menu><menu name=\'HelpActio" +
			"n\' action=\'HelpAction\'><menuitem name=\'AboutAction\' action=\'AboutAction\'/></menu" +
			"></menubar></ui>");
			this.menubar2 = ((global::Gtk.MenuBar)(this.UIManager.GetWidget ("/menubar2")));
			this.menubar2.Name = "menubar2";
			this.vbox1.Add (this.menubar2);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.menubar2]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString (@"<ui><toolbar name='tbToolBar'><toolitem name='btNew' action='btNew'/><toolitem name='btOpen' action='btOpen'/><separator/><toolitem name='btSave' action='btSave'/><toolitem name='btProperties' action='btProperties'/><toolitem name='btFind' action='btFind'/><separator/><toolitem name='btAdd' action='btAdd'/><toolitem name='btInsert' action='btInsert'/><toolitem name='btRemove' action='btRemove'/><toolitem name='btClearRow' action='btClearRow'/><toolitem name='btCopyRow' action='btCopyRow'/><toolitem name='btFillRow' action='btFillRow'/><separator/><toolitem name='btAddColumns' action='btAddColumns'/><toolitem name='btInsertColumns' action='btInsertColumns'/><toolitem name='btRemoveColumn' action='btRemoveColumn'/><toolitem name='btClearColumn' action='btClearColumn'/><toolitem name='btCopyColumn' action='btCopyColumn'/><toolitem name='btFillColumn' action='btFillColumn'/></toolbar></ui>");
			this.tbToolBar = ((global::Gtk.Toolbar)(this.UIManager.GetWidget ("/tbToolBar")));
			this.tbToolBar.Name = "tbToolBar";
			this.tbToolBar.ShowArrow = false;
			this.tbToolBar.ToolbarStyle = ((global::Gtk.ToolbarStyle)(0));
			this.tbToolBar.IconSize = ((global::Gtk.IconSize)(3));
			this.vbox1.Add (this.tbToolBar);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.tbToolBar]));
			w3.Position = 1;
			w3.Expand = false;
			w3.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.tvTable = new global::Gtk.TreeView ();
			this.tvTable.CanFocus = true;
			this.tvTable.Name = "tvTable";
			this.tvTable.EnableSearch = false;
			this.GtkScrolledWindow.Add (this.tvTable);
			this.vbox1.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.GtkScrolledWindow]));
			w5.Position = 2;
			// Container child vbox1.Gtk.Box+BoxChild
			this.sbStatus = new global::Gtk.Statusbar ();
			this.sbStatus.Name = "sbStatus";
			this.sbStatus.Spacing = 6;
			// Container child sbStatus.Gtk.Box+BoxChild
			this.lblType = new global::Gtk.Label ();
			this.lblType.Name = "lblType";
			this.sbStatus.Add (this.lblType);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.sbStatus [this.lblType]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			this.vbox1.Add (this.sbStatus);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.sbStatus]));
			w7.Position = 3;
			w7.Expand = false;
			w7.Fill = false;
			this.Add (this.vbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 746;
			this.DefaultHeight = 367;
			this.Show ();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
			this.OpenAction.Activated += new global::System.EventHandler (this.OnOpen);
			this.ExitAction.Activated += new global::System.EventHandler (this.OnExit);
			this.FindAction.Activated += new global::System.EventHandler (this.OnFind);
			this.AboutAction.Activated += new global::System.EventHandler (this.OnAbout);
			this.ImportAction.Activated += new global::System.EventHandler (this.OnImport);
			this.SaveAction.Activated += new global::System.EventHandler (this.OnSave);
			this.SaveAsAction.Activated += new global::System.EventHandler (this.OnSaveAs);
			this.convertAction.Activated += new global::System.EventHandler (this.OnExport);
			this.closeAction.Activated += new global::System.EventHandler (this.OnClose);
			this.propertiesAction.Activated += new global::System.EventHandler (this.OnProperties);
			this.addRows.Activated += new global::System.EventHandler (this.OnAddRows);
			this.removeAction.Activated += new global::System.EventHandler (this.OnRemoveRows);
			this.addColumns.Activated += new global::System.EventHandler (this.OnAddColumns);
			this.removeColumnAction.Activated += new global::System.EventHandler (this.OnRemoveColumns);
			this.newAction.Activated += new global::System.EventHandler (this.OnNew);
			this.clearAction.Activated += new global::System.EventHandler (this.OnClearRow);
			this.gotoBottomAction.Activated += new global::System.EventHandler (this.OnCopyRow);
			this.clearColumnAction.Activated += new global::System.EventHandler (this.OnClearColumn);
			this.copyColumnAction.Activated += new global::System.EventHandler (this.OnCopyColumn);
			this.FindAgainAction.Activated += new global::System.EventHandler (this.OnFindAgain);
			this.revertToSavedAction.Activated += new global::System.EventHandler (this.OnRevert);
			this.btNew.Activated += new global::System.EventHandler (this.OnNew);
			this.btOpen.Activated += new global::System.EventHandler (this.OnOpen);
			this.btSave.Activated += new global::System.EventHandler (this.OnSave);
			this.btProperties.Activated += new global::System.EventHandler (this.OnProperties);
			this.btFind.Activated += new global::System.EventHandler (this.OnFind);
			this.btAdd.Activated += new global::System.EventHandler (this.OnAddRows);
			this.btRemove.Activated += new global::System.EventHandler (this.OnRemoveRows);
			this.viewToolbarAction.Activated += new global::System.EventHandler (this.OnViewToolbarActivated);
			this.btClearRow.Activated += new global::System.EventHandler (this.OnClearRow);
			this.btCopyRow.Activated += new global::System.EventHandler (this.OnCopyRow);
			this.btAddColumns.Activated += new global::System.EventHandler (this.OnAddColumns);
			this.btRemoveColumn.Activated += new global::System.EventHandler (this.OnRemoveColumns);
			this.btClearColumn.Activated += new global::System.EventHandler (this.OnClearColumn);
			this.btCopyColumn.Activated += new global::System.EventHandler (this.OnCopyColumn);
			this.btInsert.Activated += new global::System.EventHandler (this.OnInsertRows);
			this.insertRows.Activated += new global::System.EventHandler (this.OnInsertRows);
			this.insertColumns.Activated += new global::System.EventHandler (this.OnInsertColumns);
			this.btInsertColumns.Activated += new global::System.EventHandler (this.OnInsertColumns);
			this.fillRow.Activated += new global::System.EventHandler (this.OnFillRow);
			this.fillColumn.Activated += new global::System.EventHandler (this.OnFillColumn);
			this.btFillRow.Activated += new global::System.EventHandler (this.OnFillRow);
			this.btFillColumn.Activated += new global::System.EventHandler (this.OnFillColumn);
			this.insertFormulaAction.Activated += new global::System.EventHandler (this.OnInsertFormula);
		}
	}
}
