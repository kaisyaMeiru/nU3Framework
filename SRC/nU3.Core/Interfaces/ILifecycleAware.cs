namespace nU3.Core.Interfaces
{
    /// <summary>
    /// 생명주기 관리 인터페이스
    /// 화면의 활성화/비활성화 상태 관리를 담당
    /// </summary>
    public interface ILifecycleAware
    {
        /// <summary>
        /// 화면이 활성화될 때 호출됨
        /// MDI 컨테이너에서 포커스를 받을 때 발생
        /// </summary>
        /// <remarks>
        /// 이 메서드에서 다음을 수행할 수 있습니다:
        /// - 데이터 갱신
        /// - 타이머 시작
        /// - 이벤트 구독
        /// - UI 업데이트
        /// </remarks>
        void OnActivated();

        /// <summary>
        /// 화면이 비활성화될 때 호출됨
        /// MDI 컨테이너에서 포커스를 잃을 때 발생
        /// </summary>
        /// <remarks>
        /// 이 메서드에서 다음을 수행할 수 있습니다:
        /// - 타이머 정지
        /// - 임시 데이터 저장
        /// - 리소스 절약을 위한 정리
        /// </remarks>
        void OnDeactivated();

        /// <summary>
        /// 화면이 닫히기 전에 호출됨
        /// 닫기를 취소하려면 false를 반환
        /// </summary>
        /// <returns>닫기 허용 여부 (true: 허용, false: 취소)</returns>
        /// <remarks>
        /// 이 메서드에서 다음을 확인할 수 있습니다:
        /// - 저장하지 않은 변경사항
        /// - 진행 중인 작업
        /// - 사용자 확인 필요 여부
        /// </remarks>
        bool CanClose();
    }
}
