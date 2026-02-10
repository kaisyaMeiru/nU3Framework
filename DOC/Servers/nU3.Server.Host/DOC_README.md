# nU3.Server.Host - Enhanced Web API Server

## ?? Overview

nU3.Server.Host는 nU3 Framework의 Backend API 서버입니다. WinForms 클라이언트(nU3.Shell)에 파일 전송 및 데이터베이스 액세스 서비스를 제공합니다.

## ?? Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                  nU3 Server Architecture                     │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Client Layer                                               │
│  ┌──────────────────────────────────────────┐              │
│  │ nU3.Shell (WinForms Client)              │              │
│  └──────────────────┬───────────────────────┘              │
│                     │ HTTP REST API                         │
│                     ↓                                       │
│  API Layer                                                  │
│  ┌──────────────────────────────────────────┐              │
│  │ Controllers                               │              │
│  │  ├─ FileTransferController                │              │
│  │  ├─ DBAccessController                    │              │
│  │  └─ LogController                         │              │
│  └──────────────────┬───────────────────────┘              │
│                     │                                       │
│                     ↓                                       │
│  Service Layer                                              │
│  ┌──────────────────────────────────────────┐              │
│  │ ServerFileTransferService (Singleton)    │              │
│  │ ServerDBAccessService (Scoped)           │              │
│  └──────────────────┬───────────────────────┘              │
│                     │                                       │
│                     ↓                                       │
│  Data Layer                                                 │
│  ┌──────────────────────────────────────────┐              │
│  │ File System | Oracle Database            │              │
│  └──────────────────────────────────────────┘              │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## ? Features

### ?? Core Features
- ? **RESTful API**: Clean, standard HTTP API endpoints
- ? **File Transfer**: Upload/Download with large file support
- ? **Database Access**: Oracle DB integration
- ? **Log Management**: Client log collection

### ?? Enhanced Features (v2.0)
- ? **Response Compression**: Gzip & Brotli support
- ? **Rate Limiting**: Configurable request throttling
- ? **Health Checks**: `/health`, `/health/ready`, `/health/live`
- ? **CORS**: Cross-origin request support
- ? **Structured Logging**: Request/Response logging
- ? **Error Handling**: Global exception handling
- ? **Swagger UI**: Interactive API documentation

## ??? Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Framework | ASP.NET Core | 8.0 |
| Language | C# | 12.0 |
| API Docs | Swagger/OpenAPI | Latest |
| Compression | Gzip/Brotli | Built-in |
| Health Checks | ASP.NET Core | 8.0 |

## ?? Project Structure

```
nU3.Server.Host/
├── Controllers/
│   ├── Connectivity/
│   │   ├── FileTransferController.cs    # File operations API
│   │   └── DBAccessController.cs        # Database operations API
│   └── LogController.cs                 # Log collection API
├── HealthChecks/
│   ├── DatabaseHealthCheck.cs           # DB health monitoring
│   └── FileSystemHealthCheck.cs         # File system monitoring
├── Program.cs                           # Application entry point
├── appsettings.json                     # Configuration
└── appsettings.Development.json         # Dev configuration
```

## ?? API Endpoints

### File Transfer API (`/api/v1/files`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/directory` | Get home directory path |
| POST | `/directory/config` | Configure home directory |
| GET | `/list` | List files in directory |
| GET | `/subdirectories` | List subdirectories |
| POST | `/directory/create` | Create directory |
| DELETE | `/directory` | Delete directory |
| POST | `/upload` | Upload file |
| GET | `/download` | Download file |
| GET | `/exists` | Check file exists |
| DELETE | `/` | Delete file |
| GET | `/size` | Get file size |

### Database API (`/api/v1/db`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/connect` | Connect to database |
| POST | `/transaction/begin` | Begin transaction |
| POST | `/transaction/commit` | Commit transaction |
| POST | `/transaction/rollback` | Rollback transaction |
| POST | `/query/table` | Execute query (DataTable) |
| POST | `/query/nonquery` | Execute non-query |
| POST | `/query/scalar` | Execute scalar query |
| POST | `/procedure` | Execute stored procedure |

### Log API (`/api/log`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/upload` | Upload client log file |

### Health Check API

| Endpoint | Description |
|----------|-------------|
| `/health` | Overall health status |
| `/health/ready` | Readiness probe |
| `/health/live` | Liveness probe |

## ?? Getting Started

### Prerequisites

```bash
# .NET 8 SDK
dotnet --version  # Should be 8.0.x or higher

# Oracle Client (for production)
# Install Oracle.ManagedDataAccess.Core
```

### Configuration

#### 1. **appsettings.json**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost:1521/ORCL;User Id=user;Password=pass;"
  },
  
  "ServerSettings": {
    "FileTransfer": {
      "HomeDirectory": "C:\\nU3_Server_Storage",
      "MaxUploadSizeMB": 100
    }
  },
  
  "Cors": {
    "AllowedOrigins": [ "http://localhost:*" ]
  },
  
  "RateLimiting": {
    "Enabled": true,
    "PermitLimit": 100,
    "WindowMinutes": 1
  }
}
```

#### 2. **Environment Variables**

```bash
# Connection String
CONNECTIONSTRINGS__DEFAULTCONNECTION="Data Source=...;User Id=...;Password=..."

