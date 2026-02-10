# 로컬 데이터베이스 관리 가이드 (nU3.Data)

## 1. 개요
`nU3.Data` 프로젝트는 클라이언트 사이드에서 필요한 메타데이터(메뉴, 사용자 세션, 모듈 버전 등)를 관리하기 위해 **SQLite** 기반의 로컬 데이터베이스를 제공합니다. 이는 서버 통신 장애 시에도 기본적인 화면 구성을 유지하고, 잦은 정보 조회의 성능을 향상시키기 위한 **Local-First** 전략의 핵심입니다.

## 2. 주요 클래스
- **LocalDatabaseManager**: `IDBAccessService`를 상속받아 SQLite에 대한 CRUD 기능을 제공합니다. 프레임워크 초기화 시 `InitializeSchema()`를 통해 필수 테이블을 생성합니다.
- **LocalDbService**: 실제 SQLite 연결 및 트랜잭션을 처리하는 하위 서비스입니다.

## 3. 데이터베이스 위치
로컬 데이터베이스 파일(`nU3_Local.db`)은 다음 경로에 생성됩니다:
`%AppData%
U3.Framework\Database
U3_Local.db`

## 4. 주요 테이블 스키마

### 4.1 시스템 및 보안
- **SYS_USER**: 로컬 로그인 정보 및 캐시된 사용자 정보
- **SYS_ROLE**: 사용자 역할 정의
- **SYS_DEPT**: 부서 정보
- **SYS_PERMISSION**: 프로그램별 상세 권한 (조회, 수정, 삭제 등)

### 4.2 모듈 및 메뉴
- **SYS_MODULE_MST**: 설치된 모듈 DLL 정보
- **SYS_PROG_MST**: `[nU3ProgramInfo]` 속성에서 스캔된 개별 화면 정보
- **SYS_MENU**: 화면과 연결된 계층형 메뉴 구조

## 5. 사용 예시

### 5.1 데이터베이스 초기화 (Deployer 또는 Bootstrapper)
```csharp
var localDb = new LocalDatabaseManager("nU3_Local.db");
localDb.InitializeSchema(); // 테이블이 없으면 생성
```

### 5.2 데이터 조회 (SQL 사용)
```csharp
var localDb = new LocalDatabaseManager();
var dt = localDb.ExecuteDataTable("SELECT * FROM SYS_MENU ORDER BY SORT_ORD");
```

## 6. 주의 사항
- **Stored Procedure 미지원**: SQLite 특성상 `ExecuteProcedure` 호출 시 `NotSupportedException`이 발생합니다. 복잡한 로직은 SQL 쿼리로 직접 작성해야 합니다.
- **날짜 포맷**: SQLite는 별도의 DateTime 타입을 지원하지 않으므로, 데이터 저장 시 ISO-8601 형식(`YYYY-MM-DD HH:MM:SS`)의 문자열을 사용합니다.
- **동시성**: 클라이언트 전용이므로 단일 프로세스 접근을 권장하며, 여러 스레드 접근 시 `BeginTransaction()`을 활용하여 데이터 일관성을 유지하십시오.

---
(c) 2026 nU3 Framework
