namespace nU3.Tools.Deployer.Views
{
    partial class ProgramDeployManagementControl
    {
        private System.ComponentModel.IContainer components = null;

        private DevExpress.XtraEditors.SplitContainerControl splitMain;

        // Left (scan)
        private DevExpress.XtraEditors.PanelControl pnlScanTop;
        private nU3.Core.UI.Controls.nU3LabelControl lblModulesRoot;
        private nU3.Core.UI.Controls.nU3TextEdit txtModulesRoot;
        private nU3.Core.UI.Controls.nU3SimpleButton btnBrowseModulesRoot;
        private nU3.Core.UI.Controls.nU3SimpleButton btnScanModules;
        private nU3.Core.UI.Controls.nU3GridControl dgvModuleFiles;
        private nU3.Core.UI.Controls.nU3GridView gvModuleFiles;
        private nU3.Core.UI.Controls.nU3GridControl dgvDbVersions;
        private nU3.Core.UI.Controls.nU3GridView gvDbVersions;

        // Center filter (retained)
        private DevExpress.XtraEditors.PanelControl pnlFilter;
        private nU3.Core.UI.Controls.nU3LabelControl lblFilterCategory;
        private nU3.Core.UI.Controls.nU3ComboBoxEdit cboFilterCategory;
        private nU3.Core.UI.Controls.nU3LabelControl lblFilterSubSystem;
        private nU3.Core.UI.Controls.nU3ComboBoxEdit cboFilterSubSystem;

        // Right (edit - base fields retained)
        private DevExpress.XtraEditors.PanelControl pnlEdit;
        private nU3.Core.UI.Controls.nU3LabelControl lblId;
        private nU3.Core.UI.Controls.nU3TextEdit txtModuleId;
        private nU3.Core.UI.Controls.nU3LabelControl lblName;
        private nU3.Core.UI.Controls.nU3TextEdit txtModuleName;
        private nU3.Core.UI.Controls.nU3LabelControl lblCat;
        private nU3.Core.UI.Controls.nU3ComboBoxEdit cboCategory;
        private nU3.Core.UI.Controls.nU3LabelControl lblSub;
        private nU3.Core.UI.Controls.nU3TextEdit txtSubSystem;
        private nU3.Core.UI.Controls.nU3LabelControl lblFile;
        private nU3.Core.UI.Controls.nU3TextEdit txtFileName;
        private nU3.Core.UI.Controls.nU3SimpleButton btnAdd;
        private nU3.Core.UI.Controls.nU3SimpleButton btnDelete;
        private nU3.Core.UI.Controls.nU3SimpleButton btnRefresh;
        private nU3.Core.UI.Controls.nU3SimpleButton btnSmartUpload;

