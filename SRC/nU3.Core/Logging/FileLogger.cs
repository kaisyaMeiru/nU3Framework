using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using nU3.Models;

namespace nU3.Core.Logging
{
    /// <summary>
    /// 파일 기반의 런타임 로거 구현체입니다.
    /// 
    /// 주요 기능:
    /// - 로그 메시지를 스레드 안전한 큐에 수집하고 주기적으로 파일로 기록합니다.
    /// - 파일명은 "{MachineName}_{IPAddress}_{yyyyMMdd}.log" 형식입니다.
    /// - 로그 수준에 따라 즉시 flush를 수행(에러 및 치명적 수준)합니다.
    /// - 오래된 로그 파일 자동 정리 기능 제공
    /// </summary>
    public class FileLogger : ILogger, IDisposable
    {
        private readonly string _logDirectory;
        private readonly string _machineName;
        private readonly string _ipAddress;
        private readonly ConcurrentQueue<LogEntryDto> _logQueue;
        private readonly Timer _flushTimer;
        private readonly SemaphoreSlim _writeLock;
        private string _currentLogFile;
        private DateTime _currentDate;
        private bool _disposed;

        /// <summary>
        /// 생성자: 기본 로그 디렉토리는 ApplicationData/nU3.Framework/LOG 입니다.
        /// </summary>
        /// <param name="baseDirectory">사용자 지정 로그 디렉토리(옵션)</param>
        public FileLogger(string baseDirectory = null)
        {
            _logDirectory = baseDirectory ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "nU3.Framework",
                "LOG"
            );

            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }

            _machineName = Environment.MachineName;
            _ipAddress = GetLocalIPAddress();
            _logQueue = new ConcurrentQueue<LogEntryDto>();
            _writeLock = new SemaphoreSlim(1, 1);
            _currentDate = DateTime.Now.Date;
            _currentLogFile = GetLogFileName();

            // 5초마다 큐를 파일로 flush
            _flushTimer = new Timer(async _ => await FlushAsync(), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// 로컬 IPv4 주소를 조회합니다. 실패 시 "127.0.0.1"을 반환합니다.
        /// </summary>
        private string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }

        /// <summary>
        /// 현재 날짜에 해당하는 로그 파일 경로를 생성합니다.
        /// 파일명: {MachineName}_{IpAddress}_{yyyyMMdd}.log
        /// </summary>
        private string GetLogFileName()
        {
            var date = DateTime.Now.ToString("yyyyMMdd");
            return Path.Combine(_logDirectory, $"{_machineName}_{_ipAddress}_{date}.log");
        }

        public void Trace(string message, string category = null, Exception exception = null)
            => Log(LogLevel.Trace, message, category, exception);

        public void Debug(string message, string category = null, Exception exception = null)
            => Log(LogLevel.Debug, message, category, exception);

        public void Information(string message, string category = null, Exception exception = null)
            => Log(LogLevel.Information, message, category, exception);

        public void Warning(string message, string category = null, Exception exception = null)
            => Log(LogLevel.Warning, message, category, exception);

        public void Error(string message, string category = null, Exception exception = null)
            => Log(LogLevel.Error, message, category, exception);

        public void Critical(string message, string category = null, Exception exception = null)
            => Log(LogLevel.Critical, message, category, exception);

        /// <summary>
        /// 로그 항목을 생성하고 큐에 추가합니다. Error 이상 레벨은 즉시 비동기 flush를 트리거합니다.
        /// </summary>
        public void Log(LogLevel level, string message, string category = null, Exception exception = null)
        {
            try
            {
                var entry = new LogEntryDto
                {
                    Timestamp = DateTime.Now,
                    Level = level,
                    Category = category ?? "General",
                    Message = message,
                    Exception = exception?.ToString(),
                    MachineName = _machineName,
                    IpAddress = _ipAddress
                };

                // 사용자 정보 보강(예외 발생 시 무시)
                try
                {
                    var user = nU3.Core.Security.UserSession.Current;
                    if (user != null)
                    {
                        entry.UserId = user.UserId;
                    }
                }
                catch { }

                _logQueue.Enqueue(entry);

                // Error 이상 레벨인 경우 즉시 flush 시도
                if (level >= LogLevel.Error)
                {
                    Task.Run(async () => await FlushAsync());
                }
            }
            catch
            {
                // 로깅 실패는 애플리케이션 흐름에 영향을 주지 않도록 무시
            }
        }

