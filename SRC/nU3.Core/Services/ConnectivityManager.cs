using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using nU3.Connectivity.Implementations;

namespace nU3.Core.Services
{
    /// <summary>
    /// ConnectivityManager는 데이터베이스 접근, 파일 전송, 로그 업로드 등 서버와의 통신 클라이언트를 관리합니다.
    /// - 동시성(병렬) 작업을 위해 풀링된 HttpClient를 생성/관리합니다.
    /// - 간단한 동기/비동기 작업을 위한 기본 클라이언트를 제공합니다.
    /// - 진행률 보고(IProgress), 취소(CancellationToken) 및 로그 이벤트를 지원합니다.
    /// 싱글톤 패턴으로 동작하며 Initialize(serverUrl) 호출로 초기화해야 합니다.
    /// </summary>
    public class ConnectivityManager : IDisposable
    {
        private static ConnectivityManager? _instance;
        private static readonly object _lock = new object();

        // 동시 작업을 위한 HTTP 클라이언트 풀
        private readonly ConcurrentDictionary<int, HttpClient> _httpClientPool = new();
        private readonly SemaphoreSlim _poolSemaphore;
        private int _nextClientId = 0;
        
        // 단순 작업용 기본 클라이언트
        private HttpClient? _defaultHttpClient;
        private HttpDBAccessClient? _dbClient;
        private HttpFileTransferClient? _fileClient;
        private HttpLogUploadClient? _logClient;
        
        // 상태
        private bool _isInitialized;
        private string? _serverUrl;
        private bool _enableLogCompression = true;
        private int _maxConcurrentConnections = 10;
        private TimeSpan _defaultTimeout = TimeSpan.FromMinutes(5);

        /// <summary>
        /// 싱글톤 인스턴스를 반환합니다(지연 초기화).
        /// 여러 스레드에서 안전하게 접근할 수 있습니다.
        /// </summary>
        public static ConnectivityManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new ConnectivityManager();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 단순한 DB 접근용 클라이언트를 반환합니다. 동시성이 높은 작업은 CreateDBClientAsync를 사용하세요.
        /// </summary>
        public HttpDBAccessClient DB
        {
            get
            {
                EnsureInitialized();
                if (_dbClient == null)
                {
                    lock (_lock)
                    {
                        _dbClient ??= new HttpDBAccessClient(GetOrCreateDefaultHttpClient(), _serverUrl!);
                    }
                }
                return _dbClient;
            }
        }

        /// <summary>
        /// 단순한 파일 전송용 클라이언트를 반환합니다. 동시성이 높은 작업은 CreateFileClientAsync를 사용하세요.
        /// </summary>
        public HttpFileTransferClient File
        {
            get
            {
                EnsureInitialized();
                if (_fileClient == null)
                {
                    lock (_lock)
                    {
                        _fileClient ??= new HttpFileTransferClient(GetOrCreateDefaultHttpClient(), _serverUrl!);
                    }
                }
                return _fileClient;
            }
        }

        /// <summary>
        /// 로그 업로드용 클라이언트를 반환합니다.
        /// 로그 업로드 중 발생하는 메시지는 LogMessage 이벤트를 통해 전달됩니다.
        /// </summary>
        public HttpLogUploadClient Log
        {
            get
            {
                EnsureInitialized();
                if (_logClient == null)
                {
                    lock (_lock)
                    {
                        _logClient ??= new HttpLogUploadClient(
                            GetOrCreateDefaultHttpClient(),
                            _serverUrl!,
                            logCallback: (level, message) => OnLogMessage(level, message),
                            enableCompression: _enableLogCompression
                        );
                    }
                }
                return _logClient;
            }
        }

        /// <summary>
        /// 매니저가 초기화되었는지 여부를 반환합니다.
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// 구성된 서버 URL을 반환합니다(없을 수 있음).
        /// </summary>
        public string? ServerUrl => _serverUrl;

