using nU3.Core.Context;

namespace nU3.Core.Interfaces
{       
     /// <summary>
    /// 모든 MDI 자식 폼(작업 화면)의 기본 계약
    /// 여러 인터페이스를 조합하여 완전한 작업 화면 기능 제공
    /// </summary>
    /// <remarks>
    /// IBaseWorkControl 다음 인터페이스들을 조합합니다:    
    /// - ILifecycleAware: 생명주기 관리
    /// - IWorkContextProvider: 작업 컨텍스트 관리
    /// - IResourceManager: 리소스 정리    
    /// </remarks>

    public interface IBaseWorkControlExpand : ILifecycleAware, IWorkContextProvider, IResourceManager
    {

    }
}
