# nU3.Shell - Server Host 설정 가이드

## ?? 개요

nU3.Shell에서 nU3.Server.Host API 서버에 연결하기 위한 설정 방법을 안내합니다.

---

## ?? 설정 파일

### appsettings.json 위치

```
nU3.Shell/
├── bin/
│   └── Debug/
│       └── appsettings.json  ← 이 파일 수정!
└── appsettings.json          ← 원본 (복사됨)
```

---

## ?? ServerConnection 설정

### appsettings.json 예시

```json
{
  "ServerConnection": {
    "Enabled": true,
    "BaseUrl": "https://localhost:64229",
    "Timeout": 300,
    "RetryCount": 3
  },
  "ErrorReporting": {
    "Enabled": true,
    ...
  },
  "Logging": {
    "Enabled": true,
    ...
  }
}
```

### 설정 항목 설명

| 항목 | 타입 | 기본값 | 설명 |
|------|------|--------|------|
| **Enabled** | bool | true | 서버 연결 활성화 여부 |
| **BaseUrl** | string | "https://localhost:64229" | API 서버 주소 |
| **Timeout** | int | 300 | 요청 타임아웃 (초) |
| **RetryCount** | int | 3 | 실패 시 재시도 횟수 |

---

## ?? 사용 예시

### 1. 설정 로드

```csharp
using nU3.Shell.Configuration;
using nU3.Connectivity.Implementations;

// 설정 로드
var serverConfig = ServerConnectionConfig.Load();

if (serverConfig.Enabled)
{
    // DB 클라이언트 생성
    var dbClient = new HttpDBAccessClient(serverConfig.BaseUrl);
    
    // 파일 클라이언트 생성
    var fileClient = new HttpFileTransferClient(serverConfig.BaseUrl);
    
    // 사용
    var connected = await dbClient.ConnectAsync();
    if (connected)
    {
        Console.WriteLine("서버 연결 성공!");
    }
}
```

### 2. MainShellForm에서 초기화 (ConnectivityManager 사용)

`nUShell` (메인 폼)에서 `ConnectivityManager`를 사용하여 서버 연결을 중앙에서 초기화합니다.

```csharp
public partial class nUShell : BaseWorkForm
{
    private void InitializeServerConnection()
    {
        try
        {
            var config = ServerConnectionConfig.Load();
            
            if (!config.Enabled)
            {
                LogManager.Info("구성에서 서버 연결이 비활성화되어 있습니다.", "Shell");
                barStaticItemServer.Caption = "🔴 서버: 비활성화";
                return;
            }
            
            // ConnectivityManager 초기화 (싱글톤)
            // enableLogCompression: true로 설정하여 로그 업로드 시 GZip 압축 사용
            ConnectivityManager.Instance.Initialize(config.BaseUrl, enableLogCompression: true);
            
            // 로그 메시지 이벤트 구독
            ConnectivityManager.Instance.LogMessage += OnConnectivityLogMessage;
            
            // 비동기 연결 테스트 수행
            Task.Run(async () => {
                var connected = await ConnectivityManager.Instance.TestConnectionAsync();
                // UI 업데이트 로직...
            });
        }
        catch (Exception ex)
        {
            LogManager.Error("서버 연결 초기화 실패", "Shell", ex);
        }
    }
}
```

### 3. DB 쿼리 실행 (ConnectivityManager 접근)

개별 클라이언트 인스턴스를 관리할 필요 없이 `ConnectivityManager.Instance.DB`를 사용합니다.

```csharp
private async void btnLoadData_Click(object sender, EventArgs e)
{
    try
    {
        // 쿼리 실행
        var dt = await ConnectivityManager.Instance.DB.ExecuteDataTableAsync(
            "SELECT * FROM Users WHERE Age > @age",
            new Dictionary<string, object> { { "@age", 18 } }
        );
        
        dataGridView1.DataSource = dt;
    }
    catch (Exception ex)
    {
        XtraMessageBox.Show($"데이터 로드 실패: {ex.Message}", "오류");
    }
}
```

### 4. 파일 업로드

```csharp
private async void btnUploadFile_Click(object sender, EventArgs e)
{
    if (_fileClient == null)
    {
        XtraMessageBox.Show("서버 연결이 초기화되지 않았습니다.", "오류");
        return;
    }
    
    using var openFileDialog = new OpenFileDialog();
    if (openFileDialog.ShowDialog() != DialogResult.OK)
        return;
    
    try
    {
        // 파일 읽기
        var data = await File.ReadAllBytesAsync(openFileDialog.FileName);
        
        // 서버에 업로드
        var serverPath = $"uploads/{Path.GetFileName(openFileDialog.FileName)}";
        var success = await _fileClient.UploadFileAsync(serverPath, data);
        
        if (success)
        {
            XtraMessageBox.Show("파일 업로드 성공!", "성공");
            LogManager.Info($"File uploaded: {serverPath}", "Shell");
        }
        else
        {
            XtraMessageBox.Show("파일 업로드 실패", "실패");
        }
    }
    catch (Exception ex)
    {
        LogManager.Error("File upload failed", "Shell", ex);
        XtraMessageBox.Show($"업로드 실패: {ex.Message}", "오류");
    }
}
```

---

## ?? 환경별 설정

### Development (개발)

```json
{
  "ServerConnection": {
    "Enabled": true,
    "BaseUrl": "https://localhost:64229",
    "Timeout": 300,
    "RetryCount": 3
  }
}
```

### Staging (테스트)

```json
{
  "ServerConnection": {
    "Enabled": true,
    "BaseUrl": "https://test-server.company.com",
    "Timeout": 300,
    "RetryCount": 5
  }
}
```

