namespace nU3.Shell
{
    partial class nUShell
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(nUShell));
            xtraTabControlMain = new DevExpress.XtraTab.XtraTabControl();
            barManager1 = new nU3.Core.UI.Controls.Bars.nU3BarManager(components);
            barMainMenu = new DevExpress.XtraBars.Bar();
            barStatusBar = new DevExpress.XtraBars.Bar();
            barStaticItemUser = new DevExpress.XtraBars.BarStaticItem();
            barStaticItemServer = new DevExpress.XtraBars.BarStaticItem();
            barStaticItemDatabase = new DevExpress.XtraBars.BarStaticItem();
            barStaticItemVersion = new DevExpress.XtraBars.BarStaticItem();
            barStaticItemLogMessage = new DevExpress.XtraBars.BarStaticItem();
            barStaticItemTime = new DevExpress.XtraBars.BarStaticItem();
            barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            timerStatusUpdate = new System.Windows.Forms.Timer(components);
            imageCollection = new DevExpress.Utils.ImageCollection(components);
            defaultLookAndFeel = new DevExpress.LookAndFeel.DefaultLookAndFeel(components);
            ((System.ComponentModel.ISupportInitialize)xtraTabControlMain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)barManager1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)imageCollection).BeginInit();
            SuspendLayout();
            // 
            // xtraTabControlMain
            // 
            xtraTabControlMain.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InActiveTabPageHeader;
            xtraTabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            xtraTabControlMain.Location = new System.Drawing.Point(0, 21);
            xtraTabControlMain.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            xtraTabControlMain.Name = "xtraTabControlMain";
            xtraTabControlMain.Size = new System.Drawing.Size(2000, 1163);
            xtraTabControlMain.TabIndex = 0;
            xtraTabControlMain.SelectedPageChanged += XtraTabControlMain_SelectedPageChanged;
            xtraTabControlMain.CloseButtonClick += XtraTabControlMain_CloseButtonClick;
            // 
            // barManager1
            // 
            barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] { barMainMenu, barStatusBar });
            barManager1.DockControls.Add(barDockControlTop);
            barManager1.DockControls.Add(barDockControlBottom);
            barManager1.DockControls.Add(barDockControlLeft);
            barManager1.DockControls.Add(barDockControlRight);
            barManager1.Form = this;
            barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { barStaticItemUser, barStaticItemTime, barStaticItemServer, barStaticItemDatabase, barStaticItemVersion, barStaticItemLogMessage });
            barManager1.MainMenu = barMainMenu;
            barManager1.MaxItemId = 10;
            barManager1.StatusBar = barStatusBar;
            // 
            // barMainMenu
            // 
            barMainMenu.BarName = "Main Menu";
            barMainMenu.DockCol = 0;
            barMainMenu.DockRow = 0;
            barMainMenu.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            barMainMenu.OptionsBar.AllowQuickCustomization = false;
            barMainMenu.OptionsBar.DrawDragBorder = false;
            barMainMenu.OptionsBar.MultiLine = true;
            barMainMenu.OptionsBar.UseWholeRow = true;
            barMainMenu.Text = "Main Menu";
            // 
            // barStatusBar
            // 
            barStatusBar.BarName = "Status Bar";
            barStatusBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            barStatusBar.DockCol = 0;
            barStatusBar.DockRow = 0;
            barStatusBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            barStatusBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(barStaticItemUser, true), new DevExpress.XtraBars.LinkPersistInfo(barStaticItemLogMessage, true), new DevExpress.XtraBars.LinkPersistInfo(barStaticItemServer, true), new DevExpress.XtraBars.LinkPersistInfo(barStaticItemDatabase, true), new DevExpress.XtraBars.LinkPersistInfo(barStaticItemVersion, true), new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, barStaticItemTime, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph) });
            barStatusBar.OptionsBar.AllowQuickCustomization = false;
            barStatusBar.OptionsBar.DrawDragBorder = false;
            barStatusBar.OptionsBar.UseWholeRow = true;
            barStatusBar.Text = "Status Bar";
            // 
            // barStaticItemUser
            // 
            barStaticItemUser.Caption = "User: Guest";
            barStaticItemUser.Id = 0;
            barStaticItemUser.ItemAppearance.Normal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            barStaticItemUser.ItemAppearance.Normal.Options.UseFont = true;
            barStaticItemUser.Name = "barStaticItemUser";
            // 
            // barStaticItemLogMessage
            // 
            barStaticItemLogMessage.AutoSize = DevExpress.XtraBars.BarStaticItemSize.Spring;
            barStaticItemLogMessage.Caption = "Ready";
            barStaticItemLogMessage.Id = 11;
            barStaticItemLogMessage.Name = "barStaticItemLogMessage";
            barStaticItemLogMessage.ItemDoubleClick += BarStaticItemLogMessage_ItemDoubleClick;
            // 
            // barStaticItemServer
            // 
            barStaticItemServer.Caption = "Server: Local";
            barStaticItemServer.Id = 2;
            barStaticItemServer.Name = "barStaticItemServer";
            // 
            // barStaticItemDatabase
            // 
            barStaticItemDatabase.Caption = "DB: SQLite";
            barStaticItemDatabase.Id = 3;
            barStaticItemDatabase.Name = "barStaticItemDatabase";
            // 
            // barStaticItemVersion
            // 
            barStaticItemVersion.Caption = "v1.0.0";
            barStaticItemVersion.Id = 4;
            barStaticItemVersion.Name = "barStaticItemVersion";
            // 
            // barStaticItemTime
            // 
            barStaticItemTime.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            barStaticItemTime.Caption = "Ready";
            barStaticItemTime.Id = 1;
            barStaticItemTime.ItemAppearance.Normal.Font = new System.Drawing.Font("Consolas", 9F);
            barStaticItemTime.ItemAppearance.Normal.Options.UseFont = true;
            barStaticItemTime.Name = "barStaticItemTime";
            // 
            // barDockControlTop
            // 
            barDockControlTop.CausesValidation = false;
            barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            barDockControlTop.Location = new System.Drawing.Point(0, 0);
            barDockControlTop.Manager = barManager1;
            barDockControlTop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            barDockControlTop.Size = new System.Drawing.Size(2000, 21);
            // 
            // barDockControlBottom
            // 
            barDockControlBottom.CausesValidation = false;
            barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            barDockControlBottom.Location = new System.Drawing.Point(0, 1184);
            barDockControlBottom.Manager = barManager1;
            barDockControlBottom.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            barDockControlBottom.Size = new System.Drawing.Size(2000, 35);
            // 
            // barDockControlLeft
            // 
            barDockControlLeft.CausesValidation = false;
            barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            barDockControlLeft.Location = new System.Drawing.Point(0, 21);
            barDockControlLeft.Manager = barManager1;
            barDockControlLeft.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            barDockControlLeft.Size = new System.Drawing.Size(0, 1163);
            // 
            // barDockControlRight
            // 
            barDockControlRight.CausesValidation = false;
            barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            barDockControlRight.Location = new System.Drawing.Point(2000, 21);
            barDockControlRight.Manager = barManager1;
            barDockControlRight.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            barDockControlRight.Size = new System.Drawing.Size(0, 1163);
            // 
            // timerStatusUpdate
            // 
            timerStatusUpdate.Enabled = true;
            timerStatusUpdate.Interval = 1000;
            timerStatusUpdate.Tick += TimerStatusUpdate_Tick;
            // 
            // imageCollection
            // 
            imageCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("imageCollection.ImageStream");
            // 
            // defaultLookAndFeel
            // 
            defaultLookAndFeel.LookAndFeel.SkinName = "Office 2019 Colorful";
            // 
            // nUShell
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            ClientSize = new System.Drawing.Size(2000, 1219);
            Controls.Add(xtraTabControlMain);
            Controls.Add(barDockControlLeft);
            Controls.Add(barDockControlRight);
            Controls.Add(barDockControlBottom);
            Controls.Add(barDockControlTop);
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            MinimumSize = new System.Drawing.Size(800, 600);
            Name = "nUShell";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "nU3 Healthcare Information System";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)xtraTabControlMain).EndInit();
            ((System.ComponentModel.ISupportInitialize)barManager1).EndInit();
            ((System.ComponentModel.ISupportInitialize)imageCollection).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private DevExpress.XtraTab.XtraTabControl xtraTabControlMain;
        private Core.UI.Controls.Bars.nU3BarManager barManager1;
        private DevExpress.XtraBars.Bar barMainMenu;
        private DevExpress.XtraBars.Bar barStatusBar;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarStaticItem barStaticItemUser;
        private DevExpress.XtraBars.BarStaticItem barStaticItemServer;
        private DevExpress.XtraBars.BarStaticItem barStaticItemDatabase;
        private DevExpress.XtraBars.BarStaticItem barStaticItemVersion;
        private DevExpress.XtraBars.BarStaticItem barStaticItemLogMessage;
        private DevExpress.XtraBars.BarStaticItem barStaticItemTime;
        private System.Windows.Forms.Timer timerStatusUpdate;
        private DevExpress.Utils.ImageCollection imageCollection;
        private DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel;
    }
}
