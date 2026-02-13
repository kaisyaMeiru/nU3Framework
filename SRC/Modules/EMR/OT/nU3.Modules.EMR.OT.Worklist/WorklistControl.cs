using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.Extensions.DependencyInjection;
using nU3.Core.Attributes;
using nU3.Core.Context;
using nU3.Core.Events;
using nU3.Core.Events.Contracts;
using nU3.Core.Interfaces;
using nU3.Core.Logic;
using nU3.Core.UI;
using nU3.Core.UI.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nU3.Modules.EMR.OT.Worklist
{
    [nU3ProgramInfo(
        typeof(WorklistControl),
        "수술 워크리스트",  // 프로그램 이름
        "PROG_OT_WORKLIST", // 프로그램 ID
        "CHILD"             // 폼 타입
    )]
    public partial class WorklistControl : BaseWorkControl
    {
        // 비즈니스 로직 컴포넌트 (UI와 분리)
        private readonly WorklistBizLogic _logic;

        // 1. 디자이너용 기본 생성자
        public WorklistControl()
        {
            InitializeComponent();
            InitializeGrid();
        }

        // 2. 런타임에서 DI로 사용되는 생성자
        [ActivatorUtilitiesConstructor]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public WorklistControl(IBizLogicFactory logicFactory) 
            : this()
        {
            Console.WriteLine("[IBizLogicFactory 생성자] 호출");

            // 팩토리를 통해 필요한 비즈니스 로직 인스턴스를 생성
            _logic = logicFactory.Create<WorklistBizLogic>();
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public WorklistControl(IBizLogicFactory logicFactory, IDBAccessService db, IFileTransferService file) 
            : this()
        {
            Console.WriteLine("[복수 의존성 생성자] 호출");
            _logic = logicFactory.Create<WorklistBizLogic>();
        }

        // 그리드 초기화 및 기본 설정
        private void InitializeGrid()
        {
            _gridView.OptionsBehavior.Editable = false; // 편집 금지
            _gridView.OptionsView.ShowGroupPanel = false; // 그룹 패널 숨김

            // 행 선택 변경 이벤트 핸들러 등록
            _gridView.FocusedRowChanged += _gridView_FocusedRowChanged;

            _gridView.DoubleClick += _gridView_DoubleClick;
        }

        private void _gridView_DoubleClick(object? sender, EventArgs e)
        {
            if (_gridView.GetFocusedRow() is DataRowView rowView)
            {
                // 데이터 테이블 컬럼명이 변경되어 있을 수 있으므로 안전하게 컬럼 존재 여부를 확인해서 사용
                var row = rowView.Row;
                string patientId = string.Empty;
                string patientName = string.Empty;

                if (row.Table.Columns.Contains("PatientId"))
                {
                    patientId = row["PatientId"]?.ToString() ?? string.Empty;
                }
                else if (row.Table.Columns.Contains("MODULE_ID"))
                {
                    // 레거시/다른 데이터 소스용 행동
                    patientId = row["MODULE_ID"]?.ToString() ?? string.Empty;
                }

                if (row.Table.Columns.Contains("PatientName"))
                {
                    patientName = row["PatientName"]?.ToString() ?? string.Empty;
                }
                else if (row.Table.Columns.Contains("MODULE_NAME"))
                {
                    // 레거시/다른 데이터 소스용 행동
                    patientName = row["MODULE_NAME"]?.ToString() ?? string.Empty;
                }

                if (!string.IsNullOrEmpty(patientId))
                {

                    // 환자 상세 화면 열기 요청
                    var context = Context.Clone();
                    context.CurrentPatient = new Models.PatientInfoDto() {  PatientId = patientId , PatientName = patientName };
                    context.SetParameter("Mode", "View");


                    EventBus?.GetEvent<NavigationRequestEvent>()
                    .Publish(new NavigationRequestEventPayload
                    {
                        TargetScreenId = "EMR_IN_00002",
                        Context = context,
                        Source = ProgramID
                    });

                    
                    Logger.Information($"환자 선택: {patientName} ({patientId})", ProgramID);
                }
            }
        }

        // 그리드에서 포커스된 행이 변경되었을 때 호출
        private void _gridView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (_gridView.GetFocusedRow() is DataRowView rowView)
            {
                // 데이터 테이블 컬럼명이 변경되어 있을 수 있으므로 안전하게 컬럼 존재 여부를 확인해서 사용
                var row = rowView.Row;
                string patientId = string.Empty;
                string patientName = string.Empty;

                if (row.Table.Columns.Contains("PatientId"))
                {
                    patientId = row["PatientId"]?.ToString() ?? string.Empty;
                }
                else if (row.Table.Columns.Contains("MODULE_ID"))
                {
                    // 레거시/다른 데이터 소스용 행동
                    patientId = row["MODULE_ID"]?.ToString() ?? string.Empty;
                }

                if (row.Table.Columns.Contains("PatientName"))
                {
                    patientName = row["PatientName"]?.ToString() ?? string.Empty;
                }
                else if (row.Table.Columns.Contains("MODULE_NAME"))
                {
                    // 레거시/다른 데이터 소스용 행동
                    patientName = row["MODULE_NAME"]?.ToString() ?? string.Empty;
                }

                if (!string.IsNullOrEmpty(patientId))
                {
                    // 강타입 이벤트 발행
                    var context = new PatientSelectedEventPayload() { Patient = new Models.PatientInfoDto() { PatientId = patientId, PatientName = patientName } } ;
                    EventBus?.GetEvent<Core.Events.PatientSelectedEvent>().Publish(context);

                    Logger.Information($"환자 선택: {patientName} ({patientId})", ProgramID);
                }
            }
        }

        public override void OnActivated()
        {
            base.OnActivated();
            Console.WriteLine("[OT Worklist] 활성화됨");
        }

        // 디자이너 호환을 위한 이벤트 핸들러
        private async void _btnSearch_Click1(object sender, EventArgs e)
        {
            await SearchAsync();
        }

        // 데이터 조회 수행 (비동기)
        private async Task SearchAsync()
        {
            try
            {
                // 디자이너 모드이거나 로직이 주입되지 않은 경우 안전하게 중지
                if (_logic == null)
                {
                    if (!this.DesignMode)
                        MessageBox.Show("비즈니스 로직이 주입되지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _btnSearch.Enabled = false;
                _lblTitle.Text = "데이터를 조회하는 중...";

                // 비즈니스 로직에 위임하여 데이터 조회
                var dt = await _logic.SearchWorklistAsync();
                
                _gridControl.DataSource = dt;
                
                MessageBox.Show($"조회 완료!\n\n데이터 건수: {dt.Rows.Count}", 
                    "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnSearch.Enabled = true;
                _lblTitle.Text = "수술(OT) 워크리스트 - DI 및 비즈니스 로직 분리 테스트";
            }
        }
    }
}