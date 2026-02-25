using System;
using System.Threading.Tasks;

namespace nU3.Core.Interfaces
{
    /// <summary>
    /// 애플리케이션 크래시 리포팅 기능을 제공하는 인터페이스입니다.
    /// </summary>
    public interface ICrashReporter
    {
        /// <summary>
        /// 발생한 예외를 분석하여 리포트를 생성하고 발송합니다.
        /// </summary>
        Task<bool> ReportCrashAsync(Exception exception, string? additionalInfo = null);
        
        /// <summary>
        /// 오래된 크래시 로그 파일을 정리합니다.
        /// </summary>
        void CleanupOldLogs(int daysToKeep = 30);
    }
}
