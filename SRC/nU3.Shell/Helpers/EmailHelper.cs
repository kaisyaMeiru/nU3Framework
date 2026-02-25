using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using nU3.Models;

namespace nU3.Shell.Helpers
{
    public static class EmailHelper
    {
        public static async Task<bool> SendErrorReportAsync(ErrorReport report, EmailSettings settings)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(settings.FromEmail, settings.FromName);
                    message.To.Add(settings.ToEmail);
                    message.Subject = $"[nU3 Framework] ���� ����Ʈ - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                    message.IsBodyHtml = true;
                    message.Body = BuildErrorReportHtml(report);
                    message.Priority = MailPriority.High;

                    if (report.ScreenshotPath != null && File.Exists(report.ScreenshotPath))
                    {
                        var attachment = new Attachment(report.ScreenshotPath);
                        message.Attachments.Add(attachment);
                    }

                    if (report.LogFilePath != null && File.Exists(report.LogFilePath))
                    {
                        var attachment = new Attachment(report.LogFilePath);
                        message.Attachments.Add(attachment);
                    }

                    using (var client = new SmtpClient(settings.SmtpServer, settings.SmtpPort))
                    {
                        client.EnableSsl = settings.EnableSsl;
                        
                        if (!string.IsNullOrEmpty(settings.Username))
                        {
                            client.Credentials = new NetworkCredential(settings.Username, settings.Password);
                        }

                        await client.SendMailAsync(message);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to send error report email: {ex.Message}");
                return false;
            }
        }

        private static string BuildErrorReportHtml(ErrorReport report)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><body style='font-family: Segoe UI, Arial, sans-serif;'>");
            sb.AppendLine("<h2 style='color: #d32f2f;'>nU3 Framework ���� ����Ʈ</h2>");
            sb.AppendLine("<hr/>");
            
            sb.AppendLine("<h3>�⺻ ����</h3>");
            sb.AppendLine("<table style='border-collapse: collapse; width: 100%;'>");
            sb.AppendLine($"<tr><td style='padding: 8px; border: 1px solid #ddd; background: #f5f5f5; width: 150px;'><strong>�߻� �ð�</strong></td><td style='padding: 8px; border: 1px solid #ddd;'>{report.Timestamp:yyyy-MM-dd HH:mm:ss}</td></tr>");
            sb.AppendLine($"<tr><td style='padding: 8px; border: 1px solid #ddd; background: #f5f5f5;'><strong>�����</strong></td><td style='padding: 8px; border: 1px solid #ddd;'>{report.UserId}</td></tr>");
            sb.AppendLine($"<tr><td style='padding: 8px; border: 1px solid #ddd; background: #f5f5f5;'><strong>ȣ��Ʈ</strong></td><td style='padding: 8px; border: 1px solid #ddd;'>{report.MachineName}</td></tr>");
            sb.AppendLine($"<tr><td style='padding: 8px; border: 1px solid #ddd; background: #f5f5f5;'><strong>���ø����̼� ����</strong></td><td style='padding: 8px; border: 1px solid #ddd;'>{report.ApplicationVersion}</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<h3>���� ����</h3>");
            sb.AppendLine("<table style='border-collapse: collapse; width: 100%;'>");
            sb.AppendLine($"<tr><td style='padding: 8px; border: 1px solid #ddd; background: #f5f5f5; width: 150px;'><strong>���� Ÿ��</strong></td><td style='padding: 8px; border: 1px solid #ddd;'>{report.ExceptionType}</td></tr>");
            sb.AppendLine($"<tr><td style='padding: 8px; border: 1px solid #ddd; background: #f5f5f5;'><strong>���� �޽���</strong></td><td style='padding: 8px; border: 1px solid #ddd; color: #d32f2f;'>{report.ErrorMessage}</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<h3>���� Ʈ���̽�</h3>");
            sb.AppendLine($"<pre style='background: #f5f5f5; padding: 15px; border: 1px solid #ddd; overflow-x: auto; font-size: 12px;'>{report.StackTrace}</pre>");

            if (!string.IsNullOrEmpty(report.AdditionalInfo))
            {
                sb.AppendLine("<h3>�߰� ����</h3>");
                sb.AppendLine($"<pre style='background: #f5f5f5; padding: 15px; border: 1px solid #ddd; overflow-x: auto; font-size: 12px;'>{report.AdditionalInfo}</pre>");
            }

            sb.AppendLine("<hr/>");
            sb.AppendLine("<p style='color: #666; font-size: 12px;'>�� �̸����� nU3 Framework�� ���� �ڵ����� �����Ǿ����ϴ�.</p>");
            sb.AppendLine("</body></html>");

            return sb.ToString();
        }
    }

    public class ErrorReport
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string UserId { get; set; }
        public string MachineName { get; set; } = Environment.MachineName;
        public string ApplicationVersion { get; set; }
        public string ExceptionType { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public string AdditionalInfo { get; set; }
        public string ScreenshotPath { get; set; }
        public string LogFilePath { get; set; }
    }
}
