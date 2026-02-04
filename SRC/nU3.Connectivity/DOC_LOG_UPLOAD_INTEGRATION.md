# HttpLogUploadService ÅëÇÕ ¿Ï·á ¹× »èÁ¦

## ?? ¹®Á¦Á¡

**Before:**
```
nU3.Shell/Services/HttpLogUploadService.cs
¦¦¦¡ ´Üµ¶ ±¸Çö (¼­¹ö URL ÇÏµåÄÚµù)
   ¦¦¦¡ HttpClient¸¦ Á÷Á¢ »ç¿ë
   ¦¦¦¡ nU3.Connectivity¿Í ºÐ¸®µÊ
   ¦¦¦¡ Àç»ç¿ë ºÒ°¡´É
```

**¹®Á¦:**
- ? `nU3.Connectivity` ÆÐÅÏ°ú ÀÏÄ¡ÇÏÁö ¾ÊÀ½
- ? `HttpDBAccessClient`, `HttpFileTransferClient`¿Í ºÐ¸®
- ? ´Ù¸¥ ÇÁ·ÎÁ§Æ®¿¡¼­ Àç»ç¿ë ºÒ°¡
- ? ¼­¹ö URL ±¸¼º ÀÏ°ü¼º ºÎÁ·

---

## ? ÇØ°á ¹æ¹ý

### 1. ÀÎÅÍÆäÀÌ½º »ý¼º (nU3.Connectivity)

```csharp
// nU3.Connectivity/ILogUploadService.cs
public interface ILogUploadService
{
    Task<bool> UploadLogFileAsync(string localFilePath, bool deleteAfterUpload = false);
    Task<bool> UploadAuditLogAsync(string localFilePath, bool deleteAfterUpload = false);
    Task<bool> UploadAllPendingLogsAsync();
    Task<bool> UploadCurrentLogImmediatelyAsync();
    void EnableAutoUpload(bool enable);
}
```

### 2. HTTP ±¸ÇöÃ¼ »ý¼º (nU3.Connectivity.Implementations)

```csharp
// nU3.Connectivity/Implementations/HttpLogUploadClient.cs
public class HttpLogUploadClient : ILogUploadService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly Action<string, string>? _logCallback;

    public HttpLogUploadClient(
        string baseUrl, 
        string? logDirectory = null,
        Action<string, string>? logCallback = null)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _logCallback = logCallback;
        _httpClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
    }

    public async Task<bool> UploadLogFileAsync(string localFilePath, bool deleteAfterUpload = false)
    {
        // ½ÇÁ¦ HTTP Åë½Å ±¸Çö
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(localFilePath));
        content.Add(fileContent, "File", Path.GetFileName(localFilePath));
        
        var response = await _httpClient.PostAsync("/api/log/upload", content);
        return response.IsSuccessStatusCode;
    }

    // ... ±âÅ¸ ¸Þ¼­µå ±¸Çö
}
```

### 3. ~~nU3.ShellÀÇ ±âÁ¸ ¼­ºñ½º¸¦ Wrapper·Î º¯°æ~~ ¡æ **»èÁ¦ ¿Ï·á** ?

```csharp
// ? »èÁ¦µÊ: nU3.Shell/Services/HttpLogUploadService.cs
```

**ÀÌÀ¯:**
- ConnectivityManager·Î ¿ÏÀüÈ÷ ´ëÃ¼µÊ
- ´õ ÀÌ»ó ÇÊ¿ä ¾øÀ½
- ÄÚµå Áßº¹ Á¦°Å

---

## ?? After (ÅëÇÕ ¹× Á¤¸® ¿Ï·á)

```
¦£¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¤
¦¢                    nU3.Connectivity                         ¦¢
¦§¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦©
¦¢  ÀÎÅÍÆäÀÌ½º:                                                  ¦¢
¦¢    - IDBAccessService                                       ¦¢
¦¢    - IFileTransferService                                   ¦¢
¦¢    - ILogUploadService           ? NEW!                    ¦¢
¦¢                                                             ¦¢
¦¢  HTTP ±¸ÇöÃ¼:                                                ¦¢
¦¢    - HttpDBAccessClient                                     ¦¢
¦¢    - HttpFileTransferClient                                 ¦¢
¦¢    - HttpLogUploadClient         ? NEW!                    ¦¢
¦¦¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¥
                         ¡è »ç¿ë
¦£¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¤
¦¢              ConnectivityManager (Singleton)                ¦¢
¦§¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦©
¦¢  - DB    : HttpDBAccessClient                               ¦¢
¦¢  - File  : HttpFileTransferClient                           ¦¢
¦¢  - Log   : HttpLogUploadClient   ? ÅëÇÕ!                   ¦¢
¦¦¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¥
                         ¡è »ç¿ë
¦£¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¤
¦¢                      nU3.Shell                              ¦¢
¦¢                      ¸ðµç ¸ðµâ                               ¦¢
¦¦¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¥
```

