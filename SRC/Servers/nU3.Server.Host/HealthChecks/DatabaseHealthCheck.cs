using Microsoft.Extensions.Diagnostics.HealthChecks;
using nU3.Server.Connectivity.Services;

namespace nU3.Server.Host.HealthChecks
{
    /// <summary>
    /// 데이터베이스 연결 상태를 확인하는 Health Check
    /// </summary>
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly ServerDBAccessService _dbService;

        public DatabaseHealthCheck(ServerDBAccessService dbService)
        {
            _dbService = dbService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var connected = await _dbService.ConnectAsync();
                
                if (connected)
                {
                    return HealthCheckResult.Healthy("데이터베이스 연결 정상");
                }
                
                return HealthCheckResult.Unhealthy("데이터베이스 연결 실패");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "데이터베이스 연결 점검 중 오류 발생", 
                    ex);
            }
        }
    }
}
