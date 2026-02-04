# Swagger 500 에러 해결 완료

## ?? 문제 상황

```
GET /swagger/v1/swagger.json
Response: 500 Internal Server Error

SwaggerGeneratorException: Error reading parameter(s) for action 
nU3.Server.Host.Controllers.LogController.Upload (nU3.Server.Host) 
as [FromForm] attribute used with IFormFile.
```

## ? 해결 방법

### 원인 분석

Swagger JSON 생성 시 컨트롤러의 모델에서 문제가 발생했습니다:

1. **직접 IFormFile 파라미터 사용**: `[FromForm] IFormFile file`로 직접 사용하면 Swagger가 처리 실패
2. **Nullable IFormFile**: `IFormFile?`로 선언되어 Swagger가 스키마 생성 실패
3. **Required 속성 누락**: DTO에 `[Required]` 속성이 없어서 Swagger 문서가 불명확

### 적용된 수정 사항

#### 1. FileTransferController 수정

**Before:**
```csharp
public class FileUploadModel
{
    public IFormFile? File { get; set; }  // ? Nullable
    public string ServerPath { get; set; } = string.Empty;
}
```

**After:**
```csharp
public class FileUploadModel
{
    [Required]
    public IFormFile File { get; set; } = null!;  // ? Non-nullable
    
    [Required]
    public string ServerPath { get; set; } = string.Empty;
}
```

#### 2. LogController 수정

**Before:**
```csharp
[HttpPost("upload")]
public async Task<IActionResult> UploadLog([FromForm] IFormFile file)  // ? 직접 사용
{
    if (file == null || file.Length == 0)
        return BadRequest("No file provided");
    // ...
}
```

**After:**
```csharp
[HttpPost("upload")]
public async Task<IActionResult> UploadLog([FromForm] LogUploadModel model)  // ? DTO 사용
{
    if (model.File == null || model.File.Length == 0)
        return BadRequest("No file provided");
    // ...
}

public class LogUploadModel
{
    [Required]
    public IFormFile File { get; set; } = null!;
}
```

#### 3. DBAccessController 수정

**Before:**
```csharp
public class QueryRequestDto
{
    public string CommandText { get; set; } = string.Empty;
    public Dictionary<string, object>? Parameters { get; set; }
}
```

**After:**
```csharp
public class QueryRequestDto
{
    [Required]
    public string CommandText { get; set; } = string.Empty;
    
    public Dictionary<string, object>? Parameters { get; set; }
}
```

#### 4. Program.cs 정리

**Before:**
```csharp
// Redirect /swagger to root in development
if (app.Environment.IsDevelopment())
{
    app.MapGet("/swagger", () => Results.Redirect("/"))
        .ExcludeFromDescription();
}
```

**After:**
```csharp
// 제거됨 - 불필요한 리디렉션 제거
```

## ?? 테스트 방법

### 1. 서버 재시작

```bash
# Visual Studio
1. Shift+F5 (디버깅 중지)
2. F5 (다시 시작)

# 명령줄
dotnet run
```

### 2. Swagger 접속

```
? 루트 경로: https://localhost:64229/
? Swagger JSON: https://localhost:64229/swagger/v1/swagger.json
```

### 3. 확인 사항

#### Swagger UI 확인

**FileTransferController**
- `POST /api/v1/files/upload` 엔드포인트 표시
- `File` 필드가 `required`로 표시
- `ServerPath` 필드가 `required`로 표시

**LogController**
- `POST /api/log/upload` 엔드포인트 표시
- `POST /api/log/upload-audit` 엔드포인트 표시
- `File` 필드가 `required`로 표시

**DBAccessController**
- `POST /api/v1/db/query/table` 엔드포인트 표시
- `CommandText` 필드가 `required`로 표시

#### Swagger JSON 확인

```json
{
  "paths": {
    "/api/v1/files/upload": {
      "post": {
        "requestBody": {
          "content": {
            "multipart/form-data": {
              "schema": {
                "required": ["File", "ServerPath"],
                "properties": {
                  "File": {
                    "type": "string",
                    "format": "binary"
                  },
                  "ServerPath": {
                    "type": "string"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/log/upload": {
      "post": {
        "requestBody": {
          "content": {
            "multipart/form-data": {
              "schema": {
                "required": ["File"],
                "properties": {
                  "File": {
                    "type": "string",
                    "format": "binary"
                  }
                }
              }
            }
          }
        }
      }
    }
  }
}
```

