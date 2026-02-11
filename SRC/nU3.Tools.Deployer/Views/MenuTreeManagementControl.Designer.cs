using System;

namespace nU3.Tools.Deployer.Views
{
    partial class MenuTreeManagementControl
    {
        private System.ComponentModel.IContainer components = null;
        private DevExpress.XtraEditors.SplitContainerControl splitMain;
        private DevExpress.XtraEditors.SplitContainerControl splitLeft;
        private DevExpress.XtraEditors.SplitContainerControl splitRight;
        private DevExpress.XtraEditors.GroupControl grpUsers;
        private DevExpress.XtraEditors.GroupControl grpDepts;
        private DevExpress.XtraEditors.GroupControl grpTree;
        private DevExpress.XtraEditors.GroupControl grpProg;
        private DevExpress.XtraEditors.PanelControl pnlBottom;
        private System.Windows.Forms.TreeView tvMenu;
        private DevExpress.XtraEditors.ListBoxControl lbUsers;
        private DevExpress.XtraEditors.ListBoxControl lbDepts;
        private DevExpress.XtraEditors.ListBoxControl lbPrograms;
        private nU3.Core.UI.Controls.nU3SimpleButton btnAddRoot;
        private nU3.Core.UI.Controls.nU3SimpleButton btnAddChild;
        private nU3.Core.UI.Controls.nU3SimpleButton btnDeleteNode;
        private nU3.Core.UI.Controls.nU3SimpleButton btnRefreshAll;
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
            splitMain = new DevExpress.XtraEditors.SplitContainerControl();
            splitLeft = new DevExpress.XtraEditors.SplitContainerControl();
            grpUsers = new DevExpress.XtraEditors.GroupControl();
            lbUsers = new DevExpress.XtraEditors.ListBoxControl();
            grpDepts = new DevExpress.XtraEditors.GroupControl();
            lbDepts = new DevExpress.XtraEditors.ListBoxControl();
            splitRight = new DevExpress.XtraEditors.SplitContainerControl();
            grpTree = new DevExpress.XtraEditors.GroupControl();
            tvMenu = new System.Windows.Forms.TreeView();
            cmsTree = new System.Windows.Forms.ContextMenuStrip(components);
            grpProg = new DevExpress.XtraEditors.GroupControl();
            lbPrograms = new DevExpress.XtraEditors.ListBoxControl();
            pnlBottom = new DevExpress.XtraEditors.PanelControl();
            btnAddRoot = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnAddChild = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnDeleteNode = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnRefreshAll = new nU3.Core.UI.Controls.nU3SimpleButton();
            lblAuthLevel = new nU3.Core.UI.Controls.nU3LabelControl();
            numAuthLevel = new nU3.Core.UI.Controls.nU3SpinEdit();
            btnSave = new nU3.Core.UI.Controls.nU3SimpleButton();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitMain.Panel1).BeginInit();
            splitMain.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain.Panel2).BeginInit();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitLeft).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitLeft.Panel1).BeginInit();
            splitLeft.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitLeft.Panel2).BeginInit();
            splitLeft.Panel2.SuspendLayout();
            splitLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grpUsers).BeginInit();
            grpUsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)lbUsers).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grpDepts).BeginInit();
            grpDepts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)lbDepts).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitRight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitRight.Panel1).BeginInit();
            splitRight.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitRight.Panel2).BeginInit();
            splitRight.Panel2.SuspendLayout();
            splitRight.SuspendLayout();
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
            // splitMain
            // 
            splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            splitMain.Location = new System.Drawing.Point(0, 0);
            splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            splitMain.Panel1.Controls.Add(splitLeft);
            splitMain.Panel1.Text = "Panel1";
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(splitRight);
            splitMain.Panel2.Text = "Panel2";
            splitMain.Size = new System.Drawing.Size(1305, 709);
            splitMain.SplitterPosition = 500;
            splitMain.TabIndex = 1;
            // 
            // splitLeft
            // 
            splitLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            splitLeft.Horizontal = false;
            splitLeft.Location = new System.Drawing.Point(0, 0);
            splitLeft.Name = "splitLeft";
            // 
            // splitLeft.Panel1
            // 
            splitLeft.Panel1.Controls.Add(grpUsers);
            splitLeft.Panel1.Text = "Panel1";
            // 
            // splitLeft.Panel2
            // 
            splitLeft.Panel2.Controls.Add(grpDepts);
            splitLeft.Panel2.Text = "Panel2";
            splitLeft.Size = new System.Drawing.Size(500, 709);
            splitLeft.SplitterPosition = 300;
            splitLeft.TabIndex = 0;
            // 
            // grpUsers
            // 
            grpUsers.Controls.Add(lbUsers);
            grpUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            grpUsers.Location = new System.Drawing.Point(0, 0);
            grpUsers.Name = "grpUsers";
            grpUsers.Size = new System.Drawing.Size(500, 300);
            grpUsers.TabIndex = 0;
            grpUsers.Text = "사용자 리스트";
            // 
            // lbUsers
            // 
            lbUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            lbUsers.Location = new System.Drawing.Point(2, 23);
            lbUsers.Name = "lbUsers";
            lbUsers.Size = new System.Drawing.Size(496, 275);
            lbUsers.TabIndex = 0;
            lbUsers.SelectedIndexChanged += LbUsers_SelectedIndexChanged;
            // 
            // grpDepts
            // 
            grpDepts.Controls.Add(lbDepts);
            grpDepts.Dock = System.Windows.Forms.DockStyle.Fill;
            grpDepts.Location = new System.Drawing.Point(0, 0);
            grpDepts.Name = "grpDepts";
            grpDepts.Size = new System.Drawing.Size(500, 399);
            grpDepts.TabIndex = 0;
            grpDepts.Text = "부서 리스트 (선택된 사용자)";
            // 
            // lbDepts
            // 
            lbDepts.Dock = System.Windows.Forms.DockStyle.Fill;
            lbDepts.Location = new System.Drawing.Point(2, 23);
            lbDepts.Name = "lbDepts";
            lbDepts.Size = new System.Drawing.Size(496, 374);
            lbDepts.TabIndex = 0;
            lbDepts.SelectedIndexChanged += LbDepts_SelectedIndexChanged;
            // 
            // splitRight
            // 
            splitRight.Dock = System.Windows.Forms.DockStyle.Fill;
            splitRight.Location = new System.Drawing.Point(0, 0);
            splitRight.Name = "splitRight";
            // 
            // splitRight.Panel1
            // 
            splitRight.Panel1.Controls.Add(grpTree);
            splitRight.Panel1.Text = "Panel1";
            // 
            // splitRight.Panel2
            // 
            splitRight.Panel2.Controls.Add(grpProg);
            splitRight.Panel2.Text = "Panel2";
            splitRight.Size = new System.Drawing.Size(795, 709);
            splitRight.SplitterPosition = 450;
            splitRight.TabIndex = 0;
            // 
            // grpTree
            // 
            grpTree.Controls.Add(tvMenu);
            grpTree.Dock = System.Windows.Forms.DockStyle.Fill;
            grpTree.Location = new System.Drawing.Point(0, 0);
            grpTree.Name = "grpTree";
            grpTree.Size = new System.Drawing.Size(450, 709);
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
            tvMenu.Size = new System.Drawing.Size(446, 684);
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
            grpProg.Size = new System.Drawing.Size(335, 709);
            grpProg.TabIndex = 0;
            grpProg.Text = "등록 프로그램 목록";
            // 
            // lbPrograms
            // 
            lbPrograms.Dock = System.Windows.Forms.DockStyle.Fill;
            lbPrograms.Location = new System.Drawing.Point(2, 23);
            lbPrograms.Name = "lbPrograms";
            lbPrograms.Size = new System.Drawing.Size(331, 684);
            lbPrograms.TabIndex = 0;
            // 
            // pnlBottom
            // 
            pnlBottom.Controls.Add(btnAddRoot);
            pnlBottom.Controls.Add(btnAddChild);
            pnlBottom.Controls.Add(btnDeleteNode);
            pnlBottom.Controls.Add(btnRefreshAll);
            pnlBottom.Controls.Add(lblAuthLevel);
            pnlBottom.Controls.Add(numAuthLevel);
            pnlBottom.Controls.Add(btnSave);
            pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            pnlBottom.Location = new System.Drawing.Point(0, 709);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new System.Drawing.Size(1305, 56);
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
            // btnRefreshAll
            // 
            btnRefreshAll.AuthId = "";
            btnRefreshAll.Location = new System.Drawing.Point(700, 13);
            btnRefreshAll.Name = "btnRefreshAll";
            btnRefreshAll.Size = new System.Drawing.Size(120, 30);
            btnRefreshAll.TabIndex = 6;
            btnRefreshAll.Text = "새로고침";
            btnRefreshAll.Click += BtnRefreshAll_Click;
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
            btnSave.Enabled = false;
            btnSave.Location = new System.Drawing.Point(850, 8);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(150, 40);
            btnSave.TabIndex = 5;
            btnSave.Text = "메뉴 저장";
            btnSave.Click += BtnSave_Click;
            // 
            // MenuTreeManagementControl
            // 
            Controls.Add(splitMain);
            Controls.Add(pnlBottom);
            Name = "MenuTreeManagementControl";
            Size = new System.Drawing.Size(1305, 765);
            ((System.ComponentModel.ISupportInitialize)splitMain.Panel1).EndInit();
            splitMain.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain.Panel2).EndInit();
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitLeft.Panel1).EndInit();
            splitLeft.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitLeft.Panel2).EndInit();
            splitLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitLeft).EndInit();
            splitLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grpUsers).EndInit();
            grpUsers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)lbUsers).EndInit();
            ((System.ComponentModel.ISupportInitialize)grpDepts).EndInit();
            grpDepts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)lbDepts).EndInit();
            ((System.ComponentModel.ISupportInitialize)splitRight.Panel1).EndInit();
            splitRight.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitRight.Panel2).EndInit();
            splitRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitRight).EndInit();
            splitRight.ResumeLayout(false);
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
