using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.Data.Enums;
using Kimera.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Services
{
    public class FilteringService : PropertyChangedBase
    {
        #region ::Variables::

        public string[] Filters { get; set; } = { "age:all", "age:12", "age:15", "age:18", "finish:true", "finish:false", "favorite:true", "favorite:false", "score:0-1", "score:1-2", "score:2-3", "score:3-4", "score:4-5", "status:need_processing", "status:compressed", "status:file_not_found", "status:data_not_found", "status:invalid_package", "status:exception", "status:playable", "type:single", "type:chunk" };

        private BindableCollection<string> _selectedFilters = new BindableCollection<string>();

        public BindableCollection<string> SelectedFilters
        {
            get => _selectedFilters;
            set => Set(ref _selectedFilters, value);
        }

        private FilteringMethod _method = FilteringMethod.And;

        public FilteringMethod Method
        {
            get => _method;
            set => Set(ref _method, value);
        }

        #endregion

        #region ::Constructors::

        public FilteringService()
        {
            InitializeService();
        }

        private async void InitializeService()
        {
            Log.Information("The filtering service has been initialized.");
        }

        #endregion

        #region ::Functions::

        public List<Game> FilterGames(IEnumerable<Game> games)
        {
            List<Game> result = null;

            if (_method == FilteringMethod.And)
            {
                result = games.ToList();
            }
            else if (_method == FilteringMethod.Or)
            {
                result = new List<Game>();
            }
            else if (_method == FilteringMethod.Not)
            {
                result = games.ToList();
            }

            foreach (string filter in _selectedFilters)
            {
                Predicate<Game> predicate = null;

                switch (filter.ToLower())
                {
                    case "age:all":
                        predicate = x => x.GameMetadataNavigation.AdmittedAge == Age.All;
                        break;
                    case "age:12":
                        predicate = x => x.GameMetadataNavigation.AdmittedAge == Age.Age12;
                        break;
                    case "age:15":
                        predicate = x => x.GameMetadataNavigation.AdmittedAge == Age.Age15;
                        break;
                    case "age:18":
                        predicate = x => x.GameMetadataNavigation.AdmittedAge == Age.Age18;
                        break;
                    case "finish:true":
                        predicate = x => x.GameMetadataNavigation.IsFinished == true;
                        break;
                    case "finish:false":
                        predicate = x => x.GameMetadataNavigation.IsFinished == false;
                        break;
                    case "favorite:true":
                        predicate = x => x.IsFavorite == true;
                        break;
                    case "favorite:false":
                        predicate = x => x.IsFavorite == false;
                        break;
                    case "score:0-1":
                        predicate = x => (x.GameMetadataNavigation.Score >= 0) && (x.GameMetadataNavigation.Score < 1);
                        break;
                    case "score:1-2":
                        predicate = x => (x.GameMetadataNavigation.Score >= 1) && (x.GameMetadataNavigation.Score < 2);
                        break;
                    case "score:2-3":
                        predicate = x => (x.GameMetadataNavigation.Score >= 2) && (x.GameMetadataNavigation.Score < 3);
                        break;
                    case "score:3-4":
                        predicate = x => (x.GameMetadataNavigation.Score >= 3) && (x.GameMetadataNavigation.Score < 4);
                        break;
                    case "score:4-5":
                        predicate = x => (x.GameMetadataNavigation.Score >= 4) && (x.GameMetadataNavigation.Score < 5);
                        break;
                    case "status:need_processing":
                        predicate = x => x.PackageStatus == PackageStatus.NeedProcessing;
                        break;
                    case "status:compressed":
                        predicate = x => x.PackageStatus == PackageStatus.Compressed;
                        break;
                    case "status:file_not_found":
                        predicate = x => x.PackageStatus == PackageStatus.FileNotFound;
                        break;
                    case "status:data_not_found":
                        predicate = x => x.PackageStatus == PackageStatus.DataNotFound;
                        break;
                    case "status:invalid_package":
                        predicate = x => x.PackageStatus == PackageStatus.InvalidPackage;
                        break;
                    case "status:exception":
                        predicate = x => x.PackageStatus == PackageStatus.Exception;
                        break;
                    case "status:playable":
                        predicate = x => x.PackageStatus == PackageStatus.Playable;
                        break;
                    case "type:single":
                        predicate = x => x.PackageMetadataNavigation.Type == PackageType.Single;
                        break;
                    case "type:chunk":
                        predicate = x => x.PackageMetadataNavigation.Type == PackageType.Chunk;
                        break;
                }

                if (_method == FilteringMethod.And)
                {
                    var targets = result.FindAll(predicate).ToList();
                    result = targets;
                }
                else if (_method == FilteringMethod.Or)
                {
                    var targets = games.ToList().FindAll(predicate);
                    result.AddRange(targets);
                }
                else if (_method == FilteringMethod.Not)
                {
                    result.RemoveAll(predicate);
                }
            }

            if (_method == FilteringMethod.And)
            {
                return result;
            }
            else if (_method == FilteringMethod.Or)
            {
                return result.Distinct().ToList();
            }
            else if (_method == FilteringMethod.Not)
            {
                return result;
            }
            else
            {
                return result;
            }
        }

        #endregion
    }
}
