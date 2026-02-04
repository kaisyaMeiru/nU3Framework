using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using nU3.Core.Repositories;
using nU3.Data;
using nU3.Data.Repositories;
using DevExpress.LookAndFeel;
using DevExpress.Skins;

namespace nU3.Shell
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SkinManager.EnableFormSkins();
            UserLookAndFeel.Default.SetSkinStyle("Office 2019 Black");

            var services = new ServiceCollection();
            services.AddSingleton<LocalDatabaseManager>();
            services.AddSingleton<IMenuRepository, SQLiteMenuRepository>();
            services.AddSingleton<IModuleRepository, SQLiteModuleRepository>();
            services.AddSingleton<nU3.Core.Services.ModuleLoaderService>(); // ModuleLoaderService 등록
            
            // 비즈니스 로직 팩토리 등록
            services.AddSingleton<nU3.Core.Logic.IBizLogicFactory, nU3.Core.Logic.BizLogicFactory>();

            services.AddTransient<nUShell>();
            services.AddSingleton<nU3.Core.Events.IEventAggregator, nU3.Core.Events.EventAggregator>();

            // 3. 네트워크 서비스 (싱글톤 HttpClient)
            // appsettings 또는 기본값 사용. 현재는 개발용으로 localhost 하드코딩
            string baseUrl = "https://localhost:64229"; 
            
            // 공유 HttpClient 인스턴스 등록
            services.AddSingleton(sp => new System.Net.Http.HttpClient 
            { 
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromMinutes(10) // 모든 작업에 대한 긴 타임아웃
            });

            // 공유 HttpClient를 사용하여 클라이언트 등록
            services.AddScoped<nU3.Connectivity.IDBAccessService>(sp => 
                new nU3.Connectivity.Implementations.HttpDBAccessClient(
                    sp.GetRequiredService<System.Net.Http.HttpClient>(), 
                    baseUrl
                ));
            
            services.AddScoped<nU3.Connectivity.IFileTransferService>(sp => 
                new nU3.Connectivity.Implementations.HttpFileTransferClient(
                    sp.GetRequiredService<System.Net.Http.HttpClient>(), 
                    baseUrl
                ));

            // LogUploadClient는 특정 구성 필요할 수 있지만 HttpClient 공유는 괜찮음
            services.AddScoped<nU3.Connectivity.ILogUploadService>(sp => 
                new nU3.Connectivity.Implementations.HttpLogUploadClient(
                    sp.GetRequiredService<System.Net.Http.HttpClient>(), 
                    baseUrl
                ));

            using (var provider = services.BuildServiceProvider())
            {
                // 1. 로그인 표시
                // 참고: LoginForm은 IDisposable
                using (var loginForm = new Forms.LoginForm())
                {
                    if (loginForm.ShowDialog() != DialogResult.OK)
                    {
                        return; // 로그인 실패/취소 시 종료
                    }
                }

                // 2. 메인 쉘 실행
                var form = provider.GetRequiredService<nUShell>();
                Application.Run(form);
            }
        }
    }
}
