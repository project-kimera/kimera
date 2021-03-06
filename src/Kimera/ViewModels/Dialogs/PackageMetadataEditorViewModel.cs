using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.Data.Enums;
using Kimera.Data.Extensions;
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
        private bool _isRegistered = false;

        public bool IsRegistered
        {
            get => _isRegistered;
        }

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

        private List<Tuple<string, string, string>> _presets = new List<Tuple<string, string, string>>();

        public List<Tuple<string, string, string>> Presets
        {
            get => _presets;
            set => Set(ref _presets, value);
        }

        private Tuple<string, string, string> _selectedPreset = new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);

        public Tuple<string, string, string> SelectedPreset
        {
            get => _selectedPreset;
            set
            {
                Set(ref _selectedPreset, value);
                ExecutableFilePath = value.Item2;
                CommandLineArguments = value.Item3;
            }
        }


        public string ExecutableFilePath
        {
            get => _metadata.ExecutableFilePath;
            set
            {
                _metadata.ExecutableFilePath = value;
                NotifyOfPropertyChange("ExecutableFilePath");
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

        public PackageMetadataEditorViewModel(PackageMetadata metadata)
        {
            InitializePackageMetadata(metadata);
            InitializePresets();
        }

        private void InitializePackageMetadata(PackageMetadata metadata)
        {
            try
            {
                // To check the metadata has linked with a game navigation.
                var temp = App.DatabaseContext.PackageMetadatas.Where(p => p.SystemId == metadata.SystemId).FirstOrDefault();

                if (temp == null)
                {
                    _isRegistered = false;
                    _metadata = metadata;
                }
                else
                {
                    _isRegistered = true;
                    _metadata = metadata.Copy();
                }

                Components = new BindableCollection<Component>(_metadata.Components);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while loading the package metadata.");
            }
        }

        private void InitializePresets()
        {
            Presets.Add(new Tuple<string, string, string>("Default", "%ActualEntryPointPath%", ""));
            Presets.Add(new Tuple<string, string, string>("Locale Emulator", "LEProc.exe", "-run %ActualEntryPointPath%"));
            Presets.Add(new Tuple<string, string, string>("Locale Emulator (Administrator)", "LEProc.exe", "-runas [GUID] %ActualEntryPointPath%"));
        }

        public void AddComponent()
        {
            Components.Add(new Component());
        }

        public void RemoveComponent()
        {
            Components.Remove(SelectedComponent);
        }

        public async void ExploreEntryPoint()
        {
            List<string> exts = new List<string>();
            exts.Add(".exe");

            ArchiveFileExplorerViewModel viewModel = new ArchiveFileExplorerViewModel();
            viewModel.Title = (string)App.Current.Resources["VIEW_ARCHIVEFILEEXPLORER_EP_TITLE"];
            viewModel.Caption = string.Format((string)App.Current.Resources["VIEW_ARCHIVEFILEEXPLORER_EP_CAPTION"]);
            viewModel.Extensions = exts;
            viewModel.Components = _components;

            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {
                EntryPointFilePath = viewModel.SelectedFile;
            }
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

            if (PackageType == PackageType.Single && Components.Count != 1)
            {
                MessageBox.Show((string)App.Current.Resources["VM_PACKAGEMETADATAEDITOR_INVALID_COMP_COUNT_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Warning("Too many or too few components.");
                return;
            }

            if (string.IsNullOrEmpty(EntryPointFilePath))
            {
                MessageBox.Show((string)App.Current.Resources["VM_PACKAGEMETADATAEDITOR_NULL_EP_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Warning("The entry point is null.");
                return;
            }

            if (string.IsNullOrEmpty(ExecutableFilePath))
            {
                MessageBox.Show((string)App.Current.Resources["VM_PACKAGEMETADATAEDITOR_NULL_EXE_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Warning("The entry point is null.");
                return;
            }

            Component mainComponent = Components.Where(c => c.Index == 0).FirstOrDefault();

            if (PackageType == PackageType.Single && mainComponent == null)
            {
                MessageBox.Show((string)App.Current.Resources["VM_PACKAGEMETADATAEDITOR_MAIN_COMP_NOT_EXISTS_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Warning("The main component is null.");
                return;
            }

            if (_isRegistered)
            {
                using (var transaction = await App.DatabaseContext.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    // Add new items.
                    foreach (Component component in _components)
                    {
                        Component temp = App.DatabaseContext.Components.Where(c => c.Id == component.Id).FirstOrDefault();

                        if (temp == null)
                        {
                            App.DatabaseContext.Components.Add(component);
                            _metadata.AddComponent(component);
                        }
                    }

                    // Remove items from database.
                    List<Component> components = App.DatabaseContext.Components.Where(c => c.PackageMetadata == _metadata.SystemId).ToList();

                    foreach (Component component in components)
                    {
                        if (!_components.Contains(component))
                        {
                            App.DatabaseContext.Components.Remove(component);
                            _metadata.Components.Remove(component);
                        }
                    }

                    _metadata.GameNavigation.PackageStatus = PackageStatus.NeedProcessing;

                    await App.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

                    await transaction.CommitAsync().ConfigureAwait(false);
                }
            }
            else
            {
                _metadata.Components = _components.ToHashSet();
            }

            await TryCloseAsync(true);
        }
    }
}
