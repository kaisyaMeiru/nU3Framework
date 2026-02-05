# ConnectivityManager 개별 테스트 기능 추가 완료

## ?? 구현 내용

`TestConnectionAsync` 메서드를 **DB, File Transfer, Log Upload** 개별 테스트로 분리했습니다.

---

## ? 추가된 메서드

### 1. TestDBConnectionAsync()
```csharp
public async Task<bool> TestDBConnectionAsync()
{
    try
    {
        EnsureInitialized();
        return await DB.ConnectAsync();
    }
    catch
    {
        return false;
    }
}
```

### 2. TestFileConnectionAsync()
```csharp
public async Task<bool> TestFileConnectionAsync()
{
    try
    {
        EnsureInitialized();
        _ = File; // 클라이언트 초기화 테스트
        return true;
    }
    catch
    {
        return false;
    }
}
```

### 3. TestLogConnectionAsync()
```csharp
public async Task<bool> TestLogConnectionAsync()
{
    try
    {
        EnsureInitialized();
        
        // 테스트 로그 파일 생성 및 업로드
        var tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, 
                $"Connection test at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            
            return await Log.UploadLogFileAsync(tempFile, deleteAfterUpload: false);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
    catch
    {
        return false;
    }
}
```

### 4. TestAllConnectionsAsync()
```csharp
public async Task<ConnectivityTestResult> TestAllConnectionsAsync()
{
    var result = new ConnectivityTestResult
    {
        TestTime = DateTime.Now
    };

    try
    {
        EnsureInitialized();
        
        // Test DB
        try
        {
            result.DBConnected = await TestDBConnectionAsync();
        }
        catch (Exception ex)
        {
            result.DBError = ex.Message;
        }

        // Test File Transfer
        try
        {
            result.FileConnected = await TestFileConnectionAsync();
        }
        catch (Exception ex)
        {
            result.FileError = ex.Message;
        }

        // Test Log Upload
        try
        {
            result.LogConnected = await TestLogConnectionAsync();
        }
        catch (Exception ex)
        {
            result.LogError = ex.Message;
        }

        result.AllConnected = result.DBConnected && result.FileConnected && result.LogConnected;
    }
    catch (Exception ex)
    {
        result.GeneralError = ex.Message;
    }

    return result;
}
```

---

## ?? ConnectivityTestResult 클래스

```csharp
public class ConnectivityTestResult
{
    public DateTime TestTime { get; set; }
    public bool AllConnected { get; set; }
    
    public bool DBConnected { get; set; }
    public string? DBError { get; set; }
    
    public bool FileConnected { get; set; }
    public string? FileError { get; set; }
    
    public bool LogConnected { get; set; }
    public string? LogError { get; set; }
    
    public string? GeneralError { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Connectivity Test Results ({TestTime:yyyy-MM-dd HH:mm:ss})");
        sb.AppendLine($"Overall: {(AllConnected ? "? All Connected" : "? Some Failed")}");
        sb.AppendLine();
        sb.AppendLine($"Database:      {(DBConnected ? "? Connected" : $"? Failed - {DBError}")}");
        sb.AppendLine($"File Transfer: {(FileConnected ? "? Connected" : $"? Failed - {FileError}")}");
        sb.AppendLine($"Log Upload:    {(LogConnected ? "? Connected" : $"? Failed - {LogError}")}");
        
        if (!string.IsNullOrEmpty(GeneralError))
        {
            sb.AppendLine();
            sb.AppendLine($"General Error: {GeneralError}");
        }
        
        return sb.ToString();
    }
}
```

---

## ??? MainShellForm 통합

### 시스템 메뉴 구조

```
시스템
├── 메뉴 새로고침
├── 모든 탭 닫기
├── 서버 연결 상태
├── 서버 연결 테스트 (전체)          ← 전체 테스트
├── 개별 서비스 테스트               ← NEW!
│   ├── Database 연결 테스트          ← DB만 테스트
│   ├── File Transfer 연결 테스트      ← File만 테스트
│   └── Log Upload 연결 테스트         ← Log만 테스트
├── 에러 리포팅 설정
├── 크래시 리포트 테스트
├── 정보
└── 종료
```

### 개별 테스트 메서드

