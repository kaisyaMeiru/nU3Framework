using DevExpress.XtraEditors;
using nU3.Core.Attributes;
using nU3.Core.Context;
using nU3.Core.Events;
using nU3.Core.UI;
using nU3.Core.UI.Components.Events;
using nU3.Core.UI.Controls;
using nU3.Models;
using nU3.Modules.OCS.IN.MainEntry.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nU3.Modules.OCS.IN.MainEntry
{
    /// <summary>
    /// OCS 입원 처방 메인 화면    
    /// </summary>
    [nU3ProgramInfo(typeof(OCSINPOrderMainControl), "OCS 입원 처방", "OCS_IN_001")]
    public partial class OCSINPOrderMainControl : BaseWorkControl
    {
        #region 1. 생성자를 정의한다
        public OCSINPOrderMainControl()
        {
            InitializeComponent();

            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Runtime)
            {
                LogInfo("OCSINPOrderMainControl 생성됨");
            }
        }
        #endregion

        #region 2. Form 변수를 정의한다
        
        private string mInNumber = string.Empty;
        private string mPatiNumber = string.Empty;
        private string mOrdDate = string.Empty;
        private string mAdmDate = string.Empty;
        private string mDoctorID = string.Empty;
        private string mDeptID = string.Empty;

        #endregion

        #region 3. 초기화 메서드를 정의한다

        public override void InitializeContext(WorkContext context)
        {
            base.InitializeContext(context);

            if (EventBus == null)
                LogWarning("EventBus가 할당되지 않았습니다.");
        }

        private void EnableEventListening()
        {
            // 이벤트 수신 설정 (데모에서는 구현하지 않음)
        }

        protected override void OnScreenActivated()
        {
            base.OnScreenActivated();

            // 화면 활성화 시 초기화 로직
            LoadInitialData();

            // 환자 선택 이벤트 핸들러 연결
            if(PatientListControl != null)            
                PatientListControl.PatientSelected += OnPatientSelected;

            // OtherTabControl과 OtherOrderControl 탭 동기화 이벤트 연결
            if (OtherTabControl != null)
                OtherTabControl.RefCodeSelected += OtherTabControl_RefCodeSelected;

            if (OtherOrderControl != null)
                OtherOrderControl.TabChanged += OtherOrderControl_TabChanged;

            AddEventLog("OCS 입원 처방 화면 활성화");
        }

        protected override void OnScreenDeactivated()
        {
            base.OnScreenDeactivated();

            // 환자 선택 이벤트 핸들러 해제
            if (PatientListControl != null)
                PatientListControl.PatientSelected -= OnPatientSelected;

            // 탭 동기화 이벤트 핸들러 해제
            if (OtherTabControl != null)
                OtherTabControl.RefCodeSelected -= OtherTabControl_RefCodeSelected;

            if (OtherOrderControl != null)
                OtherOrderControl.TabChanged -= OtherOrderControl_TabChanged;

            AddEventLog("OCS 입원 처방 화면 비활성화");
        }

        private void LoadInitialData()
        {
            // 초기 데이터 로딩
            // Context 및 CurrentUser에 대한 방어적 프로그래밍
            if (Context?.CurrentUser != null)
            {
                mDoctorID = Context.CurrentUser.UserId;
                mDeptID = Context.CurrentUser.DepartmentCode?.ToString() ?? string.Empty;
            }
            else
            {
                // 디자인 모드거나 컨텍스트 미초기화 시 기본값
                mDoctorID = string.Empty;
                mDeptID = string.Empty;
            }

            // 처방일자 설정
            if (dtpOrdDate != null)
                dtpOrdDate.EditValue = DateTime.Now;

            // 처방 타입 콤보박스 설정
            SetOrderTypeSelect();

            // 기타 초기화
            if (tabSupport != null)
                tabSupport.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;

            // 화면 로딩 후 포커스 설정
            PostLoad();
        }

        private void PostLoad()
        {
            if (cboOrderType != null)
                cboOrderType.Focus();
        }

        private void OnPatientSelected(object sender, PatientSelectedEventArgs e)
        {
            // 환자가 선택되면 관련 정보를 로드
            if (PatientListControl != null && PatientListControl.SelectedInNumber != null)
            {
                mInNumber = PatientListControl.SelectedInNumber;
                mPatiNumber = PatientListControl.SelectedPatiNumber;
                mAdmDate = PatientListControl.SelectedAdmDate.ToString("yyyyMMdd");

                // 환자 배너 정보 업데이트 (실제 선택된 환자 데이터 사용)
                if (PatientInfoControl != null)
                {
                    PatientInfoControl.SetPatientInfo(PatientListControl.GetSelectedPatient());                        
                }

                // 처방정보 조회
                string lRecDate = dtpOrdDate?.EditValue?.ToString()?.Replace("-", "") ?? DateTime.Now.ToString("yyyyMMdd");
                GetOrderQueryLayout(mInNumber, lRecDate);
            }
        }

        /// <summary>
        /// OtherTabControl에서 탭 버튼 클릭 시 OtherOrderControl의 탭 동기화
        /// </summary>
        private void OtherTabControl_RefCodeSelected(object sender, RefCodeSelectedEventArgs e)
        {
            if (OtherOrderControl != null)
            {
                OtherOrderControl.SetRefCodeSelect(e.SelectedTab);
            }
        }

        /// <summary>
        /// OtherOrderControl에서 탭 변경 시 OtherTabControl의 버튼 상태 동기화
        /// </summary>
        private void OtherOrderControl_TabChanged(object sender, RefCodeType refCodeType)
        {
            if (OtherTabControl != null)
            {
                OtherTabControl.SetRefCode(refCodeType);
            }
        }
        #endregion

        #region 4. 메인 버튼 이벤트를 정의한다

        private void OnMainButtonClick(object sender, EventArgs e)
        {
            string lErrMessage = string.Empty;
            string lRecStatus = string.Empty;

            switch (((SimpleButton)sender).Name.ToString().Trim())
            {
                case "btnConfirm":
                    lRecStatus = "5";

                    if (!PreSaveDataChecking(ref lErrMessage))
                    {
                        ShowMessageBox(lErrMessage, "처방완료");
                        return;
                    }

                    if (!PreSaveSelectedChecking())
                        return;

                    if (!SetDataSave(ref lErrMessage, lRecStatus))
                    {
                        ShowMessageBox(lErrMessage, "처방완료");
                        return;
                    }

                    ShowMessageBox("처방완료 되었습니다.", "처방완료");

                    // 데이터 새로고침
                    RefreshOrderData();
                    break;

                case "btnHolding":
                    lRecStatus = "4";

                    if (!PreSaveDataChecking(ref lErrMessage))
                    {
                        ShowMessageBox(lErrMessage, "처방보류");
                        return;
                    }

                    if (!SetDataSave(ref lErrMessage, lRecStatus))
                    {
                        ShowMessageBox(lErrMessage, "처방보류");
                        return;
                    }

                    ShowMessageBox("처방보류 되었습니다.", "처방보류");
                    break;

                case "btnDelete":
                    DeleteOrder();
                    break;

                case "btnSender":
                    // 전자서명 로직
                    break;
            }
        }

        private void OnTopMainButtonClick(object sender, EventArgs e)
        {
            switch (((SimpleButton)sender).Name.ToString().Trim())
            {
                case "btnOpHistory":
                    ShowPatientOpHistory();
                    break;
                case "btnFamilyHistory":
                    ShowPatientFamilyHistory();
                    break;
                case "btnPastHistory":
                    ShowPatientPastHistory();
                    break;
            }
        }

        #endregion


        private bool GetOrderQueryLayout(string InNumber, string lRecDate)
        {
            this.mOrdDate = lRecDate;

            // 1. 환자정보를 조회한다
            PatientInfoControl?.LoadPatientInfo(InNumber);

            // 2. 문제리스트를 조회한다
            ProblemListControl?.LoadProblemList(InNumber, this.mOrdDate);

            // 3. 진단코드를 조회한다
            DiagCodeControl?.LoadDiagCode(InNumber, this.mOrdDate);

            // 4. 처방코드를 조회한다
            OrderCodeControl?.LoadOrderCode(InNumber, this.mOrdDate);

            // 5. 기타처방를 조회한다
            OtherOrderControl?.LoadOtherOrder(this.mPatiNumber, this.mDeptID, this.mDoctorID);

            // 6. 전달메모를 조회한다
            SendMemoControl?.LoadMemo(this.mInNumber, this.mPatiNumber, this.mAdmDate, this.mOrdDate, null);

            return true;
        }

        private bool PreSaveDataChecking(ref string lErrMessage)
        {
            // 환자정보가 있는지 여부를 판단한다
            if (string.IsNullOrWhiteSpace(this.mPatiNumber))
            {
                lErrMessage = "환자를 선택해 주십시오.";
                return false;
            }

            return true;
        }

        private bool PreSaveSelectedChecking()
        {
            string lCurDate = DateTime.Now.ToString("yyyyMMdd");
            string lOrdDate = this.dtpOrdDate?.EditValue?.ToString()?.Replace("-", "") ?? string.Empty;

            if (lCurDate != lOrdDate)
            {
                DialogResult result = ShowMessageBox("처방일자와 현재일자가 일치하지 않습니다.\r\n그래도 저장하시겠습니까?",
                    "처방완료", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    return false;
            }

            // 진단코드가 있는지 확인
            if (DiagCodeControl != null && !DiagCodeControl.HasDiagCode())
            {
                DialogResult result = ShowMessageBox("입력된 진단코드가 존재하지 않습니다.\r\n그래도 저장하시겠습니까?",
                    "처방완료", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    return false;
            }

            // 처방코드가 있는지 확인
            if (OrderCodeControl != null && !OrderCodeControl.HasOrderCode())
            {
                DialogResult result = ShowMessageBox("입력된 처방코드가 존재하지 않습니다.\r\n그래도 저장하시겠습니까?",
                    "처방완료", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    return false;
            }

            return true;
        }

        private bool SetDataSave(ref string lErrMessage, string lStatusCode)
        {
            try
            {                
                // 처방코드 저장
                if (OrderCodeControl != null && !OrderCodeControl.SaveData(this.mInNumber, this.mOrdDate))
                {
                    lErrMessage = "처방코드 저장하는데 실패하였습니다.";
                    return false;
                }

                // 진단코드 저장
                if (DiagCodeControl != null && !DiagCodeControl.SaveData(this.mInNumber, this.mOrdDate))
                {
                    lErrMessage = "진단코드 저장하는데 실패하였습니다.";
                    return false;
                }

                // 문제리스트 저장
                if (ProblemListControl != null && !ProblemListControl.SaveData(this.mInNumber, this.mOrdDate))
                {
                    lErrMessage = "문제리스트 저장하는데 실패하였습니다.";
                    return false;
                }

                // 전달메모 저장
                if (SendMemoControl != null && !SendMemoControl.SaveData())
                {
                    lErrMessage = "전달메모 저장하는데 실패하였습니다.";
                    return false;
                }

                
                return true;
            }
            catch (Exception ex)
            {
                lErrMessage = $"저장 중 오류가 발생했습니다: {ex.Message}";
                LogError("저장 중 오류 발생", ex);
                return false;
            }
        }

        private void DataReset()
        {
            PatientInfoControl?.Reset();
            ProblemListControl?.Reset();
            DiagCodeControl?.Reset();
            OrderCodeControl?.Reset();
            SendMemoControl?.Reset();
            OtherOrderControl?.Reset();

            if (OtherTabControl != null && OtherTabControl.RefCodeType != RefCodeType.SHT)
                OtherTabControl.SetRefCode(RefCodeType.REP);
        }

        private void SetOrderTypeSelect()
        {
            if (cboOrderType == null) return;

            cboOrderType.Properties.Items.Clear();

            // 데모 데이터 - 실제로는 DB에서 조회
            cboOrderType.Properties.Items.Add("약제처방");
            cboOrderType.Properties.Items.Add("주사처방");
            cboOrderType.Properties.Items.Add("외래처방");
            cboOrderType.Properties.Items.Add("검사처방");
            cboOrderType.Properties.Items.Add("방사선처방");
            cboOrderType.Properties.Items.Add("물리치료");
            cboOrderType.Properties.Items.Add("간호처방");
            cboOrderType.Properties.Items.Add("수술처방");

            if (cboOrderType.Properties.Items.Count > 0)
                cboOrderType.SelectedIndex = 0;
        }

        private void RefreshOrderData()
        {
            string lRecDate = this.dtpOrdDate?.EditValue?.ToString()?.Replace("-", "") ?? string.Empty;

            // 환자리스트 새로고침
            //PatientListControl?.QueryLayout(lRecDate, mDoctorID, PatientListControl.WaitType);
        }

        private void DeleteOrder()
        {
            OrderCodeControl?.DeleteOrder();
        }

        private void ShowPatientOpHistory()
        {
            ShowMessageBox("수술기록 팝업", "환자정보");
        }

        private void ShowPatientFamilyHistory()
        {
            ShowMessageBox("가족력 팝업", "환자정보");
        }

        private void ShowPatientPastHistory()
        {
            ShowMessageBox("과거력 팝업", "환자정보");
        }

        private DialogResult ShowMessageBox(string message, string caption)
        {
            return MessageBox.Show(this, message, caption, MessageBoxButtons.OK);
        }

        private DialogResult ShowMessageBox(string message, string caption, MessageBoxButtons buttons)
        {
            return MessageBox.Show(this, message, caption, buttons);
        }

        private void AddEventLog(string message)
        {
            LogInfo(message);
        }

    }
}