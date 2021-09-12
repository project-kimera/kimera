using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.Data.Enums;
using Kimera.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kimera.ViewModels.Dialogs
{
    public class GameMetadataEditorViewModel : Screen
    {
        private readonly bool _useDirectWriting = false;

        public bool UseDirectWriting
        {
            get => _useDirectWriting;
        }

        private GameMetadata _metadata = new GameMetadata();

        public GameMetadata Metadata
        {
            get => _metadata;
        }

        public string Name
        {
            get => _metadata.Name;
            set
            {
                _metadata.Name = value;
                NotifyOfPropertyChange(Name);
            }
        }

        public string Description
        {
            get => _metadata.Description;
            set
            {
                _metadata.Description = value;
                NotifyOfPropertyChange(Description);
            }
        }

        public string Creator
        {
            get => _metadata.Creator;
            set
            {
                _metadata.Creator = value;
                NotifyOfPropertyChange(Creator);
            }
        }

        public Age AdmittedAge
        {
            get => _metadata.AdmittedAge;
            set
            {
                _metadata.AdmittedAge = value;
                NotifyOfPropertyChange("AdmittedAge");
            }
        }

        public string Genres
        {
            get => _metadata.Genres;
            set
            {
                _metadata.Genres = value;
                NotifyOfPropertyChange(Genres);
            }
        }

        public string Tags
        {
            get => _metadata.Tags;
            set
            {
                _metadata.Tags = value;
                NotifyOfPropertyChange(Tags);
            }
        }

        public string SupportedLanguages
        {
            get => _metadata.SupportedLanguages;
            set
            {
                _metadata.SupportedLanguages = value;
                NotifyOfPropertyChange(SupportedLanguages);
            }
        }

        public double Score
        {
            get => _metadata.Score;
            set
            {
                _metadata.Score = value;
                NotifyOfPropertyChange("Score");
            }
        }

        public string IconUri
        {
            get => _metadata.IconUri;
            set
            {
                _metadata.IconUri = value;
                NotifyOfPropertyChange(IconUri);
            }
        }

        public string ThumbnailUri
        {
            get => _metadata.ThumbnailUri;
            set
            {
                _metadata.ThumbnailUri = value;
                NotifyOfPropertyChange(ThumbnailUri);
            }
        }

        public string HomepageUrl
        {
            get => _metadata.HomepageUrl;
            set
            {
                _metadata.HomepageUrl = value;
                NotifyOfPropertyChange(HomepageUrl);
            }
        }

        public GameMetadataEditorViewModel(GameMetadata metadata, bool useDirectWriting = false)
        {
            _useDirectWriting = useDirectWriting;
            InitializeGameMetadata(metadata);
        }

        private void InitializeGameMetadata(GameMetadata metadata)
        {
            try
            {
                if (_useDirectWriting)
                {
                    _metadata = metadata;
                }
                else
                {
                    _metadata = metadata.Copy();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while loading the components data table.");
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
                MessageBox.Show((string)App.Current.Resources["VM_GAMEMETADATAEDITOR_INVALID_MD_MSG"], "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Warning("The game metadata values aren't valid.");
                return;
            }

            await TryCloseAsync(true);
        }
    }
}
