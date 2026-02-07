using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using nU3.Core.Repositories;
using nU3.Data;
using nU3.Data.Repositories;
using nU3.Connectivity;

namespace nU3.Tools.Deployer
{
    public static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Configuration Load
            var configuration = LoadConfiguration();

            var services = new ServiceCollection();
            
            // Configuration Reg
            services.AddSingleton<IConfiguration>(configuration);
            
            // Network & Server Connection
            string baseUrl = configuration.GetValue<string>("ServerConnection:BaseUrl") ?? "http://localhost:5000";
            
            // Register shared HttpClient
            services.AddSingleton(sp => new System.Net.Http.HttpClient 
            { 
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromMinutes(10)
            });

            // Register HTTP Clients for Connectivity Services
            services.AddScoped<IDBAccessService>(sp => 
                new nU3.Connectivity.Implementations.HttpDBAccessClient(
                    sp.GetRequiredService<System.Net.Http.HttpClient>(), 
                    baseUrl
                ));
            
            services.AddScoped<IFileTransferService>(sp => 
                new nU3.Connectivity.Implementations.HttpFileTransferClient(
                    sp.GetRequiredService<System.Net.Http.HttpClient>(), 
                    baseUrl
                ));

            services.AddScoped<ILogUploadService>(sp => 
                new nU3.Connectivity.Implementations.HttpLogUploadClient(
                    sp.GetRequiredService<System.Net.Http.HttpClient>(), 
                    baseUrl
                ));
            
            // Repositories (They now use the injected IDBAccessService which is HttpDBAccessClient)
            services.AddScoped<IModuleRepository, SQLiteModuleRepository>();
            services.AddScoped<IMenuRepository, SQLiteMenuRepository>();
            services.AddScoped<IProgramRepository, SQLiteProgramRepository>();
            services.AddScoped<IComponentRepository, SQLiteComponentRepository>();
            services.AddScoped<IUserRepository, SQLiteUserRepository>();
            services.AddScoped<ISecurityRepository, SQLiteSecurityRepository>();
            
            // Form
            services.AddTransient<DeployerForm>();

            ServiceProvider = services.BuildServiceProvider();
            
            var form = ServiceProvider.GetRequiredService<DeployerForm>();
            Application.Run(form);
        }

        /// <summary>
        /// Configuration Load
        /// </summary>
        private static IConfiguration LoadConfiguration()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            
            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .Build();
        }
    }
}