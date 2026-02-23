
namespace nU3.Tools.Deployer.Views
{
    partial class DomainCommonDeployControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.gcComponents = new DevExpress.XtraGrid.GridControl();
            this.gvComponents = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnDeploy = new DevExpress.XtraEditors.SimpleButton();
            this.btnSelectFile = new DevExpress.XtraEditors.SimpleButton();
            this.txtSelectedFile = new DevExpress.XtraEditors.TextEdit();
            this.cboDomain = new DevExpress.XtraEditors.ComboBoxEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcComponents)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvComponents)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSelectedFile.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDomain.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnRefresh);
            this.layoutControl1.Controls.Add(this.gcComponents);
            this.layoutControl1.Controls.Add(this.btnDeploy);
            this.layoutControl1.Controls.Add(this.btnSelectFile);
            this.layoutControl1.Controls.Add(this.txtSelectedFile);
            this.layoutControl1.Controls.Add(this.cboDomain);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1039, 694);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(951, 660);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(76, 22);
            this.btnRefresh.StyleController = this.layoutControl1;
            this.btnRefresh.TabIndex = 9;
            this.btnRefresh.Text = "새로고침";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // gcComponents
            // 
            this.gcComponents.Location = new System.Drawing.Point(12, 64);
            this.gcComponents.MainView = this.gvComponents;
            this.gcComponents.Name = "gcComponents";
            this.gcComponents.Size = new System.Drawing.Size(1015, 592);
            this.gcComponents.TabIndex = 8;
            this.gcComponents.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvComponents});
            // 
            // gvComponents
            // 
            this.gvComponents.GridControl = this.gcComponents;
            this.gvComponents.Name = "gvComponents";
            this.gvComponents.OptionsBehavior.Editable = false;
            this.gvComponents.OptionsView.ShowGroupPanel = false;
            // 
            // btnDeploy
            // 
            this.btnDeploy.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.btnDeploy.Appearance.Options.UseFont = true;
            this.btnDeploy.Location = new System.Drawing.Point(920, 38);
            this.btnDeploy.Name = "btnDeploy";
            this.btnDeploy.Size = new System.Drawing.Size(107, 22);
            this.btnDeploy.StyleController = this.layoutControl1;
            this.btnDeploy.TabIndex = 7;
            this.btnDeploy.Text = "배포하기";
            this.btnDeploy.Click += new System.EventHandler(this.btnDeploy_Click);
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(828, 12);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(199, 22);
            this.btnSelectFile.StyleController = this.layoutControl1;
            this.btnSelectFile.TabIndex = 6;
            this.btnSelectFile.Text = "파일 선택...";
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // txtSelectedFile
            // 
            this.txtSelectedFile.Location = new System.Drawing.Point(344, 12);
            this.txtSelectedFile.Name = "txtSelectedFile";
            this.txtSelectedFile.Properties.ReadOnly = true;
            this.txtSelectedFile.Size = new System.Drawing.Size(480, 20);
            this.txtSelectedFile.StyleController = this.layoutControl1;
            this.txtSelectedFile.TabIndex = 5;
            // 
            // cboDomain
            // 
            this.cboDomain.Location = new System.Drawing.Point(92, 12);
            this.cboDomain.Name = "cboDomain";
            this.cboDomain.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDomain.Properties.Items.AddRange(new object[] {
            "EMR",
            "NUR",
            "OCS",
            "ADM",
            "BIL",
            "COM"});
            this.cboDomain.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboDomain.Size = new System.Drawing.Size(171, 20);
            this.cboDomain.StyleController = this.layoutControl1;
            this.cboDomain.TabIndex = 4;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.emptySpaceItem1,
            this.layoutControlItem5,
            this.layoutControlItem6});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1039, 694);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.cboDomain;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(255, 26);
            this.layoutControlItem1.Text = "도메인 선택:";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(68, 14);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txtSelectedFile;
            this.layoutControlItem2.Location = new System.Drawing.Point(255, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(561, 26);
            this.layoutControlItem2.Text = "선택된 파일:";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(68, 14);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnSelectFile;
            this.layoutControlItem3.Location = new System.Drawing.Point(816, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(203, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnDeploy;
            this.layoutControlItem4.Location = new System.Drawing.Point(908, 26);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(111, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 26);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(908, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.gcComponents;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 52);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(1019, 596);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnRefresh;
            this.layoutControlItem6.Location = new System.Drawing.Point(939, 648);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(80, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // DomainCommonDeployControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "DomainCommonDeployControl";
            this.Size = new System.Drawing.Size(1039, 694);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcComponents)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvComponents)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSelectedFile.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDomain.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.ComboBoxEdit cboDomain;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.SimpleButton btnSelectFile;
        private DevExpress.XtraEditors.TextEdit txtSelectedFile;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.SimpleButton btnDeploy;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.GridControl gcComponents;
        private DevExpress.XtraGrid.Views.Grid.GridView gvComponents;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
    }
}
