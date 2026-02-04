using nU3.Connectivity;
using nU3.Server.Connectivity.Services;
using nU3.Server.Host.HealthChecks;
using System.Data.Common;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Security.Cryptography.X509Certificates;
using System.Linq;

// ============================================================================
// nU3 Server Host - 향상된 ASP.NET Core Web API 진입점
// ============================================================================
// 이 파일은 애플리케이션의 호스트 구성을 수행합니다. 아래 작업을 수행합니다:
// 1) WebApplication 빌더를 생성하고 설정을 로드
// 2) JSON 직렬화, Swagger, CORS, 압축, 레이트리미팅, 헬스체크 등 공통 미들웨어/서비스 구성
// 3) 애플리케이션 서비스(파일 전송, DB 액세스 등) DI 등록
// 4) 미들웨어 파이프라인을 구성하고 컨트롤러 및 헬스체크 엔드포인트를 매핑
// 5) 애플리케이션을 실행
//
// 운영 전 반드시 확인해야 할 항목:
// - ConnectionStrings 및 Database:Provider 설정
// - HTTPS 설정 및 인증서
// - CORS 허용 도메인
// - 파일 저장소 경로 및 운영 권한
// - 로그 저장/수집 정책
// ============================================================================
var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// HTTPS 사용 여부 판정
// - 설정 키 Https:Enabled 또는 Https를 확인합니다. 둘 다 없으면 기본 false.
// - 운영 환경에서는 true로 설정하고 인증서를 구성하세요.
// ============================================================================
var httpsEnabled = builder.Configuration.GetValue<bool?>("Https:Enabled")
    ?? builder.Configuration.GetValue<bool?>("Https")
    ?? false;

// ============================================================================
// 명령행 인자 처리 유틸리티
// - 예: --ip 0.0.0.0 --port 5000 --scheme http
// - 필요 시 프로세스 시작 시 외부 바인딩 주소를 지정할 수 있음
// - 주의: reverse proxy를 사용하는 경우 직접 바인딩을 사용하지 않는 것이 일반적입니다.
// ============================================================================
static string? GetArgValue(string[] args, string name)
{
    var index = Array.FindIndex(args, a => string.Equals(a, name, StringComparison.OrdinalIgnoreCase));
    if (index >= 0 && index < args.Length - 1)
    {
        return args[index + 1];
    }

    return null;
}

// ============================================================================
// 명령행 인자에서 IP/PORT/SCHEME 추출 및 적용
// - appsettings의 "urls" 값이 비어있을 때만 명령행 인자 기반 설정을 적용
// - 예외/충돌 방지를 위해 배포 환경에서는 Kestrel/Proxy 설정을 권장
// ============================================================================
var urlsFromConfig = builder.Configuration["urls"];
var ipArg = GetArgValue(args, "--ip");
var portArg = GetArgValue(args, "--port");
var schemeArg = GetArgValue(args, "--scheme");

if (string.IsNullOrWhiteSpace(urlsFromConfig)
    && (!string.IsNullOrWhiteSpace(ipArg) || !string.IsNullOrWhiteSpace(portArg) || !string.IsNullOrWhiteSpace(schemeArg)))
{
    var ip = string.IsNullOrWhiteSpace(ipArg) ? "0.0.0.0" : ipArg;
    var port = int.TryParse(portArg, out var parsedPort) ? parsedPort : (httpsEnabled ? 5001 : 5000);
    var scheme = string.IsNullOrWhiteSpace(schemeArg) ? (httpsEnabled ? "https" : "http") : schemeArg;

    // Kestrel이 바인딩할 URL을 명시적으로 설정
    builder.WebHost.UseUrls($"{scheme}://{ip}:{port}");
}

// ============================================================================
// Development helper: configure Kestrel to avoid HTTPS handshake failure when dev certificate is not available.
// - If no explicit URLs are configured and running in Development, bind localhost:64229.
// - If a localhost certificate with a private key exists in CurrentUser\My, use it for HTTPS; otherwise fall back to HTTP.
// This avoids the System.ComponentModel.Win32Exception (0x8009030D) when Schannel cannot access credentials.
// ============================================================================
if (builder.Environment.IsDevelopment())
{
    // only configure if no URLs are set by config or environment
    var configuredUrls = builder.Configuration["urls"] ?? Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
    if (string.IsNullOrWhiteSpace(configuredUrls))
    {
        builder.WebHost.ConfigureKestrel((context, options) =>
        {
            const int devPort = 64229;
            X509Certificate2? cert = null;

            try
            {
                using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);
                var certs = store.Certificates
                    .Find(X509FindType.FindBySubjectName, "localhost", validOnly: false)
                    .OfType<X509Certificate2>();

                cert = certs.FirstOrDefault(c => c.HasPrivateKey);
            }
            catch
            {
                // ignore and fallback to HTTP
                cert = null;
            }

            if (cert != null)
            {
                options.ListenLocalhost(devPort, listen => listen.UseHttps(cert));
            }
            else
            {
                // fallback to HTTP to avoid TLS handshake failure during development
                options.ListenLocalhost(devPort);
            }
        });
    }
}

