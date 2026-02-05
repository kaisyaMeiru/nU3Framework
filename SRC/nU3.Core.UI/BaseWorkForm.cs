using System;
using System.Windows.Forms;
using nU3.Core.UI.Controls.Forms;

namespace nU3.Core.UI
{
    /// <summary>
    /// 병원 팝업 대화상자의 기본 클래스입니다.
    /// DevExpress.XtraEditors.XtraForm을 상속받는 nU3Form을 기반으로 합니다.
    /// </summary>
    public class BaseWorkForm : nU3Form
    {
        public BaseWorkForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.AutoScaleMode = AutoScaleMode.Font;
        }
    }
}