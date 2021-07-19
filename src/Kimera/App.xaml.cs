using Kimera.AntiDPI;
using Kimera.Data;
using Kimera.Data.Contexts;
using Kimera.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

        private AntiDPIHelper _antiDPI = new AntiDPIHelper(Environment.CurrentDirectory);

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
            if (!PrivilegeManager.IsAdministrator() && !_debugMode)
            {
                PrivilegeManager.RunAsAdiministrator();
                return;
            }

            InitializeDatabase();
            InitializeLanguageResources();

            // Check debug mode.
            if (_debugMode)
            {
                Window testWindow = new TestWindow();
                testWindow.Show();

                base.OnStartup(e);
            }
            else
            {
                StartAntiDPI();

                base.OnStartup(e);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (!_debugMode)
            {
                StopAntiDPI();
            }

            base.OnExit(e);
        }

        private void InitializeDatabase()
        {
            _databaseContext = new KimeraContext();
            _databaseContext.Database.Migrate();
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

        private void StartAntiDPI()
        {
            try
            {
                if (!_antiDPI.CheckResources())
                {
                    _antiDPI.EnsureResources();
                }

                _antiDPI.Start();
            }
            catch
            {
                MessageBox.Show("Failed to start the AntiDPI.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StopAntiDPI()
        {
            try
            {
                _antiDPI.Stop();
            }
            catch
            {
                MessageBox.Show("Failed to stop the AntiDPI.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