// ============================================================================
// 1. JSON 직렬화 옵션 구성
// 설명:
// - PropertyNamingPolicy = null : PascalCase 유지 (기본 System.Text.Json은 camelCase로 변환)
// - DefaultIgnoreCondition = WhenWritingNull : null 속성은 출력에서 제외
// - NumberHandling = AllowReadingFromString : 숫자가 문자열로 전달되어도 파싱 허용
// - WriteIndented : 개발 환경이면 들여쓰기(가독성) 적용
// 주의:
// - 클라이언트와의 계약(케이스, null 정책 등)을 문서화하고 일관되게 유지하세요.
// ============================================================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 속성 이름을 유지(PascalCase)
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        
        // 응답에서 null 값은 생략(불필요한 필드 제거)
        options.JsonSerializerOptions.DefaultIgnoreCondition = 
            JsonIgnoreCondition.WhenWritingNull;
        
        // 문자열로 표현된 숫자도 숫자로 읽기 허용
        options.JsonSerializerOptions.NumberHandling = 
            JsonNumberHandling.AllowReadingFromString;
        
        // 개발 환경에서는 가독성을 위해 예쁘게 출력
        options.JsonSerializerOptions.WriteIndented = 
            builder.Environment.IsDevelopment();
    });

// ============================================================================
// 2. Swagger/OpenAPI 설정
// 설명:
// - Swagger는 API 문서화를 위해 사용됩니다. 개발/테스트에서 편리하지만 운영에서는 노출 제한 권장.
// - XML 주석을 포함하면 컨트롤러/모델의 주석이 문서에 반영됩니다.
// 설정 키:
// - Swagger:Title, Swagger:Version, Swagger:Description
// - Swagger:IncludeXmlComments (bool)
// - Swagger:EnableAnnotations (bool)
// ============================================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var swaggerTitle = builder.Configuration.GetValue<string>("Swagger:Title", "nU3 Server API");
    var swaggerVersion = builder.Configuration.GetValue<string>("Swagger:Version", "v1");
    var swaggerDescription = builder.Configuration.GetValue<string>("Swagger:Description", 
        "Backend API for nU3 Framework - Database Access, File Transfer, and Log Upload Services");
    
    options.SwaggerDoc("v1", new()
    {
        Title = swaggerTitle,
        Version = swaggerVersion,
        Description = swaggerDescription,
        Contact = new()
        {
            Name = "nU3 개발팀",
            Email = "dev@nu3.com"
        }
    });

    // XML 주석 파일이 존재하면 Swagger에 포함
    var includeXmlComments = builder.Configuration.GetValue<bool>("Swagger:IncludeXmlComments", true);
    if (includeXmlComments)
    {
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    }
    
    // Swagger 어노테이션 활성화(컨트롤러/메서드에 주석 기반 어노테이션 사용 시)
    var enableAnnotations = builder.Configuration.GetValue<bool>("Swagger:EnableAnnotations", true);
    if (enableAnnotations)
    {
        options.EnableAnnotations();
    }
});

// ============================================================================
// 3. CORS 구성
// 설명:
// - 외부 도메인에서 API를 호출할 때 허용할 출처(Origins)를 설정합니다.
// - 운영에서는 구체적 도메인만 허용하고 와일드카드는 최소화하세요.
// 설정 키: Cors:AllowedOrigins (배열)
// ============================================================================
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
    ?? new[] { "http://localhost:*", "https://localhost:*" };

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ============================================================================
// 4. 응답 압축 (Response Compression)
// 설명: Gzip 및 Brotli 압축을 사용하여 네트워크 대역폭을 절약합니다.
// 주의: 압축 레벨/성능 트레이드오프를 고려하세요.
// ============================================================================
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

// ============================================================================
// 5. 요청 제한 (Rate Limiting)
// 설명: 전역 고정창(Fixed Window) 정책을 사용하여 요청 빈도를 제한합니다.
// 설정 키: RateLimiting:Enabled, RateLimiting:PermitLimit, RateLimiting:WindowMinutes
// 권장: 인증 사용자별/엔드포인트별 세부 정책 구성
// ============================================================================
var rateLimitingEnabled = builder.Configuration.GetValue<bool>("RateLimiting:Enabled", true);
var permitLimit = builder.Configuration.GetValue<int>("RateLimiting:PermitLimit", 1000);
var windowMinutes = builder.Configuration.GetValue<int>("RateLimiting:WindowMinutes", 1);