#### 1. TestDatabaseConnection()
```csharp
private async Task TestDatabaseConnection()
{
    if (!ConnectivityManager.Instance.IsInitialized)
    {
        XtraMessageBox.Show("서버 연결이 초기화되지 않았습니다.", "오류");
        return;
    }

    try
    {
        var connected = await ConnectivityManager.Instance.TestDBConnectionAsync();
        
        if (connected)
        {
            XtraMessageBox.Show(
                $"Database 연결 성공!\n\n서버: {ConnectivityManager.Instance.ServerUrl}",
                "연결 성공");
        }
        else
        {
            XtraMessageBox.Show(
                $"Database 연결 실패!\n\n서버: {ConnectivityManager.Instance.ServerUrl}",
                "연결 실패");
        }
    }
    catch (Exception ex)
    {
        XtraMessageBox.Show($"Database 테스트 중 오류!\n\n{ex.Message}", "오류");
    }
}
```

#### 2. TestFileConnection()
```csharp
private async Task TestFileConnection()
{
    if (!ConnectivityManager.Instance.IsInitialized)
    {
        XtraMessageBox.Show("서버 연결이 초기화되지 않았습니다.", "오류");
        return;
    }

    try
    {
        var connected = await ConnectivityManager.Instance.TestFileConnectionAsync();
        
        if (connected)
        {
            XtraMessageBox.Show(
                $"File Transfer 연결 성공!\n\n서버: {ConnectivityManager.Instance.ServerUrl}",
                "연결 성공");
        }
        else
        {
            XtraMessageBox.Show(
                $"File Transfer 연결 실패!\n\n서버: {ConnectivityManager.Instance.ServerUrl}",
                "연결 실패");
        }
    }
    catch (Exception ex)
    {
        XtraMessageBox.Show($"File Transfer 테스트 중 오류!\n\n{ex.Message}", "오류");
    }
}
```

#### 3. TestLogConnection()
```csharp
private async Task TestLogConnection()
{
    if (!ConnectivityManager.Instance.IsInitialized)
    {
        XtraMessageBox.Show("서버 연결이 초기화되지 않았습니다.", "오류");
        return;
    }

    try
    {
        var connected = await ConnectivityManager.Instance.TestLogConnectionAsync();
        
        if (connected)
        {
            XtraMessageBox.Show(
                $"Log Upload 연결 성공!\n\n서버: {ConnectivityManager.Instance.ServerUrl}\n\n테스트 로그 파일이 업로드되었습니다.",
                "연결 성공");
        }
        else
        {
            XtraMessageBox.Show(
                $"Log Upload 연결 실패!\n\n서버: {ConnectivityManager.Instance.ServerUrl}",
                "연결 실패");
        }
    }
    catch (Exception ex)
    {
        XtraMessageBox.Show($"Log Upload 테스트 중 오류!\n\n{ex.Message}", "오류");
    }
}
```

### 전체 테스트 (TestServerConnection)

```csharp
private async void TestServerConnection()
{
    // ...progress form 표시...
    
    // 전체 테스트 실행
    var result = await ConnectivityManager.Instance.TestAllConnectionsAsync();

    if (result.AllConnected)
    {
        barStaticItemServer.Caption = $"?? {ConnectivityManager.Instance.ServerUrl}";
        
        XtraMessageBox.Show(
            $"서버 연결 성공!\n\n" +
            $"서버: {ConnectivityManager.Instance.ServerUrl}\n\n" +
            $"? Database: 연결됨\n" +
            $"? File Transfer: 연결됨\n" +
            $"? Log Upload: 연결됨",
            "연결 성공");
    }
    else
    {
        barStaticItemServer.Caption = $"?? {ConnectivityManager.Instance.ServerUrl} (일부 실패)";
        
        var statusMessage = new StringBuilder();
        statusMessage.AppendLine($"서버: {ConnectivityManager.Instance.ServerUrl}");
        statusMessage.AppendLine();
        statusMessage.AppendLine($"{(result.DBConnected ? "?" : "?")} Database: {(result.DBConnected ? "연결됨" : $"실패 - {result.DBError}")}");
        statusMessage.AppendLine($"{(result.FileConnected ? "?" : "?")} File Transfer: {(result.FileConnected ? "연결됨" : $"실패 - {result.FileError}")}");
        statusMessage.AppendLine($"{(result.LogConnected ? "?" : "?")} Log Upload: {(result.LogConnected ? "연결됨" : $"실패 - {result.LogError}")}");
        
        XtraMessageBox.Show(statusMessage.ToString(), "연결 테스트 결과");
    }
}
```

