namespace nU3.Tools.Deployer.Views
{
    partial class MenuTreeManagementControl
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.SplitContainer split;
        private System.Windows.Forms.GroupBox grpTree;
        private System.Windows.Forms.GroupBox grpProg;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.TreeView tvMenu;
        private System.Windows.Forms.ListBox lbPrograms;
        private System.Windows.Forms.Button btnAddRoot;
        private System.Windows.Forms.Button btnAddChild;
        private System.Windows.Forms.Button btnDeleteNode;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblAuthLevel;
        private System.Windows.Forms.NumericUpDown numAuthLevel;
        private System.Windows.Forms.ContextMenuStrip cmsTree;

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
            split = new System.Windows.Forms.SplitContainer();
            grpTree = new System.Windows.Forms.GroupBox();
            tvMenu = new System.Windows.Forms.TreeView();
            cmsTree = new System.Windows.Forms.ContextMenuStrip(components);
            grpProg = new System.Windows.Forms.GroupBox();
            lbPrograms = new System.Windows.Forms.ListBox();
            pnlBottom = new System.Windows.Forms.Panel();
            btnAddRoot = new System.Windows.Forms.Button();
            btnAddChild = new System.Windows.Forms.Button();
            btnDeleteNode = new System.Windows.Forms.Button();
            lblAuthLevel = new System.Windows.Forms.Label();
            numAuthLevel = new System.Windows.Forms.NumericUpDown();
            btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)split).BeginInit();
            split.Panel1.SuspendLayout();
            split.Panel2.SuspendLayout();
            split.SuspendLayout();
            grpTree.SuspendLayout();
            grpProg.SuspendLayout();
            pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numAuthLevel).BeginInit();
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
            // 
            // split.Panel2
            // 
            split.Panel2.Controls.Add(grpProg);
            split.Size = new System.Drawing.Size(1012, 554);
            split.SplitterDistance = 700;
            split.TabIndex = 1;
            // 
            // grpTree
            // 
            grpTree.Controls.Add(tvMenu);
            grpTree.Dock = System.Windows.Forms.DockStyle.Fill;
            grpTree.Location = new System.Drawing.Point(0, 0);
            grpTree.Name = "grpTree";
            grpTree.Size = new System.Drawing.Size(700, 554);
            grpTree.TabIndex = 0;
            grpTree.TabStop = false;
            grpTree.Text = "메뉴 구성";
            // 
            // tvMenu
            // 
            tvMenu.AllowDrop = true;
            tvMenu.ContextMenuStrip = cmsTree;
            tvMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            tvMenu.HideSelection = false;
            tvMenu.Location = new System.Drawing.Point(3, 19);
            tvMenu.Name = "tvMenu";
            tvMenu.Size = new System.Drawing.Size(694, 532);
            tvMenu.TabIndex = 0;
            tvMenu.AfterSelect += TvMenu_AfterSelect;
            // 
            // cmsTree
            // 
            cmsTree.Name = "cmsTree";
            cmsTree.Size = new System.Drawing.Size(61, 4);
            // 
            // grpProg
            // 
            grpProg.Controls.Add(lbPrograms);
            grpProg.Dock = System.Windows.Forms.DockStyle.Fill;
            grpProg.Location = new System.Drawing.Point(0, 0);
            grpProg.Name = "grpProg";
            grpProg.Size = new System.Drawing.Size(308, 554);
            grpProg.TabIndex = 0;
            grpProg.TabStop = false;
            grpProg.Text = "등록된 프로그램 목록";
            // 
            // lbPrograms
            // 
            lbPrograms.Dock = System.Windows.Forms.DockStyle.Fill;
            lbPrograms.FormattingEnabled = true;
            lbPrograms.ItemHeight = 15;
            lbPrograms.Location = new System.Drawing.Point(3, 19);
            lbPrograms.Name = "lbPrograms";
            lbPrograms.Size = new System.Drawing.Size(302, 532);
            lbPrograms.TabIndex = 0;
            lbPrograms.DoubleClick += LbPrograms_DoubleClick;
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
            pnlBottom.Location = new System.Drawing.Point(0, 554);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new System.Drawing.Size(1012, 56);
            pnlBottom.TabIndex = 2;
            // 
            // btnAddRoot
            // 
            btnAddRoot.Location = new System.Drawing.Point(10, 9);
            btnAddRoot.Name = "btnAddRoot";
            btnAddRoot.Size = new System.Drawing.Size(100, 30);
            btnAddRoot.TabIndex = 0;
            btnAddRoot.Text = "루트 추가";
            btnAddRoot.UseVisualStyleBackColor = true;
            btnAddRoot.Click += BtnAddRoot_Click;
            // 
            // btnAddChild
            // 
            btnAddChild.Enabled = false;
            btnAddChild.Location = new System.Drawing.Point(115, 9);
            btnAddChild.Name = "btnAddChild";
            btnAddChild.Size = new System.Drawing.Size(100, 30);
            btnAddChild.TabIndex = 1;
            btnAddChild.Text = "자식 추가";
            btnAddChild.UseVisualStyleBackColor = true;
            btnAddChild.Click += BtnAddChild_Click;
            // 
            // btnDeleteNode
            // 
            btnDeleteNode.Enabled = false;
            btnDeleteNode.Location = new System.Drawing.Point(220, 9);
            btnDeleteNode.Name = "btnDeleteNode";
            btnDeleteNode.Size = new System.Drawing.Size(100, 30);
            btnDeleteNode.TabIndex = 2;
            btnDeleteNode.Text = "삭제";
            btnDeleteNode.UseVisualStyleBackColor = true;
            btnDeleteNode.Click += BtnDeleteNode_Click;
            // 
            // lblAuthLevel
            // 
            lblAuthLevel.AutoSize = true;
            lblAuthLevel.Location = new System.Drawing.Point(400, 18);
            lblAuthLevel.Name = "lblAuthLevel";
            lblAuthLevel.Size = new System.Drawing.Size(91, 15);
            lblAuthLevel.TabIndex = 3;
            lblAuthLevel.Text = "권한 레벨:";
            // 
            // numAuthLevel
            // 
            numAuthLevel.Enabled = false;
            numAuthLevel.Location = new System.Drawing.Point(495, 14);
            numAuthLevel.Maximum = new decimal(new int[] { 9, 0, 0, 0 });
            numAuthLevel.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numAuthLevel.Name = "numAuthLevel";
            numAuthLevel.Size = new System.Drawing.Size(80, 23);
            numAuthLevel.TabIndex = 4;
            numAuthLevel.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numAuthLevel.ValueChanged += NumAuthLevel_ValueChanged;
            // 
            // btnSave
            // 
            btnSave.BackColor = System.Drawing.Color.LightGreen;
            btnSave.Location = new System.Drawing.Point(850, 7);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(150, 40);
            btnSave.TabIndex = 5;
            btnSave.Text = "메뉴 구성 저장";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += BtnSave_Click;
            // 
            // MenuEditorForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1012, 610);
            Controls.Add(split);
            Controls.Add(pnlBottom);
            Name = "MenuEditorForm";
            Text = "메뉴 구성 편집기";
            split.Panel1.ResumeLayout(false);
            split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)split).EndInit();
            split.ResumeLayout(false);
            grpTree.ResumeLayout(false);
            grpProg.ResumeLayout(false);
            pnlBottom.ResumeLayout(false);
            pnlBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numAuthLevel).EndInit();
            ResumeLayout(false);
        }
    }
}
