using Kimera.Data;
using Kimera.Data.Contexts;
using Kimera.Data.Entities;
using Kimera.Data.Extensions;
using Kimera.Network;
using Kimera.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kimera
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly bool _debugMode = true;

        private static KimeraContext _databaseContext;

        public static KimeraContext DatabaseContext
        {
            get
            {
                return _databaseContext;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledExceptionOccurred;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            InitializeLogger();

            if (!PrivilegeHelper.IsAdministrator() && !_debugMode)
            {
                Log.Warning("The current process does not have administrator privileges. The process will be restart with administrator privileges.");
                PrivilegeHelper.RunAsAdiministrator();
                return;
            }

            InitializeLanguageResources();
            InitializeDatabase();

            AntiDPIServiceProvider.InitializeService(Environment.CurrentDirectory);
            Log.Information("The Anti DPI Service has been initialized.");

            MetadataServiceProvider.InitializeService();
            Log.Information("The Metadata Service has been initialized.");

            base.OnStartup(e);
        }

        private void OnUnhandledExceptionOccurred(object sender, UnhandledExceptionEventArgs e)
        {
            _databaseContext.SaveChanges();

            AntiDPIServiceProvider.DisposeService();
            Log.Information("The Anti DPI Service has been disposed.");

            Log.CloseAndFlush();
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            AntiDPIServiceProvider.DisposeService();
            Log.Information("The Anti DPI Service has been disposed.");

            Log.CloseAndFlush();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            AntiDPIServiceProvider.DisposeService();
            Log.Information("The Anti DPI Service has been disposed.");

            Log.CloseAndFlush();

            base.OnExit(e);
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
                Log.Information("The logger has been initialized in debug mode.");
            }
            else
            {
                var log = new LoggerConfiguration()
                    .WriteTo.File(fileName, restrictedToMinimumLevel: LogEventLevel.Warning, outputTemplate: outputTemplateString, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 100000)
                    .CreateLogger();

                Log.Logger = log;
                Log.Information("The logger has been initialized.");
            }
        }

        private void InitializeLanguageResources()
        {
            ResourceDictionary dict = new ResourceDictionary();

            switch (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName)
            {
                case "en":
                    dict.Source = new Uri("..\\Resources\\StringResources.en-US.xaml", UriKind.Relative);
                    break;
                case "ko":
                    dict.Source = new Uri("..\\Resources\\StringResources.ko-KR.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("..\\Resources\\StringResources.en-US.xaml", UriKind.Relative);
                    break;
            }

            this.Resources.MergedDictionaries.Add(dict);
            Log.Information("The language resources has been initialized.");
        }

        private async void InitializeDatabase()
        {
            // Migrate and initialize the database automatically.
            _databaseContext = new KimeraContext();
            await _databaseContext.Database.MigrateAsync().ConfigureAwait(false);

            // Load the database.
            await _databaseContext.Categories.LoadAsync().ConfigureAwait(false);
            await _databaseContext.CategorySubscriptions.LoadAsync().ConfigureAwait(false);
            await _databaseContext.Components.LoadAsync().ConfigureAwait(false);
            await _databaseContext.Games.LoadAsync().ConfigureAwait(false);
            await _databaseContext.GameMetadatas.LoadAsync().ConfigureAwait(false);
            await _databaseContext.PackageMetadatas.LoadAsync().ConfigureAwait(false);

            // Ensure default categories created.
            await _databaseContext.EnsureCategoryCreated(Settings.GUID_ALL_CATEGORY, "ALL").ConfigureAwait(false);
            await _databaseContext.EnsureCategoryCreated(Settings.GUID_FAVORITE_CATEGORY, "FAVORITE").ConfigureAwait(false);

            Log.Information("The database has been initialized.");
        }
    }
}
