using System;
using nU3.Core.Context;
using nU3.Models;

namespace nU3.Core.UI.Shell
{
    /// <summary>
    /// 모듈을 호스팅하는 셸 폼을 위한 인터페이스입니다.
    /// </summary>
    public interface IShellForm
    {
        /// <summary>
        /// 셸이 초기화되었는지 여부를 가져옵니다.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// 셸 이름/식별자를 가져옵니다.
        /// </summary>
        string ShellName { get; }

        /// <summary>
        /// ProgId로 프로그램/모듈을 엽니다.
        /// </summary>
        void OpenProgram(string progId, string displayName = null);

        /// <summary>
        /// ProgId로 프로그램/모듈을 닫습니다.
        /// </summary>
        void CloseProgram(string progId, bool force = false);

        /// <summary>
        /// 열린 모든 모듈에 컨텍스트 변경을 브로드캐스트합니다.
        /// </summary>
        void BroadcastContextChange(WorkContext context);

        /// <summary>
        /// 환자를 선택하고 모든 모듈에 브로드캐스트합니다.
        /// </summary>
        void SelectPatient(PatientInfoDto patient);

        /// <summary>
        /// 검사(Exam)를 선택하고 모든 모듈에 브로드캐스트합니다.
        /// </summary>
        void SelectExam(ExamOrderDto exam, PatientInfoDto patient);

        /// <summary>
        /// 열린 모듈의 개수를 가져옵니다.
        /// </summary>
        int OpenModuleCount { get; }

        /// <summary>
        /// 현재 활성 모듈에 대한 정보를 가져옵니다.
        /// </summary>
        string GetActiveModuleInfo();

        /// <summary>
        /// 상태 표시줄 메시지를 업데이트합니다.
        /// </summary>
        void UpdateStatusMessage(string message);

        /// <summary>
        /// 모듈이 열렸을 때 발생하는 이벤트입니다.
        /// </summary>
        event EventHandler<ModuleOpenedEventArgs> ModuleOpened;

        /// <summary>
        /// 모듈이 닫혔을 때 발생하는 이벤트입니다.
        /// </summary>
        event EventHandler<ModuleClosedEventArgs> ModuleClosed;
    }

    /// <summary>
    /// 모듈 열림 이벤트의 이벤트 인자입니다.
    /// </summary>
    public class ModuleOpenedEventArgs : EventArgs
    {
        public string ProgId { get; set; }
        public string DisplayName { get; set; }
        public DateTime OpenedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// 모듈 닫힘 이벤트의 이벤트 인자입니다.
    /// </summary>
    public class ModuleClosedEventArgs : EventArgs
    {
        public string ProgId { get; set; }
        public DateTime ClosedAt { get; set; } = DateTime.Now;
    }
}
