using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using nU3.Models;

namespace nU3.Core.UI.Shell.Services
{
    /// <summary>
    /// �浹(ũ����) ����Ʈ ����
    /// ��ũ������ ĸó�ϰ�, �α� ������ �����ϸ�, ���� �������� �����ϴ� ����� �����մϴ�.
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
        /// �̸��� �������� Ȱ��ȭ�Ǿ� �ִ��� ���θ� ��ȯ�մϴ�.
        /// </summary>
        public bool EmailReportingEnabled => _emailSettings != null && !string.IsNullOrEmpty(_emailSettings.ToEmail);

        /// <summary>
        /// ũ���� �α� ���͸� ��θ� ��ȯ�մϴ�.
        /// </summary>
        public string CrashLogDirectory => _crashLogDirectory;

        /// <summary>
        /// ũ���ð� ������ �� �߻��ϴ� �̺�Ʈ�Դϴ�.
        /// </summary>
        public event EventHandler<CrashReportedEventArgs>? CrashReported;

        /// <summary>
        /// CrashReportService ������
        /// </summary>
        /// <param name="mainForm">��ũ���� ĸó�� ����� ���� ��(����)</param>
        /// <param name="emailSettings">�̸��� ���� ����(����)</param>
        /// <param name="applicationName">���ø����̼� �̸�(�������� ���)</param>
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
        /// ���ܸ� �񵿱������� ����Ʈ�մϴ�.
        /// ��ũ���� ĸó, �α� ����, �̸��� ����(������ ���)�� �����մϴ�.
        /// </summary>
        /// <param name="exception">�߻��� ����</param>
        /// <param name="additionalInfo">�߰� ����(����)</param>
        /// <param name="cancellationToken">��� ��ū(����)</param>
        /// <returns>���������� ����Ʈ�� ����/���۵Ǹ� true�� ��ȯ�մϴ�.</returns>
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

                // ��ũ���� ĸó
                bool screenshotCaptured = CaptureScreenshot(screenshotPath);

                // �α� ���� ����
                CreateLogFile(exception, logFilePath, additionalInfo);

                // ���� ����Ʈ ����
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

                // �̸��� ����(�����Ǿ� �ִ� ���)
                if (EmailReportingEnabled && _emailService != null)
                {
                    emailSent = await _emailService.SendErrorReportAsync(report, cancellationToken);
                }

                // �̺�Ʈ �߻�
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
        /// ���� ������� ���ܸ� ����Ʈ�մϴ�. (�񵿱� ȣ���� ����� ���ؽ�Ʈ��)
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
        /// ������ ũ���� �α׸� �����մϴ�.
        /// </summary>
        /// <param name="daysToKeep">������ �ϼ�</param>
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
                        // ���� ���� ���� ������ ����
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to cleanup old logs: {ex.Message}");
            }
        }

        /// <summary>
        /// ũ���� �α� ���� ����� ��ȯ�մϴ�.
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

                // ���� ���ܵ� ���
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
                    sb.AppendLine("�߰� ����");
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
    /// ũ���� ���� �̺�Ʈ�� �̺�Ʈ �����Դϴ�.
    /// </summary>
    public class CrashReportedEventArgs : EventArgs
    {
        public ErrorReport? Report { get; set; }
        public bool EmailSent { get; set; }
        public string? LogFilePath { get; set; }
        public string? ScreenshotPath { get; set; }
    }
}
