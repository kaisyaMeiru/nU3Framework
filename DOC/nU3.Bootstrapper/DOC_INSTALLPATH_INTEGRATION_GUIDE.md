# Framework 컴포넌트 자동 업데이트 통합 가이드

## BaseInstallPath 개념 정리

### 경로 계산 공식

```
최종 설치 경로 = BaseInstallPath + InstallPath + FileName

예시:
  BaseInstallPath: "C:\Program Files\nU3.Shell\"
  InstallPath: "plugins"
  FileName: "MyPlugin.dll"
  
  → C:\Program Files\nU3.Shell\plugins\MyPlugin.dll
```

### BaseInstallPath 결정 방법

```csharp
// 방법 1: 자동 (기본값 - 실행 파일 위치)
var updateService = new ComponentUpdateService(componentRepo);
// BaseInstallPath = AppDomain.CurrentDomain.BaseDirectory
// 예: C:\Program Files\nU3.Shell\

// 방법 2: 명시적 지정
var updateService = new ComponentUpdateService(
    componentRepo, 
    installBasePath: @"C:\MyApp\"
);
```

### 환경별 BaseInstallPath

| 환경 | BaseInstallPath | 설명 |
|------|----------------|------|
| **Production** | `C:\Program Files\nU3.Shell\` | 일반 설치 |
| **Development** | `D:\Projects\nU3.Framework\bin\Debug\` | 개발 중 |
| **Portable** | `D:\MyApps\nU3\` | USB 등 포터블 설치 |
| **Test** | `C:\Temp\TestEnv\` | 테스트 환경 |

---

## Bootstrapper 통합

### 시나리오: 앱 시작 전 자동 업데이트

```csharp
// nU3.Bootstrapper\Program.cs
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using nU3.Core.Repositories;
using nU3.Core.Services;
using nU3.Data;
using nU3.Data.Repositories;