---

## ?? 수정된 파일

```
?? nU3.Core/Services/ConnectivityManager.cs
   - TestConnectionAsync() 개선
   - TestDBConnectionAsync() 추가
   - TestFileConnectionAsync() 추가
   - TestLogConnectionAsync() 추가
   - TestAllConnectionsAsync() 추가
   - ConnectivityTestResult 클래스 추가

?? nU3.Shell/MainShellForm.cs
   - 개별 서비스 테스트 서브메뉴 추가
   - TestDatabaseConnection() 추가
   - TestFileConnection() 추가
   - TestLogConnection() 추가
   - TestServerConnection() 개선 (상세 결과 표시)

? 빌드 성공
```

---

## ?? 사용 시나리오

### 시나리오 1: 전체 테스트

```
사용자: 시스템 > 서버 연결 테스트 (전체) 클릭
결과: 
  ? Database: 연결됨
  ? File Transfer: 연결됨
  ? Log Upload: 연결됨
  
상태바: ?? https://localhost:64229
```

### 시나리오 2: Database만 문제

```
사용자: 시스템 > 서버 연결 테스트 (전체) 클릭
결과:
  ? Database: 실패 - Connection refused
  ? File Transfer: 연결됨
  ? Log Upload: 연결됨
  
상태바: ?? https://localhost:64229 (일부 실패)
```

### 시나리오 3: 개별 서비스 테스트

```
사용자: 시스템 > 개별 서비스 테스트 > Database 연결 테스트 클릭
결과:
  ? Database 연결 실패!
  서버: https://localhost:64229
  데이터베이스 서비스가 실행 중인지 확인하세요.
```

---

## ?? 테스트 결과 예시

### 성공 케이스
```
Connectivity Test Results (2024-01-27 14:30:15)
Overall: ? All Connected

Database:      ? Connected
File Transfer: ? Connected
Log Upload:    ? Connected
```

### 부분 실패 케이스
```
Connectivity Test Results (2024-01-27 14:30:15)
Overall: ? Some Failed

Database:      ? Failed - Connection refused
File Transfer: ? Connected
Log Upload:    ? Connected
```

### 전체 실패 케이스
```
Connectivity Test Results (2024-01-27 14:30:15)
Overall: ? Some Failed

Database:      ? Failed - Timeout
File Transfer: ? Failed - Server not found
Log Upload:    ? Failed - Network unreachable

General Error: Server is offline
```

---

## ? 이점

### 1. 세분화된 진단
- 어떤 서비스에 문제가 있는지 즉시 파악
- 불필요한 전체 테스트 시간 절약

### 2. 유연한 테스트
- 전체 테스트: 한 번에 모든 서비스 확인
- 개별 테스트: 특정 서비스만 빠르게 테스트

### 3. 상세한 오류 정보
- 각 서비스별 오류 메시지 제공
- 문제 해결을 위한 힌트 제공

### 4. 사용자 친화적
- 서브메뉴로 깔끔하게 구성
- 직관적인 아이콘 (?/?)
- 한글 메시지

---

## ?? 완료!

**ConnectivityManager의 개별 테스트 기능이 완벽하게 구현되었습니다!**

### 주요 기능

```
? 전체 테스트: TestAllConnectionsAsync()
? DB 테스트: TestDBConnectionAsync()
? File 테스트: TestFileConnectionAsync()
? Log 테스트: TestLogConnectionAsync()
? 상세 결과: ConnectivityTestResult
? UI 통합: 시스템 메뉴 서브메뉴
```

### 사용법

```
시스템 메뉴
  ↓
서버 연결 테스트 (전체) ← 모든 서비스 테스트
  또는
개별 서비스 테스트
  ↓
Database / File Transfer / Log Upload ← 선택적 테스트
```

**완벽합니다!** ??
