using Kimera.Data.Entities;
using Kimera.ViewModels;
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
        public EditCategoryNameViewModel ViewModel { get; set; } = new EditCategoryNameViewModel();

        public EditCategoryNameDialog()
        {
            InitializeComponent();
            this.DataContext = ViewModel;
        }
    }
}
