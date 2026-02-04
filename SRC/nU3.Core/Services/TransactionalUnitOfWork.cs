using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace nU3.Core.Services
{
    /// <summary>
    /// DB 및 파일 전송 작업을 하나의 단위(Unit of Work)로 묶어
    /// 순차적 또는 병렬로 실행하고 실패 시 롤백하는 유틸리티입니다.
    ///
    /// 기능 및 설계:
    /// - 각 작업은 ITransactionalOperation 인터페이스를 구현해야 합니다.
    /// - 작업 실패 시 이미 완료된 작업들을 역순으로 RollbackAsync를 호출하여 보상(Compensating)합니다.
    /// - 커밋이 필요한 작업은 ICommittableOperation을 구현하여 CommitAsync에서 최종 확정 작업을 수행합니다.
    /// - ExecuteAsync는 순차 실행, ExecuteParallelAsync는 가능한 작업을 병렬 실행하도록 시도합니다.
    /// - Saga 패턴과 유사한 보상 기반 트랜잭션 모델을 제공합니다.
    /// </summary>
    public class TransactionalUnitOfWork : IDisposable
    {
        private readonly ConnectivityManager _connectivityManager;
        private readonly List<ITransactionalOperation> _operations = new();
        private readonly List<ITransactionalOperation> _completedOperations = new();
        private readonly CancellationTokenSource _cts = new();
        
        private bool _isCommitted;
        private bool _isRolledBack;
        private bool _disposed;

        /// <summary>
        /// 현재 UnitOfWork 상태
        /// </summary>
        public UnitOfWorkState State { get; private set; } = UnitOfWorkState.Pending;

        /// <summary>
        /// 실패 시의 오류 메시지
        /// </summary>
        public string? ErrorMessage { get; private set; }

        /// <summary>
        /// 개별 작업 결과 목록
        /// </summary>
        public IReadOnlyList<OperationResult> Results => _results.AsReadOnly();
        private readonly List<OperationResult> _results = new();

        /// <summary>
        /// 전체 진행률 이벤트
        /// </summary>
        public event EventHandler<UnitOfWorkProgressEventArgs>? ProgressChanged;

        public TransactionalUnitOfWork(ConnectivityManager? connectivityManager = null)
        {
            _connectivityManager = connectivityManager ?? ConnectivityManager.Instance;
        }

        #region Add Operations

        /// <summary>
        /// DB 쿼리(SELECT) 작업 추가. 결과는 OnSuccess 콜백으로 전달될 수 있습니다.
        /// </summary>
        public TransactionalUnitOfWork AddQuery(
            string operationId,
            string commandText,
            Dictionary<string, object>? parameters = null,
            Action<System.Data.DataTable>? onSuccess = null)
        {
            _operations.Add(new DbQueryOperation
            {
                OperationId = operationId,
                CommandText = commandText,
                Parameters = parameters,
                OnSuccess = onSuccess,
                ConnectivityManager = _connectivityManager
            });
            return this;
        }

        /// <summary>
        /// DB 비조회(INSERT/UPDATE/DELETE) 작업 추가. 필요 시 롤백 SQL을 제공합니다.
        /// </summary>
        public TransactionalUnitOfWork AddNonQuery(
            string operationId,
            string commandText,
            Dictionary<string, object>? parameters = null,
            string? rollbackCommandText = null,
            Dictionary<string, object>? rollbackParameters = null)
        {
            _operations.Add(new DbNonQueryOperation
            {
                OperationId = operationId,
                CommandText = commandText,
                Parameters = parameters,
                RollbackCommandText = rollbackCommandText,
                RollbackParameters = rollbackParameters,
                ConnectivityManager = _connectivityManager
            });
            return this;
        }

        /// <summary>
        /// 파일 업로드 작업 추가. 성공 시 서버 파일 삭제(롤백) 기능을 가집니다.
        /// </summary>
        public TransactionalUnitOfWork AddFileUpload(
            string operationId,
            string localPath,
            string serverPath,
            bool deleteLocalOnSuccess = false)
        {
            _operations.Add(new FileUploadOperation
            {
                OperationId = operationId,
                LocalPath = localPath,
                ServerPath = serverPath,
                DeleteLocalOnSuccess = deleteLocalOnSuccess,
                ConnectivityManager = _connectivityManager
            });
            return this;
        }

        /// <summary>
        /// 파일 다운로드 작업 추가. 롤백 시 다운로드된 로컬 파일을 삭제합니다.
        /// </summary>
        public TransactionalUnitOfWork AddFileDownload(
            string operationId,
            string serverPath,
            string localPath)
        {
            _operations.Add(new FileDownloadOperation
            {
                OperationId = operationId,
                ServerPath = serverPath,
                LocalPath = localPath,
                ConnectivityManager = _connectivityManager
            });
            return this;
        }

        /// <summary>
        /// 커스텀 ITransactionalOperation을 직접 추가합니다.
        /// </summary>
        public TransactionalUnitOfWork AddCustomOperation(ITransactionalOperation operation)
        {
            _operations.Add(operation);
            return this;
        }

        /// <summary>
        /// 조건(predicate)이 true일 때만 addOperations 콜백을 통해 작업을 추가합니다.
        /// 유연한 워크플로우 구성이 가능합니다.
        /// </summary>
        public TransactionalUnitOfWork AddConditional(
            Func<bool> predicate,
            Action<TransactionalUnitOfWork> addOperations)
        {
            if (predicate())
            {
                addOperations(this);
            }
            return this;
        }

        #endregion

        #region Execute

        /// <summary>
        /// 모든 작업을 순차적으로 실행합니다. 실패 시 역순으로 롤백합니다.
        /// </summary>
        public async Task<bool> ExecuteAsync(
            IProgress<UnitOfWorkProgressEventArgs>? progress = null,
            CancellationToken cancellationToken = default)
        {
            if (_isCommitted || _isRolledBack)
                throw new InvalidOperationException("UnitOfWork has already been completed.");

            if (_operations.Count == 0)
            {
                State = UnitOfWorkState.Completed;
                return true;
            }

            State = UnitOfWorkState.Executing;
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cts.Token);

            try
            {
                for (int i = 0; i < _operations.Count; i++)
                {
                    linkedCts.Token.ThrowIfCancellationRequested();

                    var operation = _operations[i];
                    var progressArgs = new UnitOfWorkProgressEventArgs
                    {
                        TotalOperations = _operations.Count,
                        CurrentOperationIndex = i,
                        CurrentOperationId = operation.OperationId,
                        Phase = "Executing",
                        PercentComplete = (int)((i * 100.0) / _operations.Count)
                    };

                    progress?.Report(progressArgs);
                    ProgressChanged?.Invoke(this, progressArgs);

                    var result = await operation.ExecuteAsync(linkedCts.Token);
                    _results.Add(result);

                    if (result.Success)
                    {
                        _completedOperations.Add(operation);
                    }
                    else
                    {
                        // 작업 실패 - 모든 완료된 작업 롤백
                        ErrorMessage = $"Operation '{operation.OperationId}' failed: {result.Error}";
                        await RollbackAsync(progress, linkedCts.Token);
                        return false;
                    }
                }

                // 모든 작업 성공 - 커밋
                await CommitAsync(progress, linkedCts.Token);
                return true;
            }
            catch (OperationCanceledException)
            {
                ErrorMessage = "Operation was cancelled.";
                await RollbackAsync(progress, CancellationToken.None);
                throw;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Unexpected error: {ex.Message}";
                await RollbackAsync(progress, CancellationToken.None);
                return false;
            }
        }

        /// <summary>
        /// 의존성 그래프를 고려하지 않은 단순 병렬 실행 구현(예시).
        /// 실제 병렬 의존성 처리를 위해서는 그래프 기반 스케줄링이 필요합니다.
        /// </summary>
        public async Task<bool> ExecuteParallelAsync(
            IProgress<UnitOfWorkProgressEventArgs>? progress = null,
            CancellationToken cancellationToken = default)
        {
            if (_isCommitted || _isRolledBack)
                throw new InvalidOperationException("UnitOfWork has already been completed.");

            State = UnitOfWorkState.Executing;
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cts.Token);
            var completed = 0;
            var total = _operations.Count;
            var failed = false;
            var failLock = new object();

            try
            {
                var tasks = _operations.Select(async (operation, index) =>
                {
                    try
                    {
                        linkedCts.Token.ThrowIfCancellationRequested();
                        
                        var result = await operation.ExecuteAsync(linkedCts.Token);
                        _results.Add(result);

                        if (result.Success)
                        {
                            lock (_completedOperations)
                            {
                                _completedOperations.Add(operation);
                            }
                        }
                        else
                        {
                            lock (failLock)
                            {
                                if (!failed)
                                {
                                    failed = true;
                                    ErrorMessage = $"Operation '{operation.OperationId}' failed: {result.Error}";
                                    _cts.Cancel(); // 다른 작업 취소
                                }
                            }
                        }

                        var count = Interlocked.Increment(ref completed);
                        var progressArgs = new UnitOfWorkProgressEventArgs
                        {
                            TotalOperations = total,
                            CurrentOperationIndex = count,
                            CurrentOperationId = operation.OperationId,
                            Phase = "Executing",
                            PercentComplete = (int)((count * 100.0) / total)
                        };
                        progress?.Report(progressArgs);
                        ProgressChanged?.Invoke(this, progressArgs);

                        return result;
                    }
                    catch (OperationCanceledException)
                    {
                        return new OperationResult
                        {
                            OperationId = operation.OperationId,
                            Success = false,
                            Error = "Cancelled"
                        };
                    }
                }).ToArray();

                await Task.WhenAll(tasks);

                if (failed)
                {
                    await RollbackAsync(progress, CancellationToken.None);
                    return false;
                }

                await CommitAsync(progress, linkedCts.Token);
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Unexpected error: {ex.Message}";
                await RollbackAsync(progress, CancellationToken.None);
                return false;
            }
        }

        #endregion

        #region Commit / Rollback

        private async Task CommitAsync(
            IProgress<UnitOfWorkProgressEventArgs>? progress,
            CancellationToken cancellationToken)
        {
            if (_isCommitted) return;

            var progressArgs = new UnitOfWorkProgressEventArgs
            {
                TotalOperations = _operations.Count,
                CurrentOperationIndex = _operations.Count,
                Phase = "Committing",
                PercentComplete = 100
            };
            progress?.Report(progressArgs);
            ProgressChanged?.Invoke(this, progressArgs);

            // 커밋 가능한 작업들에 대해 CommitAsync 호출
            foreach (var operation in _completedOperations)
            {
                if (operation is ICommittableOperation committable)
                {
                    await committable.CommitAsync(cancellationToken);
                }
            }

            _isCommitted = true;
            State = UnitOfWorkState.Completed;
        }

        private async Task RollbackAsync(
            IProgress<UnitOfWorkProgressEventArgs>? progress,
            CancellationToken cancellationToken)
        {
            if (_isRolledBack) return;

            State = UnitOfWorkState.RollingBack;

            // 역순으로 롤백
            for (int i = _completedOperations.Count - 1; i >= 0; i--)
            {
                var operation = _completedOperations[i];
                
                var progressArgs = new UnitOfWorkProgressEventArgs
                {
                    TotalOperations = _completedOperations.Count,
                    CurrentOperationIndex = _completedOperations.Count - i,
                    CurrentOperationId = operation.OperationId,
                    Phase = "Rolling back",
                    PercentComplete = (int)(((_completedOperations.Count - i) * 100.0) / _completedOperations.Count)
                };
                progress?.Report(progressArgs);
                ProgressChanged?.Invoke(this, progressArgs);

                try
                {
                    await operation.RollbackAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    // 롤백 실패 시 로그 기록 후 다음으로 진행
                    System.Diagnostics.Debug.WriteLine($"Rollback failed for {operation.OperationId}: {ex.Message}");
                }
            }

            _isRolledBack = true;
            State = UnitOfWorkState.RolledBack;
        }

        /// <summary>
        /// 현재 실행 중인 작업을 취소합니다.
        /// </summary>
        public void Cancel()
        {
            _cts.Cancel();
        }

        #endregion

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _cts.Dispose();
            
            foreach (var operation in _operations)
            {
                if (operation is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }

    #region Interfaces

    /// <summary>
    /// 트랜잭셔널 작업 인터페이스
    /// </summary>
    public interface ITransactionalOperation
    {
        string OperationId { get; }
        Task<OperationResult> ExecuteAsync(CancellationToken cancellationToken);
        Task RollbackAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// 명시적 커밋을 지원하는 작업 인터페이스
    /// </summary>
    public interface ICommittableOperation
    {
        Task CommitAsync(CancellationToken cancellationToken);
    }

    #endregion

    #region Operation Implementations

    /// <summary>
    /// DB 조회 작업 구현
    /// </summary>
    internal class DbQueryOperation : ITransactionalOperation
    {
        public string OperationId { get; set; } = string.Empty;
        public string CommandText { get; set; } = string.Empty;
        public Dictionary<string, object>? Parameters { get; set; }
        public Action<System.Data.DataTable>? OnSuccess { get; set; }
        public ConnectivityManager ConnectivityManager { get; set; } = null!;

        public System.Data.DataTable? Result { get; private set; }

        public async Task<OperationResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                Result = await ConnectivityManager.DB.ExecuteDataTableAsync(CommandText, Parameters);
                OnSuccess?.Invoke(Result);
                
                return new OperationResult
                {
                    OperationId = OperationId,
                    Success = true,
                    Data = Result
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    OperationId = OperationId,
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        public Task RollbackAsync(CancellationToken cancellationToken)
        {
            // 쿼리 작업은 롤백이 필요 없습니다.
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// DB 비조회 작업(INSERT/UPDATE/DELETE) 구현 - rollback SQL 지원
    /// </summary>
    internal class DbNonQueryOperation : ITransactionalOperation
    {
        public string OperationId { get; set; } = string.Empty;
        public string CommandText { get; set; } = string.Empty;
        public Dictionary<string, object>? Parameters { get; set; }
        public string? RollbackCommandText { get; set; }
        public Dictionary<string, object>? RollbackParameters { get; set; }
        public ConnectivityManager ConnectivityManager { get; set; } = null!;

        public async Task<OperationResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var success = await ConnectivityManager.DB.ExecuteNonQueryAsync(CommandText, Parameters);
                
                return new OperationResult
                {
                    OperationId = OperationId,
                    Success = success,
                    Error = success ? null : "ExecuteNonQuery returned false"
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    OperationId = OperationId,
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(RollbackCommandText))
            {
                try
                {
                    await ConnectivityManager.DB.ExecuteNonQueryAsync(RollbackCommandText, RollbackParameters);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Rollback failed: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// 파일 업로드 작업 구현 - 롤백 시 서버에서 업로드 파일 삭제 시도
    /// </summary>
    internal class FileUploadOperation : ITransactionalOperation
    {
        public string OperationId { get; set; } = string.Empty;
        public string LocalPath { get; set; } = string.Empty;
        public string ServerPath { get; set; } = string.Empty;
        public bool DeleteLocalOnSuccess { get; set; }
        public ConnectivityManager ConnectivityManager { get; set; } = null!;

        private bool _uploaded;

        public async Task<OperationResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!System.IO.File.Exists(LocalPath))
                {
                    return new OperationResult
                    {
                        OperationId = OperationId,
                        Success = false,
                        Error = $"Local file not found: {LocalPath}"
                    };
                }

                var success = await ConnectivityManager.File.UploadFileAsync(LocalPath, ServerPath);
                _uploaded = success;

                if (success && DeleteLocalOnSuccess)
                {
                    System.IO.File.Delete(LocalPath);
                }

                return new OperationResult
                {
                    OperationId = OperationId,
                    Success = success,
                    Error = success ? null : "Upload failed"
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    OperationId = OperationId,
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken)
        {
            if (_uploaded)
            {
                try
                {
                    await ConnectivityManager.File.DeleteFileAsync(ServerPath);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to delete uploaded file: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// 파일 다운로드 작업 구현 - 롤백 시 로컬 파일 삭제 시도
    /// </summary>
    internal class FileDownloadOperation : ITransactionalOperation
    {
        public string OperationId { get; set; } = string.Empty;
        public string ServerPath { get; set; } = string.Empty;
        public string LocalPath { get; set; } = string.Empty;
        public ConnectivityManager ConnectivityManager { get; set; } = null!;

        private bool _downloaded;

        public async Task<OperationResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var success = await ConnectivityManager.File.DownloadFileAsync(ServerPath, LocalPath);
                _downloaded = success;

                return new OperationResult
                {
                    OperationId = OperationId,
                    Success = success,
                    Error = success ? null : "Download failed"
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    OperationId = OperationId,
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken)
        {
            if (_downloaded && System.IO.File.Exists(LocalPath))
            {
                try
                {
                    System.IO.File.Delete(LocalPath);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to delete downloaded file: {ex.Message}");
                }
            }
            await Task.CompletedTask;
        }
    }

    #endregion

    #region Models

    /// <summary>
    /// 개별 작업의 실행 결과 모델
    /// </summary>
    public class OperationResult
    {
        public string? OperationId { get; set; }
        public bool Success { get; set; }
        public string? Error { get; set; }
        public object? Data { get; set; }
        public TimeSpan ExecutionTime { get; set; }
    }

    /// <summary>
    /// Unit of Work의 상태를 나타내는 열거형
    /// </summary>
    public enum UnitOfWorkState
    {
        Pending,
        Executing,
        Completed,
        RollingBack,
        RolledBack,
        Failed
    }

    /// <summary>
    /// Unit of Work 진행률 이벤트 인자
    /// </summary>
    public class UnitOfWorkProgressEventArgs : EventArgs
    {
        public int TotalOperations { get; set; }
        public int CurrentOperationIndex { get; set; }
        public string? CurrentOperationId { get; set; }
        public string? Phase { get; set; }
        public int PercentComplete { get; set; }
    }

    #endregion
}
