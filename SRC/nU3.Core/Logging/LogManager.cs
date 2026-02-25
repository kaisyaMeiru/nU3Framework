using System;
using System.Threading.Tasks;
using nU3.Models;
using nU3.Core.Interfaces;

namespace nU3.Core.Logging
{
    /// <summary>
    /// 애플리케이션 전역에서 사용하는 로그 매니저(singleton)입니다.
    /// - 파일 로거(FileLogger), 오딧 로거(AuditLogger), 로그 업로드 서비스(LogUploadService)를 관리합니다.
    /// </summary>
    public sealed class LogManager
    {
        private static readonly Lazy<LogManager> _instance = new Lazy<LogManager>(() => new LogManager());
        public static LogManager Instance => _instance.Value;

        private FileLogger? _fileLogger;
        private AuditLogger? _auditLogger;
        private LogUploadService? _uploadService;
        private bool _initialized;

        private LogManager() { }

        /// <summary>
        /// 로그 매니저 초기화.
        /// </summary>
        public void Initialize(
            string? logDirectory = null,
            string? auditDirectory = null,
            IFileTransferService? fileTransferService = null,
            bool enableAutoUpload = false)
        {
            if (_initialized) return;

            _fileLogger = new FileLogger(logDirectory);
            _auditLogger = new AuditLogger(auditDirectory);

            if (fileTransferService != null)
            {
                _uploadService = new LogUploadService(fileTransferService, _fileLogger);
                if (enableAutoUpload) _uploadService.EnableAutoUpload(true);
            }

            // 초기 로깅
            // 기본 시작 로그 기록
            _fileLogger.Information("=".PadRight(80, '='), "System");
            _fileLogger.Information($"nU3 Framework Started - Version {GetVersion()}", "System");
            _fileLogger.Information($"Machine: {Environment.MachineName}", "System");
            _fileLogger.Information($"User: {Environment.UserName}", "System");
            _fileLogger.Information($"OS: {Environment.OSVersion}", "System");
            _fileLogger.Information("=".PadRight(80, '='), "System");

            _fileLogger.CleanupOldLogs(30);
            _auditLogger.CleanupOldAudits(90);

            _initialized = true;
        }

        private string GetVersion() {
            try { return System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "Unknown"; }
            catch { return "Unknown"; }
        }

        /// <summary>현재 일반 로거 (초기화 전 null)</summary>
        public ILogger? Logger => _fileLogger;

        /// <summary>현재 오딧 로거 (초기화 전 null)</summary>
        public IAuditLogger? AuditLogger => _auditLogger;

        public async Task FlushAllAsync() {
            if (!_initialized) return;
            if (_fileLogger != null) await _fileLogger.FlushAsync();
            if (_auditLogger != null) await _auditLogger.FlushAsync();
        }

        public void Shutdown() {
            if (!_initialized || _fileLogger == null) return;
            try {
                _fileLogger?.Information("=".PadRight(80, '='), "System");
                _fileLogger?.Information("nU3 Framework Shutting Down", "System");
                _fileLogger?.Information("=".PadRight(80, '='), "System");
                FlushAllAsync().Wait(TimeSpan.FromSeconds(3));
                _fileLogger.Dispose();
                _auditLogger?.Dispose();
            } catch { }
        }

        // 전역 유틸리티 메서드: 널 안전성 확보
        public static void Trace(string message, string? category = null) => Instance.Logger?.Trace(message, category);
        public static void Debug(string message, string? category = null) => Instance.Logger?.Debug(message, category);
        public static void Info(string message, string? category = null) => Instance.Logger?.Information(message, category);
        public static void Warning(string message, string? category = null) => Instance.Logger?.Warning(message, category);
        public static void Error(string message, string? category = null, Exception? exception = null) => Instance.Logger?.Error(message, category, exception);
        public static void Critical(string message, string? category = null, Exception? exception = null) => Instance.Logger?.Critical(message, category, exception);

        public static void LogAudit(AuditLogDto audit) => Instance.AuditLogger?.LogAudit(audit);
        public static void LogAction(string action, string module, string screen, string? additionalInfo = null) => Instance.AuditLogger?.LogAction(action, module, screen, additionalInfo);
        
        public static void LogCreate(string type, string id, string val, string? mod = null, string? scr = null) => Instance.AuditLogger?.LogCreate(type, id, val, mod, scr);
        public static void LogUpdate(string type, string id, string old, string val, string? mod = null, string? scr = null) => Instance.AuditLogger?.LogUpdate(type, id, old, val, mod, scr);
        public static void LogDelete(string type, string id, string old, string? mod = null, string? scr = null) => Instance.AuditLogger?.LogDelete(type, id, old, mod, scr);
    }
}
