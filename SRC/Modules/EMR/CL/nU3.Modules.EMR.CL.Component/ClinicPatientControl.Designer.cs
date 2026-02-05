namespace nU3.Modules.EMR.CL.Component
{
    partial class ClinicPatientControl
    {
        private System.ComponentModel.IContainer _components = null;

        private DevExpress.XtraLayout.LayoutControl _layoutControl = null!;
        private nU3.Core.UI.Components.Controls.PatientSelectorControl _patientSelector = null!;
        private DevExpress.XtraEditors.SimpleButton _btnClear = null!;
        private DevExpress.XtraEditors.LabelControl _lblSelectedPatient = null!;

        private DevExpress.XtraLayout.LayoutControlGroup _rootGroup = null!;
        private DevExpress.XtraLayout.LayoutControlGroup _patientGroup = null!;
        private DevExpress.XtraLayout.LayoutControlItem _patientControlItem = null!;
        private DevExpress.XtraLayout.LayoutControlGroup _infoGroup = null!;
        private DevExpress.XtraLayout.LayoutControlItem _infoItem = null!;
        private DevExpress.XtraLayout.LayoutControlGroup _buttonGroup = null!;
        private DevExpress.XtraLayout.LayoutControlItem _buttonItem = null!;

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
            _patientSelector = new nU3.Core.UI.Components.Controls.PatientSelectorControl();
            _lblSelectedPatient = new DevExpress.XtraEditors.LabelControl();
            _btnClear = new DevExpress.XtraEditors.SimpleButton();
            _rootGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            _patientGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            _patientControlItem = new DevExpress.XtraLayout.LayoutControlItem();
            _infoGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            _infoItem = new DevExpress.XtraLayout.LayoutControlItem();
            _buttonGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            _buttonItem = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)_layoutControl).BeginInit();
            _layoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_rootGroup).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_patientGroup).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_patientControlItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_infoGroup).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_infoItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_buttonGroup).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_buttonItem).BeginInit();
            SuspendLayout();
            // 
            // _layoutControl
            // 
            _layoutControl.Controls.Add(_patientSelector);
            _layoutControl.Controls.Add(_lblSelectedPatient);
            _layoutControl.Controls.Add(_btnClear);
            _layoutControl.Dock = DockStyle.Fill;
            _layoutControl.Location = new Point(0, 0);
            _layoutControl.Name = "_layoutControl";
            _layoutControl.Root = _rootGroup;
            _layoutControl.Size = new Size(1528, 793);
            _layoutControl.TabIndex = 0;
            _layoutControl.Text = "layoutControl1";
            // 
            // _patientSelector
            // 
            _patientSelector.AutoSearch = true;
            _patientSelector.Location = new Point(24, 43);
            _patientSelector.Name = "_patientSelector";
            _patientSelector.SearchLimit = 100;
            _patientSelector.Size = new Size(1480, 594);
            _patientSelector.TabIndex = 4;
            _patientSelector.PatientSelected += OnPatientSelected;
            // 
            // _lblSelectedPatient
            // 
            _lblSelectedPatient.Appearance.FontSizeDelta = 2;
            _lblSelectedPatient.Appearance.ForeColor = Color.Gray;
            _lblSelectedPatient.Appearance.Options.UseFont = true;
            _lblSelectedPatient.Appearance.Options.UseForeColor = true;
            _lblSelectedPatient.Location = new Point(24, 684);
            _lblSelectedPatient.Name = "_lblSelectedPatient";
            _lblSelectedPatient.Size = new Size(106, 18);
            _lblSelectedPatient.StyleController = _layoutControl;
            _lblSelectedPatient.TabIndex = 5;
            _lblSelectedPatient.Text = "선택된 환자: 없음";
            // 
            // _btnClear
            // 
            _btnClear.Location = new Point(24, 749);
            _btnClear.Name = "_btnClear";
            _btnClear.Size = new Size(1480, 22);
            _btnClear.StyleController = _layoutControl;
            _btnClear.TabIndex = 6;
            _btnClear.Text = "선택 초기화";
            _btnClear.Click += OnClear;
            // 
            // _rootGroup
            // 
            _rootGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            _rootGroup.GroupBordersVisible = false;
            _rootGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { _patientGroup, _infoGroup, _buttonGroup });
            _rootGroup.Name = "_rootGroup";
            _rootGroup.Size = new Size(1528, 793);
            _rootGroup.TextVisible = false;
            // 
            // _patientGroup
            // 
            _patientGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { _patientControlItem });
            _patientGroup.Location = new Point(0, 0);
            _patientGroup.Name = "_patientGroup";
            _patientGroup.Size = new Size(1508, 641);
            _patientGroup.Text = "환자 검색";
            // 
            // _patientControlItem
            // 
            _patientControlItem.Control = _patientSelector;
            _patientControlItem.Location = new Point(0, 0);
            _patientControlItem.Name = "_patientControlItem";
            _patientControlItem.Size = new Size(1484, 598);
            _patientControlItem.TextSize = new Size(0, 0);
            _patientControlItem.TextVisible = false;
            // 
            // _infoGroup
            // 
            _infoGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { _infoItem });
            _infoGroup.Location = new Point(0, 641);
            _infoGroup.Name = "_infoGroup";
            _infoGroup.Size = new Size(1508, 65);
            _infoGroup.Text = "선택된 환자 정보";
            // 
            // _infoItem
            // 
            _infoItem.Control = _lblSelectedPatient;
            _infoItem.Location = new Point(0, 0);
            _infoItem.Name = "_infoItem";
            _infoItem.Size = new Size(1484, 22);
            _infoItem.TextSize = new Size(0, 0);
            _infoItem.TextVisible = false;
            // 
            // _buttonGroup
            // 
            _buttonGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { _buttonItem });
            _buttonGroup.Location = new Point(0, 706);
            _buttonGroup.Name = "_buttonGroup";
            _buttonGroup.Size = new Size(1508, 69);
            _buttonGroup.Text = "동작";
            // 
            // _buttonItem
            // 
            _buttonItem.Control = _btnClear;
            _buttonItem.Location = new Point(0, 0);
            _buttonItem.Name = "_buttonItem";
            _buttonItem.Size = new Size(1484, 26);
            _buttonItem.TextSize = new Size(0, 0);
            _buttonItem.TextVisible = false;
            // 
            // ClinicPatientControl
            // 
            AutoScaleDimensions = new SizeF(7F, 14F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_layoutControl);
            Name = "ClinicPatientControl";
            Size = new Size(1528, 793);
            ((System.ComponentModel.ISupportInitialize)_layoutControl).EndInit();
            _layoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_rootGroup).EndInit();
            ((System.ComponentModel.ISupportInitialize)_patientGroup).EndInit();
            ((System.ComponentModel.ISupportInitialize)_patientControlItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)_infoGroup).EndInit();
            ((System.ComponentModel.ISupportInitialize)_infoItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)_buttonGroup).EndInit();
            ((System.ComponentModel.ISupportInitialize)_buttonItem).EndInit();
            ResumeLayout(false);

        }

        #endregion
    }
}