# File Storage
SERVERSETTINGS__FILETRANSFER__HOMEDIRECTORY="C:\Storage"

# ASPNETCORE Environment
ASPNETCORE_ENVIRONMENT=Development
```

### Running the Server

#### Development Mode

```bash
# Restore packages
dotnet restore

# Run
dotnet run

# Or with watch (auto-reload)
dotnet watch run
```

Server will start at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000` (root path)

#### Production Mode

```bash
# Build
dotnet build -c Release

# Publish
dotnet publish -c Release -o ./publish

# Run
cd publish
dotnet nU3.Server.Host.dll
```

### Docker Deployment

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["nU3.Server.Host.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "nU3.Server.Host.dll"]
```

```bash
# Build image
docker build -t nu3-server:latest .

# Run container
docker run -d -p 5000:80 -p 5001:443 \
  -e CONNECTIONSTRINGS__DEFAULTCONNECTION="..." \
  -v /data/storage:/app/storage \
  nu3-server:latest
```

## ?? Configuration Details

### Rate Limiting

```json
{
  "RateLimiting": {
    "Enabled": true,
    "PermitLimit": 100,      // Requests per window
    "WindowMinutes": 1       // Time window in minutes
  }
}
```

**Behavior:**
- 100 requests per minute per client IP
- Returns HTTP 429 when limit exceeded
- Auto-replenishment after window expires

### Response Compression

Automatic compression for:
- JSON responses
- Large file transfers
- Text content

**Supported formats:**
- Gzip (default)
- Brotli (modern browsers)

### Health Checks

#### `/health` - Detailed Health Status

```json
{
  "status": "Healthy",
  "timestamp": "2024-01-27T10:30:00Z",
  "duration": "00:00:00.1234567",
  "checks": [
    {
      "name": "filesystem",
      "status": "Healthy",
      "description": "File system is healthy",
      "duration": "00:00:00.0123456"
    },
    {
      "name": "database",
      "status": "Healthy",
      "description": "Database connection is healthy",
      "duration": "00:00:00.0567890"
    }
  ]
}
```

#### `/health/ready` - Kubernetes Readiness Probe

Returns 200 if server is ready to accept requests.

#### `/health/live` - Kubernetes Liveness Probe

Returns 200 if server is running (no actual checks).

## ?? Monitoring & Logging

### Log Levels

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Log Output

- **Console**: All environments
- **Debug**: Development only
- **Event Log**: Production (Windows)

### Log Format

```
Request: GET /api/v1/files/list from 192.168.1.100
Response: 200 in 45.67ms
```

## ?? Security Considerations

### Current Implementation

? **HTTPS**: Enabled by default  
? **CORS**: Configurable origins  
? **Rate Limiting**: DDoS protection  
?? **Authentication**: Not implemented (add if needed)  
?? **Authorization**: Not implemented (add if needed)

### Recommended Enhancements

```csharp
// 1. JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* config */ });

// 2. Role-based Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

// 3. API Key Authentication
builder.Services.AddAuthentication("ApiKey")
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", options => { });
```

## ?? Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Test specific controller
dotnet test --filter "FullyQualifiedName~FileTransferController"
```

See `nU3.Server.Tests` project for test examples.

## ?? Performance Tuning

### Response Compression

```csharp
// Adjust compression level
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal; // or Fastest
});
```

### Connection Pooling

```csharp
// Configure DB connection pooling
builder.Services.AddDbContext<MyContext>(options =>
{
    options.UseOracle(connStr, o =>
    {
        o.MaxPoolSize(100);
        o.MinPoolSize(10);
    });
});
```

### Caching

```csharp
// Add response caching
builder.Services.AddResponseCaching();

// In controller
[ResponseCache(Duration = 60)]
public IActionResult GetData() { }
```

## ?? Troubleshooting

### Issue: Oracle Provider Not Found

**Solution:**
```bash
dotnet add package Oracle.ManagedDataAccess.Core
```

Update Program.cs:
```csharp
DbConnection CreateConnection() 
{
    return new OracleConnection(connStr);
}
```

### Issue: Health Check Fails

**Check:**
1. File system permissions on home directory
2. Database connection string
3. Network connectivity

**View logs:**
```bash
# Development
dotnet run --verbosity detailed

# Production
# Check Event Viewer (Windows) or log files
```

### Issue: Rate Limit Too Restrictive

**Adjust settings:**
```json
{
  "RateLimiting": {
    "PermitLimit": 1000,  // Increase limit
    "WindowMinutes": 5    // Larger window
  }
}
```

## ?? Migration from v1.0

No breaking changes. New features are opt-in via configuration.

**To enable new features:**
1. Update appsettings.json with new sections
2. Restart server
3. Test with `/health` endpoint

## ?? Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Swagger/OpenAPI](https://swagger.io/)
- [Health Checks in .NET](https://docs.microsoft.com/dotnet/architecture/microservices/implement-resilient-applications/monitor-app-health)

## ?? Contributing

See testing guide in `nU3.Server.Tests/TESTING_GUIDE.md`

## ?? License

Internal use only - nU3 Framework

---

**Last Updated:** 2024-01-27  
**Version:** 2.0  
**Status:** Production Ready ?
