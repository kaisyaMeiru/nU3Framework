# nU3 Framework - 파일 로깅 및 서버 오딧 시스템

## 개요

nU3 Framework에 프로덕션급 파일 로깅 및 서버 오딧 기능이 추가되었습니다.
LOG 디렉토리에 PC명_IP_날짜별로 로그 파일이 자동 생성되며, 에러 발생 시 자동으로 서버에 업로드됩니다.

## 주요 기능

### 1. 파일 로깅 시스템

#### 로그 파일 생성 규칙
- **파일명 형식**: `{PC명}_{IP주소}_{날짜}.log`
- **예시**: `DESKTOP-ABC123_192.168.1.100_20241231.log`
- **저장 위치**: `%AppData%\nU3.Framework\LOG\`
- **자동 날짜 롤오버**: 날짜가 바뀌면 새 파일 자동 생성

#### 로그 레벨
```csharp
LogLevel.Trace        // 상세한 디버그 정보
LogLevel.Debug        // 디버그 정보
LogLevel.Information  // 일반 정보
LogLevel.Warning      // 경고
LogLevel.Error        // 에러
LogLevel.Critical     // 치명적 에러
```

#### 자동 기능
- **자동 Flush**: 5초마다 자동으로 버퍼 flush
- **즉시 Flush**: Error/Critical 레벨은 즉시 flush
- **자동 정리**: 30일 이상 된 로그 자동 삭제

### 2. 서버 오딧 로깅

#### 오딧 파일 생성 규칙
- **파일명 형식**: `{PC명}_{IP주소}_AUDIT_{날짜}.json`
- **예시**: `DESKTOP-ABC123_192.168.1.100_AUDIT_20241231.json`
- **저장 위치**: `%AppData%\nU3.Framework\AUDIT\`
- **형식**: JSON Lines (각 줄이 하나의 JSON 객체)

#### 오딧 액션 타입
```csharp
AuditAction.Create    // 생성
AuditAction.Read      // 조회
AuditAction.Update    // 수정
AuditAction.Delete    // 삭제
AuditAction.Login     // 로그인
AuditAction.Logout    // 로그아웃
AuditAction.Print     // 인쇄
AuditAction.Export    // 내보내기
AuditAction.Import    // 가져오기
AuditAction.Execute   // 실행
AuditAction.Search    // 검색
```

#### 기록되는 정보
- 사용자 ID 및 이름
- 액션 타입
- 모듈 및 화면 정보
- 엔티티 타입 및 ID
- 이전 값 / 새로운 값
- IP 주소 및 PC명
- 타임스탬프
- 성공/실패 여부

### 3. 서버 자동 업로드

#### HTTP 기반 업로드
- **에러 발생 시**: 즉시 현재 로그 업로드
- **자동 업로드**: 매일 새벽 2시 자동 업로드
- **수동 업로드**: API를 통한 수동 업로드 지원

#### 서버 API 엔드포인트
```
POST /api/log/upload         // 로그 파일 업로드
POST /api/log/upload-audit   // 오딧 로그 업로드
GET  /api/log/info           // 서버 로그 정보
```

## 설정 방법

### 1. appsettings.json 설정

```json
{
  "Logging": {
    "Enabled": true,
    "FileLogging": {
      "Enabled": true,
      "LogDirectory": "",
      "CleanupOldLogsAfterDays": 30,
      "MinimumLevel": "Information"
    },
    "AuditLogging": {
      "Enabled": true,
      "AuditDirectory": "",
      "CleanupOldAuditsAfterDays": 90
    },
    "ServerUpload": {
      "Enabled": true,
      "AutoUpload": true,
      "UploadOnError": true,
      "ServerPath": "Logs",
      "ServerUrl": "http://localhost:5000"
    }
  }
}
```

### 2. 프로그램 시작 시 초기화

```csharp
// MainShellForm 또는 Program.cs에서
LogManager.Instance.Initialize(
    logDirectory: null,              // null이면 기본 경로 사용
    auditDirectory: null,
    fileTransferService: null,       // HTTP 업로드 사용 시 null
    enableAutoUpload: true
);
```

## 사용 방법

### 1. 일반 로깅

#### 정적 메서드 사용 (간편)
```csharp
LogManager.Trace("상세한 디버그 정보", "Category");
LogManager.Debug("디버그 정보", "Category");
LogManager.Info("일반 정보", "Category");
LogManager.Warning("경고 메시지", "Category");
LogManager.Error("에러 메시지", "Category", exception);
LogManager.Critical("치명적 에러", "Category", exception);
```

#### 인스턴스 사용
```csharp
var logger = LogManager.Instance.Logger;
logger.Information("메시지", "Category");
logger.Error("에러 메시지", "Category", exception);
```

#### BaseWorkControl에서 사용
```csharp
public class MyControl : BaseWorkControl
{
    private void DoSomething()
    {
        LogInfo("작업 시작");
        
        try
        {
            // 작업 수행
            LogInfo("작업 완료");
        }
        catch (Exception ex)
        {
            LogError("작업 실패", ex);
        }
    }
}
```

### 2. 오딧 로깅

#### CRUD 오딧
```csharp
// 생성
LogManager.LogCreate("Patient", "P12345", 
    newValue: JsonSerializer.Serialize(patient),
    module: "EMR",
    screen: "PatientRegistration");