if (rateLimitingEnabled)
{
    builder.Services.AddRateLimiter(options =>
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            // 식별자: 인증된 사용자 이름 또는 원격 IP
            var identifier = context.User.Identity?.Name 
                ?? context.Connection.RemoteIpAddress?.ToString() 
                ?? "anonymous";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: identifier,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = permitLimit,
                    Window = TimeSpan.FromMinutes(windowMinutes)
                });
        });

        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    });
}

// ============================================================================
// 6. 헬스 체크 등록
// 설명: 서비스의 상태(파일 시스템, DB 등)를 점검하기 위한 Health Check를 등록
// - FileSystemHealthCheck: 서버 저장소(홈 디렉토리) 접근성/쓰기 권한 점검
// - DatabaseHealthCheck: DB 연결 상태 점검
// ============================================================================
builder.Services.AddHealthChecks()
    .AddCheck<FileSystemHealthCheck>(
        "filesystem",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "ready" })
    .AddCheck<DatabaseHealthCheck>(
        "database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "ready" });

// ============================================================================
// 7. 애플리케이션 서비스 등록 (DI)
// 설명: 애플리케이션에서 사용할 주요 서비스들을 DI 컨테이너에 등록합니다.
// ============================================================================

// 7.1 파일 전송 서비스 등록 (싱글톤)
// - ServerFileTransferService는 파일 시스템 접근을 담당하며 상태를 거의 가지지 않으므로 싱글톤으로 등록
// - 설정키: ServerSettings:FileTransfer:HomeDirectory (홈 디렉토리 경로)
builder.Services.AddSingleton<ServerFileTransferService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<ServerFileTransferService>>();
    var configuration = sp.GetRequiredService<IConfiguration>();
    var service = new ServerFileTransferService(logger, configuration);
    var homeDir = builder.Configuration.GetValue<string>("ServerSettings:FileTransfer:HomeDirectory")
        ?? @"C:\nU3_Server_Storage";
    
    // 홈 디렉토리 설정 적용
    service.SetHomeDirectory(true, homeDir);
    return service;
});

// 7.2 데이터베이스 접근 서비스 등록 (스코프)
// - 요청당 인스턴스를 생성하여 트랜잭션 안전성 보장
// - Database:Provider 설정에 따라 Oracle 또는 MariaDB(MySQL)용 연결 팩토리를 사용
builder.Services.AddScoped<ServerDBAccessService>(sp =>
{
    // Provider를 먼저 읽어서 어떤 connection string key를 사용할지 결정
    var provider = builder.Configuration.GetValue<string>("Database:Provider", "Oracle");

    // connection string key 선택: provider에 맞춘 키 우선, 없으면 DefaultConnection 사용
    var connKey = provider switch
    {
        var p when string.Equals(p, "Sqlite", StringComparison.OrdinalIgnoreCase) || string.Equals(p, "SQLite", StringComparison.OrdinalIgnoreCase) => "SqliteConnection",
        var p when string.Equals(p, "MariaDB", StringComparison.OrdinalIgnoreCase) || string.Equals(p, "MySql", StringComparison.OrdinalIgnoreCase) => "MariaDbConnection",
        _ => "DefaultConnection"
    };

    var connStr = builder.Configuration.GetConnectionString(connKey)
        ?? builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=localhost;User Id=user;Password=pass;";

    // 선택된 프로바이더에 따라 적절한 DbConnection 팩토리를 생성
    Func<DbConnection> connFactory;
    try
    {
        if (string.Equals(provider, "Oracle", StringComparison.OrdinalIgnoreCase))
        {
            // DbConnectionFactories는 런타임 반사로 공급자 어셈블리에서 Connection 타입을 찾음
            connFactory = DbConnectionFactories.CreateOracleFactory(connStr);
        }
        else if (string.Equals(provider, "MariaDB", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(provider, "MySql", StringComparison.OrdinalIgnoreCase))
        {
            connFactory = DbConnectionFactories.CreateMariaDbFactory(connStr);
        }
        else if (string.Equals(provider, "Sqlite", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(provider, "SQLite", StringComparison.OrdinalIgnoreCase))
        {
            // SQLite 지원 추가: Microsoft.Data.Sqlite 또는 System.Data.SQLite
            connFactory = DbConnectionFactories.CreateSqliteFactory(connStr);
        }
        else
        {
            throw new NotImplementedException($"지원되지 않는 DB 공급자입니다: {provider}. 설정에서 'Database:Provider'를 'Oracle', 'MariaDB' 또는 'Sqlite'로 설정하세요.");
        }
    }
    catch (Exception ex)
    {
        // 팩토리 생성 오류는 명확히 보고하여 구성 문제를 알림
        throw new InvalidOperationException($"DB 연결 팩토리 생성 실패: {ex.Message}", ex);
    }

    return new ServerDBAccessService(connStr, connFactory);
});

// 헬스 체크가 종속하는 서비스 등록
builder.Services.AddScoped<DatabaseHealthCheck>();
builder.Services.AddScoped<FileSystemHealthCheck>();

// ============================================================================
// 8. 로깅 구성
// 설명: 기본 콘솔/디버그 로그를 사용. 프로덕션 환경이면 EventLog 추가.
// 권장: 중앙 집중형 로깅(Seq, ELK, Application Insights 등) 연동
// ============================================================================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

if (builder.Environment.IsProduction())
{
    builder.Logging.AddEventLog();
}

// ============================================================================
// 9. 애플리케이션 빌드
// ============================================================================
var app = builder.Build();

// ============================================================================
// 10. 미들웨어 파이프라인 구성
// 설명: 요청 처리 파이프라인을 설정합니다. 순서가 중요함.
// - 예: 인증은 라우팅/권한 검사 이전에 등록되어야 합니다.
// ============================================================================

// 개발 환경 전용 미들웨어: 예외 페이지, Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "nU3 Server API v1");
        options.RoutePrefix = "swagger"; // Swagger UI는 /swagger에서 접근
        options.DocumentTitle = "nU3 Server API Documentation";
        options.DisplayRequestDuration();
    });
}
else
{
    // 운영 환경에서는 Swagger 노출을 설정으로 제어
    var swaggerEnabled = builder.Configuration.GetValue<bool>("Swagger:Enabled", false);
    if (swaggerEnabled)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "nU3 Server API v1");
            options.RoutePrefix = builder.Configuration.GetValue<string>("Swagger:RoutePrefix", "swagger");
            options.DocumentTitle = builder.Configuration.GetValue<string>("Swagger:Title", "nU3 Server API");
        });
    }
    
    // 전역 예외 처리 및 보안 헤더(HSTS)
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// HTTPS 리다이렉션 (설정에 따라 활성화)
if (httpsEnabled)
{
    app.UseHttpsRedirection();
}

