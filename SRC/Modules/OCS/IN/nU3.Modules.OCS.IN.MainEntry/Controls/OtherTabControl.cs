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
    public partial class OtherTabControl : UserControl
    {
        public OtherTabControl()
        {
            InitializeComponent();
        }

        public RefCodeType RefCodeType { get; set; } = RefCodeType.REP;

        public event EventHandler<RefCodeSelectedEventArgs> RefCodeSelected;

        private void btnRep_Click(object sender, EventArgs e)
        {
            RefCodeType = RefCodeType.REP;
            OnRefCodeSelected(RefCodeType.REP);
        }

        private void btnSheet_Click(object sender, EventArgs e)
        {
            RefCodeType = RefCodeType.SHT;
            OnRefCodeSelected(RefCodeType.SHT);
        }

        private void btnEtc_Click(object sender, EventArgs e)
        {
            RefCodeType = RefCodeType.ETC;
            OnRefCodeSelected(RefCodeType.ETC);
        }

        protected virtual void OnRefCodeSelected(RefCodeType refCodeType)
        {
            RefCodeSelected?.Invoke(this, new RefCodeSelectedEventArgs(refCodeType));
        }

        public void SetRefCode(RefCodeType refCodeType)
        {
            RefCodeType = refCodeType;
            
            // 버튼 선택 상태 변경
            btnRep.Appearance.ForeColor = (refCodeType == RefCodeType.REP) ? 
                System.Drawing.Color.Blue : System.Drawing.SystemColors.ControlText;
            btnSheet.Appearance.ForeColor = (refCodeType == RefCodeType.SHT) ? 
                System.Drawing.Color.Blue : System.Drawing.SystemColors.ControlText;
            btnEtc.Appearance.ForeColor = (refCodeType == RefCodeType.ETC) ? 
                System.Drawing.Color.Blue : System.Drawing.SystemColors.ControlText;
        }
    }

    public class RefCodeSelectedEventArgs : EventArgs
    {
        public RefCodeType SelectedTab { get; }

        public RefCodeSelectedEventArgs(RefCodeType selectedTab)
        {
            SelectedTab = selectedTab;
        }
    }
}