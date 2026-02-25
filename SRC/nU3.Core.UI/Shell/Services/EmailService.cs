using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using nU3.Models;

namespace nU3.Core.UI.Shell.Services
{
    /// <summary>
    /// �˸� �� ����Ʈ ������ ���� �̸��� �����Դϴ�.
    /// </summary>
    public class EmailService : IDisposable
    {
        private readonly EmailSettings _settings;
        private SmtpClient? _smtpClient;
        private bool _disposed;

        /// <summary>
        /// �������� �̸��� ���񽺸� �����մϴ�.
        /// </summary>
        public EmailService(EmailSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// ������ �̸����� �����մϴ�.
        /// </summary>
        public async Task<bool> SendAsync(
            string to,
            string subject,
            string body,
            bool isHtml = false,
            CancellationToken cancellationToken = default)
        {
            return await SendAsync(to, subject, body, isHtml, null, cancellationToken);
        }

        /// <summary>
        /// ÷�����ϰ� �Բ� �̸����� �����մϴ�.
        /// </summary>
        public async Task<bool> SendAsync(
            string to,
            string subject,
            string body,
            bool isHtml,
            string[]? attachmentPaths,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.FromEmail, _settings.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                message.To.Add(to);

                // ÷������ �߰�
                if (attachmentPaths != null)
                {
                    foreach (var path in attachmentPaths)
                    {
                        if (File.Exists(path))
                        {
                            message.Attachments.Add(new Attachment(path));
                        }
                    }
                }

                var client = GetOrCreateSmtpClient();
                await client.SendMailAsync(message, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Email send failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// ���� ����Ʈ �̸����� �����մϴ�.
        /// </summary>
        public async Task<bool> SendErrorReportAsync(
            ErrorReport report,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.FromEmail, _settings.FromName),
                    Subject = $"[nU3 Framework] ������ ���� ����Ʈ - {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    Body = BuildErrorReportHtml(report),
                    IsBodyHtml = true,
                    Priority = MailPriority.High
                };

                message.To.Add(_settings.ToEmail);

                // ��ũ���� ÷��
                if (!string.IsNullOrEmpty(report.ScreenshotPath) && File.Exists(report.ScreenshotPath))
                {
                    message.Attachments.Add(new Attachment(report.ScreenshotPath));
                }

                // �α� ���� ÷��
                if (!string.IsNullOrEmpty(report.LogFilePath) && File.Exists(report.LogFilePath))
                {
                    message.Attachments.Add(new Attachment(report.LogFilePath));
                }

                var client = GetOrCreateSmtpClient();
                await client.SendMailAsync(message, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error report email failed: {ex.Message}");
                return false;
            }
        }

        private SmtpClient GetOrCreateSmtpClient()
        {
            if (_smtpClient == null)
            {
                _smtpClient = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
                {
                    EnableSsl = _settings.EnableSsl,
                    Timeout = _settings.TimeoutMs
                };

                if (!string.IsNullOrEmpty(_settings.Username))
                {
                    _smtpClient.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
                }
            }
            return _smtpClient;
        }

        private static string BuildErrorReportHtml(ErrorReport report)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><body style='font-family: Segoe UI, Arial, sans-serif;'>");
            sb.AppendLine("<h2 style='color: #d32f2f;'>nU3 Framework ������ ���� ����Ʈ</h2>");
            sb.AppendLine("<hr/>");

            sb.AppendLine("<h3>�⺻ ����</h3>");
            sb.AppendLine("<table style='border-collapse: collapse; width: 100%;'>");
            AppendTableRow(sb, "�߻� �ð�", report.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
            AppendTableRow(sb, "�����", report.UserId ?? "Unknown");
            AppendTableRow(sb, "��ǻ�͸�", report.MachineName);
            AppendTableRow(sb, "���ø����̼�", report.ApplicationName ?? "nU3 Framework");
            AppendTableRow(sb, "����", report.ApplicationVersion ?? "Unknown");
            sb.AppendLine("</table>");

            sb.AppendLine("<h3>���� ����</h3>");
            sb.AppendLine("<table style='border-collapse: collapse; width: 100%;'>");
            AppendTableRow(sb, "���� Ÿ��", report.ExceptionType ?? "Unknown");
            AppendTableRow(sb, "���� �޽���", report.ErrorMessage ?? "No message", "#d32f2f");
            sb.AppendLine("</table>");

            sb.AppendLine("<h3>���� Ʈ���̽�</h3>");
            sb.AppendLine($"<pre style='background: #f5f5f5; padding: 15px; border: 1px solid #ddd; overflow-x: auto; font-size: 12px;'>{report.StackTrace}</pre>");

            if (!string.IsNullOrEmpty(report.AdditionalInfo))
            {
                sb.AppendLine("<h3>�߰� ����</h3>");
                sb.AppendLine($"<pre style='background: #f5f5f5; padding: 15px; border: 1px solid #ddd; overflow-x: auto; font-size: 12px;'>{report.AdditionalInfo}</pre>");
            }

            sb.AppendLine("<hr/>");
            sb.AppendLine("<p style='color: #666; font-size: 12px;'>�� ������ nU3 Framework�� �ڵ� ���� ������ �ý��ۿ��� �߼۵Ǿ����ϴ�.</p>");
            sb.AppendLine("</body></html>");

            return sb.ToString();
        }

        private static void AppendTableRow(StringBuilder sb, string label, string value, string? valueColor = null)
        {
            var colorStyle = valueColor != null ? $" color: {valueColor};" : "";
            sb.AppendLine($"<tr><td style='padding: 8px; border: 1px solid #ddd; background: #f5f5f5; width: 150px;'><strong>{label}</strong></td><td style='padding: 8px; border: 1px solid #ddd;{colorStyle}'>{value}</td></tr>");
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _smtpClient?.Dispose();
        }
    }

    /// <summary>
    /// ���� ����Ʈ ������
    /// </summary>
    public class ErrorReport
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string? UserId { get; set; }
        public string MachineName { get; set; } = Environment.MachineName;
        public string? ApplicationName { get; set; }
        public string? ApplicationVersion { get; set; }
        public string? ExceptionType { get; set; }
        public string? ErrorMessage { get; set; }
        public string? StackTrace { get; set; }
        public string? AdditionalInfo { get; set; }
        public string? ScreenshotPath { get; set; }
        public string? LogFilePath { get; set; }

        /// <summary>
        /// ���ܷκ��� ���� ����Ʈ�� �����մϴ�.
        /// </summary>
        public static ErrorReport FromException(Exception exception, string? additionalInfo = null)
        {
            return new ErrorReport
            {
                ExceptionType = exception.GetType().FullName,
                ErrorMessage = exception.Message,
                StackTrace = exception.ToString(),
                AdditionalInfo = additionalInfo,
                ApplicationVersion = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString(),
                ApplicationName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name
            };
        }
    }
}
