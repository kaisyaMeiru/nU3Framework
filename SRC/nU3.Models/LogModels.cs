using System;

namespace nU3.Models
{
    /// <summary>
    /// 로그 레벨
    /// </summary>
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Critical = 5
    }

    /// <summary>
    /// 로그 항목
    /// </summary>
    public class LogEntryDto
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public LogLevel Level { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string UserId { get; set; }
        public string MachineName { get; set; } = Environment.MachineName;
        public string IpAddress { get; set; }
        public string ProgramId { get; set; }
        public string MethodName { get; set; }
        public string AdditionalData { get; set; }
    }

    /// <summary>
    /// 서버 오딧 로그
    /// </summary>
    public class AuditLogDto
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Module { get; set; }
        public string Screen { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string IpAddress { get; set; }
        public string MachineName { get; set; } = Environment.MachineName;
        public bool IsSuccess { get; set; } = true;
        public string ErrorMessage { get; set; }
        public string AdditionalInfo { get; set; }
    }

    /// <summary>
    /// 오딧 액션 타입
    /// </summary>
    public static class AuditAction
    {
        public const string Create = "CREATE";
        public const string Read = "READ";
        public const string Update = "UPDATE";
        public const string Delete = "DELETE";
        public const string Login = "LOGIN";
        public const string Logout = "LOGOUT";
        public const string Print = "PRINT";
        public const string Export = "EXPORT";
        public const string Import = "IMPORT";
        public const string Execute = "EXECUTE";
        public const string Search = "SEARCH";
    }
}