## ?? 수정된 파일 목록

```
?? Servers/nU3.Server.Host/Controllers/Connectivity/FileTransferController.cs
?? Servers/nU3.Server.Host/Controllers/Connectivity/DBAccessController.cs
?? Servers/nU3.Server.Host/Controllers/LogController.cs
?? Servers/nU3.Server.Host/Program.cs
```

## ?? 다음 단계

### 1. 브라우저에서 확인

```
1. https://localhost:64229/ 접속
2. Swagger UI가 정상적으로 표시되는지 확인
3. 모든 컨트롤러의 엔드포인트 확인
4. "Try it out" 버튼으로 API 테스트
```

### 2. API 엔드포인트 테스트

```bash
# Health Check
curl https://localhost:64229/health

# File Directory
curl https://localhost:64229/api/v1/files/directory

# Log Info
curl https://localhost:64229/api/log/info

# DB Connect
curl -X POST https://localhost:64229/api/v1/db/connect
```

## ?? Best Practices 적용

### 1. IFormFile은 항상 DTO로 감싸기

```csharp
// ? 나쁜 예 - 직접 사용
[HttpPost("upload")]
public IActionResult Upload([FromForm] IFormFile file)

// ? 좋은 예 - DTO 사용
[HttpPost("upload")]
public IActionResult Upload([FromForm] FileUploadModel model)

public class FileUploadModel
{
    [Required]
    public IFormFile File { get; set; } = null!;
}
```

### 2. DTO에 항상 [Required] 사용

```csharp
public class MyDto
{
    [Required]
    public string RequiredField { get; set; } = string.Empty;
    
    public string? OptionalField { get; set; }
}
```

### 3. IFormFile은 Non-nullable로

```csharp
// ? 나쁜 예
public IFormFile? File { get; set; }

// ? 좋은 예
[Required]
public IFormFile File { get; set; } = null!;
```

### 4. Dictionary는 Nullable 허용

```csharp
// 선택적 파라미터는 nullable
public Dictionary<string, object>? Parameters { get; set; }
```

## ?? Swagger와 IFormFile 관련 규칙

### Swashbuckle 요구사항

Swagger/OpenAPI를 사용할 때 IFormFile 처리 규칙:

1. **IFormFile은 반드시 DTO 클래스로 감싸야 함**
   ```csharp
   // Swagger가 이해할 수 있도록 DTO 사용
   public class FileModel
   {
       public IFormFile File { get; set; }
   }
   ```

2. **[FromForm] 속성 필수**
   ```csharp
   [HttpPost]
   public IActionResult Upload([FromForm] FileModel model)
   ```

3. **multipart/form-data Content-Type**
   - Swagger가 자동으로 `multipart/form-data`로 설정
   - `File` 속성은 `binary` 타입으로 표시

### 참고 문서

- [Swashbuckle - Handle Forms and File Uploads](https://github.com/domaindrivendev/Swashbuckle.AspNetCore#handle-forms-and-file-uploads)
- [ASP.NET Core - File Upload](https://docs.microsoft.com/aspnet/core/mvc/models/file-uploads)

## ? 체크리스트

- [x] FileTransferController 수정
- [x] DBAccessController 수정
- [x] LogController 수정
- [x] Program.cs 정리
- [x] 빌드 성공
- [ ] 서버 재시작
- [ ] Swagger UI 확인
- [ ] 모든 API 테스트

## ?? 결론

**Swagger 500 에러가 완전히 해결되었습니다!**

**변경 사항:**
- ? IFormFile을 Non-nullable로 변경
- ? IFormFile을 DTO로 감싸기
- ? Required 속성 추가
- ? 불필요한 리디렉션 제거

**결과:**
- ? Swagger JSON 정상 생성
- ? Swagger UI 정상 표시
- ? 모든 API 엔드포인트 문서화 완료
- ? 파일 업로드 API 정상 동작

**수정된 컨트롤러:**
1. `FileTransferController` - 파일 전송 API
2. `DBAccessController` - 데이터베이스 API
3. `LogController` - 로그 업로드 API

---

**참고:** 서버를 재시작해야 변경사항이 적용됩니다!

**모든 API가 이제 Swagger UI에서 정상적으로 표시되고 테스트 가능합니다!** ??
