using Caliburn.Micro;
using Kimera.Entities;
using Kimera.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.ViewModels.Dialogs
{
    public class SortingEditorViewModel : Screen
    {
        private LibraryService _libraryService = IoC.Get<LibraryService>();

        private SortingService _sortingService = IoC.Get<SortingService>();

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

        public SortingEditorViewModel()
        {
            InitializeSettings();
        }

        public void InitializeSettings()
        {
            _criterion = _sortingService.Criterion;
            _order = _sortingService.Order;
        }

        public async void Confirm()
        {
            _sortingService.Criterion = _criterion;
            _sortingService.Order = _order;

            // Refresh
            if (_libraryService.SelectedCategoryGuid != Guid.Empty)
            {
                await _libraryService.UpdateGamesAsync(_libraryService.SelectedCategoryGuid).ConfigureAwait(false);
            }
            else
            {
                await _libraryService.ShowAllGamesAsync();
            }

            await TryCloseAsync(true);
        }
    }
}
