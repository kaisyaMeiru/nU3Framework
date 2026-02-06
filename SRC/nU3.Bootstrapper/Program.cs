using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using nU3.Data;
using nU3.Connectivity;
using nU3.Connectivity.Implementations;

namespace nU3.Bootstrapper
{
    /// <summary>
    /// Bootstrapper 메인 프로그램
    /// 
    /// 책임:
    /// - 애플리케이션 설정 로드
    /// - 로컬 DB 초기화
    /// - 프레임워크 컴포넌트 업데이트 확인 및 패치 (UI 제공)
    /// - 화면 모듈 검사 및 동기화 (HTTP 전용)
    /// - 메인 쉘(MainShell) 실행
    /// </summary>
    class Program
    {
        private static IConfiguration? _configuration;

        [STAThread]
        static async Task Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FileLogger.Info("=== nU3 Framework Bootstrapper 시작 ===");
            FileLogger.Info($"실행 시간: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            FileLogger.Info($"베이스 디렉토리: {AppDomain.CurrentDomain.BaseDirectory}");

            try
            {
                // 0. Configuration 로드
                FileLogger.SectionStart("시스템 설정 로드");
                _configuration = LoadConfiguration();
                FileLogger.Info("시스템 설정 로드 완료");
                FileLogger.Debug($"appsettings.json 경로: {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json")}");
                FileLogger.SectionEnd("시스템 설정 로드");

                // DB 서비스 초기화 (HTTP 클라이언트)
                string baseUrl = _configuration.GetValue<string>("ServerConnection:BaseUrl") ?? "http://localhost:5000";
                
                // HttpClient 설정
                var httpClient = new System.Net.Http.HttpClient 
                { 
                    BaseAddress = new Uri(baseUrl),
                    Timeout = TimeSpan.FromMinutes(10)
                };
                
                IDBAccessService dbService = new HttpDBAccessClient(httpClient, baseUrl);
                
                // 1. DB 연결 확인 (초기화는 서버에서 수행됨)
                FileLogger.SectionStart("데이터베이스 연결 확인");
                bool isConnected = dbService.Connect();
                if (isConnected)
                {
                    FileLogger.Info("데이터베이스 연결 성공");
                }
                else
                {
                    FileLogger.Warning("데이터베이스 연결 실패. 서버 상태를 확인하세요.");
                }
                FileLogger.SectionEnd("데이터베이스 연결 확인");



                // 2. Shell 실행 파일 위치 확인
                FileLogger.SectionStart("Shell 실행 파일 위치 확인");
                string? shellPath = FindShellPath(_configuration);
                if (string.IsNullOrEmpty(shellPath))
                {
                    FileLogger.Warning("Shell 실행 파일을 찾을 수 없습니다.");
                    MessageBox.Show("시작위치를 확인 할수 없습니다.", "오류",  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                FileLogger.Info($"Shell 경로: {shellPath}");
                FileLogger.SectionEnd("Shell 실행 파일 위치 확인");



                // 3. 프레임워크 컴포넌트 업데이트 (서버 또는 캐시에서 다운로드)
                FileLogger.SectionStart("프레임워크 컴포넌트 업데이트 확인");
                using var componentLoader = new ComponentLoader(dbService, _configuration, shellPath);
                FileLogger.Info($"설치 경로: {componentLoader.InstallPath}");
                var componentUpdates = componentLoader.CheckForUpdates();

                if (componentUpdates.Any())
                {
                    FileLogger.Info($"{componentUpdates.Count}개의 업데이트가 필요합니다.");
                    foreach (var update in componentUpdates)
                    {
                        FileLogger.Info($"  - {update.ComponentName} ({update.ComponentId}) - {update.UpdateType} - 버전: {update.ServerVersion} - 크기: {update.FileSize:N0} bytes");
                    }
                    
                    // UI를 통해 업데이트 진행
                    var result = RunUpdateWithUI(componentLoader, componentUpdates);
                    
                    if (!result)
                    {
                        // 필수 컴포넌트가 설치되지 않았을 경우 실행 중단
                        var missing = componentLoader.GetMissingRequiredComponents();
                        if (missing.Any())
                        {
                            FileLogger.Error("필수 컴포넌트가 설치되지 않아 프로그램을 실행할 수 없습니다.");
                            foreach (var m in missing)
                            {
                                FileLogger.Warning($"  필수 컴포넌트 미설치: {m.ComponentName} ({m.ComponentId})");
                            }
                            MessageBox.Show(
                                "필수 컴포넌트가 설치되지 않아 프로그램을 실행할 수 없습니다.\n\n" +
                                string.Join("\n", missing.Select(m => $"- {m.ComponentName}")),
                                "업데이트 필요",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }
                else
                {
                    FileLogger.Info("모든 컴포넌트가 최신 상태입니다.");
                }
                FileLogger.SectionEnd("프레임워크 컴포넌트 업데이트 확인");

                // 4. 화면 모듈 검사 및 동기화 (HTTP 전용)
                FileLogger.SectionStart("화면 모듈 검사 및 동기화");
                var moduleLoader = new ModuleLoader(dbService, _configuration);
                moduleLoader.CheckAndLoadModules(shellPath);
                FileLogger.Info("화면 모듈 검사 완료");
                FileLogger.SectionEnd("화면 모듈 검사 및 동기화");

                // 5. Seeder (개발 환경용 더미 데이터)
                #if DEBUG
                FileLogger.SectionStart("더미 데이터 생성");
                var seeder = new Seeder(dbService);
                seeder.SeedDummyData();
                FileLogger.Info("더미 데이터 생성 완료");
                FileLogger.SectionEnd("더미 데이터 생성");
                #endif

                // 6. Shell 실행
            FileLogger.Info("MainShell 실행 중...");
            FileLogger.Info($"실행 파일: {shellPath}");
            LaunchShell(shellPath);
            
            FileLogger.Info("=== nU3 Framework Bootstrapper 완료 ===");
            }
            catch (Exception ex)
            {
                FileLogger.Error("치명적 오류 발생", ex);
                FileLogger.Error($"예외 타입: {ex.GetType().FullName}");
                MessageBox.Show($"작업 중 오류 발생:\n{ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 애플리케이션 설정 로드
        /// </summary>
        private static IConfiguration LoadConfiguration()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var configPath = Path.Combine(basePath, "appsettings.json");
            
            var builder = new ConfigurationBuilder().SetBasePath(basePath);

            // 단일 파일 배포의 경우 파일이 존재하지 않을 수 있음
            if (File.Exists(configPath))
            {
                builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
                FileLogger.Info("설정 파일 로드: appsettings.json");
            }
            else
            {
                // 리소스에서 로드 시도
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith("appsettings.json"));

                if (resourceName != null)
                {
                    using var stream = assembly.GetManifestResourceStream(resourceName);
                    if (stream != null)
                    {
                        using var reader = new StreamReader(stream);
                        var jsonContent = reader.ReadToEnd();
                        
                        // 임시 파일로 쓰고 로드
                        var tempConfigPath = Path.Combine(Path.GetTempPath(), $"appsettings_{Guid.NewGuid()}.json");
                        File.WriteAllText(tempConfigPath, jsonContent);
                        builder.AddJsonFile(tempConfigPath, optional: false, reloadOnChange: false);
                        
                        FileLogger.Info("설정 리소스 로드: appsettings.json (임시 파일)");
                        FileLogger.Debug($"임시 경로: {tempConfigPath}");
                    }
                    else
                    {
                        FileLogger.Warning("리소스에서 appsettings.json을 로드할 수 없습니다. 기본 설정 사용.");
                    }
                }
                else
                {
                    FileLogger.Warning("appsettings.json 파일을 찾을 수 없습니다. 기본 설정 사용.");
                }
            }

            return builder.Build();
        }

        /// <summary>
        /// MainShell 또는 Shell 실행 파일을 찾습니다.
        /// </summary>
        private static string? FindShellPath(IConfiguration configuration)
        {
            // 1) appsettings의 RuntimeDirectory를 우선 사용하여 쉘 실행 파일을 찾습니다.

            try
            {
                var runtimeDir = configuration?.GetValue<string>("RuntimeDirectory");
                if (!string.IsNullOrWhiteSpace(runtimeDir))
                {
                    if(System.IO.Directory.Exists(runtimeDir))
                        return runtimeDir;
                }
            }
            catch { /* 무시하고 다음 탐색으로 진행 */ }
           
            return null;
        }

        /// <summary>
        /// UI를 통해 업데이트 진행 상황을 표시하고 결과를 반환합니다.
        /// </summary>
        private static bool RunUpdateWithUI(ComponentLoader loader, System.Collections.Generic.List<ComponentUpdateInfo> updates)
        {
            using var form = new UpdateProgressForm();
            
            bool completed = false;
            ComponentUpdateResult? result = null;

            // 업데이트 목록 초기화
            form.InitializeUpdateList(updates);

            // 진행 이벤트 바인딩
            loader.UpdateProgress += (s, e) =>
            {
                form.UpdateProgress(e);
                
                if (e.Phase == UpdatePhase.Installing)
                {
                    // 설치가 시작되면 완료로 표시
                    form.MarkComponentCompleted(e.ComponentId, true);
                }
                else if (e.Phase == UpdatePhase.Failed)
                {
                    form.MarkComponentCompleted(e.ComponentId, false);
                }
            };

            // 폼이 표시되면 백그라운드에서 업데이트 시작
            form.Shown += async (s, e) =>
            {
                await Task.Delay(500);  // UI가 렌더링될 시간 확보
                
                await Task.Run(() =>
                {
                    result = loader.UpdateAll();
                });

                completed = true;
                form.ShowResult(result!);
            };

            var dialogResult = form.ShowDialog();
            
            return completed && (result?.Success ?? false) || dialogResult == DialogResult.OK;
        }

        /// <summary>
        /// MainShell 프로세스를 실행합니다.
        /// </summary>
        private static void LaunchShell(string shellPath)
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = shellPath,
                WorkingDirectory = Path.GetDirectoryName(shellPath),
                UseShellExecute = true
            };

            System.Diagnostics.Process.Start(startInfo);
        }
    }
}