// 응답 압축 적용
app.UseResponseCompression();

// CORS 미들웨어
app.UseCors();

// 인증 및 권한 부여 미들웨어
app.UseAuthentication();
app.UseAuthorization();

// 레이트 리미팅 적용
//if (rateLimitingEnabled)
//{
//    app.UseRateLimiter();
//}

// 요청 로깅 미들웨어: 요청/응답 정보를 로깅(간단한 퍼포먼스 측정)
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var startTime = DateTime.UtcNow;
    
    logger.LogInformation(
        "Request: {Method} {Path} from {IP}",
        context.Request.Method,
        context.Request.Path,
        context.Connection.RemoteIpAddress);
    
    await next();
    
    var elapsed = DateTime.UtcNow - startTime;
    logger.LogInformation(
        "Response: {StatusCode} in {ElapsedMs}ms",
        context.Response.StatusCode,
        elapsed.TotalMilliseconds);
});

// ============================================================================
// 11. 엔드포인트 매핑
// ============================================================================

// 컨트롤러 라우팅 등록
app.MapControllers();

// 헬스 체크 엔드포인트 정의
// /health: 전체 상태(JSON)
app.MapHealthChecks("/health", new()
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            duration = report.TotalDuration,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration,
                exception = e.Value.Exception?.Message
            })
        });
        await context.Response.WriteAsync(result);
    }
});

// /health/ready: 준비 상태만 반환(태그 'ready'가 지정된 체크)
app.MapHealthChecks("/health/ready", new()
{
    Predicate = check => check.Tags.Contains("ready")
});

// /health/live: 단순 실행 확인용(추가 체크 없음)
app.MapHealthChecks("/health/live", new()
{
    Predicate = _ => false // No checks, just responds if server is running
});

// 오류 처리 엔드포인트
app.MapGet("/error", () => Results.Problem("An error occurred"))
    .ExcludeFromDescription();

// ============================================================================
// 12. 애플리케이션 시작 정보 로그
// ============================================================================
app.Logger.LogInformation("=================================================");
app.Logger.LogInformation("nU3 Server Host Starting...");
app.Logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
app.Logger.LogInformation("HTTPS: {Https}", httpsEnabled ? "Enabled" : "Disabled");
app.Logger.LogInformation("Rate Limiting: {RateLimit}", rateLimitingEnabled ? "Enabled" : "Disabled");
app.Logger.LogInformation("=================================================");

// ============================================================================
// 13. 애플리케이션 실행
// ============================================================================
app.Run();

// ============================================================================
// Application Entry Point (for testing)
// ============================================================================
public partial class Program { }