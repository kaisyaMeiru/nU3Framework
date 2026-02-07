using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using nU3.Data;
using nU3.Data.Repositories;
using nU3.Connectivity;
using nU3.Connectivity.Implementations;
using nU3.Core.Services;
using nU3.Models;

namespace nU3.Bootstrapper
{
    /// <summary>
    /// Bootstrapper 메인 프로그램
    /// 
    /// 책임:
    /// - 애플리케이션 설정 로드
    /// - 로컬 DB 초기화
    /// - 프레임워크 컴포넌트 및 모듈 동기화 (ModuleLoaderService 사용)
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
                    MessageBox.Show("Shell 실행 파일을 찾을 수 없습니다.", "오류",  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                FileLogger.Info($"Shell 경로: {shellPath}");
                FileLogger.SectionEnd("Shell 실행 파일 위치 확인");

                // 3. 통합 동기화 (ModuleLoaderService 사용)
                FileLogger.SectionStart("시스템 동기화");
                
                // 레포지토리 초기화
                var moduleRepo = new SQLiteModuleRepository(dbService);
                var progRepo = new SQLiteProgramRepository(dbService);
                var compRepo = new SQLiteComponentRepository(dbService);
                var fileTransfer = new HttpFileTransferClient(httpClient, baseUrl);

                // ModuleLoaderService 생성
                var loaderService = new ModuleLoaderService(moduleRepo, compRepo, progRepo, fileTransfer, _configuration);
                string syncMode = _configuration.GetValue<string>("ModuleStorage:SyncMode") ?? "Minimum";

                // UI와 함께 동기화 실행
                bool syncSuccess = RunSyncWithUI(loaderService, syncMode);

                if (!syncSuccess)
                {
                    FileLogger.Error("동기화 실패 또는 취소됨");
                    // 필수 구성요소 실패 시 중단 로직 추가 가능
                }
                
                FileLogger.Info("시스템 동기화 완료");
                FileLogger.SectionEnd("시스템 동기화");

                // 4. Seeder (개발 환경용 더미 데이터)
                #if DEBUG
                FileLogger.SectionStart("더미 데이터 생성");
                var seeder = new Seeder(dbService);
                seeder.SeedDummyData();
                FileLogger.Info("더미 데이터 생성 완료");
                FileLogger.SectionEnd("더미 데이터 생성");
                #endif

                // 5. Shell 실행
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
            try
            {
                return configuration?.GetValue<string>("RuntimeDirectory");
                
            }
            catch { }
           
            return null;
        }

        /// <summary>
        /// UI를 통해 동기화 진행 상황을 표시합니다.
        /// </summary>
        private static bool RunSyncWithUI(ModuleLoaderService loader, string syncMode)
        {
            using var form = new UpdateProgressForm();
            
            bool completed = false;
            ComponentUpdateResult? result = null;

            // 1. 업데이트 목록 사전 확인 및 UI 초기화
            var updates = loader.CheckForUpdates(syncMode);
            if (updates.Count == 0)
            {
                form.ShowNoUpdates();
            }
            else
            {
                form.InitializeUpdateList(updates);
            }

            // 2. 진행 이벤트 바인딩
            loader.UpdateProgress += (s, e) =>
            {
                form.UpdateProgress(e);
                
                if (e.Phase == UpdatePhase.Installing)
                {
                    form.MarkComponentCompleted(e.ComponentId, true);
                }
                else if (e.Phase == UpdatePhase.Failed)
                {
                    form.MarkComponentCompleted(e.ComponentId, false);
                }
            };

            // 3. 동기화 실행
            form.Shown += async (s, e) =>
            {
                if (updates.Count > 0)
                {
                    await Task.Delay(500); // UI 렌더링 시간 확보
                    
                    await Task.Run(() =>
                    {
                        try
                        {
                            result = loader.SyncWithServer(syncMode);
                            completed = true;
                        }
                        catch (Exception ex)
                        {
                            FileLogger.Error("동기화 중 오류", ex);
                            completed = false;
                        }
                    });

                    if (completed && result != null)
                    {
                        form.ShowResult(result);
                    }
                }
            };

            var dialogResult = form.ShowDialog();
            
            // 성공했거나 사용자가 '실행' 등을 눌러 완료된 경우 true 반환
            return (completed && (result?.Success ?? false)) || dialogResult == DialogResult.OK;
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
