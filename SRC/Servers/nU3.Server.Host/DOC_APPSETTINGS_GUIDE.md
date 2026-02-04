# nU3.Server.Host appsettings.json 설정 가이드

## 📋 전체 구조

```json
{
  "Logging": { ... },
  "ConnectionStrings": { ... },
  "ServerSettings": {
    "FileTransfer": { ... },
    "LogUpload": { ... },          // ← NEW!
    "Database": { ... }
  },
  "Cors": { ... },
  "RateLimiting": { ... },
  "ApiVersioning": { ... },
  "Swagger": { ... }                // ← NEW!
}
```

---

## 🔧 설정 항목 상세

### 1. Logging

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

**설명:**
- 서버 자체의 로깅 레벨 설정
- 클라이언트 로그 업로드와는 별개

**로그 레벨:**
- `Trace`: 매우 상세한 디버깅 정보
- `Debug`: 디버깅 정보
- `Information`: 일반 정보
- `Warning`: 경고
- `Error`: 오류
- `Critical`: 심각한 오류

---

### 2. ConnectionStrings

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost:1521/ORCL;User Id=nu3_user;Password=nu3_password;"
  }
}
```

**설명:**
- Oracle 데이터베이스 연결 문자열
- `DBAccessController`에서 사용

**형식:**
```
Data Source=서버:포트/서비스명;User Id=사용자;Password=비밀번호;
```

---

### 3. ServerSettings.FileTransfer

```json
{
  "ServerSettings": {
    "FileTransfer": {
      "HomeDirectory": "C:\\nU3_Server_Storage",
      "MaxUploadSizeMB": 100,
      "AllowedExtensions": [ ".pdf", ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".log", ".gz" ]
    }
  }
}
```

**설명:**
- 파일 전송 서비스 설정
- `FileTransferController`에서 사용

**옵션:**
- `HomeDirectory`: 파일 저장 루트 디렉토리
- `MaxUploadSizeMB`: 최대 업로드 크기 (MB)
- `AllowedExtensions`: 허용된 파일 확장자 목록
  - `[ "*" ]`: 모든 확장자 허용 (개발 환경)
  - `.gz` 추가: 압축 파일 지원

**경로 설정:**
```
C:\nU3_Server_Storage\
├── uploads\           # 일반 업로드
├── downloads\         # 다운로드 파일
└── temp\             # 임시 파일
```

---

### 4. ServerSettings.LogUpload ✨ NEW!

```json
{
  "ServerSettings": {
    "LogUpload": {
      "LogStoragePath": "C:\\ProgramData\\nU3.Framework\\ServerLogs\\ClientLogs",
      "AuditStoragePath": "C:\\ProgramData\\nU3.Framework\\ServerLogs\\ClientAudits",
      "MaxLogSizeMB": 50,
      "EnableCompression": true,
      "AutoCleanupDays": 30
    }
  }
}
```

**설명:**
- 클라이언트 로그 업로드 서비스 설정
- `LogController`에서 사용

**옵션:**
- `LogStoragePath`: 클라이언트 로그 저장 경로
- `AuditStoragePath`: 클라이언트 오딧 로그 저장 경로
- `MaxLogSizeMB`: 최대 로그 파일 크기 (MB)
- `EnableCompression`: Gzip 압축 지원 여부
- `AutoCleanupDays`: 자동 정리 기간 (일)

**저장 경로 구조:**
```
C:\ProgramData\nU3.Framework\ServerLogs\
├── ClientLogs\
│   ├── PC001_192.168.1.10_20240127.log
│   ├── PC002_192.168.1.11_20240127.log
│   └── _UploadLog_20240127.log
└── ClientAudits\
    ├── PC001_Audit_20240127.log
    └── PC002_Audit_20240127.log
```

---

### 5. ServerSettings.Database

```json
{
  "ServerSettings": {
    "Database": {
      "CommandTimeout": 30,
      "MaxRetryCount": 3
    }
  }
}
```

**설명:**
- 데이터베이스 연결 및 실행 설정

**옵션:**
- `CommandTimeout`: 명령 실행 타임아웃 (초)
- `MaxRetryCount`: 최대 재시도 횟수

---

### 6. Cors

```json
{
  "Cors": {
    "AllowedOrigins": [ "http://localhost:*", "https://localhost:*" ],
    "AllowCredentials": true
  }
}
```

**설명:**
- CORS (Cross-Origin Resource Sharing) 설정
- 클라이언트에서 API 호출 시 필요

**옵션:**
- `AllowedOrigins`: 허용된 Origin 목록
  - `*`: 모든 Origin 허용 (보안 위험!)
  - `http://localhost:*`: 로컬호스트 모든 포트
- `AllowCredentials`: 쿠키/인증 정보 전송 허용

**프로덕션 설정:**
```json
{
  "AllowedOrigins": [ 
    "https://client1.hospital.com",
    "https://client2.hospital.com"
  ],
  "AllowCredentials": true
}
```

