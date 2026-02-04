namespace nU3.Tools.Deployer.Views
{
    partial class AssemblyDeployManagementControl
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
            _panelTop = new System.Windows.Forms.Panel();
            _dgvVersions = new System.Windows.Forms.DataGridView();
            lblVersions = new System.Windows.Forms.Label();
            btnRefresh = new System.Windows.Forms.Button();
            btnSmartDeploy = new System.Windows.Forms.Button();
            btnSave = new System.Windows.Forms.Button();
            btnDelete = new System.Windows.Forms.Button();
            btnBulkDeploy = new System.Windows.Forms.Button();
            btnNewComponent = new System.Windows.Forms.Button();
            btnSyncTest = new System.Windows.Forms.Button();
            btnAsyncTest = new System.Windows.Forms.Button();
            _panelDetail = new System.Windows.Forms.Panel();
            lblType = new System.Windows.Forms.Label();
            _cboComponentType = new System.Windows.Forms.ComboBox();
            lblTypeHelp = new System.Windows.Forms.Label();
            lblId = new System.Windows.Forms.Label();
            _txtComponentId = new System.Windows.Forms.TextBox();
            lblIdHelp = new System.Windows.Forms.Label();
            lblName = new System.Windows.Forms.Label();
            _txtComponentName = new System.Windows.Forms.TextBox();
            lblFile = new System.Windows.Forms.Label();
            _txtFileName = new System.Windows.Forms.TextBox();
            lblFileHelp = new System.Windows.Forms.Label();
            lblPath = new System.Windows.Forms.Label();
            _txtInstallPath = new System.Windows.Forms.TextBox();
            lblInstallPathDefaults = new System.Windows.Forms.Label();
            _lblPathPreview = new System.Windows.Forms.Label();
            lblGroup = new System.Windows.Forms.Label();
            _txtGroupName = new System.Windows.Forms.TextBox();
            lblGroupHelp = new System.Windows.Forms.Label();
            lblPriority = new System.Windows.Forms.Label();
            _nudPriority = new System.Windows.Forms.NumericUpDown();
            lblPriorityHelp = new System.Windows.Forms.Label();
            _chkRequired = new System.Windows.Forms.CheckBox();
            _chkAutoUpdate = new System.Windows.Forms.CheckBox();
            lblDeps = new System.Windows.Forms.Label();
            _txtDependencies = new System.Windows.Forms.TextBox();
            lblDepsHelp = new System.Windows.Forms.Label();
            lblDesc = new System.Windows.Forms.Label();
            _txtDescription = new System.Windows.Forms.TextBox();
            _dgvComponents = new System.Windows.Forms.DataGridView();
            panel1 = new System.Windows.Forms.Panel();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            panel2 = new System.Windows.Forms.Panel();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            _panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_dgvVersions).BeginInit();
            _panelDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_nudPriority).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_dgvComponents).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel2.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // _panelTop
            // 
            _panelTop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            _panelTop.Controls.Add(_dgvVersions);
            _panelTop.Controls.Add(lblVersions);
            _panelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            _panelTop.Location = new System.Drawing.Point(7, 7);
            _panelTop.Name = "_panelTop";
            _panelTop.Size = new System.Drawing.Size(561, 1133);
            _panelTop.TabIndex = 0;
            // 
            // _dgvVersions
            // 
            _dgvVersions.AllowUserToAddRows = false;
            _dgvVersions.AllowUserToDeleteRows = false;
            _dgvVersions.Dock = System.Windows.Forms.DockStyle.Fill;
            _dgvVersions.Location = new System.Drawing.Point(0, 25);
            _dgvVersions.MinimumSize = new System.Drawing.Size(0, 200);
            _dgvVersions.Name = "_dgvVersions";
            _dgvVersions.ReadOnly = true;
            _dgvVersions.Size = new System.Drawing.Size(561, 1108);
            _dgvVersions.TabIndex = 1;
            // 
            // lblVersions
            // 
            lblVersions.Dock = System.Windows.Forms.DockStyle.Top;
            lblVersions.Location = new System.Drawing.Point(0, 0);
            lblVersions.Name = "lblVersions";
            lblVersions.Size = new System.Drawing.Size(561, 25);
            lblVersions.TabIndex = 0;
            lblVersions.Text = "버전 이력";
            // 
            // btnRefresh
            // 
            btnRefresh.Dock = System.Windows.Forms.DockStyle.Fill;
            btnRefresh.Location = new System.Drawing.Point(7, 7);
            btnRefresh.Margin = new System.Windows.Forms.Padding(7);
            btnRefresh.Name = "btnRefresh";
            tableLayoutPanel1.SetRowSpan(btnRefresh, 3);
            btnRefresh.Size = new System.Drawing.Size(173, 86);
            btnRefresh.TabIndex = 0;
            btnRefresh.Text = "새로고침";
            btnRefresh.Click += BtnRefresh_Click;
            // 
            // btnSmartDeploy
            // 
            btnSmartDeploy.Dock = System.Windows.Forms.DockStyle.Fill;
            btnSmartDeploy.Location = new System.Drawing.Point(187, 0);
            btnSmartDeploy.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            btnSmartDeploy.Name = "btnSmartDeploy";
            btnSmartDeploy.Padding = new System.Windows.Forms.Padding(7);
            btnSmartDeploy.Size = new System.Drawing.Size(177, 33);
            btnSmartDeploy.TabIndex = 1;
            btnSmartDeploy.Text = "개별 배포";
            btnSmartDeploy.Click += BtnSmartDeploy_Click;
            // 
            // btnSave
            // 
            btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            btnSave.Location = new System.Drawing.Point(187, 33);
            btnSave.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            btnSave.Name = "btnSave";
            btnSave.Padding = new System.Windows.Forms.Padding(7);
            btnSave.Size = new System.Drawing.Size(177, 33);
            btnSave.TabIndex = 2;
            btnSave.Text = "저장";
            btnSave.Click += BtnSave_Click;
            // 
            // btnDelete
            // 
            btnDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            btnDelete.Location = new System.Drawing.Point(374, 0);
            btnDelete.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            btnDelete.Name = "btnDelete";
            btnDelete.Padding = new System.Windows.Forms.Padding(7);
            btnDelete.Size = new System.Drawing.Size(177, 33);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "삭제";
            btnDelete.Click += BtnDelete_Click;
            // 
            // btnBulkDeploy
            // 
            tableLayoutPanel1.SetColumnSpan(btnBulkDeploy, 2);
            btnBulkDeploy.Dock = System.Windows.Forms.DockStyle.Fill;
            btnBulkDeploy.Location = new System.Drawing.Point(190, 69);
            btnBulkDeploy.Name = "btnBulkDeploy";
            btnBulkDeploy.Size = new System.Drawing.Size(368, 28);
            btnBulkDeploy.TabIndex = 4;
            btnBulkDeploy.Text = "폴더 일괄 배포";
            btnBulkDeploy.Click += BtnBulkDeploy_Click;
            // 
            // btnNewComponent
            // 
            btnNewComponent.Dock = System.Windows.Forms.DockStyle.Fill;
            btnNewComponent.Location = new System.Drawing.Point(374, 33);
            btnNewComponent.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            btnNewComponent.Name = "btnNewComponent";
            btnNewComponent.Padding = new System.Windows.Forms.Padding(7);
            btnNewComponent.Size = new System.Drawing.Size(177, 33);
            btnNewComponent.TabIndex = 5;
            btnNewComponent.Text = "신규";
            btnNewComponent.Click += BtnNewComponent_Click;
            // 
            // btnSyncTest
            // 
            btnSyncTest.BackColor = System.Drawing.Color.MistyRose;
            btnSyncTest.Location = new System.Drawing.Point(10, 496);
            btnSyncTest.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            btnSyncTest.Name = "btnSyncTest";
            btnSyncTest.Size = new System.Drawing.Size(80, 23);
            btnSyncTest.TabIndex = 6;
            btnSyncTest.Text = "Sync Test";
            btnSyncTest.UseVisualStyleBackColor = false;
            btnSyncTest.Click += BtnSyncTest_Click;
            // 
            // btnAsyncTest
            // 
            btnAsyncTest.BackColor = System.Drawing.Color.LightCyan;
            btnAsyncTest.Location = new System.Drawing.Point(10, 473);
            btnAsyncTest.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            btnAsyncTest.Name = "btnAsyncTest";
            btnAsyncTest.Size = new System.Drawing.Size(80, 23);
            btnAsyncTest.TabIndex = 7;
            btnAsyncTest.Text = "Async Test";
            btnAsyncTest.UseVisualStyleBackColor = false;
            btnAsyncTest.Click += BtnAsyncTest_Click;
            // 
            // _panelDetail
            // 
            _panelDetail.AutoScroll = true;
            _panelDetail.Controls.Add(btnAsyncTest);
            _panelDetail.Controls.Add(btnSyncTest);
            _panelDetail.Controls.Add(lblType);
            _panelDetail.Controls.Add(_cboComponentType);
            _panelDetail.Controls.Add(lblTypeHelp);
            _panelDetail.Controls.Add(lblId);
            _panelDetail.Controls.Add(_txtComponentId);
            _panelDetail.Controls.Add(lblIdHelp);
            _panelDetail.Controls.Add(lblName);
            _panelDetail.Controls.Add(_txtComponentName);
            _panelDetail.Controls.Add(lblFile);
            _panelDetail.Controls.Add(_txtFileName);
            _panelDetail.Controls.Add(lblFileHelp);
            _panelDetail.Controls.Add(lblPath);
            _panelDetail.Controls.Add(_txtInstallPath);
            _panelDetail.Controls.Add(lblInstallPathDefaults);
            _panelDetail.Controls.Add(_lblPathPreview);
            _panelDetail.Controls.Add(lblGroup);
            _panelDetail.Controls.Add(_txtGroupName);
            _panelDetail.Controls.Add(lblGroupHelp);
            _panelDetail.Controls.Add(lblPriority);
            _panelDetail.Controls.Add(_nudPriority);
            _panelDetail.Controls.Add(lblPriorityHelp);
            _panelDetail.Controls.Add(_chkRequired);
            _panelDetail.Controls.Add(_chkAutoUpdate);
            _panelDetail.Controls.Add(lblDeps);
            _panelDetail.Controls.Add(_txtDependencies);
            _panelDetail.Controls.Add(lblDepsHelp);
            _panelDetail.Controls.Add(lblDesc);
            _panelDetail.Controls.Add(_txtDescription);
            _panelDetail.Dock = System.Windows.Forms.DockStyle.Bottom;
            _panelDetail.Location = new System.Drawing.Point(7, 468);
            _panelDetail.Name = "_panelDetail";
            _panelDetail.Size = new System.Drawing.Size(561, 572);
            _panelDetail.TabIndex = 0;
            // 
            // lblType
            // 
            lblType.Location = new System.Drawing.Point(10, 10);
            lblType.Name = "lblType";
            lblType.Size = new System.Drawing.Size(120, 23);
            lblType.TabIndex = 0;
            lblType.Text = "유형:";
            // 
            // _cboComponentType
            // 
            _cboComponentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _cboComponentType.Items.AddRange(new object[] { "ScreenModule", "FrameworkCore", "SharedLibrary", "Executable", "Configuration", "Resource", "Plugin", "Other" });
            _cboComponentType.Location = new System.Drawing.Point(140, 10);
            _cboComponentType.Name = "_cboComponentType";
            _cboComponentType.Size = new System.Drawing.Size(250, 23);
            _cboComponentType.TabIndex = 1;
            _cboComponentType.SelectedIndexChanged += CboComponentType_SelectedIndexChanged;
            // 
            // lblTypeHelp
            // 
            lblTypeHelp.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            lblTypeHelp.ForeColor = System.Drawing.Color.DarkGreen;
            lblTypeHelp.Location = new System.Drawing.Point(140, 32);
            lblTypeHelp.Name = "lblTypeHelp";
            lblTypeHelp.Size = new System.Drawing.Size(250, 23);
            lblTypeHelp.TabIndex = 2;
            lblTypeHelp.Text = "유형 선택 시 설치경로/우선순위 자동 설정";
            lblTypeHelp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblId
            // 
            lblId.Location = new System.Drawing.Point(10, 60);
            lblId.Name = "lblId";
            lblId.Size = new System.Drawing.Size(120, 23);
            lblId.TabIndex = 3;
            lblId.Text = "Component ID:";
            // 
            // _txtComponentId
            // 
            _txtComponentId.Location = new System.Drawing.Point(140, 60);
            _txtComponentId.Name = "_txtComponentId";
            _txtComponentId.Size = new System.Drawing.Size(250, 23);
            _txtComponentId.TabIndex = 4;
            // 
            // lblIdHelp
            // 
            lblIdHelp.Font = new System.Drawing.Font("Segoe UI", 8F);
            lblIdHelp.ForeColor = System.Drawing.Color.Gray;
            lblIdHelp.Location = new System.Drawing.Point(140, 82);
            lblIdHelp.Name = "lblIdHelp";
            lblIdHelp.Size = new System.Drawing.Size(250, 23);
            lblIdHelp.TabIndex = 5;
            lblIdHelp.Text = "예: nU3.Core, DevExpress.XtraEditors";
            lblIdHelp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblName
            // 
            lblName.Location = new System.Drawing.Point(10, 110);
            lblName.Name = "lblName";
            lblName.Size = new System.Drawing.Size(120, 23);
            lblName.TabIndex = 6;
            lblName.Text = "이름:";
            // 
            // _txtComponentName
            // 
            _txtComponentName.Location = new System.Drawing.Point(140, 110);
            _txtComponentName.Name = "_txtComponentName";
            _txtComponentName.Size = new System.Drawing.Size(250, 23);
            _txtComponentName.TabIndex = 7;
            // 
            // lblFile
            // 
            lblFile.Location = new System.Drawing.Point(10, 140);
            lblFile.Name = "lblFile";
            lblFile.Size = new System.Drawing.Size(120, 23);
            lblFile.TabIndex = 8;
            lblFile.Text = "파일명:";
            // 
            // _txtFileName
            // 
            _txtFileName.Location = new System.Drawing.Point(140, 140);
            _txtFileName.Name = "_txtFileName";
            _txtFileName.Size = new System.Drawing.Size(250, 23);
            _txtFileName.TabIndex = 9;
            _txtFileName.TextChanged += TxtFileName_TextChanged;
            // 
            // lblFileHelp
            // 
            lblFileHelp.Font = new System.Drawing.Font("Segoe UI", 8F);
            lblFileHelp.ForeColor = System.Drawing.Color.Gray;
            lblFileHelp.Location = new System.Drawing.Point(140, 162);
            lblFileHelp.Name = "lblFileHelp";
            lblFileHelp.Size = new System.Drawing.Size(250, 23);
            lblFileHelp.TabIndex = 10;
            lblFileHelp.Text = "예: nU3.Core.dll, MyApp.exe";
            lblFileHelp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPath
            // 
            lblPath.Location = new System.Drawing.Point(10, 190);
            lblPath.Name = "lblPath";
            lblPath.Size = new System.Drawing.Size(120, 23);
            lblPath.TabIndex = 11;
            lblPath.Text = "설치 경로 (상대):";
            // 
            // _txtInstallPath
            // 
            _txtInstallPath.Location = new System.Drawing.Point(140, 190);
            _txtInstallPath.Name = "_txtInstallPath";
            _txtInstallPath.Size = new System.Drawing.Size(250, 23);
            _txtInstallPath.TabIndex = 12;
            _txtInstallPath.TextChanged += TxtInstallPath_TextChanged;
            // 
            // lblInstallPathDefaults
            // 
            lblInstallPathDefaults.Font = new System.Drawing.Font("Segoe UI", 8F);
            lblInstallPathDefaults.ForeColor = System.Drawing.Color.DarkBlue;
            lblInstallPathDefaults.Location = new System.Drawing.Point(140, 212);
            lblInstallPathDefaults.Name = "lblInstallPathDefaults";
            lblInstallPathDefaults.Size = new System.Drawing.Size(350, 23);
            lblInstallPathDefaults.TabIndex = 13;
            lblInstallPathDefaults.Text = "Core/Lib/Exe→루트, Plugin→plugins, Resource→resources";
            lblInstallPathDefaults.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _lblPathPreview
            // 
            _lblPathPreview.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            _lblPathPreview.ForeColor = System.Drawing.Color.Blue;
            _lblPathPreview.Location = new System.Drawing.Point(140, 232);
            _lblPathPreview.Name = "_lblPathPreview";
            _lblPathPreview.Size = new System.Drawing.Size(350, 23);
            _lblPathPreview.TabIndex = 14;
            _lblPathPreview.Text = "최종 경로: {실행경로}\\{파일명}";
            _lblPathPreview.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblGroup
            // 
            lblGroup.Location = new System.Drawing.Point(10, 260);
            lblGroup.Name = "lblGroup";
            lblGroup.Size = new System.Drawing.Size(120, 23);
            lblGroup.TabIndex = 15;
            lblGroup.Text = "그룹:";
            // 
            // _txtGroupName
            // 
            _txtGroupName.Location = new System.Drawing.Point(140, 260);
            _txtGroupName.Name = "_txtGroupName";
            _txtGroupName.Size = new System.Drawing.Size(250, 23);
            _txtGroupName.TabIndex = 16;
            // 
            // lblGroupHelp
            // 
            lblGroupHelp.Font = new System.Drawing.Font("Segoe UI", 8F);
            lblGroupHelp.ForeColor = System.Drawing.Color.Gray;
            lblGroupHelp.Location = new System.Drawing.Point(140, 282);
            lblGroupHelp.Name = "lblGroupHelp";
            lblGroupHelp.Size = new System.Drawing.Size(250, 23);
            lblGroupHelp.TabIndex = 17;
            lblGroupHelp.Text = "예: Framework, DevExpress, Oracle";
            // 
            // lblPriority
            // 
            lblPriority.Location = new System.Drawing.Point(10, 310);
            lblPriority.Name = "lblPriority";
            lblPriority.Size = new System.Drawing.Size(120, 23);
            lblPriority.TabIndex = 18;
            lblPriority.Text = "우선순위:";
            // 
            // _nudPriority
            // 
            _nudPriority.Location = new System.Drawing.Point(140, 310);
            _nudPriority.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            _nudPriority.Name = "_nudPriority";
            _nudPriority.Size = new System.Drawing.Size(80, 23);
            _nudPriority.TabIndex = 19;
            _nudPriority.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // lblPriorityHelp
            // 
            lblPriorityHelp.Font = new System.Drawing.Font("Segoe UI", 8F);
            lblPriorityHelp.ForeColor = System.Drawing.Color.Gray;
            lblPriorityHelp.Location = new System.Drawing.Point(230, 313);
            lblPriorityHelp.Name = "lblPriorityHelp";
            lblPriorityHelp.Size = new System.Drawing.Size(250, 23);
            lblPriorityHelp.TabIndex = 20;
            lblPriorityHelp.Text = "낮을수록 먼저 설치 (유형별 자동설정)";
            // 
            // _chkRequired
            // 
            _chkRequired.Location = new System.Drawing.Point(140, 345);
            _chkRequired.Name = "_chkRequired";
            _chkRequired.Size = new System.Drawing.Size(150, 23);
            _chkRequired.TabIndex = 21;
            _chkRequired.Text = "필수 컴포넌트";
            // 
            // _chkAutoUpdate
            // 
            _chkAutoUpdate.Checked = true;
            _chkAutoUpdate.CheckState = System.Windows.Forms.CheckState.Checked;
            _chkAutoUpdate.Location = new System.Drawing.Point(140, 370);
            _chkAutoUpdate.Name = "_chkAutoUpdate";
            _chkAutoUpdate.Size = new System.Drawing.Size(150, 23);
            _chkAutoUpdate.TabIndex = 22;
            _chkAutoUpdate.Text = "자동 업데이트";
            // 
            // lblDeps
            // 
            lblDeps.Location = new System.Drawing.Point(10, 400);
            lblDeps.Name = "lblDeps";
            lblDeps.Size = new System.Drawing.Size(120, 23);
            lblDeps.TabIndex = 23;
            lblDeps.Text = "의존성:";
            // 
            // _txtDependencies
            // 
            _txtDependencies.Location = new System.Drawing.Point(140, 400);
            _txtDependencies.Name = "_txtDependencies";
            _txtDependencies.Size = new System.Drawing.Size(250, 23);
            _txtDependencies.TabIndex = 24;
            // 
            // lblDepsHelp
            // 
            lblDepsHelp.Font = new System.Drawing.Font("Segoe UI", 8F);
            lblDepsHelp.ForeColor = System.Drawing.Color.Gray;
            lblDepsHelp.Location = new System.Drawing.Point(140, 422);
            lblDepsHelp.Name = "lblDepsHelp";
            lblDepsHelp.Size = new System.Drawing.Size(250, 23);
            lblDepsHelp.TabIndex = 25;
            lblDepsHelp.Text = "쉼표 구분, 예: nU3.Core,System.Data";
            // 
            // lblDesc
            // 
            lblDesc.Location = new System.Drawing.Point(10, 450);
            lblDesc.Name = "lblDesc";
            lblDesc.Size = new System.Drawing.Size(120, 23);
            lblDesc.TabIndex = 26;
            lblDesc.Text = "설명:";
            // 
            // _txtDescription
            // 
            _txtDescription.Location = new System.Drawing.Point(140, 450);
            _txtDescription.Multiline = true;
            _txtDescription.Name = "_txtDescription";
            _txtDescription.Size = new System.Drawing.Size(250, 60);
            _txtDescription.TabIndex = 27;
            // 
            // _dgvComponents
            // 
            _dgvComponents.AllowUserToAddRows = false;
            _dgvComponents.AllowUserToDeleteRows = false;
            _dgvComponents.Dock = System.Windows.Forms.DockStyle.Fill;
            _dgvComponents.Location = new System.Drawing.Point(0, 0);
            _dgvComponents.MultiSelect = false;
            _dgvComponents.Name = "_dgvComponents";
            _dgvComponents.ReadOnly = true;
            _dgvComponents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            _dgvComponents.Size = new System.Drawing.Size(700, 1147);
            _dgvComponents.TabIndex = 0;
            _dgvComponents.SelectionChanged += DgvComponents_SelectionChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(splitContainer1);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Padding = new System.Windows.Forms.Padding(7);
            panel1.Size = new System.Drawing.Size(1293, 1161);
            panel1.TabIndex = 2;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(7, 7);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(_dgvComponents);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panel2);
            splitContainer1.Size = new System.Drawing.Size(1279, 1147);
            splitContainer1.SplitterDistance = 700;
            splitContainer1.TabIndex = 4;
            // 
            // panel2
            // 
            panel2.Controls.Add(_panelDetail);
            panel2.Controls.Add(tableLayoutPanel1);
            panel2.Controls.Add(_panelTop);
            panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            panel2.Location = new System.Drawing.Point(0, 0);
            panel2.Name = "panel2";
            panel2.Padding = new System.Windows.Forms.Padding(7);
            panel2.Size = new System.Drawing.Size(575, 1147);
            panel2.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Controls.Add(btnNewComponent, 2, 1);
            tableLayoutPanel1.Controls.Add(btnSave, 1, 1);
            tableLayoutPanel1.Controls.Add(btnRefresh, 0, 0);
            tableLayoutPanel1.Controls.Add(btnBulkDeploy, 1, 2);
            tableLayoutPanel1.Controls.Add(btnSmartDeploy, 1, 0);
            tableLayoutPanel1.Controls.Add(btnDelete, 2, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            tableLayoutPanel1.Location = new System.Drawing.Point(7, 1040);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Size = new System.Drawing.Size(561, 100);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // AssemblyDeployManagementControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panel1);
            Name = "AssemblyDeployManagementControl";
            Size = new System.Drawing.Size(1293, 1161);
            _panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_dgvVersions).EndInit();
            _panelDetail.ResumeLayout(false);
            _panelDetail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_nudPriority).EndInit();
            ((System.ComponentModel.ISupportInitialize)_dgvComponents).EndInit();
            panel1.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.DataGridView _dgvComponents;
        private System.Windows.Forms.DataGridView _dgvVersions;
        private System.Windows.Forms.Panel _panelTop;
        private System.Windows.Forms.Panel _panelDetail;

        private System.Windows.Forms.TextBox _txtComponentId;
        private System.Windows.Forms.TextBox _txtComponentName;
        private System.Windows.Forms.TextBox _txtFileName;
        private System.Windows.Forms.TextBox _txtInstallPath;
        private System.Windows.Forms.TextBox _txtGroupName;
        private System.Windows.Forms.TextBox _txtDescription;
        private System.Windows.Forms.TextBox _txtDependencies;
        private System.Windows.Forms.ComboBox _cboComponentType;
        private System.Windows.Forms.NumericUpDown _nudPriority;
        private System.Windows.Forms.CheckBox _chkRequired;
        private System.Windows.Forms.CheckBox _chkAutoUpdate;
        private System.Windows.Forms.Label _lblPathPreview;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnSmartDeploy;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnBulkDeploy;
        private System.Windows.Forms.Button btnNewComponent;
        private System.Windows.Forms.Button btnSyncTest;
        private System.Windows.Forms.Button btnAsyncTest;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Label lblTypeHelp;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.Label lblIdHelp;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.Label lblFileHelp;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Label lblInstallPathDefaults;
        private System.Windows.Forms.Label lblGroup;
        private System.Windows.Forms.Label lblGroupHelp;
        private System.Windows.Forms.Label lblPriority;
        private System.Windows.Forms.Label lblPriorityHelp;
        private System.Windows.Forms.Label lblDeps;
        private System.Windows.Forms.Label lblDepsHelp;
        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.Label lblVersions;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
