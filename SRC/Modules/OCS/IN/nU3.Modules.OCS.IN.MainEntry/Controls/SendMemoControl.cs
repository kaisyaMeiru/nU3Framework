using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using nU3.Core.UI;

namespace nU3.Modules.OCS.IN.MainEntry.Controls
{
    public partial class SendMemoControl : UserControl
    {
        public SendMemoControl()
        {
            InitializeComponent();
        }

        public void LoadMemo(string inNumber, string patiNumber, string admDate, string ordDate, object inOutMode)
        {
            // 실제로는 DB에서 해당 환자의 전달메모를 조회
            // 데모에서는 기본 메시지 설정
            if (memoEdit != null)
                memoEdit.Text = "[환자 전달 메모]\r\n\r\n환자 상태: 안정\r\n특이사항: 없음\r\n추가 검사: 필요";
        }

        public bool SaveData()
        {
            // 실제로는 DB에 저장
            // 데모에서는 항상 성공으로 처리
            return true;
        }

        public void Reset()
        {
            if (memoEdit != null)
                memoEdit.Text = string.Empty;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (memoEdit != null)
                memoEdit.Text = string.Empty;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveData())
            {
                XtraMessageBox.Show("메모가 저장되었습니다.", "전달메모", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                XtraMessageBox.Show("메모 저장에 실패했습니다.", "전달메모", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMemoResv_Click(object sender, EventArgs e)
        {
            // 메모 예약문구 팝업
            XtraMessageBox.Show("메모 예약문구 팝업", "예약문구", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}