---

### 7. RateLimiting

```json
{
  "RateLimiting": {
    "Enabled": true,
    "PermitLimit": 100,
    "WindowMinutes": 1
  }
}
```

**설명:**
- API 호출 제한 설정 (DDoS 방지)

**옵션:**
- `Enabled`: 활성화 여부
- `PermitLimit`: 허용 요청 수
- `WindowMinutes`: 시간 창 (분)

**예시:**
- `100 / 1분`: 1분에 최대 100개 요청
- 초과 시 `429 Too Many Requests` 반환

---

### 8. ApiVersioning

```json
{
  "ApiVersioning": {
    "DefaultVersion": "1.0",
    "AssumeDefaultVersionWhenUnspecified": true,
    "ReportApiVersions": true
  }
}
```

**설명:**
- API 버전 관리 설정

**옵션:**
- `DefaultVersion`: 기본 버전
- `AssumeDefaultVersionWhenUnspecified`: 버전 미지정 시 기본 버전 사용
- `ReportApiVersions`: 응답 헤더에 지원 버전 표시

**사용 예:**
```
GET /api/v1/dbaccess/connect
GET /api/v2/dbaccess/connect
```

---

### 9. Swagger ✨ NEW!

```json
{
  "Swagger": {
    "Enabled": true,
    "Title": "nU3 Server API",
    "Description": "REST API for nU3 Framework - Database Access, File Transfer, and Log Upload Services",
    "Version": "v1",
    "RoutePrefix": "swagger",
    "EnableAnnotations": true,
    "IncludeXmlComments": true
  }
}
```

**설명:**
- Swagger UI 설정 (API 문서)

**옵션:**
- `Enabled`: Swagger 활성화 여부
- `Title`: API 문서 제목
- `Description`: API 설명
- `Version`: API 버전
- `RoutePrefix`: Swagger UI URL 경로
- `EnableAnnotations`: 어노테이션 지원
- `IncludeXmlComments`: XML 주석 포함

**접근 URL:**
```
https://localhost:64229/swagger
https://localhost:64229/swagger/index.html
```

**Swagger UI:**
```
┌─────────────────────────────────────────────────────────────┐
│  nU3 Server API                                    v1       │
├─────────────────────────────────────────────────────────────┤
│  REST API for nU3 Framework                                 │
│                                                             │
│  Endpoints:                                                 │
│  ▼ DBAccess                                                 │
│    POST /api/dbaccess/connect                               │
│    POST /api/dbaccess/execute-query                         │
│    POST /api/dbaccess/execute-datatable                     │
│                                                             │
│  ▼ FileTransfer                                             │
│    POST /api/filetransfer/upload                            │
│    GET  /api/filetransfer/download/{fileName}               │
│                                                             │
│  ▼ Log                                                      │
│    POST /api/log/upload                                     │
│    POST /api/log/upload-audit                               │
│    GET  /api/log/info                                       │
└─────────────────────────────────────────────────────────────┘
```

---

## 🌍 환경별 설정

### Production (appsettings.json)

```json
{
  "ServerSettings": {
    "FileTransfer": {
      "HomeDirectory": "C:\\nU3_Server_Storage",
      "MaxUploadSizeMB": 100,
      "AllowedExtensions": [ ".pdf", ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".log", ".gz" ]
    },
    "LogUpload": {
      "LogStoragePath": "C:\\ProgramData\\nU3.Framework\\ServerLogs\\ClientLogs",
      "AuditStoragePath": "C:\\ProgramData\\nU3.Framework\\ServerLogs\\ClientAudits",
      "MaxLogSizeMB": 50,
      "EnableCompression": true,
      "AutoCleanupDays": 30
    }
  },
  "Swagger": {
    "Enabled": false  // ← 프로덕션에서는 비활성화 권장
  }
}
```

### Development (appsettings.Development.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Debug"
    }
  },
  "ServerSettings": {
    "FileTransfer": {
      "HomeDirectory": "C:\\nU3_Dev_Storage",
      "MaxUploadSizeMB": 500,
      "AllowedExtensions": [ "*" ]  // ← 모든 확장자 허용
    },
    "LogUpload": {
      "LogStoragePath": "C:\\Temp\\nU3_Dev_Logs\\ClientLogs",
      "AuditStoragePath": "C:\\Temp\\nU3_Dev_Logs\\ClientAudits",
      "MaxLogSizeMB": 100,
      "EnableCompression": true,
      "AutoCleanupDays": 7  // ← 7일 (개발 환경)
    }
  },
  "Swagger": {
    "Enabled": true,  // ← 개발 환경에서는 활성화
    "Title": "nU3 Server API (Development)"
  },
  "DetailedErrors": true
}
```

---

## 🔒 보안 고려사항

### 1. 데이터베이스 연결 문자열

❌ **Bad: 평문 저장**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost:1521/ORCL;User Id=admin;Password=P@ssw0rd123;"
  }
}
```

