using Caliburn.Micro;
using Kimera.Data.Contexts;
using Kimera.Data.Extensions;
using Kimera.IO;
using Kimera.Network;
using Kimera.Utilities;
using Kimera.ViewModels.Pages;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Kimera.ViewModels.Specials
{
    public class ScouterViewModel : Screen
    {
        private string _password = string.Empty;

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public ScouterViewModel()
        {
            Initialize();
        }

        private async void Initialize()
        {
            await InitializeLoggerAsync();
            Log.Information("The logger has been initialized.");

            await InitializeEnvironmentVariablesAsync();
            Log.Information("The environment variables has been initialized.");

            await InitializeLocalizationAsync();
            Log.Information("The language resources has been initialized.");

            await InitializeDatabaseAsync();
            Log.Information("The database has been initialized.");

            await InitializeServicesAsync();
            Log.Information("Services has been initialized.");

            App.Current.Dispatcher.Invoke(() =>
            {
                IoC.Get<ShellViewModel>().ClearTitleBar = false;

                INavigationService navigationService = IoC.Get<INavigationService>();
                navigationService.For<LibraryViewModel>().Navigate();
            });
        }

        private async Task InitializeLoggerAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    string fileName = @"data\logs\log-.log";
                    string outputTemplateString = "{Timestamp:HH:mm:ss.ms} ({ThreadId}) [{Level}] {Message}{NewLine}{Exception}";

                    if (App.DebugMode)
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
                });
            });

            await task;
        }

        private async Task InitializeEnvironmentVariablesAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                var variables = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);

                foreach (DictionaryEntry entry in variables)
                {
                    Settings.EnvironmentVariables.Add((string)entry.Key, (string)entry.Value);
                }
            });

            await task;
        }

        private async Task InitializeLocalizationAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
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
                });
            });

            await task;
        }

        private async Task InitializeDatabaseAsync()
        {
            // Migrate and initialize the database automatically.
            App.DatabaseContext = new KimeraContext(Settings.DatabaseFilePath, _password);
            await App.DatabaseContext.Database.MigrateAsync().ConfigureAwait(false);

            // Load the database.
            await App.DatabaseContext.Categories.LoadAsync().ConfigureAwait(false);
            await App.DatabaseContext.CategorySubscriptions.LoadAsync().ConfigureAwait(false);
            await App.DatabaseContext.Components.LoadAsync().ConfigureAwait(false);
            await App.DatabaseContext.Games.LoadAsync().ConfigureAwait(false);
            await App.DatabaseContext.GameMetadatas.LoadAsync().ConfigureAwait(false);
            await App.DatabaseContext.PackageMetadatas.LoadAsync().ConfigureAwait(false);
        }

        private async Task InitializeServicesAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                if (IoC.Get<Settings>().UseAntiDPIService)
                {
                    AntiDPIServiceProvider.InitializeService(Environment.CurrentDirectory);
                }

                MetadataServiceProvider.InitializeService();

                SearchServiceProvider.InitializeService();
            });

            await task;
        }
    }
}