### Production (운영)

```json
{
  "ServerConnection": {
    "Enabled": true,
    "BaseUrl": "https://api.company.com",
    "Timeout": 600,
    "RetryCount": 3
  }
}
```

---

## ?? 보안 고려사항

### 1. HTTPS 사용

```json
{
  "ServerConnection": {
    "BaseUrl": "https://api.company.com"  // ? HTTPS 사용
  }
}
```

**주의:** 운영 환경에서는 반드시 HTTPS를 사용하세요!

### 2. 인증서 검증

개발 환경에서 자체 서명 인증서 사용 시:

```csharp
#if DEBUG
// 개발 환경에서만 인증서 검증 무시
var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = 
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
};
var httpClient = new HttpClient(handler);
var dbClient = new HttpDBAccessClient(httpClient, serverConfig.BaseUrl);
#endif
```

### 3. 타임아웃 설정

```json
{
  "ServerConnection": {
    "Timeout": 300  // 5분 (대용량 쿼리/파일 전송용)
  }
}
```

---

## ?? 연결 테스트

### 연결 테스트 코드

```csharp
private async Task<bool> TestServerConnectionAsync()
{
    var config = ServerConnectionConfig.Load();
    
    if (!config.Enabled)
    {
        LogManager.Warning("Server connection is disabled", "Shell");
        return false;
    }
    
    try
    {
        using var client = new HttpClient
        {
            BaseAddress = new Uri(config.BaseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
        
        // Health Check 호출
        var response = await client.GetAsync("/health");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            LogManager.Info($"Server health check passed: {content}", "Shell");
            return true;
        }
        else
        {
            LogManager.Warning($"Server health check failed: {response.StatusCode}", "Shell");
            return false;
        }
    }
    catch (Exception ex)
    {
        LogManager.Error($"Server connection test failed: {ex.Message}", "Shell", ex);
        return false;
    }
}
```

### UI에서 테스트

```csharp
private async void btnTestConnection_Click(object sender, EventArgs e)
{
    var config = ServerConnectionConfig.Load();
    
    var message = $"서버 연결 설정\n\n" +
                 $"활성화: {(config.Enabled ? "예" : "아니오")}\n" +
                 $"주소: {config.BaseUrl}\n" +
                 $"타임아웃: {config.Timeout}초\n" +
                 $"재시도: {config.RetryCount}회\n\n" +
                 $"연결 테스트를 진행하시겠습니까?";
    
    if (XtraMessageBox.Show(message, "서버 연결 테스트", 
        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        return;
    
    var result = await TestServerConnectionAsync();
    
    if (result)
    {
        XtraMessageBox.Show("서버 연결 성공!", "성공", 
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    else
    {
        XtraMessageBox.Show("서버 연결 실패!\n\n설정을 확인하세요.", "실패", 
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

---

## ?? 트러블슈팅

### 문제 1: 연결 타임아웃

**증상:**
```
System.Threading.Tasks.TaskCanceledException: The request was canceled due to the configured HttpClient.Timeout
```

**해결:**
```json
{
  "ServerConnection": {
    "Timeout": 600  // 타임아웃 증가 (10분)
  }
}
```

### 문제 2: SSL 인증서 오류

**증상:**
```
System.Net.Http.HttpRequestException: The SSL connection could not be established
```

**해결 (개발 환경):**
```csharp
#if DEBUG
var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
};
var httpClient = new HttpClient(handler);
#endif
```

### 문제 3: 서버를 찾을 수 없음

**증상:**
```
System.Net.Http.HttpRequestException: No such host is known
```

**해결:**
1. BaseUrl 확인
2. 서버가 실행 중인지 확인
3. 방화벽 설정 확인

```bash
# 서버 상태 확인
curl https://localhost:64229/health

# 또는
Test-NetConnection -ComputerName localhost -Port 64229
```

---

## ?? 상태 모니터링

### 상태바에 서버 상태 표시

```csharp
private async void TimerServerStatus_Tick(object sender, EventArgs e)
{
    if (_dbClient == null)
    {
        barStaticItemServer.Caption = "?? 서버 연결 없음";
        return;
    }
    
    try
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var connected = await _dbClient.ConnectAsync();
        
        if (connected)
        {
            barStaticItemServer.Caption = "?? 서버 연결됨";
        }
        else
        {
            barStaticItemServer.Caption = "?? 서버 연결 실패";
        }
    }
    catch
    {
        barStaticItemServer.Caption = "?? 서버 오류";
    }
}
```

---

## ? 체크리스트

- [ ] appsettings.json에 ServerConnection 섹션 추가
- [ ] BaseUrl을 올바른 서버 주소로 설정
- [ ] Enabled를 true로 설정
- [ ] MainShellForm에서 ServerConnectionConfig.Load() 호출
- [ ] HttpDBAccessClient 및 HttpFileTransferClient 생성
- [ ] 연결 테스트 실행
- [ ] 상태바에 서버 상태 표시

---

## ?? 완료!

이제 nU3.Shell에서 nU3.Server.Host API 서버에 연결할 수 있습니다!

**다음 단계:**
1. ? appsettings.json 설정 완료
2. ? ServerConnectionConfig 클래스 생성
3. ? MainShellForm에서 초기화
4. ?? 실제 API 호출 구현
5. ?? 오류 처리 및 로깅 추가

**참고 문서:**
- `HTTP_CLIENT_GUIDE.md` - HTTP 클라이언트 사용 가이드
- `ARCHITECTURE_SEPARATION.md` - 아키텍처 설명
- `README.md` (Server.Host) - 서버 API 문서
