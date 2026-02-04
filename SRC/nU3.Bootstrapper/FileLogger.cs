using System;
using System.IO;
using System.Text;

namespace nU3.Bootstrapper
{
    /// <summary>
    /// 파일 기반 로거입니다. LOG 디렉토리에 날짜별 로그 파일을 생성합니다.
    /// </summary>
    public static class FileLogger
    {
        private static readonly string _logDirectory;
        private static readonly object _lock = new object();

        static FileLogger()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _logDirectory = Path.Combine(baseDir, "LOG");

            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        private static string GetLogFilePath()
        {
            return Path.Combine(_logDirectory, $"{DateTime.Now:yyyyMMdd}.log");
        }

        private static void WriteLog(string level, string message, Exception? ex = null)
        {
            lock (_lock)
            {
                try
                {
                    var logFilePath = GetLogFilePath();
                    var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    var sb = new StringBuilder();

                    sb.AppendLine($"[{timestamp}] [{level}] {message}");

                    if (ex != null)
                    {
                        sb.AppendLine($"  Exception Type: {ex.GetType().Name}");
                        sb.AppendLine($"  Exception Message: {ex.Message}");
                        if (ex.StackTrace != null)
                        {
                            sb.AppendLine($"  Stack Trace:");
                            foreach (var line in ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                sb.AppendLine($"    {line}");
                            }
                        }
                        if (ex.InnerException != null)
                        {
                            sb.AppendLine($"  Inner Exception: {ex.InnerException.Message}");
                        }
                    }

                    File.AppendAllText(logFilePath, sb.ToString());
                }
                catch
                {
                    // 로그 기록 실패시 무시 (무한 루프 방지)
                }
            }
        }

        public static void Info(string message)
        {
            WriteLog("INFO", message);
            Console.WriteLine($"[INFO] {message}");
        }

        public static void Warning(string message, Exception? ex = null)
        {            
            WriteLog("WARNING", message, ex);
            Console.WriteLine($"[WARNING] {message}");
            if (ex != null)
            {
                Console.WriteLine($"  Exception: {ex.Message}");
            }
            Console.ResetColor();
        }

        
        public static void Error(string message, Exception? ex = null)
        {
            WriteLog("ERROR", message, ex);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {message}");
            if (ex != null)
            {
                Console.WriteLine($"  Exception: {ex.Message}");
            }
            Console.ResetColor();
        }

        public static void Debug(string message)
        {
#if DEBUG
            WriteLog("DEBUG", message);
            Console.WriteLine($"[DEBUG] {message}");
#endif
        }

        public static void SectionStart(string sectionName)
        {
            Info($"========== {sectionName} 시작 ==========");
        }

        public static void SectionEnd(string sectionName)
        {
            Info($"========== {sectionName} 완료 ==========");
        }

        public static void ComponentOperation(string componentId, string operation, string details)
        {
            Info($"[Component: {componentId}] {operation} - {details}");
        }

        public static void ModuleOperation(string moduleId, string moduleName, string operation, string details)
        {
            Info($"[Module: {moduleId} ({moduleName})] {operation} - {details}");
        }
    }
}
