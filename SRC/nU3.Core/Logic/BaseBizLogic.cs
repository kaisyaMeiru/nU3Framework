using System;
using System.Data;
using System.Threading.Tasks;
using nU3.Core.Interfaces;

namespace nU3.Core.Logic
{
    /// <summary>
    /// 비즈니스 로직 모듈의 공통 베이스 클래스입니다.
    /// 
    /// 목적:
    /// - UI(화면)와 데이터 접근(DB/File) 로직을 명확히 분리하여 관심사 분리를 구현합니다.
    /// - 공통으로 사용되는 DB/File 서비스 참조를 보관하고 하위 로직에서 재사용하도록 제공합니다.
    /// - 트랜잭션 처리, 표준 예외 처리, 로깅 래퍼 같은 공통 헬퍼를 배치하기 위한 확장 포인트를 제공합니다.
    /// 
    /// 사용법:
    /// - 각 화면(또는 모듈)별 비즈니스 로직 클래스는 이 클래스를 상속받아 구현합니다.
    /// - DI 컨테이너에서 IDBAccessService, IFileTransferService를 주입받아 생성합니다.
    /// 
    /// 예시:
    /// public class PatientBizLogic : BaseBizLogic
    /// {
    ///     public PatientBizLogic(IDBAccessService db, IFileTransferService file) : base(db, file) { }
    ///     public DataTable GetPatientList() => _db.ExecuteDataTable("SELECT * FROM PATIENTS");
    /// }
    /// 
    /// 주의사항:
    /// - 이 클래스는 상태를 거의 가지지 않도록 설계되어야 하며, 멀티스레드 환경에서는 주입된 서비스의 스레드 안전성을 신경써야 합니다.
    /// - 긴 시간 동안 유지되는 리소스를 보유하지 않도록 하고, 필요 시 IDisposable 패턴을 하위 클래스에서 구현하세요.
    /// </summary>
    public abstract class BaseBizLogic
    {
        /// <summary>
        /// 데이터베이스 접근 서비스. 동기/비동기 쿼리/명령 실행에 사용합니다.
        /// </summary>
        protected readonly IDBAccessService _db;

        /// <summary>
        /// 파일 전송 서비스. 파일 업로드/다운로드 및 디렉터리 작업에 사용합니다.
        /// </summary>
        protected readonly IFileTransferService _file;

        /// <summary>
        /// 생성자: 하위 클래스는 DI로 주입된 DB/File 서비스를 전달받아 사용합니다.
        /// </summary>
        /// <param name="db">데이터베이스 접근 서비스 인스턴스</param>
        /// <param name="file">파일 전송 서비스 인스턴스</param>
        protected BaseBizLogic(IDBAccessService db, IFileTransferService file)
        {
            _db = db;
            _file = file;
        }

        // 공통 헬퍼 메서드를 이곳에 추가할 수 있습니다.
        // 예: 표준 예외 처리 래퍼, 트랜잭션 실행 유틸리티, 로깅 래퍼 등
    }
}
