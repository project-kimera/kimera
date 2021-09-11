using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.ViewModels.Dialogs
{
    public class SettingsViewModel : Screen
    {
        private Settings _settings = IoC.Get<Settings>();

        public bool UseAutoStarter
        {
            get => _settings.UseAutoStarter;
            set
            {
                _settings.UseAutoStarter = value;
                NotifyOfPropertyChange("UseAutoStarter");
            }
        }

        public string WorkDirectory
        {
            get => _settings.WorkDirectory;
            set
            {
                _settings.WorkDirectory = value;
                NotifyOfPropertyChange("WorkDirectory");
            }
        }

        public string MasterKeys
        {
            get => _settings.MasterKeys;
            set
            {
                _settings.MasterKeys = value;
                NotifyOfPropertyChange("MasterKeys");
            }
        }

        public bool UseAutoRemover
        {
            get => _settings.UseAutoRemover;
            set
            {
                _settings.UseAutoRemover = value;
                NotifyOfPropertyChange("UseAutoRemover");
            }
        }

        public int AutoRemovingInterval
        {
            get => _settings.AutoRemovingInterval;
            set
            {
                _settings.AutoRemovingInterval = value;
                NotifyOfPropertyChange("AutoRemovingInterval");
            }
        }

        public bool UseAntiDPIService
        {
            get => _settings.UseAntiDPIService;
            set
            {
                _settings.UseAntiDPIService = value;
                NotifyOfPropertyChange("UseAntiDPIService");
            }
        }

        private string _versionString = string.Empty;

        public string VersionString
        {
            get => _versionString;
        }

        public SettingsViewModel()
        {
            InitializeSettings();
        }

        private void InitializeSettings()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            _versionString = $"Kimera Version {version.Major}.{version.Minor} (Build {version.Build}, Revision {version.Revision})";
        }

        public void ExploreWorkDirectory()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.Title = "Open";
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (Directory.Exists(dialog.FileName))
                {
                    WorkDirectory = dialog.FileName;
                }
            }
        }
    }
}
