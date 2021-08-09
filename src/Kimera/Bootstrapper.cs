using Caliburn.Micro;
using Kimera.Data.Contexts;
using Kimera.Data.Extensions;
using Kimera.Network;
using Kimera.Services;
using Kimera.Utilities;
using Kimera.ViewModels;
using Kimera.ViewModels.Dialogs;
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
                .Singleton<GameService>()
                .Singleton<LibraryService>()
                .Singleton<ShellViewModel>()
                .Singleton<SearcherViewModel>();

            // Shell Pages
            _container
               .PerRequest<LibraryViewModel>()
               .PerRequest<StatisticsViewModel>()
               .PerRequest<SettingsViewModel>();

            // Pages
            _container
               .PerRequest<GameViewModel>()
               .PerRequest<SingleFileRegisterViewModel>()
               .PerRequest<ChunkRegisterViewModel>()
               .PerRequest<MultipleFileRegisterViewModel>();

            // Dialogs
            _container
               .PerRequest<CategoryNameEditorViewModel>()
               .PerRequest<CategorySelectorViewModel>()
               .PerRequest<StringEditorViewModel>()
               .PerRequest<MetadataEditorViewModel>()
               .PerRequest<SettingsEditorViewModel>();
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
