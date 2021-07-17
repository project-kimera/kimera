using Kimera.Helpers;
using Kimera.Pages;
using Kimera.ViewModels;
using System.Windows;
using System.Windows.Forms;

namespace Kimera
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainViewModel viewModel = new MainViewModel(ContentFrame);
            this.DataContext = viewModel;
        }
    }
}
