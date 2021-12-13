using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Services
{
    public class SortingService : PropertyChangedBase
    {
        #region ::Variables::

        private SortingCriteria _criterion = SortingCriteria.Title;

        public SortingCriteria Criterion
        {
            get => _criterion;
            set => Set(ref _criterion, value);
        }

        private SortingOrder _order = SortingOrder.Ascending;

        public SortingOrder Order
        {
            get => _order;
            set => Set(ref _order, value);
        }

        #endregion

        #region ::Constructors::

        public SortingService()
        {
            InitializeService();
        }

        private async void InitializeService()
        {
            Log.Information("The sorting service has been initialized.");
        }

        #endregion

        #region ::Functions::

        public List<Game> SortGames(IEnumerable<Game> games)
        {
            List<Game> result = null;

            Func<Game, object> criterionSelector = null;
            
            switch (_criterion)
            {
                case SortingCriteria.Title:
                    criterionSelector = x => x.GameMetadataNavigation.Name;
                    break;
                case SortingCriteria.Creator:
                    criterionSelector = x => x.GameMetadataNavigation.Creator;
                    break;
                case SortingCriteria.Score:
                    criterionSelector = x => x.GameMetadataNavigation.Score;
                    break;
                case SortingCriteria.PlayTime:
                    criterionSelector = x => x.GameMetadataNavigation.PlayTime;
                    break;
                case SortingCriteria.FirstTime:
                    criterionSelector = x => x.GameMetadataNavigation.FirstTime;
                    break;
                case SortingCriteria.LastTime:
                    criterionSelector = x => x.GameMetadataNavigation.LastTime;
                    break;
            }

            if (_order == SortingOrder.Ascending)
            {
                result = games.OrderBy(criterionSelector).ToList();
            }
            else
            {
                result = games.OrderByDescending(criterionSelector).ToList();
            }

            return result;
        }

        #endregion
    }
}
