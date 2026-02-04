namespace nU3.Core.Interfaces
{
    /// <summary>
    /// 리소스 관리 인터페이스
    /// 동적 로드/언로드를 지원하는 모듈의 리소스 정리를 담당
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// 리소스 정리
        /// 모듈이 언로드될 때 자동으로 호출됨
        /// </summary>
        /// <remarks>
        /// 이 메서드에서 다음을 정리해야 합니다:
        /// - 타이머 정지 및 해제
        /// - 데이터베이스 연결 종료
        /// - 파일 핸들 닫기
        /// - 이벤트 구독 해제
        /// - 진행 중인 비동기 작업 취소
        /// - 기타 관리되지 않는 리소스
        /// </remarks>
        void ReleaseResources();
    }
}
