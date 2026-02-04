using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nU3.Core.UI.Shell.Services
{
    /// <summary>
    /// 충돌(크래시) 리포트 서비스
    /// 스크린샷을 캡처하고, 로그 파일을 생성하며, 오류 보고서를 전송하는 기능을 제공합니다.
    /// </summary>
    public class CrashReportService : IDisposable
    {
        private readonly Form? _mainForm;
        private readonly EmailSettings? _emailSettings;
        private readonly string _crashLogDirectory;
        private readonly string _applicationName;
        private EmailService? _emailService;
        private bool _disposed;

        /// <summary>
        /// 이메일 리포팅이 활성화되어 있는지 여부를 반환합니다.
        /// </summary>
        public bool EmailReportingEnabled => _emailSettings != null && !string.IsNullOrEmpty(_emailSettings.ToEmail);

        /// <summary>
        /// 크래시 로그 디렉터리 경로를 반환합니다.
        /// </summary>
        public string CrashLogDirectory => _crashLogDirectory;

        /// <summary>
        /// 크래시가 보고될 때 발생하는 이벤트입니다.
        /// </summary>
        public event EventHandler<CrashReportedEventArgs>? CrashReported;

        /// <summary>
        /// CrashReportService 생성자
        /// </summary>
        /// <param name="mainForm">스크린샷 캡처에 사용할 메인 폼(선택)</param>
        /// <param name="emailSettings">이메일 전송 설정(선택)</param>
        /// <param name="applicationName">애플리케이션 이름(보고서에 사용)</param>
        public CrashReportService(Form? mainForm = null, EmailSettings? emailSettings = null, string? applicationName = null)
        {
            _mainForm = mainForm;
            _emailSettings = emailSettings;
            _applicationName = applicationName ?? Assembly.GetEntryAssembly()?.GetName().Name ?? "nU3.Framework";

            _crashLogDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "nU3.Framework",
                "CrashLogs"
            );

            EnsureDirectoryExists(_crashLogDirectory);

            if (_emailSettings != null)
            {
                _emailService = new EmailService(_emailSettings);
            }
        }

        /// <summary>
        /// 예외를 비동기적으로 리포트합니다.
        /// 스크린샷 캡처, 로그 생성, 이메일 전송(설정된 경우)을 수행합니다.
        /// </summary>
        /// <param name="exception">발생한 예외</param>
        /// <param name="additionalInfo">추가 정보(선택)</param>
        /// <param name="cancellationToken">취소 토큰(선택)</param>
        /// <returns>성공적으로 리포트가 생성/전송되면 true를 반환합니다.</returns>
        public async Task<bool> ReportCrashAsync(
            Exception exception,
            string? additionalInfo = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var screenshotPath = Path.Combine(_crashLogDirectory, $"crash_{timestamp}.png");
                var logFilePath = Path.Combine(_crashLogDirectory, $"crash_{timestamp}.log");

                // 스크린샷 캡처
                bool screenshotCaptured = CaptureScreenshot(screenshotPath);

                // 로그 파일 생성
                CreateLogFile(exception, logFilePath, additionalInfo);

                // 에러 리포트 생성
                var report = new ErrorReport
                {
                    Timestamp = DateTime.Now,
                    UserId = GetCurrentUserId(),
                    MachineName = Environment.MachineName,
                    ApplicationName = _applicationName,
                    ApplicationVersion = GetApplicationVersion(),
                    ExceptionType = exception.GetType().FullName,
                    ErrorMessage = exception.Message,
                    StackTrace = exception.ToString(),
                    AdditionalInfo = additionalInfo,
                    ScreenshotPath = screenshotCaptured ? screenshotPath : null,
                    LogFilePath = File.Exists(logFilePath) ? logFilePath : null
                };

                bool emailSent = false;

                // 이메일 전송(구성되어 있는 경우)
                if (EmailReportingEnabled && _emailService != null)
                {
                    emailSent = await _emailService.SendErrorReportAsync(report, cancellationToken);
                }

                // 이벤트 발생
                CrashReported?.Invoke(this, new CrashReportedEventArgs
                {
                    Report = report,
                    EmailSent = emailSent,
                    LogFilePath = logFilePath,
                    ScreenshotPath = screenshotCaptured ? screenshotPath : null
                });

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to report crash: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 동기 방식으로 예외를 리포트합니다. (비동기 호출이 어려운 컨텍스트용)
        /// </summary>
        public bool ReportCrash(Exception exception, string? additionalInfo = null)
        {
            try
            {
                var task = ReportCrashAsync(exception, additionalInfo, CancellationToken.None);
                return task.GetAwaiter().GetResult();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 오래된 크래시 로그를 정리합니다.
        /// </summary>
        /// <param name="daysToKeep">보관할 일수</param>
        public void CleanupOldLogs(int daysToKeep = 30)
        {
            try
            {
                if (!Directory.Exists(_crashLogDirectory))
                    return;

                var files = Directory.GetFiles(_crashLogDirectory);
                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

                foreach (var file in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        if (fileInfo.CreationTime < cutoffDate)
                        {
                            File.Delete(file);
                        }
                    }
                    catch
                    {
                        // 개별 파일 삭제 오류는 무시
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to cleanup old logs: {ex.Message}");
            }
        }

        /// <summary>
        /// 크래시 로그 파일 목록을 반환합니다.
        /// </summary>
        public string[] GetCrashLogs()
        {
            try
            {
                if (!Directory.Exists(_crashLogDirectory))
                    return Array.Empty<string>();

                return Directory.GetFiles(_crashLogDirectory, "crash_*.log");
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        private bool CaptureScreenshot(string path)
        {
            try
            {
                if (_mainForm != null && !_mainForm.IsDisposed)
                {
                    if (_mainForm.InvokeRequired)
                    {
                        return (bool)_mainForm.Invoke(new Func<bool>(() =>
                            ScreenshotService.CaptureForm(_mainForm, path)));
                    }
                    return ScreenshotService.CaptureForm(_mainForm, path);
                }
                return ScreenshotService.CaptureScreen(path);
            }
            catch
            {
                return false;
            }
        }

        private void CreateLogFile(Exception exception, string logFilePath, string? additionalInfo)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("=".PadRight(80, '='));
                sb.AppendLine($"{_applicationName} - Crash Report");
                sb.AppendLine("=".PadRight(80, '='));
                sb.AppendLine();
                sb.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                sb.AppendLine($"User: {GetCurrentUserId()}");
                sb.AppendLine($"Machine: {Environment.MachineName}");
                sb.AppendLine($"OS Version: {Environment.OSVersion}");
                sb.AppendLine($"CLR Version: {Environment.Version}");
                sb.AppendLine($"Application Version: {GetApplicationVersion()}");
                sb.AppendLine($"Working Directory: {Environment.CurrentDirectory}");
                sb.AppendLine($"Command Line: {Environment.CommandLine}");
                sb.AppendLine($"64-bit OS: {Environment.Is64BitOperatingSystem}");
                sb.AppendLine($"64-bit Process: {Environment.Is64BitProcess}");
                sb.AppendLine($"Processor Count: {Environment.ProcessorCount}");
                sb.AppendLine();
                sb.AppendLine("-".PadRight(80, '-'));
                sb.AppendLine("Exception Information");
                sb.AppendLine("-".PadRight(80, '-'));
                sb.AppendLine($"Type: {exception.GetType().FullName}");
                sb.AppendLine($"Message: {exception.Message}");
                sb.AppendLine($"Source: {exception.Source}");
                sb.AppendLine($"TargetSite: {exception.TargetSite}");
                sb.AppendLine();
                sb.AppendLine("Stack Trace:");
                sb.AppendLine(exception.ToString());
                sb.AppendLine();

                // 내부 예외도 기록
                var innerException = exception.InnerException;
                var depth = 1;
                while (innerException != null)
                {
                    sb.AppendLine("-".PadRight(80, '-'));
                    sb.AppendLine($"Inner Exception {depth}");
                    sb.AppendLine("-".PadRight(80, '-'));
                    sb.AppendLine($"Type: {innerException.GetType().FullName}");
                    sb.AppendLine($"Message: {innerException.Message}");
                    sb.AppendLine($"Stack Trace: {innerException.StackTrace}");
                    sb.AppendLine();
                    innerException = innerException.InnerException;
                    depth++;
                }

                if (!string.IsNullOrEmpty(additionalInfo))
                {
                    sb.AppendLine("-".PadRight(80, '-'));
                    sb.AppendLine("추가 정보");
                    sb.AppendLine("-".PadRight(80, '-'));
                    sb.AppendLine(additionalInfo);
                    sb.AppendLine();
                }

                sb.AppendLine("=".PadRight(80, '='));

                File.WriteAllText(logFilePath, sb.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to create log file: {ex.Message}");
            }
        }

        private static string GetCurrentUserId()
        {
            try
            {
                var user = nU3.Core.Security.UserSession.Current;
                return user?.UserId ?? Environment.UserName;
            }
            catch
            {
                return Environment.UserName;
            }
        }

        private static string GetApplicationVersion()
        {
            try
            {
                return Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _emailService?.Dispose();
        }
    }

    /// <summary>
    /// 크래시 보고 이벤트의 이벤트 인자입니다.
    /// </summary>
    public class CrashReportedEventArgs : EventArgs
    {
        public ErrorReport? Report { get; set; }
        public bool EmailSent { get; set; }
        public string? LogFilePath { get; set; }
        public string? ScreenshotPath { get; set; }
    }
}
