using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using nU3.Core.Interfaces;
using nU3.Models;

namespace nU3.Shell.Helpers
{
    public class CrashReporter : ICrashReporter
    {
        private readonly Form _mainForm;
        private readonly EmailSettings _emailSettings;
        private readonly string _crashLogDirectory;

        public CrashReporter(Form mainForm, EmailSettings emailSettings)
        {
            _mainForm = mainForm;
            _emailSettings = emailSettings;
            _crashLogDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "nU3.Framework",
                "CrashLogs"
            );

            if (!Directory.Exists(_crashLogDirectory))
            {
                Directory.CreateDirectory(_crashLogDirectory);
            }
        }

        public async Task<bool> ReportCrashAsync(Exception exception, string additionalInfo = null)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var screenshotPath = Path.Combine(_crashLogDirectory, $"crash_{timestamp}.png");
                var logFilePath = Path.Combine(_crashLogDirectory, $"crash_{timestamp}.log");

                bool screenshotCaptured = false;
                if (_mainForm != null && _mainForm.InvokeRequired)
                {
                    screenshotCaptured = (bool)_mainForm.Invoke(new Func<bool>(() =>
                        ScreenshotHelper.CaptureForm(_mainForm, screenshotPath)));
                }
                else if (_mainForm != null)
                {
                    screenshotCaptured = ScreenshotHelper.CaptureForm(_mainForm, screenshotPath);
                }
                else
                {
                    screenshotCaptured = ScreenshotHelper.CaptureScreen(screenshotPath);
                }

                CreateLogFile(exception, logFilePath, additionalInfo);

                var report = new ErrorReport
                {
                    Timestamp = DateTime.Now,
                    UserId = GetCurrentUserId(),
                    MachineName = Environment.MachineName,
                    ApplicationVersion = GetApplicationVersion(),
                    ExceptionType = exception.GetType().FullName,
                    ErrorMessage = exception.Message,
                    StackTrace = exception.ToString(),
                    AdditionalInfo = additionalInfo,
                    ScreenshotPath = screenshotCaptured ? screenshotPath : null,
                    LogFilePath = File.Exists(logFilePath) ? logFilePath : null
                };

                if (_emailSettings != null && !string.IsNullOrEmpty(_emailSettings.ToEmail))
                {
                    var emailSent = await EmailHelper.SendErrorReportAsync(report, _emailSettings);
                    return emailSent;
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Crash ���� ����: {ex.Message}");
                return false;
            }
        }

        private void CreateLogFile(Exception exception, string logFilePath, string additionalInfo)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("=".PadRight(80, '='));
                sb.AppendLine("nU3 Framework - ũ���� ����Ʈ");
                sb.AppendLine("=".PadRight(80, '='));
                sb.AppendLine();
                sb.AppendLine($"Ÿ�ӽ�����: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                sb.AppendLine($"����: {GetCurrentUserId()}");
                sb.AppendLine($"�ӽ�: {Environment.MachineName}");
                sb.AppendLine($"�ü�� ����: {Environment.OSVersion}");
                sb.AppendLine($"CLR ����: {Environment.Version}");
                sb.AppendLine($"���ø����̼� ����: {GetApplicationVersion()}");
                sb.AppendLine($"�۾� ���丮: {Environment.CurrentDirectory}");
                sb.AppendLine($"���� ��: {Environment.CommandLine}");
                sb.AppendLine();
                sb.AppendLine("-".PadRight(80, '-'));
                sb.AppendLine("���� ����");
                sb.AppendLine("-".PadRight(80, '-'));
                sb.AppendLine($"����: {exception.GetType().FullName}");
                sb.AppendLine($"�޽���: {exception.Message}");
                sb.AppendLine($"��ó: {exception.Source}");
                sb.AppendLine();
                sb.AppendLine("���� ����:");
                sb.AppendLine(exception.ToString());
                sb.AppendLine();

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
            catch
            {
            }
        }

        private string GetCurrentUserId()
        {
            try
            {
                var user = nU3.Core.Security.UserSession.Current;
                return user?.UserId ?? "�� �� ����";
            }
            catch
            {
                return "�� �� ����";
            }
        }

        private string GetApplicationVersion()
        {
            try
            {
                return Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "�� �� ����";
            }
            catch
            {
                return "�� �� ����";
            }
        }

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
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
