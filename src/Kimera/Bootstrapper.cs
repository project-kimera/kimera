using Caliburn.Micro;
using Kimera.Data.Contexts;
using Kimera.Data.Extensions;
using Kimera.Network;
using Kimera.Utilities;
using Kimera.ViewModels;
using Kimera.ViewModels.Pages;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Kimera
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly bool _debugMode = true;

        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
            InitializeKimera();
        }

        private async void InitializeKimera()
        {
            await Task.Run(() =>
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

                InitializeDatabase();
                Log.Information("The database has been initialized.");

                AntiDPIServiceProvider.InitializeService(Environment.CurrentDirectory);
                Log.Information("The Anti DPI Service has been initialized.");

                MetadataServiceProvider.InitializeService();
                Log.Information("The Metadata Service has been initialized.");
            });
        }

        private void InitializeLogger()
        {
            string fileName = @"logs\log-.txt";
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

        private async void InitializeDatabase()
        {
            // Migrate and initialize the database automatically.
            App.DatabaseContext = new KimeraContext();
            await App.DatabaseContext.Database.MigrateAsync().ConfigureAwait(false);

            // Load the database.
            await App.DatabaseContext.Categories.LoadAsync().ConfigureAwait(false);
            await App.DatabaseContext.CategorySubscriptions.LoadAsync().ConfigureAwait(false);
            await App.DatabaseContext.Components.LoadAsync().ConfigureAwait(false);
            await App.DatabaseContext.Games.LoadAsync().ConfigureAwait(false);
            await App.DatabaseContext.GameMetadatas.LoadAsync().ConfigureAwait(false);
            await App.DatabaseContext.PackageMetadatas.LoadAsync().ConfigureAwait(false);

            // Ensure default categories created.
            await App.DatabaseContext.EnsureCategoryCreated(Settings.GUID_ALL_CATEGORY, "ALL").ConfigureAwait(false);
            await App.DatabaseContext.EnsureCategoryCreated(Settings.GUID_FAVORITE_CATEGORY, "FAVORITE").ConfigureAwait(false);
        }

        protected override void Configure()
        {
            _container.Instance(_container);

            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>();

            _container
               .PerRequest<ShellViewModel>()
               .PerRequest<LibraryViewModel>()
               .PerRequest<StatisticsViewModel>()
               .PerRequest<SettingsViewModel>()
               .PerRequest<SearcherViewModel>();
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync(typeof(ShellViewModel));
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "An error has occurred", MessageBoxButton.OK, MessageBoxImage.Error);

            using (var transaction = App.DatabaseContext.Database.BeginTransaction())
            {
                App.DatabaseContext.SaveChanges();

                transaction.Commit();
            }

            AntiDPIServiceProvider.DisposeService();
            Log.Information("The Anti DPI Service has been disposed.");

            Log.CloseAndFlush();
        }
    }
}
