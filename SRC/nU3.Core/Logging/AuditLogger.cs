using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using nU3.Models;

namespace nU3.Core.Logging
{
    /// <summary>
    /// 오딧(Audit) 로거의 구현체입니다.
    /// 
    /// 기능 및 동작:
    /// - 애플리케이션에서 발생하는 주요 액션(Create/Update/Delete/Read 등)을 구조화된 AuditLogDto로 큐에 적재합니다.
    /// - 내부적으로 ConcurrentQueue를 사용하여 스레드 세이프하게 로그를 수집합니다.
    /// - 타이머를 이용해 주기적으로 큐를 파일에 flush(비동기 기록)합니다.
    /// - 로그 파일은 "{MachineName}_{IPAddress}_AUDIT_{yyyyMMdd}.json" 형식으로 저장되며,
    ///   각 라인은 JSON 직렬화된 AuditLogDto 객체(한 줄에 하나의 JSON)로 기록됩니다.
    /// - 파일 쓰기는 SemaphoreSlim으로 직렬화하여 동시 쓰기 충돌을 방지합니다.
    /// - Dispose 시 남아있는 로그를 동기적으로 Flush하고 타이머를 해제합니다.
    /// 
    /// 주의사항:
    /// - 민감 정보(개인정보 등)를 로깅하지 않도록 호출부에서 주의해야 합니다.
    /// - 성능을 위해 큐에 적재만 하고 파일 기록은 비동기 배치로 수행하므로 즉시 파일에 반영되지 않을 수 있습니다.
    /// </summary>
    public class AuditLogger : IAuditLogger, IDisposable
    {
        private readonly string _auditDirectory;
        private readonly string _machineName;
        private readonly string _ipAddress;
        private readonly ConcurrentQueue<AuditLogDto> _auditQueue;
        private readonly Timer _flushTimer;
        private readonly SemaphoreSlim _writeLock;
        private string _currentAuditFile;
        private DateTime _currentDate;
        private bool _disposed;

        /// <summary>
        /// AuditLogger 생성자. 기본 로그 디렉터리는 사용자 ApplicationData 아래 "nU3.Framework/AUDIT" 입니다.
        /// </summary>
        /// <param name="baseDirectory">사용자 지정 로그 디렉터리(생략 가능)</param>
        public AuditLogger(string baseDirectory = null)
        {
            _auditDirectory = baseDirectory ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "nU3.Framework",
                "AUDIT"
            );

            if (!Directory.Exists(_auditDirectory))
            {
                Directory.CreateDirectory(_auditDirectory);
            }

            _machineName = Environment.MachineName;
            _ipAddress = GetLocalIPAddress();
            _auditQueue = new ConcurrentQueue<AuditLogDto>();
            _writeLock = new SemaphoreSlim(1, 1);
            _currentDate = DateTime.Now.Date;
            _currentAuditFile = GetAuditFileName();

