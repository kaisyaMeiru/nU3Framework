# ConnectivityManager 리팩토링 요약

## 주요 변경 사항

### 1. 동시성 지원
- **Connection Pooling**: `SemaphoreSlim`을 사용한 동시 연결 제한
- **SocketsHttpHandler**: HTTP/2 다중 연결 지원
- **Pooled Clients**: `PooledDBClient`, `PooledFileClient` - 동시 작업용 클라이언트

### 2. 배치 작업 지원
- `ExecuteBatchQueriesAsync()`: 여러 DB 쿼리 동시 실행
- `UploadFilesAsync()`: 여러 파일 동시 업로드
- `DownloadFilesAsync()`: 여러 파일 동시 다운로드

### 3. Progress 및 Cancellation 지원
- `IProgress<BatchOperationProgress>`: 진행률 리포팅
- `CancellationToken`: 작업 취소 지원
- `ProgressChanged` 이벤트: 실시간 진행 상황

---

## 사용 예시

### 기존 방식 (단순 작업)
```csharp
// 기존과 동일하게 사용 가능
var data = await ConnectivityManager.Instance.DB.ExecuteDataTableAsync("SELECT * FROM USERS");
await ConnectivityManager.Instance.File.UploadFileAsync("local.txt", "server/remote.txt");
```

### 동시 DB 쿼리 (여러 쿼리 병렬 실행)
```csharp
var queries = new BatchQueryRequest[]
{
    new() { QueryId = "Query1", CommandText = "SELECT * FROM USERS" },
    new() { QueryId = "Query2", CommandText = "SELECT * FROM ORDERS" },
    new() { QueryId = "Query3", CommandText = "SELECT * FROM PRODUCTS" }
};

// Progress 콜백과 함께 실행
var progress = new Progress<BatchOperationProgress>(p =>
{
    Console.WriteLine($"Progress: {p.CompletedItems}/{p.TotalItems} ({p.PercentComplete}%)");
});

var results = await ConnectivityManager.Instance.ExecuteBatchQueriesAsync(
    queries,
    progress,
    cancellationToken);

foreach (var result in results)
{
    if (result.Success)
        Console.WriteLine($"{result.QueryId}: {result.Data?.Rows.Count} rows in {result.ExecutionTime.TotalMilliseconds}ms");
    else
        Console.WriteLine($"{result.QueryId}: Failed - {result.Error}");
}
```

### 동시 파일 업로드
```csharp
var files = new BatchFileRequest[]
{
    new() { FileId = "File1", LocalPath = @"C:\local\file1.txt", ServerPath = "uploads/file1.txt" },
    new() { FileId = "File2", LocalPath = @"C:\local\file2.txt", ServerPath = "uploads/file2.txt" },
    new() { FileId = "File3", LocalPath = @"C:\local\file3.txt", ServerPath = "uploads/file3.txt" }
};

var cts = new CancellationTokenSource();
var results = await ConnectivityManager.Instance.UploadFilesAsync(files, progress, cts.Token);
```

### Pooled Client (개별 동시 작업)
```csharp
// 여러 스레드에서 동시에 사용 가능
var tasks = Enumerable.Range(0, 10).Select(async i =>
{
    using var pooledClient = await ConnectivityManager.Instance.CreateDBClientAsync();
    return await pooledClient.Client.ExecuteDataTableAsync($"SELECT * FROM TABLE_{i}");
});

var results = await Task.WhenAll(tasks);
```

---

## UI 연동 (AsyncOperationHelper)

### 진행률 표시 대화상자와 함께 실행
```csharp
// WinForms에서 사용
try
{
    var results = await AsyncOperationHelper.ExecuteWithProgressAsync(
        this,  // 부모 폼
        "데이터 로딩 중...",
        async (cancellationToken, progress) =>
        {
            return await ConnectivityManager.Instance.ExecuteBatchQueriesAsync(
                queries,
                progress,
                cancellationToken);
        },
        allowCancel: true);
    
    // 결과 처리
    ProcessResults(results);
}
catch (OperationCanceledException)
{
    XtraMessageBox.Show("작업이 취소되었습니다.", "취소");
}
```

### 단순 대기 표시
```csharp
var data = await AsyncOperationHelper.ExecuteWithWaitAsync(
    this,
    "데이터 조회 중...",
    async (ct) => await ConnectivityManager.Instance.DB.ExecuteDataTableAsync("SELECT * FROM USERS"),
    allowCancel: false);
```

---

## 구성 옵션

```csharp
// 초기화 시 동시 연결 수 설정
ConnectivityManager.Instance.Initialize(
    serverUrl: "https://server:5000",
    enableLogCompression: true,
    maxConcurrentConnections: 10  // 동시 연결 최대 수
);

// 런타임 변경
ConnectivityManager.Instance.MaxConcurrentConnections = 20;
ConnectivityManager.Instance.DefaultTimeout = TimeSpan.FromMinutes(10);
```

---

## 아키텍처

```
┌─────────────────────────────────────────────────────┐
│              ConnectivityManager                     │
├─────────────────────────────────────────────────────┤
│  ┌───────────────┐  ┌───────────────────────────┐   │
│  │ Default       │  │ Connection Pool           │   │
│  │ Clients       │  │ (SemaphoreSlim)           │   │
│  │ - DB          │  │ ┌─────┐ ┌─────┐ ┌─────┐   │   │
│  │ - File        │  │ │Pool1│ │Pool2│ │Pool3│   │   │
│  │ - Log         │  │ └─────┘ └─────┘ └─────┘   │   │
│  └───────────────┘  └───────────────────────────┘   │
├─────────────────────────────────────────────────────┤
│  Batch Operations                                   │
│  - ExecuteBatchQueriesAsync()                       │
│  - UploadFilesAsync()                               │
│  - DownloadFilesAsync()                             │
├─────────────────────────────────────────────────────┤
│  Events                                             │
│  - LogMessage                                       │
│  - ProgressChanged                                  │
└─────────────────────────────────────────────────────┘
```

---

## 관련 파일

| 파일 | 설명 |
|------|------|
| `nU3.Core\Services\ConnectivityManager.cs` | 리팩토링된 연결 관리자 |
| `nU3.Core.UI\Shell\AsyncOperationHelper.cs` | UI 연동 헬퍼 및 진행률 폼 |
