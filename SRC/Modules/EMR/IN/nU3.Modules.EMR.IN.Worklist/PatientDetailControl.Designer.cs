using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace nU3.Modules.EMR.IN.Worklist
{
    public partial class PatientDetailControl
    {
        // UI fields
        private GroupControl grpPatientInfo;
        private GroupControl grpContactInfo;
        private GroupControl grpEventLog;

        private LabelControl lblPatientIdLabel;
        private LabelControl lblPatientIdValue;
        private LabelControl lblPatientNameLabel;
        private LabelControl lblPatientNameValue;
        private LabelControl lblBirthDateLabel;
        private LabelControl lblBirthDateValue;
        private LabelControl lblGenderLabel;
        private LabelControl lblGenderValue;
        private LabelControl lblAgeLabel;
        private LabelControl lblAgeValue;

        private LabelControl lblPhoneLabel;
        private LabelControl lblPhoneValue;
        private LabelControl lblAddressLabel;
        private LabelControl lblAddressValue;

        private MemoEdit memoEventLog;
        private LabelControl lblStatus;

        // Initialize layout and UI components (designer-style)
        protected void InitializeLayout()
        {
            this.SuspendLayout();

            // Patient group
            grpPatientInfo = new GroupControl
            {
                Text = "환자 기본 정보",
                Location = new Point(20, 20),
                Size = new Size(760, 180)
            };

            // Contact group
            grpContactInfo = new GroupControl
            {
                Text = "연락처 정보",
                Location = new Point(20, 210),
                Size = new Size(760, 120)
            };

            // Event log group
            grpEventLog = new GroupControl
            {
                Text = "이벤트 로그 (읽기 전용)",
                Location = new Point(20, 340),
                Size = new Size(760, 240),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            memoEventLog = new MemoEdit
            {
                Location = new Point(10, 30),
                Size = new Size(740, 200),
                Dock = DockStyle.Fill
            };
            memoEventLog.Properties.ReadOnly = true;
            grpEventLog.Controls.Add(memoEventLog);

            // Status label
            lblStatus = new LabelControl
            {
                Text = "상태: 초기화 중...",
                Location = new Point(20, 590),
                Size = new Size(760, 20),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            lblStatus.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Italic);

            // Create inner labels and values
            CreatePatientInfoLabels();
            CreateContactInfoLabels();

            // Add controls to container
            this.Controls.AddRange(new Control[]
            {
                grpPatientInfo,
                grpContactInfo,
                grpEventLog,
                lblStatus
            });

            this.ResumeLayout(false);

            // initial log entry
            AddEventLog("PatientDetailControl initialized");
        }

        private void CreatePatientInfoLabels()
        {
            int labelY = 30;
            int valueY = 30;
            int rowHeight = 30;

            // Patient ID
            lblPatientIdLabel = CreateLabel("환자 ID:", 20, labelY, 100);
            lblPatientIdValue = CreateValueLabel("", 130, valueY, 200);
            grpPatientInfo.Controls.AddRange(new Control[] { lblPatientIdLabel, lblPatientIdValue });

            // Patient Name
            labelY += rowHeight; valueY += rowHeight;
            lblPatientNameLabel = CreateLabel("환자명:", 20, labelY, 100);
            lblPatientNameValue = CreateValueLabel("", 130, valueY, 200);
            grpPatientInfo.Controls.AddRange(new Control[] { lblPatientNameLabel, lblPatientNameValue });

            // Birth Date
            labelY += rowHeight; valueY += rowHeight;
            lblBirthDateLabel = CreateLabel("생년월일:", 20, labelY, 100);
            lblBirthDateValue = CreateValueLabel("", 130, valueY, 200);
            grpPatientInfo.Controls.AddRange(new Control[] { lblBirthDateLabel, lblBirthDateValue });

            // Gender / Age
            labelY += rowHeight; valueY += rowHeight;
            lblGenderLabel = CreateLabel("성별:", 20, labelY, 100);
            lblGenderValue = CreateValueLabel("", 130, valueY, 60);
            lblAgeLabel = CreateLabel("나이:", 220, labelY, 100);
            lblAgeValue = CreateValueLabel("", 330, valueY, 60);
            grpPatientInfo.Controls.AddRange(new Control[]
            {
                lblGenderLabel, lblGenderValue,
                lblAgeLabel, lblAgeValue
            });
        }

        private void CreateContactInfoLabels()
        {
            int labelY = 30;
            int valueY = 30;
            int rowHeight = 30;

            lblPhoneLabel = CreateLabel("전화:", 20, labelY, 100);
            lblPhoneValue = CreateValueLabel("", 130, valueY, 200);
            grpContactInfo.Controls.AddRange(new Control[] { lblPhoneLabel, lblPhoneValue });

            labelY += rowHeight; valueY += rowHeight;
            lblAddressLabel = CreateLabel("주소:", 20, labelY, 100);
            lblAddressValue = CreateValueLabel("", 130, valueY, 600);
            grpContactInfo.Controls.AddRange(new Control[] { lblAddressLabel, lblAddressValue });
        }

        private LabelControl CreateLabel(string text, int x, int y, int width)
        {
            var lbl = new LabelControl
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 20)
            };
            lbl.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            return lbl;
        }

        private LabelControl CreateValueLabel(string text, int x, int y, int width)
        {
            var lbl = new LabelControl
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 20)
            };
            lbl.Appearance.Font = new Font("Segoe UI", 9F);
            lbl.Appearance.ForeColor = Color.FromArgb(0, 0, 192);
            return lbl;
        }
    }
}
