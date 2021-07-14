using Kimera.AntiDPI;
using Kimera.Utilities;
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
        private bool _debugMode = true;

        private AntiDPIHelper _antiDPI = new AntiDPIHelper(Environment.CurrentDirectory);

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!PrivilegeManager.IsAdministrator() && !_debugMode)
            {
                PrivilegeManager.RunAsAdiministrator();
            }
            else
            {
                SetLanguageResources();
                StartAntiDPI();
                base.OnStartup(e);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            StopAntiDPI();
            base.OnExit(e);
        }

        private void SetLanguageResources()
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
