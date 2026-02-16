using System;
using System.Windows.Forms;
using nU3.Core.UI.Controls.Forms;
using nU3.Core.Interfaces;
using nU3.Core.Events;

namespace nU3.Core.UI
{
    /// <summary>
    /// 병원 팝업 대화상자의 기본 클래스입니다.
    /// DevExpress.XtraEditors.XtraForm을 상속받는 nU3Form을 기반으로 합니다.
    /// </summary>
    public class BaseWorkForm : nU3Form, IBaseWorkComponent
    {
        public BaseWorkForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.AutoScaleMode = AutoScaleMode.Font;
        }

        /// <summary>
        /// 이 폼을 띄운 원본 컨트롤 (부모 컨텍스트 역할)
        /// 팝업 내에서 BaseWorkComponent가 HostProgramID나 EventBus를 찾을 때 사용됩니다.
        /// </summary>
        public BaseWorkControl SourceControl { get; set; }

        #region IBaseWorkComponent Implementation
        
        /// <summary>
        /// SourceControl의 EventBus를 반환합니다.
        /// SourceControl이 없으면 null을 반환합니다.
        /// </summary>
        public virtual IEventAggregator OwnerEventBus => SourceControl?.EventBus;

        /// <summary>
        /// SourceControl의 ProgramID를 반환합니다.
        /// SourceControl이 없으면 null을 반환합니다.
        /// </summary>
        public virtual string OwnerProgramID => SourceControl?.ProgramID;

        #endregion
    }
}