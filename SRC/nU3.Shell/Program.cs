using System;
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

            // Load Configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            
            // Repositories
            services.AddSingleton<IMenuRepository, SQLiteMenuRepository>();        // Menu Repository
            services.AddSingleton<IModuleRepository, SQLiteModuleRepository>();    // DLL Module Repository
            services.AddScoped<IComponentRepository, SQLiteComponentRepository>(); // Frameork Component Repository
            services.AddScoped<IProgramRepository, SQLiteProgramRepository>();     // Program Repository
            services.AddScoped<IUserRepository, SQLiteUserRepository>();           // User Repository
            services.AddScoped<ISecurityRepository, SQLiteSecurityRepository>();   // Security Repository
            services.AddSingleton<nU3.Core.Services.ModuleLoaderService>();        // DLL Module Loader Service

            // Biz Logic Factory
            services.AddSingleton<nU3.Core.Logic.IBizLogicFactory, nU3.Core.Logic.BizLogicFactory>();
            
            // Authentication Service
            services.AddScoped<nU3.Core.Interfaces.IAuthenticationService, nU3.Core.Services.AuthenticationService>();

            services.AddTransient<nUShell>();
            services.AddTransient<Forms.LoginForm>(); // Register LoginForm
            services.AddSingleton<nU3.Core.Events.IEventAggregator, nU3.Core.Events.EventAggregator>();

            // 3. Network Connection (HttpClient)
            string baseUrl = configuration["ServerConnection:BaseUrl"] ?? "http://localhost:5000"; 
            
            // Register shared HttpClient
            services.AddSingleton(sp => new System.Net.Http.HttpClient 
            { 
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromMinutes(10) 
            });

            // Register HTTP Clients for Connectivity Services
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

            using (var provider = services.BuildServiceProvider())
            {
                // 1. Login Show
                using (var loginForm = provider.GetRequiredService<Forms.LoginForm>())
                {
                    if (loginForm.ShowDialog() != DialogResult.OK)
                    {
                        return; 
                    }
                }

                // 2. Run Shell
                var form = provider.GetRequiredService<nUShell>();
                Application.Run(form);
            }
        }
    }
}