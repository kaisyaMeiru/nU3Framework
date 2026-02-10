using System;
using System.IO;
using System.Text;

namespace nU3.Bootstrapper
{
    /// <summary>
    /// 파일 기반 로거입니다. LOG 폴더에 날짜별 로그 파일을 생성합니다.
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
                        sb.AppendLine($"  예외 타입: {ex.GetType().Name}");
                        sb.AppendLine($"  예외 메시지: {ex.Message}");
                        if (ex.StackTrace != null)
                        {
                            sb.AppendLine($"  스택 트레이스:");
                            foreach (var line in ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                sb.AppendLine($"    {line}");
                            }
                        }
                        if (ex.InnerException != null)
                        {
                            sb.AppendLine($"  내부 예외: {ex.InnerException.Message}");
                        }
                    }

                    File.AppendAllText(logFilePath, sb.ToString(), Encoding.UTF8);
                }
                catch
                {
                    // 로그 기록 실패 시 무시 (무한 루프 방지)
                }
            }
        }

        public static void Info(string message)
        {
            WriteLog("INFO", message);
            Console.WriteLine($"[정보] {message}");
        }

        public static void Warning(string message, Exception? ex = null)
        {
            WriteLog("WARNING", message, ex);
            Console.WriteLine($"[경고] {message}");
            if (ex != null)
            {
                Console.WriteLine($"  예외: {ex.Message}");
            }
            Console.ResetColor();
        }


        public static void Error(string message, Exception? ex = null)
        {
            WriteLog("ERROR", message, ex);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[오류] {message}");
            if (ex != null)
            {
                Console.WriteLine($"  예외: {ex.Message}");
            }
            Console.ResetColor();
        }

        public static void Debug(string message)
        {
#if DEBUG
            WriteLog("DEBUG", message);
            Console.WriteLine($"[디버그] {message}");
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
            Info($"[컴포넌트: {componentId}] {operation} - {details}");
        }

        public static void ModuleOperation(string moduleId, string moduleName, string operation, string details)
        {
            Info($"[모듈: {moduleId} ({moduleName})] {operation} - {details}");
        }
    }
}
