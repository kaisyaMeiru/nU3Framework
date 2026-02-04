using System;
using System.Drawing;
using System.Windows.Forms;

namespace nU3.Modules.EMR.IN.Worklist
{
    public partial class SampleWorkControl
    {
        private Label _lblTitle;
        private Label _lblPatientInfo;
        private Label _lblUserInfo;
        private Label _lblEventLog;
        private TextBox _txtEventLog;
        private Button _btnTest;

        protected void InitializeLayout()
        {
            this.SuspendLayout();

            // 타이틀 레이블
            _lblTitle = new Label
            {
                Text = "샘플 작업 화면 (이벤트 구독 데모)",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(500, 30),
                ForeColor = Color.FromArgb(0, 122, 204)
            };

            // 환자 정보 표시 레이블
            _lblPatientInfo = new Label
            {
                Text = "환자 정보: 대기 중...",
                Location = new Point(20, 70),
                Size = new Size(700, 60),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            // 사용자 정보 레이블
            _lblUserInfo = new Label
            {
                Text = "사용자 정보: 대기 중...",
                Location = new Point(20, 140),
                Size = new Size(700, 60),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            // 이벤트 로그 라벨
            _lblEventLog = new Label
            {
                Text = "수신된 이벤트:",
                Location = new Point(20, 220),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            // 이벤트 로그 텍스트박스
            _txtEventLog = new TextBox
            {
                Location = new Point(20, 250),
                Size = new Size(700, 300),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // 테스트 버튼
            _btnTest = new Button
            {
                Text = "이벤트 발행 테스트",
                Location = new Point(20, 560),
                Size = new Size(150, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            _btnTest.Click += BtnTest_Click;

            this.Controls.AddRange(new Control[] {
                _lblTitle,
                _lblPatientInfo,
                _lblUserInfo,
                _lblEventLog,
                _txtEventLog,
                _btnTest
            });

            this.ResumeLayout(false);

            AddEventLog("SampleWorkControl 초기화 완료");
        }
    }
}
