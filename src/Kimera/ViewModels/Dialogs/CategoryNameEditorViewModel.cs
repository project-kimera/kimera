using Caliburn.Micro;
using Kimera.Data.Entities;

namespace Kimera.ViewModels.Dialogs
{
    public class CategoryNameEditorViewModel : Screen
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
            set
            {
                Set(ref _selectedCategory, value);
                Text = value?.Name;
            }
        }

        private string _text = string.Empty;

        public string Text
        {
            get => _text;
            set => Set(ref _text, value);
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
