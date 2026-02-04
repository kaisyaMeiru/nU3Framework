using System;

namespace nU3.Core.Security
{
    /// <summary>
    /// 애플리케이션 전역에서 현재 로그인된 사용자의 세션(컨텍스트)을 보관하는 싱글톤 클래스입니다.
    /// 
    /// 역할 및 사용처:
    /// - 현재 사용자 ID, 사용자 이름, 소속 부서 코드, 권한 레벨 등 세션 관련 정보를 보관합니다.
    /// - 여러 모듈이나 서비스에서 현재 사용자의 정보를 조회할 때 UserSession.Current를 통해 접근합니다.
    /// - UI에서 현재 로그인 사용자 표시, 권한 체크, 감사(로그) 등에 활용됩니다.
    /// 
    /// 구현 세부사항:
    /// - 싱글톤 패턴을 사용하며, 간단한 lazy 초기화를 통해 인스턴스를 생성합니다.
    /// - 이 구현은 멀티스레드 환경에서 간단한 사용에 적합합니다. (Current 프로퍼티에서 null 병합 연산자를 사용한 지연 초기화)
    ///   그러나 SetSession/Clear 메서드는 내부적으로 동기화 처리를 하지 않으므로 복잡한 멀티스레드 시나리오에서는 외부 동기화가 필요합니다.
    /// - 상태를 메모리에만 보관하므로 애플리케이션 종료 시 자동으로 소멸됩니다. 영속화가 필요하면 별도의 스토리지에 저장해야 합니다.
    /// 
    /// 보안 및 주의사항:
    /// - 세션 객체에는 민감한 정보(예: 비밀번호 등)를 저장하지 마십시오.
    /// - UserSession.Current는 프로세스 전역 단일 인스턴스이므로, 테스트에서는 Clear 또는 재설정을 고려하세요.
    /// - 웹 애플리케이션과 같은 멀티유저 환경에서는 이 싱글톤 패턴이 적절하지 않습니다(사용자별 세션이 필요함). 이 코드는 데스크톱/클라이언트 애플리케이션을 가정합니다.
    /// 
    /// 예제:
    /// // 로그인 성공 시
    /// UserSession.Current.SetSession();
    ///
    /// // 현재 사용자 확인
    /// if (UserSession.Current.IsLoggedIn)
    /// {
    ///     Console.WriteLine($"Current user: {UserSession.Current.UserName}");
    /// }
    ///
    /// // 로그아웃 시
    /// UserSession.Current.Clear();
    /// </summary>
    public class UserSession
    {
        // 내부 싱글톤 인스턴스
        private static UserSession _instance;

        /// <summary>
        /// 전역(앱 전체)에서 접근 가능한 현재 세션 인스턴스를 반환합니다.
        /// 인스턴스가 없으면 지연 초기화로 생성합니다.
        /// </summary>
        public static UserSession Current => _instance ??= new UserSession();

        /// <summary>
        /// 로그인된 사용자의 고유 ID
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// 로그인된 사용자의 표시 이름
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// 사용자가 속한 부서 코드(예: 진료과 코드)
        /// </summary>
        public string DeptCode { get; private set; }

        /// <summary>
        /// 사용자의 권한 레벨(숫자 기반). 권한 체크에 사용됩니다.
        /// </summary>
        public int AuthLevel { get; private set; }

        /// <summary>
        /// 현재 세션이 로그인된 상태인지 여부를 반환합니다.
        /// UserId가 null 또는 빈 문자열이 아니면 true를 반환합니다.
        /// </summary>
        public bool IsLoggedIn => !string.IsNullOrEmpty(UserId);

        // 생성자는 외부에서 인스턴스를 생성하지 못하도록 private으로 제한
        private UserSession() { }

        /// <summary>
        /// 현재 세션 정보를 설정합니다. 로그인 처리 후 호출하여 사용자 정보를 초기화합니다.
        /// 이 메서드는 기존 세션 값을 덮어씁니다.
        /// </summary>
        /// <param name="userId">사용자 ID (필수)</param>
        /// <param name="userName">사용자 표시 이름</param>
        /// <param name="deptCode">부서 코드</param>
        /// <param name="authLevel">권한 레벨(정수)</param>
        public void SetSession(string userId, string userName, string deptCode, int authLevel)
        {
            UserId = userId;
            UserName = userName;
            DeptCode = deptCode;
            AuthLevel = authLevel;
        }

        /// <summary>
        /// 세션을 초기화(비우기)합니다. 로그아웃 시 호출하여 모든 사용자 관련 정보를 제거합니다.
        /// </summary>
        public void Clear()
        {
            UserId = null;
            UserName = null;
            DeptCode = null;
            AuthLevel = 0;
        }
    }
}
