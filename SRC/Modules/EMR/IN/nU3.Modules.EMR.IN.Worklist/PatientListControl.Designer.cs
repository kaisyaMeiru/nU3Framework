using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;

namespace nU3.Modules.EMR.IN.Worklist
{
    public partial class PatientListControl
    {
        private GridControl gridControl;
        private GridView gridView;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit txtSearch;
        private Label lblTitle;

        protected void InitializeLayout()
        {
            this.SuspendLayout();

            // 타이틀 레이블
            lblTitle = new Label
            {
                Text = "환자 목록 (이벤트 발행자)",
                Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(400, 30),
                ForeColor = Color.FromArgb(0, 122, 204)
            };

            // 검색 텍스트박스
            txtSearch = new DevExpress.XtraEditors.TextEdit
            {
                Location = new Point(20, 60),
                Size = new Size(200, 20)
            };
            txtSearch.Properties.NullValuePrompt = "환자명 또는 ID 검색...";

            // 검색 버튼
            btnSearch = new DevExpress.XtraEditors.SimpleButton
            {
                Text = "검색",
                Location = new Point(230, 58),
                Size = new Size(80, 24)
            };
            btnSearch.Click += BtnSearch_Click;

            // 새로고침 버튼
            btnRefresh = new DevExpress.XtraEditors.SimpleButton
            {
                Text = "새로고침",
                Location = new Point(320, 58),
                Size = new Size(100, 24)
            };
            btnRefresh.Click += BtnRefresh_Click;

            // 그리드 컨트롤
            gridControl = new GridControl
            {
                Location = new Point(20, 100),
                Size = new Size(760, 480),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            gridView = new GridView(gridControl)
            {
                OptionsBehavior = { Editable = false },
                OptionsView = { ShowGroupPanel = false }
            };
            gridControl.MainView = gridView;

            // 컬럼 설정
            SetupGridColumns();

            // 컬럼 설정 메서드는 이 파일 내에 존재
            void SetupGridColumns()
            {
                gridView.Columns.Clear();

                gridView.Columns.Add(new GridColumn
                {
                    FieldName = "PatientId",
                    Caption = "환자 ID",
                    Visible = true,
                    Width = 100
                });

                gridView.Columns.Add(new GridColumn
                {
                    FieldName = "PatientName",
                    Caption = "환자명",
                    Visible = true,
                    Width = 120
                });

                gridView.Columns.Add(new GridColumn
                {
                    FieldName = "BirthDate",
                    Caption = "생년월일",
                    Visible = true,
                    Width = 100,
                    DisplayFormat = { FormatString = "yyyy-MM-dd", FormatType = FormatType.DateTime }
                });

                gridView.Columns.Add(new GridColumn
                {
                    FieldName = "Gender",
                    Caption = "성별",
                    Visible = true,
                    Width = 60
                });

                gridView.Columns.Add(new GridColumn
                {
                    FieldName = "MobileNumber",
                    Caption = "휴대전화",
                    Visible = true,
                    Width = 120
                });

                gridView.Columns.Add(new GridColumn
                {
                    FieldName = "PhoneNumber",
                    Caption = "일반전화",
                    Visible = true,
                    Width = 120
                });

                gridView.Columns.Add(new GridColumn
                {
                    FieldName = "Address",
                    Caption = "주소",
                    Visible = true,
                    Width = 240
                });
            }

            // 이벤트 핸들러 등록
            gridView.FocusedRowChanged += GridView_FocusedRowChanged;
            gridView.DoubleClick += GridView_DoubleClick;

            // 샘플 데이터 로드
            LoadSampleData();

            // 컨트롤 추가
            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                txtSearch,
                btnSearch,
                btnRefresh,
                gridControl
            });

            this.ResumeLayout();

            
        }
    }
}