// 수정
LogManager.LogUpdate("Patient", "P12345",
    oldValue: JsonSerializer.Serialize(oldPatient),
    newValue: JsonSerializer.Serialize(newPatient),
    module: "EMR",
    screen: "PatientEdit");

// 삭제
LogManager.LogDelete("Patient", "P12345",
    oldValue: JsonSerializer.Serialize(patient),
    module: "EMR",
    screen: "PatientManagement");

// 조회
LogManager.LogRead("Patient", "P12345",
    module: "EMR",
    screen: "PatientView");
```

#### 액션 오딧
```csharp
// 일반 액션
LogManager.LogAction(
    action: AuditAction.Print,
    module: "EMR",
    screen: "PatientChart",
    additionalInfo: "Printed 5 pages");

// 사용자 정의 액션
LogManager.LogAction(
    action: "CUSTOM_ACTION",
    module: "MyModule",
    screen: "MyScreen",
    additionalInfo: "Additional info");
```

#### BaseWorkControl에서 사용
```csharp
public class PatientEditControl : BaseWorkControl
{
    private void SavePatient(Patient patient)
    {
        try
        {
            var oldPatient = LoadPatient(patient.Id);
            
            // 데이터 저장
            SaveToDatabase(patient);
            
            // 오딧 로그
            LogAudit(AuditAction.Update, "Patient", patient.Id,
                $"Updated patient: {patient.Name}");
            
            LogInfo("Patient saved successfully");
        }
        catch (Exception ex)
        {
            LogError("Failed to save patient", ex);
        }
    }
}
```

### 3. 서버 업로드

#### 수동 업로드
```csharp
// 현재 로그 즉시 업로드
var uploadService = LogManager.Instance.UploadService;
if (uploadService != null)
{
    await uploadService.UploadCurrentLogImmediatelyAsync();
}

// 모든 대기 중인 로그 업로드
await uploadService.UploadAllPendingLogsAsync();
```

#### 자동 업로드 활성화/비활성화
```csharp
var uploadService = LogManager.Instance.UploadService;
uploadService?.EnableAutoUpload(true);   // 매일 새벽 2시 자동 업로드
uploadService?.EnableAutoUpload(false);  // 자동 업로드 비활성화
```

## 로그 파일 형식

### 일반 로그 파일 예시
```
[2024-12-31 14:30:25.123] [INFORMATION] [Shell] [User:admin] Program opened: PatientRegistration
[2024-12-31 14:30:26.456] [INFORMATION] [EMR] [User:admin] [Prog:EMR.IN.WORKLIST] Patient search started
[2024-12-31 14:30:27.789] [WARNING  ] [EMR] [User:admin] No patients found for criteria
[2024-12-31 14:30:28.012] [ERROR    ] [EMR] [User:admin] Database connection failed
    Exception: System.Data.SqlClient.SqlException: Connection timeout
    at System.Data.SqlClient.SqlConnection.Open()
    at MyApp.Database.Connect()
```

### 오딧 로그 파일 예시 (JSON Lines)
```json
{"Timestamp":"2024-12-31T14:30:25.123","UserId":"admin","UserName":"관리자","Action":"LOGIN","Module":"Shell","Screen":"MainShellForm","IpAddress":"192.168.1.100","MachineName":"DESKTOP-ABC123","IsSuccess":true}
{"Timestamp":"2024-12-31T14:30:30.456","UserId":"admin","UserName":"관리자","Action":"CREATE","Module":"EMR","Screen":"PatientRegistration","EntityType":"Patient","EntityId":"P12345","NewValue":"{\"Name\":\"홍길동\",\"Age\":30}","IpAddress":"192.168.1.100","MachineName":"DESKTOP-ABC123","IsSuccess":true}
{"Timestamp":"2024-12-31T14:30:35.789","UserId":"admin","UserName":"관리자","Action":"UPDATE","Module":"EMR","Screen":"PatientEdit","EntityType":"Patient","EntityId":"P12345","OldValue":"{\"Name\":\"홍길동\",\"Age\":30}","NewValue":"{\"Name\":\"홍길동\",\"Age\":31}","IpAddress":"192.168.1.100","MachineName":"DESKTOP-ABC123","IsSuccess":true}
```

## 서버 로그 저장

### 서버측 저장 위치
```
C:\ProgramData\nU3.Framework\ServerLogs\
├── ClientLogs\                          # 클라이언트 로그
│   ├── DESKTOP-ABC123_192.168.1.100_20241231.log
│   ├── DESKTOP-XYZ456_192.168.1.101_20241231.log
│   └── _UploadLog_20241231.log         # 업로드 기록
└── ClientAudits\                        # 클라이언트 오딧
    ├── DESKTOP-ABC123_192.168.1.100_AUDIT_20241231.json
    └── DESKTOP-XYZ456_192.168.1.101_AUDIT_20241231.json
