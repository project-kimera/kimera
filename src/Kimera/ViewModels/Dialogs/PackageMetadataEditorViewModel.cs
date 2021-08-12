using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.Data.Enums;
using Kimera.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kimera.ViewModels.Dialogs
{
    public class PackageMetadataEditorViewModel : Screen
    {
        private PackageMetadata _metadata = new PackageMetadata();

        public PackageMetadata Metadata
        {
            get => _metadata;
        }

        private BindableCollection<Component> _components = new BindableCollection<Component>();

        public BindableCollection<Component> Components
        {
            get => _components;
            set => Set(ref _components, value);
        }

        private Component _selectedComponent = new Component();

        public Component SelectedComponent
        {
            get => _selectedComponent;
            set => Set(ref _selectedComponent, value);
        }

        public PackageType PackageType
        {
            get => _metadata.Type;
            set
            {
                _metadata.Type = value;
                NotifyOfPropertyChange("PackageType");
            }
        }

        public string EntryPointFilePath
        {
            get => _metadata.EntryPointFilePath;
            set
            {
                _metadata.EntryPointFilePath = value;
                NotifyOfPropertyChange("EntryPointFilePath");
            }
        }

        public string CommandLineArguments
        {
            get => _metadata.CommandLineArguments;
            set
            {
                _metadata.CommandLineArguments = value;
                NotifyOfPropertyChange("CommandLineArguments");
            }
        }

        public async Task LoadPackageMetadataAsync(PackageMetadata metadata)
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    _metadata = metadata.Copy();
                    Components = new BindableCollection<Component>(metadata.Components);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred while loading the package metadata.");
                }
            });
        }

        public async Task SavePackageMetadataAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    _metadata.Components = _components.ToHashSet();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred while saving the package metadata.");
                }
            });
        }

        public void AddComponent()
        {
            Components.Add(new Component());
        }

        public void RemoveComponent()
        {
            Components.Remove(SelectedComponent);
        }

        public async void Cancel()
        {
            await TryCloseAsync(false);
        }

        public async void Confirm(Window window)
        {
            if (window == null)
            {
                Log.Warning("The game metadata values shouldn't be null.");
                return;
            }

            if (!ValidationHelper.IsValid(window))
            {
                MessageBox.Show((string)App.Current.Resources["VM_PACKAGEMETADATAEDITOR_INVALID_MD_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Warning("The game metadata values aren't valid.");
                return;
            }

            if ((PackageType == PackageType.Executable || PackageType == PackageType.Archive) && Components.Count != 1)
            {
                MessageBox.Show((string)App.Current.Resources["VM_PACKAGEMETADATAEDITOR_INVALID_COMP_COUNT_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Warning("Too many or too few components.");
                return;
            }

            if ((PackageType != PackageType.Executable) && string.IsNullOrEmpty(EntryPointFilePath))
            {
                MessageBox.Show((string)App.Current.Resources["VM_PACKAGEMETADATAEDITOR_INVALID_PKGTYPE_FOR_NULL_EP_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Warning("The entry point is null.");
                return;
            }

            Component mainComponent = Components.Where(c => c.Index == 0).FirstOrDefault();

            if ((PackageType == PackageType.Executable || PackageType == PackageType.Archive) && mainComponent == null)
            {
                MessageBox.Show((string)App.Current.Resources["VM_PACKAGEMETADATAEDITOR_MAIN_COMP_NOT_EXISTS_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Warning("The main component is null.");
                return;
            }

            if (PackageType == PackageType.Executable && mainComponent.Type != ComponentType.Executable)
            {
                MessageBox.Show((string)App.Current.Resources["VM_PACKAGEMETADATAEDITOR_INVALID_MAIN_COMP_FOR_EXE_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Warning("The main component is not a executable component.");
                return;
            }

            if (PackageType == PackageType.Archive && mainComponent.Type != ComponentType.Archive)
            {
                MessageBox.Show((string)App.Current.Resources["VM_PACKAGEMETADATAEDITOR_INVALID_MAIN_COMP_FOR_ARCH_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Warning("The main component is not a archive component.");
                return;
            }

            _metadata.GameNavigation.PackageStatus = PackageStatus.NeedProcessing;
            await TryCloseAsync(true);
        }
    }
}
