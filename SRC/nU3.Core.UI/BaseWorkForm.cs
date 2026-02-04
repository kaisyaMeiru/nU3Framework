using System;
using System.Windows.Forms;
using nU3.Core.UI.Controls.Forms;

namespace nU3.Core.UI
{
    /// <summary>
    /// 모든 팝업 대화상자의 기본 클래스입니다.
    /// TODO: Form에서 DevExpress.XtraEditors.XtraForm으로 상속 변경 검토
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
