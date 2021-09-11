﻿using Caliburn.Micro;
using Kimera.Data.Contexts;
using Kimera.Data.Extensions;
using Kimera.IO;
using Kimera.Network;
using Kimera.Services;
using Kimera.Utilities;
using Kimera.ViewModels;
using Kimera.ViewModels.Dialogs;
using Kimera.ViewModels.Pages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
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
        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container.Instance(_container);

            // Singletons
            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<Settings>()
                .Singleton<TaskService>()
                .Singleton<GameService>()
                .Singleton<LibraryService>()
                .Singleton<ShellViewModel>()
                .Singleton<LibraryViewModel>()
                .Singleton<StatisticsViewModel>()
                .Singleton<SearcherViewModel>();

            // Pages
            _container
                .PerRequest<GameViewModel>();

            // Dialogs
            _container
                .PerRequest<SettingsViewModel>()
                .PerRequest<CategoryNameEditorViewModel>()
                .PerRequest<CategorySelectorViewModel>()
                .PerRequest<StringEditorViewModel>()
                .PerRequest<GameMetadataEditorViewModel>()
                .PerRequest<PackageMetadataEditorViewModel>()
                .PerRequest<GameRegisterViewModel>();
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            var task = Task.Factory.StartNew(() =>
            {
                if (File.Exists(Settings.DataPath))
                {
                    string json = TextFileManager.ReadTextFile(Settings.DataPath, Encoding.UTF8);

                    var settings = JsonConvert.DeserializeObject<Settings>(json);

                    IoC.BuildUp(settings);
                }
            });

            await task;

            await DisplayRootViewForAsync(typeof(ShellViewModel));
        }

        protected override async void OnExit(object sender, EventArgs e)
        {
            var settings = IoC.Get<Settings>();
            string json = JsonConvert.SerializeObject(settings);

            TextFileManager.WriteTextFile(Settings.DataPath, json, Encoding.UTF8);
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