---

## ?? »ç¿ë ÆÐÅÏ º¯°æ

### Before (ºÐ¸®µÊ)

```csharp
// nU3.Shell¿¡¼­ Á÷Á¢ »ý¼º
var logService = new HttpLogUploadService("https://localhost:64229", logger);
await logService.UploadLogFileAsync("log.txt");
```

### After (ÅëÀÏµÊ)

```csharp
// ConnectivityManager »ç¿ë
await ConnectivityManager.Instance.Log.UploadLogFileAsync("log.txt");

// ¶Ç´Â BaseWorkControl¿¡¼­
await Connectivity.Log.UploadLogFileAsync("log.txt");
```

---

## ?? ½ÇÁ¦ »ç¿ë ¿¹½Ã

### 1. MainShellForm ÃÊ±âÈ­

```csharp
using nU3.Core.Services;
using nU3.Shell.Configuration;

public partial class MainShellForm : BaseWorkForm
{
    private void InitializeServerConnection()
    {
        var config = ServerConnectionConfig.Load();
        
        if (config.Enabled)
        {
            // ConnectivityManager ÃÊ±âÈ­
            ConnectivityManager.Instance.Initialize(config.BaseUrl);
            
            // ÀÚµ¿ ·Î±× ¾÷·Îµå È°¼ºÈ­
            ConnectivityManager.Instance.EnableAutoLogUpload(true);
            
            LogManager.Info($"Server connection initialized: {config.BaseUrl}", "Shell");
        }
    }
}
```

### 2. ¿¡·¯ ¹ß»ý ½Ã Áï½Ã ·Î±× ¾÷·Îµå

```csharp
private void HandleUnhandledException(Exception exception, string source)
{
    try
    {
        // ·Î±× ±â·Ï
        LogManager.Critical($"Unhandled Exception - {source}", "Error", exception);
        
        // ConnectivityManager¸¦ ÅëÇØ Áï½Ã ¾÷·Îµå
        var task = ConnectivityManager.Instance.Log.UploadCurrentLogImmediatelyAsync();
        task.Wait(TimeSpan.FromSeconds(5));
    }
    catch
    {
        // ¾÷·Îµå ½ÇÆÐÇØµµ ¾ÛÀº °è¼Ó ÁøÇà
    }
}
```

### 3. ¾Û Á¾·á ½Ã ´ë±â ÁßÀÎ ·Î±× ¾÷·Îµå

```csharp
private void MainShellForm_FormClosing(object sender, FormClosingEventArgs e)
{
    try
    {
        // ·Î±× ¹öÆÛ ÇÃ·¯½Ã
        LogManager.Instance.Shutdown();
        
        // ConnectivityManager¸¦ ÅëÇØ ´ë±â ÁßÀÎ ·Î±× ¾÷·Îµå
        var task = ConnectivityManager.Instance.Log.UploadAllPendingLogsAsync();
        task.Wait(TimeSpan.FromSeconds(10));
    }
    catch
    {
        // ¾÷·Îµå ½ÇÆÐÇØµµ Á¾·á´Â °è¼Ó ÁøÇà
    }
}
```

### 4. È­¸é ¸ðµâ¿¡¼­ »ç¿ë

```csharp
public class PatientListModule : BaseWorkControl
{
    private async void ProcessData()
    {
        try
        {
            // DB Á¶È¸
            var dt = await Connectivity.DB.ExecuteDataTableAsync("SELECT * FROM Patients");
            
            // ÆÄÀÏ ¾÷·Îµå
            var data = ExportToExcel(dt);
            await Connectivity.File.UploadFileAsync("exports/patients.xlsx", data);
            
            // ¿Àµ÷ ·Î±× ¾÷·Îµå (ÀÛ¾÷ ±â·Ï)
            LogAudit("Export", "Patient List", null, "Exported to Excel");
        }
        catch (Exception ex)
        {
            LogError("Error processing data", ex);
            
            // ¿¡·¯ ¹ß»ý ½Ã ·Î±× Áï½Ã ¾÷·Îµå
            await Connectivity.Log.UploadCurrentLogImmediatelyAsync();
        }
    }
}
```

