# nU3 Framework - 에러 리포팅 시스템

## 개요

MainShellForm에 예상치 않은 비정상 종료 발생 시 자동으로 에러 정보를 수집하고 이메일로 전송하는 기능이 추가되었습니다.

## 주요 기능

### 1. 자동 크래시 리포팅
- **UI 스레드 예외** 처리
- **비UI 스레드 예외** 처리
- **Task 예외** 처리
- **자동 스크린샷 캡처**
- **상세 로그 파일 생성**
- **이메일 자동 전송**

### 2. 수집되는 정보
- 발생 시간
- 사용자 ID 및 권한 레벨
- 컴퓨터명
- OS 버전
- 애플리케이션 버전
- 예외 타입 및 메시지
- 전체 스택 트레이스
- 현재 활성 탭 정보
- 열려있는 탭 수
- 에러 발생 시 화면 스크린샷

### 3. 시스템 메뉴 기능
- **에러 리포팅 설정**: 현재 설정 상태 확인
- **크래시 리포트 테스트**: 실제 예외 없이 이메일 전송 테스트

## 설정 방법

### 1. appsettings.json 파일 설정

`nU3.Shell/appsettings.json` 파일에서 에러 리포팅을 설정합니다:

```json
{
  "ErrorReporting": {
    "Enabled": true,
    "Email": {
      "SmtpServer": "smtp.gmail.com",
      "SmtpPort": 587,
      "EnableSsl": true,
      "Username": "your-email@gmail.com",
      "Password": "your-app-password",
      "FromEmail": "your-email@gmail.com",
      "FromName": "nU3 Framework Error Reporter",
      "ToEmail": "admin@yourdomain.com"
    },
    "CaptureScreenshot": true,
    "CreateLogFile": true,
    "CleanupOldLogsAfterDays": 30
  }
}
```

### 2. Gmail 앱 비밀번호 생성 (Gmail 사용 시)

1. Google 계정 설정 → 보안 → 2단계 인증 활성화
2. 앱 비밀번호 생성
3. 생성된 16자리 비밀번호를 `Password` 필드에 입력

### 3. 다른 SMTP 서버 사용 (예: Office 365)

```json
{
  "Email": {
    "SmtpServer": "smtp.office365.com",
    "SmtpPort": 587,
    "EnableSsl": true,
    "Username": "your-email@company.com",
    "Password": "your-password",
    "FromEmail": "your-email@company.com",
    "ToEmail": "admin@company.com"
  }
}
```

## 사용 방법

### 1. 자동 에러 리포팅

에러 리포팅이 활성화되면 예외 발생 시 자동으로:
1. 현재 화면 스크린샷 캡처
2. 상세 로그 파일 생성 (`%AppData%\nU3.Framework\CrashLogs`)
3. 이메일로 리포트 전송
4. 사용자에게 에러 메시지 표시

### 2. 수동 테스트

**시스템 메뉴 → 크래시 리포트 테스트** 를 클릭하여:
- 실제 예외 없이 테스트 리포트 전송
- 이메일 설정 검증
- 스크린샷 및 로그 파일 생성 확인

### 3. 설정 확인

**시스템 메뉴 → 에러 리포팅 설정** 을 클릭하여:
- 활성화 상태 확인
- 수신자 이메일 확인

## 파일 구조

```
nU3.Shell/
├── appsettings.json                     # 설정 파일
├── Helpers/
│   ├── ScreenshotHelper.cs              # 스크린샷 캡처
│   ├── EmailHelper.cs                   # 이메일 전송
│   └── CrashReporter.cs                 # 크래시 리포팅
└── MainShellForm.cs                     # 전역 예외 처리
```

## 로그 파일 위치

```
%AppData%\nU3.Framework\CrashLogs\
├── crash_20241231_143025.png           # 스크린샷
└── crash_20241231_143025.log           # 로그 파일
```

## 이메일 리포트 샘플

수신되는 이메일에는 다음 정보가 포함됩니다:

- **제목**: [nU3 Framework] 비정상 종료 리포트 - 2024-12-31 14:30:25
- **기본 정보**: 발생 시간, 사용자, 컴퓨터명, 버전
- **에러 정보**: 예외 타입, 메시지
- **스택 트레이스**: 전체 스택 정보
- **첨부파일**: 스크린샷, 로그 파일

## 보안 고려사항

1. **비밀번호 보호**: `appsettings.json` 파일을 안전하게 보관
2. **앱 비밀번호 사용**: 실제 계정 비밀번호 대신 앱 비밀번호 사용
3. **암호화**: 필요시 비밀번호 암호화 추가 고려
4. **권한 제한**: appsettings.json 파일에 대한 접근 권한 제한

## 문제 해결

### 이메일이 전송되지 않는 경우

1. **SMTP 설정 확인**
   - SmtpServer 주소가 올바른지 확인
   - SmtpPort가 올바른지 확인 (일반적으로 587 또는 465)

2. **인증 정보 확인**
   - Username과 Password가 올바른지 확인
   - Gmail의 경우 앱 비밀번호 사용 확인

3. **방화벽 확인**
   - SMTP 포트가 방화벽에서 허용되는지 확인

4. **SSL/TLS 설정 확인**
   - EnableSsl이 SMTP 서버 요구사항과 일치하는지 확인

### 로그 파일이 생성되지 않는 경우

1. 디렉토리 권한 확인
2. 디스크 공간 확인

## 향후 개선 사항

- [ ] 비밀번호 암호화
- [ ] 로그 파일 압축
- [ ] 원격 로그 서버 전송
- [ ] 에러 분석 대시보드
- [ ] Slack/Teams 알림 통합

## 작성자

nU3 Framework Development Team

## 라이선스

? 2024 nU3 Framework
