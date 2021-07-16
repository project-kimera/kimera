using Kimera.Helpers;
using Kimera.Pages;
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
            var navigationHelper = NavigationHelper.Instance;
            navigationHelper.InitializeFrame(ContentFrame);
            navigationHelper.NavigateTo(NavigationHelper.LibraryPage);
        }

        private void OnLibraryClick(object sender, RoutedEventArgs e)
        {
            var navigationHelper = NavigationHelper.Instance;
            navigationHelper.NavigateTo(NavigationHelper.LibraryPage);
        }

        private void OnSettingsClick(object sender, RoutedEventArgs e)
        {
            var navigationHelper = NavigationHelper.Instance;
            navigationHelper.NavigateTo(NavigationHelper.SettingsPage);
        }
    }
}
