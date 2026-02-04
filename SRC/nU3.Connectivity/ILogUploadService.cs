using System;
using System.Threading.Tasks;

namespace nU3.Connectivity
{
    /// <summary>
    /// 로그 업로드 서비스 인터페이스
    /// 애플리케이션 로그와 감사 로그를 서버로 업로드하는 기능을 정의합니다.
    /// </summary>
    public interface ILogUploadService
    {
        /// <summary>
        /// 로컬 로그 파일을 서버로 업로드합니다.
        /// </summary>
        Task<bool> UploadLogFileAsync(string localFilePath, bool deleteAfterUpload = false);

        /// <summary>
        /// 로컬 감사 로그 파일을 서버로 업로드합니다.
        /// </summary>
        Task<bool> UploadAuditLogAsync(string localFilePath, bool deleteAfterUpload = false);

        /// <summary>
        /// 대기 중인 모든 로그 파일을 업로드합니다.
        /// </summary>
        Task<bool> UploadAllPendingLogsAsync();

        /// <summary>
        /// 현재(오늘) 로그 파일을 즉시 업로드합니다.
        /// </summary>
        Task<bool> UploadCurrentLogImmediatelyAsync();

        /// <summary>
        /// 자동 업로드 기능을 활성화 또는 비활성화합니다.
        /// </summary>
        void EnableAutoUpload(bool enable);
    }
}
