namespace nU3.Core.UI.Components.Controls
{
    partial class PatientInfoControl
    {
        /// <summary> ?꾩닔 ?붿옄?대꼫 蹂?섏엯?덈떎. </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> ?ъ슜 以묒씤 紐⑤뱺 由ъ냼?ㅻ? ?뺣━?⑸땲?? </summary>
        /// <param name="disposing">愿由щ릺??由ъ냼?ㅻ? ??젣?댁빞 ?섎㈃ true?닿퀬, 洹몃젃吏 ?딆쑝硫?false?낅땲??</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> ?붿옄?대꼫 吏?먯뿉 ?꾩슂??硫붿꽌?쒖엯?덈떎. ??硫붿꽌?쒖쓽 ?댁슜??코드 ?몄쭛湲곕줈 ?섏젙?섏? 留덉떗?쒖삤. </summary>
        private void InitializeComponent()
        {
            grpPatientInfo = new nU3.Core.UI.Controls.nU3GroupControl();
            lblDiagnosis = new nU3.Core.UI.Controls.nU3LabelControl();
            txtDiagnosis = new nU3.Core.UI.Controls.nU3TextEdit();
            lblInDate = new nU3.Core.UI.Controls.nU3LabelControl();
            txtInDate = new nU3.Core.UI.Controls.nU3TextEdit();
            lblRoomNo = new nU3.Core.UI.Controls.nU3LabelControl();
            txtRoomNo = new nU3.Core.UI.Controls.nU3TextEdit();
            lblDoctorName = new nU3.Core.UI.Controls.nU3LabelControl();
            txtDoctorName = new nU3.Core.UI.Controls.nU3TextEdit();
            lblDeptName = new nU3.Core.UI.Controls.nU3LabelControl();
            txtDeptName = new nU3.Core.UI.Controls.nU3TextEdit();
            lblAge = new nU3.Core.UI.Controls.nU3LabelControl();
            txtAge = new nU3.Core.UI.Controls.nU3TextEdit();
            lblGender = new nU3.Core.UI.Controls.nU3LabelControl();
            txtGender = new nU3.Core.UI.Controls.nU3TextEdit();
            lblPatName = new nU3.Core.UI.Controls.nU3LabelControl();
            txtPatName = new nU3.Core.UI.Controls.nU3TextEdit();
            lblPatId = new nU3.Core.UI.Controls.nU3LabelControl();
            txtPatId = new nU3.Core.UI.Controls.nU3TextEdit();
            pnlButtons = new nU3.Core.UI.Controls.nU3PanelControl();
            btnVital = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnMedi = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnAlert = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnAlergy = new nU3.Core.UI.Controls.nU3SimpleButton();
            ((System.ComponentModel.ISupportInitialize)grpPatientInfo).BeginInit();
            grpPatientInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlButtons).BeginInit();
            pnlButtons.SuspendLayout();
            SuspendLayout();
            // 
            // grpPatientInfo
            // 
            grpPatientInfo.Appearance.BackColor = Color.White;
            grpPatientInfo.Appearance.BackColor2 = Color.White;
            grpPatientInfo.Appearance.Options.UseBackColor = true;
            grpPatientInfo.Controls.Add(lblDiagnosis);
            grpPatientInfo.Controls.Add(txtDiagnosis);
            grpPatientInfo.Controls.Add(lblInDate);
            grpPatientInfo.Controls.Add(txtInDate);
            grpPatientInfo.Controls.Add(lblRoomNo);
            grpPatientInfo.Controls.Add(txtRoomNo);
            grpPatientInfo.Controls.Add(lblDoctorName);
            grpPatientInfo.Controls.Add(txtDoctorName);
            grpPatientInfo.Controls.Add(lblDeptName);
            grpPatientInfo.Controls.Add(txtDeptName);
            grpPatientInfo.Controls.Add(lblAge);
            grpPatientInfo.Controls.Add(txtAge);
            grpPatientInfo.Controls.Add(lblGender);
            grpPatientInfo.Controls.Add(txtGender);
            grpPatientInfo.Controls.Add(lblPatName);
            grpPatientInfo.Controls.Add(txtPatName);
            grpPatientInfo.Controls.Add(lblPatId);
            grpPatientInfo.Controls.Add(txtPatId);
            grpPatientInfo.Dock = DockStyle.Fill;
            grpPatientInfo.Location = new Point(0, 0);
            grpPatientInfo.Margin = new Padding(4);
            grpPatientInfo.Name = "grpPatientInfo";
            grpPatientInfo.Size = new Size(1305, 852);
            grpPatientInfo.TabIndex = 0;
            grpPatientInfo.Text = "환자정보";
            // 
            // lblDiagnosis
            // 
            lblDiagnosis.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lblDiagnosis.Appearance.Options.UseFont = true;
            lblDiagnosis.IsRequiredMarker = false;
            lblDiagnosis.Location = new Point(951, 4);
            lblDiagnosis.Margin = new Padding(4);
            lblDiagnosis.Name = "lblDiagnosis";
            lblDiagnosis.Size = new Size(30, 14);
            lblDiagnosis.TabIndex = 17;
            lblDiagnosis.Text = "진단명";
            // 
            // txtDiagnosis
            // 
            txtDiagnosis.IsRequired = false;
            txtDiagnosis.Location = new Point(989, 0);
            txtDiagnosis.Margin = new Padding(4);
            txtDiagnosis.Name = "txtDiagnosis";
            txtDiagnosis.Size = new Size(373, 20);
            txtDiagnosis.TabIndex = 16;
            // 
            // lblInDate
            // 
            lblInDate.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lblInDate.Appearance.Options.UseFont = true;
            lblInDate.IsRequiredMarker = false;
            lblInDate.Location = new Point(778, 4);
            lblInDate.Margin = new Padding(4);
            lblInDate.Name = "lblInDate";
            lblInDate.Size = new Size(40, 14);
            lblInDate.TabIndex = 15;
            lblInDate.Text = "입원일자";
            // 
            // txtInDate
            // 
            txtInDate.IsRequired = false;
            txtInDate.Location = new Point(826, 0);
            txtInDate.Margin = new Padding(4);
            txtInDate.Name = "txtInDate";
            txtInDate.Size = new Size(117, 20);
            txtInDate.TabIndex = 14;
            // 
            // lblRoomNo
            // 
            lblRoomNo.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lblRoomNo.Appearance.Options.UseFont = true;
            lblRoomNo.IsRequiredMarker = false;
            lblRoomNo.Location = new Point(676, 4);
            lblRoomNo.Margin = new Padding(4);
            lblRoomNo.Name = "lblRoomNo";
            lblRoomNo.Size = new Size(20, 14);
            lblRoomNo.TabIndex = 13;
            lblRoomNo.Text = "병실";
            // 
            // txtRoomNo
            // 
            txtRoomNo.IsRequired = false;
            txtRoomNo.Location = new Point(700, 0);
            txtRoomNo.Margin = new Padding(4);
            txtRoomNo.Name = "txtRoomNo";
            txtRoomNo.Size = new Size(70, 20);
            txtRoomNo.TabIndex = 12;
            // 
            // lblDoctorName
            // 
            lblDoctorName.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lblDoctorName.Appearance.Options.UseFont = true;
            lblDoctorName.IsRequiredMarker = false;
            lblDoctorName.Location = new Point(526, 4);
            lblDoctorName.Margin = new Padding(4);
            lblDoctorName.Name = "lblDoctorName";
            lblDoctorName.Size = new Size(40, 14);
            lblDoctorName.TabIndex = 11;
            lblDoctorName.Text = "담당의사";
            // 
            // txtDoctorName
            // 
            txtDoctorName.IsRequired = false;
            txtDoctorName.Location = new Point(574, 0);
            txtDoctorName.Margin = new Padding(4);
            txtDoctorName.Name = "txtDoctorName";
            txtDoctorName.Size = new Size(94, 20);
            txtDoctorName.TabIndex = 10;
            // 
            // lblDeptName
            // 
            lblDeptName.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lblDeptName.Appearance.Options.UseFont = true;
            lblDeptName.IsRequiredMarker = false;
            lblDeptName.Location = new Point(406, 4);
            lblDeptName.Margin = new Padding(4);
            lblDeptName.Name = "lblDeptName";
            lblDeptName.Size = new Size(30, 14);
            lblDeptName.TabIndex = 9;
            lblDeptName.Text = "진료과";
            // 
            // txtDeptName
            // 
            txtDeptName.IsRequired = false;
            txtDeptName.Location = new Point(444, 0);
            txtDeptName.Margin = new Padding(4);
            txtDeptName.Name = "txtDeptName";
            txtDeptName.Size = new Size(74, 20);
            txtDeptName.TabIndex = 8;
            // 
            // lblAge
            // 
            lblAge.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lblAge.Appearance.Options.UseFont = true;
            lblAge.IsRequiredMarker = false;
            lblAge.Location = new Point(340, 4);
            lblAge.Margin = new Padding(4);
            lblAge.Name = "lblAge";
            lblAge.Size = new Size(20, 14);
            lblAge.TabIndex = 7;
            lblAge.Text = "나이";
            // 
            // txtAge
            // 
            txtAge.IsRequired = false;
            txtAge.Location = new Point(368, 0);
            txtAge.Margin = new Padding(4);
            txtAge.Name = "txtAge";
            txtAge.Size = new Size(30, 20);
            txtAge.TabIndex = 6;
            // 
            // lblGender
            // 
            lblGender.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lblGender.Appearance.Options.UseFont = true;
            lblGender.IsRequiredMarker = false;
            lblGender.Location = new Point(272, 4);
            lblGender.Margin = new Padding(4);
            lblGender.Name = "lblGender";
            lblGender.Size = new Size(20, 14);
            lblGender.TabIndex = 5;
            lblGender.Text = "성별";
            // 
            // txtGender
            // 
            txtGender.IsRequired = false;
            txtGender.Location = new Point(300, 0);
            txtGender.Margin = new Padding(4);
            txtGender.Name = "txtGender";
            txtGender.Size = new Size(32, 20);
            txtGender.TabIndex = 4;
            // 
            // lblPatName
            // 
            lblPatName.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lblPatName.Appearance.Options.UseFont = true;
            lblPatName.IsRequiredMarker = false;
            lblPatName.Location = new Point(155, 4);
            lblPatName.Margin = new Padding(4);
            lblPatName.Name = "lblPatName";
            lblPatName.Size = new Size(30, 14);
            lblPatName.TabIndex = 3;
            lblPatName.Text = "환자명";
            // 
            // txtPatName
            // 
            txtPatName.IsRequired = false;
            txtPatName.Location = new Point(193, 0);
            txtPatName.Margin = new Padding(4);
            txtPatName.Name = "txtPatName";
            txtPatName.Size = new Size(71, 20);
            txtPatName.TabIndex = 2;
            // 
            // lblPatId
            // 
            lblPatId.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lblPatId.Appearance.Options.UseFont = true;
            lblPatId.IsRequiredMarker = false;
            lblPatId.Location = new Point(60, 4);
            lblPatId.Margin = new Padding(4);
            lblPatId.Name = "lblPatId";
            lblPatId.Size = new Size(34, 14);
            lblPatId.TabIndex = 1;
            lblPatId.Text = "환자ID";
            // 
            // txtPatId
            // 
            txtPatId.IsRequired = false;
            txtPatId.Location = new Point(102, 0);
            txtPatId.Margin = new Padding(4);
            txtPatId.Name = "txtPatId";
            txtPatId.Size = new Size(45, 20);
            txtPatId.TabIndex = 0;
            // 
            // pnlButtons
            // 
            pnlButtons.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlButtons.Appearance.BackColor = Color.Transparent;
            pnlButtons.Appearance.Options.UseBackColor = true;
            pnlButtons.Controls.Add(btnVital);
            pnlButtons.Controls.Add(btnMedi);
            pnlButtons.Controls.Add(btnAlert);
            pnlButtons.Controls.Add(btnAlergy);
            pnlButtons.Location = new Point(2769, 0);
            pnlButtons.Margin = new Padding(4);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Size = new Size(30, 55);
            pnlButtons.TabIndex = 1;
            // 
            // btnVital
            // 
            btnVital.AuthId = "";
            btnVital.Location = new Point(3, 0);
            btnVital.Margin = new Padding(4);
            btnVital.Name = "btnVital";
            btnVital.Size = new Size(26, 28);
            btnVital.TabIndex = 3;
            btnVital.ToolTip = "생체징후";
            btnVital.Click += btnVital_Click;
            // 
            // btnMedi
            // 
            btnMedi.AuthId = "";
            btnMedi.Location = new Point(3, 0);
            btnMedi.Margin = new Padding(4);
            btnMedi.Name = "btnMedi";
            btnMedi.Size = new Size(26, 28);
            btnMedi.TabIndex = 2;
            btnMedi.ToolTip = "투약정보";
            btnMedi.Click += btnMedi_Click;
            // 
            // btnAlert
            // 
            btnAlert.AuthId = "";
            btnAlert.Location = new Point(3, 0);
            btnAlert.Margin = new Padding(4);
            btnAlert.Name = "btnAlert";
            btnAlert.Size = new Size(26, 28);
            btnAlert.TabIndex = 1;
            btnAlert.ToolTip = "주의사항";
            btnAlert.Click += btnAlert_Click;
            // 
            // btnAlergy
            // 
            btnAlergy.AuthId = "";
            btnAlergy.Location = new Point(3, 0);
            btnAlergy.Margin = new Padding(4);
            btnAlergy.Name = "btnAlergy";
            btnAlergy.Size = new Size(26, 28);
            btnAlergy.TabIndex = 0;
            btnAlergy.ToolTip = "알레르기";
            btnAlergy.Click += btnAlergy_Click;
            // 
            // PatientInfoControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlButtons);
            Controls.Add(grpPatientInfo);
            Margin = new Padding(4);
            Name = "PatientInfoControl";
            Size = new Size(1305, 852);
            ((System.ComponentModel.ISupportInitialize)grpPatientInfo).EndInit();
            grpPatientInfo.ResumeLayout(false);
            grpPatientInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pnlButtons).EndInit();
            pnlButtons.ResumeLayout(false);
            ResumeLayout(false);
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
