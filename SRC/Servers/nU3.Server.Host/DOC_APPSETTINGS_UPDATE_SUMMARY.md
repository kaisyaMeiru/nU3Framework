# appsettings.json 설정 완료

## ?? 수정 내용

서버 설정 파일에 **로그 업로드** 및 **Swagger** 설정을 추가했습니다.

---

## ?? 수정된 파일

```
?? Servers/nU3.Server.Host/appsettings.json
   - ServerSettings.LogUpload 추가
   - Swagger 설정 추가
   - AllowedExtensions에 .gz 추가

?? Servers/nU3.Server.Host/appsettings.Development.json
   - ServerSettings.LogUpload 추가 (개발용)
   - Swagger 설정 추가 (개발용)
   - 로깅 레벨 상세화

? Servers/nU3.Server.Host/APPSETTINGS_GUIDE.md
   - 전체 설정 가이드 문서

? 빌드 성공
```

---

## ?? 주요 추가 설정

### 1. ServerSettings.LogUpload

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

**기능:**
- 클라이언트 로그 저장 경로 설정
- Gzip 압축 지원
- 자동 정리 (30일)

---

### 2. Swagger 설정

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

**기능:**
- API 문서 자동 생성
- 접근 URL: `https://localhost:64229/swagger`

---

### 3. AllowedExtensions 업데이트

```json
{
  "ServerSettings": {
    "FileTransfer": {
      "AllowedExtensions": [ 
        ".pdf", ".jpg", ".png", ".doc", ".docx", 
        ".xls", ".xlsx", ".txt", ".log", 
        ".gz"  // ← 압축 파일 지원
      ]
    }
  }
}
```

---

## ?? 환경별 설정

### Production (appsettings.json)

```json
{
  "ServerSettings": {
    "LogUpload": {
      "LogStoragePath": "C:\\ProgramData\\nU3.Framework\\ServerLogs\\ClientLogs",
      "AutoCleanupDays": 30  // 30일 보관
    }
  },
  "Swagger": {
    "Enabled": true  // 프로덕션에서는 false 권장
  }
}
```

### Development (appsettings.Development.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",  // 상세 로깅
      "Microsoft.AspNetCore": "Debug"
    }
  },
  "ServerSettings": {
    "FileTransfer": {
      "AllowedExtensions": [ "*" ]  // 모든 확장자 허용
    },
    "LogUpload": {
      "LogStoragePath": "C:\\Temp\\nU3_Dev_Logs\\ClientLogs",
      "AutoCleanupDays": 7  // 7일 보관
    }
  },
  "Swagger": {
    "Enabled": true,  // 개발 환경에서 활성화
    "Title": "nU3 Server API (Development)"
  }
}
```

---

## ?? 설정 비교

| 항목 | Production | Development |
|------|-----------|-------------|
| **로그 레벨** | Information | Debug |
| **로그 보관 기간** | 30일 | 7일 |
| **최대 업로드 크기** | 100MB | 500MB |
| **허용 확장자** | 제한됨 | 모두 허용 (*) |
| **Swagger** | false 권장 | true |
| **DetailedErrors** | false | true |

---

## ?? Swagger 404 오류 해결

### Before (오류 발생)

```
GET /swagger → 404 Not Found
```

**원인:**
- Swagger 설정 없음
- `appsettings.json`에 Swagger 섹션 누락

### After (해결)

```json
{
  "Swagger": {
    "Enabled": true,
    "RoutePrefix": "swagger"
  }
}
```

```
GET /swagger → 200 OK (Swagger UI)
```

---

## ?? 디렉토리 구조

### Production

```
C:\ProgramData\nU3.Framework\
└── ServerLogs\
    ├── ClientLogs\
    │   ├── PC001_192.168.1.10_20240127.log
    │   ├── PC002_192.168.1.11_20240127.log
    │   └── _UploadLog_20240127.log
    └── ClientAudits\
        ├── PC001_Audit_20240127.log
        └── PC002_Audit_20240127.log
```

### Development

```
C:\Temp\
└── nU3_Dev_Logs\
    ├── ClientLogs\
    │   └── test.log
    └── ClientAudits\
        └── test_audit.log
```

---

## ?? 사용 방법

### 1. 디렉토리 생성

```powershell
# Production
mkdir C:\ProgramData\nU3.Framework\ServerLogs\ClientLogs
mkdir C:\ProgramData\nU3.Framework\ServerLogs\ClientAudits

# Development
mkdir C:\Temp\nU3_Dev_Logs\ClientLogs
mkdir C:\Temp\nU3_Dev_Logs\ClientAudits
```

### 2. 서버 실행

```bash
cd Servers\nU3.Server.Host
dotnet run
```

### 3. Swagger UI 접근

```
브라우저에서:
https://localhost:64229/swagger
```

### 4. API 테스트

```bash
# 로그 업로드
curl -X POST https://localhost:64229/api/log/upload \
  -H "Content-Type: multipart/form-data" \
  -F "File=@test.log"

# 연결 테스트
curl -X POST https://localhost:64229/api/dbaccess/connect \
  -H "Content-Type: application/json"
```

---

## ? 검증 체크리스트

```
? appsettings.json 업데이트
? appsettings.Development.json 업데이트
? ServerSettings.LogUpload 추가
? Swagger 설정 추가
? AllowedExtensions에 .gz 추가
? 환경별 설정 분리
? 가이드 문서 작성
? 빌드 성공
```

---

## ?? 보안 권장사항

### 1. 프로덕션 Swagger 비활성화

```json
{
  "Swagger": {
    "Enabled": false  // ← 프로덕션에서는 비활성화
  }
}
```

### 2. CORS 제한

```json
{
  "Cors": {
    "AllowedOrigins": [ 
      "https://client.hospital.com"  // ← 특정 도메인만
    ]
  }
}
```

### 3. ConnectionStrings 보안

```bash
# User Secrets 사용
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "..."
```

---

## ?? 관련 문서

- `APPSETTINGS_GUIDE.md`: 전체 설정 가이드
- `README.md`: 서버 개요
- `ENHANCEMENT_REPORT.md`: 개선 사항
- `SWAGGER_FIX_SUMMARY.md`: Swagger 설정
- `LOG_COMPRESSION_GUIDE.md`: 로그 압축

---

## ?? 완료!

**appsettings.json 설정이 완료되었습니다!**

### 주요 개선 사항

```
? 로그 업로드 설정 추가
? Swagger UI 설정 추가
? 압축 파일 지원 (.gz)
? 환경별 설정 분리
? 상세 가이드 문서
```

### 다음 단계

1. **디렉토리 생성**: PowerShell 스크립트 실행
2. **서버 실행**: `dotnet run`
3. **Swagger 접근**: `https://localhost:64229/swagger`
4. **API 테스트**: Swagger UI에서 직접 테스트

**완벽합니다!** ??
