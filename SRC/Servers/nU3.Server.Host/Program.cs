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
var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// HTTPS 사용 여부 판정
// ============================================================================
var httpsEnabled = builder.Configuration.GetValue<bool?>("Https:Enabled")
    ?? builder.Configuration.GetValue<bool?>("Https")
    ?? false;

// ============================================================================
// 명령행 인자 처리 유틸리티
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

    builder.WebHost.UseUrls($"{scheme}://{ip}:{port}");
}

// ============================================================================
// Development helper: configure Kestrel
// ============================================================================
if (builder.Environment.IsDevelopment())
{
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
                cert = null;
            }

            if (cert != null)
            {
                options.ListenLocalhost(devPort, listen => listen.UseHttps(cert));
            }
            else
            {
                options.ListenLocalhost(devPort);
            }
        });
    }
}

// ============================================================================
// 1. JSON 직렬화 옵션 구성
// ============================================================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        options.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
    });

// ============================================================================
// 2. Swagger/OpenAPI 설정
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
            Name = "nU3 Framework",
            Email = "shcho@cef.or.kr"
        }
    });

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
    
    var enableAnnotations = builder.Configuration.GetValue<bool>("Swagger:EnableAnnotations", true);
    if (enableAnnotations)
    {
        options.EnableAnnotations();
    }
});

// ============================================================================
// 3. CORS 구성
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
// ============================================================================

// 7.1 파일 전송 서비스 등록 (싱글톤)
builder.Services.AddSingleton<ServerFileTransferService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<ServerFileTransferService>>();
    var configuration = sp.GetRequiredService<IConfiguration>();
    var service = new ServerFileTransferService(logger, configuration);
    var homeDir = configuration.GetValue<string>("ServerSettings:FileTransfer:HomeDirectory") ?? @"C:\nU3_Server_Storage";
    
    service.SetHomeDirectory(true, homeDir);
    return service;
});

// 7.2 데이터베이스 접근 서비스 등록 (스코프)
builder.Services.AddScoped<ServerDBAccessService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    
    // 신규 설정 경로 적용: ServerSettings:Database:Provider
    var provider = config.GetValue<string>("ServerSettings:Database:Provider", "Oracle");

    // 신규 설정 경로 적용: ServerSettings:Database:Connections:{Provider}
    var connStr = config.GetValue<string>($"ServerSettings:Database:Connections:{provider}")
        ?? "Data Source=localhost;User Id=user;Password=pass;";

    // SQLite 경로 동적 처리
    if (string.Equals(provider, "Sqlite", StringComparison.OrdinalIgnoreCase) || 
        string.Equals(provider, "SQLite", StringComparison.OrdinalIgnoreCase))
    {
        var dbDir = config.GetValue<string>("ServerSettings:Database:DbDirectory") ?? "Server_Database";
        var builder = new System.Data.Common.DbConnectionStringBuilder { ConnectionString = connStr };
        
        if (builder.TryGetValue("Data Source", out var pathObj) && pathObj is string dbPath)
        {
            if (!Path.IsPathRooted(dbPath))
            {
                var basePath = Path.IsPathRooted(dbDir) 
                    ? dbDir 
                    : Path.Combine(AppContext.BaseDirectory, dbDir);
                    
                dbPath = Path.GetFullPath(Path.Combine(basePath, dbPath));
                builder["Data Source"] = dbPath;
                connStr = builder.ConnectionString;
            }
        }
    }

    Func<DbConnection> connFactory;
    try
    {
        if (string.Equals(provider, "Oracle", StringComparison.OrdinalIgnoreCase))
        {
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
            connFactory = DbConnectionFactories.CreateSqliteFactory(connStr);
        }
        else
        {
            throw new NotImplementedException($"지원되지 않는 DB 공급자입니다: {provider}.");
        }
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"DB 연결 팩토리 생성 실패: {ex.Message}", ex);
    }

    return new ServerDBAccessService(connStr, connFactory);
});

// IDBAccessService 인터페이스 등록 (ServerDBAccessService 재사용)
builder.Services.AddScoped<IDBAccessService>(sp => sp.GetRequiredService<ServerDBAccessService>());

// SQLite 스키마 초기화 서비스 등록
builder.Services.AddScoped<SqliteSchemaService>();

// 헬스 체크가 종속하는 서비스 등록
builder.Services.AddScoped<DatabaseHealthCheck>();
builder.Services.AddScoped<FileSystemHealthCheck>();

// ============================================================================
// 8. 로깅 구성
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
// ============================================================================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "nU3 Server API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "nU3 Server API Documentation";
        options.DisplayRequestDuration();
    });
}
else
{
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
    
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

if (httpsEnabled)
{
    app.UseHttpsRedirection();
}

app.UseResponseCompression();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

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
app.MapControllers();

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

app.MapHealthChecks("/health/ready", new()
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new()
{
    Predicate = _ => false 
});

app.MapGet("/error", () => Results.Problem("An error occurred"))
    .ExcludeFromDescription();

// ============================================================================
// 12. 애플리케이션 시작 정보 로그 및 초기화
// ============================================================================
app.Logger.LogInformation("=================================================");
app.Logger.LogInformation("nU3 Server Host Starting...");
app.Logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
app.Logger.LogInformation("HTTPS: {Https}", httpsEnabled ? "Enabled" : "Disabled");
app.Logger.LogInformation("Rate Limiting: {RateLimit}", rateLimitingEnabled ? "Enabled" : "Disabled");
app.Logger.LogInformation("=================================================");

// SQLite 스키마 초기화 실행
using (var scope = app.Services.CreateScope())
{
    try
    {
        var schemaService = scope.ServiceProvider.GetRequiredService<SqliteSchemaService>();
        schemaService.Initialize();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Failed to initialize SQLite schema.");
    }
}

// ============================================================================
// 13. 애플리케이션 실행
// ============================================================================
app.Run();

public partial class Program { }