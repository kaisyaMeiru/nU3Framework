using System.Windows.Forms;

namespace nU3.Core.UI.Interfaces
{
    /// <summary>
    /// 메인 셸이 내비게이션 서비스를 위해 제공해야 할 최소한의 기능 정의입니다.
    /// </summary>
    public interface IShellView
    {
        void ShowContent(Control content, string programId, string? displayName);
        bool IsProgramOpen(string programId);
        void ActivateProgram(string programId);
    }
}