namespace nU3.Bootstrapper
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // 1. DI 설정
                var services = new ServiceCollection();
                services.AddSingleton<LocalDatabaseManager>();
                services.AddScoped<IComponentRepository, SQLiteComponentRepository>();
                var provider = services.BuildServiceProvider();

                // 2. 컴포넌트 업데이트 서비스 생성
                var componentRepo = provider.GetRequiredService<IComponentRepository>();
                
                // 실행 파일이 있는 디렉토리를 기준으로 설정
                // 예: C:\Program Files\nU3.Shell\nU3.Bootstrapper.exe
                //  → BaseInstallPath = C:\Program Files\nU3.Shell\
                var baseInstallPath = AppDomain.CurrentDomain.BaseDirectory;
                
                var updateService = new ComponentUpdateService(componentRepo, baseInstallPath);

                // 3. 업데이트 확인 및 실행
                var updateTask = CheckAndUpdateComponents(updateService);
                updateTask.Wait();

                if (!updateTask.Result)
                {
                    MessageBox.Show(
                        "컴포넌트 업데이트에 실패했습니다.\n프로그램을 종료합니다.",
                        "업데이트 실패",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // 4. 메인 Shell 실행
                var shellPath = Path.Combine(baseInstallPath, "nU3.Shell.exe");
                if (File.Exists(shellPath))
                {
                    System.Diagnostics.Process.Start(shellPath);
                }
                else
                {
                    MessageBox.Show("nU3.Shell.exe를 찾을 수 없습니다.", "오류");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"부트스트랩 오류: {ex.Message}", "오류");
            }
        }

        private static async Task<bool> CheckAndUpdateComponents(ComponentUpdateService updateService)
        {
            try
            {
                // 필수 컴포넌트 확인
                var missing = updateService.GetMissingComponents();
                var updates = updateService.CheckForUpdates();

                if (!missing.Any() && !updates.Any())
                {
                    Console.WriteLine("모든 컴포넌트가 최신입니다.");
                    return true;
                }

                // 진행률 표시 폼
                using var progressForm = new BootstrapProgressForm();
                progressForm.Show();

                var progress = new Progress<ComponentUpdateProgressEventArgs>(p =>
                {
                    progressForm.UpdateProgress(
                        p.CurrentComponentName ?? p.CurrentComponentId,
                        p.PercentComplete);
                });

                // 업데이트 실행
                var result = await updateService.UpdateAllAsync(progress);

                if (result.Success)
                {
                    progressForm.UpdateProgress("업데이트 완료", 100);
                    await Task.Delay(1000);  // 1초 대기
                    return true;
                }
                else
                {
                    MessageBox.Show(
                        $"{result.Message}\n\n실패 목록:\n" +
                        string.Join("\n", result.FailedComponents.Select(f => $"- {f.ComponentId}: {f.Error}")),
                        "업데이트 실패",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"업데이트 확인 중 오류: {ex.Message}", "오류");
                return false;
            }
        }
    }

    /// <summary>
    /// 간단한 진행률 표시 폼
    /// </summary>
    public class BootstrapProgressForm : Form
    {
        private readonly Label _lblStatus;
        private readonly ProgressBar _progressBar;

        public BootstrapProgressForm()
        {
            this.Text = "nU3 Framework - 업데이트 중";
            this.Size = new System.Drawing.Size(400, 120);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;

            _lblStatus = new Label
            {
                Text = "컴포넌트 확인 중...",
                Left = 20,
                Top = 20,
                Width = 350
            };

            _progressBar = new ProgressBar
            {
                Left = 20,
                Top = 50,
                Width = 350,
                Height = 25
            };

            this.Controls.Add(_lblStatus);
            this.Controls.Add(_progressBar);
        }

        public void UpdateProgress(string status, int percent)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string, int>(UpdateProgress), status, percent);
                return;
            }

            _lblStatus.Text = status;
            _progressBar.Value = Math.Min(100, Math.Max(0, percent));
        }
    }
}
```

---

## Shell에서 선택적 업데이트

### 시나리오: 시작 시 백그라운드 체크

```csharp
// nU3.Shell\MainShellForm.cs
private async void MainShellForm_Load(object sender, EventArgs e)
{
    // 백그라운드에서 업데이트 확인
    _ = Task.Run(async () =>
    {
        await Task.Delay(5000);  // 5초 후 확인
        
        var componentRepo = Program.ServiceProvider.GetRequiredService<IComponentRepository>();
        var updateService = new ComponentUpdateService(componentRepo);
        
        var updates = updateService.CheckForUpdates();
        
        if (updates.Any())
        {
            this.Invoke((MethodInvoker)delegate
            {
                toolStripStatusUpdate.Text = $"업데이트 {updates.Count}개 가능";
                toolStripStatusUpdate.ForeColor = Color.Orange;
                toolStripStatusUpdate.IsLink = true;
                toolStripStatusUpdate.Click += (s, e) => ShowUpdateDialog();
            });
        }
    });
}

