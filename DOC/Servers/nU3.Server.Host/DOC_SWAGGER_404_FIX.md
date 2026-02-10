# Swagger 404 에러 해결 가이드

## ?? 문제 상황

```
Request: GET /swagger from ::1
Response: 404 in 10.5025ms
```

## ? 해결 방법

### 방법 1: 루트 URL로 접속 (권장)

Swagger UI가 루트 경로에 설정되어 있습니다:

```
? https://localhost:64229/swagger  (404 에러)
? https://localhost:64229/          (Swagger UI)
? http://localhost:64228/           (Swagger UI)
```

### 방법 2: 서버 재시작

디버깅 중이라면:

1. **Visual Studio**: 디버깅 중지 (Shift+F5)
2. **재시작**: F5 또는 Ctrl+F5
3. **자동으로 브라우저 열림**: `http://localhost:64228` 또는 `https://localhost:64229`

### 방법 3: Hot Reload 시도

코드 변경 사항을 적용하려면:

1. Visual Studio에서 "Hot Reload" 버튼 클릭
2. 또는 브라우저 새로고침 (Ctrl+F5)

## ?? 접속 가능한 URL

### Swagger UI
```
? http://localhost:64228/
? https://localhost:64229/
? http://localhost:64228/swagger (리디렉션 추가됨)
```

### API 엔드포인트 테스트
```
? http://localhost:64228/health
? http://localhost:64228/api/v1/files/directory
? http://localhost:64228/api/v1/db/connect
```

### Swagger JSON
```
? http://localhost:64228/swagger/v1/swagger.json
```

## ?? 현재 설정 확인

Program.cs의 Swagger 설정:

```csharp
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "nU3 Server API v1");
    options.RoutePrefix = string.Empty; // ← 루트 경로에 Swagger UI
});
```

**`RoutePrefix = string.Empty`** 의미:
- Swagger UI가 `/` (루트)에 위치
- `/swagger`가 아닌 `/`로 접속해야 함

## ??? 문제가 계속되면

### 1. 환경 변수 확인

```powershell
# 현재 환경 확인
$env:ASPNETCORE_ENVIRONMENT

# Development로 설정
$env:ASPNETCORE_ENVIRONMENT = "Development"
```

### 2. 로그 확인

시작 로그를 확인:
```
=================================================
nU3 Server Host Starting...
Environment: Development
HTTPS: True
Rate Limiting: Enabled
=================================================
```

**Environment가 Production이면** Swagger가 비활성화됩니다!

### 3. Program.cs 수정 (선택사항)

모든 환경에서 Swagger 활성화 (개발 전용):

```csharp
// Before
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(...);
}

// After (모든 환경에서 활성화)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "nU3 Server API v1");
    options.RoutePrefix = string.Empty;
});
```

## ?? 체크리스트

- [ ] 서버 재시작
- [ ] `http://localhost:64228/` (루트)로 접속
- [ ] 환경이 Development인지 확인
- [ ] 브라우저 캐시 클리어 (Ctrl+Shift+Del)
- [ ] 다른 브라우저로 시도

## ?? 빠른 해결

```bash
# 1. 디버깅 중지
# 2. 다시 실행
dotnet run

# 3. 브라우저 열기
start http://localhost:64228/
```

## ?? 추가 지원

여전히 문제가 있다면 다음을 확인:

1. **포트 충돌**: 다른 애플리케이션이 64228/64229 포트 사용 중
2. **방화벽**: Windows 방화벽이 연결 차단
3. **IIS/IIS Express**: IIS가 실행 중이면 충돌 가능

---

**요약:** `/swagger` 대신 `/` (루트)로 접속하거나, 서버를 재시작하세요! ?
