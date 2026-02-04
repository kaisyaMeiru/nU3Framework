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
            this._components = new System.ComponentModel.Container();
            this._layoutControl = new DevExpress.XtraLayout.LayoutControl();
            this._patientSelector = new nU3.Core.UI.Components.Controls.PatientSelectorControl();
            this._btnClear = new DevExpress.XtraEditors.SimpleButton();
            this._lblSelectedPatient = new DevExpress.XtraEditors.LabelControl();
            this._rootGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            this._patientGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            this._patientControlItem = new DevExpress.XtraLayout.LayoutControlItem();
            this._infoGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            this._infoItem = new DevExpress.XtraLayout.LayoutControlItem();
            this._buttonGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            this._buttonItem = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this._layoutControl)).BeginInit();
            this._layoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._rootGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._patientGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._patientControlItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._infoGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._infoItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._buttonGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._buttonItem)).BeginInit();
            this.SuspendLayout();
            // 
            // _layoutControl
            // 
            this._layoutControl.Controls.Add(this._patientSelector);
            this._layoutControl.Controls.Add(this._lblSelectedPatient);
            this._layoutControl.Controls.Add(this._btnClear);
            this._layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._layoutControl.Location = new System.Drawing.Point(0, 0);
            this._layoutControl.Name = "_layoutControl";
            this._layoutControl.Root = this._rootGroup;
            this._layoutControl.Size = new System.Drawing.Size(1528, 850);
            this._layoutControl.TabIndex = 0;
            this._layoutControl.Text = "layoutControl1";
            // 
            // _patientSelector
            // 
            this._patientSelector.AutoSearch = true;
            this._patientSelector.Location = new System.Drawing.Point(24, 45);
            this._patientSelector.Name = "_patientSelector";
            this._patientSelector.SearchLimit = 100;
            this._patientSelector.Size = new System.Drawing.Size(1480, 300);
            this._patientSelector.TabIndex = 4;
            this._patientSelector.PatientSelected += new System.EventHandler<nU3.Core.UI.Components.Events.PatientSelectedEventArgs>(this.OnPatientSelected);
            // 
            // _btnClear
            // 
            this._btnClear.Location = new System.Drawing.Point(24, 429);
            this._btnClear.Name = "_btnClear";
            this._btnClear.Size = new System.Drawing.Size(120, 22);
            this._btnClear.StyleController = this._layoutControl;
            this._btnClear.TabIndex = 6;
            this._btnClear.Text = "선택 초기화";
            this._btnClear.Click += new System.EventHandler(this.OnClear);
            // 
            // _lblSelectedPatient
            // 
            this._lblSelectedPatient.Appearance.FontSizeDelta = 2;
            this._lblSelectedPatient.Appearance.ForeColor = System.Drawing.Color.Gray;
            this._lblSelectedPatient.Appearance.Options.UseFont = true;
            this._lblSelectedPatient.Appearance.Options.UseForeColor = true;
            this._lblSelectedPatient.Location = new System.Drawing.Point(24, 371);
            this._lblSelectedPatient.Name = "_lblSelectedPatient";
            this._lblSelectedPatient.Size = new System.Drawing.Size(126, 16);
            this._lblSelectedPatient.StyleController = this._layoutControl;
            this._lblSelectedPatient.TabIndex = 5;
            this._lblSelectedPatient.Text = "선택된 환자: 없음";
            // 
            // _rootGroup
            // 
            this._rootGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this._rootGroup.GroupBordersVisible = false;
            this._rootGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this._patientGroup,
            this._infoGroup,
            this._buttonGroup});
            this._rootGroup.Name = "_rootGroup";
            this._rootGroup.Size = new System.Drawing.Size(1528, 850);
            this._rootGroup.TextVisible = false;
            // 
            // _patientGroup
            // 
            this._patientGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this._patientControlItem});
            this._patientGroup.Location = new System.Drawing.Point(0, 0);
            this._patientGroup.Name = "_patientGroup";
            this._patientGroup.Size = new System.Drawing.Size(1508, 349);
            this._patientGroup.Text = "환자 검색";
            // 
            // _patientControlItem
            // 
            this._patientControlItem.Control = this._patientSelector;
            this._patientControlItem.Location = new System.Drawing.Point(0, 0);
            this._patientControlItem.Name = "_patientControlItem";
            this._patientControlItem.Size = new System.Drawing.Size(1484, 304);
            this._patientControlItem.TextSize = new System.Drawing.Size(0, 0);
            this._patientControlItem.TextVisible = false;
            // 
            // _infoGroup
            // 
            this._infoGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this._infoItem});
            this._infoGroup.Location = new System.Drawing.Point(0, 349);
            this._infoGroup.Name = "_infoGroup";
            this._infoGroup.Size = new System.Drawing.Size(1508, 64);
            this._infoGroup.Text = "선택된 환자 정보";
            // 
            // _infoItem
            // 
            this._infoItem.Control = this._lblSelectedPatient;
            this._infoItem.Location = new System.Drawing.Point(0, 0);
            this._infoItem.Name = "_infoItem";
            this._infoItem.Size = new System.Drawing.Size(1484, 20);
            this._infoItem.TextSize = new System.Drawing.Size(0, 0);
            this._infoItem.TextVisible = false;
            // 
            // _buttonGroup
            // 
            this._buttonGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this._buttonItem});
            this._buttonGroup.Location = new System.Drawing.Point(0, 413);
            this._buttonGroup.Name = "_buttonGroup";
            this._buttonGroup.Size = new System.Drawing.Size(1508, 66);
            this._buttonGroup.Text = "동작";
            // 
            // _buttonItem
            // 
            this._buttonItem.Control = this._btnClear;
            this._buttonItem.Location = new System.Drawing.Point(0, 0);
            this._buttonItem.Name = "_buttonItem";
            this._buttonItem.Size = new System.Drawing.Size(1484, 26);
            this._buttonItem.TextSize = new System.Drawing.Size(0, 0);
            this._buttonItem.TextVisible = false;
            // 
            // ClinicPatientControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._layoutControl);
            this.Name = "ClinicPatientControl";
            this.Size = new System.Drawing.Size(1528, 850);
            ((System.ComponentModel.ISupportInitialize)(this._layoutControl)).EndInit();
            this._layoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._rootGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._patientGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._patientControlItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._infoGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._infoItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._buttonGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._buttonItem)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