        // Right (edit)
        private System.Windows.Forms.TableLayoutPanel tblEdit;
        private nU3.Core.UI.Controls.nU3LabelControl lblVer;
        private nU3.Core.UI.Controls.nU3TextEdit txtActiveVersion;
        private nU3.Core.UI.Controls.nU3LabelControl lblHash;
        private nU3.Core.UI.Controls.nU3TextEdit txtFileHash;
        private nU3.Core.UI.Controls.nU3LabelControl lblSize;
        private nU3.Core.UI.Controls.nU3TextEdit txtFileSize;
        private nU3.Core.UI.Controls.nU3LabelControl lblStorage;
        private nU3.Core.UI.Controls.nU3TextEdit txtStoragePath;
        private nU3.Core.UI.Controls.nU3LabelControl lblDeployDesc;
        private nU3.Core.UI.Controls.nU3TextEdit txtDeployDesc;

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
            dgvModuleFiles = new nU3.Core.UI.Controls.nU3GridControl();
            gvModuleFiles = new nU3.Core.UI.Controls.nU3GridView();
            pnlScanTop = new DevExpress.XtraEditors.PanelControl();
            txtModulesRoot = new nU3.Core.UI.Controls.nU3TextEdit();
            btnBrowseModulesRoot = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnScanModules = new nU3.Core.UI.Controls.nU3SimpleButton();
            chkUpdated = new nU3.Core.UI.Controls.nU3CheckEdit();
            lblModulesRoot = new nU3.Core.UI.Controls.nU3LabelControl();
            tblEdit = new System.Windows.Forms.TableLayoutPanel();
            txtModuleId = new nU3.Core.UI.Controls.nU3TextEdit();
            lblName = new nU3.Core.UI.Controls.nU3LabelControl();
            txtModuleName = new nU3.Core.UI.Controls.nU3TextEdit();
            cboCategory = new nU3.Core.UI.Controls.nU3ComboBoxEdit();
            lblSub = new nU3.Core.UI.Controls.nU3LabelControl();
            txtSubSystem = new nU3.Core.UI.Controls.nU3TextEdit();
            lblFile = new nU3.Core.UI.Controls.nU3LabelControl();
            txtFileName = new nU3.Core.UI.Controls.nU3TextEdit();
            lblVer = new nU3.Core.UI.Controls.nU3LabelControl();
            txtActiveVersion = new nU3.Core.UI.Controls.nU3TextEdit();
            lblHash = new nU3.Core.UI.Controls.nU3LabelControl();
            txtFileHash = new nU3.Core.UI.Controls.nU3TextEdit();
            lblSize = new nU3.Core.UI.Controls.nU3LabelControl();
            txtFileSize = new nU3.Core.UI.Controls.nU3TextEdit();
            lblStorage = new nU3.Core.UI.Controls.nU3LabelControl();
            txtStoragePath = new nU3.Core.UI.Controls.nU3TextEdit();
            lblDeployDesc = new nU3.Core.UI.Controls.nU3LabelControl();
            txtDeployDesc = new nU3.Core.UI.Controls.nU3TextEdit();
            lblCat = new nU3.Core.UI.Controls.nU3LabelControl();
            lblId = new nU3.Core.UI.Controls.nU3LabelControl();
            dgvDbVersions = new nU3.Core.UI.Controls.nU3GridControl();
            gvDbVersions = new nU3.Core.UI.Controls.nU3GridView();
            pnlFilter = new DevExpress.XtraEditors.PanelControl();
            cboFilterCategory = new nU3.Core.UI.Controls.nU3ComboBoxEdit();
            lblFilterCategory = new nU3.Core.UI.Controls.nU3LabelControl();
            lblFilterSubSystem = new nU3.Core.UI.Controls.nU3LabelControl();
            cboFilterSubSystem = new nU3.Core.UI.Controls.nU3ComboBoxEdit();
            pnlEdit = new DevExpress.XtraEditors.PanelControl();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            btnRefresh = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnDelete = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnSmartUpload = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnAdd = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnBulkUpload = new nU3.Core.UI.Controls.nU3SimpleButton();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitMain.Panel1).BeginInit();
            splitMain.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain.Panel2).BeginInit();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvModuleFiles).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gvModuleFiles).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlScanTop).BeginInit();
            pnlScanTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtModulesRoot.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chkUpdated.Properties).BeginInit();
            tblEdit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtModuleId.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtModuleName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cboCategory.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtSubSystem.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtFileName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtActiveVersion.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtFileHash.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtFileSize.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtStoragePath.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtDeployDesc.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvDbVersions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gvDbVersions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlFilter).BeginInit();
            pnlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)cboFilterCategory.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cboFilterSubSystem.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlEdit).BeginInit();
            pnlEdit.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
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
            splitMain.Panel1.Controls.Add(dgvModuleFiles);
            splitMain.Panel1.Controls.Add(pnlScanTop);
            splitMain.Panel1.Text = "Panel1";
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(tblEdit);
            splitMain.Panel2.Controls.Add(dgvDbVersions);
            splitMain.Panel2.Controls.Add(pnlFilter);
            splitMain.Panel2.Controls.Add(pnlEdit);
            splitMain.Panel2.Text = "Panel2";
            splitMain.Size = new System.Drawing.Size(2214, 1207);
            splitMain.SplitterPosition = 725;
            splitMain.TabIndex = 0;
            // 
            // dgvModuleFiles
            // 
            dgvModuleFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvModuleFiles.Location = new System.Drawing.Point(0, 40);
            dgvModuleFiles.MainView = gvModuleFiles;
            dgvModuleFiles.Name = "dgvModuleFiles";
            dgvModuleFiles.Size = new System.Drawing.Size(725, 1167);
            dgvModuleFiles.TabIndex = 0;
            dgvModuleFiles.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvModuleFiles });
            // 
            // gvModuleFiles
            // 
            gvModuleFiles.GridControl = dgvModuleFiles;
            gvModuleFiles.Name = "gvModuleFiles";
            gvModuleFiles.OptionsBehavior.Editable = false;
            gvModuleFiles.OptionsView.ShowGroupPanel = false;
            // 
            // pnlScanTop
            // 
            pnlScanTop.Controls.Add(txtModulesRoot);
            pnlScanTop.Controls.Add(btnBrowseModulesRoot);
            pnlScanTop.Controls.Add(btnScanModules);
            pnlScanTop.Controls.Add(chkUpdated);
            pnlScanTop.Controls.Add(lblModulesRoot);
            pnlScanTop.Dock = System.Windows.Forms.DockStyle.Top;
            pnlScanTop.Location = new System.Drawing.Point(0, 0);
            pnlScanTop.Name = "pnlScanTop";
            pnlScanTop.Size = new System.Drawing.Size(725, 40);
            pnlScanTop.TabIndex = 1;
            // 
            // txtModulesRoot
            // 
            txtModulesRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            txtModulesRoot.Location = new System.Drawing.Point(92, 2);
            txtModulesRoot.Name = "txtModulesRoot";
            txtModulesRoot.Properties.AutoHeight = false;
            txtModulesRoot.Properties.ReadOnly = true;
            txtModulesRoot.Size = new System.Drawing.Size(287, 36);
            txtModulesRoot.TabIndex = 1;
            // 
            // btnBrowseModulesRoot
            // 
            btnBrowseModulesRoot.AuthId = "";
            btnBrowseModulesRoot.Dock = System.Windows.Forms.DockStyle.Right;
            btnBrowseModulesRoot.Location = new System.Drawing.Point(379, 2);
            btnBrowseModulesRoot.Margin = new System.Windows.Forms.Padding(7);
            btnBrowseModulesRoot.Name = "btnBrowseModulesRoot";
            btnBrowseModulesRoot.Size = new System.Drawing.Size(90, 36);
            btnBrowseModulesRoot.TabIndex = 2;
            btnBrowseModulesRoot.Text = "Browse";
            btnBrowseModulesRoot.Click += BtnBrowseModulesRoot_Click;
            // 
            // btnScanModules
            // 
            btnScanModules.AuthId = "";
            btnScanModules.Dock = System.Windows.Forms.DockStyle.Right;
            btnScanModules.Location = new System.Drawing.Point(469, 2);
            btnScanModules.Margin = new System.Windows.Forms.Padding(7);
            btnScanModules.Name = "btnScanModules";
            btnScanModules.Size = new System.Drawing.Size(90, 36);
            btnScanModules.TabIndex = 3;
            btnScanModules.Text = "Scan";
            btnScanModules.Click += BtnScanModules_Click;
            // 
            // chkUpdated
            // 
            chkUpdated.Dock = System.Windows.Forms.DockStyle.Right;
            chkUpdated.Location = new System.Drawing.Point(559, 2);
            chkUpdated.Margin = new System.Windows.Forms.Padding(3, 3, 13, 3);
            chkUpdated.Name = "chkUpdated";
            chkUpdated.Properties.AutoHeight = false;
            chkUpdated.Properties.Caption = "Need Update";
            chkUpdated.Size = new System.Drawing.Size(164, 36);
            chkUpdated.TabIndex = 4;
            // 
            // lblModulesRoot
            // 
            lblModulesRoot.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblModulesRoot.Dock = System.Windows.Forms.DockStyle.Left;
            lblModulesRoot.Location = new System.Drawing.Point(2, 2);
            lblModulesRoot.Name = "lblModulesRoot";
            lblModulesRoot.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            lblModulesRoot.Size = new System.Drawing.Size(90, 36);
            lblModulesRoot.TabIndex = 0;
            lblModulesRoot.Text = "Modules:";
            // 
            // tblEdit
            // 
            tblEdit.BackColor = System.Drawing.SystemColors.ActiveCaption;
            tblEdit.ColumnCount = 2;
            tblEdit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            tblEdit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblEdit.Controls.Add(txtModuleId, 1, 0);
            tblEdit.Controls.Add(lblName, 0, 1);
            tblEdit.Controls.Add(txtModuleName, 1, 1);
            tblEdit.Controls.Add(cboCategory, 1, 2);
            tblEdit.Controls.Add(lblSub, 0, 3);
            tblEdit.Controls.Add(txtSubSystem, 1, 3);
            tblEdit.Controls.Add(lblFile, 0, 4);
            tblEdit.Controls.Add(txtFileName, 1, 4);
            tblEdit.Controls.Add(lblVer, 0, 5);
            tblEdit.Controls.Add(txtActiveVersion, 1, 5);
            tblEdit.Controls.Add(lblHash, 0, 6);
            tblEdit.Controls.Add(txtFileHash, 1, 6);
            tblEdit.Controls.Add(lblSize, 0, 7);
            tblEdit.Controls.Add(txtFileSize, 1, 7);
            tblEdit.Controls.Add(lblStorage, 0, 8);
            tblEdit.Controls.Add(txtStoragePath, 1, 8);
            tblEdit.Controls.Add(lblDeployDesc, 0, 9);
            tblEdit.Controls.Add(txtDeployDesc, 1, 9);
            tblEdit.Controls.Add(lblCat, 0, 2);
            tblEdit.Controls.Add(lblId, 0, 0);
            tblEdit.Dock = System.Windows.Forms.DockStyle.Bottom;
            tblEdit.Location = new System.Drawing.Point(0, 836);
            tblEdit.Name = "tblEdit";
            tblEdit.RowCount = 10;
            tblEdit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tblEdit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tblEdit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tblEdit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tblEdit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tblEdit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tblEdit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tblEdit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tblEdit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tblEdit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tblEdit.Size = new System.Drawing.Size(1474, 272);
            tblEdit.TabIndex = 0;
            // 
            // txtModuleId
            // 
            txtModuleId.Dock = System.Windows.Forms.DockStyle.Fill;
            txtModuleId.Location = new System.Drawing.Point(103, 3);
            txtModuleId.Name = "txtModuleId";
            txtModuleId.Properties.AutoHeight = false;
            txtModuleId.Size = new System.Drawing.Size(1368, 21);
            txtModuleId.TabIndex = 1;
            // 
            // lblName
            // 
            lblName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblName.Dock = System.Windows.Forms.DockStyle.Top;
            lblName.Location = new System.Drawing.Point(3, 30);
            lblName.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblName.Name = "lblName";
            lblName.Size = new System.Drawing.Size(94, 21);
            lblName.TabIndex = 2;
            lblName.Text = "이름:";
            // 
            // txtModuleName
            // 
            txtModuleName.Dock = System.Windows.Forms.DockStyle.Fill;
            txtModuleName.Location = new System.Drawing.Point(103, 30);
            txtModuleName.Name = "txtModuleName";
            txtModuleName.Properties.AutoHeight = false;
            txtModuleName.Size = new System.Drawing.Size(1368, 21);
            txtModuleName.TabIndex = 3;
            // 
            // cboCategory
            // 
            cboCategory.Dock = System.Windows.Forms.DockStyle.Fill;
            cboCategory.Location = new System.Drawing.Point(103, 57);
            cboCategory.Name = "cboCategory";
            cboCategory.Properties.AutoHeight = false;
            cboCategory.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cboCategory.Size = new System.Drawing.Size(1368, 21);
            cboCategory.TabIndex = 5;
            // 
            // lblSub
            // 
            lblSub.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblSub.Location = new System.Drawing.Point(3, 84);
            lblSub.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblSub.Name = "lblSub";
            lblSub.Size = new System.Drawing.Size(90, 21);
            lblSub.TabIndex = 6;
            lblSub.Text = "서브시스템:";
            // 
            // txtSubSystem
            // 
            txtSubSystem.Dock = System.Windows.Forms.DockStyle.Fill;
            txtSubSystem.Location = new System.Drawing.Point(103, 84);
            txtSubSystem.Name = "txtSubSystem";
            txtSubSystem.Properties.AutoHeight = false;
            txtSubSystem.Size = new System.Drawing.Size(1368, 21);
            txtSubSystem.TabIndex = 7;
            // 
            // lblFile
            // 
            lblFile.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblFile.Location = new System.Drawing.Point(3, 111);
            lblFile.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblFile.Name = "lblFile";
            lblFile.Size = new System.Drawing.Size(90, 21);
            lblFile.TabIndex = 8;
            lblFile.Text = "파일명:";
            // 
            // txtFileName
            // 
            txtFileName.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFileName.Location = new System.Drawing.Point(103, 111);
            txtFileName.Name = "txtFileName";
            txtFileName.Properties.AutoHeight = false;
            txtFileName.Size = new System.Drawing.Size(1368, 21);
            txtFileName.TabIndex = 9;
            // 
            // lblVer
            // 
            lblVer.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblVer.Location = new System.Drawing.Point(3, 138);
            lblVer.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblVer.Name = "lblVer";
            lblVer.Size = new System.Drawing.Size(90, 21);
            lblVer.TabIndex = 10;
            lblVer.Text = "버전:";
            // 
            // txtActiveVersion
            // 
            txtActiveVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            txtActiveVersion.Location = new System.Drawing.Point(103, 138);
            txtActiveVersion.Name = "txtActiveVersion";
            txtActiveVersion.Properties.AutoHeight = false;
            txtActiveVersion.Size = new System.Drawing.Size(1368, 21);
            txtActiveVersion.TabIndex = 11;
            // 
            // lblHash
            // 
            lblHash.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblHash.Location = new System.Drawing.Point(3, 165);
            lblHash.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblHash.Name = "lblHash";
            lblHash.Size = new System.Drawing.Size(90, 21);
            lblHash.TabIndex = 12;
            lblHash.Text = "해시:";
            // 
            // txtFileHash
            // 
            txtFileHash.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFileHash.Location = new System.Drawing.Point(103, 165);
            txtFileHash.Name = "txtFileHash";
            txtFileHash.Properties.AutoHeight = false;
            txtFileHash.Size = new System.Drawing.Size(1368, 21);
            txtFileHash.TabIndex = 13;
            // 
            // lblSize
            // 
            lblSize.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblSize.Location = new System.Drawing.Point(3, 192);
            lblSize.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblSize.Name = "lblSize";
            lblSize.Size = new System.Drawing.Size(90, 21);
            lblSize.TabIndex = 14;
            lblSize.Text = "파일크기:";
            // 
            // txtFileSize
            // 
            txtFileSize.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFileSize.Location = new System.Drawing.Point(103, 192);
            txtFileSize.Name = "txtFileSize";
            txtFileSize.Properties.AutoHeight = false;
            txtFileSize.Size = new System.Drawing.Size(1368, 21);
            txtFileSize.TabIndex = 15;
            // 
            // lblStorage
            // 
            lblStorage.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblStorage.Location = new System.Drawing.Point(3, 219);
            lblStorage.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblStorage.Name = "lblStorage";
            lblStorage.Size = new System.Drawing.Size(90, 21);
            lblStorage.TabIndex = 16;
            lblStorage.Text = "저장경로:";
            // 
            // txtStoragePath
            // 
            txtStoragePath.Dock = System.Windows.Forms.DockStyle.Fill;
            txtStoragePath.Location = new System.Drawing.Point(103, 219);
            txtStoragePath.Name = "txtStoragePath";
            txtStoragePath.Properties.AutoHeight = false;
            txtStoragePath.Size = new System.Drawing.Size(1368, 21);
            txtStoragePath.TabIndex = 17;
            // 
            // lblDeployDesc
            // 
            lblDeployDesc.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblDeployDesc.Location = new System.Drawing.Point(3, 246);
            lblDeployDesc.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblDeployDesc.Name = "lblDeployDesc";
            lblDeployDesc.Size = new System.Drawing.Size(90, 25);
            lblDeployDesc.TabIndex = 18;
            lblDeployDesc.Text = "배포설명:";
            // 
            // txtDeployDesc
            // 
            txtDeployDesc.Dock = System.Windows.Forms.DockStyle.Fill;
            txtDeployDesc.Location = new System.Drawing.Point(103, 246);
            txtDeployDesc.Name = "txtDeployDesc";
            txtDeployDesc.Properties.AutoHeight = false;
            txtDeployDesc.Size = new System.Drawing.Size(1368, 23);
            txtDeployDesc.TabIndex = 19;
            // 
            // lblCat
            // 
            lblCat.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblCat.Dock = System.Windows.Forms.DockStyle.Top;
            lblCat.Location = new System.Drawing.Point(3, 57);
            lblCat.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblCat.Name = "lblCat";
            lblCat.Size = new System.Drawing.Size(94, 21);
            lblCat.TabIndex = 4;
            lblCat.Text = "카테고리:";
            // 
            // lblId
            // 
            lblId.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblId.Dock = System.Windows.Forms.DockStyle.Top;
            lblId.Location = new System.Drawing.Point(3, 3);
            lblId.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblId.Name = "lblId";
            lblId.Size = new System.Drawing.Size(94, 21);
            lblId.TabIndex = 0;
            lblId.Text = "ModuleId:";
            // 
            // dgvDbVersions
            // 
            dgvDbVersions.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvDbVersions.Location = new System.Drawing.Point(0, 40);
            dgvDbVersions.MainView = gvDbVersions;
            dgvDbVersions.Name = "dgvDbVersions";
            dgvDbVersions.Size = new System.Drawing.Size(1474, 1068);
            dgvDbVersions.TabIndex = 0;
            dgvDbVersions.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvDbVersions });
            // 
            // gvDbVersions
            // 
            gvDbVersions.GridControl = dgvDbVersions;
            gvDbVersions.Name = "gvDbVersions";
            gvDbVersions.OptionsBehavior.Editable = false;
            gvDbVersions.OptionsView.ShowGroupPanel = false;
            // 
            // pnlFilter
            // 
            pnlFilter.Controls.Add(cboFilterCategory);
            pnlFilter.Controls.Add(lblFilterCategory);
            pnlFilter.Controls.Add(lblFilterSubSystem);
            pnlFilter.Controls.Add(cboFilterSubSystem);
            pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            pnlFilter.Location = new System.Drawing.Point(0, 0);
            pnlFilter.Name = "pnlFilter";
            pnlFilter.Size = new System.Drawing.Size(1474, 40);
            pnlFilter.TabIndex = 1;
            // 
            // cboFilterCategory
            // 
            cboFilterCategory.Location = new System.Drawing.Point(79, 8);
            cboFilterCategory.Name = "cboFilterCategory";
            cboFilterCategory.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cboFilterCategory.Size = new System.Drawing.Size(120, 28);
            cboFilterCategory.TabIndex = 1;
            cboFilterCategory.SelectedIndexChanged += CboFilter_Changed;
            // 
            // lblFilterCategory
            // 
            lblFilterCategory.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblFilterCategory.Location = new System.Drawing.Point(3, 6);
            lblFilterCategory.Name = "lblFilterCategory";
            lblFilterCategory.Size = new System.Drawing.Size(70, 23);
            lblFilterCategory.TabIndex = 0;
            lblFilterCategory.Text = "System:";
            // 
            // lblFilterSubSystem
            // 
            lblFilterSubSystem.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblFilterSubSystem.Location = new System.Drawing.Point(205, 5);
            lblFilterSubSystem.Name = "lblFilterSubSystem";
            lblFilterSubSystem.Size = new System.Drawing.Size(44, 23);
            lblFilterSubSystem.TabIndex = 2;
            lblFilterSubSystem.Text = "Sub:";
            // 
            // cboFilterSubSystem
            // 
            cboFilterSubSystem.Location = new System.Drawing.Point(255, 8);
            cboFilterSubSystem.Name = "cboFilterSubSystem";
            cboFilterSubSystem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cboFilterSubSystem.Size = new System.Drawing.Size(120, 28);
            cboFilterSubSystem.TabIndex = 3;
            cboFilterSubSystem.SelectedIndexChanged += CboFilter_Changed;
            // 
            // pnlEdit
            // 
            pnlEdit.Controls.Add(tableLayoutPanel1);
            pnlEdit.Dock = System.Windows.Forms.DockStyle.Bottom;
            pnlEdit.Location = new System.Drawing.Point(0, 1108);
            pnlEdit.MinimumSize = new System.Drawing.Size(250, 0);
            pnlEdit.Name = "pnlEdit";
            pnlEdit.Padding = new System.Windows.Forms.Padding(1);
            pnlEdit.Size = new System.Drawing.Size(1474, 99);
            pnlEdit.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel1.Controls.Add(btnRefresh, 0, 0);
            tableLayoutPanel1.Controls.Add(btnDelete, 2, 0);
            tableLayoutPanel1.Controls.Add(btnSmartUpload, 1, 0);
            tableLayoutPanel1.Controls.Add(btnAdd, 2, 1);
            tableLayoutPanel1.Controls.Add(btnBulkUpload, 1, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new System.Drawing.Size(1468, 93);
            tableLayoutPanel1.TabIndex = 5;
            // 
            // btnRefresh
            // 
            btnRefresh.AuthId = "";
            btnRefresh.Dock = System.Windows.Forms.DockStyle.Fill;
            btnRefresh.Location = new System.Drawing.Point(3, 3);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new System.Drawing.Size(483, 40);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "새로고침";
            btnRefresh.Click += BtnRefresh_Click;
            // 
            // btnDelete
            // 
            btnDelete.AuthId = "";
            btnDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            btnDelete.Location = new System.Drawing.Point(981, 3);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new System.Drawing.Size(484, 40);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "삭제";
            btnDelete.Click += BtnDelete_Click;
            // 
            // btnSmartUpload
            // 
            btnSmartUpload.AuthId = "";
            btnSmartUpload.Dock = System.Windows.Forms.DockStyle.Fill;
            btnSmartUpload.Location = new System.Drawing.Point(492, 3);
            btnSmartUpload.Name = "btnSmartUpload";
            btnSmartUpload.Size = new System.Drawing.Size(483, 40);
            btnSmartUpload.TabIndex = 1;
            btnSmartUpload.Text = "스마트 업로드";
            btnSmartUpload.Click += BtnSmartUpload_Click;
            // 
            // btnAdd
            // 
            btnAdd.AuthId = "";
            btnAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            btnAdd.Location = new System.Drawing.Point(981, 49);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new System.Drawing.Size(484, 41);
            btnAdd.TabIndex = 4;
            btnAdd.Text = "저장";
            btnAdd.Click += BtnAdd_Click;
            // 
            // btnBulkUpload
            // 
            btnBulkUpload.AuthId = "";
            btnBulkUpload.Dock = System.Windows.Forms.DockStyle.Fill;
            btnBulkUpload.Location = new System.Drawing.Point(492, 49);
            btnBulkUpload.Name = "btnBulkUpload";
            btnBulkUpload.Size = new System.Drawing.Size(483, 41);
            btnBulkUpload.TabIndex = 5;
            btnBulkUpload.Text = "일괄 업로드";
            btnBulkUpload.Click += btnBulkUpload_Click;
            // 
            // ProgramDeployManagementControl
            // 
            Controls.Add(splitMain);
            Name = "ProgramDeployManagementControl";
            Size = new System.Drawing.Size(2214, 1207);
            ((System.ComponentModel.ISupportInitialize)splitMain.Panel1).EndInit();
            splitMain.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain.Panel2).EndInit();
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvModuleFiles).EndInit();
            ((System.ComponentModel.ISupportInitialize)gvModuleFiles).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlScanTop).EndInit();
            pnlScanTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)txtModulesRoot.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)chkUpdated.Properties).EndInit();
            tblEdit.ResumeLayout(false);
            tblEdit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)txtModuleId.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtModuleName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cboCategory.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtSubSystem.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtFileName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtActiveVersion.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtFileHash.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtFileSize.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtStoragePath.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtDeployDesc.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvDbVersions).EndInit();
            ((System.ComponentModel.ISupportInitialize)gvDbVersions).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlFilter).EndInit();
            pnlFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)cboFilterCategory.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cboFilterSubSystem.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlEdit).EndInit();
            pnlEdit.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);

        }

        private nU3.Core.UI.Controls.nU3CheckEdit chkUpdated;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private nU3.Core.UI.Controls.nU3SimpleButton btnBulkUpload;
    }
}