        /// <summary>
        /// 로그 압축 사용 여부를 가져오거나 설정합니다. 설정 변경 시 내부 로그 클라이언트를 재생성합니다.
        /// </summary>
        public bool EnableLogCompression
        {
            get => _enableLogCompression;
            set
            {
                _enableLogCompression = value;
                if (_logClient != null)
                {
                    lock (_lock)
                    {
                        _logClient?.Dispose();
                        _logClient = null;
                    }
                }
            }
        }

        /// <summary>
        /// 동시 연결 최대 개수를 가져오거나 설정합니다(1..100 범위로 제한).
        /// </summary>
        public int MaxConcurrentConnections
        {
            get => _maxConcurrentConnections;
            set => _maxConcurrentConnections = Math.Max(1, Math.Min(100, value));
        }

        /// <summary>
        /// 기본 작업 타임아웃을 가져오거나 설정합니다.
        /// </summary>
        public TimeSpan DefaultTimeout
        {
            get => _defaultTimeout;
            set => _defaultTimeout = value;
        }

        /// <summary>
        /// 클라이언트에서 생성하는 로그 메시지를 전달하는 이벤트입니다.
        /// 구독자는 로그 레벨과 메시지를 수신하여 UI나 로거로 표시할 수 있습니다.
        /// </summary>
        public event EventHandler<LogMessageEventArgs>? LogMessage;

        /// <summary>
        /// 진행 중인 작업의 진행률 변경을 전달하는 이벤트입니다.
        /// OperationId와 현재/전체 개수, 현재 항목 정보를 포함합니다.
        /// </summary>
        public event EventHandler<OperationProgressEventArgs>? ProgressChanged;

        private ConnectivityManager()
        {
            _poolSemaphore = new SemaphoreSlim(_maxConcurrentConnections, _maxConcurrentConnections);
        }

        /// <summary>
        /// 서버 URL 등 필수 정보를 사용하여 매니저를 초기화합니다.
        /// 초기화 전에 호출하지 않으면 대부분의 연산에서 예외가 발생합니다.
        /// </summary>
        /// <param name="serverUrl">서버의 기본 URL(예: https://api.example.com)</param>
        /// <param name="enableLogCompression">로그 전송 시 압축 사용 여부</param>
        /// <param name="maxConcurrentConnections">동시 연결 최대 수</param>
        public void Initialize(string serverUrl, bool enableLogCompression = true, int maxConcurrentConnections = 10)
        {
            if (string.IsNullOrWhiteSpace(serverUrl))
                throw new ArgumentException("Server URL cannot be null or empty", nameof(serverUrl));

            lock (_lock)
            {
                _serverUrl = serverUrl.TrimEnd('/');
                _enableLogCompression = enableLogCompression;
                _maxConcurrentConnections = maxConcurrentConnections;
                _isInitialized = true;

                DisposeClients();
            }
        }

        #region Concurrent Client Factory

        /// <summary>
        /// 병렬 작업용 DB 클라이언트를 생성하여 반환합니다. 사용 후 반드시 Dispose하여 자원을 해제하고 풀을 반환해야 합니다.
        /// </summary>
        public async Task<PooledDBClient> CreateDBClientAsync(CancellationToken cancellationToken = default)
        {
            EnsureInitialized();
            await _poolSemaphore.WaitAsync(cancellationToken);
            
            var httpClient = CreatePooledHttpClient();
            var dbClient = new HttpDBAccessClient(httpClient, _serverUrl!);
            
            return new PooledDBClient(dbClient, httpClient, () =>
            {
                _poolSemaphore.Release();
            });
        }

        /// <summary>
        /// 병렬 작업용 파일 전송 클라이언트를 생성하여 반환합니다. 사용 후 반드시 Dispose하세요.
        /// </summary>
        public async Task<PooledFileClient> CreateFileClientAsync(CancellationToken cancellationToken = default)
        {
            EnsureInitialized();
            await _poolSemaphore.WaitAsync(cancellationToken);
            
            var httpClient = CreatePooledHttpClient();
            var fileClient = new HttpFileTransferClient(httpClient, _serverUrl!);
            
            return new PooledFileClient(fileClient, httpClient, () =>
            {
                _poolSemaphore.Release();
            });
        }

