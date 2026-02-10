namespace nU3.Modules.OCS.IN.OrderEntry.Controls
{
    partial class PatientInfoControl
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpPatientInfo = new nU3.Core.UI.Controls.nU3GroupControl();
            this.lblDiagnosis = new nU3.Core.UI.Controls.nU3LabelControl();
            this.txtDiagnosis = new nU3.Core.UI.Controls.nU3TextEdit();
            this.lblInDate = new nU3.Core.UI.Controls.nU3LabelControl();
            this.txtInDate = new nU3.Core.UI.Controls.nU3TextEdit();
            this.lblRoomNo = new nU3.Core.UI.Controls.nU3LabelControl();
            this.txtRoomNo = new nU3.Core.UI.Controls.nU3TextEdit();
            this.lblDoctorName = new nU3.Core.UI.Controls.nU3LabelControl();
            this.txtDoctorName = new nU3.Core.UI.Controls.nU3TextEdit();
            this.lblDeptName = new nU3.Core.UI.Controls.nU3LabelControl();
            this.txtDeptName = new nU3.Core.UI.Controls.nU3TextEdit();
            this.lblAge = new nU3.Core.UI.Controls.nU3LabelControl();
            this.txtAge = new nU3.Core.UI.Controls.nU3TextEdit();
            this.lblGender = new nU3.Core.UI.Controls.nU3LabelControl();
            this.txtGender = new nU3.Core.UI.Controls.nU3TextEdit();
            this.lblPatName = new nU3.Core.UI.Controls.nU3LabelControl();
            this.txtPatName = new nU3.Core.UI.Controls.nU3TextEdit();
            this.lblPatId = new nU3.Core.UI.Controls.nU3LabelControl();
            this.txtPatId = new nU3.Core.UI.Controls.nU3TextEdit();
            this.pnlButtons = new nU3.Core.UI.Controls.nU3PanelControl();
            this.btnVital = new nU3.Core.UI.Controls.nU3SimpleButton();
            this.btnMedi = new nU3.Core.UI.Controls.nU3SimpleButton();
            this.btnAlert = new nU3.Core.UI.Controls.nU3SimpleButton();
            this.btnAlergy = new nU3.Core.UI.Controls.nU3SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.grpPatientInfo)).BeginInit();
            this.grpPatientInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDiagnosis.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRoomNo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDoctorName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAge.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGender.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPatientInfo
            // 
            this.grpPatientInfo.Appearance.BackColor = System.Drawing.Color.White;
            this.grpPatientInfo.Appearance.BackColor2 = System.Drawing.Color.White;
            this.grpPatientInfo.Appearance.Options.UseBackColor = true;
            this.grpPatientInfo.Controls.Add(this.lblDiagnosis);
            this.grpPatientInfo.Controls.Add(this.txtDiagnosis);
            this.grpPatientInfo.Controls.Add(this.lblInDate);
            this.grpPatientInfo.Controls.Add(this.txtInDate);
            this.grpPatientInfo.Controls.Add(this.lblRoomNo);
            this.grpPatientInfo.Controls.Add(this.txtRoomNo);
            this.grpPatientInfo.Controls.Add(this.lblDoctorName);
            this.grpPatientInfo.Controls.Add(this.txtDoctorName);
            this.grpPatientInfo.Controls.Add(this.lblDeptName);
            this.grpPatientInfo.Controls.Add(this.txtDeptName);
            this.grpPatientInfo.Controls.Add(this.lblAge);
            this.grpPatientInfo.Controls.Add(this.txtAge);
            this.grpPatientInfo.Controls.Add(this.lblGender);
            this.grpPatientInfo.Controls.Add(this.txtGender);
            this.grpPatientInfo.Controls.Add(this.lblPatName);
            this.grpPatientInfo.Controls.Add(this.txtPatName);
            this.grpPatientInfo.Controls.Add(this.lblPatId);
            this.grpPatientInfo.Controls.Add(this.txtPatId);
            this.grpPatientInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPatientInfo.Location = new System.Drawing.Point(0, 0);
            this.grpPatientInfo.Name = "grpPatientInfo";
            this.grpPatientInfo.Size = new System.Drawing.Size(1280, 68);
            this.grpPatientInfo.TabIndex = 0;
            this.grpPatientInfo.Text = "환자정보";
            // 
            // lblDiagnosis
            // 
            this.lblDiagnosis.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblDiagnosis.Appearance.Options.UseFont = true;
            this.lblDiagnosis.Location = new System.Drawing.Point(832, 24);
            this.lblDiagnosis.Name = "lblDiagnosis";
            this.lblDiagnosis.Size = new System.Drawing.Size(80, 18);
            this.lblDiagnosis.TabIndex = 17;
            this.lblDiagnosis.Text = "진단명";
            // 
            // txtDiagnosis
            // 
            this.txtDiagnosis.Location = new System.Drawing.Point(918, 22);
            this.txtDiagnosis.Name = "txtDiagnosis";
            this.txtDiagnosis.Properties.ReadOnly = true;
            this.txtDiagnosis.Size = new System.Drawing.Size(320, 20);
            this.txtDiagnosis.TabIndex = 16;
            // 
            // lblInDate
            // 
            this.lblInDate.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblInDate.Appearance.Options.UseFont = true;
            this.lblInDate.Location = new System.Drawing.Point(632, 24);
            this.lblInDate.Name = "lblInDate";
            this.lblInDate.Size = new System.Drawing.Size(80, 18);
            this.lblInDate.TabIndex = 15;
            this.lblInDate.Text = "입원일자";
            // 
            // txtInDate
            // 
            this.txtInDate.Location = new System.Drawing.Point(718, 22);
            this.txtInDate.Name = "txtInDate";
            this.txtInDate.Properties.ReadOnly = true;
            this.txtInDate.Size = new System.Drawing.Size(100, 20);
            this.txtInDate.TabIndex = 14;
            // 
            // lblRoomNo
            // 
            this.lblRoomNo.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblRoomNo.Appearance.Options.UseFont = true;
            this.lblRoomNo.Location = new System.Drawing.Point(544, 24);
            this.lblRoomNo.Name = "lblRoomNo";
            this.lblRoomNo.Size = new System.Drawing.Size(60, 18);
            this.lblRoomNo.TabIndex = 13;
            this.lblRoomNo.Text = "병실";
            // 
            // txtRoomNo
            // 
            this.txtRoomNo.Location = new System.Drawing.Point(610, 22);
            this.txtRoomNo.Name = "txtRoomNo";
            this.txtRoomNo.Properties.ReadOnly = true;
            this.txtRoomNo.Size = new System.Drawing.Size(60, 20);
            this.txtRoomNo.TabIndex = 12;
            // 
            // lblDoctorName
            // 
            this.lblDoctorName.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblDoctorName.Appearance.Options.UseFont = true;
            this.lblDoctorName.Location = new System.Drawing.Point(416, 24);
            this.lblDoctorName.Name = "lblDoctorName";
            this.lblDoctorName.Size = new System.Drawing.Size(80, 18);
            this.lblDoctorName.TabIndex = 11;
            this.lblDoctorName.Text = "담당의사";
            // 
            // txtDoctorName
            // 
            this.txtDoctorName.Location = new System.Drawing.Point(502, 22);
            this.txtDoctorName.Name = "txtDoctorName";
            this.txtDoctorName.Properties.ReadOnly = true;
            this.txtDoctorName.Size = new System.Drawing.Size(100, 20);
            this.txtDoctorName.TabIndex = 10;
            // 
            // lblDeptName
            // 
            this.lblDeptName.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblDeptName.Appearance.Options.UseFont = true;
            this.lblDeptName.Location = new System.Drawing.Point(336, 24);
            this.lblDeptName.Name = "lblDeptName";
            this.lblDeptName.Size = new System.Drawing.Size(60, 18);
            this.lblDeptName.TabIndex = 9;
            this.lblDeptName.Text = "진료과";
            // 
            // txtDeptName
            // 
            this.txtDeptName.Location = new System.Drawing.Point(402, 22);
            this.txtDeptName.Name = "txtDeptName";
            this.txtDeptName.Properties.ReadOnly = true;
            this.txtDeptName.Size = new System.Drawing.Size(100, 20);
            this.txtDeptName.TabIndex = 8;
            // 
            // lblAge
            // 
            this.lblAge.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblAge.Appearance.Options.UseFont = true;
            this.lblAge.Location = new System.Drawing.Point(256, 24);
            this.lblAge.Name = "lblAge";
            this.lblAge.Size = new System.Drawing.Size(40, 18);
            this.lblAge.TabIndex = 7;
            this.lblAge.Text = "나이";
            // 
            // txtAge
            // 
            this.txtAge.Location = new System.Drawing.Point(302, 22);
            this.txtAge.Name = "txtAge";
            this.txtAge.Properties.ReadOnly = true;
            this.txtAge.Size = new System.Drawing.Size(60, 20);
            this.txtAge.TabIndex = 6;
            // 
            // lblGender
            // 
            this.lblGender.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblGender.Appearance.Options.UseFont = true;
            this.lblGender.Location = new System.Drawing.Point(208, 24);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(40, 18);
            this.lblGender.TabIndex = 5;
            this.lblGender.Text = "성별";
            // 
            // txtGender
            // 
            this.txtGender.Location = new System.Drawing.Point(254, 22);
            this.txtGender.Name = "txtGender";
            this.txtGender.Properties.ReadOnly = true;
            this.txtGender.Size = new System.Drawing.Size(40, 20);
            this.txtGender.TabIndex = 4;
            // 
            // lblPatName
            // 
            this.lblPatName.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblPatName.Appearance.Options.UseFont = true;
            this.lblPatName.Location = new System.Drawing.Point(96, 24);
            this.lblPatName.Name = "lblPatName";
            this.lblPatName.Size = new System.Drawing.Size(60, 18);
            this.lblPatName.TabIndex = 3;
            this.lblPatName.Text = "환자명";
            // 
            // txtPatName
            // 
            this.txtPatName.Location = new System.Drawing.Point(162, 22);
            this.txtPatName.Name = "txtPatName";
            this.txtPatName.Properties.ReadOnly = true;
            this.txtPatName.Size = new System.Drawing.Size(80, 20);
            this.txtPatName.TabIndex = 2;
            // 
            // lblPatId
            // 
            this.lblPatId.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblPatId.Appearance.Options.UseFont = true;
            this.lblPatId.Location = new System.Drawing.Point(16, 24);
            this.lblPatId.Name = "lblPatId";
            this.lblPatId.Size = new System.Drawing.Size(60, 18);
            this.lblPatId.TabIndex = 1;
            this.lblPatId.Text = "환자ID";
            // 
            // txtPatId
            // 
            this.txtPatId.Location = new System.Drawing.Point(82, 22);
            this.txtPatId.Name = "txtPatId";
            this.txtPatId.Properties.ReadOnly = true;
            this.txtPatId.Size = new System.Drawing.Size(80, 20);
            this.txtPatId.TabIndex = 0;
            // 
            // pnlButtons
            // 
            this.pnlButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlButtons.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.pnlButtons.Appearance.Options.UseBackColor = true;
            this.pnlButtons.Controls.Add(this.btnVital);
            this.pnlButtons.Controls.Add(this.btnMedi);
            this.pnlButtons.Controls.Add(this.btnAlert);
            this.pnlButtons.Controls.Add(this.btnAlergy);
            this.pnlButtons.Location = new System.Drawing.Point(1254, 22);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(26, 44);
            this.pnlButtons.TabIndex = 1;
            // 
            // btnVital
            // 
            this.btnVital.Location = new System.Drawing.Point(2, 32);
            this.btnVital.Name = "btnVital";
            this.btnVital.Size = new System.Drawing.Size(22, 22);
            this.btnVital.TabIndex = 3;
            this.btnVital.ToolTip = "생체징후";
            this.btnVital.Click += new System.EventHandler(this.btnVital_Click);
            // 
            // btnMedi
            // 
            this.btnMedi.Location = new System.Drawing.Point(2, 22);
            this.btnMedi.Name = "btnMedi";
            this.btnMedi.Size = new System.Drawing.Size(22, 22);
            this.btnMedi.TabIndex = 2;
            this.btnMedi.ToolTip = "투약정보";
            this.btnMedi.Click += new System.EventHandler(this.btnMedi_Click);
            // 
            // btnAlert
            // 
            this.btnAlert.Location = new System.Drawing.Point(2, 12);
            this.btnAlert.Name = "btnAlert";
            this.btnAlert.Size = new System.Drawing.Size(22, 22);
            this.btnAlert.TabIndex = 1;
            this.btnAlert.ToolTip = "주의사항";
            this.btnAlert.Click += new System.EventHandler(this.btnAlert_Click);
            // 
            // btnAlergy
            // 
            this.btnAlergy.Location = new System.Drawing.Point(2, 2);
            this.btnAlergy.Name = "btnAlergy";
            this.btnAlergy.Size = new System.Drawing.Size(22, 22);
            this.btnAlergy.TabIndex = 0;
            this.btnAlergy.ToolTip = "알레르기";
            this.btnAlergy.Click += new System.EventHandler(this.btnAlergy_Click);
            // 
            // PatientInfoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.grpPatientInfo);
            this.Name = "PatientInfoControl";
            this.Size = new System.Drawing.Size(1280, 68);
            ((System.ComponentModel.ISupportInitialize)(this.grpPatientInfo)).EndInit();
            this.grpPatientInfo.ResumeLayout(false);
            this.grpPatientInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDiagnosis.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRoomNo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDoctorName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAge.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGender.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private nU3.Core.UI.Controls.nU3GroupControl grpPatientInfo;
        private nU3.Core.UI.Controls.nU3LabelControl lblDiagnosis;
        private nU3.Core.UI.Controls.nU3TextEdit txtDiagnosis;
        private nU3.Core.UI.Controls.nU3LabelControl lblInDate;
        private nU3.Core.UI.Controls.nU3TextEdit txtInDate;
        private nU3.Core.UI.Controls.nU3LabelControl lblRoomNo;
        private nU3.Core.UI.Controls.nU3TextEdit txtRoomNo;
        private nU3.Core.UI.Controls.nU3LabelControl lblDoctorName;
        private nU3.Core.UI.Controls.nU3TextEdit txtDoctorName;
        private nU3.Core.UI.Controls.nU3LabelControl lblDeptName;
        private nU3.Core.UI.Controls.nU3TextEdit txtDeptName;
        private nU3.Core.UI.Controls.nU3LabelControl lblAge;
        private nU3.Core.UI.Controls.nU3TextEdit txtAge;
        private nU3.Core.UI.Controls.nU3LabelControl lblGender;
        private nU3.Core.UI.Controls.nU3TextEdit txtGender;
        private nU3.Core.UI.Controls.nU3LabelControl lblPatName;
        private nU3.Core.UI.Controls.nU3TextEdit txtPatName;
        private nU3.Core.UI.Controls.nU3LabelControl lblPatId;
        private nU3.Core.UI.Controls.nU3TextEdit txtPatId;
        private nU3.Core.UI.Controls.nU3PanelControl pnlButtons;
        private nU3.Core.UI.Controls.nU3SimpleButton btnVital;
        private nU3.Core.UI.Controls.nU3SimpleButton btnMedi;
        private nU3.Core.UI.Controls.nU3SimpleButton btnAlert;
        private nU3.Core.UI.Controls.nU3SimpleButton btnAlergy;
    }
}