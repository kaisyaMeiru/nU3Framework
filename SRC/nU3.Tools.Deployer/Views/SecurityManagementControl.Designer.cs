namespace nU3.Tools.Deployer.Views
{
    partial class SecurityManagementControl
    {
        private System.ComponentModel.IContainer components = null;

        // Split Container
        private DevExpress.XtraEditors.SplitContainerControl splitMain;
        private DevExpress.XtraEditors.SplitContainerControl splitRight;

        // Left Panel (Targets)
        private DevExpress.XtraTab.XtraTabControl tabTargets;
        private DevExpress.XtraTab.XtraTabPage pageUsers;
        private DevExpress.XtraTab.XtraTabPage pageRoles;
        private DevExpress.XtraTab.XtraTabPage pageDepts;

        // Users Page
        private nU3.Core.UI.Controls.nU3GridControl dgvUsers;
        private nU3.Core.UI.Controls.nU3GridView gvUsers;
        private DevExpress.XtraEditors.PanelControl pnlUsersTop;
        private nU3.Core.UI.Controls.nU3SimpleButton btnAddUser;
        private nU3.Core.UI.Controls.nU3SimpleButton btnDeleteUser;
        private nU3.Core.UI.Controls.nU3SimpleButton btnEditUser;
        private nU3.Core.UI.Controls.nU3SimpleButton btnSeedTestData; // NEW

        // Roles Page
        private nU3.Core.UI.Controls.nU3GridControl dgvRoles;
        private nU3.Core.UI.Controls.nU3GridView gvRoles;
        // No Add/Delete buttons for Roles (Enum driven)

        // Depts Page
        private nU3.Core.UI.Controls.nU3GridControl dgvDepts;
        private nU3.Core.UI.Controls.nU3GridView gvDepts;
        // No Add/Delete buttons for Depts (Enum driven)

        // Right Panel (Modules & Permissions)
        private nU3.Core.UI.Controls.nU3GridControl dgvModules;
        private nU3.Core.UI.Controls.nU3GridView gvModules;
        private DevExpress.XtraEditors.GroupControl grpPermissions;
        private nU3.Core.UI.Controls.nU3SimpleButton btnSavePermission;
        private System.Windows.Forms.FlowLayoutPanel pnlChecks;
        
        private nU3.Core.UI.Controls.nU3CheckEdit chkRead;
        private nU3.Core.UI.Controls.nU3CheckEdit chkCreate;
        private nU3.Core.UI.Controls.nU3CheckEdit chkUpdate;
        private nU3.Core.UI.Controls.nU3CheckEdit chkDelete;
        private nU3.Core.UI.Controls.nU3CheckEdit chkPrint;
        private nU3.Core.UI.Controls.nU3CheckEdit chkExport;
        private nU3.Core.UI.Controls.nU3CheckEdit chkApprove;
        private nU3.Core.UI.Controls.nU3CheckEdit chkCancel;
        private nU3.Core.UI.Controls.nU3LabelControl lblCurrentTarget;

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
            splitMain = new DevExpress.XtraEditors.SplitContainerControl();
            tabTargets = new DevExpress.XtraTab.XtraTabControl();
            pageUsers = new DevExpress.XtraTab.XtraTabPage();
            dgvUsers = new nU3.Core.UI.Controls.nU3GridControl();
            gvUsers = new nU3.Core.UI.Controls.nU3GridView();
            pnlUsersTop = new DevExpress.XtraEditors.PanelControl();
            btnAddUser = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnEditUser = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnDeleteUser = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnSeedTestData = new nU3.Core.UI.Controls.nU3SimpleButton();
            pageRoles = new DevExpress.XtraTab.XtraTabPage();
            dgvRoles = new nU3.Core.UI.Controls.nU3GridControl();
            gvRoles = new nU3.Core.UI.Controls.nU3GridView();
            pageDepts = new DevExpress.XtraTab.XtraTabPage();
            dgvDepts = new nU3.Core.UI.Controls.nU3GridControl();
            gvDepts = new nU3.Core.UI.Controls.nU3GridView();
            splitRight = new DevExpress.XtraEditors.SplitContainerControl();
            dgvModules = new nU3.Core.UI.Controls.nU3GridControl();
            gvModules = new nU3.Core.UI.Controls.nU3GridView();
            grpPermissions = new DevExpress.XtraEditors.GroupControl();
            pnlChecks = new System.Windows.Forms.FlowLayoutPanel();
            chkRead = new nU3.Core.UI.Controls.nU3CheckEdit();
            chkCreate = new nU3.Core.UI.Controls.nU3CheckEdit();
            chkUpdate = new nU3.Core.UI.Controls.nU3CheckEdit();
            chkDelete = new nU3.Core.UI.Controls.nU3CheckEdit();
            chkPrint = new nU3.Core.UI.Controls.nU3CheckEdit();
            chkExport = new nU3.Core.UI.Controls.nU3CheckEdit();
            chkApprove = new nU3.Core.UI.Controls.nU3CheckEdit();
            chkCancel = new nU3.Core.UI.Controls.nU3CheckEdit();
            btnSavePermission = new nU3.Core.UI.Controls.nU3SimpleButton();
            lblCurrentTarget = new nU3.Core.UI.Controls.nU3LabelControl();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitMain.Panel1).BeginInit();
            splitMain.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain.Panel2).BeginInit();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tabTargets).BeginInit();
            tabTargets.SuspendLayout();
            pageUsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsers).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gvUsers).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlUsersTop).BeginInit();
            pnlUsersTop.SuspendLayout();
            pageRoles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRoles).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gvRoles).BeginInit();
            pageDepts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDepts).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gvDepts).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitRight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitRight.Panel1).BeginInit();
            splitRight.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitRight.Panel2).BeginInit();
            splitRight.Panel2.SuspendLayout();
            splitRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvModules).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gvModules).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grpPermissions).BeginInit();
            grpPermissions.SuspendLayout();
            pnlChecks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chkRead.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chkCreate.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chkUpdate.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chkDelete.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chkPrint.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chkExport.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chkApprove.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chkCancel.Properties).BeginInit();
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
            splitMain.Panel1.Controls.Add(tabTargets);
            splitMain.Panel1.Text = "Panel1";
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(splitRight);
            splitMain.Panel2.Text = "Panel2";
            splitMain.Size = new System.Drawing.Size(1305, 852);
            splitMain.SplitterPosition = 400;
            splitMain.TabIndex = 0;
            // 
            // tabTargets
            // 
            tabTargets.Dock = System.Windows.Forms.DockStyle.Fill;
            tabTargets.Location = new System.Drawing.Point(0, 0);
            tabTargets.Name = "tabTargets";
            tabTargets.SelectedTabPage = pageUsers;
            tabTargets.Size = new System.Drawing.Size(400, 852);
            tabTargets.TabIndex = 0;
            tabTargets.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] { pageUsers, pageRoles, pageDepts });
            // 
            // pageUsers
            // 
            pageUsers.Controls.Add(dgvUsers);
            pageUsers.Controls.Add(pnlUsersTop);
            pageUsers.Name = "pageUsers";
            pageUsers.Size = new System.Drawing.Size(398, 826);
            pageUsers.Text = "사용자";
            // 
            // dgvUsers
            // 
            dgvUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvUsers.Location = new System.Drawing.Point(0, 35);
            dgvUsers.MainView = gvUsers;
            dgvUsers.Name = "dgvUsers";
            dgvUsers.Size = new System.Drawing.Size(398, 791);
            dgvUsers.TabIndex = 1;
            dgvUsers.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvUsers });
            // 
            // gvUsers
            // 
            gvUsers.GridControl = dgvUsers;
            gvUsers.Name = "gvUsers";
            gvUsers.OptionsBehavior.Editable = false;
            gvUsers.OptionsView.ShowGroupPanel = false;
            gvUsers.FocusedRowChanged += gvUsers_FocusedRowChanged;
            // 
            // pnlUsersTop
            // 
            pnlUsersTop.Controls.Add(btnAddUser);
            pnlUsersTop.Controls.Add(btnEditUser);
            pnlUsersTop.Controls.Add(btnDeleteUser);
            pnlUsersTop.Controls.Add(btnSeedTestData);
            pnlUsersTop.Dock = System.Windows.Forms.DockStyle.Top;
            pnlUsersTop.Location = new System.Drawing.Point(0, 0);
            pnlUsersTop.Name = "pnlUsersTop";
            pnlUsersTop.Size = new System.Drawing.Size(398, 35);
            pnlUsersTop.TabIndex = 0;
            // 
            // btnAddUser
            // 
            btnAddUser.AuthId = "";
            btnAddUser.Location = new System.Drawing.Point(5, 5);
            btnAddUser.Name = "btnAddUser";
            btnAddUser.Size = new System.Drawing.Size(75, 23);
            btnAddUser.TabIndex = 0;
            btnAddUser.Text = "추가";
            btnAddUser.Click += BtnAddUser_Click;
            // 
            // btnEditUser
            // 
            btnEditUser.AuthId = "";
            btnEditUser.Location = new System.Drawing.Point(85, 5);
            btnEditUser.Name = "btnEditUser";
            btnEditUser.Size = new System.Drawing.Size(75, 23);
            btnEditUser.TabIndex = 1;
            btnEditUser.Text = "수정";
            btnEditUser.Click += BtnEditUser_Click;
            // 
            // btnDeleteUser
            // 
            btnDeleteUser.AuthId = "";
            btnDeleteUser.Location = new System.Drawing.Point(165, 5);
            btnDeleteUser.Name = "btnDeleteUser";
            btnDeleteUser.Size = new System.Drawing.Size(75, 23);
            btnDeleteUser.TabIndex = 2;
            btnDeleteUser.Text = "삭제";
            btnDeleteUser.Click += BtnDeleteUser_Click;
            // 
            // btnSeedTestData
            // 
            btnSeedTestData.AuthId = "";
            btnSeedTestData.Location = new System.Drawing.Point(245, 5);
            btnSeedTestData.Name = "btnSeedTestData";
            btnSeedTestData.Size = new System.Drawing.Size(90, 23);
            btnSeedTestData.TabIndex = 3;
            btnSeedTestData.Text = "테스트 생성";
            btnSeedTestData.Click += BtnSeedTestData_Click;
            // 
            // pageRoles
            // 
            pageRoles.Controls.Add(dgvRoles);
            pageRoles.Name = "pageRoles";
            pageRoles.Size = new System.Drawing.Size(0, 0);
            pageRoles.Text = "역할 (Role)";
            // 
            // dgvRoles
            // 
            dgvRoles.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvRoles.Location = new System.Drawing.Point(0, 0);
            dgvRoles.MainView = gvRoles;
            dgvRoles.Name = "dgvRoles";
            dgvRoles.Size = new System.Drawing.Size(0, 0);
            dgvRoles.TabIndex = 1;
            dgvRoles.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvRoles });
            // 
            // gvRoles
            // 
            gvRoles.GridControl = dgvRoles;
            gvRoles.Name = "gvRoles";
            gvRoles.OptionsBehavior.Editable = false;
            gvRoles.OptionsView.ShowGroupPanel = false;
            gvRoles.FocusedRowChanged += gvRoles_FocusedRowChanged;
            // 
            // pageDepts
            // 
            pageDepts.Controls.Add(dgvDepts);
            pageDepts.Name = "pageDepts";
            pageDepts.Size = new System.Drawing.Size(0, 0);
            pageDepts.Text = "부서";
            // 
            // dgvDepts
            // 
            dgvDepts.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvDepts.Location = new System.Drawing.Point(0, 0);
            dgvDepts.MainView = gvDepts;
            dgvDepts.Name = "dgvDepts";
            dgvDepts.Size = new System.Drawing.Size(0, 0);
            dgvDepts.TabIndex = 1;
            dgvDepts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvDepts });
            // 
            // gvDepts
            // 
            gvDepts.GridControl = dgvDepts;
            gvDepts.Name = "gvDepts";
            gvDepts.OptionsBehavior.Editable = false;
            gvDepts.OptionsView.ShowGroupPanel = false;
            gvDepts.FocusedRowChanged += gvDepts_FocusedRowChanged;
            // 
            // splitRight
            // 
            splitRight.Dock = System.Windows.Forms.DockStyle.Fill;
            splitRight.Horizontal = false;
            splitRight.Location = new System.Drawing.Point(0, 0);
            splitRight.Name = "splitRight";
            // 
            // splitRight.Panel1
            // 
            splitRight.Panel1.Controls.Add(dgvModules);
            splitRight.Panel1.Text = "Panel1";
            // 
            // splitRight.Panel2
            // 
            splitRight.Panel2.Controls.Add(grpPermissions);
            splitRight.Panel2.Text = "Panel2";
            splitRight.Size = new System.Drawing.Size(895, 852);
            splitRight.SplitterPosition = 400;
            splitRight.TabIndex = 0;
            // 
            // dgvModules
            // 
            dgvModules.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvModules.Location = new System.Drawing.Point(0, 0);
            dgvModules.MainView = gvModules;
            dgvModules.Name = "dgvModules";
            dgvModules.Size = new System.Drawing.Size(895, 400);
            dgvModules.TabIndex = 0;
            dgvModules.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvModules });
            // 
            // gvModules
            // 
            gvModules.GridControl = dgvModules;
            gvModules.Name = "gvModules";
            gvModules.OptionsBehavior.Editable = false;
            gvModules.OptionsView.ShowGroupPanel = false;
            gvModules.FocusedRowChanged += gvModules_FocusedRowChanged;
            // 
            // grpPermissions
            // 
            grpPermissions.Controls.Add(pnlChecks);
            grpPermissions.Controls.Add(btnSavePermission);
            grpPermissions.Controls.Add(lblCurrentTarget);
            grpPermissions.Dock = System.Windows.Forms.DockStyle.Fill;
            grpPermissions.Location = new System.Drawing.Point(0, 0);
            grpPermissions.Name = "grpPermissions";
            grpPermissions.Size = new System.Drawing.Size(895, 442);
            grpPermissions.TabIndex = 0;
            grpPermissions.Text = "권한 설정";
            // 
            // pnlChecks
            // 
            pnlChecks.Controls.Add(chkRead);
            pnlChecks.Controls.Add(chkCreate);
            pnlChecks.Controls.Add(chkUpdate);
            pnlChecks.Controls.Add(chkDelete);
            pnlChecks.Controls.Add(chkPrint);
            pnlChecks.Controls.Add(chkExport);
            pnlChecks.Controls.Add(chkApprove);
            pnlChecks.Controls.Add(chkCancel);
            pnlChecks.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlChecks.Location = new System.Drawing.Point(2, 47);
            pnlChecks.Name = "pnlChecks";
            pnlChecks.Padding = new System.Windows.Forms.Padding(10);
            pnlChecks.Size = new System.Drawing.Size(891, 360);
            pnlChecks.TabIndex = 0;
            // 
            // chkRead
            // 
            chkRead.Location = new System.Drawing.Point(13, 13);
            chkRead.Name = "chkRead";
            chkRead.Properties.Caption = "조회 (Read)";
            chkRead.Size = new System.Drawing.Size(120, 20);
            chkRead.TabIndex = 0;
            // 
            // chkCreate
            // 
            chkCreate.Location = new System.Drawing.Point(139, 13);
            chkCreate.Name = "chkCreate";
            chkCreate.Properties.Caption = "생성 (Create)";
            chkCreate.Size = new System.Drawing.Size(120, 20);
            chkCreate.TabIndex = 1;
            // 
            // chkUpdate
            // 
            chkUpdate.Location = new System.Drawing.Point(265, 13);
            chkUpdate.Name = "chkUpdate";
            chkUpdate.Properties.Caption = "수정 (Update)";
            chkUpdate.Size = new System.Drawing.Size(120, 20);
            chkUpdate.TabIndex = 2;
            // 
            // chkDelete
            // 
            chkDelete.Location = new System.Drawing.Point(391, 13);
            chkDelete.Name = "chkDelete";
            chkDelete.Properties.Caption = "삭제 (Delete)";
            chkDelete.Size = new System.Drawing.Size(120, 20);
            chkDelete.TabIndex = 3;
            // 
            // chkPrint
            // 
            chkPrint.Location = new System.Drawing.Point(517, 13);
            chkPrint.Name = "chkPrint";
            chkPrint.Properties.Caption = "출력 (Print)";
            chkPrint.Size = new System.Drawing.Size(120, 20);
            chkPrint.TabIndex = 4;
            // 
            // chkExport
            // 
            chkExport.Location = new System.Drawing.Point(643, 13);
            chkExport.Name = "chkExport";
            chkExport.Properties.Caption = "내보내기 (Export)";
            chkExport.Size = new System.Drawing.Size(120, 20);
            chkExport.TabIndex = 5;
            // 
            // chkApprove
            // 
            chkApprove.Location = new System.Drawing.Point(13, 39);
            chkApprove.Name = "chkApprove";
            chkApprove.Properties.Caption = "승인 (Approve)";
            chkApprove.Size = new System.Drawing.Size(120, 20);
            chkApprove.TabIndex = 6;
            // 
            // chkCancel
            // 
            chkCancel.Location = new System.Drawing.Point(139, 39);
            chkCancel.Name = "chkCancel";
            chkCancel.Properties.Caption = "취소 (Cancel)";
            chkCancel.Size = new System.Drawing.Size(120, 20);
            chkCancel.TabIndex = 7;
            // 
            // btnSavePermission
            // 
            btnSavePermission.AuthId = "";
            btnSavePermission.Dock = System.Windows.Forms.DockStyle.Bottom;
            btnSavePermission.Location = new System.Drawing.Point(2, 407);
            btnSavePermission.Name = "btnSavePermission";
            btnSavePermission.Size = new System.Drawing.Size(891, 33);
            btnSavePermission.TabIndex = 1;
            btnSavePermission.Text = "권한 저장";
            btnSavePermission.Click += BtnSavePermission_Click;
            // 
            // lblCurrentTarget
            // 
            lblCurrentTarget.Dock = System.Windows.Forms.DockStyle.Top;
            lblCurrentTarget.IsRequiredMarker = false;
            lblCurrentTarget.Location = new System.Drawing.Point(2, 23);
            lblCurrentTarget.Name = "lblCurrentTarget";
            lblCurrentTarget.Padding = new System.Windows.Forms.Padding(10, 5, 5, 5);
            lblCurrentTarget.Size = new System.Drawing.Size(93, 24);
            lblCurrentTarget.TabIndex = 2;
            lblCurrentTarget.Text = "선택된 대상 없음";
            // 
            // SecurityManagementControl
            // 
            Controls.Add(splitMain);
            Name = "SecurityManagementControl";
            Size = new System.Drawing.Size(1305, 852);
            ((System.ComponentModel.ISupportInitialize)splitMain.Panel1).EndInit();
            splitMain.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain.Panel2).EndInit();
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tabTargets).EndInit();
            tabTargets.ResumeLayout(false);
            pageUsers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvUsers).EndInit();
            ((System.ComponentModel.ISupportInitialize)gvUsers).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlUsersTop).EndInit();
            pnlUsersTop.ResumeLayout(false);
            pageRoles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvRoles).EndInit();
            ((System.ComponentModel.ISupportInitialize)gvRoles).EndInit();
            pageDepts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvDepts).EndInit();
            ((System.ComponentModel.ISupportInitialize)gvDepts).EndInit();
            ((System.ComponentModel.ISupportInitialize)splitRight.Panel1).EndInit();
            splitRight.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitRight.Panel2).EndInit();
            splitRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitRight).EndInit();
            splitRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvModules).EndInit();
            ((System.ComponentModel.ISupportInitialize)gvModules).EndInit();
            ((System.ComponentModel.ISupportInitialize)grpPermissions).EndInit();
            grpPermissions.ResumeLayout(false);
            grpPermissions.PerformLayout();
            pnlChecks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)chkRead.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)chkCreate.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)chkUpdate.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)chkDelete.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)chkPrint.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)chkExport.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)chkApprove.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)chkCancel.Properties).EndInit();
            ResumeLayout(false);
        }
    }
}
