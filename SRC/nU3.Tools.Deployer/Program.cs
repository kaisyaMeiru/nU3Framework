using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using nU3.Core.Repositories;
using nU3.Data;
using nU3.Data.Repositories;

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

            // Configuration 로드
            var configuration = LoadConfiguration();

            var services = new ServiceCollection();
            
            // Configuration 등록
            services.AddSingleton<IConfiguration>(configuration);
            
            // DB Manager
            services.AddSingleton<LocalDatabaseManager>();
            
            // Repositories
            services.AddScoped<IModuleRepository, SQLiteModuleRepository>();
            services.AddScoped<IMenuRepository, SQLiteMenuRepository>();
            services.AddScoped<IProgramRepository, SQLiteProgramRepository>();
            services.AddScoped<IComponentRepository, SQLiteComponentRepository>();
            
            // Form
            services.AddTransient<DeployerForm>();

            ServiceProvider = services.BuildServiceProvider();
            
            var form = ServiceProvider.GetRequiredService<DeployerForm>();
            Application.Run(form);
        }

        /// <summary>
        /// Configuration 로드
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
