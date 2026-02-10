using System;
using System.Threading.Tasks;
using nU3.Models;

namespace nU3.Core.Logging
{
    /// <summary>
    /// 기본 로거 인터페이스입니다.
    /// 애플리케이션 전반에서 로그를 남기는 기능을 정의합니다.
    /// 구현체는 레벨별 로그 메서드와 일반 로그, 비동기 Flush 기능을 제공해야 합니다.
    /// </summary>
    public interface ILogger
    {
        event EventHandler<string> MessageLogged;
        void Trace(string message, string category = null, Exception exception = null);
        void Debug(string message, string category = null, Exception exception = null);
        void Information(string message, string category = null, Exception exception = null);
        void Warning(string message, string category = null, Exception exception = null);
        void Error(string message, string category = null, Exception exception = null);
        void Critical(string message, string category = null, Exception exception = null);
        void Log(LogLevel level, string message, string category = null, Exception exception = null);
        Task FlushAsync();
    }

    /// <summary>
    /// 감사(Audit) 로그 인터페이스입니다.
    /// 생성/조회/수정/삭제 등 감사 목적의 로그를 기록하는 메서드를 제공합니다.
    /// </summary>
    public interface IAuditLogger
    {
        void LogAudit(AuditLogDto audit);
        void LogCreate(string entityType, string entityId, string newValue, string module = null, string screen = null);
        void LogUpdate(string entityType, string entityId, string oldValue, string newValue, string module = null, string screen = null);
        void LogDelete(string entityType, string entityId, string oldValue, string module = null, string screen = null);
        void LogRead(string entityType, string entityId, string module = null, string screen = null);
        void LogAction(string action, string module, string screen, string additionalInfo = null);
        Task FlushAsync();
    }

    /// <summary>
    /// 로그 업로드 서비스 인터페이스입니다.
    /// 로컬에 저장된 로그 파일을 서버로 업로드하거나 자동 업로드 기능을 제어합니다.
    /// </summary>
    public interface ILogUploadService
    {
        Task<bool> UploadLogFileAsync(string localFilePath, bool deleteAfterUpload = false);
        Task<bool> UploadAllPendingLogsAsync();
        void EnableAutoUpload(bool enable);
    }
}
