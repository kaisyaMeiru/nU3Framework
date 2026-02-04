using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nU3.Core.UI.Shell.Services
{
    /// <summary>
    /// 알림 및 리포트 전송을 위한 이메일 서비스입니다.
    /// </summary>
    public class EmailService : IDisposable
    {
        private readonly EmailSettings _settings;
        private SmtpClient? _smtpClient;
        private bool _disposed;

        /// <summary>
        /// 설정으로 이메일 서비스를 생성합니다.
        /// </summary>
        public EmailService(EmailSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// 간단한 이메일을 전송합니다.
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
        /// 첨부파일과 함께 이메일을 전송합니다.
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

                // 첨부파일 추가
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
        /// 에러 리포트 이메일을 전송합니다.
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
                    Subject = $"[nU3 Framework] 비정상 종료 리포트 - {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    Body = BuildErrorReportHtml(report),
                    IsBodyHtml = true,
                    Priority = MailPriority.High
                };

                message.To.Add(_settings.ToEmail);

                // 스크린샷 첨부
                if (!string.IsNullOrEmpty(report.ScreenshotPath) && File.Exists(report.ScreenshotPath))
                {
                    message.Attachments.Add(new Attachment(report.ScreenshotPath));
                }

                // 로그 파일 첨부
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
            sb.AppendLine("<h2 style='color: #d32f2f;'>nU3 Framework 비정상 종료 리포트</h2>");
            sb.AppendLine("<hr/>");

            sb.AppendLine("<h3>기본 정보</h3>");
            sb.AppendLine("<table style='border-collapse: collapse; width: 100%;'>");
            AppendTableRow(sb, "발생 시간", report.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
            AppendTableRow(sb, "사용자", report.UserId ?? "Unknown");
            AppendTableRow(sb, "컴퓨터명", report.MachineName);
            AppendTableRow(sb, "애플리케이션", report.ApplicationName ?? "nU3 Framework");
            AppendTableRow(sb, "버전", report.ApplicationVersion ?? "Unknown");
            sb.AppendLine("</table>");

            sb.AppendLine("<h3>에러 정보</h3>");
            sb.AppendLine("<table style='border-collapse: collapse; width: 100%;'>");
            AppendTableRow(sb, "에러 타입", report.ExceptionType ?? "Unknown");
            AppendTableRow(sb, "에러 메시지", report.ErrorMessage ?? "No message", "#d32f2f");
            sb.AppendLine("</table>");

            sb.AppendLine("<h3>스택 트레이스</h3>");
            sb.AppendLine($"<pre style='background: #f5f5f5; padding: 15px; border: 1px solid #ddd; overflow-x: auto; font-size: 12px;'>{report.StackTrace}</pre>");

            if (!string.IsNullOrEmpty(report.AdditionalInfo))
            {
                sb.AppendLine("<h3>추가 정보</h3>");
                sb.AppendLine($"<pre style='background: #f5f5f5; padding: 15px; border: 1px solid #ddd; overflow-x: auto; font-size: 12px;'>{report.AdditionalInfo}</pre>");
            }

            sb.AppendLine("<hr/>");
            sb.AppendLine("<p style='color: #666; font-size: 12px;'>이 메일은 nU3 Framework의 자동 에러 리포팅 시스템에서 발송되었습니다.</p>");
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
    /// 에러 리포트 데이터
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
        /// 예외로부터 에러 리포트를 생성합니다.
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