            // 10초마다 폴더에 flush 시도
            _flushTimer = new Timer(async _ => await FlushAsync(), null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// 로컬 머신의 IPv4 주소를 반환합니다. 실패 시 로컬 루프백(127.0.0.1)을 반환합니다.
        /// </summary>
        private string GetLocalIPAddress()
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
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
        /// 현재 날짜 기준으로 기록할 Audit 파일 경로를 생성합니다.
        /// 파일명 형식: {MachineName}_{IpAddress}_AUDIT_{yyyyMMdd}.json
        /// </summary>
        private string GetAuditFileName()
        {
            var date = DateTime.Now.ToString("yyyyMMdd");
            return Path.Combine(_auditDirectory, $"{_machineName}_{_ipAddress}_AUDIT_{date}.json");
        }

        /// <summary>
        /// Audit 로그 항목을 큐에 추가합니다. 내부에서 타임스탬프와 머신/사용자 정보를 보강합니다.
        /// 호출은 스레드 세이프하며 예외는 내부에서 흡수됩니다(로깅 오류가 애플리케이션 동작을 방해하지 않도록).
        /// </summary>
        public void LogAudit(AuditLogDto audit)
        {
            try
            {
                // 기본 정보 설정
                audit.Timestamp = DateTime.Now;
                audit.MachineName = _machineName;
                audit.IpAddress = _ipAddress;

                // 현재 사용자 정보 추가 (UserSession이 예외를 일으켜도 무시)
                try
                {
                    var user = nU3.Core.Security.UserSession.Current;
                    if (user != null)
                    {
                        audit.UserId = user.UserId;
                        audit.UserName = user.UserName;
                    }
                }
                catch { }

                _auditQueue.Enqueue(audit);
            }
            catch { }
        }

        /// <summary>
        /// 엔티티 생성 이벤트를 기록합니다. 기본적으로 Action은 "Create"로 설정됩니다.
        /// </summary>
        public void LogCreate(string entityType, string entityId, string newValue, string module = null, string screen = null)
        {
            LogAudit(new AuditLogDto
            {
                Action = AuditAction.Create,
                EntityType = entityType,
                EntityId = entityId,
                NewValue = newValue,
                Module = module,
                Screen = screen
            });
        }

        /// <summary>
        /// 엔티티 수정 이벤트를 기록합니다.
        /// </summary>
        public void LogUpdate(string entityType, string entityId, string oldValue, string newValue, string module = null, string screen = null)
        {
            LogAudit(new AuditLogDto
            {
                Action = AuditAction.Update,
                EntityType = entityType,
                EntityId = entityId,
                OldValue = oldValue,
                NewValue = newValue,
                Module = module,
                Screen = screen
            });
        }

        /// <summary>
        /// 엔티티 삭제 이벤트를 기록합니다.
        /// </summary>
        public void LogDelete(string entityType, string entityId, string oldValue, string module = null, string screen = null)
        {
            LogAudit(new AuditLogDto
            {
                Action = AuditAction.Delete,
                EntityType = entityType,
                EntityId = entityId,
                OldValue = oldValue,
                Module = module,
                Screen = screen
            });
        }

        /// <summary>
        /// 엔티티 조회(Read) 이벤트를 기록합니다.
        /// </summary>
        public void LogRead(string entityType, string entityId, string module = null, string screen = null)
        {
            LogAudit(new AuditLogDto
            {
                Action = AuditAction.Read,
                EntityType = entityType,
                EntityId = entityId,
                Module = module,
                Screen = screen
            });
        }

        /// <summary>
        /// 자유 형식의 액션을 기록합니다(예: 사용자 동작, 버튼 클릭 등).
        /// </summary>
        public void LogAction(string action, string module, string screen, string additionalInfo = null)
        {
            LogAudit(new AuditLogDto
            {
                Action = action,
                Module = module,
                Screen = screen,
                AdditionalInfo = additionalInfo
            });
        }

        /// <summary>
        /// 큐에 적재된 Audit 항목들을 파일에 비동기 기록합니다.
        /// - 동일 날짜가 아닐 경우 파일명을 교체하여 날짜별 파일로 기록합니다.
        /// - 내부적으로 JsonSerializer를 사용해 각 항목을 한 줄의 JSON으로 직렬화하여 기록합니다.
        /// - 동시 쓰기를 SemaphoreSlim으로 보호합니다.
        /// </summary>
        public async Task FlushAsync()
        {
            if (_auditQueue.IsEmpty || _disposed)
                return;

            await _writeLock.WaitAsync();
            try
            {
                if (DateTime.Now.Date != _currentDate)
                {
                    _currentDate = DateTime.Now.Date;
                    _currentAuditFile = GetAuditFileName();
                }

                var entries = new System.Collections.Generic.List<AuditLogDto>();
                while (_auditQueue.TryDequeue(out var entry))
                {
                    entries.Add(entry);
                }

                if (entries.Count == 0)
                    return;

                var sb = new StringBuilder();
                foreach (var entry in entries)
                {
                    var json = JsonSerializer.Serialize(entry, new JsonSerializerOptions 
                    { 
                        WriteIndented = false 
                    });
                    sb.AppendLine(json);
                }

                await File.AppendAllTextAsync(_currentAuditFile, sb.ToString());
            }
            catch { }
            finally
            {
                _writeLock.Release();
            }
        }

        /// <summary>
        /// 지정일수(daysToKeep) 이전의 오래된 Audit 파일을 삭제합니다.
        /// 기본 보관 기간은 90일입니다.
        /// </summary>
        public void CleanupOldAudits(int daysToKeep = 90)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
                var files = Directory.GetFiles(_auditDirectory, "*_AUDIT_*.json");

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
        /// 저장된 모든 Audit 파일의 경로 배열을, 생성일 내림차순으로 반환합니다.
        /// 예외가 발생하면 빈 배열을 반환합니다.
        /// </summary>
        public string[] GetAllAuditFiles()
        {
            try
            {
                return Directory.GetFiles(_auditDirectory, "*_AUDIT_*.json")
                    .OrderByDescending(f => File.GetCreationTime(f))
                    .ToArray();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// 리소스를 해제합니다. 타이머를 정지시키고 남아있는 큐를 flush 합니다.
        /// Dispose 이후에는 인스턴스를 재사용하면 안 됩니다.
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
