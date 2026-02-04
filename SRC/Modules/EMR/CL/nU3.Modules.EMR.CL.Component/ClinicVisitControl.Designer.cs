namespace nU3.Modules.EMR.CL.Component
{
    partial class ClinicVisitControl
    {
        private System.ComponentModel.IContainer _components = null;

        private DevExpress.XtraLayout.LayoutControl _layoutControl = null!;
        private nU3.Core.UI.Components.Controls.DateRangeControl _dateRangeControl = null!;
        private nU3.Core.UI.Components.Controls.ChecklistControl _checklistControl = null!;
        private DevExpress.XtraEditors.TextEdit _txtChiefComplaint = null!;
        private DevExpress.XtraEditors.MemoEdit _txtNotes = null!;
        private DevExpress.XtraEditors.SimpleButton _btnSave = null!;
        private DevExpress.XtraEditors.SimpleButton _btnLoad = null!;
        private DevExpress.XtraEditors.LabelControl _lblPatientInfo = null!;

        private DevExpress.XtraLayout.LayoutControlGroup _rootGroup = null!;
        private DevExpress.XtraLayout.LayoutControlGroup _patientGroup = null!;
        private DevExpress.XtraLayout.LayoutControlItem _patientItem = null!;
        private DevExpress.XtraLayout.LayoutControlGroup _dateGroup = null!;
        private DevExpress.XtraLayout.LayoutControlItem _dateItem = null!;
        private DevExpress.XtraLayout.LayoutControlGroup _chiefGroup = null!;
        private DevExpress.XtraLayout.LayoutControlItem _chiefItem = null!;
        private DevExpress.XtraLayout.LayoutControlGroup _checklistGroup = null!;
        private DevExpress.XtraLayout.LayoutControlItem _checklistItem = null!;
        private DevExpress.XtraLayout.LayoutControlGroup _notesGroup = null!;
        private DevExpress.XtraLayout.LayoutControlItem _notesItem = null!;
        private DevExpress.XtraLayout.LayoutControlGroup _buttonGroup = null!;
        private DevExpress.XtraLayout.LayoutControlItem _loadButtonItem = null!;
        private DevExpress.XtraLayout.LayoutControlItem _saveButtonItem = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            _layoutControl = new DevExpress.XtraLayout.LayoutControl();
            _lblPatientInfo = new DevExpress.XtraEditors.LabelControl();
            _dateRangeControl = new nU3.Core.UI.Components.Controls.DateRangeControl();
            _txtChiefComplaint = new DevExpress.XtraEditors.TextEdit();
            _checklistControl = new nU3.Core.UI.Components.Controls.ChecklistControl();
            _txtNotes = new DevExpress.XtraEditors.MemoEdit();
            _btnLoad = new DevExpress.XtraEditors.SimpleButton();
            _btnSave = new DevExpress.XtraEditors.SimpleButton();
            _rootGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            _patientGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            _patientItem = new DevExpress.XtraLayout.LayoutControlItem();
            _dateGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            _dateItem = new DevExpress.XtraLayout.LayoutControlItem();
            _chiefGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            _chiefItem = new DevExpress.XtraLayout.LayoutControlItem();
            _checklistGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            _checklistItem = new DevExpress.XtraLayout.LayoutControlItem();
            _notesGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            _notesItem = new DevExpress.XtraLayout.LayoutControlItem();
            _buttonGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            _loadButtonItem = new DevExpress.XtraLayout.LayoutControlItem();
            _saveButtonItem = new DevExpress.XtraLayout.LayoutControlItem();
            checklistControl1 = new nU3.Core.UI.Components.Controls.ChecklistControl();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)_layoutControl).BeginInit();
            _layoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_txtChiefComplaint.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_txtNotes.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_rootGroup).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_patientGroup).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_patientItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_dateGroup).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_dateItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_chiefGroup).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_chiefItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_checklistGroup).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_checklistItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_notesGroup).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_notesItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_buttonGroup).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_loadButtonItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_saveButtonItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            SuspendLayout();
            // 
            // _layoutControl
            // 
            _layoutControl.Controls.Add(checklistControl1);
            _layoutControl.Controls.Add(_lblPatientInfo);
            _layoutControl.Controls.Add(_dateRangeControl);
            _layoutControl.Controls.Add(_txtChiefComplaint);
            _layoutControl.Controls.Add(_checklistControl);
            _layoutControl.Controls.Add(_txtNotes);
            _layoutControl.Controls.Add(_btnLoad);
            _layoutControl.Controls.Add(_btnSave);
            _layoutControl.Dock = DockStyle.Fill;
            _layoutControl.Location = new Point(0, 0);
            _layoutControl.Name = "_layoutControl";
            _layoutControl.Root = _rootGroup;
            _layoutControl.Size = new Size(1528, 793);
            _layoutControl.TabIndex = 0;
            _layoutControl.Text = "layoutControl1";
            // 
            // _lblPatientInfo
            // 
            _lblPatientInfo.Appearance.FontSizeDelta = 2;
            _lblPatientInfo.Appearance.ForeColor = Color.Gray;
            _lblPatientInfo.Appearance.Options.UseFont = true;
            _lblPatientInfo.Appearance.Options.UseForeColor = true;
            _lblPatientInfo.Location = new Point(24, 43);
            _lblPatientInfo.Name = "_lblPatientInfo";
            _lblPatientInfo.Size = new Size(145, 18);
            _lblPatientInfo.StyleController = _layoutControl;
            _lblPatientInfo.TabIndex = 3;
            _lblPatientInfo.Text = "선택된 환자가 없습니다.";
            // 
            // _dateRangeControl
            // 
            _dateRangeControl.AllowNull = false;
            _dateRangeControl.EndDate = new DateTime(2026, 2, 4, 0, 0, 0, 0);
            _dateRangeControl.Location = new Point(24, 108);
            _dateRangeControl.Margin = new Padding(4, 3, 4, 3);
            _dateRangeControl.Name = "_dateRangeControl";
            _dateRangeControl.ShowQuickButtons = true;
            _dateRangeControl.Size = new Size(1480, 28);
            _dateRangeControl.StartDate = new DateTime(2026, 1, 5, 0, 0, 0, 0);
            _dateRangeControl.TabIndex = 4;
            _dateRangeControl.DateRangeChanged += OnDateRangeChanged;
            // 
            // _txtChiefComplaint
            // 
            _txtChiefComplaint.Location = new Point(24, 183);
            _txtChiefComplaint.Name = "_txtChiefComplaint";
            _txtChiefComplaint.Properties.NullValuePrompt = "주증상을 입력하세요...";
            _txtChiefComplaint.Size = new Size(1480, 20);
            _txtChiefComplaint.StyleController = _layoutControl;
            _txtChiefComplaint.TabIndex = 5;
            // 
            // _checklistControl
            // 
            _checklistControl.ItemSpacing = 5;
            _checklistControl.Location = new Point(24, 250);
            _checklistControl.Name = "_checklistControl";
            _checklistControl.ShowButtons = true;
            _checklistControl.Size = new Size(1480, 246);
            _checklistControl.Style = Core.UI.Components.Controls.ChecklistStyle.VerticalList;
            _checklistControl.TabIndex = 6;
            _checklistControl.Title = null;
            _checklistControl.CheckStateChanged += OnCheckStateChanged;
            // 
            // _txtNotes
            // 
            _txtNotes.Location = new Point(24, 543);
            _txtNotes.Name = "_txtNotes";
            _txtNotes.Properties.NullValuePrompt = "비고 내용을 입력하세요...";
            _txtNotes.Size = new Size(1480, 138);
            _txtNotes.StyleController = _layoutControl;
            _txtNotes.TabIndex = 7;
            // 
            // _btnLoad
            // 
            _btnLoad.Location = new Point(24, 728);
            _btnLoad.Name = "_btnLoad";
            _btnLoad.Size = new Size(738, 22);
            _btnLoad.StyleController = _layoutControl;
            _btnLoad.TabIndex = 8;
            _btnLoad.Text = "진료 기록 조회";
            _btnLoad.Click += OnLoadVisitRecords;
            // 
            // _btnSave
            // 
            _btnSave.Location = new Point(766, 728);
            _btnSave.Name = "_btnSave";
            _btnSave.Size = new Size(738, 22);
            _btnSave.StyleController = _layoutControl;
            _btnSave.TabIndex = 9;
            _btnSave.Text = "진료 기록 저장";
            _btnSave.Click += OnSaveVisitRecord;
            // 
            // _rootGroup
            // 
            _rootGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            _rootGroup.GroupBordersVisible = false;
            _rootGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { _patientGroup, _dateGroup, _chiefGroup, _checklistGroup, _notesGroup, _buttonGroup, layoutControlItem1 });
            _rootGroup.Name = "_rootGroup";
            _rootGroup.Size = new Size(1528, 793);
            _rootGroup.TextVisible = false;
            // 
            // _patientGroup
            // 
            _patientGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { _patientItem });
            _patientGroup.Location = new Point(0, 0);
            _patientGroup.Name = "_patientGroup";
            _patientGroup.Size = new Size(1508, 65);
            _patientGroup.Text = "현재 환자";
            // 
            // _patientItem
            // 
            _patientItem.Control = _lblPatientInfo;
            _patientItem.Location = new Point(0, 0);
            _patientItem.Name = "_patientItem";
            _patientItem.Size = new Size(1484, 22);
            _patientItem.TextSize = new Size(0, 0);
            _patientItem.TextVisible = false;
            // 
            // _dateGroup
            // 
            _dateGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { _dateItem });
            _dateGroup.Location = new Point(0, 65);
            _dateGroup.Name = "_dateGroup";
            _dateGroup.Size = new Size(1508, 75);
            _dateGroup.Text = "진료 기간";
            // 
            // _dateItem
            // 
            _dateItem.Control = _dateRangeControl;
            _dateItem.Location = new Point(0, 0);
            _dateItem.Name = "_dateItem";
            _dateItem.Size = new Size(1484, 32);
            _dateItem.TextSize = new Size(0, 0);
            _dateItem.TextVisible = false;
            // 
            // _chiefGroup
            // 
            _chiefGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { _chiefItem });
            _chiefGroup.Location = new Point(0, 140);
            _chiefGroup.Name = "_chiefGroup";
            _chiefGroup.Size = new Size(1508, 67);
            _chiefGroup.Text = "주증상";
            // 
            // _chiefItem
            // 
            _chiefItem.Control = _txtChiefComplaint;
            _chiefItem.Location = new Point(0, 0);
            _chiefItem.Name = "_chiefItem";
            _chiefItem.Size = new Size(1484, 24);
            _chiefItem.TextSize = new Size(0, 0);
            _chiefItem.TextVisible = false;
            // 
            // _checklistGroup
            // 
            _checklistGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { _checklistItem });
            _checklistGroup.Location = new Point(0, 207);
            _checklistGroup.Name = "_checklistGroup";
            _checklistGroup.Size = new Size(1508, 293);
            _checklistGroup.Text = "증상 체크리스트";
            // 
            // _checklistItem
            // 
            _checklistItem.Control = _checklistControl;
            _checklistItem.Location = new Point(0, 0);
            _checklistItem.MinSize = new Size(1, 187);
            _checklistItem.Name = "_checklistItem";
            _checklistItem.Size = new Size(1484, 250);
            _checklistItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            _checklistItem.TextSize = new Size(0, 0);
            _checklistItem.TextVisible = false;
            // 
            // _notesGroup
            // 
            _notesGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { _notesItem });
            _notesGroup.Location = new Point(0, 500);
            _notesGroup.Name = "_notesGroup";
            _notesGroup.Size = new Size(1508, 185);
            _notesGroup.Text = "비고";
            // 
            // _notesItem
            // 
            _notesItem.Control = _txtNotes;
            _notesItem.Location = new Point(0, 0);
            _notesItem.Name = "_notesItem";
            _notesItem.Size = new Size(1484, 142);
            _notesItem.TextSize = new Size(0, 0);
            _notesItem.TextVisible = false;
            // 
            // _buttonGroup
            // 
            _buttonGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { _loadButtonItem, _saveButtonItem });
            _buttonGroup.Location = new Point(0, 685);
            _buttonGroup.Name = "_buttonGroup";
            _buttonGroup.Size = new Size(1508, 69);
            _buttonGroup.Text = "동작";
            // 
            // _loadButtonItem
            // 
            _loadButtonItem.Control = _btnLoad;
            _loadButtonItem.Location = new Point(0, 0);
            _loadButtonItem.Name = "_loadButtonItem";
            _loadButtonItem.Size = new Size(742, 26);
            _loadButtonItem.TextSize = new Size(0, 0);
            _loadButtonItem.TextVisible = false;
            // 
            // _saveButtonItem
            // 
            _saveButtonItem.Control = _btnSave;
            _saveButtonItem.Location = new Point(742, 0);
            _saveButtonItem.Name = "_saveButtonItem";
            _saveButtonItem.Size = new Size(742, 26);
            _saveButtonItem.TextSize = new Size(0, 0);
            _saveButtonItem.TextVisible = false;
            // 
            // checklistControl1
            // 
            checklistControl1.ItemSpacing = 5;
            checklistControl1.Location = new Point(12, 765);
            checklistControl1.Name = "checklistControl1";
            checklistControl1.ShowButtons = true;
            checklistControl1.Size = new Size(1504, 17);
            checklistControl1.Style = Core.UI.Components.Controls.ChecklistStyle.VerticalList;
            checklistControl1.TabIndex = 10;
            checklistControl1.Title = null;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = checklistControl1;
            layoutControlItem1.Location = new Point(0, 754);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(1508, 21);
            layoutControlItem1.TextSize = new Size(0, 0);
            layoutControlItem1.TextVisible = false;
            // 
            // ClinicVisitControl
            // 
            AutoScaleDimensions = new SizeF(7F, 14F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_layoutControl);
            Name = "ClinicVisitControl";
            Size = new Size(1528, 793);
            ((System.ComponentModel.ISupportInitialize)_layoutControl).EndInit();
            _layoutControl.ResumeLayout(false);
            _layoutControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_txtChiefComplaint.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)_txtNotes.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)_rootGroup).EndInit();
            ((System.ComponentModel.ISupportInitialize)_patientGroup).EndInit();
            ((System.ComponentModel.ISupportInitialize)_patientItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)_dateGroup).EndInit();
            ((System.ComponentModel.ISupportInitialize)_dateItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)_chiefGroup).EndInit();
            ((System.ComponentModel.ISupportInitialize)_chiefItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)_checklistGroup).EndInit();
            ((System.ComponentModel.ISupportInitialize)_checklistItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)_notesGroup).EndInit();
            ((System.ComponentModel.ISupportInitialize)_notesItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)_buttonGroup).EndInit();
            ((System.ComponentModel.ISupportInitialize)_loadButtonItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)_saveButtonItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private Core.UI.Components.Controls.ChecklistControl checklistControl1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}
