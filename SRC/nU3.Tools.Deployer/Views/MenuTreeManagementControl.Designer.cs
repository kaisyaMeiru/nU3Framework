namespace nU3.Tools.Deployer.Views
{
    partial class MenuTreeManagementControl
    {
        private System.ComponentModel.IContainer components = null;
        private DevExpress.XtraEditors.SplitContainerControl split;
        private DevExpress.XtraEditors.GroupControl grpTree;
        private DevExpress.XtraEditors.GroupControl grpProg;
        private DevExpress.XtraEditors.PanelControl pnlBottom;
        private System.Windows.Forms.TreeView tvMenu;
        private DevExpress.XtraEditors.ListBoxControl lbPrograms;
        private nU3.Core.UI.Controls.nU3SimpleButton btnAddRoot;
        private nU3.Core.UI.Controls.nU3SimpleButton btnAddChild;
        private nU3.Core.UI.Controls.nU3SimpleButton btnDeleteNode;
        private nU3.Core.UI.Controls.nU3SimpleButton btnSave;
        private nU3.Core.UI.Controls.nU3LabelControl lblAuthLevel;
        private nU3.Core.UI.Controls.nU3SpinEdit numAuthLevel;
        private System.Windows.Forms.ContextMenuStrip cmsTree;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            split = new DevExpress.XtraEditors.SplitContainerControl();
            grpTree = new DevExpress.XtraEditors.GroupControl();
            tvMenu = new System.Windows.Forms.TreeView();
            cmsTree = new System.Windows.Forms.ContextMenuStrip(components);
            grpProg = new DevExpress.XtraEditors.GroupControl();
            lbPrograms = new DevExpress.XtraEditors.ListBoxControl();
            pnlBottom = new DevExpress.XtraEditors.PanelControl();
            btnAddRoot = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnAddChild = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnDeleteNode = new nU3.Core.UI.Controls.nU3SimpleButton();
            lblAuthLevel = new nU3.Core.UI.Controls.nU3LabelControl();
            numAuthLevel = new nU3.Core.UI.Controls.nU3SpinEdit();
            btnSave = new nU3.Core.UI.Controls.nU3SimpleButton();
            ((System.ComponentModel.ISupportInitialize)split).BeginInit();
            ((System.ComponentModel.ISupportInitialize)split.Panel1).BeginInit();
            split.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)split.Panel2).BeginInit();
            split.Panel2.SuspendLayout();
            split.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grpTree).BeginInit();
            grpTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grpProg).BeginInit();
            grpProg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)lbPrograms).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlBottom).BeginInit();
            pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numAuthLevel.Properties).BeginInit();
            SuspendLayout();
            // 
            // split
            // 
            split.Dock = System.Windows.Forms.DockStyle.Fill;
            split.Location = new System.Drawing.Point(0, 0);
            split.Name = "split";
            // 
            // split.Panel1
            // 
            split.Panel1.Controls.Add(grpTree);
            split.Panel1.Text = "Panel1";
            // 
            // split.Panel2
            // 
            split.Panel2.Controls.Add(grpProg);
            split.Panel2.Text = "Panel2";
            split.Size = new System.Drawing.Size(1406, 607);
            split.SplitterPosition = 700;
            split.TabIndex = 1;
            // 
            // grpTree
            // 
            grpTree.Controls.Add(tvMenu);
            grpTree.Dock = System.Windows.Forms.DockStyle.Fill;
            grpTree.Location = new System.Drawing.Point(0, 0);
            grpTree.Name = "grpTree";
            grpTree.Size = new System.Drawing.Size(700, 607);
            grpTree.TabIndex = 0;
            grpTree.Text = "메뉴 트리";
            // 
            // tvMenu
            // 
            tvMenu.BorderStyle = System.Windows.Forms.BorderStyle.None;
            tvMenu.ContextMenuStrip = cmsTree;
            tvMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            tvMenu.HideSelection = false;
            tvMenu.Location = new System.Drawing.Point(2, 23);
            tvMenu.Name = "tvMenu";
            tvMenu.Size = new System.Drawing.Size(696, 582);
            tvMenu.TabIndex = 0;
            tvMenu.AfterSelect += TvMenu_AfterSelect;
            // 
            // cmsTree
            // 
            cmsTree.ImageScalingSize = new System.Drawing.Size(24, 24);
            cmsTree.Name = "cmsTree";
            cmsTree.Size = new System.Drawing.Size(61, 4);
            // 
            // grpProg
            // 
            grpProg.Controls.Add(lbPrograms);
            grpProg.Dock = System.Windows.Forms.DockStyle.Fill;
            grpProg.Location = new System.Drawing.Point(0, 0);
            grpProg.Name = "grpProg";
            grpProg.Size = new System.Drawing.Size(696, 607);
            grpProg.TabIndex = 0;
            grpProg.Text = "등록 프로그램 목록";
            // 
            // lbPrograms
            // 
            lbPrograms.Dock = System.Windows.Forms.DockStyle.Fill;
            lbPrograms.Location = new System.Drawing.Point(2, 23);
            lbPrograms.Name = "lbPrograms";
            lbPrograms.Size = new System.Drawing.Size(692, 582);
            lbPrograms.TabIndex = 0;
            // 
            // pnlBottom
            // 
            pnlBottom.Controls.Add(btnAddRoot);
            pnlBottom.Controls.Add(btnAddChild);
            pnlBottom.Controls.Add(btnDeleteNode);
            pnlBottom.Controls.Add(lblAuthLevel);
            pnlBottom.Controls.Add(numAuthLevel);
            pnlBottom.Controls.Add(btnSave);
            pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            pnlBottom.Location = new System.Drawing.Point(0, 607);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new System.Drawing.Size(1406, 56);
            pnlBottom.TabIndex = 2;
            // 
            // btnAddRoot
            // 
            btnAddRoot.AuthId = "";
            btnAddRoot.Location = new System.Drawing.Point(10, 13);
            btnAddRoot.Name = "btnAddRoot";
            btnAddRoot.Size = new System.Drawing.Size(100, 30);
            btnAddRoot.TabIndex = 0;
            btnAddRoot.Text = "루트 추가";
            btnAddRoot.Click += BtnAddRoot_Click;
            // 
            // btnAddChild
            // 
            btnAddChild.AuthId = "";
            btnAddChild.Enabled = false;
            btnAddChild.Location = new System.Drawing.Point(115, 13);
            btnAddChild.Name = "btnAddChild";
            btnAddChild.Size = new System.Drawing.Size(100, 30);
            btnAddChild.TabIndex = 1;
            btnAddChild.Text = "자식 추가";
            btnAddChild.Click += BtnAddChild_Click;
            // 
            // btnDeleteNode
            // 
            btnDeleteNode.AuthId = "";
            btnDeleteNode.Enabled = false;
            btnDeleteNode.Location = new System.Drawing.Point(220, 13);
            btnDeleteNode.Name = "btnDeleteNode";
            btnDeleteNode.Size = new System.Drawing.Size(100, 30);
            btnDeleteNode.TabIndex = 2;
            btnDeleteNode.Text = "삭제";
            btnDeleteNode.Click += BtnDeleteNode_Click;
            // 
            // lblAuthLevel
            // 
            lblAuthLevel.IsRequiredMarker = false;
            lblAuthLevel.Location = new System.Drawing.Point(400, 21);
            lblAuthLevel.Name = "lblAuthLevel";
            lblAuthLevel.Size = new System.Drawing.Size(48, 14);
            lblAuthLevel.TabIndex = 3;
            lblAuthLevel.Text = "권한 레벨:";
            // 
            // numAuthLevel
            // 
            numAuthLevel.EditValue = new decimal(new int[] { 1, 0, 0, 0 });
            numAuthLevel.Enabled = false;
            numAuthLevel.IsRequired = false;
            numAuthLevel.Location = new System.Drawing.Point(465, 18);
            numAuthLevel.Name = "numAuthLevel";
            numAuthLevel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            numAuthLevel.Properties.IsFloatValue = false;
            numAuthLevel.Properties.MaskSettings.Set("mask", "N00");
            numAuthLevel.Properties.MaxValue = new decimal(new int[] { 9, 0, 0, 0 });
            numAuthLevel.Properties.MinValue = new decimal(new int[] { 1, 0, 0, 0 });
            numAuthLevel.Size = new System.Drawing.Size(80, 20);
            numAuthLevel.TabIndex = 4;
            numAuthLevel.ValueChanged += NumAuthLevel_ValueChanged;
            // 
            // btnSave
            // 
            btnSave.Appearance.BackColor = System.Drawing.Color.LightGreen;
            btnSave.Appearance.Options.UseBackColor = true;
            btnSave.AuthId = "";
            btnSave.Location = new System.Drawing.Point(850, 8);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(150, 40);
            btnSave.TabIndex = 5;
            btnSave.Text = "메뉴 저장";
            btnSave.Click += BtnSave_Click;
            // 
            // MenuTreeManagementControl
            // 
            Controls.Add(split);
            Controls.Add(pnlBottom);
            Name = "MenuTreeManagementControl";
            Size = new System.Drawing.Size(1406, 663);
            ((System.ComponentModel.ISupportInitialize)split.Panel1).EndInit();
            split.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)split.Panel2).EndInit();
            split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)split).EndInit();
            split.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grpTree).EndInit();
            grpTree.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grpProg).EndInit();
            grpProg.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)lbPrograms).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlBottom).EndInit();
            pnlBottom.ResumeLayout(false);
            pnlBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numAuthLevel.Properties).EndInit();
            ResumeLayout(false);

        }

        #endregion
    }
}