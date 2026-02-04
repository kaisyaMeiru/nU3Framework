using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;

namespace nU3.Core.UI.Controls.Forms
{
    /// <summary>
    /// DevExpress Tabbed Form을 위한 nU3 전용 Form입니다.
    ///
    /// 설명: DevExpress TabForm(Tabbed Form) 기반 UI를 사용할 때 편의성을 제공합니다.
    ///       기본적으로는 XtraForm을 상속하지만, 필요시 TabForm 컨트롤과 통합할 수 있도록 자리표시자를 유지합니다.
    /// </summary>
    [ToolboxItem(true)]
    public class nU3TabForm : DevExpress.XtraBars.TabForm
    {
        // 향후 TabFormControl 통합을 위한 자리표시자입니다.
        public nU3TabForm()
        {
            StartPosition = FormStartPosition.CenterScreen;
        }
        private TabFormControl tabFormControl1;
        private TabFormPage tabFormPage1;
        private TabFormContentContainer tabFormContentContainer1;

        private void InitializeComponent()
        {

        }
    }
}
