using Kimera.ViewModels;
using System.Windows;

namespace Kimera.Dialogs
{
    /// <summary>
    /// EditStringDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditStringDialog : Window
    {
        public EditStringViewModel ViewModel { get; set; } = new EditStringViewModel();

        public EditStringDialog()
        {
            InitializeComponent();
            this.DataContext = ViewModel;
        }
    }
}
