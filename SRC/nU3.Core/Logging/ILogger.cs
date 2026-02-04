using System;
using System.Threading.Tasks;
using nU3.Models;

namespace nU3.Core.Logging
{
    /// <summary>
    /// 일반 로그 기록기 인터페이스
    /// 애플리케이션 전반에서 사용되는 로그 수준별 메서드를 정의합니다.
    /// 구현체는 로그를 파일, 콘솔, 원격 서버 등 원하는 대상으로 기록할 수 있습니다.
    /// </summary>
    public interface ILogger
    {
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
    /// 오딧(Audit) 로그 전용 인터페이스
    /// 데이터 변경/조회 등 감사 목적의 이벤트를 기록하기 위한 메서드들을 정의합니다.
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
    /// 로그 업로드 서비스 인터페이스
    /// 로컬 로그 파일을 서버로 업로드하는 기능을 제공합니다.
    /// </summary>
    public interface ILogUploadService
    {
        Task<bool> UploadLogFileAsync(string localFilePath, bool deleteAfterUpload = false);
        Task<bool> UploadAllPendingLogsAsync();
        void EnableAutoUpload(bool enable);
    }
}