private async void ShowUpdateDialog()
{
    var componentRepo = Program.ServiceProvider.GetRequiredService<IComponentRepository>();
    var updateService = new ComponentUpdateService(componentRepo);
    
    var updates = updateService.CheckForUpdates();
    
    var msg = "다음 컴포넌트를 업데이트할 수 있습니다:\n\n" +
              string.Join("\n", updates.Select(u => $"- {u.ComponentName} ({u.Version})")) +
              "\n\n지금 업데이트하시겠습니까?\n(업데이트 후 재시작이 필요합니다)";
    
    if (MessageBox.Show(msg, "업데이트 확인", MessageBoxButtons.YesNo) != DialogResult.Yes)
        return;
    
    try
    {
        var result = await AsyncOperationHelper.ExecuteWithProgressAsync(
            this,
            "컴포넌트 업데이트 중...",
            async (ct, progress) =>
            {
                var updateProgress = new Progress<ComponentUpdateProgressEventArgs>(p =>
                {
                    progress.Report(new BatchOperationProgress
                    {
                        TotalItems = p.TotalComponents,
                        CompletedItems = p.CurrentIndex,
                        CurrentItem = p.CurrentComponentName,
                        PercentComplete = p.PercentComplete
                    });
                });
                
                return await updateService.UpdateAllAsync(updateProgress, ct);
            });
        
        if (result.Success)
        {
            MessageBox.Show(
                "업데이트가 완료되었습니다.\n프로그램을 재시작해주세요.",
                "완료",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            Application.Exit();
        }
    }
    catch (OperationCanceledException)
    {
        MessageBox.Show("업데이트가 취소되었습니다.", "취소");
    }
}
```

---

## 경로 설정 체크리스트

### Deployer에서 배포 시

```
? 올바른 설정:
  InstallPath: ""                     → 루트
  InstallPath: "plugins"              → plugins\
  InstallPath: "resources\images"     → resources\images\

? 잘못된 설정:
  InstallPath: "C:\Program Files\"    → 절대 경로 X
  InstallPath: "..\OtherApp\"         → 상위 경로 X
  InstallPath: "/plugins"             → 슬래시(/) 대신 백슬래시(\) 사용
```

### 클라이언트에서 설치 시

```csharp
// ComponentUpdateService가 자동으로 처리
var installPath = GetInstallPath(component);
// "C:\Program Files\nU3.Shell\" + "plugins" + "MyPlugin.dll"
// = "C:\Program Files\nU3.Shell\plugins\MyPlugin.dll"

// 폴더 자동 생성
var directory = Path.GetDirectoryName(installPath);
if (!Directory.Exists(directory))
    Directory.CreateDirectory(directory);
    
// 파일 복사
File.Copy(cacheFile, installPath, overwrite: true);
```

---

## 실제 사용 흐름

### 1. Deployer에서 배포

```
관리자 작업:
┌─────────────────────────────────────────┐
│ [Smart Deploy] 클릭                     │
│   ↓                                     │
│ nU3.Core.dll 선택                       │
│   ↓                                     │
│ 자동 분석:                              │
│   ComponentId: "nU3.Core"               │
│   ComponentType: FrameworkCore          │
│   InstallPath: ""  ← 자동 설정 (루트)   │
│   Priority: 10                          │
│   ↓                                     │
│ DB 저장 + 서버 복사                     │
│   SYS_COMPONENT_MST                     │
│   SYS_COMPONENT_VER                     │
│   D:\ServerStorage\Components\...       │
└─────────────────────────────────────────┘
```

### 2. Bootstrapper에서 업데이트

```
클라이언트 시작:
┌─────────────────────────────────────────┐
│ Bootstrapper.exe 실행                   │
│   ↓                                     │
│ DB에서 활성 버전 조회                   │
│   ↓                                     │
│ 로컬 설치 확인:                         │
│   BaseInstallPath = C:\Program Files\   │
│                     nU3.Shell\          │
│   ↓                                     │
│ 경로 계산:                              │
│   "" + "nU3.Core.dll"                   │
│   = C:\...\nU3.Shell\nU3.Core.dll       │
│   ↓                                     │
│ 비교:                                   │
│   서버 버전: 1.0.1.0                    │
│   로컬 버전: 1.0.0.0                    │
│   → 업데이트 필요!                      │
│   ↓                                     │
│ 다운로드:                               │
│   서버 → 캐시                           │
│   캐시 → 설치 경로                      │
│   ↓                                     │
│ Shell 실행                              │
└─────────────────────────────────────────┘
```

---

## 실전 코드 예시

### Bootstrapper의 Main 함수

```csharp
[STAThread]
static void Main()
{
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);

    // 현재 실행 경로를 BaseInstallPath로 사용
    var baseInstallPath = AppDomain.CurrentDomain.BaseDirectory;
    Console.WriteLine($"BaseInstallPath: {baseInstallPath}");
    
    // 예: C:\Program Files\nU3.Shell\
    //     D:\Projects\nU3.Framework\bin\Debug\

    // DI 설정
    var services = new ServiceCollection();
    services.AddSingleton<LocalDatabaseManager>();
    services.AddScoped<IComponentRepository, SQLiteComponentRepository>();
    var provider = services.BuildServiceProvider();

    var componentRepo = provider.GetRequiredService<IComponentRepository>();
    var updateService = new ComponentUpdateService(componentRepo, baseInstallPath);

    // 업데이트 실행
    var updateResult = ExecuteUpdates(updateService);
    
    if (updateResult)
    {
        // Shell 실행
        LaunchShell(baseInstallPath);
    }
}

