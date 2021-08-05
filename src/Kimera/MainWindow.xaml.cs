using Kimera.Utilities;
using Kimera.Pages;
using Kimera.ViewModels;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System;

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

        private void OnNavigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0;
            animation.To = 1;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            ContentFrame.BeginAnimation(OpacityProperty, animation);
        }
    }
}
