# 연결성 및 인프라 마스터 문서 (Connectivity & Infrastructure Master Document)

**버전:** 1.0 (통합본)
**날짜:** 2026-02-10
**컨텍스트:** nU3.Framework (연결성, 데이터, 서버)

---

## 1. 개요

**연결성 계층 (Connectivity Layer)**은 REST API를 통해 모든 외부 통신(데이터베이스, 파일, 로그)을 추상화합니다.
*   **중앙 집중점:** `ConnectivityManager` (싱글톤).
*   **프로토콜:** HTTP/1.1 (또는 2.0) 및 JSON/BSON.
*   **보안:** JWT Bearer 토큰이 자동으로 주입됩니다.

---

## 2. 연결성 관리자 (`nU3.Core`)

**역할:** HTTP 클라이언트의 수명 주기, 연결 풀링 및 상태 확인을 관리합니다.

### 주요 기능
1.  **싱글톤 접근:** `ConnectivityManager.Instance.DB`, `.File`, `.Log`.
2.  **연결 풀링:** 병렬 고성능 작업을 위한 `CreateDBClientAsync()`.
3.  **일괄 작업:** `ExecuteBatchQueriesAsync`, `UploadFilesAsync` (세마포어 제어).
4.  **상태 확인:** `TestAllConnectionsAsync()`를 통해 DB, 파일, 로그 서비스를 검증합니다.
5.  **로그 압축:** 활성화된 경우 로그 업로드 시 GZip 자동 압축.

### 사용법
```csharp
// 초기화
ConnectivityManager.Instance.Initialize("https://api.nu3.com", true);

// DB 쿼리
var dt = await ConnectivityManager.Instance.DB.ExecuteDataTableAsync("SELECT * FROM USERS");

// 파일 업로드
await ConnectivityManager.Instance.File.UploadFileAsync("local.txt", "server/path.txt");
```

---

## 3. 클라이언트 구현체 (`nU3.Connectivity`)

### `HttpDBAccessClient`
*   **엔드포인트:** `/api/v1/db/query/table`, `/api/v1/db/procedure` 등.
*   **기능:**
    *   HTTP를 통한 SQL/프로시저 실행.
    *   `DataTable`, `DataSet` 또는 스칼라 반환.
    *   트랜잭션 처리 (`Begin`, `Commit`, `Rollback`).

### `HttpFileTransferClient`
*   **엔드포인트:** `/api/v1/files/upload`, `/api/v1/files/download`.
*   **기능:**
    *   Multipart/form-data 업로드.
    *   디렉토리 관리 (생성, 삭제, 목록).
    *   이어받기 기능 (예정).

### `HttpLogUploadClient`
*   **엔드포인트:** `/api/log/upload`.
*   **기능:**
    *   GZip 자동 압축 (>1KB).
    *   대기 중인 로그 일괄 업로드.
    *   크래시 로그 즉시 업로드.

---

## 4. 로컬 데이터베이스 (`nU3.Data`)

**역할:** 오프라인 기능 및 메타데이터 캐싱.
*   **엔진:** SQLite (`nU3_Local.db`).
*   **경로:** `%AppData%\nU3.Framework\Database\`.
*   **스키마:** `SYS_MENU`, `SYS_MODULE_MST`, `SYS_USER` (세션).
*   **관리자:** `LocalDatabaseManager` (`IDBAccessService`를 구현하여 호환성 유지).

---

## 5. 서버 구성 (`nU3.Server.Host`)

**파일:** `appsettings.json`

### 주요 섹션
```json
{
  "ServerSettings": {
    "FileTransfer": {
      "HomeDirectory": "C:\\Storage",
      "AllowedExtensions": [ ".pdf", ".png", ".dll" ]
    },
    "LogUpload": {
      "LogStoragePath": "C:\\Logs\\Client",
      "EnableCompression": true
    },
    "Database": {
      "CommandTimeout": 30
    }
  },
  "Swagger": { "Enabled": true } // 개발 환경 전용
}
```

---

## 6. 구현 상태 (Phase 2)

*   **완료됨:**
    *   모든 HTTP 클라이언트 (`DB`, `File`, `Log`).
    *   풀링 및 배치를 지원하는 `ConnectivityManager`.
    *   로컬 SQLite 통합.
    *   서버 `appsettings.json` 구조.
    *   DevExpress UI 통합 (기본 클래스에서 연결성 사용).

*   **다음 단계:**
    *   실시간 알림 (SignalR).
    *   고급 오프라인 동기화.