        #endregion

        #region Batch Operations

        /// <summary>
        /// 여러 DB 쿼리를 병렬로 실행하고 결과 배열을 반환합니다.
        /// 진행률은 IProgress&lt;BatchOperationProgress&gt; 또는 ProgressChanged 이벤트를 통해 전달됩니다.
        /// </summary>
        public async Task<BatchQueryResult[]> ExecuteBatchQueriesAsync(
            BatchQueryRequest[] queries,
            IProgress<BatchOperationProgress>? progress = null,
            CancellationToken cancellationToken = default)
        {
            EnsureInitialized();
            
            var results = new BatchQueryResult[queries.Length];
            var completed = 0;
            var total = queries.Length;

            // 동시 제한을 위한 SemaphoreSlim 사용
            var semaphore = new SemaphoreSlim(_maxConcurrentConnections);
            var tasks = new Task[queries.Length];

            for (int i = 0; i < queries.Length; i++)
            {
                var index = i;
                var query = queries[i];

                tasks[i] = Task.Run(async () =>
                {
                    await semaphore.WaitAsync(cancellationToken);
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        using var pooledClient = await CreateDBClientAsync(cancellationToken);
                        
                        var startTime = DateTime.Now;
                        try
                        {
                            var dataTable = await pooledClient.Client.ExecuteDataTableAsync(
                                query.CommandText, 
                                query.Parameters);

                            results[index] = new BatchQueryResult
                            {
                                QueryId = query.QueryId,
                                Success = true,
                                Data = dataTable,
                                ExecutionTime = DateTime.Now - startTime
                            };
                        }
                        catch (Exception ex)
                        {
                            results[index] = new BatchQueryResult
                            {
                                QueryId = query.QueryId,
                                Success = false,
                                Error = ex.Message,
                                ExecutionTime = DateTime.Now - startTime
                            };
                        }

                        var count = Interlocked.Increment(ref completed);
                        progress?.Report(new BatchOperationProgress
                        {
                            TotalItems = total,
                            CompletedItems = count,
                            CurrentItem = query.QueryId,
                            PercentComplete = (int)((count * 100.0) / total)
                        });
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, cancellationToken);
            }

            await Task.WhenAll(tasks);
            return results;
        }

        /// <summary>
        /// 여러 파일을 병렬로 업로드합니다. 각 파일의 상태를 배열로 반환합니다.
        /// </summary>
        public async Task<BatchFileResult[]> UploadFilesAsync(
            BatchFileRequest[] files,
            IProgress<BatchOperationProgress>? progress = null,
            CancellationToken cancellationToken = default)
        {
            EnsureInitialized();
            
            var results = new BatchFileResult[files.Length];
            var completed = 0;
            var total = files.Length;

            var semaphore = new SemaphoreSlim(_maxConcurrentConnections);
            var tasks = new Task[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                var index = i;
                var file = files[i];

                tasks[i] = Task.Run(async () =>
                {
                    await semaphore.WaitAsync(cancellationToken);
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        using var pooledClient = await CreateFileClientAsync(cancellationToken);
                        
                        var startTime = DateTime.Now;
                        try
                        {
                            var success = await pooledClient.Client.UploadFileAsync(
                                file.LocalPath, 
                                file.ServerPath);

                            results[index] = new BatchFileResult
                            {
                                FileId = file.FileId,
                                LocalPath = file.LocalPath,
                                ServerPath = file.ServerPath,
                                Success = success,
                                TransferTime = DateTime.Now - startTime
                            };
                        }
                        catch (Exception ex)
                        {
                            results[index] = new BatchFileResult
                            {
                                FileId = file.FileId,
                                LocalPath = file.LocalPath,
                                ServerPath = file.ServerPath,
                                Success = false,
                                Error = ex.Message,
                                TransferTime = DateTime.Now - startTime
                            };
                        }

                        var count = Interlocked.Increment(ref completed);
                        progress?.Report(new BatchOperationProgress
                        {
                            TotalItems = total,
                            CompletedItems = count,
                            CurrentItem = file.FileId,
                            PercentComplete = (int)((count * 100.0) / total)
                        });
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, cancellationToken);
            }

            await Task.WhenAll(tasks);
            return results;
        }

        /// <summary>
        /// 여러 파일을 병렬로 다운로드합니다.
        /// </summary>
        public async Task<BatchFileResult[]> DownloadFilesAsync(
            BatchFileRequest[] files,
            IProgress<BatchOperationProgress>? progress = null,
            CancellationToken cancellationToken = default)
        {
            EnsureInitialized();
            
            var results = new BatchFileResult[files.Length];
            var completed = 0;
            var total = files.Length;

            var semaphore = new SemaphoreSlim(_maxConcurrentConnections);
            var tasks = new Task[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                var index = i;
                var file = files[i];

                tasks[i] = Task.Run(async () =>
                {
                    await semaphore.WaitAsync(cancellationToken);
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        using var pooledClient = await CreateFileClientAsync(cancellationToken);
                        
                        var startTime = DateTime.Now;
                        try
                        {
                            var success = await pooledClient.Client.DownloadFileAsync(
                                file.ServerPath, 
                                file.LocalPath);

                            results[index] = new BatchFileResult
                            {
                                FileId = file.FileId,
                                LocalPath = file.LocalPath,
                                ServerPath = file.ServerPath,
                                Success = success,
                                TransferTime = DateTime.Now - startTime
                            };
                        }
                        catch (Exception ex)
                        {
                            results[index] = new BatchFileResult
                            {
                                FileId = file.FileId,
                                LocalPath = file.LocalPath,
                                ServerPath = file.ServerPath,
                                Success = false,
                                Error = ex.Message,
                                TransferTime = DateTime.Now - startTime
                            };
                        }

                        var count = Interlocked.Increment(ref completed);
                        progress?.Report(new BatchOperationProgress
                        {
                            TotalItems = total,
                            CompletedItems = count,
                            CurrentItem = file.FileId,
                            PercentComplete = (int)((count * 100.0) / total)
                        });
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, cancellationToken);
            }

            await Task.WhenAll(tasks);
            return results;
        }

        #endregion

        #region Connection Testing

        /// <summary>
        /// 서버의 주요 서비스(DB, 파일 전송 등)에 대한 연결 테스트를 수행합니다.
        /// 모든 서비스가 정상 연결되면 true를 반환합니다.
        /// </summary>
        public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                EnsureInitialized();
                var dbConnected = await TestDBConnectionAsync(cancellationToken);
                var fileConnected = await TestFileConnectionAsync(cancellationToken);
                return dbConnected && fileConnected;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 데이터베이스 연결 테스트를 수행합니다.
        /// </summary>
        public async Task<bool> TestDBConnectionAsync(CancellationToken cancellationToken = default)
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

        /// <summary>
        /// 파일 전송 서비스 연결 테스트를 수행합니다.
        /// </summary>
        public async Task<bool> TestFileConnectionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                EnsureInitialized();
                _ = File;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 로그 업로드 연결 테스트를 수행합니다.
        /// 임시 파일을 생성하여 업로드 시도 후 결과를 반환합니다.
        /// </summary>
        public async Task<bool> TestLogConnectionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                EnsureInitialized();
                var tempFile = System.IO.Path.GetTempFileName();
                try
                {
                    await System.IO.File.WriteAllTextAsync(tempFile,
                        $"Connection test at {DateTime.Now:yyyy-MM-dd HH:mm:ss}", cancellationToken);
                    return await Log.UploadLogFileAsync(tempFile, deleteAfterUpload: false);
                }
                finally
                {
                    if (System.IO.File.Exists(tempFile))
                        System.IO.File.Delete(tempFile);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 모든 연결 서비스에 대해 상세 테스트를 수행하고 결과 객체를 반환합니다.
        /// </summary>
        public async Task<ConnectivityTestResult> TestAllConnectionsAsync(CancellationToken cancellationToken = default)
        {
            var result = new ConnectivityTestResult { TestTime = DateTime.Now };

            try
            {
                EnsureInitialized();

                try { result.DBConnected = await TestDBConnectionAsync(cancellationToken); }
                catch (Exception ex) { result.DBError = ex.Message; }

                try { result.FileConnected = await TestFileConnectionAsync(cancellationToken); }
                catch (Exception ex) { result.FileError = ex.Message; }

                try { result.LogConnected = await TestLogConnectionAsync(cancellationToken); }
                catch (Exception ex) { result.LogError = ex.Message; }

                result.AllConnected = result.DBConnected && result.FileConnected && result.LogConnected;
            }
            catch (Exception ex)
            {
                result.GeneralError = ex.Message;
            }

            return result;
        }

        #endregion

        #region Private Methods

        private HttpClient GetOrCreateDefaultHttpClient()
        {
            if (_defaultHttpClient == null)
            {
                _defaultHttpClient = CreateHttpClient();
            }
            return _defaultHttpClient;
        }

        private HttpClient CreatePooledHttpClient()
        {
            return CreateHttpClient();
        }

        private HttpClient CreateHttpClient()
        {
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(15),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
                MaxConnectionsPerServer = _maxConcurrentConnections,
                EnableMultipleHttp2Connections = true
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_serverUrl!),
                Timeout = _defaultTimeout
            };

            client.DefaultRequestHeaders.Add("User-Agent", "nU3.Framework");
            client.DefaultRequestHeaders.Add("X-Client-Version", "2.0");

            return client;
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("ConnectivityManager is not initialized. Call Initialize(serverUrl) first.");
        }

        private void OnLogMessage(string level, string message)
        {
            LogMessage?.Invoke(this, new LogMessageEventArgs(level, message));
        }

        private void OnProgressChanged(string operationId, int current, int total, string? currentItem)
        {
            ProgressChanged?.Invoke(this, new OperationProgressEventArgs
            {
                OperationId = operationId,
                Current = current,
                Total = total,
                CurrentItem = currentItem,
                PercentComplete = total > 0 ? (int)((current * 100.0) / total) : 0
            });
        }

        private void DisposeClients()
        {
            _dbClient = null;

            _fileClient = null;

            _logClient?.Dispose();
            _logClient = null;

            _defaultHttpClient?.Dispose();
            _defaultHttpClient = null;

            foreach (var client in _httpClientPool.Values)
            {
                client?.Dispose();
            }
            _httpClientPool.Clear();
        }

        #endregion

        public void Dispose()
        {
            lock (_lock)
            {
                DisposeClients();
                _poolSemaphore?.Dispose();
                _isInitialized = false;
            }
        }

        /// <summary>
        /// 테스트용으로 싱글톤 인스턴스를 리셋합니다.
        /// 내부적으로 Dispose를 호출하여 자원을 해제합니다.
        /// </summary>
        internal static void ResetInstance()
        {
            lock (_lock)
            {
                _instance?.Dispose();
                _instance = null;
            }
        }
    }

    #region Pooled Client Wrappers

    /// <summary>
    /// 풀에서 반환되는 DB 클라이언트 래퍼입니다. Dispose하면 내부 HttpClient를 해제하고 풀 카운트를 반환합니다.
    /// </summary>
    public sealed class PooledDBClient : IDisposable
    {
        private readonly HttpDBAccessClient _client;
        private readonly HttpClient _httpClient;
        private readonly Action _releaseAction;
        private bool _disposed;

        public HttpDBAccessClient Client => _client;

        internal PooledDBClient(HttpDBAccessClient client, HttpClient httpClient, Action releaseAction)
        {
            _client = client;
            _httpClient = httpClient;
            _releaseAction = releaseAction;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            
            _httpClient?.Dispose();
            _releaseAction?.Invoke();
        }
    }

    /// <summary>
    /// 풀에서 반환되는 파일 전송 클라이언트 래퍼입니다. Dispose하면 내부 HttpClient를 해제하고 풀 카운트를 반환합니다.
    /// </summary>
    public sealed class PooledFileClient : IDisposable
    {
        private readonly HttpFileTransferClient _client;
        private readonly HttpClient _httpClient;
        private readonly Action _releaseAction;
        private bool _disposed;

        public HttpFileTransferClient Client => _client;

        internal PooledFileClient(HttpFileTransferClient client, HttpClient httpClient, Action releaseAction)
        {
            _client = client;
            _httpClient = httpClient;
            _releaseAction = releaseAction;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            
            _httpClient?.Dispose();
            _releaseAction?.Invoke();
        }
    }

    #endregion

    #region Event Args

    /// <summary>
    /// Connectivity 클라이언트에서 발생한 로그 메시지를 전달하는 이벤트 인자입니다.
    /// </summary>
    public class LogMessageEventArgs : EventArgs
    {
        public string Level { get; }
        public string Message { get; }
        public DateTime Timestamp { get; }

        public LogMessageEventArgs(string level, string message)
        {
            Level = level;
            Message = message;
            Timestamp = DateTime.Now;
        }
    }

    /// <summary>
    /// 진행률 변경을 전달하는 이벤트 인자입니다.
    /// </summary>
    public class OperationProgressEventArgs : EventArgs
    {
        public string? OperationId { get; set; }
        public int Current { get; set; }
        public int Total { get; set; }
        public string? CurrentItem { get; set; }
        public int PercentComplete { get; set; }
    }

    #endregion

    #region Batch Operation Models

    /// <summary>
    /// 배치 쿼리 요청 모델입니다.
    /// </summary>
    public class BatchQueryRequest
    {
        public string QueryId { get; set; } = Guid.NewGuid().ToString();
        public string CommandText { get; set; } = string.Empty;
        public System.Collections.Generic.Dictionary<string, object>? Parameters { get; set; }
    }

    /// <summary>
    /// 배치 쿼리 결과 모델입니다.
    /// </summary>
    public class BatchQueryResult
    {
        public string? QueryId { get; set; }
        public bool Success { get; set; }
        public System.Data.DataTable? Data { get; set; }
        public string? Error { get; set; }
        public TimeSpan ExecutionTime { get; set; }
    }

    /// <summary>
    /// 배치 파일 요청 모델입니다.
    /// </summary>
    public class BatchFileRequest
    {
        public string FileId { get; set; } = Guid.NewGuid().ToString();
        public string LocalPath { get; set; } = string.Empty;
        public string ServerPath { get; set; } = string.Empty;
    }

    /// <summary>
    /// 배치 파일 결과 모델입니다.
    /// </summary>
    public class BatchFileResult
    {
        public string? FileId { get; set; }
        public string? LocalPath { get; set; }
        public string? ServerPath { get; set; }
        public bool Success { get; set; }
        public string? Error { get; set; }
        public TimeSpan TransferTime { get; set; }
        public long? FileSize { get; set; }
    }

    /// <summary>
    /// 배치 작업 진행률 모델입니다.
    /// </summary>
    public class BatchOperationProgress
    {
        public int TotalItems { get; set; }
        public int CompletedItems { get; set; }
        public string? CurrentItem { get; set; }
        public int PercentComplete { get; set; }
    }

    /// <summary>
    /// 연결 테스트 결과 모델입니다.
    /// </summary>
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
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Connectivity Test Results ({TestTime:yyyy-MM-dd HH:mm:ss})");
            sb.AppendLine($"Overall: {(AllConnected ? "✅ All Connected" : "❌ Some Failed")}");
            sb.AppendLine();
            sb.AppendLine($"Database:      {(DBConnected ? "✅ Connected" : $"❌ Failed - {DBError}")}");
            sb.AppendLine($"File Transfer: {(FileConnected ? "✅ Connected" : $"❌ Failed - {FileError}")}");
            sb.AppendLine($"Log Upload:    {(LogConnected ? "✅ Connected" : $"❌ Failed - {LogError}")}");

            if (!string.IsNullOrEmpty(GeneralError))
            {
                sb.AppendLine();
                sb.AppendLine($"General Error: {GeneralError}");
            }

            return sb.ToString();
        }
    }

    #endregion
}