        /// <summary>
        /// 큐에 있는 로그를 파일에 기록합니다.
        /// - 날짜 변경이 감지되면 파일명을 갱신하여 날짜별 파일로 저장합니다.
        /// - 파일 쓰기는 SemaphoreSlim으로 보호합니다.
        /// </summary>
        public async Task FlushAsync()
        {
            if (_logQueue.IsEmpty || _disposed)
                return;

            await _writeLock.WaitAsync();
            try
            {
                // 날짜가 변경되었으면 새 파일 사용
                if (DateTime.Now.Date != _currentDate)
                {
                    _currentDate = DateTime.Now.Date;
                    _currentLogFile = GetLogFileName();
                }

                var entries = new System.Collections.Generic.List<LogEntryDto>();
                while (_logQueue.TryDequeue(out var entry))
                {
                    entries.Add(entry);
                }

                if (entries.Count == 0)
                    return;

                var sb = new StringBuilder();
                foreach (var entry in entries)
                {
                    sb.AppendLine(FormatLogEntry(entry));
                }

                await File.AppendAllTextAsync(_currentLogFile, sb.ToString());
            }
            catch
            {
                // 파일 쓰기 실패는 무시(로그 자체가 장애를 유발하면 안됨)
            }
            finally
            {
                _writeLock.Release();
            }
        }

        /// <summary>
        /// 로그 항목을 사람이 읽기 편한 문자열로 포맷합니다.
        /// 포함 정보: 타임스탬프, 레벨, 카테고리, 사용자, 프로그램ID, 메시지, 예외, 추가 데이터
        /// </summary>
        private string FormatLogEntry(LogEntryDto entry)
        {
            var sb = new StringBuilder();
            sb.Append($"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] ");
            sb.Append($"[{entry.Level.ToString().ToUpper().PadRight(11)}] ");
            sb.Append($"[{entry.Category}] ");
            
            if (!string.IsNullOrEmpty(entry.UserId))
            {
                sb.Append($"[User:{entry.UserId}] ");
            }

            if (!string.IsNullOrEmpty(entry.ProgramId))
            {
                sb.Append($"[Prog:{entry.ProgramId}] ");
            }

            sb.Append(entry.Message);

            if (!string.IsNullOrEmpty(entry.Exception))
            {
                sb.AppendLine();
                sb.Append("    Exception: ");
                sb.Append(entry.Exception.Replace("\n", "\n    "));
            }

            if (!string.IsNullOrEmpty(entry.AdditionalData))
            {
                sb.AppendLine();
                sb.Append("    Additional: ");
                sb.Append(entry.AdditionalData);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 지정한 일수(daysToKeep) 이전의 오래된 로그 파일을 삭제합니다. 기본값: 30일
        /// </summary>
        public void CleanupOldLogs(int daysToKeep = 30)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
                var files = Directory.GetFiles(_logDirectory, "*.log");

                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 특정 날짜의 로그 파일 경로를 반환합니다. 기본값: 오늘 날짜
        /// </summary>
        public string GetLogFilePath(DateTime? date = null)
        {
            var targetDate = date ?? DateTime.Now;
            var dateStr = targetDate.ToString("yyyyMMdd");
            return Path.Combine(_logDirectory, $"{_machineName}_{_ipAddress}_{dateStr}.log");
        }

        /// <summary>
        /// 존재하는 모든 로그 파일의 경로 배열을 생성일 내림차순으로 반환합니다.
        /// 예외 발생 시 빈 배열 반환
        /// </summary>
        public string[] GetAllLogFiles()
        {
            try
            {
                return Directory.GetFiles(_logDirectory, "*.log")
                    .OrderByDescending(f => File.GetCreationTime(f))
                    .ToArray();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// 리소스 해제: 타이머 정지, 남은 로그 flush, 락 해제
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _flushTimer?.Dispose();
            FlushAsync().Wait();
            _writeLock?.Dispose();
        }
    }
}
