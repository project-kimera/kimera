using Kimera.Data.Contexts;
using Kimera.Data.Extensions;
using Kimera.Network;
using Kimera.Utilities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Kimera
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly bool _debugMode = true;

        public static KimeraContext DatabaseContext { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            InitializeKimera();

            base.OnStartup(e);
        }

        private async void InitializeKimera()
        {
            InitializeLogger();
            Log.Information("The logger has been initialized.");

            if (!_debugMode && !PrivilegeHelper.IsAdministrator())
            {
                Log.Warning("The current process does not have administrator privileges. The process will be restart with administrator privileges.");
                PrivilegeHelper.RunAsAdiministrator();
                return;
            }

            InitializeLocalization();
            Log.Information("The language resources has been initialized.");

            await InitializeDatabaseAsync();
            Log.Information("The database has been initialized.");

            AntiDPIServiceProvider.InitializeService(Environment.CurrentDirectory);
            Log.Information("The Anti DPI Service has been initialized.");

            MetadataServiceProvider.InitializeService();
            Log.Information("The Metadata Service has been initialized.");
        }

        private void InitializeLogger()
        {
            string fileName = @"data\logs\log-.txt";
            string outputTemplateString = "{Timestamp:HH:mm:ss.ms} ({ThreadId}) [{Level}] {Message}{NewLine}{Exception}";

            if (_debugMode)
            {
                var log = new LoggerConfiguration()
                    .Enrich.WithProperty("ThreadId", Thread.CurrentThread.ManagedThreadId)
                    .WriteTo.File(fileName, restrictedToMinimumLevel: LogEventLevel.Verbose, outputTemplate: outputTemplateString, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 100000)
                    .CreateLogger();

                Log.Logger = log;
            }
            else
            {
                var log = new LoggerConfiguration()
                    .WriteTo.File(fileName, restrictedToMinimumLevel: LogEventLevel.Warning, outputTemplate: outputTemplateString, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 100000)
                    .CreateLogger();

                Log.Logger = log;
            }
        }

        private void InitializeLocalization()
        {
            ResourceDictionary dict = new ResourceDictionary();

            switch (Thread.CurrentThread.CurrentCulture.Name)
            {
                case "en-US":
                    dict.Source = new Uri(@"..\Resources\Localizations\Localization.en-US.xaml", UriKind.Relative);
                    break;
                case "ko-KR":
                    dict.Source = new Uri(@"..\Resources\Localizations\Localization.ko-KR.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri(@"..\Resources\Localizations\Localization.en-US.xaml", UriKind.Relative);
                    break;
            }

            App.Current.Resources.MergedDictionaries.Add(dict);
        }

        private async Task InitializeDatabaseAsync()
        {
            // Migrate and initialize the database automatically.
            App.DatabaseContext = new KimeraContext(@"data\Kimera.db");
            await App.DatabaseContext.Database.MigrateAsync().ConfigureAwait(false);

            // Load the database.
            await App.DatabaseContext.Categories.LoadAsync().ConfigureAwait(false);
            await App.DatabaseContext.CategorySubscriptions.LoadAsync().ConfigureAwait(false);
            await App.DatabaseContext.Components.LoadAsync().ConfigureAwait(false);
            await App.DatabaseContext.Games.LoadAsync().ConfigureAwait(false);
            await App.DatabaseContext.GameMetadatas.LoadAsync().ConfigureAwait(false);
            await App.DatabaseContext.PackageMetadatas.LoadAsync().ConfigureAwait(false);

            // Ensure default categories created.
            await App.DatabaseContext.EnsureCategoryCreatedAsync(Settings.GUID_ALL_CATEGORY, "ALL").ConfigureAwait(false);
            await App.DatabaseContext.EnsureCategoryCreatedAsync(Settings.GUID_FAVORITE_CATEGORY, "FAVORITE").ConfigureAwait(false);
        }
    }
}
