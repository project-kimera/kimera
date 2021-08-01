using Kimera.Data.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Kimera.Dialogs
{
    /// <summary>
    /// EditItemDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditCategoryNameDialog : Window
    {
        public string Caption
        {
            set
            {
                CaptionTextBlock.Text = value;
            }
        }

        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();

        public ObservableCollection<Category> Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
                CategoriesComboBox.ItemsSource = value;
            }
        }

        public string SelectedCategoryName
        {
            get
            {
                return CategoriesComboBox.Text;
            }
            set
            {
                CategoriesComboBox.Text = value;
                NameTextBox.Text = value;
            }
        }

        public string ChangedCategoryName
        {
            get
            {
                return NameTextBox.Text;
            }
            set
            {
                NameTextBox.Text = value;
            }
        }

        public EditCategoryNameDialog()
        {
            InitializeComponent();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NameTextBox.Text = CategoriesComboBox.Text;
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OnConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
