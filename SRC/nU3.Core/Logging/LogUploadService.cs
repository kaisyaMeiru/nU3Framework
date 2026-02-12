using System;
using nU3.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace nU3.Core.Logging
{
    /// <summary>
    /// 로컬에 저장된 로그 파일들을 서버로 업로드하는 서비스 구현체입니다.
    /// 파일 전송을 위한 IFileTransferService를 주입받아 동작합니다.
    /// - 특정 파일 업로드
    /// - 보류중인 모든 로그 업로드
    /// - 자동 업로드 스케줄 기능(매일 2시)
    /// </summary>
    public class LogUploadService : ILogUploadService
    {
        private readonly IFileTransferService _fileTransferService;
        private readonly FileLogger _logger;
        private readonly string _logDirectory;
        private readonly string _serverLogPath;
        private bool _autoUploadEnabled;
        private System.Threading.Timer _autoUploadTimer;

        public LogUploadService(
            IFileTransferService fileTransferService,
            FileLogger logger,
            string serverLogPath = "Logs")
        {
            _fileTransferService = fileTransferService;
            _logger = logger;
            _serverLogPath = serverLogPath;
            
            _logDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "nU3.Framework",
                "LOG"
            );
        }

        /// <summary>
        /// 지정한 로컬 로그 파일을 서버로 업로드합니다.
        /// 성공 시(및 deleteAfterUpload=true) 로컬 파일을 삭제할 수 있습니다.
        /// </summary>
        public async Task<bool> UploadLogFileAsync(string localFilePath, bool deleteAfterUpload = false)
        {
            try
            {
                if (!File.Exists(localFilePath))
                {
                    _logger?.Warning($"Log file not found: {localFilePath}", "LogUpload");
                    return false;
                }

                var fileName = Path.GetFileName(localFilePath);
                var serverPath = $"{_serverLogPath}/{fileName}";

                _logger?.Information($"Uploading log file: {fileName}", "LogUpload");

                var success = await _fileTransferService.UploadFileAsync(localFilePath, serverPath);

                if (success)
                {
                    _logger?.Information($"Successfully uploaded log file: {fileName}", "LogUpload");

                    if (deleteAfterUpload)
                    {
                        try
                        {
                            File.Delete(localFilePath);
                            _logger?.Information($"Deleted local log file: {fileName}", "LogUpload");
                        }
                        catch (Exception ex)
                        {
                            _logger?.Warning($"Failed to delete local log file: {ex.Message}", "LogUpload");
                        }
                    }
                }
                else
                {
                    _logger?.Error($"Failed to upload log file: {fileName}", "LogUpload");
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error uploading log file: {ex.Message}", "LogUpload", ex);
                return false;
            }
        }

        /// <summary>
        /// 보류 중인 모든 로그 파일을 서버로 업로드합니다(오늘 날짜의 파일 제외).
        /// 업로드 실패해도 다음 파일로 계속 진행합니다.
        /// </summary>
        public async Task<bool> UploadAllPendingLogsAsync()
        {
            try
            {
                if (!Directory.Exists(_logDirectory))
                    return false;

                // 오늘 날짜의 로그는 제외 (현재 사용 중일 수 있음)
                var today = DateTime.Now.ToString("yyyyMMdd");
                var logFiles = Directory.GetFiles(_logDirectory, "*.log")
                    .Where(f => !Path.GetFileName(f).Contains(today))
                    .ToList();

                if (logFiles.Count == 0)
                {
                    _logger?.Information("No pending logs to upload", "LogUpload");
                    return true;
                }

                _logger?.Information($"Found {logFiles.Count} log files to upload", "LogUpload");

                var successCount = 0;
                foreach (var logFile in logFiles)
                {
                    // 각 파일을 업로드하되, 실패해도 계속 진행
                    if (await UploadLogFileAsync(logFile, deleteAfterUpload: true))
                    {
                        successCount++;
                    }

                    // 서버 부하를 줄이기 위해 각 업로드 사이에 약간의 지연
                    await Task.Delay(500);
                }

                _logger?.Information($"Uploaded {successCount}/{logFiles.Count} log files", "LogUpload");
                return successCount > 0;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error uploading pending logs: {ex.Message}", "LogUpload", ex);
                return false;
            }
        }

        /// <summary>
        /// 자동 업로드 기능을 활성화/비활성화 합니다.
        /// 활성화 시 매일 새벽 2시에 UploadAllPendingLogsAsync를 실행하도록 타이머를 설정합니다.
        /// </summary>
        public void EnableAutoUpload(bool enable)
        {
            _autoUploadEnabled = enable;

            if (enable)
            {
                // 매일 새벽 2시에 자동 업로드 실행
                _autoUploadTimer?.Dispose();
                _autoUploadTimer = new System.Threading.Timer(
                    async _ => await AutoUploadCallback(),
                    null,
                    GetTimeUntil2AM(),
                    TimeSpan.FromDays(1)
                );

                _logger?.Information("Auto log upload enabled (Daily at 2:00 AM)", "LogUpload");
            }
            else
            {
                _autoUploadTimer?.Dispose();
                _autoUploadTimer = null;
                _logger?.Information("Auto log upload disabled", "LogUpload");
            }
        }

        private TimeSpan GetTimeUntil2AM()
        {
            var now = DateTime.Now;
            var next2AM = DateTime.Today.AddHours(2);
            
            if (now >= next2AM)
            {
                next2AM = next2AM.AddDays(1);
            }

            return next2AM - now;
        }

        private async Task AutoUploadCallback()
        {
            try
            {
                _logger?.Information("Starting automatic log upload", "LogUpload");
                await UploadAllPendingLogsAsync();
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error in automatic log upload: {ex.Message}", "LogUpload", ex);
            }
        }

        /// <summary>
        /// 크래시나 치명적 오류 발생 시 현재 로그 파일을 즉시 업로드하도록 합니다.
        /// 현재 로그 파일을 flush하고 복사본을 업로드합니다(원본은 삭제하지 않음).
        /// </summary>
        public async Task<bool> UploadCurrentLogImmediatelyAsync()
        {
            try
            {
                // 현재 사용 중인 로그 파일을 flush하고 업로드
                await _logger.FlushAsync();
                var currentLogFile = _logger.GetLogFilePath();

                if (File.Exists(currentLogFile))
                {
                    // 현재 로그는 삭제하지 않고 복사본 업로드
                    return await UploadLogFileAsync(currentLogFile, deleteAfterUpload: false);
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error uploading current log: {ex.Message}", "LogUpload", ex);
                return false;
            }
        }
    }
}
