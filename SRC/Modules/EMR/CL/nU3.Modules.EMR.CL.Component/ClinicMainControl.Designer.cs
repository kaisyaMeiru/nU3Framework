namespace nU3.Modules.EMR.CL.Component
{
    partial class ClinicMainControl
    {
        private System.ComponentModel.IContainer components = null;

        private DevExpress.XtraTab.XtraTabControl _tabControl = null!;
        private DevExpress.XtraTab.XtraTabPage _tabPatient = null!;
        private DevExpress.XtraTab.XtraTabPage _tabVisit = null!;
        private DevExpress.XtraTab.XtraTabPage _tabStats = null!;
        private ClinicPatientControl _patientControl = null!;
        private ClinicVisitControl _visitControl = null!;
        private ClinicStatsControl _statsControl = null!;

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
            _tabControl = new DevExpress.XtraTab.XtraTabControl();
            _tabPatient = new DevExpress.XtraTab.XtraTabPage();
            _patientControl = new ClinicPatientControl();
            _tabVisit = new DevExpress.XtraTab.XtraTabPage();
            _visitControl = new ClinicVisitControl();
            _tabStats = new DevExpress.XtraTab.XtraTabPage();
            _statsControl = new ClinicStatsControl();
            clinicPatientControl1 = new ClinicPatientControl();
            clinicStatsControl1 = new ClinicStatsControl();
            ((System.ComponentModel.ISupportInitialize)_tabControl).BeginInit();
            _tabControl.SuspendLayout();
            _tabPatient.SuspendLayout();
            _tabVisit.SuspendLayout();
            _tabStats.SuspendLayout();
            SuspendLayout();
            // 
            // _tabControl
            // 
            _tabControl.Dock = DockStyle.Fill;
            _tabControl.Location = new Point(0, 0);
            _tabControl.Margin = new Padding(4, 5, 4, 5);
            _tabControl.Name = "_tabControl";
            _tabControl.SelectedTabPage = _tabPatient;
            _tabControl.Size = new Size(2206, 1081);
            _tabControl.TabIndex = 0;
            _tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] { _tabPatient, _tabVisit, _tabStats });
            // 
            // _tabPatient
            // 
            _tabPatient.Controls.Add(_patientControl);
            _tabPatient.Margin = new Padding(4, 5, 4, 5);
            _tabPatient.Name = "_tabPatient";
            _tabPatient.Size = new Size(2204, 1043);
            _tabPatient.Text = "환자 선택";
            // 
            // _patientControl
            // 
            _patientControl.Dock = DockStyle.Fill;
            _patientControl.Location = new Point(0, 0);
            _patientControl.Margin = new Padding(6, 8, 6, 8);
            _patientControl.Name = "_patientControl";
            _patientControl.Size = new Size(2204, 1043);
            _patientControl.TabIndex = 0;
            _patientControl.PatientSelected += OnPatientSelectedFromControl;
            // 
            // _tabVisit
            // 
            _tabVisit.Controls.Add(_visitControl);
            _tabVisit.Margin = new Padding(4, 5, 4, 5);
            _tabVisit.Name = "_tabVisit";
            _tabVisit.Size = new Size(2204, 1043);
            _tabVisit.Text = "진료 기록";
            // 
            // _visitControl
            // 
            _visitControl.Dock = DockStyle.Fill;
            _visitControl.Location = new Point(0, 0);
            _visitControl.Margin = new Padding(6, 8, 6, 8);
            _visitControl.Name = "_visitControl";
            _visitControl.Size = new Size(2204, 1043);
            _visitControl.TabIndex = 0;
            _visitControl.VisitRecorded += OnVisitRecordedFromControl;
            // 
            // _tabStats
            // 
            _tabStats.Controls.Add(_statsControl);
            _tabStats.Margin = new Padding(4, 5, 4, 5);
            _tabStats.Name = "_tabStats";
            _tabStats.Size = new Size(2204, 1043);
            _tabStats.Text = "진료 통계";
            // 
            // _statsControl
            // 
            _statsControl.Dock = DockStyle.Fill;
            _statsControl.Location = new Point(0, 0);
            _statsControl.Margin = new Padding(6, 8, 6, 8);
            _statsControl.Name = "_statsControl";
            _statsControl.Size = new Size(2204, 1043);
            _statsControl.TabIndex = 0;
            // 
            // clinicPatientControl1
            // 
            clinicPatientControl1.Location = new Point(0, 0);
            clinicPatientControl1.Margin = new Padding(4, 5, 4, 5);
            clinicPatientControl1.Name = "clinicPatientControl1";
            clinicPatientControl1.Size = new Size(1528, 850);
            clinicPatientControl1.TabIndex = 1;
            // 
            // clinicStatsControl1
            // 
            clinicStatsControl1.Dock = DockStyle.Fill;
            clinicStatsControl1.Location = new Point(0, 0);
            clinicStatsControl1.Margin = new Padding(4, 5, 4, 5);
            clinicStatsControl1.Name = "clinicStatsControl1";
            clinicStatsControl1.Size = new Size(1339, 697);
            clinicStatsControl1.TabIndex = 2;
            // 
            // ClinicMainControl
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_tabControl);
            Margin = new Padding(4, 5, 4, 5);
            Name = "ClinicMainControl";
            Size = new Size(2206, 1081);
            ((System.ComponentModel.ISupportInitialize)_tabControl).EndInit();
            _tabControl.ResumeLayout(false);
            _tabPatient.ResumeLayout(false);
            _tabVisit.ResumeLayout(false);
            _tabStats.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private ClinicStatsControl clinicStatsControl1;
        private ClinicPatientControl clinicPatientControl1;
    }
}
