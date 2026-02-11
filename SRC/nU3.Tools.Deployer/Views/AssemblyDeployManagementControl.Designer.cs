namespace nU3.Tools.Deployer.Views
{
    partial class AssemblyDeployManagementControl
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

        #region Component Designer generated code

        private void InitializeComponent()
        {
            _panelTop = new DevExpress.XtraEditors.PanelControl();
            _dgvVersions = new nU3.Core.UI.Controls.nU3GridControl();
            _gvVersions = new nU3.Core.UI.Controls.nU3GridView();
            lblVersions = new nU3.Core.UI.Controls.nU3LabelControl();
            _panelDetail = new DevExpress.XtraEditors.PanelControl();
            btnAsyncTest = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnSyncTest = new nU3.Core.UI.Controls.nU3SimpleButton();
            lblType = new nU3.Core.UI.Controls.nU3LabelControl();
            _cboComponentType = new nU3.Core.UI.Controls.nU3ComboBoxEdit();
            lblTypeHelp = new nU3.Core.UI.Controls.nU3LabelControl();
            lblId = new nU3.Core.UI.Controls.nU3LabelControl();
            _txtComponentId = new nU3.Core.UI.Controls.nU3TextEdit();
            lblIdHelp = new nU3.Core.UI.Controls.nU3LabelControl();
            lblName = new nU3.Core.UI.Controls.nU3LabelControl();
            _txtComponentName = new nU3.Core.UI.Controls.nU3TextEdit();
            lblFile = new nU3.Core.UI.Controls.nU3LabelControl();
            _txtFileName = new nU3.Core.UI.Controls.nU3TextEdit();
            lblFileHelp = new nU3.Core.UI.Controls.nU3LabelControl();
            lblPath = new nU3.Core.UI.Controls.nU3LabelControl();
            _txtInstallPath = new nU3.Core.UI.Controls.nU3TextEdit();
            lblInstallPathDefaults = new nU3.Core.UI.Controls.nU3LabelControl();
            _lblPathPreview = new nU3.Core.UI.Controls.nU3LabelControl();
            lblGroup = new nU3.Core.UI.Controls.nU3LabelControl();
            _txtGroupName = new nU3.Core.UI.Controls.nU3TextEdit();
            lblGroupHelp = new nU3.Core.UI.Controls.nU3LabelControl();
            lblPriority = new nU3.Core.UI.Controls.nU3LabelControl();
            _nudPriority = new nU3.Core.UI.Controls.nU3SpinEdit();
            lblPriorityHelp = new nU3.Core.UI.Controls.nU3LabelControl();
            _chkRequired = new nU3.Core.UI.Controls.nU3CheckEdit();
            _chkAutoUpdate = new nU3.Core.UI.Controls.nU3CheckEdit();
            lblDeps = new nU3.Core.UI.Controls.nU3LabelControl();
            _txtDependencies = new nU3.Core.UI.Controls.nU3TextEdit();
            lblDepsHelp = new nU3.Core.UI.Controls.nU3LabelControl();
            lblDesc = new nU3.Core.UI.Controls.nU3LabelControl();
            _txtDescription = new nU3.Core.UI.Controls.nU3MemoEdit();
            _dgvComponents = new nU3.Core.UI.Controls.nU3GridControl();
            _gvComponents = new nU3.Core.UI.Controls.nU3GridView();
            panel1 = new DevExpress.XtraEditors.PanelControl();
            splitContainer1 = new DevExpress.XtraEditors.SplitContainerControl();
            panel2 = new DevExpress.XtraEditors.PanelControl();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            btnNewComponent = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnSave = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnRefresh = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnBulkDeploy = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnSmartDeploy = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnDelete = new nU3.Core.UI.Controls.nU3SimpleButton();
            ((System.ComponentModel.ISupportInitialize)_panelTop).BeginInit();
            _panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_dgvVersions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_gvVersions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_panelDetail).BeginInit();
            _panelDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_cboComponentType.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_txtComponentId.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_txtComponentName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_txtFileName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_txtInstallPath.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_txtGroupName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudPriority.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_chkRequired.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_chkAutoUpdate.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_txtDependencies.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_txtDescription.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_dgvComponents).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_gvComponents).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panel1).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1.Panel1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1.Panel2).BeginInit();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panel2).BeginInit();
            panel2.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // _panelTop
            // 
            _panelTop.Controls.Add(_dgvVersions);
            _panelTop.Controls.Add(lblVersions);
            _panelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            _panelTop.Location = new System.Drawing.Point(9, 9);
            _panelTop.Name = "_panelTop";
            _panelTop.Size = new System.Drawing.Size(559, 144);
            _panelTop.TabIndex = 0;
            // 
            // _dgvVersions
            // 
            _dgvVersions.Dock = System.Windows.Forms.DockStyle.Fill;
            _dgvVersions.Location = new System.Drawing.Point(2, 27);
            _dgvVersions.MainView = _gvVersions;
            _dgvVersions.MinimumSize = new System.Drawing.Size(0, 200);
            _dgvVersions.Name = "_dgvVersions";
            _dgvVersions.Size = new System.Drawing.Size(555, 200);
            _dgvVersions.TabIndex = 1;
            _dgvVersions.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { _gvVersions });
            // 
            // _gvVersions
            // 
            _gvVersions.GridControl = _dgvVersions;
            _gvVersions.Name = "_gvVersions";
            _gvVersions.OptionsBehavior.Editable = false;
            _gvVersions.OptionsView.ShowGroupPanel = false;
            // 
            // lblVersions
            // 
            lblVersions.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblVersions.Dock = System.Windows.Forms.DockStyle.Top;
            lblVersions.IsRequiredMarker = false;
            lblVersions.Location = new System.Drawing.Point(2, 2);
            lblVersions.Name = "lblVersions";
            lblVersions.Size = new System.Drawing.Size(555, 25);
            lblVersions.TabIndex = 0;
            lblVersions.Text = "버전 이력";
            // 
            // _panelDetail
            // 
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
            _panelDetail.Location = new System.Drawing.Point(9, 253);
            _panelDetail.Name = "_panelDetail";
            _panelDetail.Size = new System.Drawing.Size(559, 572);
            _panelDetail.TabIndex = 0;
            // 
            // btnAsyncTest
            // 
            btnAsyncTest.AuthId = "";
            btnAsyncTest.Location = new System.Drawing.Point(10, 473);
            btnAsyncTest.Name = "btnAsyncTest";
            btnAsyncTest.Size = new System.Drawing.Size(80, 23);
            btnAsyncTest.TabIndex = 7;
            btnAsyncTest.Text = "Async Test";
            btnAsyncTest.Click += BtnAsyncTest_Click;
            // 
            // btnSyncTest
            // 
            btnSyncTest.AuthId = "";
            btnSyncTest.Location = new System.Drawing.Point(10, 496);
            btnSyncTest.Name = "btnSyncTest";
            btnSyncTest.Size = new System.Drawing.Size(80, 23);
            btnSyncTest.TabIndex = 6;
            btnSyncTest.Text = "Sync Test";
            btnSyncTest.Click += BtnSyncTest_Click;
            // 
            // lblType
            // 
            lblType.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblType.IsRequiredMarker = false;
            lblType.Location = new System.Drawing.Point(10, 10);
            lblType.Name = "lblType";
            lblType.Size = new System.Drawing.Size(120, 23);
            lblType.TabIndex = 0;
            lblType.Text = "유형:";
            // 
            // _cboComponentType
            // 
            _cboComponentType.IsRequired = false;
            _cboComponentType.Location = new System.Drawing.Point(140, 10);
            _cboComponentType.Name = "_cboComponentType";
            _cboComponentType.Properties.AutoHeight = false;
            _cboComponentType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            _cboComponentType.Properties.Items.AddRange(new object[] { "ScreenModule", "FrameworkCore", "SharedLibrary", "Executable", "Configuration", "Resource", "Plugin", "Other" });
            _cboComponentType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            _cboComponentType.Size = new System.Drawing.Size(250, 23);
            _cboComponentType.TabIndex = 1;
            _cboComponentType.SelectedIndexChanged += CboComponentType_SelectedIndexChanged;
            // 
            // lblTypeHelp
            // 
            lblTypeHelp.Appearance.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            lblTypeHelp.Appearance.ForeColor = System.Drawing.Color.DarkGreen;
            lblTypeHelp.Appearance.Options.UseFont = true;
            lblTypeHelp.Appearance.Options.UseForeColor = true;
            lblTypeHelp.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblTypeHelp.IsRequiredMarker = false;
            lblTypeHelp.Location = new System.Drawing.Point(140, 32);
            lblTypeHelp.Name = "lblTypeHelp";
            lblTypeHelp.Size = new System.Drawing.Size(250, 23);
            lblTypeHelp.TabIndex = 2;
            lblTypeHelp.Text = "유형 선택 시 설치경로/우선순위 자동 설정";
            // 
            // lblId
            // 
            lblId.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblId.IsRequiredMarker = false;
            lblId.Location = new System.Drawing.Point(10, 60);
            lblId.Name = "lblId";
            lblId.Size = new System.Drawing.Size(120, 23);
            lblId.TabIndex = 3;
            lblId.Text = "Component ID:";
            // 
            // _txtComponentId
            // 
            _txtComponentId.IsRequired = false;
            _txtComponentId.Location = new System.Drawing.Point(140, 60);
            _txtComponentId.Name = "_txtComponentId";
            _txtComponentId.Properties.AutoHeight = false;
            _txtComponentId.Size = new System.Drawing.Size(250, 23);
            _txtComponentId.TabIndex = 4;
            // 
            // lblIdHelp
            // 
            lblIdHelp.Appearance.Font = new System.Drawing.Font("Segoe UI", 8F);
            lblIdHelp.Appearance.ForeColor = System.Drawing.Color.Gray;
            lblIdHelp.Appearance.Options.UseFont = true;
            lblIdHelp.Appearance.Options.UseForeColor = true;
            lblIdHelp.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblIdHelp.IsRequiredMarker = false;
            lblIdHelp.Location = new System.Drawing.Point(140, 82);
            lblIdHelp.Name = "lblIdHelp";
            lblIdHelp.Size = new System.Drawing.Size(250, 23);
            lblIdHelp.TabIndex = 5;
            lblIdHelp.Text = "예: nU3.Core, DevExpress.XtraEditors";
            // 
            // lblName
            // 
            lblName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblName.IsRequiredMarker = false;
            lblName.Location = new System.Drawing.Point(10, 110);
            lblName.Name = "lblName";
            lblName.Size = new System.Drawing.Size(120, 23);
            lblName.TabIndex = 6;
            lblName.Text = "이름:";
            // 
            // _txtComponentName
            // 
            _txtComponentName.IsRequired = false;
            _txtComponentName.Location = new System.Drawing.Point(140, 110);
            _txtComponentName.Name = "_txtComponentName";
            _txtComponentName.Properties.AutoHeight = false;
            _txtComponentName.Size = new System.Drawing.Size(250, 23);
            _txtComponentName.TabIndex = 7;
            // 
            // lblFile
            // 
            lblFile.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblFile.IsRequiredMarker = false;
            lblFile.Location = new System.Drawing.Point(10, 140);
            lblFile.Name = "lblFile";
            lblFile.Size = new System.Drawing.Size(120, 23);
            lblFile.TabIndex = 8;
            lblFile.Text = "파일명:";
            // 
            // _txtFileName
            // 
            _txtFileName.IsRequired = false;
            _txtFileName.Location = new System.Drawing.Point(140, 140);
            _txtFileName.Name = "_txtFileName";
            _txtFileName.Properties.AutoHeight = false;
            _txtFileName.Size = new System.Drawing.Size(250, 23);
            _txtFileName.TabIndex = 9;
            _txtFileName.TextChanged += TxtFileName_TextChanged;
            // 
            // lblFileHelp
            // 
            lblFileHelp.Appearance.Font = new System.Drawing.Font("Segoe UI", 8F);
            lblFileHelp.Appearance.ForeColor = System.Drawing.Color.Gray;
            lblFileHelp.Appearance.Options.UseFont = true;
            lblFileHelp.Appearance.Options.UseForeColor = true;
            lblFileHelp.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblFileHelp.IsRequiredMarker = false;
            lblFileHelp.Location = new System.Drawing.Point(140, 162);
            lblFileHelp.Name = "lblFileHelp";
            lblFileHelp.Size = new System.Drawing.Size(250, 23);
            lblFileHelp.TabIndex = 10;
            lblFileHelp.Text = "예: nU3.Core.dll, MyApp.exe";
            // 
            // lblPath
            // 
            lblPath.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblPath.IsRequiredMarker = false;
            lblPath.Location = new System.Drawing.Point(10, 190);
            lblPath.Name = "lblPath";
            lblPath.Size = new System.Drawing.Size(120, 23);
            lblPath.TabIndex = 11;
            lblPath.Text = "설치 경로 (상대):";
            // 
            // _txtInstallPath
            // 
            _txtInstallPath.IsRequired = false;
            _txtInstallPath.Location = new System.Drawing.Point(140, 190);
            _txtInstallPath.Name = "_txtInstallPath";
            _txtInstallPath.Properties.AutoHeight = false;
            _txtInstallPath.Size = new System.Drawing.Size(250, 23);
            _txtInstallPath.TabIndex = 12;
            _txtInstallPath.TextChanged += TxtInstallPath_TextChanged;
            // 
            // lblInstallPathDefaults
            // 
            lblInstallPathDefaults.Appearance.Font = new System.Drawing.Font("Segoe UI", 8F);
            lblInstallPathDefaults.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
            lblInstallPathDefaults.Appearance.Options.UseFont = true;
            lblInstallPathDefaults.Appearance.Options.UseForeColor = true;
            lblInstallPathDefaults.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblInstallPathDefaults.IsRequiredMarker = false;
            lblInstallPathDefaults.Location = new System.Drawing.Point(140, 212);
            lblInstallPathDefaults.Name = "lblInstallPathDefaults";
            lblInstallPathDefaults.Size = new System.Drawing.Size(350, 23);
            lblInstallPathDefaults.TabIndex = 13;
            lblInstallPathDefaults.Text = "Core/Lib/Exe→루트, Plugin→plugins, Resource→resources";
            // 
            // _lblPathPreview
            // 
            _lblPathPreview.Appearance.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            _lblPathPreview.Appearance.ForeColor = System.Drawing.Color.Blue;
            _lblPathPreview.Appearance.Options.UseFont = true;
            _lblPathPreview.Appearance.Options.UseForeColor = true;
            _lblPathPreview.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            _lblPathPreview.IsRequiredMarker = false;
            _lblPathPreview.Location = new System.Drawing.Point(140, 232);
            _lblPathPreview.Name = "_lblPathPreview";
            _lblPathPreview.Size = new System.Drawing.Size(350, 23);
            _lblPathPreview.TabIndex = 14;
            _lblPathPreview.Text = "최종 경로: {실행경로}\\{파일명}";
            // 
            // lblGroup
            // 
            lblGroup.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblGroup.IsRequiredMarker = false;
            lblGroup.Location = new System.Drawing.Point(10, 260);
            lblGroup.Name = "lblGroup";
            lblGroup.Size = new System.Drawing.Size(120, 23);
            lblGroup.TabIndex = 15;
            lblGroup.Text = "그룹:";
            // 
            // _txtGroupName
            // 
            _txtGroupName.IsRequired = false;
            _txtGroupName.Location = new System.Drawing.Point(140, 260);
            _txtGroupName.Name = "_txtGroupName";
            _txtGroupName.Properties.AutoHeight = false;
            _txtGroupName.Size = new System.Drawing.Size(250, 23);
            _txtGroupName.TabIndex = 16;
            // 
            // lblGroupHelp
            // 
            lblGroupHelp.Appearance.Font = new System.Drawing.Font("Segoe UI", 8F);
            lblGroupHelp.Appearance.ForeColor = System.Drawing.Color.Gray;
            lblGroupHelp.Appearance.Options.UseFont = true;
            lblGroupHelp.Appearance.Options.UseForeColor = true;
            lblGroupHelp.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblGroupHelp.IsRequiredMarker = false;
            lblGroupHelp.Location = new System.Drawing.Point(140, 282);
            lblGroupHelp.Name = "lblGroupHelp";
            lblGroupHelp.Size = new System.Drawing.Size(250, 23);
            lblGroupHelp.TabIndex = 17;
            lblGroupHelp.Text = "예: Framework, DevExpress, Oracle";
            // 
            // lblPriority
            // 
            lblPriority.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblPriority.IsRequiredMarker = false;
            lblPriority.Location = new System.Drawing.Point(10, 310);
            lblPriority.Name = "lblPriority";
            lblPriority.Size = new System.Drawing.Size(120, 23);
            lblPriority.TabIndex = 18;
            lblPriority.Text = "우선순위:";
            // 
            // _nudPriority
            // 
            _nudPriority.EditValue = new decimal(new int[] { 100, 0, 0, 0 });
            _nudPriority.IsRequired = false;
            _nudPriority.Location = new System.Drawing.Point(140, 310);
            _nudPriority.Name = "_nudPriority";
            _nudPriority.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            _nudPriority.Properties.IsFloatValue = false;
            _nudPriority.Properties.MaskSettings.Set("mask", "N00");
            _nudPriority.Properties.MaxValue = new decimal(new int[] { 999, 0, 0, 0 });
            _nudPriority.Size = new System.Drawing.Size(80, 20);
            _nudPriority.TabIndex = 19;
            _nudPriority.ValueChanged += CboComponentType_SelectedIndexChanged;
            // 
            // lblPriorityHelp
            // 
            lblPriorityHelp.Appearance.Font = new System.Drawing.Font("Segoe UI", 8F);
            lblPriorityHelp.Appearance.ForeColor = System.Drawing.Color.Gray;
            lblPriorityHelp.Appearance.Options.UseFont = true;
            lblPriorityHelp.Appearance.Options.UseForeColor = true;
            lblPriorityHelp.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblPriorityHelp.IsRequiredMarker = false;
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
            _chkRequired.Properties.AutoHeight = false;
            _chkRequired.Properties.Caption = "필수 컴포넌트";
            _chkRequired.Size = new System.Drawing.Size(150, 23);
            _chkRequired.TabIndex = 21;
            // 
            // _chkAutoUpdate
            // 
            _chkAutoUpdate.EditValue = true;
            _chkAutoUpdate.Location = new System.Drawing.Point(140, 370);
            _chkAutoUpdate.Name = "_chkAutoUpdate";
            _chkAutoUpdate.Properties.AutoHeight = false;
            _chkAutoUpdate.Properties.Caption = "자동 업데이트";
            _chkAutoUpdate.Size = new System.Drawing.Size(150, 23);
            _chkAutoUpdate.TabIndex = 22;
            // 
            // lblDeps
            // 
            lblDeps.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblDeps.IsRequiredMarker = false;
            lblDeps.Location = new System.Drawing.Point(10, 400);
            lblDeps.Name = "lblDeps";
            lblDeps.Size = new System.Drawing.Size(120, 23);
            lblDeps.TabIndex = 23;
            lblDeps.Text = "의존성:";
            // 
            // _txtDependencies
            // 
            _txtDependencies.IsRequired = false;
            _txtDependencies.Location = new System.Drawing.Point(140, 400);
            _txtDependencies.Name = "_txtDependencies";
            _txtDependencies.Properties.AutoHeight = false;
            _txtDependencies.Size = new System.Drawing.Size(250, 23);
            _txtDependencies.TabIndex = 24;
            // 
            // lblDepsHelp
            // 
            lblDepsHelp.Appearance.Font = new System.Drawing.Font("Segoe UI", 8F);
            lblDepsHelp.Appearance.ForeColor = System.Drawing.Color.Gray;
            lblDepsHelp.Appearance.Options.UseFont = true;
            lblDepsHelp.Appearance.Options.UseForeColor = true;
            lblDepsHelp.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblDepsHelp.IsRequiredMarker = false;
            lblDepsHelp.Location = new System.Drawing.Point(140, 422);
            lblDepsHelp.Name = "lblDepsHelp";
            lblDepsHelp.Size = new System.Drawing.Size(250, 23);
            lblDepsHelp.TabIndex = 25;
            lblDepsHelp.Text = "쉼표 구분, 예: nU3.Core,System.Data";
            // 
            // lblDesc
            // 
            lblDesc.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblDesc.IsRequiredMarker = false;
            lblDesc.Location = new System.Drawing.Point(10, 450);
            lblDesc.Name = "lblDesc";
            lblDesc.Size = new System.Drawing.Size(120, 23);
            lblDesc.TabIndex = 26;
            lblDesc.Text = "설명:";
            // 
            // _txtDescription
            // 
            _txtDescription.IsRequired = false;
            _txtDescription.Location = new System.Drawing.Point(140, 450);
            _txtDescription.Name = "_txtDescription";
            _txtDescription.Size = new System.Drawing.Size(250, 60);
            _txtDescription.TabIndex = 27;
            // 
            // _dgvComponents
            // 
            _dgvComponents.Dock = System.Windows.Forms.DockStyle.Fill;
            _dgvComponents.Location = new System.Drawing.Point(0, 0);
            _dgvComponents.MainView = _gvComponents;
            _dgvComponents.Name = "_dgvComponents";
            _dgvComponents.Size = new System.Drawing.Size(700, 834);
            _dgvComponents.TabIndex = 0;
            _dgvComponents.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { _gvComponents });
            // 
            // _gvComponents
            // 
            _gvComponents.GridControl = _dgvComponents;
            _gvComponents.Name = "_gvComponents";
            _gvComponents.OptionsBehavior.Editable = false;
            _gvComponents.OptionsView.ShowGroupPanel = false;
            _gvComponents.SelectionChanged += DgvComponents_SelectionChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(splitContainer1);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Padding = new System.Windows.Forms.Padding(7);
            panel1.Size = new System.Drawing.Size(1305, 852);
            panel1.TabIndex = 2;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(9, 9);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(_dgvComponents);
            splitContainer1.Panel1.Text = "Panel1";
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panel2);
            splitContainer1.Panel2.Text = "Panel2";
            splitContainer1.Size = new System.Drawing.Size(1287, 834);
            splitContainer1.SplitterPosition = 700;
            splitContainer1.TabIndex = 4;
            // 
            // panel2
            // 
            panel2.Controls.Add(_panelTop);
            panel2.Controls.Add(tableLayoutPanel1);
            panel2.Controls.Add(_panelDetail);
            panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            panel2.Location = new System.Drawing.Point(0, 0);
            panel2.Name = "panel2";
            panel2.Padding = new System.Windows.Forms.Padding(7);
            panel2.Size = new System.Drawing.Size(577, 834);
            panel2.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel1.Controls.Add(btnNewComponent, 2, 1);
            tableLayoutPanel1.Controls.Add(btnSave, 1, 1);
            tableLayoutPanel1.Controls.Add(btnRefresh, 0, 0);
            tableLayoutPanel1.Controls.Add(btnBulkDeploy, 1, 2);
            tableLayoutPanel1.Controls.Add(btnSmartDeploy, 1, 0);
            tableLayoutPanel1.Controls.Add(btnDelete, 2, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            tableLayoutPanel1.Location = new System.Drawing.Point(9, 153);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel1.Size = new System.Drawing.Size(559, 100);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // btnNewComponent
            // 
            btnNewComponent.AuthId = "";
            btnNewComponent.Dock = System.Windows.Forms.DockStyle.Fill;
            btnNewComponent.Location = new System.Drawing.Point(372, 33);
            btnNewComponent.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            btnNewComponent.Name = "btnNewComponent";
            btnNewComponent.Size = new System.Drawing.Size(177, 33);
            btnNewComponent.TabIndex = 5;
            btnNewComponent.Text = "신규";
            btnNewComponent.Click += BtnNewComponent_Click;
            // 
            // btnSave
            // 
            btnSave.AuthId = "";
            btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            btnSave.Location = new System.Drawing.Point(186, 33);
            btnSave.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(176, 33);
            btnSave.TabIndex = 2;
            btnSave.Text = "저장";
            btnSave.Click += BtnSave_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.AuthId = "";
            btnRefresh.Dock = System.Windows.Forms.DockStyle.Fill;
            btnRefresh.Location = new System.Drawing.Point(7, 7);
            btnRefresh.Margin = new System.Windows.Forms.Padding(7);
            btnRefresh.Name = "btnRefresh";
            tableLayoutPanel1.SetRowSpan(btnRefresh, 3);
            btnRefresh.Size = new System.Drawing.Size(172, 86);
            btnRefresh.TabIndex = 0;
            btnRefresh.Text = "새로고침";
            btnRefresh.Click += BtnRefresh_Click;
            // 
            // btnBulkDeploy
            // 
            btnBulkDeploy.AuthId = "";
            btnBulkDeploy.Dock = System.Windows.Forms.DockStyle.Fill;
            btnBulkDeploy.Location = new System.Drawing.Point(189, 69);
            btnBulkDeploy.Name = "btnBulkDeploy";
            btnBulkDeploy.Size = new System.Drawing.Size(180, 28);
            btnBulkDeploy.TabIndex = 4;
            btnBulkDeploy.Text = "폴더 일괄 배포";
            btnBulkDeploy.Click += BtnBulkDeploy_Click;
            // 
            // btnSmartDeploy
            // 
            btnSmartDeploy.AuthId = "";
            btnSmartDeploy.Dock = System.Windows.Forms.DockStyle.Fill;
            btnSmartDeploy.Location = new System.Drawing.Point(186, 0);
            btnSmartDeploy.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            btnSmartDeploy.Name = "btnSmartDeploy";
            btnSmartDeploy.Size = new System.Drawing.Size(176, 33);
            btnSmartDeploy.TabIndex = 1;
            btnSmartDeploy.Text = "개별 배포";
            btnSmartDeploy.Click += BtnSmartDeploy_Click;
            // 
            // btnDelete
            // 
            btnDelete.AuthId = "";
            btnDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            btnDelete.Location = new System.Drawing.Point(372, 0);
            btnDelete.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new System.Drawing.Size(177, 33);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "삭제";
            btnDelete.Click += BtnDelete_Click;
            // 
            // AssemblyDeployManagementControl
            // 
            Controls.Add(panel1);
            Name = "AssemblyDeployManagementControl";
            Size = new System.Drawing.Size(1305, 852);
            ((System.ComponentModel.ISupportInitialize)_panelTop).EndInit();
            _panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_dgvVersions).EndInit();
            ((System.ComponentModel.ISupportInitialize)_gvVersions).EndInit();
            ((System.ComponentModel.ISupportInitialize)_panelDetail).EndInit();
            _panelDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_cboComponentType.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)_txtComponentId.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)_txtComponentName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)_txtFileName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)_txtInstallPath.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)_txtGroupName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudPriority.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)_chkRequired.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)_chkAutoUpdate.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)_txtDependencies.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)_txtDescription.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)_dgvComponents).EndInit();
            ((System.ComponentModel.ISupportInitialize)_gvComponents).EndInit();
            ((System.ComponentModel.ISupportInitialize)panel1).EndInit();
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1.Panel1).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1.Panel2).EndInit();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)panel2).EndInit();
            panel2.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private nU3.Core.UI.Controls.nU3GridControl _dgvComponents;
        private nU3.Core.UI.Controls.nU3GridView _gvComponents;
        private nU3.Core.UI.Controls.nU3GridControl _dgvVersions;
        private nU3.Core.UI.Controls.nU3GridView _gvVersions;
        private DevExpress.XtraEditors.PanelControl _panelTop;
        private DevExpress.XtraEditors.PanelControl _panelDetail;

        private nU3.Core.UI.Controls.nU3TextEdit _txtComponentId;
        private nU3.Core.UI.Controls.nU3TextEdit _txtComponentName;
        private nU3.Core.UI.Controls.nU3TextEdit _txtFileName;
        private nU3.Core.UI.Controls.nU3TextEdit _txtInstallPath;
        private nU3.Core.UI.Controls.nU3TextEdit _txtGroupName;
        private nU3.Core.UI.Controls.nU3MemoEdit _txtDescription;
        private nU3.Core.UI.Controls.nU3TextEdit _txtDependencies;
        private nU3.Core.UI.Controls.nU3ComboBoxEdit _cboComponentType;
        private nU3.Core.UI.Controls.nU3SpinEdit _nudPriority;
        private nU3.Core.UI.Controls.nU3CheckEdit _chkRequired;
        private nU3.Core.UI.Controls.nU3CheckEdit _chkAutoUpdate;
        private nU3.Core.UI.Controls.nU3LabelControl _lblPathPreview;
        private nU3.Core.UI.Controls.nU3SimpleButton btnRefresh;
        private nU3.Core.UI.Controls.nU3SimpleButton btnSmartDeploy;
        private nU3.Core.UI.Controls.nU3SimpleButton btnSave;
        private nU3.Core.UI.Controls.nU3SimpleButton btnDelete;
        private nU3.Core.UI.Controls.nU3SimpleButton btnBulkDeploy;
        private nU3.Core.UI.Controls.nU3SimpleButton btnNewComponent;
        private nU3.Core.UI.Controls.nU3SimpleButton btnSyncTest;
        private nU3.Core.UI.Controls.nU3SimpleButton btnAsyncTest;
        private nU3.Core.UI.Controls.nU3LabelControl lblType;
        private nU3.Core.UI.Controls.nU3LabelControl lblTypeHelp;
        private nU3.Core.UI.Controls.nU3LabelControl lblId;
        private nU3.Core.UI.Controls.nU3LabelControl lblIdHelp;
        private nU3.Core.UI.Controls.nU3LabelControl lblName;
        private nU3.Core.UI.Controls.nU3LabelControl lblFile;
        private nU3.Core.UI.Controls.nU3LabelControl lblFileHelp;
        private nU3.Core.UI.Controls.nU3LabelControl lblPath;
        private nU3.Core.UI.Controls.nU3LabelControl lblInstallPathDefaults;
        private nU3.Core.UI.Controls.nU3LabelControl lblGroup;
        private nU3.Core.UI.Controls.nU3LabelControl lblGroupHelp;
        private nU3.Core.UI.Controls.nU3LabelControl lblPriority;
        private nU3.Core.UI.Controls.nU3LabelControl lblPriorityHelp;
        private nU3.Core.UI.Controls.nU3LabelControl lblDeps;
        private nU3.Core.UI.Controls.nU3LabelControl lblDepsHelp;
        private nU3.Core.UI.Controls.nU3LabelControl lblDesc;
        private nU3.Core.UI.Controls.nU3LabelControl lblVersions;
        private DevExpress.XtraEditors.PanelControl panel1;
        private DevExpress.XtraEditors.PanelControl panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.SplitContainerControl splitContainer1;
    }
}