✅ **Good: 환경 변수 사용**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=${DB_HOST}:${DB_PORT}/${DB_SID};User Id=${DB_USER};Password=${DB_PASSWORD};"
  }
}
```

또는 **User Secrets** 사용:
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=..."
```

### 2. CORS 설정

❌ **Bad: 모든 Origin 허용**
```json
{
  "Cors": {
    "AllowedOrigins": [ "*" ]
  }
}
```

✅ **Good: 특정 Origin만 허용**
```json
{
  "Cors": {
    "AllowedOrigins": [ 
      "https://client.hospital.com",
      "https://admin.hospital.com"
    ]
  }
}
```

### 3. Swagger

❌ **Bad: 프로덕션에서 활성화**
```json
{
  "Swagger": {
    "Enabled": true  // ← 보안 위험!
  }
}
```

✅ **Good: 프로덕션에서 비활성화**
```json
{
  "Swagger": {
    "Enabled": false
  }
}
```

---

## 📊 설정 검증

### appsettings.json 검증 체크리스트

```
✅ ConnectionStrings
   - DefaultConnection 설정됨
   - 올바른 형식
   - 보안 (User Secrets 사용 권장)

✅ ServerSettings.FileTransfer
   - HomeDirectory 존재
   - 쓰기 권한 있음
   - AllowedExtensions 적절

✅ ServerSettings.LogUpload
   - LogStoragePath 존재
   - AuditStoragePath 존재
   - 쓰기 권한 있음
   - .gz 확장자 허용 (압축 지원)

✅ Cors
   - AllowedOrigins 적절
   - "*" 사용 안 함 (프로덕션)

✅ Swagger
   - Enabled: false (프로덕션)
   - Enabled: true (개발)
```

---

## 🧪 테스트

### 1. 설정 로드 테스트

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// 설정 읽기
var fileTransferSettings = builder.Configuration.GetSection("ServerSettings:FileTransfer");
var logUploadSettings = builder.Configuration.GetSection("ServerSettings:LogUpload");

Console.WriteLine($"FileTransfer HomeDirectory: {fileTransferSettings["HomeDirectory"]}");
Console.WriteLine($"LogUpload LogStoragePath: {logUploadSettings["LogStoragePath"]}");
```

### 2. Swagger 접근 테스트

```bash
# 개발 환경
curl https://localhost:64229/swagger

# 응답: 200 OK (HTML)
```

### 3. 로그 업로드 테스트

```bash
# 클라이언트에서
curl -X POST https://localhost:64229/api/log/upload \
  -F "File=@test.log"

# 서버에서 확인
dir C:\ProgramData\nU3.Framework\ServerLogs\ClientLogs\test.log
```

---

## 🎯 빠른 시작

### 최소 설정 (개발 환경)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost:1521/ORCL;User Id=dev_user;Password=dev_password;"
  },
  "ServerSettings": {
    "FileTransfer": {
      "HomeDirectory": "C:\\Temp\\nU3_Storage",
      "MaxUploadSizeMB": 500,
      "AllowedExtensions": [ "*" ]
    },
    "LogUpload": {
      "LogStoragePath": "C:\\Temp\\nU3_Logs\\ClientLogs",
      "AuditStoragePath": "C:\\Temp\\nU3_Logs\\ClientAudits",
      "MaxLogSizeMB": 100,
      "EnableCompression": true,
      "AutoCleanupDays": 7
    }
  },
  "Swagger": {
    "Enabled": true
  }
}
```

### 디렉토리 생성

```powershell
# 개발 환경
mkdir C:\Temp\nU3_Storage
mkdir C:\Temp\nU3_Logs\ClientLogs
mkdir C:\Temp\nU3_Logs\ClientAudits

# 프로덕션
mkdir C:\nU3_Server_Storage
mkdir C:\ProgramData\nU3.Framework\ServerLogs\ClientLogs
mkdir C:\ProgramData\nU3.Framework\ServerLogs\ClientAudits
```

---

## 📚 관련 문서

- `README.md`: 서버 개요
- `ENHANCEMENT_REPORT.md`: 개선 사항
- `SWAGGER_FIX_SUMMARY.md`: Swagger 설정 가이드
- `LOG_COMPRESSION_GUIDE.md`: 로그 압축 가이드

---

## 🎉 완료!

**appsettings.json 설정이 완료되었습니다!**

### 주요 추가 사항

```
✅ ServerSettings.LogUpload (로그 업로드 설정)
✅ Swagger 설정
✅ .gz 확장자 지원 (압축 파일)
✅ 환경별 설정 (Development)
```

### 다음 단계

1. 디렉토리 생성
2. 서버 실행
3. Swagger UI 접근: `https://localhost:64229/swagger`
4. 로그 업로드 테스트

**완벽합니다!** 🚀
