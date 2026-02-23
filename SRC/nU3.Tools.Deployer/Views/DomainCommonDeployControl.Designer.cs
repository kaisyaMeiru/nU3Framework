
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
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            gcComponents = new DevExpress.XtraGrid.GridControl();
            gvComponents = new DevExpress.XtraGrid.Views.Grid.GridView();
            btnDeploy = new DevExpress.XtraEditors.SimpleButton();
            btnSelectFile = new DevExpress.XtraEditors.SimpleButton();
            txtSelectedFile = new DevExpress.XtraEditors.TextEdit();
            cboDomain = new DevExpress.XtraEditors.ComboBoxEdit();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gcComponents).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gvComponents).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtSelectedFile.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cboDomain.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(btnRefresh);
            layoutControl1.Controls.Add(gcComponents);
            layoutControl1.Controls.Add(btnDeploy);
            layoutControl1.Controls.Add(btnSelectFile);
            layoutControl1.Controls.Add(txtSelectedFile);
            layoutControl1.Controls.Add(cboDomain);
            layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            layoutControl1.Location = new System.Drawing.Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = Root;
            layoutControl1.Size = new System.Drawing.Size(1039, 694);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new System.Drawing.Point(12, 660);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new System.Drawing.Size(1015, 22);
            btnRefresh.StyleController = layoutControl1;
            btnRefresh.TabIndex = 9;
            btnRefresh.Text = "새로고침";
            btnRefresh.Click += btnRefresh_Click;
            // 
            // gcComponents
            // 
            gcComponents.Location = new System.Drawing.Point(12, 64);
            gcComponents.MainView = gvComponents;
            gcComponents.Name = "gcComponents";
            gcComponents.Size = new System.Drawing.Size(1015, 592);
            gcComponents.TabIndex = 8;
            gcComponents.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvComponents });
            // 
            // gvComponents
            // 
            gvComponents.GridControl = gcComponents;
            gvComponents.Name = "gvComponents";
            gvComponents.OptionsBehavior.Editable = false;
            gvComponents.OptionsView.ShowGroupPanel = false;
            // 
            // btnDeploy
            // 
            btnDeploy.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            btnDeploy.Appearance.Options.UseFont = true;
            btnDeploy.Location = new System.Drawing.Point(920, 38);
            btnDeploy.Name = "btnDeploy";
            btnDeploy.Size = new System.Drawing.Size(107, 22);
            btnDeploy.StyleController = layoutControl1;
            btnDeploy.TabIndex = 7;
            btnDeploy.Text = "배포하기";
            btnDeploy.Click += btnDeploy_Click;
            // 
            // btnSelectFile
            // 
            btnSelectFile.Location = new System.Drawing.Point(828, 12);
            btnSelectFile.Name = "btnSelectFile";
            btnSelectFile.Size = new System.Drawing.Size(199, 22);
            btnSelectFile.StyleController = layoutControl1;
            btnSelectFile.TabIndex = 6;
            btnSelectFile.Text = "파일 선택...";
            btnSelectFile.Click += btnSelectFile_Click;
            // 
            // txtSelectedFile
            // 
            txtSelectedFile.Location = new System.Drawing.Point(357, 12);
            txtSelectedFile.Name = "txtSelectedFile";
            txtSelectedFile.Properties.ReadOnly = true;
            txtSelectedFile.Size = new System.Drawing.Size(467, 20);
            txtSelectedFile.StyleController = layoutControl1;
            txtSelectedFile.TabIndex = 5;
            // 
            // cboDomain
            // 
            cboDomain.Location = new System.Drawing.Point(102, 12);
            cboDomain.Name = "cboDomain";
            cboDomain.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cboDomain.Properties.Items.AddRange(new object[] { "EMR", "NUR", "OCS", "ADM", "BIL", "COM" });
            cboDomain.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cboDomain.Size = new System.Drawing.Size(161, 20);
            cboDomain.StyleController = layoutControl1;
            cboDomain.TabIndex = 4;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem1, layoutControlItem2, layoutControlItem3, layoutControlItem4, emptySpaceItem1, layoutControlItem5, layoutControlItem6 });
            Root.Name = "Root";
            Root.Size = new System.Drawing.Size(1039, 694);
            Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = cboDomain;
            layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new System.Drawing.Size(255, 26);
            layoutControlItem1.Text = "업무시스템 선택:";
            layoutControlItem1.TextSize = new System.Drawing.Size(78, 14);
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = txtSelectedFile;
            layoutControlItem2.Location = new System.Drawing.Point(255, 0);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new System.Drawing.Size(561, 26);
            layoutControlItem2.Text = "선택된 파일:";
            layoutControlItem2.TextSize = new System.Drawing.Size(78, 14);
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.Control = btnSelectFile;
            layoutControlItem3.Location = new System.Drawing.Point(816, 0);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new System.Drawing.Size(203, 26);
            layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            layoutControlItem4.Control = btnDeploy;
            layoutControlItem4.Location = new System.Drawing.Point(908, 26);
            layoutControlItem4.Name = "layoutControlItem4";
            layoutControlItem4.Size = new System.Drawing.Size(111, 26);
            layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.AllowHotTrack = false;
            emptySpaceItem1.Location = new System.Drawing.Point(0, 26);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new System.Drawing.Size(908, 26);
            emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem5
            // 
            layoutControlItem5.Control = gcComponents;
            layoutControlItem5.Location = new System.Drawing.Point(0, 52);
            layoutControlItem5.Name = "layoutControlItem5";
            layoutControlItem5.Size = new System.Drawing.Size(1019, 596);
            layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            layoutControlItem6.Control = btnRefresh;
            layoutControlItem6.Location = new System.Drawing.Point(0, 648);
            layoutControlItem6.Name = "layoutControlItem6";
            layoutControlItem6.Size = new System.Drawing.Size(1019, 26);
            layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            layoutControlItem6.TextVisible = false;
            // 
            // DomainCommonDeployControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(layoutControl1);
            Name = "DomainCommonDeployControl";
            Size = new System.Drawing.Size(1039, 694);
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gcComponents).EndInit();
            ((System.ComponentModel.ISupportInitialize)gvComponents).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtSelectedFile.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cboDomain.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).EndInit();
            ResumeLayout(false);

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
