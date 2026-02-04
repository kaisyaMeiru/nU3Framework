using Microsoft.Extensions.Diagnostics.HealthChecks;
using nU3.Server.Connectivity.Services;

namespace nU3.Server.Host.HealthChecks
{
    /// <summary>
    /// 파일 시스템 상태를 확인하는 Health Check
    /// </summary>
    public class FileSystemHealthCheck : IHealthCheck
    {
        private readonly ServerFileTransferService _fileService;

        public FileSystemHealthCheck(ServerFileTransferService fileService)
        {
            _fileService = fileService;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var homeDir = _fileService.GetHomeDirectory();
                
                if (string.IsNullOrEmpty(homeDir))
                {
                    return Task.FromResult(
                        HealthCheckResult.Degraded("홈 디렉토리가 구성되지 않았습니다."));
                }

                if (!Directory.Exists(homeDir))
                {
                    return Task.FromResult(
                        HealthCheckResult.Unhealthy($"홈 디렉토리가 존재하지 않습니다: {homeDir}"));
                }

                // 쓰기 권한 테스트
                var testFile = Path.Combine(homeDir, $"_healthcheck_{Guid.NewGuid()}.tmp");
                try
                {
                    File.WriteAllText(testFile, "health check");
                    File.Delete(testFile);
                }
                catch (Exception ex)
                {
                    return Task.FromResult(
                        HealthCheckResult.Unhealthy(
                            "홈 디렉토리에 쓰기 권한이 없습니다.", 
                            ex));
                }

                return Task.FromResult(
                    HealthCheckResult.Healthy("파일 시스템 상태 정상"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(
                    HealthCheckResult.Unhealthy(
                        "파일 시스템 점검 중 오류 발생", 
                        ex));
            }
        }
    }
}
