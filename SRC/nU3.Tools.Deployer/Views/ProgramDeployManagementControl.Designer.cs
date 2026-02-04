namespace nU3.Tools.Deployer.Views
{
    partial class ProgramDeployManagementControl
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.SplitContainer splitMain;

        // Left (scan)
        private System.Windows.Forms.Panel pnlScanTop;
        private System.Windows.Forms.Label lblModulesRoot;
        private System.Windows.Forms.TextBox txtModulesRoot;
        private System.Windows.Forms.Button btnBrowseModulesRoot;
        private System.Windows.Forms.Button btnScanModules;
        private System.Windows.Forms.DataGridView dgvModuleFiles;
        private System.Windows.Forms.DataGridView dgvDbVersions;

        // Center filter (retained)
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Label lblFilterCategory;
        private System.Windows.Forms.ComboBox cboFilterCategory;
        private System.Windows.Forms.Label lblFilterSubSystem;
        private System.Windows.Forms.ComboBox cboFilterSubSystem;

        // Right (edit - base fields retained)
        private System.Windows.Forms.Panel pnlEdit;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.TextBox txtModuleId;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtModuleName;
        private System.Windows.Forms.Label lblCat;
        private System.Windows.Forms.ComboBox cboCategory;
        private System.Windows.Forms.Label lblSub;
        private System.Windows.Forms.TextBox txtSubSystem;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnSmartUpload;

        // Right (edit)
        private System.Windows.Forms.TableLayoutPanel tblEdit;
        private System.Windows.Forms.Label lblVer;
        private System.Windows.Forms.TextBox txtActiveVersion;
        private System.Windows.Forms.Label lblHash;
        private System.Windows.Forms.TextBox txtFileHash;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.TextBox txtFileSize;
        private System.Windows.Forms.Label lblStorage;
        private System.Windows.Forms.TextBox txtStoragePath;
        private System.Windows.Forms.Label lblDeployDesc;
        private System.Windows.Forms.TextBox txtDeployDesc;

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
            splitMain = new System.Windows.Forms.SplitContainer();
            dgvModuleFiles = new System.Windows.Forms.DataGridView();
            pnlScanTop = new System.Windows.Forms.Panel();
            txtModulesRoot = new System.Windows.Forms.TextBox();
            btnBrowseModulesRoot = new System.Windows.Forms.Button();
            btnScanModules = new System.Windows.Forms.Button();
            chkUpdated = new System.Windows.Forms.CheckBox();
            lblModulesRoot = new System.Windows.Forms.Label();
            tblEdit = new System.Windows.Forms.TableLayoutPanel();
            txtModuleId = new System.Windows.Forms.TextBox();
            lblName = new System.Windows.Forms.Label();
            txtModuleName = new System.Windows.Forms.TextBox();
            cboCategory = new System.Windows.Forms.ComboBox();
            lblSub = new System.Windows.Forms.Label();
            txtSubSystem = new System.Windows.Forms.TextBox();
            lblFile = new System.Windows.Forms.Label();
            txtFileName = new System.Windows.Forms.TextBox();
            lblVer = new System.Windows.Forms.Label();
            txtActiveVersion = new System.Windows.Forms.TextBox();
            lblHash = new System.Windows.Forms.Label();
            txtFileHash = new System.Windows.Forms.TextBox();
            lblSize = new System.Windows.Forms.Label();
            txtFileSize = new System.Windows.Forms.TextBox();
            lblStorage = new System.Windows.Forms.Label();
            txtStoragePath = new System.Windows.Forms.TextBox();
            lblDeployDesc = new System.Windows.Forms.Label();
            txtDeployDesc = new System.Windows.Forms.TextBox();
            lblCat = new System.Windows.Forms.Label();
            lblId = new System.Windows.Forms.Label();
            dgvDbVersions = new System.Windows.Forms.DataGridView();
            pnlFilter = new System.Windows.Forms.Panel();
            cboFilterCategory = new System.Windows.Forms.ComboBox();
            lblFilterCategory = new System.Windows.Forms.Label();
            lblFilterSubSystem = new System.Windows.Forms.Label();
            cboFilterSubSystem = new System.Windows.Forms.ComboBox();
            pnlEdit = new System.Windows.Forms.Panel();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            btnRefresh = new System.Windows.Forms.Button();
            btnDelete = new System.Windows.Forms.Button();
            btnSmartUpload = new System.Windows.Forms.Button();
            btnAdd = new System.Windows.Forms.Button();
            btnBulkUpload = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvModuleFiles).BeginInit();
            pnlScanTop.SuspendLayout();
            tblEdit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDbVersions).BeginInit();
            pnlFilter.SuspendLayout();
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
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(tblEdit);
            splitMain.Panel2.Controls.Add(dgvDbVersions);
            splitMain.Panel2.Controls.Add(pnlFilter);
            splitMain.Panel2.Controls.Add(pnlEdit);
            splitMain.Size = new System.Drawing.Size(1200, 700);
            splitMain.SplitterDistance = 725;
            splitMain.SplitterWidth = 6;
            splitMain.TabIndex = 0;
            // 
            // dgvModuleFiles
            // 
            dgvModuleFiles.AllowUserToAddRows = false;
            dgvModuleFiles.AllowUserToDeleteRows = false;
            dgvModuleFiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dgvModuleFiles.ColumnHeadersHeight = 34;
            dgvModuleFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvModuleFiles.Location = new System.Drawing.Point(0, 40);
            dgvModuleFiles.MultiSelect = false;
            dgvModuleFiles.Name = "dgvModuleFiles";
            dgvModuleFiles.ReadOnly = true;
            dgvModuleFiles.RowHeadersWidth = 62;
            dgvModuleFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvModuleFiles.Size = new System.Drawing.Size(725, 660);
            dgvModuleFiles.TabIndex = 0;
            // 
            // pnlScanTop
            // 
            pnlScanTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
            txtModulesRoot.Location = new System.Drawing.Point(90, 0);
            txtModulesRoot.Multiline = true;
            txtModulesRoot.Name = "txtModulesRoot";
            txtModulesRoot.ReadOnly = true;
            txtModulesRoot.Size = new System.Drawing.Size(357, 38);
            txtModulesRoot.TabIndex = 1;
            // 
            // btnBrowseModulesRoot
            // 
            btnBrowseModulesRoot.Dock = System.Windows.Forms.DockStyle.Right;
            btnBrowseModulesRoot.Location = new System.Drawing.Point(447, 0);
            btnBrowseModulesRoot.Margin = new System.Windows.Forms.Padding(7);
            btnBrowseModulesRoot.Name = "btnBrowseModulesRoot";
            btnBrowseModulesRoot.Size = new System.Drawing.Size(90, 38);
            btnBrowseModulesRoot.TabIndex = 2;
            btnBrowseModulesRoot.Text = "Browse";
            btnBrowseModulesRoot.Click += BtnBrowseModulesRoot_Click;
            // 
            // btnScanModules
            // 
            btnScanModules.Dock = System.Windows.Forms.DockStyle.Right;
            btnScanModules.Location = new System.Drawing.Point(537, 0);
            btnScanModules.Margin = new System.Windows.Forms.Padding(7);
            btnScanModules.Name = "btnScanModules";
            btnScanModules.Size = new System.Drawing.Size(90, 38);
            btnScanModules.TabIndex = 3;
            btnScanModules.Text = "Scan";
            btnScanModules.Click += BtnScanModules_Click;
            // 
            // chkUpdated
            // 
            chkUpdated.AutoSize = true;
            chkUpdated.Dock = System.Windows.Forms.DockStyle.Right;
            chkUpdated.Location = new System.Drawing.Point(627, 0);
            chkUpdated.Margin = new System.Windows.Forms.Padding(3, 3, 13, 3);
            chkUpdated.Name = "chkUpdated";
            chkUpdated.Size = new System.Drawing.Size(96, 38);
            chkUpdated.TabIndex = 4;
            chkUpdated.Text = "Need Update";
            chkUpdated.UseVisualStyleBackColor = true;
            // 
            // lblModulesRoot
            // 
            lblModulesRoot.Dock = System.Windows.Forms.DockStyle.Left;
            lblModulesRoot.Location = new System.Drawing.Point(0, 0);
            lblModulesRoot.Name = "lblModulesRoot";
            lblModulesRoot.Size = new System.Drawing.Size(90, 38);
            lblModulesRoot.TabIndex = 0;
            lblModulesRoot.Text = "Modules:";
            lblModulesRoot.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            tblEdit.Location = new System.Drawing.Point(0, 329);
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
            tblEdit.Size = new System.Drawing.Size(469, 272);
            tblEdit.TabIndex = 0;
            // 
            // txtModuleId
            // 
            txtModuleId.Dock = System.Windows.Forms.DockStyle.Fill;
            txtModuleId.Location = new System.Drawing.Point(103, 3);
            txtModuleId.Name = "txtModuleId";
            txtModuleId.Size = new System.Drawing.Size(363, 23);
            txtModuleId.TabIndex = 1;
            // 
            // lblName
            // 
            lblName.Dock = System.Windows.Forms.DockStyle.Top;
            lblName.Location = new System.Drawing.Point(3, 30);
            lblName.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblName.Name = "lblName";
            lblName.Size = new System.Drawing.Size(94, 24);
            lblName.TabIndex = 2;
            lblName.Text = "모듈명:";
            lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtModuleName
            // 
            txtModuleName.Dock = System.Windows.Forms.DockStyle.Fill;
            txtModuleName.Location = new System.Drawing.Point(103, 30);
            txtModuleName.Name = "txtModuleName";
            txtModuleName.Size = new System.Drawing.Size(363, 23);
            txtModuleName.TabIndex = 3;
            // 
            // cboCategory
            // 
            cboCategory.Dock = System.Windows.Forms.DockStyle.Fill;
            cboCategory.Location = new System.Drawing.Point(103, 57);
            cboCategory.Name = "cboCategory";
            cboCategory.Size = new System.Drawing.Size(363, 23);
            cboCategory.TabIndex = 5;
            // 
            // lblSub
            // 
            lblSub.Location = new System.Drawing.Point(3, 84);
            lblSub.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblSub.Name = "lblSub";
            lblSub.Size = new System.Drawing.Size(90, 24);
            lblSub.TabIndex = 6;
            lblSub.Text = "서브시스템:";
            lblSub.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtSubSystem
            // 
            txtSubSystem.Dock = System.Windows.Forms.DockStyle.Fill;
            txtSubSystem.Location = new System.Drawing.Point(103, 84);
            txtSubSystem.Name = "txtSubSystem";
            txtSubSystem.Size = new System.Drawing.Size(363, 23);
            txtSubSystem.TabIndex = 7;
            // 
            // lblFile
            // 
            lblFile.Location = new System.Drawing.Point(3, 111);
            lblFile.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblFile.Name = "lblFile";
            lblFile.Size = new System.Drawing.Size(90, 24);
            lblFile.TabIndex = 8;
            lblFile.Text = "파일명:";
            lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtFileName
            // 
            txtFileName.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFileName.Location = new System.Drawing.Point(103, 111);
            txtFileName.Name = "txtFileName";
            txtFileName.Size = new System.Drawing.Size(363, 23);
            txtFileName.TabIndex = 9;
            // 
            // lblVer
            // 
            lblVer.Location = new System.Drawing.Point(3, 138);
            lblVer.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblVer.Name = "lblVer";
            lblVer.Size = new System.Drawing.Size(90, 24);
            lblVer.TabIndex = 10;
            lblVer.Text = "버전:";
            lblVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtActiveVersion
            // 
            txtActiveVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            txtActiveVersion.Location = new System.Drawing.Point(103, 138);
            txtActiveVersion.Name = "txtActiveVersion";
            txtActiveVersion.Size = new System.Drawing.Size(363, 23);
            txtActiveVersion.TabIndex = 11;
            // 
            // lblHash
            // 
            lblHash.Location = new System.Drawing.Point(3, 165);
            lblHash.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblHash.Name = "lblHash";
            lblHash.Size = new System.Drawing.Size(90, 24);
            lblHash.TabIndex = 12;
            lblHash.Text = "파일해시:";
            lblHash.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtFileHash
            // 
            txtFileHash.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFileHash.Location = new System.Drawing.Point(103, 165);
            txtFileHash.Name = "txtFileHash";
            txtFileHash.Size = new System.Drawing.Size(363, 23);
            txtFileHash.TabIndex = 13;
            // 
            // lblSize
            // 
            lblSize.Location = new System.Drawing.Point(3, 192);
            lblSize.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblSize.Name = "lblSize";
            lblSize.Size = new System.Drawing.Size(90, 24);
            lblSize.TabIndex = 14;
            lblSize.Text = "파일크기:";
            lblSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtFileSize
            // 
            txtFileSize.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFileSize.Location = new System.Drawing.Point(103, 192);
            txtFileSize.Name = "txtFileSize";
            txtFileSize.Size = new System.Drawing.Size(363, 23);
            txtFileSize.TabIndex = 15;
            // 
            // lblStorage
            // 
            lblStorage.Location = new System.Drawing.Point(3, 219);
            lblStorage.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblStorage.Name = "lblStorage";
            lblStorage.Size = new System.Drawing.Size(90, 24);
            lblStorage.TabIndex = 16;
            lblStorage.Text = "저장경로:";
            lblStorage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtStoragePath
            // 
            txtStoragePath.Dock = System.Windows.Forms.DockStyle.Fill;
            txtStoragePath.Location = new System.Drawing.Point(103, 219);
            txtStoragePath.Name = "txtStoragePath";
            txtStoragePath.Size = new System.Drawing.Size(363, 23);
            txtStoragePath.TabIndex = 17;
            // 
            // lblDeployDesc
            // 
            lblDeployDesc.Location = new System.Drawing.Point(3, 246);
            lblDeployDesc.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblDeployDesc.Name = "lblDeployDesc";
            lblDeployDesc.Size = new System.Drawing.Size(90, 25);
            lblDeployDesc.TabIndex = 18;
            lblDeployDesc.Text = "배포설명:";
            lblDeployDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtDeployDesc
            // 
            txtDeployDesc.Dock = System.Windows.Forms.DockStyle.Fill;
            txtDeployDesc.Location = new System.Drawing.Point(103, 246);
            txtDeployDesc.Name = "txtDeployDesc";
            txtDeployDesc.Size = new System.Drawing.Size(363, 23);
            txtDeployDesc.TabIndex = 19;
            // 
            // lblCat
            // 
            lblCat.Dock = System.Windows.Forms.DockStyle.Top;
            lblCat.Location = new System.Drawing.Point(3, 57);
            lblCat.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblCat.Name = "lblCat";
            lblCat.Size = new System.Drawing.Size(94, 24);
            lblCat.TabIndex = 4;
            lblCat.Text = "카테고리:";
            lblCat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblId
            // 
            lblId.Dock = System.Windows.Forms.DockStyle.Top;
            lblId.Location = new System.Drawing.Point(3, 3);
            lblId.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            lblId.Name = "lblId";
            lblId.Size = new System.Drawing.Size(94, 24);
            lblId.TabIndex = 0;
            lblId.Text = "ModuleId:";
            lblId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvDbVersions
            // 
            dgvDbVersions.AllowUserToAddRows = false;
            dgvDbVersions.AllowUserToDeleteRows = false;
            dgvDbVersions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dgvDbVersions.ColumnHeadersHeight = 34;
            dgvDbVersions.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvDbVersions.Location = new System.Drawing.Point(0, 40);
            dgvDbVersions.MultiSelect = false;
            dgvDbVersions.Name = "dgvDbVersions";
            dgvDbVersions.RowHeadersWidth = 62;
            dgvDbVersions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvDbVersions.Size = new System.Drawing.Size(469, 561);
            dgvDbVersions.TabIndex = 0;
            // 
            // pnlFilter
            // 
            pnlFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnlFilter.Controls.Add(cboFilterCategory);
            pnlFilter.Controls.Add(lblFilterCategory);
            pnlFilter.Controls.Add(lblFilterSubSystem);
            pnlFilter.Controls.Add(cboFilterSubSystem);
            pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            pnlFilter.Location = new System.Drawing.Point(0, 0);
            pnlFilter.Name = "pnlFilter";
            pnlFilter.Size = new System.Drawing.Size(469, 40);
            pnlFilter.TabIndex = 1;
            // 
            // cboFilterCategory
            // 
            cboFilterCategory.DropDownHeight = 120;
            cboFilterCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboFilterCategory.IntegralHeight = false;
            cboFilterCategory.ItemHeight = 15;
            cboFilterCategory.Location = new System.Drawing.Point(79, 6);
            cboFilterCategory.Name = "cboFilterCategory";
            cboFilterCategory.Size = new System.Drawing.Size(120, 23);
            cboFilterCategory.TabIndex = 1;
            cboFilterCategory.SelectedIndexChanged += CboFilter_Changed;
            // 
            // lblFilterCategory
            // 
            lblFilterCategory.Location = new System.Drawing.Point(3, 6);
            lblFilterCategory.Name = "lblFilterCategory";
            lblFilterCategory.Size = new System.Drawing.Size(70, 23);
            lblFilterCategory.TabIndex = 0;
            lblFilterCategory.Text = "System:";
            lblFilterCategory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFilterSubSystem
            // 
            lblFilterSubSystem.Location = new System.Drawing.Point(205, 5);
            lblFilterSubSystem.Name = "lblFilterSubSystem";
            lblFilterSubSystem.Size = new System.Drawing.Size(44, 23);
            lblFilterSubSystem.TabIndex = 2;
            lblFilterSubSystem.Text = "Sub:";
            lblFilterSubSystem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboFilterSubSystem
            // 
            cboFilterSubSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboFilterSubSystem.Location = new System.Drawing.Point(255, 6);
            cboFilterSubSystem.Name = "cboFilterSubSystem";
            cboFilterSubSystem.Size = new System.Drawing.Size(120, 23);
            cboFilterSubSystem.TabIndex = 3;
            cboFilterSubSystem.SelectedIndexChanged += CboFilter_Changed;
            // 
            // pnlEdit
            // 
            pnlEdit.AutoScroll = true;
            pnlEdit.BackColor = System.Drawing.SystemColors.AppWorkspace;
            pnlEdit.Controls.Add(tableLayoutPanel1);
            pnlEdit.Dock = System.Windows.Forms.DockStyle.Bottom;
            pnlEdit.Location = new System.Drawing.Point(0, 601);
            pnlEdit.MinimumSize = new System.Drawing.Size(250, 0);
            pnlEdit.Name = "pnlEdit";
            pnlEdit.Padding = new System.Windows.Forms.Padding(1);
            pnlEdit.Size = new System.Drawing.Size(469, 99);
            pnlEdit.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Controls.Add(btnRefresh, 0, 0);
            tableLayoutPanel1.Controls.Add(btnDelete, 2, 0);
            tableLayoutPanel1.Controls.Add(btnSmartUpload, 1, 0);
            tableLayoutPanel1.Controls.Add(btnAdd, 2, 1);
            tableLayoutPanel1.Controls.Add(btnBulkUpload, 1, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(1, 1);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(467, 97);
            tableLayoutPanel1.TabIndex = 5;
            // 
            // btnRefresh
            // 
            btnRefresh.Dock = System.Windows.Forms.DockStyle.Fill;
            btnRefresh.Location = new System.Drawing.Point(3, 3);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Padding = new System.Windows.Forms.Padding(3);
            tableLayoutPanel1.SetRowSpan(btnRefresh, 2);
            btnRefresh.Size = new System.Drawing.Size(149, 94);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "새로고침";
            btnRefresh.Click += BtnRefresh_Click;
            // 
            // btnDelete
            // 
            btnDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            btnDelete.Location = new System.Drawing.Point(313, 3);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new System.Drawing.Size(151, 44);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "삭제";
            btnDelete.Click += BtnDelete_Click;
            // 
            // btnSmartUpload
            // 
            btnSmartUpload.Dock = System.Windows.Forms.DockStyle.Fill;
            btnSmartUpload.Location = new System.Drawing.Point(158, 3);
            btnSmartUpload.Name = "btnSmartUpload";
            btnSmartUpload.Size = new System.Drawing.Size(149, 44);
            btnSmartUpload.TabIndex = 1;
            btnSmartUpload.Text = "개별 업로드";
            btnSmartUpload.Click += BtnSmartUpload_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new System.Drawing.Point(3, 103);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new System.Drawing.Size(114, 14);
            btnAdd.TabIndex = 4;
            btnAdd.Text = "저장";
            btnAdd.Click += BtnAdd_Click;
            // 
            // btnBulkUpload
            // 
            tableLayoutPanel1.SetColumnSpan(btnBulkUpload, 2);
            btnBulkUpload.Dock = System.Windows.Forms.DockStyle.Fill;
            btnBulkUpload.Location = new System.Drawing.Point(158, 53);
            btnBulkUpload.Name = "btnBulkUpload";
            btnBulkUpload.Size = new System.Drawing.Size(306, 44);
            btnBulkUpload.TabIndex = 5;
            btnBulkUpload.Text = "일괄업로드";
            btnBulkUpload.Click += btnBulkUpload_Click;
            // 
            // ProgramDeployManagementControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            Controls.Add(splitMain);
            Name = "ProgramDeployManagementControl";
            Size = new System.Drawing.Size(1200, 700);
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvModuleFiles).EndInit();
            pnlScanTop.ResumeLayout(false);
            pnlScanTop.PerformLayout();
            tblEdit.ResumeLayout(false);
            tblEdit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDbVersions).EndInit();
            pnlFilter.ResumeLayout(false);
            pnlEdit.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }
        private System.Windows.Forms.CheckBox chkUpdated;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnBulkUpload;
    }
}
