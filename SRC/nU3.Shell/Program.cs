using System;
using System.IO.Pipes;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using nU3.Core.Repositories;
using nU3.Data;
using nU3.Data.Repositories;
using nU3.Connectivity;
using nU3.Connectivity.Implementations;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using nU3.Core.Interfaces;

namespace nU3.Shell
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SkinManager.EnableFormSkins();
            UserLookAndFeel.Default.SetSkinStyle("Office 2019 Black");

            // ---------------------------------------------------------
            // 단일 인스턴스 확인 및 URI 처리
            // ---------------------------------------------------------
            // nU3.Shell의 다른 인스턴스가 실행 중인지 확인
            var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            var processes = System.Diagnostics.Process.GetProcessesByName(currentProcess.ProcessName);

            if (processes.Length > 1)
            {
                // URI 인자가 존재하면, Named Pipe를 통해 실행 중인 인스턴스로 전달
                if (args.Length > 0 && args[0].StartsWith("nu3://", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        using (var client = new NamedPipeClientStream(".", "nU3_Shell_Pipe", PipeDirection.Out))
                        {
                            client.Connect(1000); // 1초 대기
                            using (var writer = new System.IO.StreamWriter(client))
                            {
                                writer.Write($"URI|{args[0]}");
                                writer.Flush();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // 연결 실패. 다른 인스턴스가 응답하지 않거나 아직 수신 대기 상태가 아닐 수 있음.
                        // 메시지를 표시하거나 종료할 수 있으나, 중복 실행 방지를 위해 종료함.
                    }
                }
                
                // 현재 인스턴스 종료
                return;
            }

            // ---------------------------------------------------------
            // ConnectivityManager 팩토리 구성 (순환 의존성 해결)
            // ---------------------------------------------------------
            nU3.Core.Services.ConnectivityManager.DBClientFactory = (client, url) => new HttpDBAccessClient(client, url);
            nU3.Core.Services.ConnectivityManager.FileClientFactory = (client, url) => new HttpFileTransferClient(client, url);
            nU3.Core.Services.ConnectivityManager.LogClientFactory = (client, url, cb, compress) => new HttpLogUploadClient(client, url, null, cb, compress);

            // ---------------------------------------------------------
            // 전역 문화권 및 날짜 형식 구성
            // ---------------------------------------------------------
            var culture = new System.Globalization.CultureInfo("ko-KR");
            
            // 날짜 형식 사용자 정의
            culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            culture.DateTimeFormat.LongTimePattern = "HH:mm:ss";
            culture.DateTimeFormat.DateSeparator = "-";

            // 현재 스레드에 적용
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

            // 향후 스레드에 적용
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;
            
            // DevExpress 컨트롤이 이 문화권을 사용하도록 보장
            // DevExpress 컨트롤은 일반적으로 CurrentThread.CurrentCulture를 따름
            // ---------------------------------------------------------

            // 구성 로드
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            
            // 리포지토리
            services.AddSingleton<IMenuRepository, SQLiteMenuRepository>();        // 메뉴 저장소
            services.AddSingleton<IModuleRepository, SQLiteModuleRepository>();    // 모듈 저장소
            services.AddScoped<IComponentRepository, SQLiteComponentRepository>(); // 프레임워크 컴포넌트 저장소
            services.AddScoped<IProgramRepository, SQLiteProgramRepository>();     // 프로그램 저장소
            services.AddScoped<IUserRepository, SQLiteUserRepository>();           // 사용자 저장소
            services.AddScoped<ISecurityRepository, SQLiteSecurityRepository>();   // 보안 저장소
            services.AddSingleton<nU3.Core.Services.ModuleLoaderService>();        // DLL 모듈 로더 서비스

            // 비즈니스 로직 팩토리
            services.AddSingleton<nU3.Core.Logic.IBizLogicFactory, nU3.Core.Logic.BizLogicFactory>();
            
            // 3. 네트워크 연결 설정
            string baseUrl = configuration["ServerConnection:BaseUrl"] ?? "http://localhost:5000";

            // 인증 서비스 (IDP 연동)
            services.AddScoped<nU3.Core.Interfaces.IAuthenticationService>(sp => 
                new nU3.Connectivity.Implementations.HttpAuthenticationClient(
                    sp.GetRequiredService<System.Net.Http.HttpClient>(), 
                    baseUrl
                ));

            services.AddTransient<nUShell>();
            services.AddTransient<Forms.LoginForm>(); // 로그인 폼 등록
            services.AddSingleton<nU3.Core.Events.IEventAggregator, nU3.Core.Events.EventAggregator>();

            // 공유 HttpClient 등록
            services.AddSingleton(sp => new System.Net.Http.HttpClient 
            { 
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromMinutes(10) 
            });

            // 연결 서비스를 위한 HTTP 클라이언트 등록
            services.AddScoped<IDBAccessService>(sp => 
                new HttpDBAccessClient(
                    sp.GetRequiredService<System.Net.Http.HttpClient>(), 
                    baseUrl
                ));
            
            services.AddScoped<IFileTransferService>(sp => 
                new HttpFileTransferClient(
                    sp.GetRequiredService<System.Net.Http.HttpClient>(), 
                    baseUrl
                ));

            services.AddScoped<ILogUploadService>(sp => 
                new HttpLogUploadClient(
                    sp.GetRequiredService<System.Net.Http.HttpClient>(), 
                    baseUrl
                ));

            // 비즈니스 데이터 액세스 서비스 (레거시 백엔드 어댑터)
            services.AddScoped<nU3.Core.Interfaces.IDataService, nU3.Connectivity.Implementations.ServiceAdapter>();

            using (var provider = services.BuildServiceProvider())
            {
                // 1. 로그인 화면 표시
                using (var loginForm = provider.GetRequiredService<Forms.LoginForm>())
                {
                    if (loginForm.ShowDialog() != DialogResult.OK)
                    {
                        return; 
                    }
                }

                // 2. 셸 실행
                var form = provider.GetRequiredService<nUShell>();
                
                if (args.Length > 0)
                {
                    form.StartupUri = args[0];
                }

                Application.Run(form);
            }
        }
    }
}