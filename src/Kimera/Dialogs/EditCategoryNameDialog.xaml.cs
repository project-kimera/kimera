using Kimera.ViewModels;
using System.Windows;

namespace Kimera.Dialogs
{
    /// <summary>
    /// EditItemDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditCategoryNameDialog : Window
    {
        public EditCategoryNameViewModel ViewModel { get; set; } = new EditCategoryNameViewModel();

        public EditCategoryNameDialog()
        {
            InitializeComponent();
            this.DataContext = ViewModel;
        }
    }
}
