using Kimera.Data;
using Kimera.Data.Contexts;
using Kimera.Data.Entities;
using Kimera.Data.Extensions;
using Kimera.Network;
using Kimera.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
            if (!PrivilegeHelper.IsAdministrator() && !_debugMode)
            {
                PrivilegeHelper.RunAsAdiministrator();
                return;
            }

            InitializeLanguageResources();
            InitializeDatabase();

            // Network
            AntiDPIServiceProvider.InitializeService(Environment.CurrentDirectory);
            MetadataServiceProvider.InitializeService();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            AntiDPIServiceProvider.DisposeService();

            base.OnExit(e);
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
            await _databaseContext.EnsureCategoryCreated(Settings.GUID_UNCATEGORIZED_CATEGORY, "UNCATEGORIZED").ConfigureAwait(false);
            await _databaseContext.EnsureCategoryCreated(Settings.GUID_FAVORITE_CATEGORY, "FAVORITE").ConfigureAwait(false);
        }
    }
}
