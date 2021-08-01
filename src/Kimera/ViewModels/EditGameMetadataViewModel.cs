using Kimera.Commands;
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

namespace Kimera.ViewModels
{
    public class EditGameMetadataViewModel : ViewModelBase
    {
        private GameMetadata _previousMetadata = new GameMetadata();

        private GameMetadata _metadata = new GameMetadata();

        public GameMetadata Metadata
        {
            get
            {
                return _metadata;
            }
            set
            {
                _metadata = value;
                RaisePropertyChanged();
            }
        }

        public string Name
        {
            get
            {
                return _metadata.Name;
            }
            set
            {
                _metadata.Name = value;
                RaisePropertyChanged();
            }
        }

        public string Description
        {
            get
            {
                return _metadata.Description;
            }
            set
            {
                _metadata.Description = value;
                RaisePropertyChanged();
            }
        }

        public string Creator
        {
            get
            {
                return _metadata.Creator;
            }
            set
            {
                _metadata.Creator = value;
                RaisePropertyChanged();
            }
        }

        public Age AdmittedAge
        {
            get
            {
                return _metadata.AdmittedAge;
            }
            set
            {
                _metadata.AdmittedAge = value;
                RaisePropertyChanged();
            }
        }

        public string Genres
        {
            get
            {
                return _metadata.Genres;
            }
            set
            {
                _metadata.Genres = value;
                RaisePropertyChanged();
            }
        }

        public string Tags
        {
            get
            {
                return _metadata.Tags;
            }
            set
            {
                _metadata.Tags = value;
                RaisePropertyChanged();
            }
        }

        public string SupportedLanguages
        {
            get
            {
                return _metadata.SupportedLanguages;
            }
            set
            {
                _metadata.SupportedLanguages = value;
                RaisePropertyChanged();
            }
        }

        public double Score
        {
            get
            {
                return _metadata.Score;
            }
            set
            {
                _metadata.Score = value;
                RaisePropertyChanged();
            }
        }

        public string IconUri
        {
            get
            {
                return _metadata.IconUri;
            }
            set
            {
                _metadata.IconUri = value;
                RaisePropertyChanged();
            }
        }

        public string ThumbnailUri
        {
            get
            {
                return _metadata.ThumbnailUri;
            }
            set
            {
                _metadata.ThumbnailUri = value;
                RaisePropertyChanged();
            }
        }

        public string HomepageUrl
        {
            get
            {
                return _metadata.HomepageUrl;
            }
            set
            {
                _metadata.HomepageUrl = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<Window> CancelCommand { get; }

        public RelayCommand<Window> ConfirmCommand { get; }

        public EditGameMetadataViewModel(GameMetadata metadata)
        {
            _previousMetadata = metadata.Copy();

            if (metadata == null)
            {
                _metadata = new GameMetadata();
            }
            else
            {
                _metadata = metadata;
            }

            CancelCommand = new RelayCommand<Window>(Cancel);
            ConfirmCommand = new RelayCommand<Window>(Confirm);
        }

        private void Cancel(Window window)
        {
            _metadata = _previousMetadata;

            if (window != null)
            {
                window.Close();
            }
        }

        private void Confirm(Window window)
        {
            if (!ValidationHelper.IsValid(window))
            {
                MessageBox.Show("메타데이터가 유효하지 않습니다. 각 값들을 형식에 맞게 수정해주세요.", "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Warning("The game metadata values aren't valid.");
                return;
            }

            if (window != null)
            {
                window.Close();
            }
        }
    }
}