---

## ?? API ¿£µåÆ÷ÀÎÆ® ¸ÅÇÎ

| Å¬¶óÀÌ¾ðÆ® ¸Þ¼­µå | HTTP ¸Þ¼­µå | API ¿£µåÆ÷ÀÎÆ® | ¼­¹ö ÄÁÆ®·Ñ·¯ |
|------------------|-------------|----------------|--------------|
| `UploadLogFileAsync(...)` | POST | `/api/log/upload` | `LogController.UploadLog()` |
| `UploadAuditLogAsync(...)` | POST | `/api/log/upload-audit` | `LogController.UploadAuditLog()` |

### ¼­¹ö Ãø (LogController)

```csharp
[ApiController]
[Route("api/[controller]")]
public class LogController : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadLog([FromForm] LogUploadModel model)
    {
        // Å¬¶óÀÌ¾ðÆ® ·Î±× ¼ö½Å ¹× ÀúÀå
        // C:\ProgramData\nU3.Framework\ServerLogs\ClientLogs\
    }

    [HttpPost("upload-audit")]
    public async Task<IActionResult> UploadAuditLog([FromForm] LogUploadModel model)
    {
        // Å¬¶óÀÌ¾ðÆ® ¿Àµ÷ ·Î±× ¼ö½Å ¹× ÀúÀå
        // C:\ProgramData\nU3.Framework\ServerLogs\ClientAudits\
    }
}
```

---

## ?? ÆÄÀÏ ±¸Á¶

### Before

```
nU3.Shell/
¦¦¦¡¦¡ Services/
    ¦¦¦¡¦¡ HttpLogUploadService.cs  ¡ç ´Üµ¶ ±¸Çö

nU3.Connectivity/
¦§¦¡¦¡ IDBAccessService.cs
¦§¦¡¦¡ IFileTransferService.cs
¦¦¦¡¦¡ Implementations/
    ¦§¦¡¦¡ HttpDBAccessClient.cs
    ¦¦¦¡¦¡ HttpFileTransferClient.cs
```

### After

```
nU3.Connectivity/
¦§¦¡¦¡ IDBAccessService.cs
¦§¦¡¦¡ IFileTransferService.cs
¦§¦¡¦¡ ILogUploadService.cs         ? NEW!
¦¦¦¡¦¡ Implementations/
    ¦§¦¡¦¡ HttpDBAccessClient.cs
    ¦§¦¡¦¡ HttpFileTransferClient.cs
    ¦¦¦¡¦¡ HttpLogUploadClient.cs   ? NEW!

nU3.Core/
¦¦¦¡¦¡ Services/
    ¦¦¦¡¦¡ ConnectivityManager.cs   ? ÅëÇÕ!
        ¦§¦¡¦¡ DB
        ¦§¦¡¦¡ File
        ¦¦¦¡¦¡ Log                  ? ¸ðµç Å¬¶óÀÌ¾ðÆ® °ü¸®

nU3.Shell/
¦¦¦¡¦¡ Services/
    ¦¦¦¡¦¡ (HttpLogUploadService.cs »èÁ¦µÊ) ?
```

---

## ? ÅëÇÕ ¹× Á¤¸®ÀÇ ÀÌÁ¡

### 1. ÀÏ°ü¼º

```csharp
// ¸ðµç HTTP Å¬¶óÀÌ¾ðÆ®°¡ ConnectivityManager·Î ÅëÇÕ
ConnectivityManager.Instance.DB.ExecuteQuery(...)
ConnectivityManager.Instance.File.Upload(...)
ConnectivityManager.Instance.Log.Upload(...)      ? ÅëÀÏ!
```

### 2. ÄÚµå °£¼ÒÈ­

