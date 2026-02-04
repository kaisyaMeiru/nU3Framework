using nU3.Core.Context;

namespace nU3.Core.Interfaces
{
    /// <summary>
    /// 작업 컨텍스트 관리 인터페이스
    /// 모듈 간 데이터 전달 및 컨텍스트 관리를 담당
    /// </summary>
    public interface IWorkContextProvider
    {
        /// <summary>
        /// 작업 컨텍스트 초기화
        /// 모듈이 처음 로드될 때 호출됨
        /// </summary>
        /// <param name="context">초기 작업 컨텍스트</param>
        void InitializeContext(WorkContext context);

        /// <summary>
        /// 컨텍스트 업데이트
        /// 런타임 중 컨텍스트가 변경될 때 호출됨
        /// </summary>
        /// <param name="context">새로운 작업 컨텍스트</param>
        void UpdateContext(WorkContext context);

        /// <summary>
        /// 현재 작업 컨텍스트 가져오기
        /// </summary>
        /// <returns>현재 작업 컨텍스트의 복사본</returns>
        WorkContext GetContext();
    }
}
