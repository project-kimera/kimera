using Caliburn.Micro;
using Kimera.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.ViewModels.Dialogs
{
    public class CategorySelectorViewModel : Screen
    {
        private string _title = string.Empty;

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _caption = string.Empty;

        public string Caption
        {
            get => _caption;
            set => Set(ref _caption, value);
        }

        private BindableCollection<Category> _categories = new BindableCollection<Category>();

        public BindableCollection<Category> Categories
        {
            get => _categories;
            set => Set(ref _categories, value);
        }

        private Category _selectedCategory;

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set => Set(ref _selectedCategory, value);
        }

        public async void Cancel()
        {
            await TryCloseAsync(false);
        }

        public async void Confirm()
        {
            await TryCloseAsync(true);
        }
    }
}