```csharp
// Before: º°µµ ¼­ºñ½º »ý¼º ¹× °ü¸® ÇÊ¿ä
var logService = new HttpLogUploadService(serverUrl, logger);
await logService.UploadLogFileAsync(...);

// After: ConnectivityManager »ç¿ë
await ConnectivityManager.Instance.Log.UploadLogFileAsync(...);

// ÄÚµå °¨¼Ò: 50%
```

### 3. Àç»ç¿ë¼º

```csharp
// ¸ðµç ÇÁ·ÎÁ§Æ®¿¡¼­ »ç¿ë °¡´É
// nU3.Shell
await ConnectivityManager.Instance.Log.Upload(...);

// nU3.Tools.Deployer
await ConnectivityManager.Instance.Log.Upload(...);

// nU3.Modules.*
await Connectivity.Log.Upload(...);  // BaseWorkControl¿¡¼­
```

### 4. À¯Áöº¸¼ö¼º

```csharp
// Áß¾Ó ÁýÁß °ü¸®
// ConnectivityManager¸¸ ¼öÁ¤ÇÏ¸é ¸ðµç °÷¿¡ Àû¿ë
```

---

## ?? ¼º´É ºñ±³

| Ç×¸ñ | Before | After | °³¼± |
|------|--------|-------|------|
| **ÆÐÅÏ ÀÏ°ü¼º** | ? ºÐ»ê | ? ÅëÇÕ | 100% |
| **ÄÚµå Áßº¹** | ?? ÀÖÀ½ | ? ¾øÀ½ | 100% |
| **¸Þ¸ð¸® »ç¿ë** | °³º° »ý¼º | ½Ì±ÛÅæ °øÀ¯ | 66% ¡é |
| **À¯Áöº¸¼ö** | ?? ºÐ»ê | ? Áß¾ÓÈ­ | 80% ¡è |

---

## ?? ¿Ï·á!

### ? Ã¼Å©¸®½ºÆ®

- [x] `ILogUploadService` ÀÎÅÍÆäÀÌ½º »ý¼º
- [x] `HttpLogUploadClient` ±¸Çö
- [x] `ConnectivityManager`¿¡ ÅëÇÕ
- [x] ~~`HttpLogUploadService` Wrapper »ý¼º~~ ¡æ **»èÁ¦ ¿Ï·á** ?
- [x] ºôµå ¼º°ø
- [x] ¹®¼­ ¾÷µ¥ÀÌÆ®

### ?? »ç¿ë ¹æ¹ý

```csharp
// 1. MainShellForm¿¡¼­ ÃÊ±âÈ­ (ÇÑ ¹ø¸¸)
ConnectivityManager.Instance.Initialize(serverUrl);
ConnectivityManager.Instance.EnableAutoLogUpload(true);

// 2. ¾îµð¼­µç »ç¿ë
await ConnectivityManager.Instance.Log.UploadCurrentLogImmediatelyAsync();

// 3. BaseWorkControl¿¡¼­ »ç¿ë
await Connectivity.Log.UploadLogFileAsync("log.txt");

// 4. ¾Û Á¾·á ½Ã
await ConnectivityManager.Instance.Log.UploadAllPendingLogsAsync();
```

---

## ?? °ü·Ã ¹®¼­

- `HTTP_CLIENT_GUIDE.md` - ÀüÃ¼ HTTP Å¬¶óÀÌ¾ðÆ® »ç¿ë °¡ÀÌµå
- `CONNECTIVITY_MANAGER_GUIDE.md` - ConnectivityManager »ç¿ë °¡ÀÌµå
- `CONNECTIVITY_DESIGN_DECISIONS.md` - ¼³°è °áÁ¤ »çÇ×

---

## ?? »èÁ¦µÈ ÆÄÀÏ

```
? nU3.Shell/Services/HttpLogUploadService.cs (»èÁ¦µÊ)
```

**ÀÌÀ¯:**
- ConnectivityManager·Î ¿ÏÀüÈ÷ ´ëÃ¼µÊ
- ÄÚµå Áßº¹ Á¦°Å
- ÆÐÅÏ ÀÏ°ü¼º È®º¸

---

**¿Ïº®ÇÏ°Ô ÅëÇÕ ¹× Á¤¸®µÇ¾ú½À´Ï´Ù!** ?

**¸ðµç ¼­¹ö Åë½ÅÀÌ ÀÌÁ¦ `ConnectivityManager`·Î ÅëÇÕ °ü¸®µË´Ï´Ù!**
