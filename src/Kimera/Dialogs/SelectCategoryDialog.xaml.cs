using Kimera.ViewModels;
using System.Windows;

namespace Kimera.Dialogs
{
    /// <summary>
    /// SelectCategoryDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SelectCategoryDialog : Window
    {
        public SelectCategoryViewModel ViewModel { get; set; } = new SelectCategoryViewModel();

        public SelectCategoryDialog()
        {
            InitializeComponent();
            this.DataContext = ViewModel;
        }
    }
}
