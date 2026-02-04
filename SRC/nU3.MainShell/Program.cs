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
            services.AddScoped<IMenuRepository, SQLiteMenuRepository>();
            services.AddScoped<IModuleRepository, SQLiteModuleRepository>();
            services.AddTransient<MdiShellFormcs>();
            services.AddSingleton<nU3.Core.Events.IEventAggregator, nU3.Core.Events.EventAggregator>();

            using (var provider = services.BuildServiceProvider())
            {
                

                var form = provider.GetRequiredService<MdiShellFormcs>();
                Application.Run(form);
            }
        }
    }
}