private static bool ExecuteUpdates(ComponentUpdateService updateService)
{
    // 필수 컴포넌트 확인
    var missing = updateService.GetMissingComponents();
    if (missing.Any())
    {
        Console.WriteLine($"필수 컴포넌트 {missing.Count}개 누락!");
        // 강제 다운로드
    }

    // 업데이트 확인
    var updates = updateService.CheckForUpdates();
    if (!updates.Any())
    {
        Console.WriteLine("모든 컴포넌트 최신 버전");
        return true;
    }

    Console.WriteLine($"{updates.Count}개 업데이트 시작...");
    
    using var progressForm = new BootstrapProgressForm();
    progressForm.Show();

    var progress = new Progress<ComponentUpdateProgressEventArgs>(p =>
    {
        var status = $"[{p.Phase}] {p.CurrentComponentName ?? p.CurrentComponentId}";
        progressForm.UpdateProgress(status, p.PercentComplete);
        Console.WriteLine($"{status} ({p.PercentComplete}%)");
    });

    var task = updateService.UpdateAllAsync(progress);
    task.Wait();

    var result = task.Result;
    progressForm.Close();

    if (result.Success)
    {
        Console.WriteLine("? 모든 업데이트 완료");
        return true;
    }
    else
    {
        Console.WriteLine($"? 업데이트 실패: {result.Message}");
        foreach (var (id, error) in result.FailedComponents)
        {
            Console.WriteLine($"  - {id}: {error}");
        }
        return false;
    }
}

private static void LaunchShell(string baseInstallPath)
{
    var shellPath = Path.Combine(baseInstallPath, "nU3.Shell.exe");
    
    if (!File.Exists(shellPath))
    {
        MessageBox.Show($"Shell을 찾을 수 없습니다:\n{shellPath}", "오류");
        return;
    }

    var startInfo = new System.Diagnostics.ProcessStartInfo
    {
        FileName = shellPath,
        WorkingDirectory = baseInstallPath,
        UseShellExecute = true
    };

    System.Diagnostics.Process.Start(startInfo);
}
```

---

## 경로 검증

### 설치 전 검증

```csharp
public bool ValidateInstallPath(ComponentMstDto component)
{
    // 1. 절대 경로 체크
    if (Path.IsPathRooted(component.InstallPath))
    {
        throw new ArgumentException("InstallPath must be relative path");
    }

    // 2. 상위 경로 체크
    if (component.InstallPath?.Contains("..") == true)
    {
        throw new ArgumentException("InstallPath cannot contain '..'");
    }

    // 3. 최종 경로 생성 가능 여부
    try
    {
        var fullPath = GetInstallPath(component);
        var directory = Path.GetDirectoryName(fullPath);
        return !string.IsNullOrEmpty(directory);
    }
    catch
    {
        return false;
    }
}
```

---

## 요약

| 항목 | 설명 | 예시 |
|------|------|------|
| **BaseInstallPath** | 실행 파일 위치 (자동) | `C:\Program Files\nU3.Shell\` |
| **InstallPath** | 상대 경로 (DB 설정) | `""`, `plugins`, `resources\images` |
| **FileName** | 파일명 (DB 설정) | `nU3.Core.dll`, `MyPlugin.dll` |
| **최종 경로** | Base + Install + File | `C:\...\nU3.Shell\plugins\MyPlugin.dll` |

**핵심 원칙:**
- ? InstallPath는 **상대 경로**만 사용
- ? BaseInstallPath는 **자동 결정** (실행 위치)
- ? 빈값(`""`) = 루트 디렉토리
- ? 폴더는 자동 생성됨
