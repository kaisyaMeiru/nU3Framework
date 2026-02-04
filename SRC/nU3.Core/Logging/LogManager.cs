using System;
using System.Threading.Tasks;
using nU3.Models;

namespace nU3.Core.Logging
{
    /// <summary>
    /// 애플리케이션 전역에서 사용하는 로그 매니저(singleton)입니다.
    /// - 파일 로거(FileLogger), 오딧 로거(AuditLogger), 로그 업로드 서비스(LogUploadService)를 관리합니다.
    /// - 초기화(Initialize) 시 로그 디렉터리를 설정하고, 시작 메시지를 기록합니다.
    /// - 전역 유틸리티 메서드(Trace/Debug/Info/Error/Critical 등)를 제공하여 편리하게 로그를 남길 수 있습니다.
    /// </summary>
    public sealed class LogManager
    {
        private static readonly Lazy<LogManager> _instance = new Lazy<LogManager>(() => new LogManager());
        public static LogManager Instance => _instance.Value;

        private FileLogger _fileLogger;
        private AuditLogger _auditLogger;
        private LogUploadService _uploadService;
        private bool _initialized;

        private LogManager()
        {
        }

        /// <summary>
        /// 로그 매니저 초기화.
        /// - logDirectory: 일반 로그 파일 저장 경로
        /// - auditDirectory: 오딧 로그 저장 경로
        /// - fileTransferService: 로그 업로드에 사용할 원격 파일 전송 서비스
        /// - enableAutoUpload: 초기화 시 자동 업로드 설정
        /// </summary>
        public void Initialize(
            string logDirectory = null,
            string auditDirectory = null,
            Connectivity.IFileTransferService fileTransferService = null,
            bool enableAutoUpload = false)
        {
            if (_initialized)
                return;

            _fileLogger = new FileLogger(logDirectory);
            _auditLogger = new AuditLogger(auditDirectory);

            if (fileTransferService != null)
            {
                _uploadService = new LogUploadService(fileTransferService, _fileLogger);
                if (enableAutoUpload)
                {
                    _uploadService.EnableAutoUpload(true);
                }
            }

            // 기본 시작 로그 기록
            _fileLogger.Information("=".PadRight(80, '='), "System");
            _fileLogger.Information($"nU3 Framework Started - Version {GetVersion()}", "System");
            _fileLogger.Information($"Machine: {Environment.MachineName}", "System");
            _fileLogger.Information($"User: {Environment.UserName}", "System");
            _fileLogger.Information($"OS: {Environment.OSVersion}", "System");
            _fileLogger.Information("=".PadRight(80, '='), "System");

            // 오래된 로그 정리
            _fileLogger.CleanupOldLogs(30);
            _auditLogger.CleanupOldAudits(90);

            _initialized = true;
        }

        private string GetVersion()
        {
            try
            {
                return System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// 현재 사용 가능한 일반 로거를 반환합니다. Initialize 호출 전에는 사용 불가합니다.
        /// </summary>
        public ILogger Logger => _fileLogger ?? throw new InvalidOperationException("LogManager not initialized. Call Initialize() first.");

        /// <summary>
        /// 현재 사용 가능한 오딧 로거를 반환합니다. Initialize 호출 전에는 사용 불가합니다.
        /// </summary>
        public IAuditLogger AuditLogger => _auditLogger ?? throw new InvalidOperationException("LogManager not initialized. Call Initialize() first.");

        /// <summary>
        /// 로그 업로드 서비스(있을 경우)를 반환합니다.
        /// </summary>
        public ILogUploadService UploadService => _uploadService;

        /// <summary>
        /// 모든 로그(일반/오딧)를 강제로 Flush 합니다.
        /// </summary>
        public async Task FlushAllAsync()
        {
            if (!_initialized)
                return;

            await _fileLogger?.FlushAsync();
            await _auditLogger?.FlushAsync();
        }

        /// <summary>
        /// 예외 발생 시 호출: 치명적 로그 기록 및 즉시 업로드 시도
        /// (비동기) 내부적으로 로그를 남기고 Flush, 업로드를 시도합니다.
        /// </summary>
        public async Task OnErrorAsync(Exception exception, string context = null)
        {
            if (!_initialized)
                return;

            try
            {
                // 치명적 로그 기록
                _fileLogger?.Critical($"Critical Error: {exception.Message}", context ?? "Error", exception);

                // 즉시 flush
                await FlushAllAsync();

                // 업로드 서비스가 있으면 현재 로그 즉시 업로드
                if (_uploadService != null)
                {
                    await _uploadService.UploadCurrentLogImmediatelyAsync();
                }
            }
            catch
            {
                // 로깅 자체에서 예외가 발생하면 무시
            }
        }

        /// <summary>
        /// 애플리케이션 종료 시 호출: 종료 로그 기록 및 리소스 정리
        /// </summary>
        public void Shutdown()
        {
            if (!_initialized)
                return;

            try
            {
                _fileLogger?.Information("=".PadRight(80, '='), "System");
                _fileLogger?.Information("nU3 Framework Shutting Down", "System");
                _fileLogger?.Information("=".PadRight(80, '='), "System");

                FlushAllAsync().Wait(TimeSpan.FromSeconds(5));

                _fileLogger?.Dispose();
                _auditLogger?.Dispose();
            }
            catch { }
        }

        // 전역 유틸리티 메서드: 편의성 제공
        public static void Trace(string message, string category = null) 
            => Instance.Logger?.Trace(message, category);

        public static void Debug(string message, string category = null) 
            => Instance.Logger?.Debug(message, category);

        public static void Info(string message, string category = null) 
            => Instance.Logger?.Information(message, category);

        public static void Warning(string message, string category = null) 
            => Instance.Logger?.Warning(message, category);

        public static void Error(string message, string category = null, Exception exception = null) 
            => Instance.Logger?.Error(message, category, exception);

        public static void Critical(string message, string category = null, Exception exception = null) 
            => Instance.Logger?.Critical(message, category, exception);

        // Audit 관련 편의 메서드
        public static void LogAudit(AuditLogDto audit) 
            => Instance.AuditLogger?.LogAudit(audit);

        public static void LogCreate(string entityType, string entityId, string newValue, string module = null, string screen = null)
            => Instance.AuditLogger?.LogCreate(entityType, entityId, newValue, module, screen);

        public static void LogUpdate(string entityType, string entityId, string oldValue, string newValue, string module = null, string screen = null)
            => Instance.AuditLogger?.LogUpdate(entityType, entityId, oldValue, newValue, module, screen);

        public static void LogDelete(string entityType, string entityId, string oldValue, string module = null, string screen = null)
            => Instance.AuditLogger?.LogDelete(entityType, entityId, oldValue, module, screen);

        public static void LogAction(string action, string module, string screen, string additionalInfo = null)
            => Instance.AuditLogger?.LogAction(action, module, screen, additionalInfo);
    }
}