```

## 성능 고려사항

### 비동기 로깅
- 로그는 큐에 저장되고 비동기로 파일에 기록
- 애플리케이션 성능에 영향 최소화

### 자동 정리
- 오래된 로그 자동 삭제로 디스크 공간 관리
- 로그: 30일 (기본값)
- 오딧: 90일 (기본값)

### 버퍼링
- 일반 로그: 5초마다 flush
- Error/Critical: 즉시 flush
- 애플리케이션 종료 시: 모든 로그 flush

## 보안 고려사항

### 민감 정보 보호
- 비밀번호, 개인정보는 로그에 기록하지 않도록 주의
- 필요시 마스킹 처리

```csharp
// 나쁜 예
LogManager.Info($"User login: {userId}, Password: {password}");

// 좋은 예
LogManager.Info($"User login: {userId}");
```

### 오딧 로그 무결성
- JSON 형식으로 저장되어 파싱 및 분석 용이
- 타임스탬프, 사용자 정보 자동 기록
- 변조 방지를 위해 서버에 즉시 업로드 권장

## 문제 해결

### 로그 파일이 생성되지 않는 경우
1. 로깅 시스템이 초기화되었는지 확인
2. appsettings.json의 Logging.Enabled가 true인지 확인
3. 디렉토리 권한 확인

### 서버 업로드 실패
1. 서버 URL 확인
2. 네트워크 연결 확인
3. 서버 API가 실행 중인지 확인
4. 방화벽 설정 확인

### 로그 파일 크기가 너무 큰 경우
1. 로그 레벨을 Information 이상으로 설정
2. Debug/Trace 로그 최소화
3. CleanupOldLogsAfterDays 값 조정

## 모범 사례

### 1. 적절한 로그 레벨 사용
```csharp
// 디버깅 중에만 필요한 정보
LogManager.Debug("Loop iteration: {i}");

// 중요한 비즈니스 이벤트
LogManager.Info("Patient registered: {patientId}");

// 복구 가능한 문제
LogManager.Warning("Retry attempt 3/5");

// 예외 상황
LogManager.Error("Failed to save patient", ex);

// 시스템 장애
LogManager.Critical("Database connection lost", ex);
```

### 2. 의미있는 카테고리 사용
```csharp
LogManager.Info("Processing started", "PaymentProcess");
LogManager.Info("Payment validated", "PaymentProcess");
LogManager.Info("Payment completed", "PaymentProcess");
```

### 3. 구조화된 로깅
```csharp
// 검색 및 분석이 쉽도록 구조화
LogManager.Info(
    $"Order processed: OrderId={orderId}, Amount={amount}, Status={status}",
    "OrderManagement");
```

### 4. 중요한 액션은 반드시 오딧
```csharp
// 데이터 변경
LogManager.LogUpdate("Patient", patientId, oldValue, newValue, "EMR", "PatientEdit");

// 민감한 조회
LogManager.LogRead("PatientChart", patientId, "EMR", "ChartView");

// 중요한 작업
LogManager.LogAction(AuditAction.Print, "EMR", "Prescription", $"Printed prescription for {patientId}");
```

## API 레퍼런스

### LogManager
```csharp
// 초기화
LogManager.Instance.Initialize(logDirectory, auditDirectory, fileTransferService, enableAutoUpload);

// 로거 접근
ILogger logger = LogManager.Instance.Logger;
IAuditLogger auditLogger = LogManager.Instance.AuditLogger;
ILogUploadService uploadService = LogManager.Instance.UploadService;

// 편의 메서드
LogManager.Info(message, category);
LogManager.Error(message, category, exception);
LogManager.LogAudit(auditDto);
LogManager.LogAction(action, module, screen, additionalInfo);

// 종료
LogManager.Instance.Shutdown();
```

### ILogger
```csharp
void Trace(string message, string category = null, Exception exception = null);
void Debug(string message, string category = null, Exception exception = null);
void Information(string message, string category = null, Exception exception = null);
void Warning(string message, string category = null, Exception exception = null);
void Error(string message, string category = null, Exception exception = null);
void Critical(string message, string category = null, Exception exception = null);
Task FlushAsync();
```

### IAuditLogger
```csharp
void LogAudit(AuditLogDto audit);
void LogCreate(string entityType, string entityId, string newValue, string module, string screen);
void LogUpdate(string entityType, string entityId, string oldValue, string newValue, string module, string screen);
void LogDelete(string entityType, string entityId, string oldValue, string module, string screen);
void LogRead(string entityType, string entityId, string module, string screen);
void LogAction(string action, string module, string screen, string additionalInfo);
Task FlushAsync();
```

## 라이선스

? 2024 nU3 Framework
